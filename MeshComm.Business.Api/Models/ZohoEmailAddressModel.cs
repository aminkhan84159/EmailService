using System.Text.Json.Serialization;

namespace MeshComm.Business.Api.Models
{
    public class ZohoEmailAddressModel
    {
        [JsonPropertyName("address")]
        public string EmailAddress { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
