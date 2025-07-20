using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Obelix.Api.Services.Items.Data.Data;
using Obelix.Api.Services.Items.Data.Models;
using Obelix.Api.Services.Items.Services.Contracts;
using Obelix.Api.Services.Items.Shared.Models;

namespace Obelix.Api.Services.Items.Services.Implementations;

/// <summary>
/// Implementation of the item service.
/// </summary>
public class ItemService : IItemService
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemService"/> class.
    /// </summary>
    public ItemService(
        ApplicationDbContext context,
        IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }
    
    /// <inheritdoc />
    public async Task<ItemVM?> GetItemByIdAsync(string id)
    {
        var item = await this.context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        
        if (item == null) 
        {
            throw new KeyNotFoundException($"Item with id {id} not found.");
        }
        
        return this.mapper.Map<ItemVM>(item);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ItemVM?>> GetAllItemsAsync()
    {
        var items = await this.context.Items
            .AsNoTracking()
            .ToListAsync();
        
        return this.mapper.Map<List<ItemVM>>(items);
    }

    /// <inheritdoc />
    public async Task<ItemVM> CreateItemAsync(ItemIM itemIM)
    {
        if (itemIM == null)
        {
            throw new ArgumentNullException(nameof(itemIM), "Item input model cannot be null.");
        }
        
        if (string.IsNullOrWhiteSpace(itemIM.Name) || 
            string.IsNullOrWhiteSpace(itemIM.Type) || 
            string.IsNullOrWhiteSpace(itemIM.SerialNumber))
        {
            throw new ArgumentException("Item input model properties cannot be null or empty.");
        }
        
        var item = this.mapper.Map<Item>(itemIM);
        
        this.context.Items.Add(item);
        await this.context.SaveChangesAsync();
        return this.mapper.Map<ItemVM>(item);
    }

    /// <inheritdoc />
    public async Task<ItemVM?> UpdateItemAsync(ItemIM itemIM, string itemId)
    {
        var item = await this.context.Items
            .FirstOrDefaultAsync(i => i.Id == itemId && !i.IsDeleted);
        
        if (item == null) 
        {
            throw new KeyNotFoundException($"Item with id {itemId} not found.");
        }
        
        if (itemIM is null)
        {
            throw new ArgumentNullException(nameof(itemIM), "Item update model cannot be null.");
        }
        
        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("Item id cannot be null or empty.", nameof(itemId));
        }
        
        mapper.Map(itemIM, item);
        
        this.context.Items.Update(item);
        await this.context.SaveChangesAsync();
        return this.mapper.Map<ItemVM>(item);
    }

    /// <inheritdoc />
    public async Task DeleteItemAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) 
        {
            throw new ArgumentException("Item id cannot be null or empty.", nameof(id));
        }
        
        var item = await this.context.Items
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        
        if (item == null) 
        {
            throw new KeyNotFoundException($"Item with id {id} not found.");
        }
        
        item.IsDeleted = true; // Soft delete
        this.context.Items.Update(item);
        await this.context.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<ItemVM?>> GetDeletedItemsAsync()
    {
        var deletedItems = await this.context.Items
            .AsNoTracking()
            .Where(i => i.IsDeleted)
            .ToListAsync();
        
        return this.mapper.Map<List<ItemVM>>(deletedItems);
    }

    /// <inheritdoc />
    public async Task<ItemIM> AnalyzeItemAsync(List<IFormFile> files)
    {
        if (files.Count == 0)
            throw new InvalidOperationException("No files provided.");

        if (files.Count > 3)
            throw new InvalidOperationException("Too many files provided. Please provide a maximum of 3 files.");

        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:8111"),
            Timeout = Timeout.InfiniteTimeSpan // really never cancel
        };

        using var formData = new MultipartFormDataContent();

        var fileNumber = 0;
        foreach (var file in files)
        {
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var fileContent = stream.ToArray();
            formData.Add(new ByteArrayContent(fileContent), "files", file.FileName);
        }

        var response = await client.PostAsync($"/item/analyze", formData);
        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync();
        var analyzeResult = await JsonSerializer.DeserializeAsync<ItemIM>(responseStream);

        if (analyzeResult == null)
            throw new InvalidOperationException("Failed to parse item analysis results.");

        return analyzeResult;

    }
}