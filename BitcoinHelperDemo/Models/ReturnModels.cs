namespace BitcoinHelperDemo.Models
{
    public class BaseObject
    {
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }

    public class TimeMachineObject : BaseObject
    { 
        public DateTimeOffset? BuyDate { get; set; }
        public DateTimeOffset? SellDate { get; set; }
        public string? Text { get; set; }
    }

    public class DownwardTrendObject : BaseObject
    {
        public int ConsecutiveDecreaseDays { get; set; }
        public string? Text { get; set; }
    }

    public class HighestTradingDayObject : BaseObject
    {
        public DateTimeOffset? Day { get; set; }
        public double TradingVolume { get; set; }
    }
}
