namespace Obelix.Api.Services.Items.Services.Contracts;

public interface ICurrentUser
{
    public string UserId { get; }

    public string UserRole { get; }
}