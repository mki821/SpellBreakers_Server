using SpellBreakers_Server.Packet;
using SpellBreakers_Server.PacketHandlers.Auth;
using SpellBreakers_Server.PacketHandlers.Games;
using SpellBreakers_Server.PacketHandlers.Rooms;
using SpellBreakers_Server.Tcp;
using SpellBreakers_Server.Udp;

namespace SpellBreakers_Server
{
    public class Server
    {
        private readonly int _port;

        public Server(int port)
        {
            _port = port;

            PacketHandler.Register(PacketId.Register, new RegisterHandler());
            PacketHandler.Register(PacketId.Login, new LoginHandler());
            PacketHandler.Register(PacketId.AutoLogin, new AutoLoginHandler());
            PacketHandler.Register(PacketId.UdpConnect, new UdpConnectHandler());

            PacketHandler.Register(PacketId.ListRoom, new ListRoomHandler());
            PacketHandler.Register(PacketId.CreateRoom, new CreateRoomHandler());
            PacketHandler.Register(PacketId.JoinRoom, new JoinRoomHandler());
            PacketHandler.Register(PacketId.LeaveRoom, new LeaveRoomHandler());

            PacketHandler.Register(PacketId.Chat, new ChatHandler());
            PacketHandler.Register(PacketId.SwitchRole, new SwitchRoleHnadler());

            PacketHandler.Register(PacketId.Ready, new ReadyHandler());

            PacketHandler.Register(PacketId.Move, new MoveHandler());
            PacketHandler.Register(PacketId.Skill, new SkillHandler());
        }

        public async Task StartAsync()
        {
            TcpServer tcpServer = new TcpServer(_port);
            UdpServer udpServer = new UdpServer(_port + 1);

            _ = tcpServer.StartAsync();
            _ = udpServer.StartAsync();

            await Task.Delay(-1);
        }
    }
}
