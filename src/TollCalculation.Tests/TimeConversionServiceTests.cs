using FluentAssertions;
using TollCalculation.Core.Services;

namespace TollCalculation.Tests
{
    [TestFixture]
    public class TimeConversionServiceTests
    {
        private TimeConversionService _timeConversionService;

        [SetUp]
        public void SetUp()
        {
            _timeConversionService = new TimeConversionService();
        }

        [Test]
        public void ConvertToUtc_Should_Return_Correct_Utc_When_Input_Is_Date_TimeOffset()
        {
            // Arrange
            var swedishDateTimeOffset = new DateTimeOffset(2024, 11, 10, 12, 0, 0, TimeSpan.FromHours(1)); // 12:00 +01:00

            // Act
            var utcTime = _timeConversionService.ConvertToUtc(swedishDateTimeOffset);

            // Assert
            utcTime.Should().Be(swedishDateTimeOffset.UtcDateTime);
        }

        [Test]
        public void ConvertToSwedishTime_Should_Convert_Time_Correctly()
        {
            // Arrange
            var utcTime = new DateTime(2024, 3, 31, 1, 0, 0, DateTimeKind.Utc);

            // Act
            var swedishTime = _timeConversionService.ConvertToSwedishTime(utcTime);

            // Assert
            var expectedSwedishTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm"));
            swedishTime.Should().Be(expectedSwedishTime);
        }
    }
}
