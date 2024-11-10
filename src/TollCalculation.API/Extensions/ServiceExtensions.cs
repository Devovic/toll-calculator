using Microsoft.EntityFrameworkCore;
using TollCalculation.Core.Interfaces;
using TollCalculation.Infrastructure.Data;
using TollCalculation.Infrastructure.Repositories;

namespace TollCalculation.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddDbContext<TollContext>(opt => opt.UseInMemoryDatabase("TollDb"));
            services.AddScoped<ITollRepository, TollRepository>();
        }
    }
}
