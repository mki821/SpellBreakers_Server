using MessagePack;

namespace SpellBreakers_Server.Packet
{
    [MessagePackObject]
    public class PacketBase
    {
        [Key(0)] public virtual ushort ID { get; set; }
    }

    [MessagePackObject]
    public class UdpPacketBase : PacketBase
    {
        [Key(1)] public string Token { get; set; } = "";
    }

    [MessagePackObject]
    public class RegisterPacket : PacketBase
    {
        public override ushort ID => 1;
        [Key(1)] public string Nickname { get; set; } = "";
        [Key(2)] public string Password { get; set; } = "";
    }

    [MessagePackObject]
    public class RegisterResponsePacket : PacketBase
    {
        public override ushort ID => 2;
        [Key(1)] public bool Success { get; set; }
        [Key(2)] public string Message { get; set; } = "";
    }

    [MessagePackObject]
    public class LoginPacket : PacketBase
    {
        public override ushort ID => 3;
        [Key(1)] public string Nickname { get; set; } = "";
        [Key(2)] public string Password { get; set; } = "";
    }

    [MessagePackObject]
    public class LoginResponsePacket : PacketBase
    {
        public override ushort ID => 4;
        [Key(1)] public bool Success { get; set; }
        [Key(2)] public string Message { get; set; } = "";
        [Key(3)] public string IssuedToken { get; set; } = "";
    }

    [MessagePackObject]
    public class MovePacket : UdpPacketBase
    {
        public override ushort ID => 5;
        [Key(2)] public float X { get; set; }
        [Key(3)] public float Y { get; set; }
    }
}
