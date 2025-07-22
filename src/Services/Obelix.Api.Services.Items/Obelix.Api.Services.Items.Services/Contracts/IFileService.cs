using Microsoft.AspNetCore.Http;

namespace Obelix.Api.Services.Items.Services.Contracts;

public interface IFileService
{
    /// <summary>
    /// Transfers the file to the server and stores it in the Uploads folder.
    /// The file name is changed to a GUID to prevent conflicts.
    /// </summary>
    /// <param name="file">File from form request to be stored</param>
    /// <returns>
    /// Returns the path of the stored file.
    /// </returns>
    /// <exception cref="Exception">Throws if file extension is not allowed or file size exceeds maximum allowed size.</exception>
    public Task<string> StoreFileAsync(IFormFile file, byte[] key);

    /// <summary>
    /// Get file from the server by path.
    /// </summary>
    /// <param name="path">Path of the file to be retrieved</param>
    /// <returns>Byte array of the file</returns>
    public Task<byte[]> GetFileAsync(string path, byte[] key);

    /// <summary>
    /// Deletes the file from the server by path.
    /// </summary>
    /// <param name="path">Path of the file to be deleted</param>
    /// <returns></returns>
    public Task DeleteFileAsync(string path);
}