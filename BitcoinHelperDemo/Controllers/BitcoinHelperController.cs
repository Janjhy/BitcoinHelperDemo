using Microsoft.AspNetCore.Mvc;
using BitcoinHelperDemo.Models;
using System;

namespace BitcoinHelperDemo.Controllers
{
    // Controller to handle requests.
    // For bitcoin, the earliest datapoint returned by Crypto Api is 2013-5-13.
    [ApiController]
    [Route("[controller]")]
    public class BitcoinHelperController : ControllerBase
    {
        private Helpers _helpers = new Helpers();
        private CryptoApiService _cryptoApiService = new CryptoApiService();
        private InformationHandler _informationHandler = new InformationHandler();

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return "Request route is BitcoinHelper/{param}/{YYYY}/{mm}/{dd}/to/{YYYY}/{mm}/{dd}. " +
                " Available param values are  timemachine, downwardtrend and tradinghigh.";
        }

        // GET: BitcoinHelper/{param}/{YYYY}/{mm}/{dd}/to/{YYYY}/{mm}/{dd}
        [HttpGet]
        [Route("{param}/{fromYear:int}/{fromMonth:int}/{fromDay:int}/to/{toYear:int}/{toMonth:int}/{toDay:int}")]
        public async Task<ActionResult<string>> GetRangeData(string param, int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay)
        {
            bool isValid = _helpers.ValidateRange(fromYear, fromMonth, fromDay, toYear, toMonth, toDay);
            if (!isValid) return BadRequest("Invalid date range.");
            DateTime fromDate;
            DateTime toDate;

            try
            {
                fromDate = new DateTime(fromYear, fromMonth, fromDay, 0, 0, 0, DateTimeKind.Utc);
                toDate = new DateTime(toYear, toMonth, toDay, 0, 0, 0, DateTimeKind.Utc);
            }
            catch (Exception ex)
            {
                return BadRequest("Error in constructing request.");
            }

            // Convert valid dates to timestamps for Crypto Api
            long fromTimestamp = new DateTimeOffset(fromDate).ToUnixTimeSeconds();
            long toTimestamp = new DateTimeOffset(toDate).ToUnixTimeSeconds();
            if (fromTimestamp == 0 || toTimestamp == 0) return BadRequest("Error in constructing request.");

            // Add an hour to the toTimestamp by adding 3600 seconds
            string path = _helpers.ConstructBitcoinPath(fromTimestamp, (toTimestamp + 3600));
            CryptoApiDataClass data = await _cryptoApiService.GetCryptoRangeData(path);
            
            if(data == null)
            {
                return NotFound();
            }
            if (data.prices == null || data.total_volumes == null || data.market_caps == null)
            {
                return NotFound();
            }

            string jsonString = "";
            // If the request date was from before first available datapoint from api,
            // then use the timestamp of first datapoint when calling AddTimesAndSerialize().
            switch (param)
            {
                case "timemachine":
                    TimeMachineObject timemachineObject = _informationHandler.BuildTimeMachineObject(data);
                    jsonString = _helpers.AddTimesAndSerialize(timemachineObject, fromTimestamp, toTimestamp);
                    break;
                case "downwardtrend":
                    DownwardTrendObject trendObject = _informationHandler.BuildDownwardTrendObject(data);
                    jsonString = _helpers.AddTimesAndSerialize(trendObject, fromTimestamp, toTimestamp);
                    break;
                case "tradinghigh":
                    HighestTradingDayObject highTradingObject = _informationHandler.BuildHighestTradingDayObject(data);
                    jsonString = _helpers.AddTimesAndSerialize(highTradingObject, fromTimestamp, toTimestamp);
                    break;
                default:
                    break;
            }
            return jsonString;
        }
    }
}