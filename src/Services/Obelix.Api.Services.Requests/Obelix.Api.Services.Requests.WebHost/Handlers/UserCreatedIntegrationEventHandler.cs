using Obelix.Api.EventBus.Abstraction.Abstractions;
using Obelix.Api.Services.Requests.Services.Contracts;
using Obelix.Api.Services.Shared.IntegrationEvent;

namespace Obelix.Api.Services.Requests.WebHost.Handlers;

/// <summary>
/// Handles <see cref="UserCreatedIntegrationEvent"/>.
/// </summary>
public class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
{
    private readonly ILogger<UserCreatedIntegrationEvent> logger;
    private readonly IUserService userService;

    public UserCreatedIntegrationEventHandler(ILogger<UserCreatedIntegrationEvent> logger, IUserService userService)
    {
        this.logger = logger;
        this.userService = userService;
    }

    /// <summary>
    /// Handles <see cref="UserCreatedIntegrationEvent"/>.
    /// </summary>
    /// <param name="event">Event to be handled.</param>
    public async Task Handle(UserCreatedIntegrationEvent @event)
    {
        this.logger.LogInformation("Handling UserCreatedIntegrationEvent for user {UserId}", @event.UserId);
        
        var user = await this.userService.GetUserByIdAsync(@event.UserId);

        if (user is not null)
        {
            this.logger.LogWarning("User with id {UserId} already exists", @event.UserId);
            return;
        }

        try
        {
            await this.userService.CreateUserAsync(@event.UserId, @event.FirstName, @event.LastName);
            
            this.logger.LogInformation("Created user with id {UserId}, first and last names {Names}", @event.UserId, $"{@event.FirstName} {@event.LastName}");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error while handling UserCreatedIntegrationEvent for user {UserId}", @event.UserId);
            throw;
        }
    }
}