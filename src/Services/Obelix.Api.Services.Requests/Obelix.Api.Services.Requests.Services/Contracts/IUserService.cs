namespace Obelix.Api.Services.Requests.Services.Contracts;

public interface IUserService
{
    Task CreateUserAsync(string userId, string firstName, string lastName);
    
    Task<string?> GetUserByIdAsync(string userId);
}