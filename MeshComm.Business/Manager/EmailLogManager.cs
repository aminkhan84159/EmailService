using MeshComm.Business.Interfaces;
using MeshComm.Data.Messages.EmailLog;
using MeshComm.Handlers.EmailLog;

namespace MeshComm.Business.Manager
{
    public class EmailLogManager(
        GetEmailLogHandler _getEmailLogHandler) : IEmailLogManager
    {
        public async Task<GetEmailLogListResponse> GetAllAsync()
        {
            var getEmailLogListRequest = new GetEmailLogListRequest();

            return await _getEmailLogHandler.HandleAsync(getEmailLogListRequest);
        }
    }
}
