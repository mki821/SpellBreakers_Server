using SpellBreakers_Server.DB;
using SpellBreakers_Server.GameSystem.Characters;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.GameSystem
{
    public class StatManager
    {
        private static Lazy<StatManager> _instance = new Lazy<StatManager>(() => new StatManager());
        public static StatManager Instance => _instance.Value;

        private Dictionary<CharacterType, Stat> _characterStats = new Dictionary<CharacterType, Stat>();
        private StatRepository _statRepository;

        public StatManager()
        {
            _statRepository = new StatRepository("characters.db");
        }

        public Stat GetCharacterStat(CharacterType type)
        {
            if(_characterStats.TryGetValue(type, out Stat stat))
            {
                return stat;
            }

            Stat? newStat = _statRepository.GetCharacterStat(type);
            if(newStat != null)
            {
                _characterStats.Add(type, newStat.Value);
                return newStat.Value;
            }

            throw new Exception($"Unassigned Character Information - {type}");
        }
    }
}
