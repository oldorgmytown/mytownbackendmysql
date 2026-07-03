using System.Collections.Concurrent;

namespace mytown.Helpers
{
    public class ConnectionManager
    {
        private static readonly ConcurrentDictionary<int, string> _connections = new();

        public void AddConnection(int shopperRegId, string connectionId)
        {
            _connections[shopperRegId] = connectionId;
        }

        public void RemoveConnection(int shopperRegId)
        {
            _connections.TryRemove(shopperRegId, out _);
        }

        public string? GetConnection(int shopperRegId)
        {
            _connections.TryGetValue(shopperRegId, out var connectionId);
            return connectionId;
        }

        public int? GetShopperId(string connectionId)
        {
            var item = _connections.FirstOrDefault(x => x.Value == connectionId);

            if (item.Equals(default(KeyValuePair<int, string>)))
                return null;

            return item.Key;
        }
    }
}