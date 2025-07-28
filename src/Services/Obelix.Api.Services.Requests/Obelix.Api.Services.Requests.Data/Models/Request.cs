using System.ComponentModel.DataAnnotations;
using Obelix.Api.Services.Shared.Data.Interfaces;
using Obelix.Api.Services.Shared.Enums;

namespace Obelix.Api.Services.Requests.Data.Models;

public class Request : ISoftDelete
{
    [MaxLength(256)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [MaxLength(256)]
    public string UserId { get; set; } = string.Empty;
    
    [MaxLength(256)]
    public string ItemId { get; set; } = string.Empty;
    
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;
    
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public bool IsDeleted { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; } = null;
    public DateTime? RejectedAt { get; set; } = null;
    public DateTime? ReturnedAt { get; set; } = null;
    
    
    public User? User { get; set; } = null;
    public Item? Item { get; set; } = null;
    
}