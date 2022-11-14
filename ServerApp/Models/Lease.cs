using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleLeasing.Models
{
    [Table("Leases")]
    public class Lease
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Driver LeaseHolder { get; set; } = null!;

        public Vehicle Vehicle { get; set; } = null!;

        public static Lease Of(Driver driver, Vehicle vehicle, DateTime start, DateTime end)
        {
            return new Lease()
            {
                LeaseHolder = driver,
                Vehicle = vehicle,
                StartDate = start,
                EndDate = end
            };
        }
    }
}
