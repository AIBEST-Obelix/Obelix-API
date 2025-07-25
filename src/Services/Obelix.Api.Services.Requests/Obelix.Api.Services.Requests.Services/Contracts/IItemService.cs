namespace Obelix.Api.Services.Requests.Services.Contracts;

public interface IItemService
{
    Task CreateItemAsync(string itemId, string name, bool isDeleted);
    
    Task<string?> GetItemByIdAsync(string itemId);
}