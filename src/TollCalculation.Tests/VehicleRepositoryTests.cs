using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TollCalculation.Infrastructure.Data;
using TollCalculation.Infrastructure.Repositories;

namespace TollCalculation.Tests
{
    [TestFixture]
    public class VehicleRepositoryTests
    {
        private TollContext _context;
        private VehicleRepository _vehicleRepository;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new DbContextOptionsBuilder<TollContext>()
                .UseInMemoryDatabase(databaseName: "TollDb" + Guid.NewGuid())
                .Options;

            _context = new TollContext(options);
            _vehicleRepository = new VehicleRepository(_context);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [TestCase("car", false)]
        [TestCase("motorbike", true)]
        [TestCase("tractor", true)]
        [TestCase("emergency", true)]
        [TestCase("diplomat", true)]
        [TestCase("foreign", true)]
        [TestCase("military", true)]
        public async Task GetVehicleByType_Should_Return_Vehicle_When_Type_Exists(string expectedType, bool expectedTollFreeStatus)
        {
            // Act
            var result = await _vehicleRepository.GetVehicleByType(expectedType);

            // Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(expectedType);
            result.IsTollFree.Should().Be(expectedTollFreeStatus);
        }

        [Test]
        public async Task GetVehicleByType_Should_Return_Null_When_Type_Does_Not_Exist()
        {
            // Arrange
            var vehicleType = "boat";

            // Act
            var result = await _vehicleRepository.GetVehicleByType(vehicleType);

            // Assert
            result.Should().BeNull();
        }
    }
}
