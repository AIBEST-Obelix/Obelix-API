using System.ComponentModel.DataAnnotations;
using Obelix.Api.Services.Shared.Data.Interfaces;

namespace Obelix.Api.Services.Requests.Data.Models;

/// <summary>
/// Represents an item entity with a unique identifier.
/// </summary>
public class Item
{
    /// <summary>
    /// Unique identifier for the item.
    /// </summary>
    [MaxLength(256)]
    public string Id { get; set; } = null!;
    
    public string Name { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; } = false;
}