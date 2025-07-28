using System.ComponentModel.DataAnnotations;

namespace Obelix.Api.Services.Identity.Shared.Models.User;

/// <summary>
/// Represents an input model for User.
/// </summary>
public class UserIM
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

    /// <summary>
    /// Gets or sets the first name of the User.
    /// </summary>    
    
    [Required]
    [StringLength(40, ErrorMessage = "Last name must be less than 40 characters and at least 4 characters long.", MinimumLength = 2)]
    [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the User.
    /// </summary>
    [Required]
    [StringLength(40, ErrorMessage = "Last name must be less than 40 characters and at least 4 characters long.", MinimumLength = 2)]
    [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;
}