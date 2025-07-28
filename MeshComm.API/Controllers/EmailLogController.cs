using MeshComm.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeshComm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailLogController(
        ILogger<EmailLogController> _logger,
        IEmailLogManager _EmailLogManager) :
        ApiController(_logger)
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("list", Name = "EmailLogController-GetAllLogsAsync")]
        public async Task<IActionResult> GetAllLogsAsync()
        {
            return await GetResponseAsync(async () =>
            {
                var EmailLogs = await _EmailLogManager.GetAllAsync();

                return Ok(EmailLogs);
            });
        }
    }
}
