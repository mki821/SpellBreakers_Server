namespace SpellBreakers_Server.Packet
{
    public enum PacketId : ushort
    {
        None = 0,

        Register,
        RegisterResponse,

        Login,
        LoginResponse,

        Move
    }
}
