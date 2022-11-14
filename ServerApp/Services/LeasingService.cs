using VehicleLeasing.Data;
using VehicleLeasing.Exceptions;
using VehicleLeasing.Models;
using VehicleLeasing.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace VehicleLeasing.Services
{
    public class LeasingService : ILeasingService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IVehicleService vehicleService;
        private readonly IServerTime serverTime;
        private readonly IOptions<AppConfig> appConfig;

        public LeasingService(ApplicationDbContext dbContext, IVehicleService vehicleService
            , IServerTime serverTime, IOptions<AppConfig> appConfig)
        {
            this.dbContext = dbContext;
            this.vehicleService = vehicleService;
            this.serverTime = serverTime;
            this.appConfig = appConfig;
        }

        public async Task<Result<Lease, Exception>> CreateLease(Driver driver, Vehicle vehicle, int leaseDuration)
        {
            try
            {
                var transaction = dbContext.Database.BeginTransaction();
                var failed = true;

                try
                {
                    var vehicleInLease = await vehicleService.IsVehicleInLease(vehicle);

                    if (vehicleInLease.IsSome && vehicleInLease.Value)
                    {
                        return Result<Lease, Exception>
                            .Err(new VehicleInLeaseException());
                    }

                    if (vehicleInLease.IsError)
                    {
                        return Result<Lease, Exception>
                            .Err(vehicleInLease.Error);
                    }

                    var driverLeases = await GetDriverLeases(driver, true);

                    if (driverLeases.IsSome && driverLeases.Value.Count == appConfig.Value.LeasesLimitPerDriver)
                    {
                        return Result<Lease, Exception>
                            .Err(new LeasesLimitExceeded());
                    }

                    if (driverLeases.IsError)
                    {
                        return Result<Lease, Exception>
                            .Err(driverLeases.Error);
                    }

                    var now = serverTime.UtcNow;

                    var lease = dbContext.Leases.Add(Lease.Of(driver, vehicle, now, now.AddSeconds(leaseDuration)));

                    await dbContext.SaveChangesAsync();

                    failed = false;
                    return Result<Lease, Exception>.Some(lease.Entity);
                }
                catch (Exception e)
                {
                    failed = true;

                    return Result<Lease, Exception>.Err(e);
                }
                finally
                {
                    if (failed)
                        transaction.Rollback();
                    else
                        transaction.Commit();
                }
            }
            catch(Exception e)
            {
                return Result<Lease, Exception>.Err(e);
            }
        }

        public async Task<Result<List<Lease>, Exception>> GetLeases(bool activeOnly = true)
        {
            try
            {
                 var leasesQuery = dbContext
                    .Leases
                    .Include(lease => lease.Vehicle)
                    .ThenInclude(vehicle => vehicle.Type);

                if (activeOnly)
                {
                    return Result<List<Lease>, Exception>
                        .Some(await leasesQuery.Where(lease => lease.EndDate > serverTime.UtcNow).ToListAsync());
                }

                return Result<List<Lease>, Exception>
                    .Some(await leasesQuery.ToListAsync());
            }
            catch (Exception e)
            {
                return Result<List<Lease>, Exception>.Err(e);
            }
        }

        public async Task<Result<List<Lease>, Exception>> GetDriverLeases(Driver driver, bool activeOnly = true)
        {
            try
            {
                var leasesQuery = dbContext
                    .Leases
                    .Include(lease => lease.LeaseHolder)
                    .Include(lease => lease.Vehicle)
                    .ThenInclude(vehicle => vehicle.Type)
                    .Where(lease => lease.LeaseHolder.Id == driver.Id);

                if (activeOnly)
                {
                    leasesQuery = leasesQuery.Where(lease => lease.EndDate > serverTime.UtcNow);
                }

                return Result<List<Lease>, Exception>.Some(await leasesQuery.ToListAsync());
            }
            catch (Exception e)
            {
                return Result<List<Lease>, Exception>.Err(e);
            }
        }
    }
}
