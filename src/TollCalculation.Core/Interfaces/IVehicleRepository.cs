using TollCalculation.Core.Entities;

namespace TollCalculation.Core.Interfaces
{
    public interface IVehicleRepository
    {
        Vehicle? GetVehicleByType(string type);
    }
}
