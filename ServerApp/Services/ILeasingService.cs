using VehicleLeasing.Models;
using VehicleLeasing.Util;

namespace VehicleLeasing.Services
{
    public interface ILeasingService
    {
        Task<Result<List<Lease>, Exception>> GetLeases(bool activeOnly = true);
        Task<Result<List<Lease>, Exception>> GetDriverLeases(Driver driver, bool activeOnly = true);
        Task<Result<Lease, Exception>> CreateLease(Driver driver, Vehicle vehicle, int leaseDuration);
    }
}
