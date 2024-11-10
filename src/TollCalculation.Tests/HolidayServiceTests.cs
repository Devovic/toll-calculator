using PublicHoliday;
using TollCalculation.Infrastructure.Services;
using FluentAssertions;

namespace TollCalculation.Tests
{
    public class HolidayServiceTests
    {
        private readonly SwedenPublicHoliday _swedenPublicHoliday;
        private readonly HolidayService _holidayService;

        public HolidayServiceTests()
        {
            _swedenPublicHoliday = new SwedenPublicHoliday();
            _holidayService = new HolidayService(_swedenPublicHoliday);
        }

        [TestCase("2024-11-04")]
        [TestCase("2024-11-08")]
        public void IsTollFreeDate_When_Date_Is_Work_Day_Should_Return_False(DateTime date)
        {
            // Act
            var result = _holidayService.IsTollFreeDate(date);

            // Assert
            result.Should().BeFalse();
        }

        [TestCase("2024-11-09")] // Saturday
        [TestCase("2024-11-10")] // Sunday
        public void IsTollFreeDate_When_Date_Is_Weekend_Should_Return_True(DateTime date)
        {
            // Act
            var result = _holidayService.IsTollFreeDate(date);

            // Assert
            result.Should().BeTrue();
        }

        [TestCase("2024-12-24")] // Christmas Eve
        [TestCase("2024-12-25")] // Christmas Day
        [TestCase("2025-01-01")] // New Years Day
        public void IsTollFreeDate_When_Date_Is_Public_Holiday_Should_Return_True(DateTime date)
        {
            // Act
            var result = _holidayService.IsTollFreeDate(date);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsTollFreeDate_When_Month_Is_July_Should_Return_True()
        {
            // Arrange
            var date = new DateTime(2024, 7, 1);

            // Act
            var result = _holidayService.IsTollFreeDate(date);

            // Assert
            result.Should().BeTrue();
        }
    }
}
