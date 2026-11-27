using mytown.Controllers.Helpers;
using System.Collections.Concurrent;

namespace mytown.Helpers
{
    public class ConnectionManager
    {
        // Key: "Shopper_10", "Business_5"
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        // Key: ConnectionId
        private static readonly ConcurrentDictionary<string, ConnectionUser> _connectionUsers = new();

        private string GetKey(int userId, UserType userType)
        {
            return $"{userType}_{userId}";
        }

        public void AddConnection(int userId, UserType userType, string connectionId)
        {
            var key = GetKey(userId, userType);

            _connections[key] = connectionId;

            _connectionUsers[connectionId] = new ConnectionUser
            {
                UserId = userId,
                UserType = userType
            };
        }

        public void RemoveConnection(int userId, UserType userType)
        {
            var key = GetKey(userId, userType);

            if (_connections.TryRemove(key, out var connectionId))
            {
                _connectionUsers.TryRemove(connectionId, out _);
            }
        }

        public string? GetConnection(int userId, UserType userType)
        {
            var key = GetKey(userId, userType);

            _connections.TryGetValue(key, out var connectionId);

            return connectionId;
        }

        public ConnectionUser? GetUser(string connectionId)
        {
            _connectionUsers.TryGetValue(connectionId, out var user);

            return user;
        }
    }
}