using System.Text.Json.Serialization;

namespace MeshComm.Business.Api.Models
{
    public class EmailData
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;

        [JsonPropertyName("additional_info")]
        public List<string> AdditionalInfo { get; set; } = null!;

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
    }

}
