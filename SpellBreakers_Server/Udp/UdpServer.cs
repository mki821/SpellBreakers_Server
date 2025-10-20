using System.Net;
using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.Udp
{
    class UdpServer
    {
        private readonly Socket _socket;

        public UdpServer(int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public async Task StartAsync()
        {
            Console.WriteLine($"[서버] UDP {(_socket.LocalEndPoint as IPEndPoint)?.Port} 번 포트에서 시작됨!");

            byte[] buffer = new byte[1024];

            while (true)
            {
                SocketReceiveFromResult result = await _socket.ReceiveFromAsync(buffer, SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));

                PacketBase? packet = UdpPacketHelper.Deserialize(buffer, result.ReceivedBytes);
                if (packet == null) return;

                if (packet is UdpPacketBase udpPacket)
                {
                    bool isValidToken = Users.AuthenticationManager.Instance.IsValidToken(udpPacket.Token);
                    if (!isValidToken)
                    {
                        Console.WriteLine($"[서버] 유효하지 않은 토큰 : {udpPacket.Token}");

                        return;
                    }

                    User? user = UserManager.Instance.GetByToken(udpPacket.Token);
                    if (user == null)
                    {
                        Console.WriteLine($"[서버] 접속하지 않은 유저 : {udpPacket.Token}");

                        return;
                    }

                    user.UdpEndPoint = result.RemoteEndPoint;

                    PacketHandler.Handle(user.TcpSocket, packet);
                }
            }
        }
    }
}
