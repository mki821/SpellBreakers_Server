using SpellBreakers_Server.GameSystem.Entities;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem.Characters
{
    public class Character_Magician : Character
    {
        public Character_Magician(CharacterInfo info, EntityManager manager) : base(info, manager)
        {

        }

        protected override void Skill1(SkillPacket packet)
        {
            string id = Guid.NewGuid().ToString();
            Vector spawnPosition = packet.SpawnPosition;

            Projectile projectile = (Projectile)EntityManager.AddEntity(EntityType.Projectile, id, spawnPosition);
            projectile.TargetPosition = packet.TargetPosition;
            projectile.OwnerID = packet.OwnerID;
        }

        protected override void Skill2(SkillPacket packet)
        {

        }

        protected override void Skill3(SkillPacket packet)
        {

        }

        protected override void Skill4(SkillPacket packet)
        {

        }

        protected override void Skill5(SkillPacket packet)
        {

        }

        protected override void Skill6(SkillPacket packet)
        {

        }

        protected override void Skill7(SkillPacket packet)
        {

        }

        protected override void Skill8(SkillPacket packet)
        {

        }
    }
}
