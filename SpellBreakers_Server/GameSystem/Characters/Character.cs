using SpellBreakers_Server.GameSystem.Entities;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem.Characters
{
    public abstract class Character : Entity
    {
        private Dictionary<ushort, long> _skillCooldowns = new Dictionary<ushort, long>();

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
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if(_skillCooldowns.TryGetValue(packet.SkillType, out long nextAvailable) && now < nextAvailable) return;

            _skillCooldowns[packet.SkillType] = now + 1000;

            switch (packet.SkillType)
            {
                case 1: Skill1(packet); break;
                case 2: Skill2(packet); break;
                case 3: Skill3(packet); break;
                case 4: Skill4(packet); break;
                case 5: Skill5(packet); break;
                case 6: Skill6(packet); break;
                case 7: Skill7(packet); break;
                case 8: Skill8(packet); break;
            }
        }

        protected abstract void Skill1(SkillPacket packet);
        protected abstract void Skill2(SkillPacket packet);
        protected abstract void Skill3(SkillPacket packet);
        protected abstract void Skill4(SkillPacket packet);
        protected abstract void Skill5(SkillPacket packet);
        protected abstract void Skill6(SkillPacket packet);
        protected abstract void Skill7(SkillPacket packet);
        protected abstract void Skill8(SkillPacket packet);
    }
}
