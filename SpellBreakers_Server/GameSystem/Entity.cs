using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class Entity : EntityInfo
    {
        public Vector TargetPosition { get; set; }
        public virtual float Speed { get; set; }
        public virtual float Radius { get; set; } = 1.0f;

        public virtual void Update(float deltaTime)
        {
            if (Speed == 0 || !IsMoving) return;

            Vector direction = TargetPosition - Position;
            float distance = direction.Magnitude;

            if (distance < 0.01f)
            {
                Position = TargetPosition;
                IsMoving = false;

                return;
            }

            float moveDistance = Speed * deltaTime;
            if (moveDistance >= distance)
            {
                Position = TargetPosition;
                IsMoving = false;
            }
            else
            {
                Position += direction.Normalized * moveDistance;
            }
        }

        public virtual void OnCollision(Entity other)
        {

        }
    }
}
