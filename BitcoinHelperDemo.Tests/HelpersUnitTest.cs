using BitcoinHelperDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace BitcoinHelperDemo.Tests
{
    public class HelpersUnitTest
    {
        // Data for 2020-03-01 to 2021-08-01, over 90 days
        readonly string filename1 = "../../../test-file1.json";
        // Data for 2020-01-19 to 2020-01-21, under 90 days
        readonly string filename2 = "../../../test-file2.json";
        // Data example from where range dates are same. So from hour is 00 and to hour is 01 on same day.
        readonly string filename8 = "../../../test-file8.json";

        private CryptoApiDataClass ReadJsonFile(string filename)
        {
            string json = File.ReadAllText(filename);
            CryptoApiDataClass data = JsonSerializer.Deserialize<CryptoApiDataClass>(json) ?? throw new ArgumentException();
            return data;
        }

        private Helpers CreateDefaultHelpers()
        {
            return new Helpers();
        }

        [Fact]
        public void HourDataPointsToDays_Over90Days_ShouldReturnInput()
        {
            CryptoApiDataClass data = ReadJsonFile(filename1);

            var helpers = CreateDefaultHelpers();
            var inputData = data.prices;
            var unmodifiedData = helpers.HourDataPointsToDays(inputData);

            Assert.Equal(inputData, unmodifiedData);
        }

        [Fact]
        public void HourDataPointsToDays_SingleDayData_ShouldReturnCorrect()
        {
            CryptoApiDataClass data = ReadJsonFile(filename8);
            List<List<double>> correctList = new List<List<double>>
            {
              new List<double>{ 1577751007981, 20339632487.157036 }
            };

            var helpers = CreateDefaultHelpers();
            var inputData = data.total_volumes;
            var res = helpers.HourDataPointsToDays(inputData);

            Assert.Equal(correctList, res);
        }

        [Fact]
        public void HourDataPointsToDays_TestFile2_ShouldReturn_CorrectList()
        {
            List<List<double>> correctList = new List<List<double>>
            {
              new List<double>{ 1579392282472, 8030.890983244613 },
              new List<double>{ 1579478664166, 7833.230953209291 },
              new List<double>{ 1579565263674, 7778.216161699133 }
            };

            CryptoApiDataClass data = ReadJsonFile(filename2);

            var helpers = CreateDefaultHelpers();
            var inputData = data.prices;
            var modifiedData = helpers.HourDataPointsToDays(inputData);

            Assert.Equal(correctList, modifiedData);
        }

        [Fact]
        public void CloserToMidnight_FirstCloser_ShouldReturnFirst()
        {
            DateTimeOffset first = DateTimeOffset.FromUnixTimeMilliseconds(1575589109000);
            DateTimeOffset second = DateTimeOffset.FromUnixTimeMilliseconds(1575592709000);

            var helpers = CreateDefaultHelpers();

            var res = helpers.CloserToMidnight(first, second);

            Assert.Equal(0, res);
        }

        [Fact]
        public void CloserToMidnight_SecondCloser_ShouldReturnSecond()
        {
            DateTimeOffset first = DateTimeOffset.FromUnixTimeMilliseconds(1575587429000);
            DateTimeOffset second = DateTimeOffset.FromUnixTimeMilliseconds(1575591029000);

            var helpers = CreateDefaultHelpers();

            var res = helpers.CloserToMidnight(first, second);

            Assert.Equal(1, res);
        }

        [Fact]
        public void CloserToMidnight_MinutesAndSecondsEqual_ShouldReturnFirst()
        {
            DateTimeOffset first = DateTimeOffset.FromUnixTimeMilliseconds(1575588600000);
            DateTimeOffset second = DateTimeOffset.FromUnixTimeMilliseconds(1575592200000);

            var helpers = CreateDefaultHelpers();

            var res = helpers.CloserToMidnight(first, second);

            Assert.Equal(0, res);
        }

        [Fact]
        public void ValidateRange_ValidRange_ReturnTrue()
        {
            var helpers = CreateDefaultHelpers();
            bool res = helpers.ValidateRange(2020, 1, 12, 2020, 4, 4);

            Assert.True(res);
        }

        [Fact]
        public void ValidateRange_ToDateBeforeFromDate_ReturnFalse()
        {
            var helpers = CreateDefaultHelpers();
            bool res = helpers.ValidateRange(2020, 1, 12, 2019, 4, 4);

            Assert.False(res);
        }

        [Fact]
        public void ValidateRange_InvalidValuesForDate_ReturnFalse()
        {
            var helpers = CreateDefaultHelpers();
            bool res = helpers.ValidateRange(2019, 154, 24, 2020, 3, 5);

            Assert.False(res);
        }

        [Fact]
        public void ValidateRange_YearWayTooEarly_ReturnFalse()
        {
            var helpers = CreateDefaultHelpers();
            bool res = helpers.ValidateRange(1600, 1, 24, 2020, 3, 5);

            Assert.False(res);
        }

        [Fact]
        public void ConstructBitcoinPath_ReturnCorrect()
        {
            var helpers = CreateDefaultHelpers();
            string correct = "https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from=1578780000&to=1598780000";
            var res = helpers.ConstructBitcoinPath(1578780000, 1598780000);
            
            Assert.Equal(correct, res);
        }

        [Fact]
        public void TimestampToDate_InputUnixSeconds_ReturnCorrect()
        {
            var helpers = CreateDefaultHelpers();
            var correct = new DateTime(2013, 04, 28, 0, 0, 0, DateTimeKind.Utc);

            var res = helpers.TimestampToDate(1367107200);
            Assert.Equal(correct, res);
        }

        [Fact]
        public void TimestampToDate_InputUnixMilliseconds_ReturnCorrect()
        {
            var helpers = CreateDefaultHelpers();
            var correct = new DateTime(2013, 04, 28, 0, 0, 0, DateTimeKind.Utc);

            var res = helpers.TimestampToDate(1367107200000);
            Assert.Equal(correct, res);
        }

        [Fact]
        public void AddTimesAndSerialize_TrendObject1_ReturnCorrect()
        {
            var helpers = CreateDefaultHelpers();
            string correct = "{\"ConsecutiveDecreaseDays\":1,\"Text\":null,\"From\":\"2020-01-11T22:00:00+00:00\",\"To\":\"2020-08-30T09:33:20+00:00\"}";
            DownwardTrendObject trendObject = new DownwardTrendObject();
            trendObject.ConsecutiveDecreaseDays = 1;
            var res = helpers.AddTimesAndSerialize(trendObject, 1578780000, 1598780000);
            Assert.Equal(correct, res);
        }

        [Fact]
        public void AddTimesAndSerialize_TrendObject2_ReturnCorrect()
        {
            var helpers = CreateDefaultHelpers();
            string correct = "{\"ConsecutiveDecreaseDays\":0,\"Text\":\"No price decrease in given range.\",\"From\":\"2020-01-11T22:00:00+00:00\",\"To\":\"2020-08-30T09:33:20+00:00\"}";
            DownwardTrendObject trendObject = new DownwardTrendObject();
            trendObject.ConsecutiveDecreaseDays = 0;
            trendObject.Text = "No price decrease in given range.";
            var res = helpers.AddTimesAndSerialize(trendObject, 1578780000, 1598780000);
            Assert.Equal(correct, res);
        }
    }
}
