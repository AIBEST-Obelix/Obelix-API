using Obelix.Api.Services.Identity.Services.Contracts;
using Obelix.Api.Services.Identity.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Obelix.Api.EventBus.Abstraction.Abstractions;
using Obelix.Api.EventBus.Shared.Services.Contracts;
using Obelix.Api.EventBus.Shared.Services.Implementations;
using Obelix.Api.Services.Identity.Services.Contracts;
using Obelix.Api.Services.Identity.Services.Implementations;

namespace Obelix.Api.Services.Identity.Services;


/// <summary>
/// Static class for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add Services.
    /// </summary>
    /// <param name="services">Services.</param>
    public static void AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped<ITokenService, TokenService>();
    }
}