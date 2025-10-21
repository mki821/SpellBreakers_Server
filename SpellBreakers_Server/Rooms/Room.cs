using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Tcp;
using SpellBreakers_Server.Users;

namespace SpellBreakers_Server.Rooms
{
    public class Room
    {
        private List<User> _players = new List<User>();
        private List<User> _spectators = new List<User>();

        public ushort ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        private object _locker = new object();

        private const int MaxPlayerCount = 2;
        private const int MaxSpectatorCount = 4;

        public int PlayerCount => _players.Count;
        public int SpectatorCount => _spectators.Count;

        public Room(string name, string password)
        {
            Name = name;
            Password = password;
        }

        public async Task TryJoin(User user, string password)
        {
            JoinRoomResponsePacket response = new JoinRoomResponsePacket();

            if(!string.Equals(password, Password))
            {
                response.Success = false;
                response.Message = "방 참가 실패 : 비밀번호가 틀렸습니다!";

                await TcpPacketHelper.SendAsync(user.TcpSocket, response);

                return;
            }

            bool success = false;

            lock(_locker)
            {
                if (PlayerCount < MaxPlayerCount)
                {
                    _players.Add(user);
                    success = true;
                }
                else if (SpectatorCount < MaxSpectatorCount)
                {
                    _spectators.Add(user);
                    success = true;
                }
            }

            if(success)
            {
                user.CurrentRoom = this;
                response.Success = true;

                await TcpPacketHelper.SendAsync(user.TcpSocket, response);
                await UpdateRoomInfo();
            }
            else
            {
                response.Success = false;
                response.Message = "방 참가 실패 : 방에 인원이 가득 찼습니다!";

                await TcpPacketHelper.SendAsync(user.TcpSocket, response);
            }
        }

        public async Task Leave(User user)
        {
            lock (_locker)
            {
                _players.Remove(user);
                _spectators.Remove(user);
            }

            LeaveRoomResponsePacket response = new LeaveRoomResponsePacket();
            response.Success = true;

            await TcpPacketHelper.SendAsync(user.TcpSocket, response);
            await UpdateRoomInfo();
        }

        public async Task SwitchRole(User user)
        {
            SwitchRoleResponsePacket response = new SwitchRoleResponsePacket();

            lock(_locker)
            {
                if (_players.Contains(user))
                {
                    if (SpectatorCount < MaxSpectatorCount)
                    {
                        _players.Remove(user);
                        _spectators.Add(user);

                        response.Success = true;
                        response.ToSpectator = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "관전석이 가득 찼습니다!";
                    }
                }
                else if (_spectators.Contains(user))
                {
                    if (PlayerCount < MaxPlayerCount)
                    {
                        _spectators.Remove(user);
                        _players.Add(user);

                        response.Success = true;
                        response.ToSpectator = false;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "플레이어석이 가득 찼습니다!";
                    }
                }
            }

            await TcpPacketHelper.SendAsync(user.TcpSocket, response);

            if(response.Success)
            {
                await UpdateRoomInfo();
            }
        }

        public async Task UpdateRoomInfo()
        {
            RoomInfoPacket packet = new RoomInfoPacket();

            for (int i = 0; i < PlayerCount; ++i)
            {
                packet.Players.Add(new UserElement { Nickname = _players[i].Nickname ?? string.Empty });
            }

            for (int i = 0; i < SpectatorCount; ++i)
            {
                packet.Spectators.Add(new UserElement { Nickname = _spectators[i].Nickname ?? string.Empty });
            }

            await Broadcast(packet);
        }

        public async Task Broadcast<T>(T packet) where T : PacketBase
        {
            List<Task> tasks = new List<Task>();

            lock(_locker)
            {
                for (int i = 0; i < PlayerCount; ++i)
                {
                    tasks.Add(TcpPacketHelper.SendAsync(_players[i].TcpSocket, packet));
                }

                for (int i = 0; i < SpectatorCount; ++i)
                {
                    tasks.Add(TcpPacketHelper.SendAsync(_spectators[i].TcpSocket, packet));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
