using System.ComponentModel.DataAnnotations;

namespace Obelix.Api.Services.Identity.Shared.Models.User;

/// <summary>
/// Represents an input model for User.
/// </summary>
public class LoginIM
{
    /// <summary>
    /// Gets or sets the username of the User.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not in a valid format")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password of the User.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {6} and at max {100} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}