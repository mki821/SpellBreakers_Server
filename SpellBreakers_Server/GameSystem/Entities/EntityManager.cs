using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem.Entities
{
    public class EntityManager
    {
        private Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();

        public IEnumerable<Entity> Entities => _entities.Values;

        public Entity AddEntity(EntityType type, string id, Vector position)
        {
            Entity entity = type switch
            {
                EntityType.Projectile => new Projectile(new DefaultEntityInfo
                {
                    EntityType = (ushort)type,
                    EntityID = id,
                    Position = position
                }),
                _ => new Entity(new DefaultEntityInfo
                {
                    EntityType = (ushort)type,
                    EntityID = id,
                    Position = position
                })
            };

            _entities.Add(id, entity);

            return entity;
        }

        public Character AddCharacter(CharacterType type, string id, Vector position)
        {
            Character character = new Character(new CharacterInfo
            {
                EntityType = (ushort)EntityType.Character,
                EntityID = id,
                Position = position,
                CharacterType = (ushort)type
            });

            _entities.Add(id, character);

            return character;
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

                    float dx = a.EntityInfo.Position.X - b.EntityInfo.Position.X;
                    float dy = a.EntityInfo.Position.Y - b.EntityInfo.Position.Y;
                    float dz = a.EntityInfo.Position.Z - b.EntityInfo.Position.Z;
                    float radiusSum = a.Radius + b.Radius;

                    if (dx * dx + dy * dy + dz * dz < radiusSum * radiusSum)
                    {
                        a.OnCollision(b);
                        b.OnCollision(a);
                    }
                }
            }
        }

        public void RemoveDeadEntities()
        {
            List<string> ids = [];

            foreach (Entity entity in _entities.Values)
            {
                if(entity.IsDead)
                {
                    ids.Add(entity.EntityInfo.EntityID);
                }
            }

            for (int i = 0; i < ids.Count; ++i)
            {
                _entities.Remove(ids[i]);
            }
        }
    }
}
