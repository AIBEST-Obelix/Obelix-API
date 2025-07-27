using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Obelix.Api.Services.Requests.Services.Contracts;
using Obelix.Api.Services.Requests.Shared.Models;
using Obelix.Api.Services.Shared.Data.Models.Identity;
using System.Net.Mime;

namespace Obelix.Api.Services.Requests.WebHost.Controllers;

/// <summary>
/// Controller for managing request-related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
public class RequestController : ControllerBase
{
    private readonly ILogger<RequestController> logger;
    private readonly IRequestService requestService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestController"/> class.
    /// </summary>
    public RequestController(
        IRequestService requestService,
        ILogger<RequestController> logger)
    {
        this.requestService = requestService;
        this.logger = logger;
    }

    /// <summary>
    /// Gets a request by its ID.
    /// </summary>
    /// <param name="id">The ID of the request.</param>
    /// <returns>The request with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRequestByIdAsync(string id)
    {
        try
        {
            var request = await this.requestService.GetRequestByIdAsync(id);
            return Ok(request);
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Request not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting request by ID: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Gets all requests.
    /// </summary>
    [HttpGet]
    [Authorize(Policy=UserPolicies.AdminPermissions)]
    public async Task<IActionResult> GetAllRequestsAsync()
    {
        try
        {
            var requests = await this.requestService.GetAllRequestsAsync();
            return Ok(requests);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting all requests.");
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Gets requests by status.
    /// </summary>
    /// <param name="status">The status to filter by.</param>
    /// <returns>List of requests with the specified status.</returns>
    [HttpGet("status/{status}")]
    [Authorize(Policy=UserPolicies.AdminPermissions)]
    public async Task<IActionResult> GetRequestsByStatusAsync(string status)
    {
        try
        {
            var requests = await this.requestService.GetRequestsByStatusAsync(status);
            return Ok(requests);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid status provided: {Status}", status);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting requests by status: {Status}", status);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Gets requests by user ID.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <returns>List of requests for the specified user.</returns>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetRequestsByUserIdAsync(string userId)
    {
        try
        {
            var requests = await this.requestService.GetRequestsByUserIdAsync(userId);
            return Ok(requests);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid user ID provided: {UserId}", userId);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting requests by user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Gets requests for the current authenticated user.
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequestsAsync()
    {
        try
        {
            var requests = await this.requestService.GetMyRequestsAsync();
            return Ok(requests);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting requests for the current user.");
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Gets deleted requests.
    /// </summary>
    [HttpGet("deleted")]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> GetDeletedRequestsAsync()
    {
        try
        {
            var requests = await this.requestService.GetDeletedRequestsAsync();
            return Ok(requests);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while getting deleted requests.");
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Creates a new request.
    /// </summary>
    /// <param name="requestIM">The request to create.</param>
    /// <returns>The created request.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateRequestAsync([FromBody] RequestIM requestIM)
    {
        if (requestIM == null)
        {
            return BadRequest(new { Message = "Request data is required." });
        }

        try
        {
            var createdRequest = await this.requestService.CreateRequestAsync(requestIM);
            return CreatedAtAction(nameof(GetRequestByIdAsync), new { id = createdRequest.Id }, createdRequest);
        }
        catch (ArgumentNullException ex)
        {
            this.logger.LogWarning(ex, "Invalid request data provided.");
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid request data provided.");
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Referenced entity not found.");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while creating request.");
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Updates a request by its ID.
    /// </summary>
    /// <param name="id">The ID of the request to update.</param>
    /// <param name="requestIM">The updated request data.</param>
    /// <returns>A response indicating the result of the update.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRequestAsync(string id, [FromBody] RequestIM requestIM)
    {
        if (requestIM == null)
        {
            return BadRequest(new { Message = "Request data is required." });
        }
        
        try
        {
            var updatedRequest = await this.requestService.UpdateRequestAsync(id, requestIM);
            return Ok(updatedRequest);
        }
        catch (ArgumentNullException ex)
        {
            this.logger.LogWarning(ex, "Invalid request data provided.");
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid request data provided.");
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Request not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while updating request: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Deletes a request by its ID.
    /// </summary>
    /// <param name="id">The ID of the request to delete.</param>
    /// <returns>A response indicating the result of the deletion.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> DeleteRequestAsync(string id)
    {
        try
        {
            await this.requestService.DeleteRequestAsync(id);
            return Ok(new { Message = "Request deleted successfully." });
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid request ID provided: {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Request not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while deleting request: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Approves a request by its ID.
    /// </summary>
    /// <param name="id">The ID of the request to approve.</param>
    /// <returns>The approved request.</returns>
    [HttpPatch("{id}/approve")]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> ApproveRequestAsync(string id)
    {
        try
        {
            var approvedRequest = await this.requestService.ApproveRequestAsync(id);
            return Ok(approvedRequest);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid request ID provided: {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Request not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogWarning(ex, "Cannot approve request: {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while approving request: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Rejects a request by its ID.
    /// </summary>
    /// <param name="id">The ID of the request to reject.</param>
    /// <returns>The rejected request.</returns>
    [HttpPatch("{id}/reject")]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> RejectRequestAsync(string id)
    {
        try
        {
            var rejectedRequest = await this.requestService.RejectRequestAsync(id);
            return Ok(rejectedRequest);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid request ID provided: {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Request not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogWarning(ex, "Cannot reject request: {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while rejecting request: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }

    /// <summary>
    /// Marks a request as returned by its ID.
    /// </summary>
    /// <param name="id">The ID of the request to mark as returned.</param>
    /// <returns>The request marked as returned.</returns>
    [HttpPatch("{id}/return")]
    [Authorize(Policy = UserPolicies.AdminPermissions)]
    public async Task<IActionResult> MarkAsReturnedAsync(string id)
    {
        try
        {
            var returnedRequest = await this.requestService.MarkAsReturnedAsync(id);
            return Ok(returnedRequest);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogWarning(ex, "Invalid request ID provided: {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, "Request not found: {Id}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogWarning(ex, "Cannot mark request as returned: {Id}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while marking request as returned: {Id}", id);
            return StatusCode(500, new { Message = "An error occurred while processing your request." });
        }
    }
}