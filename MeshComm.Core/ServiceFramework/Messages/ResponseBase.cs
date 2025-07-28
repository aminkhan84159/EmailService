using MeshComm.Core.ServiceFramework.Enums;
using MeshComm.Core.ServiceFramework.Interfaces;

namespace MeshComm.Core.ServiceFramework.Messages
{
    public class ResponseBase : IResponseBase
    {
        public ResponseBase()
        {
            ValidationErrors = [];
        }

        public string? Exception { get; set; }
        public double ExecutionTimeSeconds { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public ResponseStatusTypeEnum ResponseStatusType { get; set; }
        public List<string> ValidationErrors { get; set; }
    }
}
