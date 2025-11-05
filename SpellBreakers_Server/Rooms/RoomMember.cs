using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.Rooms
{
    public class RoomMember
    {
        public User User { get; }
        public bool IsReady { get; set; }

        public RoomMember(User user)
        {
            User = user;
        }
    }
}
