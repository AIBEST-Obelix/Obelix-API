using System.ComponentModel.DataAnnotations;

namespace Obelix.Api.Services.Identity.Shared.Models.User;

/// <summary>
/// Represents the view model for an admin.
/// </summary>
public class UserCompanyVM
{
    /// <summary>
    /// Gets or sets the id of the admin.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username of the admin.
    /// </summary>
    [Display(Name = "Email")]
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the role of the user for a company.
    /// </summary>
    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;
}