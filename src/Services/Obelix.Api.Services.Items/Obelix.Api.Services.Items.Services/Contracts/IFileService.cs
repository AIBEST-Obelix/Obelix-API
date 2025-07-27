using Microsoft.AspNetCore.Http;

namespace Obelix.Api.Services.Items.Services.Contracts;

public interface IFileService
{
    /// <summary>
    /// Stores a file asynchronously.
    /// </summary>
    /// <param name="file">File to store</param>
    /// <param name="key">Encryption key</param>
    /// <returns></returns>
    Task<string> StoreFileAsync(IFormFile file, byte[] key);

    /// <summary>
    /// Retrieves a file asynchronously.
    /// </summary>
    /// <param name="path">Path to item</param>
    /// <param name="key">Encryption key</param>
    /// <returns>Byte array of decoded file contents</returns>
    Task<byte[]> GetFileAsync(string path, byte[] key);

    /// <summary>
    /// Deletes a file asynchronously.
    /// </summary>
    /// <param name="path">Path to item</param>
    /// <returns></returns>
    Task DeleteFileAsync(string path);
}