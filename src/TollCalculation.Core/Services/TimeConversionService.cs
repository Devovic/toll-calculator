using TollCalculation.Core.Interfaces;

namespace TollCalculation.Core.Services
{
    public class TimeConversionService : ITimeConversionService
    {
        public DateTime ConvertToSwedishTime(DateTime utcTime)
        {
            var swedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, swedishTimeZone);
        }

        public DateTime ConvertToUtc(DateTimeOffset inputTime)
        {
            return inputTime.UtcDateTime;
        }
    }
}
