using SpellBreakers_Server.GameSystem.Entities;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class Character : Entity
    {
        public CharacterInfo CharacterInfo => (CharacterInfo)EntityInfo;

        public Character(CharacterInfo info) : base(info)
        {
            info.Stat = StatManager.Instance.GetCharacterStat((CharacterType)info.CharacterType);
            info.CurrentHealth = info.Stat.MaxHealth;
            info.CurrentMana = info.Stat.MaxMana;

            Speed = info.Stat.Speed;
        }

        public void TakeDamage(float damage)
        {
            CharacterInfo.CurrentHealth -= damage;

            if(CharacterInfo.CurrentHealth < 0 )
            {
                IsDead = true;
            }
        }
    }
}
