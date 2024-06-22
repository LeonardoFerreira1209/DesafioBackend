using Azure.Messaging.ServiceBus;
using Domain.Contracts.Providers.ServiceBus;
using Domain.Dtos.Configurations;
using Infrastructure.ServiceBus.Base;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Infrastructure.ServiceBus;

/// <summary>
/// Serviço de subscriber de eventos.
/// </summary>
/// <param name="appSettings"></param>
/// <param name="emailProviderFactory"></param>
public class EventServiceBusSubscriber(
    IServiceProvider serviceProvider,
    IOptions<AppSettings> appSettings)
    : ServiceBusSubscriberBase(appSettings, QUEUE_OR_TOPIC_NAME), IServiceBusSubscriber
{
    /// <summary>
    /// Const de nome da queue ou topico.
    /// </summary>
    private const string QUEUE_OR_TOPIC_NAME = "events";

    /// <summary>
    /// String de conexão do service bus.
    /// </summary>
    private readonly string busConnection = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING")
            ?? appSettings.Value.ServiceBus.ConnectionString;

    /// <summary>
    /// Processa as mensagens.
    /// </summary>
    /// <param name="messageEvent">Dados da mensagem do evento.</param>
    /// <returns>Task</returns>
    public override async Task ProcessMensageAsync(
        ProcessMessageEventArgs messageEvent)
    {
        var message = messageEvent.Message;

        try
        {
            //EventMessage<object> eventMessage =
            //    JsonConvert.DeserializeObject<EventMessage<object>>(
            //        message.Body.ToString());

            //await ProccesByEventTypeAsync(
            //    eventMessage.EventType, eventMessage.Data)
            //        .ContinueWith(async (task) =>
            //        {

            //            using var scope
            //                = serviceProvider.CreateScope();

            //            var eventRepository
            //                = scope.ServiceProvider
            //                    .GetService<IEventRepository>();

            //            await eventRepository.GetAsync(even
            //                => even.Id == eventMessage.Id).ContinueWith(async (evenTask) =>
            //                {
            //                    var eventEntity
            //                        = evenTask.Result;

            //                    eventEntity.Processed
            //                        = DateTime.Now;

            //                    await eventRepository
            //                        .UpdateAsync(eventEntity);

            //                    var unitOfWork = scope.ServiceProvider
            //                        .GetService<IUnitOfWork<Context>>();

            //                    await unitOfWork.CommitAsync();

            //                }).Unwrap();

            //        }).Unwrap();

            //Log.Information(
            //    $"[LOG INFORMATION] - {nameof(EventServiceBusSubscriber)} - METHOD {nameof(ProcessMensageAsync)} - Memsagem consumida com sucesso: {JsonConvert.SerializeObject(eventMessage.Data)}.\n");
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception: {exception.Message} - {JsonConvert.SerializeObject(exception)}\n");

            var cloneMessage = new ServiceBusMessage(messageEvent.Message)
            {
                ScheduledEnqueueTime
                    = DateTime.UtcNow.AddHours(1)
            };

            ServiceBusSender sender =
                _busClient.CreateSender(QUEUE_OR_TOPIC_NAME);

            await sender.SendMessageAsync(cloneMessage);
            await sender.CloseAsync();

            throw;
        }
    }
}
