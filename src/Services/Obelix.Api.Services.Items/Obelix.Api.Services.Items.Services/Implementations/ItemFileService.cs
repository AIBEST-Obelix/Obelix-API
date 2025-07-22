using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Obelix.Api.Services.Items.Data.Data;
using Obelix.Api.Services.Items.Data.Models;
using Obelix.Api.Services.Items.Services.Contracts;

namespace Obelix.Api.Services.Items.Services.Implementations;

public class ItemFileService : IItemFileService
{
    private readonly IFileService fileService;
    private readonly ApplicationDbContext context;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemFileService"/> class.
    /// </summary>
    /// <param name="fileService">File service</param>
    /// <param name="context">Database context</param>
    public ItemFileService(IFileService fileService, ApplicationDbContext context)
    {
        this.fileService = fileService;
        this.context = context;
    }
    
    /// <inheritdoc />
    public async Task<ItemFile> CreateItemFileAsync(IFormFile file, byte[] key, string itemId)
    {
        
        var filePath = await this.fileService.StoreFileAsync(file, key);

        var itemFile = new ItemFile()
        {
            FileName = file.FileName,
            FilePath = filePath,
            ItemId = itemId
        };

        await this.context.ItemFiles.AddAsync(itemFile);
        await this.context.SaveChangesAsync();
        
        return itemFile;
    }

    /// <inheritdoc />
    public async Task DeleteItemFileAsync(string id)
    {
        var itemFile = await this.context.ItemFiles.FirstOrDefaultAsync(x => x.Id == id);
        
        if (itemFile is null)
        {
            throw new Exception("ItemFile not found");
        }

        await this.fileService.DeleteFileAsync(itemFile.FilePath);
        
        this.context.ItemFiles.Remove(itemFile);
        await this.context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<List<string>> GetItemFilesIdsByItemIdAsync(string itemId)
    {
        var item = await this.context.Items
            .Include(x => x.ItemFiles)
            .FirstOrDefaultAsync(x => x.Id == itemId);

        if (item is null)
        {
            throw new Exception("Item not found");
        }
        
        var itemFilesIds = item.ItemFiles.Select(x => x.Id).ToList();
        
        return itemFilesIds;
    }

    /// <inheritdoc />
    public async Task<ItemFile> GetItemFileAsync(string id)
    {
        var itemFile = await this.context.ItemFiles.FirstOrDefaultAsync(x => x.Id == id);
        
        if (itemFile is null)
        {
            throw new Exception("Item file not found");
        }

        return itemFile;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ItemFile>> GetItemFilesAsync()
    {
        var itemFiles = await this.context.ItemFiles.ToListAsync();

        return itemFiles;
    }

    /// <inheritdoc />
    public async Task<byte[]> GetItemFileContentAsync(string id, byte[] key)
    {
        var itemFile = await this.context.ItemFiles.FirstOrDefaultAsync(x => x.Id == id);

        if (itemFile is null)
        {
            throw new Exception("Item file not found");
        }

        var itemFileData = await this.fileService.GetFileAsync(itemFile.FilePath, key);

        return itemFileData;
    }
}