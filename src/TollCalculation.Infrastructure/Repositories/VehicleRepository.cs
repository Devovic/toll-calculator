using Microsoft.EntityFrameworkCore;
using TollCalculation.Core.Entities;
using TollCalculation.Core.Interfaces;
using TollCalculation.Infrastructure.Data;

namespace TollCalculation.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly TollContext _context;

        public VehicleRepository(TollContext context)
        {
            _context = context;
            SeedVehicles();
        }
        public async Task<Vehicle?> GetVehicleByType(string type)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Type == type);

            return vehicle;
        }

        private void SeedVehicles()
        {
            if (!_context.TollPrices.Any())
            {
                var vehicles = new List<Vehicle>
                {
                    new Vehicle { Type = "car", IsTollFree = false },
                    new Vehicle { Type = "motorbike", IsTollFree = true },
                    new Vehicle { Type = "tractor", IsTollFree = true },
                    new Vehicle { Type = "emergency", IsTollFree = true },
                    new Vehicle { Type = "diplomat", IsTollFree = true },
                    new Vehicle { Type = "foreign", IsTollFree = true },
                    new Vehicle { Type = "military", IsTollFree = true }
                };

                _context.Vehicles.AddRange(vehicles);
                _context.SaveChanges();
            }
        }
    }
}
