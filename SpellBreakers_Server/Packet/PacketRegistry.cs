namespace SpellBreakers_Server.Packet
{
    public static class PacketRegistry
    {
        private static readonly Dictionary<ushort, Type> _typeById = new Dictionary<ushort, Type>
        {
            { 0, typeof(PacketBase) },
            { 1, typeof(RegisterPacket) },
            { 2, typeof(RegisterResponsePacket) },
            { 3, typeof(LoginPacket) },
            { 4, typeof(LoginResponsePacket) },
            { 5, typeof(ChatPacket) },
            { 6, typeof(MovePacket) },
        };

        public static Type GetTypeById(ushort id) => _typeById[id];
    }
}
