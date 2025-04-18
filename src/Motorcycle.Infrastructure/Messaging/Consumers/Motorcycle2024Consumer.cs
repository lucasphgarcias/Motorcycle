using System.Text;
using Microsoft.EntityFrameworkCore;
using Motorcycle.Domain.Entities;
using Motorcycle.Infrastructure.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Motorcycle.Domain.Events;
using Motorcycle.Infrastructure.Data.Context;
using Motorcycle.Infrastructure.Messaging.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Motorcycle.Infrastructure.Messaging.Consumers;

public class Motorcycle2024Consumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly ILogger<Motorcycle2024Consumer> _logger;
    private readonly string _exchangeName = "motorcycle.events";
    private readonly string _queueName = "motorcycle.2024.notifications";
    private readonly string _routingKey = "motorcycle_created_event";
    public Motorcycle2024Consumer(
        IOptions<RabbitMqSettings> settings,
        IServiceProvider serviceProvider,
        ILogger<Motorcycle2024Consumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        try
        {
            int retryCount = 0;
            const int maxRetries = 10;
            bool connected = false;

            while (!connected && retryCount < maxRetries)
            {
                try
                {
                    // Configuração da conexão com o RabbitMQ
                    var factory = new ConnectionFactory
                    {
                        HostName = settings.Value.HostName,
                        UserName = settings.Value.UserName,
                        Password = settings.Value.Password,
                        VirtualHost = settings.Value.VirtualHost,
                        Port = settings.Value.Port,
                        DispatchConsumersAsync = true // Habilitar consumo assíncrono
                    };

                    // Criar a conexão e o canal
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    // Declarar a exchange
                    _channel.ExchangeDeclare(
                        exchange: _exchangeName,
                        type: ExchangeType.Topic,
                        durable: true,
                        autoDelete: false);

                    // Declarar a fila
                    _channel.QueueDeclare(
                        queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false);

                    // Vincular a fila à exchange com a routing key apropriada
                    _channel.QueueBind(
                        queue: _queueName,
                        exchange: _exchangeName,
                        routingKey: _routingKey);

                    connected = true;
                    _logger.LogInformation("Consumidor de motos 2024 inicializado com sucesso");
                }
                catch (Exception ex)
                {
                    retryCount++;
                    
                    if (retryCount >= maxRetries)
                        throw;
                    
                    _logger.LogWarning(ex, "Erro ao conectar com RabbitMQ. Tentativa {Retry} de {MaxRetries}. Aguardando 5 segundos...", 
                        retryCount, maxRetries);
                    
                    Thread.Sleep(5000); // Aguardar 5 segundos antes de tentar novamente
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar o consumidor de motos 2024");
            throw;
        }
    }

    // protected override Task ExecuteAsync(CancellationToken stoppingToken)
    // {
    //     stoppingToken.ThrowIfCancellationRequested();

    //     var consumer = new AsyncEventingBasicConsumer(_channel);
    //     // consumer.Received += async (_, ea) =>
    //     {
    //         try
    //         {
    //             var body = ea.Body.ToArray();
    //             var message = Encoding.UTF8.GetString(body);

    //             _logger.LogInformation("Mensagem recebida: {Message}", message);

    //             // Deserializar a mensagem
    //             var @event = JsonSerializer.Deserialize<MotorcycleCreatedEvent>(message);

    //             if (@event != null)
    //             {
    //                 _logger.LogInformation("Evento deserializado: {EventType}, Ano: {Year}", @event.GetType().Name, @event.Year);
    //                 // Verificar se a moto é do ano 2024
    //                 if (@event.Year == 2024)
    //                 {
    //                     await ProcessMotorcycle2024EventAsync(@event);
    //                     _logger.LogInformation("Notificação processada para moto do ano 2024, ID: {MotorcycleId}", @event.MotorcycleId);
    //                 }
    //             }

    //             // Confirmar o processamento da mensagem
    //             _channel.BasicAck(ea.DeliveryTag, false);
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogError(ex, "Erro ao processar mensagem: {ExceptionMessage}", ex.Message);
                
    //             // Em caso de erro, rejeitar a mensagem e mandá-la de volta para a fila para reprocessamento
    //             _channel.BasicNack(ea.DeliveryTag, false, true);
    //         }
    //     };

    //     consumer.Received += async (_, ea) =>
    //     {
    //         try
    //         {
    //             var body = ea.Body.ToArray();
    //             var message = Encoding.UTF8.GetString(body);
    //             _logger.LogInformation("Mensagem recebida: {Message}", message);

    //             // Usar JsonSerializerOptions para garantir a deserialização correta
    //             var options = new JsonSerializerOptions
    //             {
    //                 PropertyNameCaseInsensitive = true,
    //                 Converters = { new JsonStringEnumConverter() }
    //             };

    //             // Deserializar o evento com as opções configuradas
    //             var @event = JsonSerializer.Deserialize<MotorcycleCreatedEvent>(message, options);

    //             _logger.LogInformation("Evento deserializado: {EventType}, Ano: {@Year}, Placa: {LicensePlate}", 
    //                 @event.GetType().Name, @event.Year, @event.LicensePlate);

    //             if (@event != null && @event.Year == 2024)
    //             {
    //                 await ProcessMotorcycle2024EventAsync(@event);
    //                 _logger.LogInformation("Notificação processada para moto do ano 2024, ID: {MotorcycleId}", @event.MotorcycleId);
    //             }
    //             else
    //             {
    //                 _logger.LogInformation("Moto não é do ano 2024 (Ano: {@Year}), notificação ignorada", @event?.Year);
    //             }

    //             // Confirmar o processamento da mensagem
    //             _channel.BasicAck(ea.DeliveryTag, false);
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogError(ex, "Erro ao processar mensagem de notificação de moto 2024");
                
    //             // Em caso de erro, rejeitar a mensagem e mandá-la de volta para a fila para reprocessamento
    //             _channel.BasicNack(ea.DeliveryTag, false, true);
    //         }
    //     };
    //     // // Iniciar o consumo da fila
    //     // _channel.BasicConsume(
    //     //     queue: _queueName,
    //     //     autoAck: false, // Desativar auto-ack para garantir processamento adequado
    //     //     consumer: consumer);

    //     // return Task.CompletedTask;
    // }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Mensagem recebida: {Message}", message);

                // Usar JsonSerializerOptions para garantir a deserialização correta
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                // Deserializar o evento com as opções configuradas
                var @event = JsonSerializer.Deserialize<MotorcycleCreatedEvent>(message, options);

                _logger.LogInformation("Evento deserializado: {EventType}, Ano: {@Year}, Placa: {LicensePlate}", 
                    @event.GetType().Name, @event.Year, @event.LicensePlate);

                if (@event != null && @event.Year == 2024)
                {
                    await ProcessMotorcycle2024EventAsync(@event);
                    _logger.LogInformation("Notificação processada para moto do ano 2024, ID: {MotorcycleId}", @event.MotorcycleId);
                }
                else
                {
                    _logger.LogInformation("Moto não é do ano 2024 (Ano: {@Year}), notificação ignorada", @event?.Year);
                }

                // Confirmar o processamento da mensagem
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem de notificação de moto 2024");
                
                // Em caso de erro, rejeitar a mensagem e mandá-la de volta para a fila para reprocessamento
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        // Iniciar o consumo da fila
        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false, // Desativar auto-ack para garantir processamento adequado
            consumer: consumer);

        return Task.CompletedTask;
    }
    private async Task ProcessMotorcycle2024EventAsync(MotorcycleCreatedEvent @event)
    {
        // Usar um escopo para obter serviços scoped
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MotorcycleDbContext>();

        // Criar a notificação
        var notification = new Motorcycle2024Notification
        {
            Id = Guid.NewGuid(),
            MotorcycleId = @event.MotorcycleId,
            LicensePlate = @event.LicensePlate,
            Model = @event.Model,
            NotificationTimestamp = @event.OccurredOn,
            CreatedAt = DateTime.UtcNow
        };

        // Salvar no banco de dados
        await dbContext.Motorcycle2024Notifications.AddAsync(notification);
        await dbContext.SaveChangesAsync();

        _logger.LogInformation("Notificação de moto 2024 armazenada, ID: {NotificationId}", notification.Id);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}