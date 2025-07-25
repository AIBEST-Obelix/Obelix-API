using System.ComponentModel.DataAnnotations;

namespace Obelix.Api.Services.Requests.Data.Models;

public class User
{
    [MaxLength(256)]
    public string Id { get; set; } = null!;
    [MaxLength(256)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(256)]
    public string LastName { get; set; } = string.Empty;
}