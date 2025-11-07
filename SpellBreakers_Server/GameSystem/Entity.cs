using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class Entity : EntityInfo
    {
        public Vector TargetPosition { get; set; }
        public float Speed { get; set; } = 5.0f;
        public bool IsMoving { get; set; }
    }
}
