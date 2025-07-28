using MeshComm.Business.Api.Models;

namespace MeshComm.Business.Api.Interfaces
{
    public interface IZohoCommunicationServiceManager
    {
        Task<ZohoResponseModel> SendEmailAsync(ZohoEmailModel zohoEmailModel);
    }
}
