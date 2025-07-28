using System.Text.Json.Serialization;

namespace MeshComm.Business.Api.Models
{
    public class ToRecipientModel
    {
        [JsonPropertyName("email_address")]
        public ZohoEmailAddressModel EmailAddresses { get; set; } = null!;
    }
}
