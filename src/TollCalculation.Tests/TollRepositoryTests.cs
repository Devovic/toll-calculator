using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TollCalculation.Infrastructure.Data;
using TollCalculation.Infrastructure.Repositories;

namespace TollCalculation.Tests
{
    [TestFixture]
    public class TollRepositoryTests
    {
        private TollContext _context;
        private TollRepository _tollRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var options = new DbContextOptionsBuilder<TollContext>()
                .UseInMemoryDatabase(databaseName: "TollDb" + Guid.NewGuid())
                .Options;

            _context = new TollContext(options);
            _tollRepository = new TollRepository(_context);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _context?.Dispose();
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
        public void GetTollFee_Should_Return_Correct_Fee(string dateTimeString, int expectedFee)
        {
            // Arrange
            DateTime dateTime = DateTime.Parse(dateTimeString);

            // Act
            var fee = _tollRepository.GetTollFee(dateTime);

            // Assert
            fee.Should().Be(expectedFee);
        }
    }
}
