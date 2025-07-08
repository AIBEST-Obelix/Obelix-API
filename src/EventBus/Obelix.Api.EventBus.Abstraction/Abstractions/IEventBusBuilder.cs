using Microsoft.Extensions.DependencyInjection;

namespace Obelix.Api.EventBus.Abstraction.Abstractions;

public interface IEventBusBuilder
{
    public IServiceCollection Services { get; }
}
