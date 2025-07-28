using MeshComm.Business.Interfaces;
using MeshComm.Data.Messages.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeshComm.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController(
        ILogger<EmailController> _logger,
        IEmailManager _emailManager) : 
        ApiController(_logger)
    {
        [HttpPost(Name = "EmailController-SendEmailAsync")]
        public async Task<IActionResult> SendEmailAsync([FromBody] SendEmailRequest sendEmailRequest)
        {
            return await GetResponseAsync(async () =>
            {
                var sendEmailResponse = await _emailManager.SendEmailAsync(sendEmailRequest);

                return Ok(sendEmailResponse);
            });
        }
    }
}
