using MeshComm.Core.ServiceFramework.Messages;

namespace MeshComm.Data.Messages.Webhook
{
    public class WebhookRequest : RequestBase
    {
        public string EventName { get; set; } = null!;
        public string RequestId { get; set; } = null!;
    }

    public class WebhookResponse : ResponseBase
    {
        public bool IsUpdated { get; set; }
    }
}
