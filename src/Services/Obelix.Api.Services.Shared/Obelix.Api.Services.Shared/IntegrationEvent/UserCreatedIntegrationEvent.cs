namespace Obelix.Api.Services.Shared.IntegrationEvent;

public record UserCreatedIntegrationEvent(string UserId, string FirstName, string LastName) : EventBus.Abstraction.Events.IntegrationEvent;