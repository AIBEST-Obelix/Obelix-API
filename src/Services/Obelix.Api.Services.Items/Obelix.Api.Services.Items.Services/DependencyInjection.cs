using Obelix.Api.Services.Items.Services.Contracts;
using Obelix.Api.Services.Items.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Obelix.Api.Services.Items.Data.Models;

namespace Obelix.Api.Services.Items.Services;


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
            .AddScoped<IItemService, ItemService>()
            .AddScoped<IFileService, FileService>()
            .AddScoped<IEncryptionService, EncryptionService>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped<IItemFileService, ItemFileService>();
    }
}