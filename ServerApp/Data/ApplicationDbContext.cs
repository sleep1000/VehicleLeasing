using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VehicleLeasing.Models;

namespace VehicleLeasing.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<Driver>
    {
        public virtual DbSet<Lease> Leases { get; set; } = null!;
        public virtual DbSet<Vehicle> Vehicles { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }
    }
}