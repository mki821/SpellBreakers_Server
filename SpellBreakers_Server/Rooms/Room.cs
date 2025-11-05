using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Tcp;
using SpellBreakers_Server.Udp;
using SpellBreakers_Server.Users;
using System;

namespace SpellBreakers_Server.Rooms
{
    public class Room
    {
        private readonly List<RoomMember> _players = new List<RoomMember>();
        private readonly List<RoomMember> _spectators = new List<RoomMember>();
        private readonly Dictionary<string, RoomMember> _roomMembers = new Dictionary<string, RoomMember>();

        public ushort ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        private readonly Lock _locker = new Lock();

        private const int MaxPlayerCount = 2;
        private const int MaxSpectatorCount = 4;

        public int PlayerCount => _players.Count;
        public int SpectatorCount => _spectators.Count;

        private CancellationTokenSource? _startCountdownCts = null;

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
                    RoomMember member = new RoomMember(user);
                    _players.Add(member);
                    _roomMembers.Add(user.ID, member);

                    success = true;
                }
                else if (SpectatorCount < MaxSpectatorCount)
                {
                    RoomMember member = new RoomMember(user);
                    _spectators.Add(member);
                    _roomMembers.Add(user.ID, member);

                    success = true;
                }
            }

            if(success)
            {
                user.CurrentRoom = this;
                response.Success = true;

                await TcpPacketHelper.SendAsync(user.TcpSocket, response);
                await UpdateRoomInfo();

                ChatPacket chat = new ChatPacket { Message = $"{user.Nickname} 님이 참가하였습니다." };

                await Broadcast(chat);
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
            if(!_roomMembers.TryGetValue(user.ID, out RoomMember? member) || member == null)
            {
                return;
            }

            lock (_locker)
            {
                _players.Remove(member);
                _spectators.Remove(member);
            }

            if(_players.Count == 0 && _spectators.Count == 0)
            {
                RoomManager.Instance.Remove(this);
            }

            LeaveRoomResponsePacket response = new LeaveRoomResponsePacket();
            response.Success = true;

            await TcpPacketHelper.SendAsync(user.TcpSocket, response);
            await UpdateRoomInfo();

            ChatPacket chat = new ChatPacket { Message = $"{user.Nickname} 님이 퇴장하였습니다." };

            await Broadcast(chat);
        }

        public async Task SwitchRole(User user)
        {
            SwitchRoleResponsePacket response = new SwitchRoleResponsePacket();

            lock(_locker)
            {
                RoomMember member = _roomMembers[user.ID];

                if (_players.Contains(member))
                {
                    if (SpectatorCount < MaxSpectatorCount)
                    {
                        _players.Remove(member);
                        _spectators.Add(member);

                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "관전석이 가득 찼습니다!";
                    }
                }
                else if (_spectators.Contains(member))
                {
                    if (PlayerCount < MaxPlayerCount)
                    {
                        _spectators.Remove(member);
                        _players.Add(member);

                        response.Success = true;
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

        public async Task Ready(User user)
        {
            ReadyResponsePacket response = new ReadyResponsePacket();

            lock (_locker)
            {
                _roomMembers[user.ID].IsReady = !_roomMembers[user.ID].IsReady;

                if (_startCountdownCts != null && !_roomMembers[user.ID].IsReady)
                {
                    _startCountdownCts.Cancel();
                    _startCountdownCts = null;
                }

                bool allReady = _roomMembers.Values.All(member => member.IsReady);

                if (allReady && _startCountdownCts == null)
                {
                    _startCountdownCts = new CancellationTokenSource();
                    _ = StartCountdownAsync();
                }
            }

            response.IsReady = _roomMembers[user.ID].IsReady;

            await TcpPacketHelper.SendAsync(user.TcpSocket, response);
            await UpdateRoomInfo();
        }

        private async Task StartCountdownAsync()
        {
            if (_startCountdownCts == null) return;

            ChatPacket chat = new ChatPacket();

            try
            {
                for(int i = 5; i > 0; --i)
                {
                    chat.Message = $"{i}초 뒤 게임이 시작합니다.";

                    await Broadcast(chat);

                    await Task.Delay(1000, _startCountdownCts.Token);
                }

                StartGamePacket packet = new StartGamePacket();

                await Broadcast(packet);
            }
            catch (TaskCanceledException)
            {
                chat.Message = $"카운트다운이 취소되었습니다.";

                await Broadcast(chat);
            }
        }

        public async Task UpdateRoomInfo()
        {
            RoomInfoPacket packet = new RoomInfoPacket();

            for (int i = 0; i < PlayerCount; ++i)
            {
                packet.Players.Add(new UserElement { Nickname = _players[i].User.Nickname, IsReady = _players[i].IsReady });
            }

            for (int i = 0; i < SpectatorCount; ++i)
            {
                packet.Spectators.Add(new UserElement { Nickname = _spectators[i].User.Nickname, IsReady = _spectators[i].IsReady });
            }

            await Broadcast(packet);
        }

        public async Task Broadcast<T>(T packet) where T : PacketBase
        {
            await BroadcastInternal(packet, member =>
            {
                return TcpPacketHelper.SendAsync(member.User.TcpSocket, packet);
            });
        }

        public async Task BroadcastUdp<T>(T packet) where T : UdpPacketBase
        {
            await BroadcastInternal(packet, member =>
            {
                User user = member.User;

                if (user.UdpEndPoint != null)
                {
                    return UdpPacketHelper.SendAsync(user.UdpEndPoint, packet);
                }

                return Task.CompletedTask;
            });
        }

        private async Task BroadcastInternal<T>(T packet, Func<RoomMember, Task> func) where T : PacketBase
        {
            List<Task> tasks = new List<Task>();

            lock (_locker)
            {
                for (int i = 0; i < PlayerCount; ++i)
                {
                    tasks.Add(func(_players[i]));
                }

                for (int i = 0; i < SpectatorCount; ++i)
                {
                    tasks.Add(func(_spectators[i]));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
