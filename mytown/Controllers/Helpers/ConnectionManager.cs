using mytown.Controllers.Helpers;
using System.Collections.Concurrent;

namespace mytown.Helpers
{
    public class ConnectionManager
    {
        // Key: "Shopper_10" -> set of connectionIds (a user can have many tabs)
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _connections = new();

        // Key: ConnectionId
        private static readonly ConcurrentDictionary<string, ConnectionUser> _connectionUsers = new();

        private string GetKey(int userId, UserType userType) => $"{userType}_{userId}";

        public void AddConnection(int userId, UserType userType, string connectionId)
        {
            var key = GetKey(userId, userType);
            var set = _connections.GetOrAdd(key, _ => new ConcurrentDictionary<string, byte>());
            set[connectionId] = 0;

            _connectionUsers[connectionId] = new ConnectionUser
            {
                UserId = userId,
                UserType = userType
            };
        }

        // Remove ONE connection only (by connectionId)
        public void RemoveConnection(string connectionId)
        {
            if (!_connectionUsers.TryRemove(connectionId, out var user)) return;

            var key = GetKey(user.UserId, user.UserType);
            if (_connections.TryGetValue(key, out var set))
            {
                set.TryRemove(connectionId, out _);
                if (set.IsEmpty) _connections.TryRemove(key, out _);
            }
        }

        public List<string> GetConnections(int userId, UserType userType)
        {
            return _connections.TryGetValue(GetKey(userId, userType), out var set)
                ? set.Keys.ToList()
                : new List<string>();
        }

        // Kept so existing code (online check) still compiles
        public string? GetConnection(int userId, UserType userType)
        {
            return GetConnections(userId, userType).FirstOrDefault();
        }

        public ConnectionUser? GetUser(string connectionId)
        {
            _connectionUsers.TryGetValue(connectionId, out var user);
            return user;
        }
    }
}