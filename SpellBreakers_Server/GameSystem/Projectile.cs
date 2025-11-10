using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class Projectile : Entity
    {
        public override float Speed => 15.0f;

        public override void Update(float deltaTime)
        {
            Vector direction = TargetPosition - Position;
            float distance = direction.Magnitude;

            if (distance < 0.01f)
            {
                Position = TargetPosition;
                IsDead = true;
            }

            float moveDistance = Speed * deltaTime;
            if (moveDistance >= distance)
            {
                Position = TargetPosition;
                IsDead = true;
            }
            else
            {
                Position += direction.Normalized * moveDistance;
            }
        }

        public override void OnCollision()
        {
            IsDead = true;
        }
    }
}
