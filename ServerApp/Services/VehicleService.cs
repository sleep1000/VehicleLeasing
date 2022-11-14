using VehicleLeasing.Models;
using VehicleLeasing.Data;
using VehicleLeasing.Util;
using Microsoft.EntityFrameworkCore;

namespace VehicleLeasing.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IServerTime serverTime;

        public VehicleService(ApplicationDbContext dbContext, IServerTime serverTime)
        {
            this.dbContext = dbContext;
            this.serverTime = serverTime;
        }

        public async Task<Result<Vehicle, Exception>> GetVehicleById(int id)
        {
            try
            {
                var vehicle = await dbContext.Vehicles.Include(vehicle => vehicle.Type).FirstAsync(s => s.Id == id);
                return Result<Vehicle, Exception>.Some(vehicle);
            }
            catch (Exception e)
            {
                return Result<Vehicle, Exception>.Err(e);
            }
        }

        public async Task<Result<List<Vehicle>, Exception>> GetVehicles()
        {
            try
            {
                var vehicleLeases = await (from vehicle in dbContext.Vehicles.Include(vehicle => vehicle.Type)
                                        from lease in dbContext.Leases
                                                        .Where(lease => lease.EndDate > serverTime.UtcNow 
                                                                        && lease.Vehicle.Id == vehicle.Id)
                                                        .DefaultIfEmpty()
                                        select new { vehicle, lease})
                                       .ToListAsync();

                var vehicles = vehicleLeases.Select(obj =>
                {
                    obj.vehicle.CurrentLease = obj.lease;
                    return obj.vehicle;
                }).ToList();

                return Result<List<Vehicle>, Exception>.Some(vehicles);
            }
            catch (Exception e)
            {
                return Result<List<Vehicle>, Exception>.Err(e);
            }
        }

        public async Task<Result<bool, Exception>> IsVehicleInLease(Vehicle vehicle)
        {
            try
            {
                return Result<bool, Exception>.Some(
                    await dbContext
                        .Leases
                        .Where(lease => lease.Vehicle.Id == vehicle.Id && lease.EndDate > serverTime.UtcNow)
                        .AsNoTracking()
                        .CountAsync() > 0);
            }
            catch (Exception e)
            {
                return Result<bool, Exception>.Err(e);
            }
        }
    }
}
