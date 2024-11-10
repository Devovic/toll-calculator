using Microsoft.EntityFrameworkCore;
using PublicHoliday;
using TollCalculation.Core.Interfaces;
using TollCalculation.Core.Services;
using TollCalculation.Infrastructure.Data;
using TollCalculation.Infrastructure.Repositories;
using TollCalculation.Infrastructure.Services;

namespace TollCalculation.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddDbContext<TollContext>(opt => opt.UseInMemoryDatabase("TollDb"));
            services.AddScoped<ITollRepository, TollRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
        }

        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<ITollFeeCalculatorService, TollFeeCalculatorService>();
            services.AddScoped<ITollCalculator, TollCalculator>();
            services.AddSingleton<IHolidayService, HolidayService>();
            services.AddSingleton<SwedenPublicHoliday>();
            services.AddSingleton<ITimeConversionService, TimeConversionService>();
        }
    }
}
