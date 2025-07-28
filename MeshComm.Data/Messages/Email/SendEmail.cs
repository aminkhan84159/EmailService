using MeshComm.Core.ServiceFramework.Messages;

namespace MeshComm.Data.Messages.Email
{
    public class SendEmailRequest : RequestBase
    {
        public List<string> RecipientEmail { get; set; } = null!;
        public List<string> Cc { get; set; } = null!;
        public List<string> Bcc { get; set; } = null!;
        public string EmailSubject { get; set; } = null!;
        public string EmailBody { get; set; } = null!;
    }

    public class SendEmailResponse : ResponseBase
    {
    }
}
