namespace SpellBreakers_Server.Packet
{
    public enum PacketId : ushort
    {
        None = 0,

        Register,
        RegisterResponse,

        Login,
        LoginResponse,

        ListRoom,
        ListRoomResponse,
        CreateRoom,
        JoinRoom,
        JoinRoomResponse,
        LeaveRoom,
        LeaveRoomResponse,

        RoomInfo,
        SwitchRole,
        SwitchRoleResponse,

        Chat,

        Move
    }
}
