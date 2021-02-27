using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RELATIVE VIGOR INDEX
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<RviResult> GetRvi<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod = 10)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateRvi(history, lookbackPeriod);

            // initialize
            int size = historyList.Count;
            List<RviResult> results = new List<RviResult>(size);

            // roll through history
            List<decimal> numerators = new List<decimal>(), denominators = new List<decimal>();
            for (int i = 0; i < size; i++)
            {
                RviResult result = new RviResult
                {
                    Date = historyList[i].Date
                };

                decimal numerator, denominator;
                if (i >= 3)
                {
                    decimal a = historyList[i].Close - historyList[i].Open;
                    decimal b = historyList[i - 1].Close - historyList[i - 1].Open;
                    decimal c = historyList[i - 2].Close - historyList[i - 2].Open;
                    decimal d = historyList[i - 3].Close - historyList[i - 3].Open;
                    numerator = (a + (2 * b) + (2 * c) + d) / 6;
                    decimal e = historyList[i].High - historyList[i].Low;
                    decimal f = historyList[i - 1].High - historyList[i - 1].Low;
                    decimal g = historyList[i - 2].High - historyList[i - 2].Low;
                    decimal h = historyList[i - 3].High - historyList[i - 3].Low;
                    denominator = (e + (2 * f) + (2 * g) + h) / 6;
                }
                else
                {
                    numerator = denominator = 0;
                }
                numerators.Add(numerator);
                denominators.Add(denominator);

                if (i >= lookbackPeriod)
                {
                    decimal sumNumeratorSma = 0m, sumDenominatorSma = 0m;
                    for (int p = i + 1 - lookbackPeriod; p <= i; p++)
                    {
                        sumNumeratorSma += numerators[p];
                        sumDenominatorSma += denominators[p];
                    }
                    result.Rvi = sumNumeratorSma / sumDenominatorSma;
                }
                else
                {
                    result.Rvi = 0;
                }
                results.Add(result);

                result.Signal = i >= 3 ? (results[i].Rvi + (2 * results[i - 1].Rvi) + (2 * results[i - 2].Rvi) + results[i - 3].Rvi) / 6 : 0;
            }

            return results;
        }

        private static void ValidateRvi<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for Stochastic.");
            }

            // check history
            int qtyHistory = history.Count();
            if (qtyHistory < lookbackPeriod)
            {
                string message = "Insufficient history provided for Stochastic.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, lookbackPeriod);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
