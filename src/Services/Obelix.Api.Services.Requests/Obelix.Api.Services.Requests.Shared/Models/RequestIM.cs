using System.ComponentModel.DataAnnotations;
using Obelix.Api.Services.Shared.Enums;

namespace Obelix.Api.Services.Requests.Shared.Models;

public class RequestIM
{
    [Required]
    [MaxLength(256)]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(256)]
    public string ItemId { get; set; } = string.Empty;
    
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
}