using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using MeshComm.API.Authorization;
using MeshComm.Business.Api.Interfaces;
using MeshComm.Business.Api.Manager;
using MeshComm.Business.Interfaces;
using MeshComm.Business.Manager;
using MeshComm.DataServices.DataServices;
using MeshComm.DataServices.Interfaces;
using MeshComm.Entities;
using MeshComm.Entities.Validators;
using MeshComm.Handlers.Email;
using MeshComm.Handlers.EmailLog;
using MeshComm.Handlers.Webhook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;

namespace MeshComm.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                // Create logger that will be reconfigured to use the serilog service definitions
                // in the running host
                .CreateBootstrapLogger();

            var builder = WebApplication.CreateBuilder(args);

            ConfigureAzureKeyVault(builder);
            RegisterContext(builder);
            RegisterValidator(builder);
            RegisterSerilog(builder);
            RegisterDataService(builder);
            RegisterHandler(builder);
            RegisterManager(builder);
            RegisterAuth(builder);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}'"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureAzureKeyVault(WebApplicationBuilder builder)
        {
            var keyVaultEndpoint = new Uri(builder.Configuration["KeyVaultUrl"] ?? throw new ArgumentNullException("keyVaultEndpoint"));

            builder.Configuration.AddAzureKeyVault(
                keyVaultEndpoint,
                new DefaultAzureCredential(),
                new AzureKeyVaultConfigurationOptions()
                );
        }

        private static void RegisterDataService(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IEmailLogService, EmailLogService>();
        }

        private static void RegisterManager(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IEmailManager, EmailManager>();
            builder.Services.AddTransient<IEmailLogManager, EmailLogManager>();
            builder.Services.AddTransient<IZohoCommunicationServiceManager, ZohoCommunicationServiceManager>();
            builder.Services.AddTransient<IWebhookManager, WebhookManager>();
        }

        private static void RegisterContext(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetValue<string>(builder.Configuration["ServiceMeshCoreConnectionString"] ?? throw new ArgumentNullException("connectionString"));

            builder.Services.AddDbContext<EmailContext>(options =>
                options.UseMySql((connectionString),
                        new MySqlServerVersion(new Version(8, 0, 34))));
        }

        private static void RegisterValidator(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<EmailLogValidator>();
        }

        private static void RegisterHandler(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<SendEmailHandler>();
            builder.Services.AddTransient<GetEmailLogHandler>();
            builder.Services.AddTransient<WebhookHandler>();
        }

        private static void RegisterSerilog(WebApplicationBuilder builder)
        {
            builder.Services.AddSerilog((services, logBuilder) =>
            {
                // Load logBuilder from appsettings.*.json
                logBuilder.ReadFrom.Configuration(builder.Configuration);
                logBuilder.Enrich.FromLogContext();

                // Write to Azure loggers (when deployed to App Service)
                // NOTE: Serilog has support for Azure log streaming and App Insights telemetry
                //       but is simpler to use the loggers configured (injected) by Azure site extensions
                var providers = new LoggerProviderCollection();
                services.GetServices<ILoggerProvider>();
                logBuilder.WriteTo.Providers(providers);
            });
        }

        private static void RegisterAuth(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication("Bearer")
            .AddScheme<AuthenticationSchemeOptions, StaticTokenHandler>("Bearer", null);
        }
    }
}
