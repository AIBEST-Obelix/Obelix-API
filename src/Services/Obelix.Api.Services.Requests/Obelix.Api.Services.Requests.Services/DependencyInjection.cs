using Microsoft.Extensions.DependencyInjection;
using Obelix.Api.Services.Items.Services.Contracts;
using Obelix.Api.Services.Requests.Services.Contracts;
using Obelix.Api.Services.Requests.Services.Implementations;

namespace Obelix.Api.Services.Requests.Services;

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
            .AddScoped<IRequestService, RequestService>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IItemService, ItemService>();
    }
}