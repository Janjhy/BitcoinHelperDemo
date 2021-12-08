using Xunit;
using BitcoinHelperDemo;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using BitcoinHelperDemo.Models;

namespace BitcoinHelperDemo.Tests
{
    public class InformationHandlerUnitTest
    {
        
        // Data for 2020-03-01 to 2021-08-01, over 90 days
        readonly string filename1 = "../../../test-file1.json";
        // Data for 2020-01-19 to 2020-01-21, under 90 days
        readonly string filename2 = "../../../test-file2.json";
        // Data for 2021-12-02 to 2021-12-02, 1 day
        readonly string filename3 = "../../../test-file3.json";
        // Data where price only rises consecutively
        readonly string filename4 = "../../../test-file4.json";
        // Data which has a bitcoin profit to be made
        readonly string filename5 = "../../../test-file5.json";
        // Data where price only decreases
        readonly string filename6 = "../../../test-file6.json";
        // Data where prices are equal
        readonly string filename7 = "../../../test-file7.json";
        // Data example from where range dates are same. So from hour is 00 and to hour is 01 on same day.
        readonly string filename8 = "../../../test-file8.json";

        private CryptoApiDataClass ReadJsonFile(string filename) {
            string json = File.ReadAllText(filename);
            CryptoApiDataClass data = JsonSerializer.Deserialize<CryptoApiDataClass>(json) ?? throw new ArgumentException();
            return data;
        }

        private InformationHandler CreateDefaultInformationHandler()
        {
            return new InformationHandler();
        }
        
        [Fact]
        public void DownwardTrendCalc_2DownwardDays_Return2()
        {
            CryptoApiDataClass data = ReadJsonFile(filename2);
            var infoHandler = CreateDefaultInformationHandler();
            int result = infoHandler.DownwardTrendCalc(data.prices);
            Assert.Equal(2, result);
        }

        [Fact]
        public void DownwardTrendCalc_8DownwardDays_Return8()
        {
            CryptoApiDataClass data = ReadJsonFile(filename1);
            var infoHandler = CreateDefaultInformationHandler();
            int result = infoHandler.DownwardTrendCalc(data.prices);
            Assert.Equal(8, result);
        }

        [Fact]
        public void DownwardTrendCalc_1DayData_Return0()
        {
            CryptoApiDataClass data = ReadJsonFile(filename3);
            var infoHandler = CreateDefaultInformationHandler();
            int result = infoHandler.DownwardTrendCalc(data.prices);
            Assert.Equal(0, result);
        }

        [Fact]
        public void DownwardTrendCalc_PricesGrows_Return0()
        {
            CryptoApiDataClass data = ReadJsonFile(filename4);
            var infoHandler = CreateDefaultInformationHandler();
            int result = infoHandler.DownwardTrendCalc(data.prices);
            Assert.Equal(0, result);
        }

        [Fact]
        public void HighestValueCalc_OneHighest_ReturnCorrect()
        {
            var correct = new List<double> { 1627344000000, 45361497210.93923 };
            CryptoApiDataClass data = ReadJsonFile(filename1);
            var infoHandler = CreateDefaultInformationHandler();
            List<double> result = infoHandler.HighestValueCalc(data.total_volumes);
            Assert.Equal(correct, result);
        }

        [Fact]
        public void HighestValueCalc_MultipleHighest_ReturnNewest()
        {
            var correct = new List<double> { 1579565263674, 35371750848.77523 };
            CryptoApiDataClass data = ReadJsonFile(filename4);
            var infoHandler = CreateDefaultInformationHandler();
            List<double> result = infoHandler.HighestValueCalc(data.total_volumes);
            Assert.Equal(correct, result);
        }

        [Fact]
        public void HighestValueCalc_OneDayData_ReturnCorrect()
        {
                var correct = new List<double> { 1577751007981, 20339632487.157036 };
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            List<double> result = infoHandler.HighestValueCalc(data.total_volumes);
            Assert.Equal(correct, result);
        }

