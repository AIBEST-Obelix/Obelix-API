using Obelix.Api.Services.Requests.Shared.Models;

namespace Obelix.Api.Services.Requests.Services.Contracts;

/// <summary>
/// Interface for request service.
/// </summary>
public interface IRequestService
{
    /// <summary>
    /// Get request by id.
    /// </summary>
    /// <param name="id">Id of the request.</param>
    /// <returns>Request.</returns>
    Task<RequestVM?> GetRequestByIdAsync(string id);

    /// <summary>
    /// Get all requests.
    /// </summary>
    /// <returns>List of requests.</returns>
    Task<IEnumerable<RequestVM>> GetAllRequestsAsync();

    /// <summary>
    /// Create a new request.
    /// </summary>
    /// <param name="request">Request to create.</param>
    /// <returns>Created request.</returns>
    Task<RequestVM> CreateRequestAsync(RequestIM request);

    /// <summary>
    /// Update an existing request.
    /// </summary>
    /// <param name="id">Id of the request to update.</param>
    /// <param name="request">Updated request data.</param>
    /// <returns>Updated request.</returns>
    Task<RequestVM?> UpdateRequestAsync(string id, RequestIM request);

    /// <summary>
    /// Delete a request.
    /// </summary>
    /// <param name="id">Id of the request to delete.</param>
    /// <returns>True if deleted successfully.</returns>
    Task DeleteRequestAsync(string id);

    /// <summary>
    /// Get deleted requests.
    /// </summary>
    /// <returns>List of deleted requests.</returns>
    Task<IEnumerable<RequestVM>> GetDeletedRequestsAsync();

    /// <summary>
    /// Approve a request.
    /// </summary>
    /// <param name="id">Id of the request to approve.</param>
    /// <returns>Updated request.</returns>
    Task<RequestVM> ApproveRequestAsync(string id);

    /// <summary>
    /// Reject a request.
    /// </summary>
    /// <param name="id">Id of the request to reject.</param>
    /// <returns>Updated request.</returns>
    Task<RequestVM> RejectRequestAsync(string id);

    /// <summary>
    /// Mark a request as returned.
    /// </summary>
    /// <param name="id">Id of the request to mark as returned.</param>
    /// <returns>Updated request.</returns>
    Task<RequestVM> MarkAsReturnedAsync(string id);

    /// <summary>
    /// Get requests by status.
    /// </summary>
    /// <param name="status">Status to filter by.</param>
    /// <returns>List of requests with the specified status.</returns>
    Task<IEnumerable<RequestVM>> GetRequestsByStatusAsync(string status);

    /// <summary>
    /// Get requests by user id.
    /// </summary>
    /// <param name="userId">Id of the user to get requests for.</param>
    /// <returns>List of requests for the specified user.</returns>
    Task<IEnumerable<RequestVM>> GetRequestsByUserIdAsync(string userId);

    /// <summary>
    /// Gets requests for the current authenticated user.
    /// </summary>
    /// <returns>List of requests for the current user.</returns>
    Task<IEnumerable<RequestVM>> GetMyRequestsAsync();

    /// <summary>
    /// Gets request count by month for analytics.
    /// </summary>
    /// <param name="year">Year to get analytics for.</param>
    /// <returns>Dictionary with month names as keys and request counts as values.</returns>
    Task<Dictionary<string, int>> GetRequestCountByMonthAsync(int year);
}