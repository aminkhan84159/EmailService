using MeshComm.Business.Interfaces;
using MeshComm.Data.Messages.Webhook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeshComm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController(
        ILogger<WebhookController> _logger,
        IWebhookManager _WebhookManager)
        : ApiController(_logger)
    {
        [HttpPost]
        public async Task<IActionResult> HandleWebhookAsync([FromBody] WebhookRequest webhookRequest)
        {
            return await GetResponseAsync(async () =>
            {
                var webhookResponse = await _WebhookManager.HandleWebhookAsync(webhookRequest);

                return Ok(webhookResponse);
            });
        }
    }
}
