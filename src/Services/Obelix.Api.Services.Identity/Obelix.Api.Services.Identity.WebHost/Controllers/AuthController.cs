using System.IdentityModel.Tokens.Jwt;
using Obelix.Api.Services.Identity.Services.Contracts;
using Obelix.Api.Services.Identity.Shared.Models;
using Obelix.Api.Services.Identity.Shared.Models.Token;
using Obelix.Api.Services.Identity.Shared.Models.User;
using Obelix.Api.Services.Shared.Data.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Obelix.Api.Services.Identity.WebHost.Controllers;

/// <summary>
/// Controller for user authentication.
/// </summary>
[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly ITokenService tokenService;
    private readonly ICurrentUser currentUser;
    private readonly IConfiguration configuration;
    private readonly ILogger<AuthController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Authentication Service.</param>
    /// <param name="tokenService">Token Service.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="currentUser">Current User.</param>
    /// <param name="logger">Logger.</param>
    public AuthController(
        IAuthService authService,
        ITokenService tokenService,
        IConfiguration configuration,
        ICurrentUser currentUser,
        ILogger<AuthController> logger)
    {
        this.authService = authService;
        this.tokenService = tokenService;
        this.configuration = configuration;
        this.currentUser = currentUser;
        this.logger = logger;
    }
    
    /// <summary>
    /// Route to log in a user.
    /// </summary>
    /// <param name="userIm">Username and Password.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginIM userIm)
    {
        this.logger.LogInformation("User with username {Email} is trying to login.", userIm.Email);

        if (!await this.authService.CheckIfUserExistsAsync(userIm.Email))
        {
            this.logger.LogWarning("User with username {Email} does not exist.", userIm.Email);
            return this.BadRequest(new ResponseModel() { Status = "login-failed", Message = "Invalid Email." });
        }
        
        if (!await this.authService.CheckIsPasswordCorrectAsync(userIm.Email, userIm.Password))
        {
            this.logger.LogWarning("User with username {Email} has entered an incorrect password.", userIm.Email);
            return this.BadRequest(new ResponseModel { Status = "login-failed", Message = "Invalid Password." });
        }

        var tokens = await this.tokenService.CreateTokensForUserAsync(userIm.Email);

        this.logger.LogInformation("User with email {Email} has logged in successfully.", userIm.Email);

        return this.Ok(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(tokens.AccessToken),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(tokens.RefreshToken),
            Expiration = tokens.AccessToken!.ValidTo,
        });
    }
    
    /// <summary>
    /// Register an admin.
    /// </summary>
    /// <param name="userIm">Register model.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [Route("register/admin")]
    public async Task<ActionResult<ResponseModel>> RegisterAsync([FromBody] UserIM userIm)
    {
        this.logger.LogInformation("User with username {Email} is trying to register as an admin.", userIm.Email);

        if (await this.authService.CheckIfUserExistsAsync(userIm.Email))
        {
            this.logger.LogWarning("User with username {Email} already exists.", userIm.Email);
            return this.Conflict(new ResponseModel { Status = "register-failed", Message = "User already exists." });
        }

        var response = await this.authService.CreateAdminAsync(userIm);

        if (!response.Item1)
        {
            this.logger.LogWarning("User with username {Email} failed to register. Reason: {Reason}", userIm.Email, response.Item2);
            return this.BadRequest(new ResponseModel { Status = "register-failed", Message = response.Item2 });
        }
        
        this.logger.LogInformation("User with username {Email} has registered successfully.", userIm.Email);

        return this.Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
    }
    
    /// <summary>
    /// Registers a user.
    /// </summary>
    /// <param name="userIm">User model.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("register/user")]
    public async Task<ActionResult<ResponseModel>> RegisterUserAsync([FromBody] UserIM userIm)
    {
        this.logger.LogInformation("User with username {Email} is trying to register as a user.", userIm.Email);

        if (await this.authService.CheckIfUserExistsAsync(userIm.Email))
        {
            this.logger.LogWarning("User with username {Email} already exists.", userIm.Email);
            return this.Conflict(new ResponseModel { Status = "register-failed", Message = "User already exists." });
        }

        var response = await this.authService.CreateUserAsync(userIm);

        if (!response.Item1)
        {
            this.logger.LogWarning("User with username {Email} failed to register as a user. Reason: {Reason}", userIm.Email, response.Item2);
            return this.BadRequest(new ResponseModel { Status = "register-failed", Message = response.Item2 });
        }
        
        this.logger.LogInformation("User with username {Email} has registered as a user successfully.", userIm.Email);

        return this.Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
    }

    /// <summary>
    /// Renews a access token.
    /// </summary>
    /// <param name="tokensIM">Token model.</param>
    /// <returns>Response with the result.</returns>
    [HttpPost]
    [Route("renew")]
    public async Task<IActionResult> RenewAsync([FromBody] TokensIM tokensIM)
    {
        if (tokensIM is null)
        {
            return this.Unauthorized(new ResponseModel { Status = "renew-failed", Message = "No token provided." });
        }

        var tokens = await this.tokenService.CreateNewTokensAsync(tokensIM);

        if (tokens.AccessToken is null)
        {
            return this.BadRequest(new ResponseModel { Status = "renew-failed", Message = "Invalid token." });
        }

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(tokens.AccessToken),
            refreshToken = new JwtSecurityTokenHandler().WriteToken(tokens.RefreshToken),
            Expiration = tokens.AccessToken!.ValidTo,
        });
    }
}