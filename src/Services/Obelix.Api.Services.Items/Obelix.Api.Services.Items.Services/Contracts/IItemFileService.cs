using Microsoft.AspNetCore.Http;
using Obelix.Api.Services.Items.Data.Models;

namespace Obelix.Api.Services.Items.Services.Contracts;

public interface IItemFileService
{
    /// <summary>
    /// Create item file from file and store it in the database.
    /// </summary>
    /// <param name="file">File to be stored</param>
    /// <param name="key">The key to be used for encryption</param>
    /// <param name="itemId">The id of the item</param>
    /// <returns>itemFile object</returns>
    Task<ItemFile> CreateItemFileAsync(IFormFile file, byte[] key, string itemId);

    /// <summary>
    /// Get item file by id.
    /// </summary>
    /// <param name="id">Id of the item file</param>
    /// <returns>ItemFile object</returns>
    Task<ItemFile> GetItemFileAsync(string id);

    /// <summary>
    /// Get all item files from the database.
    /// </summary>
    /// <returns>Collection of ItemFile objects</returns>
    Task<IEnumerable<ItemFile>> GetItemFilesAsync();

    /// <summary>
    /// Delete item file by id.
    /// </summary>
    /// <param name="id">Id of the item file</param>
    /// <returns></returns>
    Task DeleteItemFileAsync(string id);

    /// <summary>
    /// Get all item files ids by item id.
    /// </summary>
    /// <param name="itemId">Id of the item</param>
    /// <returns>Collection of item file ids</returns>
    Task<List<string>> GetItemFilesIdsByItemIdAsync(string itemId);

    /// <summary>
    /// Get item file content by id.
    /// <param name="id">Id of the item file</param>
    /// <param name="key">The key to be used for decryption</param>
    /// <returns>Byte array of the item file content</returns>
    /// </summary>
    Task<byte[]> GetItemFileContentAsync(string id, byte[] key);
}