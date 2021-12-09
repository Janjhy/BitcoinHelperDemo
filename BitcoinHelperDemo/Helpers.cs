using BitcoinHelperDemo.Models;
using System.Text.Json;

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

            // Add the first datapoint automatically as it is closest to midnight of the first day
            compressedData.Add(data[0]); ;
            for (int i=0; i<data.Count-1; i++)
            {
                if (i == 0)
                {
                    continue;
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
            DateTime dt1temp = new DateTime(first.Year, first.Month, first.Day, 00, 00, 00);
            DateTime dt2temp = new DateTime(second.Year, second.Month, second.Day, 00, 00, 00);

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

        public bool ValidateRange(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay)
        {
            var from = new DateTime();
            var to = new DateTime();
            // Check if entered values are valid to form a date.
            try
            {
                from = new DateTime(fromYear, fromMonth, fromDay);
                to = new DateTime(toYear, toMonth, toDay);
            }
            catch (Exception)
            {
                return false;
            }
            // If date is before Unix Epoch return false. 
            var fromTimestamp = new DateTimeOffset(from).ToUnixTimeSeconds();
            if(fromTimestamp < 0)
            {
                return false;
            }
            // Check if from date after before to date.
            if (from > to)
            {
                return false;
            }
            return true;
        }

        public string ConstructBitcoinPath(long from, long to)
        {
            string basePath = "https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&";
            return $"{basePath}from={from}&to={to}";
        }

        public DateTimeOffset? TimestampToDate(double timestamp)
        {
            try
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp));
                return date;
            }
            catch(Exception)
            {
                try
                {
                    var date = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(timestamp));
                    return date;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string AddTimesAndSerialize(BaseObject dataObject, double fromTimestamp, double toTimestamp) 
        {
            dataObject.From = TimestampToDate(fromTimestamp);
            dataObject.To = TimestampToDate(toTimestamp);
            string jsonString = JsonSerializer.Serialize<object>(dataObject);
            return jsonString;
        }
    }
}
