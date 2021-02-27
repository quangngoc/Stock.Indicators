using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StochResult : ResultBase
    {
        public decimal? Oscillator { get; set; }
        public decimal? Signal { get; set; }
        public decimal? PercentJ { get; set; }
        public bool? IsOverbought => Signal == null ? (bool?)null : Signal.Value > 80;
        public bool? IsOversold => Signal == null ? (bool?)null : Signal.Value < 20;
    }
}
