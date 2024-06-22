using Application.Configurations.Extensions.Initializers;
using Domain.Dtos.Configurations;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json.Serialization;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var configurations = builder.Configuration;

    /// <sumary>
    /// Pega o appsettings baseado no ambiente em execu��o.
    /// </sumary>
    configurations
         .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();

    /// <sumary>
    /// Configura as configura��es de inicializa��o da aplica��o.
    /// </sumary>
    builder.Services
        .ConfigureSerilog(configurations)
        .AddHttpContextAccessor()
        .Configure<AppSettings>(configurations)
        .AddSingleton<AppSettings>()
        .AddEndpointsApiExplorer()
        .AddOptions()
        .ConfigureLanguage()
        .ConfigureFixedRateLimit()
        .ConfigureDependencies(configurations)
        .ConfigureDatabase(configurations)
        .ConfigureIdentityServer(configurations)
        .AddAuthorization()
        .ConfigureAuthentication(configurations)
        .ConfigureApplicationCookie()
        .ConfigureHealthChecks(configurations)
        .ConfigureCors()
        .AddControllers(options =>
        {
            options.EnableEndpointRouting = false;
            options.Filters.Add(new ProducesAttribute("application/json"));

        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

    builder.Services
           .ConfigureSwagger(configurations);

    var applicationbuilder = builder.Build();

    applicationbuilder
        .UseMiddleware<ErrorHandlerMiddleware>()
        .UseHttpsRedirection()
        .UseRateLimiter()
        .UseDefaultFiles()
        .UseStaticFiles()
        .UseCookiePolicy()
        .UseCors()
        .UseResponseCaching()
        .UseAuthorization()
        .UseAuthentication()
        .UseHealthChecks()
        .UseSwaggerConfigurations(configurations);

    applicationbuilder.UseHsts();

    applicationbuilder.MapControllers();

    applicationbuilder
        .ConfigureServiceBusSubscriber();

    applicationbuilder
       .Lifetime.ApplicationStarted
           .Register(() => Log.Debug(
                   $"[LOG DEBUG] - Aplica��o inicializada com sucesso: [AUTHIO.API]\n"));

    applicationbuilder.Run();
}
catch (Exception exception)
{
    Log.Error($"[LOG ERROR] - Ocorreu um erro ao inicializar a aplicacao [AUTHIO.API] - {exception.Message}\n"); throw;
}