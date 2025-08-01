using Obelix.Api.Services.Identity.Data.Models.Identity;
using Obelix.Api.Services.Identity.Services.Contracts;
using Obelix.Api.Services.Identity.Shared.Models.User;
using Obelix.Api.Services.Shared.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Obelix.Api.EventBus.Shared.Services.Contracts;
using Obelix.Api.Services.Shared.IntegrationEvent;

namespace Obelix.Api.Services.Identity.Services.Implementations;


/// <summary>
/// Class that implements <see cref="IAuthService"/>.
/// </summary>
internal class AuthService : IAuthService
{
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IIntegrationEventService? integrationEventService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userManager">User manager.</param>
    /// <param name="roleManager">Role manager.</param>
    /// <param name="integrationEventService">Integration event service (optional).</param>
    public AuthService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IIntegrationEventService? integrationEventService = null)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.integrationEventService = integrationEventService;
    }
    
    /// <inheritdoc/>
    public async Task<bool> CheckIfUserExistsAsync(string email)
    {
        return await this.userManager.FindByNameAsync(email) != null;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIsPasswordCorrectAsync(string email, string password)
    {
        var user = await this.userManager.FindByEmailAsync(email);

        return !(user is null || !await this.userManager.CheckPasswordAsync(user, password));
    }

    public async Task<Tuple<bool, string?>> CreateUserAsync(UserIM userIm)
    {
        User user = new ()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = userIm.Email,
            Email = userIm.Email,
            FirstName = userIm.FirstName,
            LastName = userIm.LastName
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

        // Publish UserCreatedIntegrationEvent if service is available
        if (this.integrationEventService != null)
        {
            var userCreatedEvent = new UserCreatedIntegrationEvent(
                user.Id,
                user.FirstName,
                user.LastName
            );
            await this.integrationEventService.PublishThroughEventBusAsync(userCreatedEvent);
        }

        return new (true, null);
    }

    /// <inheritdoc/>
    public async Task<Tuple<bool, string?>> CreateAdminAsync(UserIM userIm)
    {
        User admin = new()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = userIm.Email,
            Email = userIm.Email,
            FirstName = userIm.FirstName,
            LastName = userIm.LastName
        };

        var result = await this.userManager.CreateAsync(admin, userIm.Password);

        if (!result.Succeeded)
        {
            return new(false, result.Errors.FirstOrDefault()?.Description);
        }

        if (!await this.roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await this.roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }

        if (await this.roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await this.userManager.AddToRoleAsync(admin, UserRoles.Admin);
        }

        // Publish UserCreatedIntegrationEvent if service is available
        if (this.integrationEventService != null)
        {
            var userCreatedEvent = new UserCreatedIntegrationEvent(
                admin.Id,
                admin.FirstName,
                admin.LastName
            );
            await this.integrationEventService.PublishThroughEventBusAsync(userCreatedEvent);
        }

        return new(true, null);
    }
}