using System.Collections.Concurrent;
using System.Net.Sockets;

namespace SpellBreakers_Server.Users
{
    public class UserManager
    {
        private static Lazy<UserManager> _instance = new Lazy<UserManager>(() => new UserManager());
        public static UserManager Instance => _instance.Value;

        private readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

        private UserManager() { }

        public void AddOrUpdate(User user)
        {
            _users[user.Token] = user;
        }

        public void Remove(string token)
        {
            _users.TryRemove(token, out _);
        }

        public User? GetByToken(string token)
        {
            _users.TryGetValue(token, out User? user);
            return user;
        }

        public User? GetBySocket(Socket socket)
        {
            foreach(User user in _users.Values)
            {
                if (user.TcpSocket == socket)
                {
                    return user;
                }
            }

            return null;
        }
    }
}
