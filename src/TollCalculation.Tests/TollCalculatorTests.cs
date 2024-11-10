using TollCalculation.Core;
using FluentAssertions;
using TollCalculation.Core.Interfaces;
using FakeItEasy;

namespace TollCalculation.Tests
{
    [TestFixture]
    public class TollCalculatorTests
    {
        private readonly TollCalculator _tollCalculator;
        private readonly ITollRepository _tollRepository;


        public TollCalculatorTests()
        {
            _tollRepository = A.Fake<ITollRepository>();
            _tollCalculator = new TollCalculator(_tollRepository);
        }

        public void CalculateDailyToll_With_Empty_Dates_Should_Return_Zero_Fee()
        {
            // Arrange
            DateTime[] emptyTimes = [];

            // Act
            var result = _tollCalculator.CalculateDailyToll(emptyTimes);

            // Assert
            result.Should().Be(0);
        }

        [TestCase("2024-11-09 12:00:00")]
        [TestCase("2024-11-10 12:00:00")]
        public void CalculateDailyToll_Toll_Free_Day_Should_Return_Zero_Fee(string dateTimeString)
        {
            // Arrange
            DateTime time = DateTime.Parse(dateTimeString);

            // Act
            var result = _tollCalculator.CalculateDailyToll([time]);

            // Assert
            result.Should().Be(0);
        }

        [TestCase("2024-11-08 02:00:00", 0)]
        [TestCase("2024-11-08 06:29:00", 8)]
        [TestCase("2024-11-08 06:30:00", 13)]
        [TestCase("2024-11-08 06:59:00", 13)]
        [TestCase("2024-11-08 07:00:00", 18)]
        [TestCase("2024-11-08 07:59:00", 18)]
        [TestCase("2024-11-08 08:00:00", 13)]
        [TestCase("2024-11-08 08:29:00", 13)]
        [TestCase("2024-11-08 12:00:00", 8)]
        [TestCase("2024-11-08 12:30:00", 8)]
        [TestCase("2024-11-08 15:00:00", 13)]
        [TestCase("2024-11-08 15:29:00", 13)]
        [TestCase("2024-11-08 15:30:00", 18)]
        [TestCase("2024-11-08 16:59:00", 18)]
        [TestCase("2024-11-08 17:00:00", 13)]
        [TestCase("2024-11-08 17:59:00", 13)]
        [TestCase("2024-11-08 18:00:00", 8)]
        [TestCase("2024-11-08 18:29:00", 8)]
        [TestCase("2024-11-08 18:30:00", 0)]
        public void CalculateDailyToll_Single_Pass_Should_Return_Correct_Fee(string dateTime, int expectedFee)
        {
            // Arrange
            DateTime time = DateTime.Parse(dateTime);

            A.CallTo(() => _tollRepository.GetTollFee(time)).Returns(expectedFee);

            // Act
            var result = _tollCalculator.CalculateDailyToll([time]);

            // Assert
            result.Should().Be(expectedFee);
        }

        [Test]
        public void CalculateDailyToll_Unordered_Dates_Should_Sort_And_Calculate_Correctly()
        {
            // Arrange
            DateTime[] times =
            {
                new DateTime(2024, 11, 8, 7, 0, 0), // 18
                new DateTime(2024, 11, 8, 6, 30, 0), // 13
                new DateTime(2024, 11, 8, 2, 0, 0), // 0
            };

            A.CallTo(() => _tollRepository.GetTollFee(times[0])).Returns(0);
            A.CallTo(() => _tollRepository.GetTollFee(times[1])).Returns(13);
            A.CallTo(() => _tollRepository.GetTollFee(times[2])).Returns(18);

            // Act
            var result = _tollCalculator.CalculateDailyToll(times);

            // Assert
            result.Should().Be(18);
        }

        [Test]
        public void CalculateDailyToll_Multiple_Passes_Within_60_Minutes_Should_Return_Highest_Fee()
        {
            // Arrange
            DateTime[] times =
            {
                new DateTime(2024, 11, 8, 6, 15, 0), // 8
                new DateTime(2024, 11, 8, 6, 45, 0), // 13
                new DateTime(2024, 11, 8, 7, 10, 0), // 18
            };

            A.CallTo(() => _tollRepository.GetTollFee(times[0])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[1])).Returns(13);
            A.CallTo(() => _tollRepository.GetTollFee(times[2])).Returns(18);

            // Act
            var result = _tollCalculator.CalculateDailyToll(times);

            // Assert
            result.Should().Be(18);
        }

        [Test]
        public void CalculateDailyToll_Multiple_Passes_Exceeding_60_Minutes_Should_Add_Fees_Separately()
        {
            // Arrange
            DateTime[] times =
            {
                new DateTime(2024, 11, 8, 6, 15, 0), // 8
                new DateTime(2024, 11, 8, 7, 00, 0), // 18
                new DateTime(2024, 11, 8, 14, 15, 0), // 8
                new DateTime(2024, 11, 8, 15, 00, 0), // 13
                new DateTime(2024, 11, 8, 18, 00, 0), // 8
            };

            A.CallTo(() => _tollRepository.GetTollFee(times[0])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[1])).Returns(18);
            A.CallTo(() => _tollRepository.GetTollFee(times[2])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[3])).Returns(13);
            A.CallTo(() => _tollRepository.GetTollFee(times[4])).Returns(8);

            // Act
            var result = _tollCalculator.CalculateDailyToll(times);

            // Assert
            result.Should().Be(39);
        }

        [Test]
        public void CalculateDailyToll_Exceeding_Daily_Max_Fee_Should_Return_Max_Fee()
        {
            // Arrange
            DateTime[] times =
            {
                new DateTime(2024, 11, 8, 9, 0, 0), // 8
                new DateTime(2024, 11, 8, 10, 0, 0), // 8
                new DateTime(2024, 11, 8, 11, 0, 0), // 8
                new DateTime(2024, 11, 8, 12, 0, 0), // 8
                new DateTime(2024, 11, 8, 13, 0, 0), // 8
                new DateTime(2024, 11, 8, 15, 30, 0), // 18
                new DateTime(2024, 11, 8, 16, 30, 0), // 18
            };

            A.CallTo(() => _tollRepository.GetTollFee(times[0])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[1])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[2])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[3])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[4])).Returns(8);
            A.CallTo(() => _tollRepository.GetTollFee(times[5])).Returns(18);
            A.CallTo(() => _tollRepository.GetTollFee(times[6])).Returns(18);

            // Act
            var result = _tollCalculator.CalculateDailyToll(times);

            // Assert
            result.Should().Be(60);
        }
    }
}
