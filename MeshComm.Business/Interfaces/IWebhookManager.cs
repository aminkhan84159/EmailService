using MeshComm.Data.Messages.Webhook;

namespace MeshComm.Business.Interfaces
{
    public interface IWebhookManager
    {
        Task<WebhookResponse> HandleWebhookAsync(WebhookRequest webhookRequest);
    }
}
