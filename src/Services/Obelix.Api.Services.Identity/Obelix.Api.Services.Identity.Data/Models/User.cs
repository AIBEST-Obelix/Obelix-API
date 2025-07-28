using Obelix.Api.Services.Shared.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obelix.Api.Services.Identity.Data.Models.Identity;

/// <summary>
/// Represents a user in the identity system.
/// </summary>
public class User : IdentityUser, ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the user is deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [MaxLength(100)]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    [MaxLength(100)]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Email come to IdentityUser:
    // public string Email { get; set; }
}