        [Fact]
        public void MaximumChronologicalDifferenceCalc_ReturnCorrectPair()
        {
            var correct = new List<List<double>> {
            new List<double> { 1598745600000, 7643.032509322002 },
            new List<double> { 1599004800000, 9974.610365860795 }
            };
            CryptoApiDataClass data = ReadJsonFile(filename5);
            var infoHandler = CreateDefaultInformationHandler();

            List<List<double>> result = infoHandler.MaximumChronologicalDifferenceCalc(data.prices);

            Assert.Equal(correct, result);
        }

        [Fact]
        public void MaximumChronologicalDifferenceCalc_PricesOnlyDecrease_ReturnNegativeDifference()
        {
            CryptoApiDataClass data = ReadJsonFile(filename6);
            var infoHandler = CreateDefaultInformationHandler();

            List<List<double>> result = infoHandler.MaximumChronologicalDifferenceCalc(data.prices);
            double resultCalc = result[1][1] - result[0][1];

            Assert.True(resultCalc < 0);
        }

        [Fact]
        public void MaximumChronologicalDifferenceCalc_PricesAreEqual_ReturnZeroDifference()
        {
            CryptoApiDataClass data = ReadJsonFile(filename7);
            var infoHandler = CreateDefaultInformationHandler();

            List<List<double>> result = infoHandler.MaximumChronologicalDifferenceCalc(data.prices);
            double resultCalc = result[1][1] - result[0][1];

            Assert.True(resultCalc == 0);
        }

        [Fact]
        public void MaximumChronologicalDifferenceCalc_1DayData_ReturnNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();

            List<List<double>> result = infoHandler.MaximumChronologicalDifferenceCalc(data.prices);

            Assert.Null(result);
        }

        [Fact]
        public void BuildTimeMachineObject_EqualBuyDate_ReturnEqual()
        {
            var correct = new TimeMachineObject();
            correct.BuyDate = DateTime.Parse("30/08/2020 00:00:00 +00:00");
            CryptoApiDataClass data = ReadJsonFile(filename5);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Equal(correct.BuyDate, res.BuyDate);
        }

        [Fact]
        public void BuildTimeMachineObject_EqualSellDate_ReturnEqual()
        {
            var correct = new TimeMachineObject();
            correct.SellDate = DateTime.Parse("02/09/2020 00:00:00 +00:00");
            CryptoApiDataClass data = ReadJsonFile(filename5);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Equal(correct.SellDate, res.SellDate);
        }

        [Fact]
        public void BuildTimeMachineObject_EqualText_ReturnEqual()
        {
            var correct = new TimeMachineObject();
            CryptoApiDataClass data = ReadJsonFile(filename5);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Equal(correct.Text, res.Text);
        }

        [Fact]
        public void BuildTimeMachineObject_PricesOnlyDecrease_StringEqual()
        {
            CryptoApiDataClass data = ReadJsonFile(filename6);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            string correct = "No profit to be made with given range.";

            Assert.Equal(correct, res.Text);
        }

