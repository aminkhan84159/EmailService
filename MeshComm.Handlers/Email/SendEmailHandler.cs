using MeshComm.Business.Api.Interfaces;
using MeshComm.Business.Api.Models;
using MeshComm.Core.ServiceFramework.Handlers;
using MeshComm.Data.Enums;
using MeshComm.Data.Messages.Email;
using MeshComm.DataServices.Interfaces;
using MeshComm.Entities;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text.Json;

namespace MeshComm.Handlers.Email
{
    public class SendEmailHandler(
        ILogger _logger,
        EmailContext _EmailContext,
        IEmailLogService _EmailLogService,
        IZohoCommunicationServiceManager _zohoCommunicationServiceManager,
        IConfiguration _configuration)
        : HandlerBase<SendEmailRequest, SendEmailResponse>(_logger, _EmailContext)
    {
        private readonly string SenderAddress = _configuration["SenderEmailAddress"] ?? throw new ArgumentNullException(nameof(SenderAddress));
        protected override async Task<bool> HandleCoreAsync()
        {
            var emailMessage = SetupEmailMessage();
            ZohoResponseModel zohoResponseModel;

            try
            {
                zohoResponseModel = await _zohoCommunicationServiceManager.SendEmailAsync(emailMessage);

                await CreateEmailLog(zohoResponseModel);          
                
                if (zohoResponseModel is null)
                {
                    return BadRequest("Failed to send email, received invalid status from communication service");
                }
            }
            catch (Exception ex)
            {
                Response.Exception = ex.Message;

                return BadRequest($"Unable to send email! Try again");
            }

            return Success();
        }

        public string GetHtmlTemplate()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Templates", "EmailTemplate.html");
            var template = File.ReadAllText(path);
            template = template.Replace("{{EmailBodyContent}}", Request.EmailBody);

            return template;
        }

        private ZohoEmailModel SetupEmailMessage()
        {
            var zohoEmailModel = new ZohoEmailModel
            {
                From = new ZohoEmailAddressModel
                {
                    EmailAddress = SenderAddress,
                    Name = "no-reply-beta"
                },
                To = Request.RecipientEmail.Select(x => new ToRecipientModel
                {
                    EmailAddresses = new ZohoEmailAddressModel { EmailAddress = x, Name = "" }
                }).ToList(),
                Cc = Request.Cc?.Select(x => new ToCcModel
                {
                    EmailAddresses = new ZohoEmailAddressModel { EmailAddress = x, Name = "" }
                }).ToList() ?? [],
                Bcc = Request.Bcc?.Select(x => new ToBccModel
                {
                    EmailAddresses = new ZohoEmailAddressModel { EmailAddress = x, Name = "" }
                }).ToList() ?? [],
                Subject = Request.EmailSubject,
                HtmlBody = GetHtmlTemplate()
            };

            return zohoEmailModel;
        }

        private async Task CreateEmailLog(ZohoResponseModel zohoResponseModel)
        {
            var recipientEmails = "[" + string.Join(", ", Request.RecipientEmail) + "]";
            var ccEmails = Request.Cc.Count == 0 ? null : string.Join(", ", Request.Cc);
            var bccEmails = Request.Bcc.Count == 0 ? null : string.Join(", ", Request.Bcc);

            ccEmails = ccEmails is not null ? "[" + ccEmails + "]" : null;
            bccEmails = bccEmails is not null ? "[" + bccEmails + "]" : null;

            var emailLog = new MeshComm.Entities.Entities.EmailLog()
            {
                RequestId = zohoResponseModel.RequestId,
                RequestingApplication = "App",
                EmailBody = Request.EmailBody,
                RecipientEmail = recipientEmails,
                Cc = ccEmails,
                Bcc = bccEmails,
                ResponseObject = JsonSerializer.Serialize(zohoResponseModel),
                EmailTransmissionState = EmailTransmissionStateEnum.Sent.ToString(),
                CreatedBy = 3,
                CreatedOn = DateTime.UtcNow
            };

            await _EmailLogService.AddAsync(emailLog);
        }
    }
}
