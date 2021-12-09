using BitcoinHelperDemo.Models;

namespace BitcoinHelperDemo
{
    public class InformationHandler
    {
        private Helpers _helpers = new Helpers();

        // Returns the longest streak for which the value declined.
        public int DownwardTrendCalc(List<List<double>> data)
        {
            var modifiedData = _helpers.HourDataPointsToDays(data);

            int downwardCounter = 0;
            int longestDownwardStreak = 0;
            for(int i=0; i< modifiedData.Count-1; i++)
            {
                var currentDayValue = modifiedData[i][1];
                var nextDayValue = modifiedData[i+1][1];

                if (nextDayValue < currentDayValue)
                {
                    downwardCounter++;
                    if (downwardCounter > longestDownwardStreak) longestDownwardStreak = downwardCounter;
                }
                else
                {
                    downwardCounter = 0;
                }

                if (modifiedData[i + 1] == modifiedData.Last()) break;
            }
            return longestDownwardStreak;
        }

        // Loops through the data and returns the largest datapoint value with its given timestamp.
        public List<double> HighestValueCalc(List<List<double>> data)
        {
            var modifiedData = _helpers.HourDataPointsToDays(data);

            var highest = new List<double>();

            for(int i=0; i< modifiedData.Count; i++)
            {
                var currentDay = modifiedData[i];

                if(i == 0) highest = currentDay;
                else if (currentDay[1] >= highest[1]) highest = currentDay;
            }
            return highest;
        }

        // Returns a pair of items with the highest positive difference in prices chronologically, pair with
        // smallest negative difference if prices only decrease, or pair with no difference if prices are equal in given range.
        public List<List<double>>? MaximumChronologicalDifferenceCalc(List<List<double>> data)
        {
            var modifiedData = _helpers.HourDataPointsToDays(data);
            if (modifiedData.Count < 2)
            {
                return null;
            }

            List<double> min = modifiedData[0];
            List<double> max = modifiedData[1];
            List<double> tempMin = modifiedData[0];

            // Loops through data, saving appropriate date and price pairs if the difference grows larger in a positive manner.
            for(int i=1; i<modifiedData.Count; i++)
            {
                double largestDifference = max[1] - min[1];
                double currentDifference = modifiedData[i][1] - tempMin[1];
                if (largestDifference < currentDifference)
                {
                    min = tempMin;
                    max = modifiedData[i];
                }
                if(modifiedData[i][1] < tempMin[1])
                {
                    tempMin = modifiedData[i];
                }
            }
            return new List<List<double>>{ min, max };
        }

        public TimeMachineObject BuildTimeMachineObject(CryptoApiDataClass data)
        {
            TimeMachineObject timemachineObject = new TimeMachineObject();
            var timemachineData = MaximumChronologicalDifferenceCalc(data.prices);
            if(timemachineData == null)
            {
                timemachineObject.Text = "No profit to be made with given range.";
            }
            else if ((timemachineData[1][1] - timemachineData[0][1]) <= 0) timemachineObject.Text = "No profit to be made with given range.";
            else
            {
                timemachineObject.BuyDate = _helpers.TimestampToDate(timemachineData[0][0]);
                timemachineObject.SellDate = _helpers.TimestampToDate(timemachineData[1][0]);
            }
            return timemachineObject;
        }

        public DownwardTrendObject BuildDownwardTrendObject(CryptoApiDataClass data)
        {
            DownwardTrendObject downwardTrendObject = new DownwardTrendObject();
            int downwardtrendData = DownwardTrendCalc(data.prices);
            if (downwardtrendData <= 0) downwardTrendObject.Text = "No price decrease in given range.";
            else
            {
                downwardTrendObject.ConsecutiveDecreaseDays = downwardtrendData;
            }
            return downwardTrendObject;
        }

        public HighestTradingDayObject BuildHighestTradingDayObject(CryptoApiDataClass data)
        {
            HighestTradingDayObject highTradingObject = new HighestTradingDayObject();
            var highesttradingdayData = HighestValueCalc(data.total_volumes);
            highTradingObject.Day = _helpers.TimestampToDate(highesttradingdayData[0]);
            highTradingObject.TradingVolume = highesttradingdayData[1];
            return highTradingObject;
        }
    }
}
