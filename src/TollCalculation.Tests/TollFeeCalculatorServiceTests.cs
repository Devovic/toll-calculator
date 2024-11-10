using FakeItEasy;
using FluentAssertions;
using TollCalculation.Core.Entities;
using TollCalculation.Core.Interfaces;
using TollCalculation.Core.Services;

namespace TollCalculation.Tests
{
    [TestFixture]
    public class TollFeeCalculatorServiceTests
    {
        private TollFeeCalculatorService _service;
        private IVehicleRepository _vehicleRepository;
        private ITimeConversionService _timeZoneService;
        private IHolidayService _holidayService;
        private ITollCalculator _tollCalculator;

        [SetUp]
        public void SetUp()
        {
            _vehicleRepository = A.Fake<IVehicleRepository>();
            _timeZoneService = A.Fake<ITimeConversionService>();
            _holidayService = A.Fake<IHolidayService>();
            _tollCalculator = A.Fake<ITollCalculator>();

            _service = new TollFeeCalculatorService(_vehicleRepository,
                                                    _timeZoneService,
                                                    _holidayService,
                                                    _tollCalculator);
        }

        [Test]
        public async Task CalculateTool_Should_Return_Zero_Fee_For_All_Days_When_Vehicle_Is_Toll_Free()
        {
            // Arrange
            var vehicleType = "motorbike";
            var dates = new[]
            {
                new DateTime(2024, 11, 10, 10, 0, 0),
                new DateTime(2024, 11, 11, 10, 0, 0)
            };

            A.CallTo(() => _vehicleRepository.GetVehicleByType(vehicleType)).Returns(new Vehicle { IsTollFree = true });

            // Act
            var result = await _service.CalculateTool(vehicleType, dates);

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainKey(dates[0].Date);
            result.Should().ContainKey(dates[1].Date);

            var firstDayFee = result[dates[0].Date];
            var SecondDayFee = result[dates[1].Date];

            firstDayFee.Should().Be(0);
            SecondDayFee.Should().Be(0);
        }

        [Test]
        public async Task CalculateTool_Should_Call_Toll_Calculator_For_Each_Day_When_Vehicle_Is_Not_Toll_Free()
        {
            // Arrange
            var vehicleType = "car";
            var dates = new[]
            {
                new DateTime(2024, 11, 10, 8, 0, 0),
                new DateTime(2024, 11, 11, 18, 0, 0),
                new DateTime(2024, 11, 12, 20, 0, 0)
            };
            A.CallTo(() => _vehicleRepository.GetVehicleByType(vehicleType))
                .Returns(new Vehicle { IsTollFree = false });

            A.CallTo(() => _timeZoneService.ConvertToSwedishTime(A<DateTime>.Ignored))
                .ReturnsLazily((DateTime date) =>
                {
                    var swedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
                    return TimeZoneInfo.ConvertTimeFromUtc(date, swedishTimeZone);
                });

            A.CallTo(() => _holidayService.IsTollFreeDate(dates[0].Date)).Returns(true); // Sunday is toll-free
            A.CallTo(() => _holidayService.IsTollFreeDate(dates[1].Date)).Returns(false); // Monday is not toll-free
            A.CallTo(() => _holidayService.IsTollFreeDate(dates[2].Date)).Returns(false); // Tuesday is not toll-free

            A.CallTo(() => _tollCalculator.CalculateDailyToll(A<DateTime[]>.That.Matches(d =>
                d.Any(dt => dt.Date == TimeZoneInfo.ConvertTimeFromUtc(dates[1], TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm")).Date))))
                .Returns(8);

            A.CallTo(() => _tollCalculator.CalculateDailyToll(A<DateTime[]>.That.Matches(d =>
                d.Any(dt => dt.Date == TimeZoneInfo.ConvertTimeFromUtc(dates[2], TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm")).Date))))
                .Returns(0);

            // Act
            var result = await _service.CalculateTool(vehicleType, dates);

            // Assert
            result.Should().HaveCount(3); 
            result[dates[0].Date].Should().Be(0); // Fee 0 for Sunday
            result[dates[1].Date].Should().Be(8); // Fee 8 for Monday
            result[dates[2].Date].Should().Be(0); // Fee 0 for Tuesday

            A.CallTo(() => _tollCalculator.CalculateDailyToll(A<DateTime[]>.Ignored)).MustHaveHappenedTwiceExactly();
        }
    }
}
