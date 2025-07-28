using MeshComm.Core.ServiceFramework.Handlers;
using MeshComm.Data.Messages.EmailLog;
using MeshComm.DataServices.Interfaces;
using MeshComm.Data.Dtos;
using MeshComm.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MeshComm.Handlers.EmailLog
{
    public class GetEmailLogHandler(
        ILogger _logger,
        EmailContext _EmailContext,
        IEmailLogService _EmailLogService)
        : HandlerBase<GetEmailLogListRequest, GetEmailLogListResponse>(_logger, _EmailContext)
    {
        protected override async Task<bool> HandleCoreAsync()
        {
            var EmailLogs = await _EmailLogService.GetAll().ToListAsync();

            if (EmailLogs is null || EmailLogs.Count == 0)
                return NotFound("No Email Log Found");

            Response.EmailLogs = EmailLogs.Select(x => new EmailLogDto
            {
                EmailLogId = x.EmailLogId,
                RequestId = x.RequestId,
                RequestingApplication = x.RequestingApplication,
                EmailBody = x.EmailBody,
                RecipientEmail = x.RecipientEmail.Substring(1, x.RecipientEmail.Length - 2).Split(", ").ToList(),
                Cc = x.Cc is null ? []: x.Cc.Substring(1, x.Cc.Length - 2).Split(", ").ToList(),
                Bcc = x.Bcc is null ? [] : x.Bcc.Substring(1, x.Bcc.Length - 2).Split(", ").ToList(),
                ResponseObject = x.ResponseObject,
                EmailTransmissionState = x.EmailTransmissionState,
                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                UpdatedBy = x.UpdatedBy,
                UpdatedOn = x.UpdatedOn,
                Version = x.Version
            }).ToList();

            return Success();
        }
    }
}
