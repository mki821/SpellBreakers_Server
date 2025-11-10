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
            AddEntity(EntityType.Character, token1, 10.0f, 0.0f, 0.0f);
            AddEntity(EntityType.Character, token2, -10.0f, 0.0f, 0.0f);

            _ = Task.Run(UpdateAsync);
        }

        private Entity AddEntity(EntityType type, string id, float x, float y, float z)
        {
            Entity entity = type switch
            {
                EntityType.Character => new Character
                {
                    EntityType = (ushort)type,
                    EntityID = id,
                    Position = new Vector(x, y, z)
                },
                EntityType.Projectile => new Projectile
                {
                    EntityType = (ushort)type,
                    EntityID = id,
                    Position = new Vector(x, y, z)
                },
                _ => new Entity
                {
                    EntityType = (ushort)type,
                    EntityID = id,
                    Position = new Vector(x, y, z)
                }
            };

            _entities.Add(id, entity);

            return entity;
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

                    List<string> deadEntityIds = new List<string>();

                    foreach (Entity entity in _entities.Values)
                    {
                        if (entity.IsDead)
                        {
                            deadEntityIds.Add(entity.EntityID);
                        }

                        entity.Update(deltaTime);
                    }

                    EntityInfoPacket packet = new EntityInfoPacket
                    {
                        Entities = _entities.Values
                    };

                    await _room.BroadcastUdp(packet);

                    foreach(string id in deadEntityIds)
                    {
                        _entities.Remove(id);
                    }

                    await Task.Delay(33, _cts.Token);
                }
            }
            catch(TaskCanceledException)
            {

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

        public void FireProjectile(FireProjectilePacket packet)
        {
            string id = Guid.NewGuid().ToString();
            Vector spawnPosition = packet.SpawnPosition;

            Entity projectile = AddEntity(EntityType.Projectile, id, spawnPosition.X, spawnPosition.Y, spawnPosition.Z);
            projectile.TargetPosition = packet.TargetPosition;
        }
    }
}
