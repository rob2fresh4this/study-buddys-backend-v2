

namespace study_buddys_backend_v2.Services
{
    public class UserConnectionManager
    {
        private static readonly Dictionary<string, string> _connections = new();

        public void AddConnection(string userId, string connectionId)
        {
            _connections[userId] = connectionId;
        }

        public void RemoveConnection(string connectionId)
        {
            var user = _connections.FirstOrDefault(x => x.Value == connectionId);
            if (!string.IsNullOrEmpty(user.Key))
            {
                _connections.Remove(user.Key);
            }
        }

        public string? GetConnectionId(string userId)
        {
            return _connections.TryGetValue(userId, out var connectionId) ? connectionId : null;
        }
    }
}
