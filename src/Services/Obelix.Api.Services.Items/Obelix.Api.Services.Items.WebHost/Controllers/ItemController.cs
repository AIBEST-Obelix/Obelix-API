using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Obelix.Api.Services.Items.Services.Contracts;
using Obelix.Api.Services.Items.Shared.Models;
using Obelix.Api.Services.Items.WebHost.Hubs;
using Obelix.Api.Services.Shared.Data.Models.Identity;
using OpenTelemetry.Trace;

namespace Obelix.Api.Services.Items.WebHost.Controllers;

/// <summary>
/// Controller for managing item-related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> logger;
    private readonly IItemService itemService;
    private readonly IItemFileService itemFileService;
    private readonly IHubContext<ItemHub> itemHubContext;
    private readonly string AESKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsController"/> class.
    /// </summary>
    public ItemsController(
        IConfiguration configuration,
        IItemService itemService,
        IItemFileService itemFileService,
        IHubContext<ItemHub> itemHubContext,
        ILogger<ItemsController> logger)
    {
        this.itemService = itemService;
        this.itemFileService = itemFileService;
        this.logger = logger;
        this.AESKey = configuration["Encryption:AESKey"] ?? throw new InvalidOperationException("AES Key is not configured.");
    }
    
    // implementation of methods for handling item-related requests
    
    /// <summary>
    /// Gets an item by its ID.
    /// /// </summary>
    /// <param name="id">The ID of the item.</param>
    /// <returns>The item with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemByIdAsync(string id)
    {
        try
        {
            var item = await this.itemService.GetItemByIdAsync(id);
            return Ok(item);
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Item not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting item by ID: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }
    
    /// <summary>
    /// Gets all items.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllItemsAsync()
    {
        try
        {
            var items = await this.itemService.GetAllItemsAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting all items.");
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }
    
    /// <summary>
    /// Creates a new item.
    /// </summary>
    /// <param name="itemIM">The item to create.</param>
    /// <returns>The created item.</returns>
    [HttpPost]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> CreateItemAsync(List<IFormFile> files)
    {
        if (files.Count == 0)
            return BadRequest("No files uploaded.");
        if (files.Count > 3)
            return BadRequest("Too many files uploaded. Please upload a maximum of 3 files.");

        try
        {
            var item = new ItemIM()
            {
                Name = "Initial",
                SerialNumber = "Initial",
                Type = "Initial"
            };
            var itemId = await this.itemService.CreateItemAsync(item);
            
            var key = Encoding.UTF8.GetBytes(this.AESKey);
            
            foreach (var file in files)
            {
                await this.itemFileService.CreateItemFileAsync(file, key, itemId.Id);
            }
            
            HttpContext.Response.OnCompleted(async () =>
            {
                try
                {
                    
<<<<<<< Updated upstream
                    var result = await itemService.AnalyzeItemAsync(itemAnalyzeIM.Files);
=======
                    var result = await itemService.AnalyzeItemAsync(files);

                    var @event = new ItemCreatedIntegrationEvent(
                        itemId.Id,
                        result.Name,
                        false
                    );

                    await this.eventService.PublishThroughEventBusAsync(@event);
>>>>>>> Stashed changes
                    
                    await this.itemService.UpdateItemAsync(new ItemIM
                    {
                        Name = result.Name,
                        SerialNumber = result.SerialNumber,
                        Type = result.Type,
                    }, itemId.Id);
                    
                    this.logger.LogInformation("Item {ItemId} analyzed successfully.", itemId.Id);
            
                    // send notification via SignalR
                    await itemHubContext.Clients.All
                        .SendAsync("ItemAnalyzed", new { ItemId = itemId , Message = "Item analyzed successfully!" });
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Error while analyzing item: {Message}", e.Message);
                    
                    // send notification via SignalR
                    await itemHubContext.Clients.All
                        .SendAsync("ItemAnalysisFailed", new { ItemId = itemId, Message = "Item analysis failed." });
                }  
            });
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogWarning(ex,"Analysis failed for item: {Message}", ex.Message);
            return NotFound(new ResponseModel { Status = "analyze-failed", Message =  "Analysis failed." });
        }
        
        return this.Ok(new ResponseModel { Status = "Success", Message = "Item analyzed successfully!" });
    }
    
    /// <summary>
    /// Deletes an item by its ID.
    /// </summary>
    /// <param name="id">The ID of the item to delete.</param>
    /// <returns>A response indicating the result of the deletion.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> DeleteItemAsync(string id)
    {
        try
        {
            await this.itemService.DeleteItemAsync(id);
            return Ok(new { Message = "Item deleted successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Item not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while deleting item: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }
    
    /// <summary>
    /// Updates an item by its ID.
    /// </summary>
    /// <param name="id">The ID of the item to update.</param>
    /// <param name="itemIM">The updated item data.</param>
    /// <returns>A response indicating the result of the update.</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> UpdateItemAsync(string id, [FromBody] ItemIM itemIM)
    {
        if (itemIM == null)
        {
            return BadRequest(new { Message = "Item data is required." });
        }

        try
        {
            await this.itemService.UpdateItemAsync(itemIM, id);
            return Ok(new { Message = "Item updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Item not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while updating item: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }
}