using MeshComm.Data.Messages.Email;

namespace MeshComm.Business.Interfaces
{
    public interface IEmailManager
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailRequest sendEmailRequest);
    }
}
