using MeshComm.Core.ServiceFramework.Enums;

namespace MeshComm.Core.ServiceFramework.Interfaces
{
    public interface IResponseBase
    {
        string? Exception { get; set; }
        double ExecutionTimeSeconds { get; set; }
        bool IsSuccess { get; set; }
        string? Message { get; set; }
        ResponseStatusTypeEnum ResponseStatusType { get; set; }
        List<string> ValidationErrors { get; set; }
    }
}
