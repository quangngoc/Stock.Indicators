using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class RviResult : ResultBase
    {
        public decimal? Rvi { get; set; }

        public decimal? Signal { get; set; }

        public decimal Width => Rvi == null || Signal == null ? 0 : (Rvi.Value - Signal.Value);
    }
}
