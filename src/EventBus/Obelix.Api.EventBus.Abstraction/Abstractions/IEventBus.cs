using Obelix.Api.EventBus.Abstraction.Events;

namespace Obelix.Api.EventBus.Abstraction.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
