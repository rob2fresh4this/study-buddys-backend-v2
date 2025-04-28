using Microsoft.AspNetCore.SignalR;

namespace study_buddys_backend_v2.Hubs
{
    public class CommunityHub : Hub
    {
        public async Task JoinCommunity(string communityId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Community-{communityId}");
        }

        public async Task LeaveCommunity(string communityId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Community-{communityId}");
        }
    }
}