using System.Text.Json.Serialization;
using VehicleLeasing.Models;

using VehicleLeasing.Util;

namespace VehicleLeasing.Dto
{
    public class VehicleResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        [JsonPropertyName("imageSrc")]
        public string? ImageSrc { get; set; }

        [JsonPropertyName("currentLease")]
        public LeaseResponseDto? CurrentLease { get; set; }

        internal static VehicleResponseDto Of(Vehicle vehicle, IServerTime serverTime, string imagePathPrefix)
        {
            return new VehicleResponseDto()
            {
                Id = vehicle.Id,
                Type = vehicle.Type.Name,
                ImageSrc = $"/{imagePathPrefix}/{vehicle.ImageName}",
                CurrentLease = vehicle.CurrentLease != null
                                    ? LeaseResponseDto.Of(vehicle.CurrentLease, serverTime)
                                    : null
            };
        }

        public static ResponseDto ResponseOf(Vehicle vehicle, IServerTime serverTime, string imagePathPrefix)
        {
            return ResponseOf(vehicle, serverTime, imagePathPrefix, ResponseStatuses.Ok);
        }

        public static ResponseDto ResponseOf(Vehicle vehicle, IServerTime serverTime, string imagePathPrefix, string status)
        {
            return ResponseDto.Of(Of(vehicle, serverTime, imagePathPrefix), status);
        }
    }
}
