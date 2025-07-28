using Obelix.Api.Services.Identity.Data.Data;
using Obelix.Api.Services.Identity.Data.Models.Identity;
using Obelix.Api.Services.Identity.Services.Contracts;
using Obelix.Api.Services.Identity.Shared.Models.User;
using Obelix.Api.Services.Shared.Data.Models.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Obelix.Api.Services.Identity.Services.Implementations;

internal class UserService : IUserService
{
    private readonly UserManager<User> userManager;
    private readonly IMapper mapper;
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="userManager">User Manager.</param>
    /// <param name="mapper">AutoMapper.</param>
    /// <param name="context">Db Context.</param>
    public UserService(
        UserManager<User> userManager,
        IMapper mapper,
        ApplicationDbContext context)
    {
        this.userManager = userManager;
        this.mapper = mapper;
        this.context = context;
    }
    
    /// <inheritdoc/>
    public async Task<UserVM?> GetUserByIdAsync(string id)
    {
        var admin = await this.userManager.FindByIdAsync(id);

        return admin is null ? null : this.mapper.Map<UserVM>(admin);
    }

    /// <inheritdoc/>
    public async Task<string> GetUserUsernameByIdAsync(string id)
    {
        var user = await this.userManager.FindByIdAsync(id);

        return await this.userManager.GetUserNameAsync(user);
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateOneTimeTokenForUserAsync(string userId, string token, string type, string purpose)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        
        return await this.userManager.VerifyUserTokenAsync(user, type, purpose, token);
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> ChangePasswordAsync(string userId, string newPassword)
    {
        var user = await this.userManager.FindByIdAsync(userId);

        var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

        return await this.userManager.ResetPasswordAsync(user, token, newPassword);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateUserAsync(string username, UserUM newUserInfo)
    {
        var user = await this.userManager.FindByNameAsync(username);

        if (user is null)
        {
            return false;
        }

        if (user.Email != newUserInfo.Email)
        {
            var normalizedEmail = this.userManager.NormalizeEmail(newUserInfo.Email);
            var normalizedUsername = this.userManager.NormalizeName(newUserInfo.Email);
            
            var existingUser = await this.userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return false;
            }

            var existingUserByName = await this.userManager.FindByNameAsync(normalizedUsername);
            if (existingUserByName != null && existingUserByName.Id != user.Id)
            {
                return false;
            }
        }
        
        user.Email = newUserInfo.Email;
        user.FirstName = newUserInfo.FirstName;
        user.LastName = newUserInfo.LastName;
        user.UserName = newUserInfo.Email;

        if (!string.IsNullOrEmpty(newUserInfo.Password))
        {
            var passwordResult = await this.ChangePasswordAsync(user.Id, newUserInfo.Password);
            if (!passwordResult.Succeeded)
            {
                return false;
            }
        }

        try
        {
            var result = await this.userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
        {
            // handle unique constraint violation specifically
            if (pgEx.ConstraintName == "UserNameIndex")
            {
                // username/email already exists
                return false;
            }
            // re-throw if it's a different constraint violation
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<string> GenerateOneTimeTokenForUserAsync(string username, string type, string purpose)
    {
        var user = await this.userManager.FindByNameAsync(username);

        return await this.userManager.GenerateUserTokenAsync(user, type, purpose);
    }

    /// <inheritdoc/>
    public async Task<UserVM> GetUserByUsernameAsync(string username)
    {
        return this.mapper.Map<UserVM>(await this.userManager.FindByNameAsync(username));
    }

    /// <inheritdoc/>
    public async Task<List<UserVM>> GetAllAdminsAsync()
    {
        var admins = await this.userManager.GetUsersInRoleAsync(UserRoles.Admin);
        
        return admins.Select(user => this.mapper.Map<UserVM>(user)).ToList();
    }
    
    /// <inheritdoc/>
    public async Task<List<UserVM>> GetAllUsersAsync()
    {
        var users = await this.userManager.GetUsersInRoleAsync(UserRoles.User);

        return users.Select(user => this.mapper.Map<UserVM>(user)).ToList();
    }

    //// <inheritdoc/>
    public async Task<IdentityResult> DeleteUserAsync(string id)
    {
        var user = await this.userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }
        
        var userRoles = await this.userManager.GetRolesAsync(user);
        if (userRoles.Any())
        {
            var removeRolesResult = await this.userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeRolesResult.Succeeded)
            {
                return removeRolesResult;
            }
        }
        this.context.Users.Remove(user);

        await context.SaveChangesAsync();
        
        return await this.userManager.DeleteAsync(user!);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetUserRolesByIdAsync(string id)
    {
        var admin = await this.userManager.FindByIdAsync(id);
        
        if (admin is null)
        {
            return [];
        }

        return (await this.userManager.GetRolesAsync(admin)).ToList();
    }
}