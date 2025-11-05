namespace SpellBreakers_Server.Packet
{
    public enum PacketId : ushort
    {
        None = 0,

        Register,
        RegisterResponse,

        Login,
        AutoLogin,
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

        Ready,
        ReadyResponse,
        StartGame,

        Move
    }
}
