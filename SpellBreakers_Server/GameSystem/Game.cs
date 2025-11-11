using System.Diagnostics;
using SpellBreakers_Server.GameSystem.Entities;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Rooms;

namespace SpellBreakers_Server.GameSystem
{
    public class Game : IDisposable
    {
        private readonly Room _room;
        private readonly EntityManager _entityManager;

        private readonly CancellationTokenSource _cts;

        public Game(Room room)
        {
            _room = room;
            _entityManager = new EntityManager();
            _cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Start(string token1, string token2)
        {
            _entityManager.AddCharacter(CharacterType.Magician, token1, new Vector(10.0f, 0.0f, 0.0f));
            _entityManager.AddCharacter(CharacterType.Magician, token2, new Vector(-10.0f, 0.0f, 0.0f));

            _ = Task.Run(UpdateAsync);
        }

        public void End()
        {
            _cts.Cancel();
        }

        private async Task UpdateAsync()
        {
            CancellationToken token = _cts.Token;
            Stopwatch stopwatch = Stopwatch.StartNew();
            uint currentTick = 0;

            try
            {
                while(!token.IsCancellationRequested)
                {
                    float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();

                    ++currentTick;

                    _entityManager.Update(deltaTime);
                    _entityManager.CheckCollision();

                    EntityInfoPacket packet = new EntityInfoPacket
                    {
                        Tick = currentTick,
                        Entities = _entityManager.Entities.Select(entity => entity.EntityInfo)
                    };

                    await _room.BroadcastUdp(packet);

                    _entityManager.RemoveDeadEntities();

                    await Task.Delay(33, _cts.Token);
                }
            }
            catch(TaskCanceledException)
            {

            }
        }

        public void Move(MovePacket packet)
        {
            if(_entityManager.TryGetValue(packet.Token, out Entity? entity))
            {
                entity.TargetPosition = packet.TargetPosition;
                entity.EntityInfo.IsMoving = true;
            }
        }

        public void FireProjectile(FireProjectilePacket packet)
        {
            string id = Guid.NewGuid().ToString();
            Vector spawnPosition = packet.SpawnPosition;

            Projectile projectile = (Projectile)_entityManager.AddEntity(EntityType.Projectile, id, spawnPosition);
            projectile.TargetPosition = packet.TargetPosition;
            projectile.OwnerID = packet.OwnerID;
        }
    }
}
