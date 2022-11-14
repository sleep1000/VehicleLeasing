using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VehicleLeasing.Dto
{
    public class LeaseRequestDto
    {
        [JsonPropertyName("vehicleId")]
        public int VehicleId { get; set; }

        [Range(1, 10)]
        [JsonPropertyName("leaseDuration")]
        public int LeaseDuration { get; set; }

        [JsonPropertyName("signalRConnectionId")]
        public string? SignalRConnectionId { get; set; }
    }
}
