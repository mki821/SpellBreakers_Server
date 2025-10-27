namespace SpellBreakers_Server.Packet
{
    public static class PacketRegistry
    {
        private static readonly Dictionary<ushort, Type> _typeById = new Dictionary<ushort, Type>
        {
            { (ushort)PacketId.None, typeof(PacketBase) },

            { (ushort)PacketId.Register, typeof(RegisterPacket) },
            { (ushort)PacketId.RegisterResponse, typeof(RegisterResponsePacket) },

            { (ushort)PacketId.Login, typeof(LoginPacket) },
            { (ushort)PacketId.LoginResponse, typeof(LoginResponsePacket) },

            { (ushort)PacketId.UdpConnect, typeof(UdpConnectPacket) },

            { (ushort)PacketId.ListRoom, typeof(ListRoomPacket) },
            { (ushort)PacketId.ListRoomResponse, typeof(ListRoomResponsePacket) },
            { (ushort)PacketId.CreateRoom, typeof(CreateRoomPacket) },
            { (ushort)PacketId.JoinRoom, typeof(JoinRoomPacket) },
            { (ushort)PacketId.JoinRoomResponse, typeof(JoinRoomResponsePacket) },
            { (ushort)PacketId.LeaveRoom, typeof(LeaveRoomPacket) },
            { (ushort)PacketId.LeaveRoomResponse, typeof(LeaveRoomResponsePacket) },

            { (ushort)PacketId.RoomInfo, typeof(RoomInfoPacket) },
            { (ushort)PacketId.SwitchRole, typeof(SwitchRolePacket) },
            { (ushort)PacketId.SwitchRoleResponse, typeof(SwitchRoleResponsePacket) },

            { (ushort)PacketId.Chat, typeof(ChatPacket) },

            { (ushort)PacketId.Move, typeof(MovePacket) },
        };

        public static Type GetTypeById(ushort id) => _typeById[id];
    }
}
