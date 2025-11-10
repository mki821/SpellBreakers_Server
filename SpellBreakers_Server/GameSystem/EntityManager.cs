using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class EntityManager
    {
        private Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();

        public IEnumerable<Entity> Entities => _entities.Values;

        public Entity AddEntity(EntityType type, string id, float x, float y, float z)
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

        public bool TryGetValue(string id, out Entity? entity)
        {
            return _entities.TryGetValue(id, out entity);
        }

        public void Update(float deltaTime)
        {
            foreach (Entity entity in _entities.Values)
            {
                entity.Update(deltaTime);
            }
        }

        public void CheckCollision()
        {
            List<Entity> entities = _entities.Values.ToList();
            for (int i = 0; i < entities.Count; ++i)
            {
                for (int j = i + 1; j < entities.Count; ++j)
                {
                    Entity a = entities[i];
                    Entity b = entities[j];

                    float dx = a.Position.X - b.Position.X;
                    float dy = a.Position.Y - b.Position.Y;
                    float dz = a.Position.Z - b.Position.Z;
                    float radiusSum = a.Radius + b.Radius;

                    if (dx * dx + dy * dy + dz * dz < radiusSum * radiusSum)
                    {
                        a.OnCollision();
                        b.OnCollision();
                    }
                }
            }
        }

        public void RemoveDeadEntities()
        {
            IEnumerable<string> ids = _entities.Keys;
            foreach(string id in ids)
            {
                if (_entities[id].IsDead)
                {
                    _entities.Remove(id);
                }
            }
        }
    }
}
