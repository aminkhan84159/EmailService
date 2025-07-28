using System.Text.Json.Serialization;

namespace MeshComm.Business.Api.Models
{
    public class ZohoEmailModel
    {
        [JsonPropertyName("from")]
        public ZohoEmailAddressModel From { get; set; } = null!;

        [JsonPropertyName("to")]
        public List<ToRecipientModel> To { get; set; } = null!;

        [JsonPropertyName("cc")]
        public List<ToCcModel> Cc { get; set; } = null!;

        [JsonPropertyName("bcc")]
        public List<ToBccModel> Bcc { get; set; } = null!;

        [JsonPropertyName("subject")]
        public string Subject { get; set; } = null!;

        [JsonPropertyName("htmlbody")]
        public string HtmlBody { get; set; } = null!;
    }
}
