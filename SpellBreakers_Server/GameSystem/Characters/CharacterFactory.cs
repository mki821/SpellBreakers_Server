using SpellBreakers_Server.GameSystem.Entities;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem.Characters
{
    public static class CharacterFactory
    {
        public static Character Create(CharacterType type, CharacterInfo info, EntityManager manager)
        {
            return type switch
            {
                CharacterType.Magician => new Character_Magician(info, manager),
                _ => new Character_Magician(info, manager)
            };
        }
    }
}
