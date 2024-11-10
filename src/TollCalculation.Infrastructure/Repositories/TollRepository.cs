using Microsoft.EntityFrameworkCore;
using TollCalculation.Core.Entities;
using TollCalculation.Core.Interfaces;
using TollCalculation.Infrastructure.Data;

namespace TollCalculation.Infrastructure.Repositories
{
    public class TollRepository : ITollRepository
    {
        private readonly TollContext _context;

        public TollRepository(TollContext context)
        {
            _context = context;
            SeedTollPrices();
        }

        public async Task<int> GetTollFee(DateTime time)
        {
            var timeOfDay = time.TimeOfDay;
            var tollFee = await _context.TollPrices
                           .Where(p => timeOfDay >= p.StartTime && timeOfDay <= p.EndTime)
                           .Select(p => p.Amount)
                           .FirstOrDefaultAsync();

            return tollFee == 0 ? 0 : tollFee;
        }

        private void SeedTollPrices()
        {
            if (!_context.TollPrices.Any())
            {
                var tollPrices = new List<TollPrice>
            {
                new TollPrice { StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(6, 29, 59), Amount = 8 },
                new TollPrice { StartTime = new TimeSpan(6, 30, 0), EndTime = new TimeSpan(6, 59, 59), Amount = 13 },
                new TollPrice { StartTime = new TimeSpan(7, 0, 0), EndTime = new TimeSpan(7, 59, 59), Amount = 18 },
                new TollPrice { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(8, 29, 59), Amount = 13 },
                new TollPrice { StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(14, 59, 59), Amount = 8 },
                new TollPrice { StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(15, 29, 59), Amount = 13 },
                new TollPrice { StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(16, 59, 59), Amount = 18 },
                new TollPrice { StartTime = new TimeSpan(17, 0, 0), EndTime = new TimeSpan(17, 59, 59), Amount = 13 },
                new TollPrice { StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(18, 29, 59), Amount = 8 },
                new TollPrice { StartTime = new TimeSpan(18, 30, 0), EndTime = new TimeSpan(23, 59, 59), Amount = 0 },
                new TollPrice { StartTime = new TimeSpan(0, 0, 0), EndTime = new TimeSpan(5, 59, 59), Amount = 0 }};

                _context.TollPrices.AddRange(tollPrices);
                _context.SaveChanges();
            }
        }
    }
}
