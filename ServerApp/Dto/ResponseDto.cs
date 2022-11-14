using System.Text.Json.Serialization;

namespace VehicleLeasing.Dto
{
    public class ResponseDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = ResponseStatuses.Ok;

        [JsonPropertyName("payload")]
        public object? Payload { get; set; } = null;

        public static ResponseDto Of(object? payload, string status = ResponseStatuses.Ok)
        {
            return new ResponseDto()
            {
                Status = status,
                Payload = payload
            };
        }
    }
}
