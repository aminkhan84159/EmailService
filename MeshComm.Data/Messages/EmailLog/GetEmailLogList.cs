using MeshComm.Core.ServiceFramework.Messages;
using MeshComm.Data.Dtos;

namespace MeshComm.Data.Messages.EmailLog
{
    public class GetEmailLogListRequest : RequestBase
    {
    }

    public class GetEmailLogListResponse : ResponseBase
    {
        public List<EmailLogDto> EmailLogs { get; set; } = null!;
    } 
}
