using TollCalculation.Core.Entities;

namespace TollCalculation.Core.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetVehicleByType(string type);
    }
}
