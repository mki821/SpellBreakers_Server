using System.Net;
using System.Net.Sockets;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.Tcp
{
    class TcpServer
    {
        private readonly Socket _listener;

        public TcpServer(int port)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, port));
            _listener.Listen(100);
        }

        public async Task StartAsync()
        {
            Console.WriteLine($"[서버] TCP {(_listener.LocalEndPoint as IPEndPoint)?.Port} 번 포트에서 시작됨!");

            while (true)
            {
                Socket client = await _listener.AcceptAsync();
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(Socket socket)
        {
            Console.WriteLine($"[서버] 클라이언트 접속 : {socket.RemoteEndPoint}");

            try
            {
                while (true)
                {
                    PacketBase? packet = await TcpPacketHelper.ReceiveAsync(socket);
                    if (packet == null) break;

                    PacketHandler.Handle(socket, packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[서버] 클라이언트 오류 : {socket.RemoteEndPoint} - {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"[서버] 클라이언트 종료 : {socket.RemoteEndPoint}");

                socket.Close();
            }
        }
    }
}
