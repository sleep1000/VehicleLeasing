using VehicleLeasing.Models;
using VehicleLeasing.Util;

namespace VehicleLeasing.Services
{
    public interface IVehicleService
    {
        Task<Result<Vehicle, Exception>> GetVehicleById(int id);
        Task<Result<bool, Exception>> IsVehicleInLease(Vehicle vehicle);
        Task<Result<List<Vehicle>, Exception>> GetVehicles();
    }
}
