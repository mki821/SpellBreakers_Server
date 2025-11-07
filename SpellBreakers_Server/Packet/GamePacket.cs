using MessagePack;

namespace SpellBreakers_Server.Packet
{

    [MessagePackObject]
    public class EntityInfoPacket : UdpPacketBase
    {
        public override ushort ID => (ushort)PacketId.EntityInfo;
        [Key(2)] public IEnumerable<EntityInfo>? Entities = null;
    }

    [MessagePackObject]
    public class EntityInfo
    {
        [Key(0)] public ushort EntityType { get; set; }
        [Key(1)] public string EntityID { get; set; } = "";
        [Key(2)] public Vector Position { get; set; }
    }

    [MessagePackObject]
    public struct Vector
    {
        public readonly static Vector zero = new Vector(0, 0, 0);

        [Key(0)] public float X;
        [Key(1)] public float Y;
        [Key(2)] public float Z;

        [IgnoreMember] public readonly float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z);
        [IgnoreMember] public Vector Normalized
        {
            get
            {
                float magnitude = Magnitude;
                return magnitude > 0 ? new Vector(X / magnitude, Y / magnitude, Z / magnitude) : zero;
            }
        }

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector operator +(Vector vec1, Vector vec2) => new Vector(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
        public static Vector operator -(Vector vec1, Vector vec2) => new Vector(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
        public static Vector operator *(Vector vec, float scalar) => new Vector(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        public static Vector operator /(Vector vec, float scalar) => new Vector(vec.X / scalar, vec.Y / scalar, vec.Z / scalar);
    }

    [MessagePackObject]
    public class MovePacket : UdpPacketBase
    {
        public override ushort ID => (ushort)PacketId.Move;
        [Key(2)] public Vector TargetPosition { get; set; }
    }
}
