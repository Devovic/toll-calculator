using TollCalculation.Core.Interfaces;

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

        public int CalculateDailyToll(DateTime[] dates)
        {
            if (dates == null || dates.Length == 0) return 0;

            var sortedTimes = dates.OrderBy(t => t).ToArray();

            DateTime intervalStart = dates[0];
            int tempFee = GetTollFee(intervalStart);

            int totalFee = 0;
            foreach (DateTime date in dates.Skip(1))
            {
                int nextFee = GetTollFee(date);
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

        public int GetTollFee(DateTime date)
        {
            return _tollRepository.GetTollFee(date);
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