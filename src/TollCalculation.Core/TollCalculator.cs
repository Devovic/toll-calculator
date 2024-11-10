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
            if (IsTollFreeDate(date)) return 0;;

            return _tollRepository.GetTollFee(date);
        }

        private Boolean IsTollFreeDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            //if (year == 2013)
            //{
            //    if (month == 1 && day == 1 ||
            //        month == 3 && (day == 28 || day == 29) ||
            //        month == 4 && (day == 1 || day == 30) ||
            //        month == 5 && (day == 1 || day == 8 || day == 9) ||
            //        month == 6 && (day == 5 || day == 6 || day == 21) ||
            //        month == 7 ||
            //        month == 11 && day == 1 ||
            //        month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
            //    {
            //        return true;
            //    }
            //}
            return false;
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