using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MeshComm.API.Authorization
{
    public class StaticTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public StaticTokenHandler(IOptionsMonitor<AuthenticationSchemeOptions> _options,
            ILoggerFactory _logger,
            UrlEncoder _encoder,
            IConfiguration configuration)
                : base(_options, _logger, _encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
            var expectedToken = _configuration.GetValue<string>(_configuration["ApiAuthToken"]!) ?? throw new ArgumentException("ApiAuthToken");

            if (token == null || token != expectedToken)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "ThirdPartyService") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
