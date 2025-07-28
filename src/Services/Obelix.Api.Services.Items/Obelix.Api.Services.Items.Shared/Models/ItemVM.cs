namespace Obelix.Api.Services.Items.Shared.Models;

/// <summary>
/// ViewModel for Item.
/// </summary>
public class ItemVM
{
    /// <summary>
    /// Gets or sets the item id.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item serial number.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the creation date of the item.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}