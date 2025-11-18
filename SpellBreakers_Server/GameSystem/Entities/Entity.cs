using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem.Entities
{
    public class Entity
    {
        public EntityInfo EntityInfo { get; set; }
        protected EntityManager EntityManager { get; }

        public bool IsDead { get; set; }
        public Vector TargetPosition { get; set; }
        public virtual float Speed { get; set; }
        public virtual float Radius { get; set; } = 1.0f;

        public Entity(EntityInfo info, EntityManager manager)
        {
            EntityInfo = info;
            EntityManager = manager;
        }

        public virtual void Update(float deltaTime)
        {
            if (Speed == 0 || !EntityInfo.IsMoving) return;

            Vector direction = TargetPosition - EntityInfo.Position;
            float distance = direction.Magnitude;

            if (distance < 0.01f)
            {
                EntityInfo.Position = TargetPosition;
                EntityInfo.IsMoving = false;

                return;
            }

            float moveDistance = Speed * deltaTime;
            if (moveDistance >= distance)
            {
                EntityInfo.Position = TargetPosition;
                EntityInfo.IsMoving = false;
            }
            else
            {
                EntityInfo.Position += direction.Normalized * moveDistance;
            }
        }

        public virtual void OnCollision(Entity other)
        {

        }
    }
}
