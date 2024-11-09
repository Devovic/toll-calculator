﻿using TollCalculation.Core;
using FluentAssertions;

namespace TollCalculation.Tests
{
    [TestFixture]
    public class TollCalculatorTests
    {
        private readonly TollCalculator _tollCalculator;

        public TollCalculatorTests()
        {
            _tollCalculator = new TollCalculator();
        }

        public void CalculateDailyToll_With_Empty_Dates_Should_Return_Zero_Fee()
        {
            // Arrange
            DateTime[] emptyTimes = [];
            var vehicle = new Car();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, null);

            // Assert
            result.Should().Be(0);
        }

        [TestCase("2024-11-09 12:00:00")]
        [TestCase("2024-11-10 12:00:00")]
        public void CalculateDailyToll_Toll_Free_Day_Should_Return_Zero_Fee(string dateTimeString)
        {
            // Arrange
            DateTime time = DateTime.Parse(dateTimeString);
            var vehicle = new Car();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, [time]);

            // Assert
            result.Should().Be(0);
        }

        [Test]
        public void CalculateDailyToll_Toll_Free_Vehicle_Should_Return_Zero_Fee()
        {
            // Arrange
            DateTime[] times =
            {
                new DateTime(2024, 10, 7, 7, 30, 0),
            };
            var vehicle = new Motorbike();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, times);

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
            var vehicle = new Car();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, [time]);

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
            var vehicle = new Car();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, times);

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
            var vehicle = new Car();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, times);

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
            var vehicle = new Car();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, times);

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
            var vehicle = new Car();

            // Act
            var result = _tollCalculator.CalculateDailyToll(vehicle, times);

            // Assert
            result.Should().Be(60);
        }
    }
}
