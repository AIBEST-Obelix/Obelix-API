namespace Obelix.Api.Services.Shared.IntegrationEvent;

public record ItemCreatedIntegrationEvent(string ItemId, string Name, bool IsDeleted) : EventBus.Abstraction.Events.IntegrationEvent;