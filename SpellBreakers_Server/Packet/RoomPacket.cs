using MessagePack;

namespace SpellBreakers_Server.Packet
{
    [MessagePackObject]
    public class ListRoomPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.ListRoom;
    }
    [MessagePackObject]
    public class ListRoomResponsePacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.ListRoomResponse;
        [Key(1)] public List<RoomElement> Rooms { get; set; } = new List<RoomElement>();
    }

    [MessagePackObject]
    public class RoomElement
    {
        [Key(0)] public ushort ID { get; set; }
        [Key(1)] public string Name { get; set; } = "";
        [Key(2)] public bool Locked { get; set; }
        [Key(3)] public bool Playing { get; set; }
        [Key(4)] public ushort Players { get; set; }
        [Key(5)] public ushort Spectators { get; set; }
    }

    [MessagePackObject]
    public class CreateRoomPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.CreateRoom;
        [Key(1)] public string Name { get; set; } = "";
        [Key(2)] public string Password { get; set; } = "";
    }

    [MessagePackObject]
    public class JoinRoomPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.JoinRoom;
        [Key(1)] public ushort RoomID { get; set; }
        [Key(2)] public string Password { get; set; } = "";
    }

    [MessagePackObject]
    public class JoinRoomResponsePacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.JoinRoomResponse;
        [Key(1)] public bool Success { get; set; }
        [Key(2)] public string Message { get; set; } = "";
    }

    [MessagePackObject]
    public class LeaveRoomPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.LeaveRoom;
    }

    [MessagePackObject]
    public class LeaveRoomResponsePacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.LeaveRoomResponse;
        [Key(1)] public bool Success { get; set; }
        [Key(2)] public string Message { get; set; } = "";
    }

    [MessagePackObject]
    public class RoomInfoPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.RoomInfo;
        [Key(1)] public List<UserElement> Players { get; set; } = new List<UserElement>();
        [Key(2)] public List<UserElement> Spectators { get; set; } = new List<UserElement>();
    }

    [MessagePackObject]
    public class UserElement
    {
        [Key(0)] public string Nickname { get; set; } = "";
    }

    [MessagePackObject]
    public class ChatPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.Chat;
        [Key(1)] public string Sender { get; set; } = "";
        [Key(2)] public string Message { get; set; } = "";
    }

    [MessagePackObject]
    public class SwitchRolePacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.SwitchRole;
    }

    [MessagePackObject]
    public class SwitchRoleResponsePacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.SwitchRoleResponse;
        [Key(1)] public bool Success { get; set; }
        [Key(2)] public string Message { get; set; } = "";
    }

    [MessagePackObject]
    public class MovePacket : UdpPacketBase
    {
        public override ushort ID => (ushort)PacketId.Move;
        [Key(2)] public float X { get; set; }
        [Key(3)] public float Y { get; set; }
    }
}
