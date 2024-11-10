using PublicHoliday;
using TollCalculation.Core.Interfaces;

namespace TollCalculation.Infrastructure.Services
{
    public class HolidayService : IHolidayService
    {
        private const int TollFreeMonthJuly = 7;
        private readonly SwedenPublicHoliday _swedenPublicHoliday;

        public HolidayService(SwedenPublicHoliday swedenPublicHoliday)
        {
            _swedenPublicHoliday = swedenPublicHoliday ?? throw new ArgumentNullException(nameof(swedenPublicHoliday));
        }

        public bool IsTollFreeDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return true;

            if (IsFreeTollMonth(date.Month))
                return true;

            if (IsHoliday(date))
                return true;

            return false;
        }
        private bool IsHoliday(DateTime date)
        {
            return _swedenPublicHoliday.IsPublicHoliday(date);
        }

        private static bool IsFreeTollMonth(int month)
        {
            return month == TollFreeMonthJuly;
        }
    }
}
