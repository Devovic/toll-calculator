using TollCalculation.Core.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TollCalculation.Core
{
    public class TollCalculator
    {
        private const int MaxFee = 60;
        private const int FeeIntervalMinutes = 60;
        private readonly ITollRepository _tollRepository;

        public TollCalculator(ITollRepository tollRepository)
        {
            _tollRepository = tollRepository;
        }

        public async Task<int> CalculateDailyToll(DateTime[] dates)
        {
            if (dates == null || dates.Length == 0) return 0;

            var sortedTimes = dates.OrderBy(t => t).ToArray();

            DateTime intervalStart = dates[0];
            int tempFee = await _tollRepository.GetTollFee(intervalStart);

            int totalFee = 0;
            foreach (DateTime date in dates.Skip(1))
            {
                int nextFee = await _tollRepository.GetTollFee(date);
                int minutes = GetMinutesBetween(intervalStart, date);

                if (minutes < FeeIntervalMinutes)
                {
                    tempFee = GetHighestFee(tempFee, nextFee);
                }
                else
                {
                    totalFee += tempFee;
                    intervalStart = date;
                    tempFee = nextFee;

                    if (totalFee >= MaxFee)
                        return MaxFee;
                }
            }

            totalFee += tempFee;
            return EnsureMaxFeeLimit(totalFee);
        }

        private static int GetMinutesBetween(DateTime start, DateTime end)
        {
            return (int)(end - start).TotalMinutes;
        }

        private static int GetHighestFee(int fee1, int fee2)
        {
            return fee1 > fee2 ? fee1 : fee2;
        }

        private static int EnsureMaxFeeLimit(int totalFee)
        {
            return totalFee < MaxFee ? totalFee : MaxFee;
        }
    }
}