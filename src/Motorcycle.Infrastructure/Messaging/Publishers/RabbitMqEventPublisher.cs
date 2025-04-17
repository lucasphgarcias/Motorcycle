using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Motorcycle.Domain.Events;
using Motorcycle.Domain.Interfaces.Services;
using Motorcycle.Infrastructure.Messaging.Configuration;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Motorcycle.Infrastructure.Messaging.Publishers;

public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly string _exchangeName = "motorcycle.events";
    private bool _disposed;

    public RabbitMqEventPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqEventPublisher> logger)
    {
        _logger = logger;

        try
        {
            // Configuração da conexão com o RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = settings.Value.HostName,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password,
                VirtualHost = settings.Value.VirtualHost,
                Port = settings.Value.Port
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

            _logger.LogInformation("Conexão com o RabbitMQ estabelecida com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao estabelecer conexão com o RabbitMQ");
            throw;
        }
    }

    // public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent
    // {
    //     if (_disposed)
    //         throw new ObjectDisposedException(nameof(RabbitMqEventPublisher));

    //     try
    //     {
    //         // Executar a publicação em uma Task para ser assíncrono
    //         await Task.Run(() =>
    //         {
    //             // Determinar a routing key com base no tipo do evento
    //             var routingKey = GetRoutingKey(@event);
    //                 _logger.LogInformation("Publicando evento: {EventType} com routing key: {RoutingKey}, conteúdo: {@Event}", 
    //                     typeof(T).Name, routingKey, @event);

    //             // Serializar o evento
    //             var message = JsonSerializer.Serialize(@event);
    //             var body = Encoding.UTF8.GetBytes(message);

    //             // Publicar a mensagem
    //             _channel.BasicPublish(
    //                 exchange: _exchangeName,
    //                 routingKey: routingKey,
    //                 basicProperties: null,
    //                 body: body);

    //             _logger.LogInformation("Evento publicado: {EventType}, ID: {EventId}", @event.GetType().Name, @event.Id);
    //         }, cancellationToken);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Erro ao publicar evento {EventType}", typeof(T).Name);
    //         throw;
    //     }
    // }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqEventPublisher));

        try
        {
            // Executar a publicação em uma Task para ser assíncrono
            await Task.Run(() =>
            {
                // Determinar a routing key com base no tipo do evento
                var routingKey = GetRoutingKey(@event);

                // Usar opções específicas para serialização
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    //DefaultIgnoreCondition = JsonIgnoreCondition.Never // Não ignorar valores default
                };

                // Adicionar o tipo para facilitar a desserialização
                // (use este ou outro método para garantir que o tipo seja incluído)
                var eventWithType = new Dictionary<string, object>
                {
                    ["$type"] = @event.GetType().Name,
                    ["Id"] = @event.Id,
                    ["OccurredOn"] = @event.OccurredOn
                };
                
                // Adicionar todas as outras propriedades do evento usando reflexão
                foreach (var prop in @event.GetType().GetProperties())
                {
                    if (prop.Name != "Id" && prop.Name != "OccurredOn")
                    {
                        eventWithType[prop.Name] = prop.GetValue(@event);
                    }
                }

                // Serializar o dicionário com as opções configuradas
                var message = JsonSerializer.Serialize(eventWithType, options);
                
                _logger.LogInformation("Publicando evento: {EventType} com routing key: {RoutingKey}, conteúdo: {Message}", 
                    @event.GetType().Name, routingKey, message);

                var body = Encoding.UTF8.GetBytes(message);

                // Publicar a mensagem
                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                _logger.LogInformation("Evento publicado: {EventType}, ID: {EventId}", @event.GetType().Name, @event.Id);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar evento {EventType}", typeof(T).Name);
            throw;
        }
    }
    public async Task PublishRangeAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqEventPublisher));

        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }

    private string GetRoutingKey<T>(T @event) where T : IDomainEvent
    {
        // Obter o nome do tipo sem o namespace
        var eventType = @event.GetType().Name;
        
        // Converter para snake_case para seguir convenções do RabbitMQ
        var routingKey = ToSnakeCase(eventType);
        
        return routingKey;
    }

    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();
        result.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        _disposed = true;
    }

    ~RabbitMqEventPublisher()
    {
        Dispose(false);
    }
}