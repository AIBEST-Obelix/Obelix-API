namespace Obelix.Api.Services.Shared.Data.Models.Identity;

/// <summary>
/// Defines the user roles within the application. These roles are used to manage access levels and permissions
/// for different types of users, ensuring that users can only access features and data relevant to their role.
/// </summary>
public static class UserRoles
{
    /// <summary>
    /// Global administrator role. Users with this role have full access to all features and data in the application.
    /// </summary>
    public const string Admin = "Admin";
    
    /// <summary>
    /// Represents the general user role. Users with this role have access to general features of the application
    /// that do not require account or administrative privileges.
    /// </summary>
    public const string User = "User";
}