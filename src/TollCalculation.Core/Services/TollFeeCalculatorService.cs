using TollCalculation.Core.Interfaces;

namespace TollCalculation.Core.Services
{
    public class TollFeeCalculatorService : ITollFeeCalculatorService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ITimeConversionService _timeZoneService;
        private readonly IHolidayService _holidayService;
        private readonly ITollCalculator _tollCalculator;

        public TollFeeCalculatorService(IVehicleRepository vehicleRepository,
            ITimeConversionService timeZoneService,
            IHolidayService holidayService,
            ITollCalculator tollCalculator)
        {
            _vehicleRepository = vehicleRepository;
            _timeZoneService = timeZoneService;
            _holidayService = holidayService;
            _tollCalculator = tollCalculator;
        }

        public async Task<Dictionary<DateTime, int>> CalculateTool(string vehicleType, DateTime[] utcDates)
        {
            var vehicle = await _vehicleRepository.GetVehicleByType(vehicleType);

            if (vehicle?.IsTollFree == true)
            {
                return utcDates
                    .Select(date => date.Date)
                    .Distinct()
                    .ToDictionary(date => date, dates => 0);
            }

            var groupedByDay = GroupDatesByDay(utcDates);
            var tollFeeResults = new Dictionary<DateTime, int>();

            foreach (var day in groupedByDay)
            {
                if (_holidayService.IsTollFreeDate(day.Key))
                {
                    AddTollFee(tollFeeResults, day.Key, 0);
                }
                else
                {
                    var dailyTollFee = await _tollCalculator.CalculateDailyToll(day.Value.ToArray());
                    AddTollFee(tollFeeResults, day.Key, dailyTollFee);
                }
            }

            return tollFeeResults;
        }

        private Dictionary<DateTime, List<DateTime>> GroupDatesByDay(DateTime[] dates)
        {
            return dates
                .Select(_timeZoneService.ConvertToSwedishTime)
                .GroupBy(time => time.Date)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToList()
                );
        }

        private static void AddTollFee(Dictionary<DateTime, int> dailyTollFees, DateTime date, int tollFee)
        {
            dailyTollFees[date] = tollFee;
        }
    }
}
