using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    [Authorize]
    public class OrderHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Get the user ID from JWT token
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                // Add the connection to a group named after the user ID
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            // Add to general orders group
            await Groups.AddToGroupAsync(Context.ConnectionId, "orders");
            
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "orders");

            await base.OnDisconnectedAsync(exception);
        }
    }
}
