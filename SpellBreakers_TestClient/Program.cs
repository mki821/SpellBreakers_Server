using System.Buffers;
using System.Net;
using System.Net.Sockets;
using MessagePack;

namespace SpellBreakers_TestClient
{
    internal class Program
    {
        public static async Task Main()
        {
            Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await tcpSocket.ConnectAsync("127.0.0.1", 5050);

            _ = Task.Run(() => HandleListen(tcpSocket));

            Console.ReadLine();

            LoginPacket packet = new LoginPacket
            {
                Nickname = "Test01",
                Password = "Test01!"
            };

            await SendAsync(tcpSocket, packet);

            Console.ReadLine();
        }

        private static async Task HandleListen(Socket socket)
        {
            byte[] buffer = new byte[1024];
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            await udpSocket.ConnectAsync("127.0.0.1", 5051);

            while (true)
            {
                PacketBase? packet = await ReceiveAsync(socket);
                if (packet == null) continue;

                Console.WriteLine(packet.ID);

                if (packet is LoginResponsePacket response)
                {
                    Console.WriteLine(response.IssuedToken);

                    for(int i = 0; i < 100; ++i)
                    {
                        MovePacket move = new MovePacket
                        {
                            Token = response.IssuedToken,
                            X = 1,
                            Y = 3,
                        };

                        await SendAsync(udpSocket, new IPEndPoint(IPAddress.Any, 5051), move);
                    }
                }
            }
        }

        public static async Task SendAsync<T>(Socket socket, T packet) where T : PacketBase
        {
            byte[] header = new byte[6];
            byte[] body = MessagePackSerializer.Serialize(packet);

            BitConverter.GetBytes(packet.ID).CopyTo(header, 0);
            BitConverter.GetBytes(body.Length).CopyTo(header, 2);

            ArraySegment<byte>[] buffer = [new ArraySegment<byte>(header), new ArraySegment<byte>(body)];

            await socket.SendAsync(buffer, SocketFlags.None);
        }
        public static async Task SendAsync<T>(Socket socket, EndPoint endPoint, T packet) where T : UdpPacketBase
        {
            byte[] body = MessagePackSerializer.Serialize(packet);
            await socket.SendAsync(body, SocketFlags.None);
        }

        public static async Task<PacketBase?> ReceiveAsync(Socket socket)
        {
            byte[] header = new byte[6];

            int got = await socket.ReceiveAsync(header.AsMemory(), SocketFlags.None);
            if (got == 0) return null;

            ushort id = BitConverter.ToUInt16(header, 0);
            int length = BitConverter.ToInt32(header, 2);

            byte[] body = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                int offset = 0;

                while (offset < length)
                {
                    int received = await socket.ReceiveAsync(body.AsMemory(offset, length - offset), SocketFlags.None);
                    if (received == 0) return null;

                    offset += received;
                }

                Type type = PacketRegistry.GetTypeById(id);
                return (PacketBase?)MessagePackSerializer.Deserialize(type, body.AsMemory(0, length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(body);
            }
        }
    }
}
