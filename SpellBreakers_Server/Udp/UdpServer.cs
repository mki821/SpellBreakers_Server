using System.Buffers;
using System.Net;
using System.Net.Sockets;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.Udp
{
    class UdpServer
    {
        private static readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public static Socket Socket => socket;

        public UdpServer(int port)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public async Task StartAsync()
        {
            Console.WriteLine($"[서버] UDP {(socket.LocalEndPoint as IPEndPoint)?.Port} 번 포트에서 시작됨!");

            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);

            while (true)
            {
                SocketReceiveFromResult result = await socket.ReceiveFromAsync(buffer, SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));

                PacketBase? packet = UdpPacketHelper.Deserialize(buffer, result.ReceivedBytes);
                if (packet == null) continue;

                if (packet is UdpPacketBase udpPacket)
                {
                    User? user = UserManager.Instance.GetByToken(udpPacket.Token);
                    if (user == null)
                    {
                        Console.WriteLine($"[서버] 접속하지 않은 유저 : {udpPacket.Token}");

                        continue;
                    }

                    user.UdpEndPoint = result.RemoteEndPoint;

                    PacketHandler.Handle(user.TcpSocket, packet);
                }
            }
        }
    }
}
