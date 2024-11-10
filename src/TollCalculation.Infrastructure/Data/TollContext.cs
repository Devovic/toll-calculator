using Microsoft.EntityFrameworkCore;
using TollCalculation.Core.Entities;

namespace TollCalculation.Infrastructure.Data
{
    public class TollContext : DbContext
    {
        public TollContext(DbContextOptions<TollContext> options) : base(options) { }

        public DbSet<TollPrice> TollPrices { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
    }
}
