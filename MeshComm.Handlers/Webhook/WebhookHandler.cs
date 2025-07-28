using MeshComm.Core.ServiceFramework.Handlers;
using MeshComm.Data.Enums;
using MeshComm.Data.Messages.Webhook;
using MeshComm.DataServices.Interfaces;
using MeshComm.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MeshComm.Handlers.Webhook
{
    public class WebhookHandler(
        ILogger _logger,
        EmailContext _EmailContext,
        IEmailLogService _EmailLogService)
        : HandlerBase<WebhookRequest, WebhookResponse>(_logger, _EmailContext)
    {
        protected override async Task<bool> HandleCoreAsync()
        {
            if(Request.RequestId is null)
                return BadRequest("Request ID cannot be null");

            var EmailLog = await _EmailLogService.GetAll()
                .Where(x => x.RequestId == Request.RequestId)
                .FirstOrDefaultAsync();

            if (EmailLog is null)
                return BadRequest($"Email log with Request ID: {Request.RequestId} not found");

            if (Request.EventName == "softbounce" || Request.EventName == "hardbounce")
            {
                EmailLog.EmailTransmissionState = EmailTransmissionStateEnum.Failed.ToString();
                EmailLog.UpdatedOn = DateTime.UtcNow;
                EmailLog.UpdatedBy = 3;

                await _EmailLogService.UpdateAsync(EmailLog);
                Response.IsUpdated = true;
            }
            else
            {
                Response.IsUpdated = false;
            }

            return Success();
        }
    }
}
