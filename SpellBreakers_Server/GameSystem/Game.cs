using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Rooms;

namespace SpellBreakers_Server.GameSystem
{
    public class Game
    {
        private Room _room;
        private Dictionary<string, EntityInfo> _entityInfos = new Dictionary<string, EntityInfo>();

        private CancellationTokenSource _cts;

        public Game(Room room)
        {
            _room = room;
            _cts = new CancellationTokenSource();
        }

        public void Start(string token1, string token2)
        {
            CreateCharacter(token1, 10.0f, 0.0f);
            CreateCharacter(token2, -10.0f, 0.0f);

            _ = UpdateAsync();
        }

        private void CreateCharacter(string id, float x, float y)
        {
            EntityInfo newInfo = new EntityInfo
            {
                EntityType = (ushort)EntityType.Character,
                EntityID = id,
                X = y,
                Y = x
            };

            _entityInfos.Add(newInfo.EntityID, newInfo);
        }

        public void End()
        {
            _cts.Cancel();
        }

        private async Task UpdateAsync()
        {
            try
            {
                while(true)
                {
                    EntityInfoPacket packet = new EntityInfoPacket
                    {
                        Entities = _entityInfos.Values.ToList()
                    };

                    await _room.BroadcastUdp(packet);

                    await Task.Delay(33, _cts.Token);
                }
            }
            catch(TaskCanceledException)
            {

            }
        }
    }
}
