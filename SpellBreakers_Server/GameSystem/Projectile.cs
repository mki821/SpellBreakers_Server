using SpellBreakers_Server.GameSystem.Characters;
using SpellBreakers_Server.GameSystem.Entities;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class Projectile : Entity
    {
        public override float Speed => 15.0f;

        public string OwnerID { get; set; } = "";

        public Projectile(EntityInfo info, EntityManager manager) : base(info, manager) { }

        public override void Update(float deltaTime)
        {
            Vector direction = TargetPosition - EntityInfo.Position;
            float distance = direction.Magnitude;
            float moveDistance = Speed * deltaTime;

            if (distance <= moveDistance || distance < 0.01f)
            {
                EntityInfo.Position = TargetPosition;
                IsDead = true;
            }
            else
            {
                EntityInfo.Position += direction.Normalized * moveDistance;
            }
        }

        public override void OnCollision(Entity other)
        {
            if(other is Character character && other.EntityInfo.EntityID != OwnerID)
            {
                character.TakeDamage(1.0f);
                IsDead = true;
            }
        }
    }
}
