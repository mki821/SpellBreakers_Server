using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class Projectile : Entity
    {
        public override float Speed => 15.0f;

        public string OwnerID { get; set; } = "";

        public override void Update(float deltaTime)
        {
            Vector direction = TargetPosition - Position;
            float distance = direction.Magnitude;
            float moveDistance = Speed * deltaTime;

            if (distance <= moveDistance || distance < 0.01f)
            {
                Position = TargetPosition;
                IsDead = true;
            }
            else
            {
                Position += direction.Normalized * moveDistance;
            }
        }

        public override void OnCollision(Entity other)
        {
            if(other.EntityType == (ushort)GameSystem.EntityType.Character && other.EntityID != OwnerID)
            {
                IsDead = true;
            }
        }
    }
}
