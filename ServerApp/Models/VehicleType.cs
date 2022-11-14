using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleLeasing.Models
{
    [Table("VehicleTypes")]
    public class VehicleType
    {
        public short Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
