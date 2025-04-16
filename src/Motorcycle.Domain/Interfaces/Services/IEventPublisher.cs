using Motorcycle.Domain.Events;

namespace Motorcycle.Domain.Interfaces.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent;
    Task PublishRangeAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IDomainEvent;
}