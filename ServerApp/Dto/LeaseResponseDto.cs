using VehicleLeasing.Models;
using VehicleLeasing.Util;
using System.Text.Json.Serialization;

namespace VehicleLeasing.Dto
{
    public class LeaseResponseDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("vehicleId")]
        public int VehicleId { get; set; }
        [JsonIgnore]
        public DateTime StartDate { get; set; }
        [JsonIgnore]
        public DateTime EndDate { get; set; }
        [JsonPropertyName("remain")]
        public int Remain { get; set; }

        internal static LeaseResponseDto Of(Lease lease, IServerTime serverTime)
        {
            return new LeaseResponseDto()
            {
                Id = lease.Id,
                VehicleId = lease.Vehicle.Id,
                StartDate = lease.StartDate,
                EndDate = lease.EndDate,
                Remain = (int)lease.EndDate.Subtract(serverTime.UtcNow).TotalSeconds
            };
        }

        public static ResponseDto ResponseOf(Lease lease, IServerTime serverTime)
        {
            return ResponseOf(lease, serverTime, ResponseStatuses.Ok);
        }

        public static ResponseDto ResponseOf(Lease lease, IServerTime serverTime, string status)
        {
            return ResponseDto.Of(Of(lease, serverTime), status);
        }

    }
}
