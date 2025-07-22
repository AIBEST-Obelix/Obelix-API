using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Obelix.Api.Services.Items.Services.Contracts;

namespace Obelix.Api.Services.Items.Services.Implementations;

/// <summary>
/// Current user.
/// </summary>
class CurrentUser : ICurrentUser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUser"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">HTTP Context Accessors.</param>
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        this.UserId = httpContextAccessor?
            .HttpContext?
            .User
            .Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
            .Value!;

        this.UserRole = httpContextAccessor?
            .HttpContext?
            .User
            .Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?
            .Value!;
    }

    /// <inheritdoc/>
    public string UserId { get; }

    /// <inheritdoc/>
    public string UserRole { get; }
}