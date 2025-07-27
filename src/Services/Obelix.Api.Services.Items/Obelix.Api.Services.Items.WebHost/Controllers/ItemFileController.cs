using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Obelix.Api.Services.Items.Services.Contracts;

namespace Obelix.Api.Services.Items.WebHost.Controllers;

/// <summary>
/// Controller for managing item file-related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
public class ItemsFileController : ControllerBase
{
    private readonly IItemFileService itemFileService;
    private readonly IFileService fileService;
    private readonly IItemService itemService;
    private readonly ILogger<ItemsFileController> logger;
    private readonly ICurrentUser currentUser;
    private readonly string AESKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsFileController"/> class.
    /// </summary>
    /// <param name="itemFileService">Item file service</param>
    /// <param name="fileService">File service</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="itemService">Item service</param>
    /// <param name="logger">Logger</param>
    /// <param name="currentUser">Current user</param>
    public ItemsFileController(
        IItemFileService itemFileService,
        IFileService fileService,
        IConfiguration configuration,
        IItemService itemService,
        ILogger<ItemsFileController> logger,
        ICurrentUser currentUser)
    {
        this.itemFileService = itemFileService;
        this.fileService = fileService;
        this.itemService = itemService;
        this.logger = logger;
        this.currentUser = currentUser;
        this.AESKey = configuration["Encryption:AESKey"] ?? throw new InvalidOperationException("AES Key is not configured.");
    }
    
    /// <summary>
    /// Get all item files
    /// </summary>
    /// <returns>All item files</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetItemFiles()
    {
        try
        {
            var itemFiles = await this.itemFileService.GetItemFilesAsync();
            return Ok(itemFiles);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Download item file by id
    /// </summary>
    /// <param name="id">Item file id</param>
    /// <streams>Item file contents</streams>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetItemFileById(string id)
    {
        this.logger.LogInformation("User {UserId} is downloading item file {ItemFileId}",
            this.currentUser.UserId,
            id);

        try
        {
            var key = Encoding.UTF8.GetBytes(this.AESKey);

            var itemFile = await this.itemFileService.GetItemFileAsync(id);
            var itemFileData = await this.fileService.GetFileAsync(itemFile.FilePath, key);

            return this.File(itemFileData, GetMimeTypeForFileExtension(itemFile.FileName), itemFile.FileName);
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex,"User {UserId} is trying to download item file {ItemFileId} but failed with error: {Error}",
                this.currentUser.UserId,
                id,
                ex.Message);

            return this.BadRequest();
        }
    }

    /// <summary>
    /// Gets all item files for an item
    /// </summary>
    /// <param name="itemId">Item id</param>
    /// <returns>All item files for аn item</returns>
    [HttpGet("item/{itemId}")]
    [Authorize]
    public async Task<IActionResult> GetItemFilesByItemId(string itemId)
    {
        this.logger.LogInformation("User {UserId} is getting item files for item {ItemId}",
            this.currentUser.UserId,
            itemId);

        try
        {
            var itemFile = await this.itemFileService.GetItemFilesIdsByItemIdAsync(itemId);
            return this.Ok(itemFile);
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex,"User {UserId} is trying to get item files for item {ItemId} but failed with error: {Error}",
                this.currentUser.UserId,
                itemId,
                ex.Message);

            return this.BadRequest();
        }
    }

    /// <summary>
    /// Get the mime type for a file extension
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <returns>The mime type</returns>
    private string GetMimeTypeForFileExtension(string filePath)
    {
        const string defaultContentType = "application/octet-stream";

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = defaultContentType;
        }

        return contentType;
    }


    /// <summary>
    /// Delete item file by id
    /// </summary>
    /// <param name="id">Item file id</param>
    /// <returns>OK 200 if successfully deleted</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteItemFile(string id)
    {
        try
        {
            await this.itemFileService.DeleteItemFileAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}