using System;

namespace BitcoinHelperDemo
{
    public class Helpers
    {
        // Used when CryptoApi response includes hours prices instead of only day prices.
        // Returns a compressed object where the closest data points to the midnight UTC are added.
        // For accurate calculation, make sure to add an extra hour in the CryptoApi request, so that the last datapoint is after the last days midnight.
        public List<List<double>> HourDataPointsToDays(List<List<double>> data)
        {
            var compressedData = new List<List<double>>();

            for(int i=0; i<data.Count-1; i++)
            {
                // Add the first datapoint automatically as it is closest to midnight of the first day
                if (i == 0)
                {
                    compressedData.Add(data[i]);
                }
                else
                {
                    DateTimeOffset dateItem = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(data[i][0]));
                    DateTimeOffset dateItemNext = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(data[i + 1][0]));
                    
                    // If difference between two datapoints is large, the data is daily data and does not need modification
                    if ((dateItemNext - dateItem).TotalHours > 22) return data;

                    // Check if current datapoint is before midnight, and next is after. Then check which is closer to midnight and add to list.
                    if(dateItem.Day != dateItemNext.Day)
                    {
                        int res = CloserToMidnight(dateItem, dateItemNext);
                        if (res == 0) compressedData.Add(data[i]);
                        else if (res == 1) compressedData.Add(data[i + 1]);
                        else if (res == -1) throw new Exception();
                        else throw new Exception();

                        if (data[i + 1] == data.Last()) break;
                    } 
                }
            }

            return compressedData;
        }

        // Compares two DateTimeOffset items and returns 0 if first is closer, and 1 if second is closer.
        // Assumes that days are consecutive.
        // Essentially calculates the distance the two dates have to midnight.
        public int CloserToMidnight(DateTimeOffset first, DateTimeOffset second)
        {
            var dt1temp = new DateTime(first.Year, first.Month, first.Day, 00, 00, 00);
            var dt2temp = new DateTime(second.Year, second.Month, second.Day, 00, 00, 00);

            TimeSpan time1 = new TimeSpan();
            TimeSpan time2 = new TimeSpan();
            TimeSpan time24 = new TimeSpan(24, 0, 0);

            time1 = first.DateTime - dt1temp;
            time2 = second.DateTime - dt2temp;

            if (time1.Hours >= 12) time1 = time24 - time1;
            if (time2.Hours >= 12) time2 = time24 - time2;

            if (time1 < time2 || time1 == time2) return 0;
            else if (time1 > time2) return 1;

            return -1;
        }
    }
}
