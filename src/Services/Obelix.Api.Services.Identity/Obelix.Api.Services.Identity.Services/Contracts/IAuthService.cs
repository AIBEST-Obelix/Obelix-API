using Obelix.Api.Services.Identity.Shared.Models.User;

namespace Obelix.Api.Services.Identity.Services.Contracts;

/// <summary>
/// Interface for authentication service.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Checks if user exists in the Database.
    /// </summary>
    /// <param name="email">Email of the user.</param>
    /// <returns>Does user exists.</returns>
    Task<bool> CheckIfUserExistsAsync(string email);
    
    /// <summary>
    /// Checks if users's provided password is correct.
    /// </summary>
    /// <param name="email">Email of the user.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>Is password correct.</returns>
    Task<bool> CheckIsPasswordCorrectAsync(string email, string password);

    /// <summary>
    /// Saves user to the database.
    /// </summary>
    /// <param name="userIm">User info.</param>
    /// <returns>Is creating successful.</returns>
    Task<Tuple<bool, string?>> CreateUserAsync(UserIM userIm);
    
    /// <summary>
    /// Saves admin to the database.
    /// </summary>
    /// <param name="userIm">Admin info.</param>
    /// <returns>Is creating successful.</returns>
    Task<Tuple<bool, string?>> CreateAdminAsync(UserIM userIm);
}