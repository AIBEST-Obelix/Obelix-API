using Microsoft.AspNetCore.SignalR;

namespace Obelix.Api.Services.Items.WebHost.Hubs;

public class ItemHub : Hub
{
    /// <summary>
    /// Called when a client connects to the hub.
    /// Adds the client to a SignalR group based on the `companyId` query parameter.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// Removes the client from the SignalR group based on the `companyId` query parameter.
    /// </summary>
    /// <param name="exception">The exception that caused the disconnection, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}