        [Fact]
        public void BuildTimeMachineObject_PricesOnlyDecrease_BuyDateNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename6);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Null(res.BuyDate);
        }

        [Fact]
        public void BuildTimeMachineObject_PricesOnlyDecrease_SellDateNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename6);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Null(res.SellDate);
        }

        [Fact]
        public void BuildTimeMachineObject_PricesAreEqual_StringEqual()
        {
            CryptoApiDataClass data = ReadJsonFile(filename7);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            string correct = "No profit to be made with given range.";

            Assert.Equal(correct, res.Text);
        }

        [Fact]
        public void BuildTimeMachineObject_PricesAreEqual_BuyDateNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename7);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Null(res.BuyDate);
        }

        [Fact]
        public void BuildTimeMachineObject_PricesAreEqual_SellDateNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename7);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Null(res.SellDate);
        }

        [Fact]
        public void BuildTimeMachineObject_OneDayData_StringEqual()
        {
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            string correct = "No profit to be made with given range.";

            Assert.Equal(correct, res.Text);
        }

        [Fact]
        public void BuildTimeMachineObject_OneDayData_BuyDateNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Null(res.BuyDate);
        }

        [Fact]
        public void BuildTimeMachineObject_OneDayData_SellDateNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            TimeMachineObject res = infoHandler.BuildTimeMachineObject(data);

            Assert.Null(res.SellDate);
        }

        [Fact]
        public void BuildDownwardTrendObject_2DecreaseDays_StringIsNull()
        {
            CryptoApiDataClass data = ReadJsonFile(filename2);
            var infoHandler = CreateDefaultInformationHandler();
            DownwardTrendObject res = infoHandler.BuildDownwardTrendObject(data);

            Assert.Null(res.Text);
        }

        [Fact]
        public void BuildDownwardTrendObject_2DecreaseDays_ObjectDecreaseDaysIs2()
        {
            CryptoApiDataClass data = ReadJsonFile(filename2);
            var infoHandler = CreateDefaultInformationHandler();
            DownwardTrendObject res = infoHandler.BuildDownwardTrendObject(data);

            Assert.Equal(2, res.ConsecutiveDecreaseDays);
        }

        [Fact]
        public void BuildDownwardTrendObject_PricesOnlyIncrease_ObjectDecreaseDaysIs0()
        {
            CryptoApiDataClass data = ReadJsonFile(filename4);
            var infoHandler = CreateDefaultInformationHandler();
            DownwardTrendObject res = infoHandler.BuildDownwardTrendObject(data);

            Assert.Equal(0, res.ConsecutiveDecreaseDays);
        }

        [Fact]
        public void BuildDownwardTrendObject_PricesOnlyIncrease_StringEqual()
        {
            CryptoApiDataClass data = ReadJsonFile(filename4);
            var infoHandler = CreateDefaultInformationHandler();
            DownwardTrendObject res = infoHandler.BuildDownwardTrendObject(data);

            string correct = "No price decrease in given range.";

            Assert.Equal(correct, res.Text);
        }  

        [Fact]
        public void BuildDownwardTrendObject_OneDayData_ObjectDecreaseDaysIs0()
        {
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            DownwardTrendObject res = infoHandler.BuildDownwardTrendObject(data);

            Assert.Equal(0, res.ConsecutiveDecreaseDays);
        }

        [Fact]
        public void BuildDownwardTrendObject_OneDayData_StringEqual()
        {
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            DownwardTrendObject res = infoHandler.BuildDownwardTrendObject(data);

            string correct = "No price decrease in given range.";

            Assert.Equal(correct, res.Text);
        }

        [Fact]
        public void BuildHighestTradingDayObject_CorrectVolume()
        {
            var correct = 45361497210.93923;
            CryptoApiDataClass data = ReadJsonFile(filename1);
            var infoHandler = CreateDefaultInformationHandler();
            HighestTradingDayObject res = infoHandler.BuildHighestTradingDayObject(data);

            Assert.Equal(correct, res.TradingVolume);
        }

        [Fact]
        public void BuildHighestTradingDayObject_DateEqual()
        {
            var correct = DateTime.Parse("27/07/2021 00:00:00 +00:00");
            CryptoApiDataClass data = ReadJsonFile(filename1);
            var infoHandler = CreateDefaultInformationHandler();
            HighestTradingDayObject res = infoHandler.BuildHighestTradingDayObject(data);

            Assert.Equal(correct, res.Day);
        }

        [Fact]
        public void BuildHighestTradingDayObject_OneDayData_CorrectVolume()
        {
            var correct = 20339632487.157036;
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            HighestTradingDayObject res = infoHandler.BuildHighestTradingDayObject(data);

            Assert.Equal(correct, res.TradingVolume);
        }

        [Fact]
        public void BuildHighestTradingDayObject_OneDayData_CorrectDay()
        {
            var correct = new DateTime(2019, 12, 31, 0, 10, 07, 981, DateTimeKind.Utc);
            CryptoApiDataClass data = ReadJsonFile(filename8);
            var infoHandler = CreateDefaultInformationHandler();
            HighestTradingDayObject res = infoHandler.BuildHighestTradingDayObject(data);

            Assert.Equal(correct, res.Day);
        }
    }
}