using MeshComm.Core.Services;
using MeshComm.DataServices.Interfaces;
using MeshComm.Entities;
using MeshComm.Entities.Entities;
using MeshComm.Entities.Validators;

namespace MeshComm.DataServices.DataServices
{
    public class EmailLogService : GenericService<EmailLog,EmailLogValidator>, IEmailLogService
    {
        public EmailLogService(
            EmailContext _EmailContext,
            EmailLogValidator _EmailLogValidators)
            :base(_EmailContext, _EmailLogValidators)
        {
        }
    }
}
