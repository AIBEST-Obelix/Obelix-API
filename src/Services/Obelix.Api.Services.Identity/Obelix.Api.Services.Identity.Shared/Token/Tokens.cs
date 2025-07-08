using System.IdentityModel.Tokens.Jwt;

namespace Obelix.Api.Services.Identity.Shared.Models.Token;

/// <summary>
/// Tokens for the users.
/// </summary>
public class Tokens
{
    /// <summary>
    /// Gets or sets access token.
    /// </summary>
    public JwtSecurityToken? AccessToken { get; set; } = new ();

    /// <summary>
    /// Gets or sets refresh token.
    /// </summary>
    public JwtSecurityToken? RefreshToken { get; set; } = new ();
}