using System;

namespace BitcoinHelperDemo
{
    public class InformationHandler
    {
        private Helpers CreateDefaultHelpers()
        {
            return new Helpers();
        }

        public int DownwardTrendCalc(List<List<double>> data)
        {
            var helpers = CreateDefaultHelpers();
            var modifiedData = helpers.HourDataPointsToDays(data);

            int downwardCounter = 0;
            int longestDownwardStreak = 0;
            for(int i=0; i< modifiedData.Count-1; i++)
            {
                var currentDayPrice = modifiedData[i][1];
                var nextDayPrice = modifiedData[i+1][1];

                if (nextDayPrice < currentDayPrice)
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

            var helpers = CreateDefaultHelpers();
            var modifiedData = helpers.HourDataPointsToDays(data);

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
        public List<List<double>> MaximumChronologicalDifferenceCalc(List<List<double>> data)
        {
            var helpers = CreateDefaultHelpers();
            var modifiedData = helpers.HourDataPointsToDays(data);
            if (modifiedData.Count < 2)
            {
                return null;
            }

            List<double> min = data[0];
            List<double> max = data[1];
            List<double> tempMin = data[0];

            // Loops through data, saving appropriate date and price pairs if the difference grows larger in a positive manner.
            for(int i=1; i<modifiedData.Count; i++)
            {
                double largestDifference = max[1] - min[1];
                double currentDifference = data[i][1] - tempMin[1];
                if (largestDifference < currentDifference)
                {
                    min = tempMin;
                    max = data[i];
                }
                if(data[i][1] < tempMin[1])
                {
                    tempMin = data[i];
                }
            }
            return new List<List<double>>{ min, max };
        }
    }
}
