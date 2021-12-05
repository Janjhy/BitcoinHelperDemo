using Xunit;
using BitcoinHelperDemo;
using System;
using System.IO;
using System.Text.Json;



namespace BitcoinHelperDemo.Tests
{
    public class InformationHandlerUnitTest
    {
        
        // Data for 2020-03-01 to 2020-08-01, over 90 days
        string filename1 = "../../../test-file1.json";
        // Data for 2020-01-19 to 2020-01-21, under 90 days
        string filename2 = "../../../test-file2.json";

        public CryptoApiDataClass ReadJsonFile(string filename) {
            string json = File.ReadAllText(filename1);
            CryptoApiDataClass data = JsonSerializer.Deserialize<CryptoApiDataClass>(json) ?? throw new ArgumentException();
            return data;
        }

        private InformationHandler CreateDefaultInformationHandler()
        {
            return new InformationHandler();
        }
        
        [Fact]
        public void DownwardTrendCalc_Under90Days_Return2()
        {
            CryptoApiDataClass data = ReadJsonFile(filename2);
            var infoHandler = CreateDefaultInformationHandler();
            int result = infoHandler.DownwardTrendCalc(data);
            Assert.Equal(2, result);
        }
    }
}