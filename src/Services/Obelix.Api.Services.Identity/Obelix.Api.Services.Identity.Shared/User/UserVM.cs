using System.ComponentModel.DataAnnotations;

namespace Obelix.Api.Services.Identity.Shared.Models.User;

/// <summary>
/// Represents the view model for an User.
/// </summary>
public class UserVM
{
    /// <summary>
    /// Gets or sets the id of the User.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username of the User.
    /// </summary>
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    
    /// <summary>
    /// Gets or sets the first name of the User.
    /// </summary>
    
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    
    
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;
    
    
    
}
