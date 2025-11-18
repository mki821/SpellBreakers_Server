using SpellBreakers_Server.GameSystem.Entities;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem.Characters
{
    public abstract class Character : Entity
    {
        public CharacterInfo CharacterInfo => (CharacterInfo)EntityInfo;

        public Character(CharacterInfo info, EntityManager manager) : base(info, manager)
        {
            info.Stat = StatManager.Instance.GetCharacterStat((CharacterType)info.CharacterType);
            info.CurrentHealth = info.Stat.MaxHealth;
            info.CurrentMana = info.Stat.MaxMana;

            Speed = info.Stat.Speed;
        }

        public void TakeDamage(float damage)
        {
            CharacterInfo.CurrentHealth -= damage;

            if(CharacterInfo.CurrentHealth <= 0 )
            {
                IsDead = true;
            }
        }

        public void UseSkill(SkillPacket packet)
        {
            switch (packet.SkillType)
            {
                case 1: Skill1(packet); break;
            }
        }

        protected abstract void Skill1(SkillPacket packet);
        protected abstract void Skill2(SkillPacket packet);
        protected abstract void Skill3(SkillPacket packet);
        protected abstract void Skill4(SkillPacket packet);
    }
}
