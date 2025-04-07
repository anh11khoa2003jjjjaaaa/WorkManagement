namespace WorkManagement.Hubs
{
    using System.Collections.Concurrent;

    public interface IUserConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string connectionId);
        List<string> GetConnections(string userId);
    }

    public class UserConnectionManager : IUserConnectionManager
    {
        private readonly ConcurrentDictionary<string, List<string>> _userConnections = new();

        public void AddConnection(string userId, string connectionId)
        {
            if (!_userConnections.ContainsKey(userId))
            {
                _userConnections[userId] = new List<string>();
            }

            _userConnections[userId].Add(connectionId);
        }

        public void RemoveConnection(string connectionId)
        {
            foreach (var connections in _userConnections.Values)
            {
                connections.Remove(connectionId);
            }
        }

        public List<string> GetConnections(string userId)
        {
            return _userConnections.ContainsKey(userId) ? _userConnections[userId] : new List<string>();
        }
    }
}