using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Rooms;
using System.Diagnostics;

namespace SpellBreakers_Server.GameSystem
{
    public class Game : IDisposable
    {
        private Room _room;
        private Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();

        private CancellationTokenSource _cts;

        public Game(Room room)
        {
            _room = room;
            _cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Start(string token1, string token2)
        {
            AddCharacter(token1, 10.0f, 0.0f, 0.0f);
            AddCharacter(token2, -10.0f, 0.0f, 0.0f);

            _ = Task.Run(UpdateAsync);
        }

        private void AddCharacter(string id, float x, float y, float z)
        {
            Entity entity = new Entity
            {
                EntityType = (ushort)EntityType.Character,
                EntityID = id,
                Position = new Vector(x, y, z)
            };

            _entities.Add(id, entity);
        }

        public void End()
        {
            _cts.Cancel();
        }

        private async Task UpdateAsync()
        {
            CancellationToken token = _cts.Token;
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                while(!token.IsCancellationRequested)
                {
                    float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();

                    UpdateMovement(deltaTime);

                    EntityInfoPacket packet = new EntityInfoPacket
                    {
                        Entities = _entities.Values
                    };

                    await _room.BroadcastUdp(packet);

                    await Task.Delay(33, _cts.Token);
                }
            }
            catch(TaskCanceledException)
            {

            }
        }

        public void UpdateMovement(float deltaTime)
        {
            foreach(Entity entity in _entities.Values)
            {
                if (!entity.IsMoving) continue;

                Vector direction = entity.TargetPosition - entity.Position;
                float distance = direction.Magnitude;

                if(distance < 0.01f)
                {
                    entity.Position = entity.TargetPosition;
                    entity.IsMoving = false;

                    continue;
                }

                float moveDistance = entity.Speed * deltaTime;
                if(moveDistance >= distance)
                {
                    entity.Position = entity.TargetPosition;
                    entity.IsMoving = false;
                }
                else
                {
                    entity.Position += direction.Normalized * moveDistance;
                }
            }
        }

        public void Move(MovePacket packet)
        {
            if(_entities.TryGetValue(packet.Token, out Entity? entity))
            {
                entity.TargetPosition = packet.TargetPosition;
                entity.IsMoving = true;
            }
        }
    }
}
