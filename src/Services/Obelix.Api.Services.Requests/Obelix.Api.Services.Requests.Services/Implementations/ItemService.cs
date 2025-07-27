using Microsoft.EntityFrameworkCore;
using Obelix.Api.Services.Requests.Data.Data;
using Obelix.Api.Services.Requests.Services.Contracts;

namespace Obelix.Api.Services.Requests.Services.Implementations;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemService"/> class.
    /// </summary>
    /// <param name="context">Item DB Context.</param>
    public ItemService(
        ApplicationDbContext context)
    {
        this.context = context;
    }

    public Task CreateItemAsync(string itemId, string name, bool isDeleted)
    {
        var item = new Data.Models.Item
        {
            Id = itemId,
            Name = name,
            IsDeleted = isDeleted
        };

        this.context.Items.Add(item);
        return this.context.SaveChangesAsync();
    }
    
    public async Task<string?> GetItemByIdAsync(string itemId)
    {
        var item = await this.context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == itemId && !i.IsDeleted);
        
        if (item is null)
        {
            return null;
        }

        return item?.Id;
    }
}