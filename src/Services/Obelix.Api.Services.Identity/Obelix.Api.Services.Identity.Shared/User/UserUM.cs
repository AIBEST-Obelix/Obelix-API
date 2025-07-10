using System.ComponentModel.DataAnnotations;

namespace Obelix.Api.Services.Identity.Shared.Models.User;

/// <summary>
///  Represents the update model for an User.
/// </summary>
public class UserUM
{
    /// <summary>
    /// Gets or sets the email of the User.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not in a valid format")]
    [Display(Name = "Email")]
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the password of the User.
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }
}