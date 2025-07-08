using Obelix.Api.Services.Identity.Data.Models.Identity;
using Obelix.Api.Services.Identity.Services.Contracts;
using Obelix.Api.Services.Identity.Shared.Models.User;
using Obelix.Api.Services.Shared.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Obelix.Api.Services.Identity.Services.Implementations;


/// <summary>
/// Class that implements <see cref="IAuthService"/>.
/// </summary>
internal class AuthService : IAuthService
{
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userManager">User manager.</param>
    /// <param name="roleManager">Role manager.</param>
    public AuthService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }
    
    /// <inheritdoc/>
    public async Task<bool> CheckIfUserExistsAsync(string username)
    {
        return await this.userManager.FindByNameAsync(username) != null;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIsPasswordCorrectAsync(string username, string password)
    {
        var user = await this.userManager.FindByNameAsync(username);

        return !(user is null || !await this.userManager.CheckPasswordAsync(user, password));
    }

    public async Task<Tuple<bool, string?>> CreateUserAsync(UserIM userIm)
    {
        User user = new ()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = userIm.UserName,
        };

        var result = await this.userManager.CreateAsync(user, userIm.Password);

        if (!result.Succeeded)
        {
            return new (false, result.Errors.FirstOrDefault()?.Description);
        }
        
        if (!await this.roleManager.RoleExistsAsync(UserRoles.User))
        {
            await this.roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }

        if (await this.roleManager.RoleExistsAsync(UserRoles.User))
        {
            await this.userManager.AddToRoleAsync(user, UserRoles.User);
        }

        return new (true, null);
    }

    /// <inheritdoc/>
    public async Task<Tuple<bool, string?>> CreateAdminAsync(UserIM userIm)
    {
        User admin = new ()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = userIm.UserName,
        };

        var result = await this.userManager.CreateAsync(admin, userIm.Password);

        if (!result.Succeeded)
        {
            return new (false, result.Errors.FirstOrDefault()?.Description);
        }
        
        if (!await this.roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await this.roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }

        if (await this.roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await this.userManager.AddToRoleAsync(admin, UserRoles.Admin);
        }

        return new (true, null);
    }

    /// <inheritdoc/>
    public async Task<Tuple<bool, string?>> CreateAccountantAsync(UserIM userIm)
    {
        User accountant = new ()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = userIm.UserName,
        };

        var result = await this.userManager.CreateAsync(accountant, userIm.Password);

        if (!result.Succeeded)
        {
            return new (false, result.Errors.FirstOrDefault()?.Description);
        }
        
        if (!await this.roleManager.RoleExistsAsync(UserRoles.Accountant))
        {
            await this.roleManager.CreateAsync(new IdentityRole(UserRoles.Accountant));
        }

        if (await this.roleManager.RoleExistsAsync(UserRoles.Accountant))
        {
            await this.userManager.AddToRoleAsync(accountant, UserRoles.Accountant);
        }

        return new (true, null);
    }
}