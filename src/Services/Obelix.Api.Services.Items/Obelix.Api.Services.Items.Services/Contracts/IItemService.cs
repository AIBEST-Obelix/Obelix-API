using Microsoft.AspNetCore.Http;
using Obelix.Api.Services.Items.Data.Models;
using Obelix.Api.Services.Items.Shared.Models;

namespace Obelix.Api.Services.Items.Services.Contracts;

/// <summary>
/// Interface for items service.
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Get item by id.
    /// </summary>
    /// <param name="id">Id of the item.</param>
    /// <returns>Item.</returns>
    Task<ItemVM?> GetItemByIdAsync(string id);

    /// <summary>
    /// Get all items.
    /// </summary>
    /// <returns>List of items.</returns>
    Task<IEnumerable<ItemVM?>> GetAllItemsAsync();

    /// <summary>
    /// Create a new item.
    /// </summary>
    /// <param name="item">Item to create.</param>
    /// <returns>Created item.</returns>
    Task<ItemVM> CreateItemAsync(ItemIM itemIM);

    /// <summary>
    /// Update an existing item.
    /// </summary>
    /// <param name="itemIM">Item input model to update with.</param>
    /// <param name="itemId">Id of the item to update.</param>
    /// <returns>Updated item.</returns>
    Task<ItemVM?> UpdateItemAsync(ItemIM itemIM, string itemId);

    /// <summary>
    /// Delete an item by id.
    /// </summary>
    /// <param name="id">Id of the item to delete.</param>
    Task DeleteItemAsync(string id);
    
    /// <summary>
    /// Get deleted items.
    /// </summary>
    /// <returns>List of deleted items.</returns>
    Task<IEnumerable<ItemVM?>> GetDeletedItemsAsync();
    
    /// <summary>
    /// Analyze item files.
    /// </summary>
    /// <param name="files">Files to analyze.</param>
    /// <returns>Analyzed item input model.</returns>
    Task<ItemIM> AnalyzeItemAsync(List<IFormFile> files);

    /// <summary>
    /// Gets item count by month for analytics.
    /// </summary>
    /// <param name="year">Year to get analytics for.</param>
    /// <returns>Dictionary with month names as keys and item counts as values.</returns>
    Task<Dictionary<string, int>> GetItemCountByMonthAsync(int year);
}