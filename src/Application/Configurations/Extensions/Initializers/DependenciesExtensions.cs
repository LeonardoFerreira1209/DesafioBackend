using Domain.Contracts.Providers.ServiceBus;
using Domain.Contracts.Repositories;
using Domain.Contracts.Repositories.Base;
using Domain.Contracts.Services;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.Repositories.BASE;
using Infrastructure.ServiceBus;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configurations.Extensions.Initializers;

/// <summary>
/// Classe de configuração do depêndencias da aplicação.
/// </summary>
public static class DependenciesExtensions
{
    /// <summary>
    /// Configuração das dependencias (Serrvices, Repository, Facades, etc...).
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureDependencies(
        this IServiceCollection services, IConfiguration configurations)
    {
        services
            .AddSingleton(serviceProvider => configurations)
        // Services
            .AddTransient<IFeatureFlagsService, FeatureFlagsService>()
            .AddTransient<IAuthenticationService, AuthenticationService>()
            .AddTransient<ITokenService, TokenService>()
        // Repository
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
            .AddScoped<IFeatureFlagsRepository, FeatureFlagsRepository>()
            .AddScoped(typeof(IGenericRepository<>), typeof(IGenericRepository<>))
            .AddScoped<IFeatureFlagsRepository, FeatureFlagsRepository>()
        // Infra
            .AddTransient<IEventServiceBusProvider, EventServiceBusProvider>()
            .AddSingleton<EventServiceBusSubscriber>();

        return services;
    }
}
