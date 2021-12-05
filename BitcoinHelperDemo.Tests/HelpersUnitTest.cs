using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace BitcoinHelperDemo.Tests
{
    public class HelpersUnitTest
    {
        // Data for 2020-03-01 to 2020-08-01, over 90 days
        string filename1 = "../../../test-file1.json";
        // Data for 2020-01-19 to 2020-01-21, under 90 days
        string filename2 = "../../../test-file2.json";

        public CryptoApiDataClass ReadJsonFile(string filename)
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
        public void HourDataPointsToDays_Should_Throw_When_NotUnixTime()
        {

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
    }
}
