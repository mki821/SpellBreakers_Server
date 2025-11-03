using MessagePack;

namespace SpellBreakers_Server.Packet
{
    [MessagePackObject]
    public class RegisterPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.Register;
        [Key(1)] public string UserID { get; set; } = "";
        [Key(2)] public string Nickname { get; set; } = "";
        [Key(3)] public string Password { get; set; } = "";
    }

    [MessagePackObject]
    public class RegisterResponsePacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.RegisterResponse;
        [Key(1)] public bool Success { get; set; }
        [Key(2)] public string Message { get; set; } = "";
    }

    [MessagePackObject]
    public class LoginPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.Login;
        [Key(1)] public string UserID { get; set; } = "";
        [Key(2)] public string Password { get; set; } = "";
    }

    [MessagePackObject]
    public class AutoLoginPacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.AutoLogin;
        [Key(1)] public string Token { get; set; } = "";
    }

    [MessagePackObject]
    public class LoginResponsePacket : PacketBase
    {
        public override ushort ID => (ushort)PacketId.LoginResponse;
        [Key(1)] public bool Success { get; set; }
        [Key(2)] public string Message { get; set; } = "";
        [Key(3)] public string IssuedToken { get; set; } = "";
    }

    [MessagePackObject]
    public class UdpConnectPacket : UdpPacketBase
    {
        public override ushort ID => (ushort)PacketId.UdpConnect;
    }
}
