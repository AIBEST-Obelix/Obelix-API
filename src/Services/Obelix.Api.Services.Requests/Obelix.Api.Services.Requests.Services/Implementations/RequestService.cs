using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Obelix.Api.Services.Items.Services.Contracts;
using Obelix.Api.Services.Requests.Data.Data;
using Obelix.Api.Services.Requests.Data.Models;
using Obelix.Api.Services.Requests.Services.Contracts;
using Obelix.Api.Services.Requests.Shared.Models;
using Obelix.Api.Services.Shared.Enums;

namespace Obelix.Api.Services.Requests.Services.Implementations;

/// <summary>
/// Implementation of the request service.
/// </summary>
public class RequestService : IRequestService
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly ICurrentUser currentUser;
    private readonly ILogger<RequestService> logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestService"/> class.
    /// </summary>
    public RequestService(
        ApplicationDbContext context,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<RequestService> logger)
    {
        this.context = context;
        this.mapper = mapper;
        this.currentUser = currentUser;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<RequestVM?> GetRequestByIdAsync(string id)
    {
        var request = await this.context.Requests
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Item)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        
        if (request == null) 
        {
            throw new KeyNotFoundException($"Request with id {id} not found.");
        }
        
        return this.mapper.Map<RequestVM>(request);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestVM>> GetAllRequestsAsync()
    {
        var requests = await this.context.Requests
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Item)
            .Where(r => !r.IsDeleted)
            .ToListAsync();
        
        return this.mapper.Map<List<RequestVM>>(requests);
    }

    /// <inheritdoc />
    public async Task<RequestVM> CreateRequestAsync(RequestIM requestIM)
    {
        if (requestIM == null)
        {
            throw new ArgumentNullException(nameof(requestIM), "Request input model cannot be null.");
        }
        
        if (string.IsNullOrWhiteSpace(requestIM.UserId) || 
            string.IsNullOrWhiteSpace(requestIM.ItemId))
        {
            throw new ArgumentException("UserId and ItemId are required.");
        }

        // Verify that the user and item exist
        var userExists = await this.context.Users
            .AnyAsync(u => u.Id == requestIM.UserId);
        
        if (!userExists)
        {
            throw new KeyNotFoundException($"User with id {requestIM.UserId} not found.");
        }

        var itemExists = await this.context.Items
            .AnyAsync(i => i.Id == requestIM.ItemId && !i.IsDeleted);
        
        if (!itemExists)
        {
            throw new KeyNotFoundException($"Item with id {requestIM.ItemId} not found.");
        }
        
        var request = this.mapper.Map<Request>(requestIM);
        
        this.context.Requests.Add(request);
        await this.context.SaveChangesAsync();
        
        // Reload with navigation properties for the response
        var createdRequest = await this.context.Requests
            .Include(r => r.User)
            .Include(r => r.Item)
            .FirstAsync(r => r.Id == request.Id);
        
        return this.mapper.Map<RequestVM>(createdRequest);
    }

    /// <inheritdoc />
    public async Task<RequestVM?> UpdateRequestAsync(string id, RequestIM requestIM)
    {
        var request = await this.context.Requests
            .Include(r => r.User)
            .Include(r => r.Item)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        
        if (request == null) 
        {
            throw new KeyNotFoundException($"Request with id {id} not found.");
        }
        
        if (requestIM is null)
        {
            throw new ArgumentNullException(nameof(requestIM), "Request update model cannot be null.");
        }
        
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Request id cannot be null or empty.", nameof(id));
        }

        // Verify that the user and item exist if they're being updated
        if (!string.IsNullOrWhiteSpace(requestIM.UserId) && requestIM.UserId != request.UserId)
        {
            var userExists = await this.context.Users
                .AnyAsync(u => u.Id == requestIM.UserId);
            
            if (!userExists)
            {
                throw new KeyNotFoundException($"User with id {requestIM.UserId} not found.");
            }
        }

        if (!string.IsNullOrWhiteSpace(requestIM.ItemId) && requestIM.ItemId != request.ItemId)
        {
            var itemExists = await this.context.Items
                .AnyAsync(i => i.Id == requestIM.ItemId && !i.IsDeleted);
            
            if (!itemExists)
            {
                throw new KeyNotFoundException($"Item with id {requestIM.ItemId} not found.");
            }
        }
        
        mapper.Map(requestIM, request);
        
        this.context.Requests.Update(request);
        await this.context.SaveChangesAsync();
        return this.mapper.Map<RequestVM>(request);
    }

    /// <inheritdoc />
    public async Task DeleteRequestAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) 
        {
            throw new ArgumentException("Request id cannot be null or empty.", nameof(id));
        }
        
        var request = await this.context.Requests
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        
        if (request == null) 
        {
            throw new KeyNotFoundException($"Request with id {id} not found.");
        }
        
        request.IsDeleted = true; // Soft delete
        this.context.Requests.Update(request);
        await this.context.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<RequestVM>> GetDeletedRequestsAsync()
    {
        var deletedRequests = await this.context.Requests
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Item)
            .Where(r => r.IsDeleted)
            .ToListAsync();
        
        return this.mapper.Map<List<RequestVM>>(deletedRequests);
    }

    /// <inheritdoc />
    public async Task<RequestVM> ApproveRequestAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Request id cannot be null or empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(this.currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User must be authenticated to approve requests.");
        }

        var request = await this.context.Requests
            .Include(r => r.User)
            .Include(r => r.Item)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (request == null)
        {
            throw new KeyNotFoundException($"Request with id {id} not found.");
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException($"Request with id {id} is not in pending status and cannot be approved.");
        }

        request.Status = RequestStatus.Approved;
        request.ApprovedAt = DateTime.UtcNow;

        this.context.Requests.Update(request);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Request {RequestId} approved by user {UserId}", id, this.currentUser.UserId);

        return this.mapper.Map<RequestVM>(request);
    }

    /// <inheritdoc />
    public async Task<RequestVM> RejectRequestAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Request id cannot be null or empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(this.currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User must be authenticated to reject requests.");
        }

        var request = await this.context.Requests
            .Include(r => r.User)
            .Include(r => r.Item)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (request == null)
        {
            throw new KeyNotFoundException($"Request with id {id} not found.");
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException($"Request with id {id} is not in pending status and cannot be rejected.");
        }

        request.Status = RequestStatus.Rejected;
        request.RejectedAt = DateTime.UtcNow;

        this.context.Requests.Update(request);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Request {RequestId} rejected by user {UserId}", id, this.currentUser.UserId);

        return this.mapper.Map<RequestVM>(request);
    }

    /// <inheritdoc />
    public async Task<RequestVM> MarkAsReturnedAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Request id cannot be null or empty.", nameof(id));
        }

        var request = await this.context.Requests
            .Include(r => r.User)
            .Include(r => r.Item)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (request == null)
        {
            throw new KeyNotFoundException($"Request with id {id} not found.");
        }

        if (request.Status != RequestStatus.Approved)
        {
            throw new InvalidOperationException($"Request with id {id} is not approved and cannot be marked as returned.");
        }

        request.Status = RequestStatus.Returned;
        request.ReturnedAt = DateTime.UtcNow;

        this.context.Requests.Update(request);
        await this.context.SaveChangesAsync();

        return this.mapper.Map<RequestVM>(request);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestVM>> GetRequestsByStatusAsync(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status cannot be null or empty.", nameof(status));
        }

        if (!Enum.TryParse<RequestStatus>(status, true, out var requestStatus))
        {
            throw new ArgumentException($"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames<RequestStatus>())}", nameof(status));
        }

        var requests = await this.context.Requests
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Item)
            .Where(r => !r.IsDeleted && r.Status == requestStatus)
            .ToListAsync();

        return this.mapper.Map<List<RequestVM>>(requests);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestVM>> GetRequestsByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        var requests = await this.context.Requests
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Item)
            .Where(r => !r.IsDeleted && r.UserId == userId)
            .ToListAsync();

        return this.mapper.Map<List<RequestVM>>(requests);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestVM>> GetMyRequestsAsync()
    {
        if (string.IsNullOrWhiteSpace(this.currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User must be authenticated to view their requests.");
        }

        this.logger.LogInformation("Getting requests for current user {UserId}", this.currentUser.UserId);

        return await this.GetRequestsByUserIdAsync(this.currentUser.UserId);
    }
}