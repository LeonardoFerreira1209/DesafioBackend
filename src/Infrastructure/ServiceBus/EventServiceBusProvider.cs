using Domain.Contracts.Providers.ServiceBus;
using Domain.Dtos.Configurations;
using Infrastructure.ServiceBus.Base;
using Microsoft.Extensions.Options;

namespace Infrastructure.ServiceBus;

/// <summary>
/// Classe de provider barramento de mensagem de eventos.
/// </summary>
public class EventServiceBusProvider(IOptions<AppSettings> appsettings)
    : ServiceBusProviderBase(
        Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING")
            ?? appsettings.Value.ServiceBus.ConnectionString, "events"), IEventServiceBusProvider
{

}
