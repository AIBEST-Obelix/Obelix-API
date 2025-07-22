using System.Drawing;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Obelix.Api.Services.Items.Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Constants = Obelix.Api.Services.Items.Shared.Helpers.Constants;
using Size = SixLabors.ImageSharp.Size;

namespace Obelix.Api.Services.Items.Services.Implementations;

public class FileService : IFileService
{
    private readonly List<string> allowedExtensions = [".jpg", ".jpeg", ".png"];
    private readonly IEncryptionService encryptionService;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    /// <param name="encryptionService">Encryption service</param>
    /// <param name="configuration">Configuration</param>
    public FileService(IEncryptionService encryptionService, IConfiguration configuration)
    {
        this.encryptionService = encryptionService;
        this.configuration = configuration;
    }

    /// <inheritdoc />
    public async Task<string> StoreFileAsync(IFormFile file, byte[] key)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!this.allowedExtensions.Contains(extension))
        {
            throw new Exception($"Invalid file extension \'{extension}\'. Only {string.Join(", ", this.allowedExtensions)} are allowed.");
        }

        if (file.Length > Constants.MaxFileSize)
        {
            throw new Exception($"File size exceeds maximum allowed size of {Constants.MaxFileSize / (1024 * 1024)} MB.");
        }

        var newFileName = Guid.NewGuid() + extension;
        var uploadsDirectory = Environment.GetEnvironmentVariable("UPLOADS_DIRECTORY") is not null ?
                    Path.Combine(Environment.GetEnvironmentVariable("UPLOADS_DIRECTORY")!, "global", DateTime.Now.ToString("yyyy/MM/dd").Replace("/", Path.DirectorySeparatorChar.ToString()))
                               :
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "global", DateTime.Now.ToString("yyyy/MM/dd").Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (!Directory.Exists(uploadsDirectory))
        {
            Directory.CreateDirectory(uploadsDirectory);
        }

        byte[] fileBytes;

        if (extension == ".pdf")
        {
            // For PDFs, just read the bytes
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
        }
        else
        {
            // For images, apply optimization
            await using var imageStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(imageStream);

            int maxWidth = 1024, maxHeight = 768;
            if (image.Height > image.Width)
            {
                maxWidth = 768;
                maxHeight = 1024;
            }

            if (image.Width > maxWidth || image.Height > maxHeight)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, maxHeight)
                }));
            }

            using var ms = new MemoryStream();
            await image.SaveAsync(ms, new JpegEncoder { Quality = 67 });
            fileBytes = ms.ToArray();
        }

        // Encrypt and save the file
        var encryptedFile = this.encryptionService.Encrypt(fileBytes, key);
        var filePath = Path.Combine(uploadsDirectory, newFileName);
        await File.WriteAllBytesAsync(filePath, encryptedFile);

        return filePath;
    }

    /// <inheritdoc />
    public async Task<byte[]> GetFileAsync(string path, byte[] key)
    {
        var file = await File.ReadAllBytesAsync(path);
        var decryptedFile = this.encryptionService.Decrypt(file, key);
        return decryptedFile;
    }

    /// <inheritdoc />
    public async Task DeleteFileAsync(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}