using Microsoft.AspNetCore.Identity;

namespace VehicleLeasing.Models
{
    public class Driver : IdentityUser
    {
        public ICollection<Lease> Leases { get; set; } = new HashSet<Lease>();
    }
}