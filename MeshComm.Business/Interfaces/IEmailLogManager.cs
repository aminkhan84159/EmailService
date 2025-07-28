using MeshComm.Data.Messages.EmailLog;

namespace MeshComm.Business.Interfaces
{
    public interface IEmailLogManager
    {
        Task<GetEmailLogListResponse> GetAllAsync();
    }
}
