using MeshComm.Business.Api.Interfaces;
using MeshComm.Business.Api.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace MeshComm.Business.Api.Manager
{
    public class ZohoCommunicationServiceManager(IConfiguration _configuration) : IZohoCommunicationServiceManager
    {
        private readonly string ZeptoMailApiKey = _configuration.GetValue<string>(_configuration["MeshCommZeptoMailEmailApiKey"]!) ?? throw new ArgumentNullException(nameof(ZeptoMailApiKey));
        private readonly string BaseAddress = _configuration["BaseAddress"] ?? throw new ArgumentNullException(nameof(BaseAddress));
        public async Task<ZohoResponseModel> SendEmailAsync(ZohoEmailModel zohoEmailModel)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-enczapikey", ZeptoMailApiKey);

                var json = JsonSerializer.Serialize(zohoEmailModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(BaseAddress, content);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var emailResponse = JsonSerializer.Deserialize<ZohoResponseModel>(responseBody);

                return emailResponse!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Zoho email communication send async failed, reason: {ex.Message}", ex);
            }
        }
    }
}
