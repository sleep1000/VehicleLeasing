using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleLeasing.Models
{
    [Table("Vehicles")]
    public class Vehicle
    {
        public int Id { get; set; }

        public VehicleType Type { get; set; } = null!;

        [NotMapped]
        public Lease? CurrentLease { get; set; } = null;

        public string? ImageName { get; set; }
    }
}
