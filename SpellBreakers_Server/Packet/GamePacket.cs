using MessagePack;

namespace SpellBreakers_Server.Packet
{

    [MessagePackObject]
    public class EntityInfoPacket : UdpPacketBase
    {
        public override ushort ID => (ushort)PacketId.EntityInfo;
        [Key(2)] public List<EntityInfo> Entities = new List<EntityInfo>();
    }

    [MessagePackObject]
    public class EntityInfo
    {
        [Key(0)] public ushort EntityType { get; set; }
        [Key(1)] public string EntityID { get; set; } = "";
        [Key(2)] public float X { get; set; }
        [Key(3)] public float Y { get; set; }
    }

    [MessagePackObject]
    public class MovePacket : UdpPacketBase
    {
        public override ushort ID => (ushort)PacketId.Move;
        [Key(2)] public float X { get; set; }
        [Key(3)] public float Y { get; set; }
    }
}
