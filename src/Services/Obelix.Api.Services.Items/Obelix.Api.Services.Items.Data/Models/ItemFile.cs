using System.ComponentModel.DataAnnotations;

namespace Obelix.Api.Services.Items.Data.Models;

public class ItemFile
{
    /// <summary>
    /// Unique identifier for the item file.
    /// </summary>
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// The name of the file associated with the item.
    /// </summary>
    [MaxLength(256)]
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// The path where the file is stored.
    /// </summary>
    [MaxLength(512)]
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// The item associated with this file.
    /// </summary>
    public Item Item { get; set; } = null!;
 
    /// <summary>
    /// The unique identifier of the item associated with this file.
    /// </summary>
    [MaxLength(100)]
    public string ItemId { get; set; } = string.Empty;
}