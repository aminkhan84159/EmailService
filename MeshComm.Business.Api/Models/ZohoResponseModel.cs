using System.Text.Json.Serialization;

namespace MeshComm.Business.Api.Models
{
    public class ZohoResponseModel
    {
        [JsonPropertyName("data")]
        public List<EmailData> Data { get; set; } = null!;

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; } = null!;

        [JsonPropertyName("object")]
        public string ObjectType { get; set; } = null!;

    }
}
