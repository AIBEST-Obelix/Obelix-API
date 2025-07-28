using Microsoft.EntityFrameworkCore;
using Obelix.Api.Services.Requests.Data.Data;
using Obelix.Api.Services.Requests.Data.Models;
using Obelix.Api.Services.Requests.Services.Contracts;

namespace Obelix.Api.Services.Requests.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="context">User DB Context.</param>
    public UserService(
        ApplicationDbContext context)
    {
        this.context = context;
    }

    public Task CreateUserAsync(string userId, string firstName, string lastName)
    {
        var user = new User
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName
        };

        this.context.Users.Add(user);
        return this.context.SaveChangesAsync();
    }

    public async Task<string?> GetUserByIdAsync(string userId)
    {
        var user = await this.context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == userId);
        
        if (user is null)
        {
            return null;
        }
        
        return user.Id;
    }
}