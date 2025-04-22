using Microsoft.AspNetCore.SignalR;
using study_buddys_backend_v2.Services;

namespace study_buddys_backend_v2.Hubs
{
    public class DirectMessageHub : Hub
    {
        private readonly UserConnectionManager _connectionManager;

        public DirectMessageHub(UserConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _connectionManager.AddConnection(userId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionManager.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            var receiverConnectionId = _connectionManager.GetConnectionId(receiverId);
            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, message);
            }
        }
    }
}
