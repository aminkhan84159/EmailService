using MeshComm.Business.Interfaces;
using MeshComm.Data.Messages.Email;
using MeshComm.Handlers.Email;

namespace MeshComm.Business.Manager
{
    public class EmailManager(
        SendEmailHandler _sendEmailHandler) : IEmailManager
    {
        public async Task<SendEmailResponse> SendEmailAsync(SendEmailRequest sendEmailRequest)
        {
            return await _sendEmailHandler.HandleAsync(sendEmailRequest);
        }
    }
}
