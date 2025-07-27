using Obelix.Api.EventBus.Abstraction.Abstractions;
using Obelix.Api.Services.Requests.Services.Contracts;
using Obelix.Api.Services.Shared.IntegrationEvent;

namespace Obelix.Api.Services.Requests.WebHost.Handlers;

/// <summary>
/// Handles <see cref="ItemCreatedIntegrationEvent"/>.
/// </summary>
public class ItemCreatedIntegrationEventHandler : IIntegrationEventHandler<ItemCreatedIntegrationEvent>
{
    private readonly ILogger<ItemCreatedIntegrationEvent> logger;
    private readonly IItemService itemService;

    public ItemCreatedIntegrationEventHandler(ILogger<ItemCreatedIntegrationEvent> logger, IItemService itemService)
    {
        this.logger = logger;
        this.itemService = itemService;
    }

    /// <summary>
    /// Handles <see cref="UserCreatedIntegrationEvent"/>.
    /// </summary>
    /// <param name="event">Event to be handled.</param>
    public async Task Handle(ItemCreatedIntegrationEvent @event)
    {
        this.logger.LogInformation("Handling UserCreatedIntegrationEvent for item {ItemId}", @event.ItemId);
        
        var item = await this.itemService.GetItemByIdAsync(@event.ItemId);

        if (item is not null)
        {
            this.logger.LogWarning("Item with id {ItemId} already exists", @event.ItemId);
            return;
        }

        try
        {
            await this.itemService.CreateItemAsync(@event.ItemId, @event.Name, @event.IsDeleted);
            
            this.logger.LogInformation("Created item with id {ItemId}, name {Name}, isDeleted {IsDeleted}", @event.ItemId, @event.Name, @event.IsDeleted);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error while handling ItemCreatedIntegrationEvent for item {ItemId}", @event.ItemId);
            throw;
        }
    }
}