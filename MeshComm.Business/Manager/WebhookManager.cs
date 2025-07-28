using MeshComm.Business.Interfaces;
using MeshComm.Data.Messages.Webhook;
using MeshComm.Handlers.Webhook;

namespace MeshComm.Business.Manager
{
    public class WebhookManager(
        WebhookHandler _WebhookHandler)
        : IWebhookManager
    {
        public async Task<WebhookResponse> HandleWebhookAsync(WebhookRequest webhookRequest)
        {
            return await _WebhookHandler.HandleAsync(webhookRequest);
        }
    }
}
