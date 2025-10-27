namespace SpellBreakers_Server.Packet
{
    public enum PacketId : ushort
    {
        None = 0,

        Register,
        RegisterResponse,

        Login,
        LoginResponse,

        UdpConnect,

        ListRoom,
        ListRoomResponse,
        CreateRoom,
        JoinRoom,
        JoinRoomResponse,
        LeaveRoom,
        LeaveRoomResponse,

        RoomInfo,
        Chat,
        SwitchRole,
        SwitchRoleResponse,

        Move
    }
}
