using System.ComponentModel.DataAnnotations;
using Obelix.Api.Services.Shared.Data.Interfaces;

namespace Obelix.Api.Services.Items.Data.Models;

public class Item : ISoftDelete
{
    /// <summary>
    /// Unique identifier for the item.
    /// </summary>
    [MaxLength(256)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Name of the item.
    /// </summary>
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of the item.
    /// </summary>
    [MaxLength(128)]
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Serial number of the item.
    /// </summary>
    [MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// A collection of files associated with the item.
    /// </summary>
    public ICollection<ItemFile> ItemFiles { get; set; } = new List<ItemFile>();
    
    /// <summary>
    /// Indicates whether the item is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
    
}