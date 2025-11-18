using Microsoft.Data.Sqlite;
using SpellBreakers_Server.GameSystem.Characters;
using SpellBreakers_Server.Packet;

namespace SpellBreakers_Server.DB
{
    public class StatRepository
    {
        private readonly string _connectionString;

        public StatRepository(string path)
        {
            _connectionString = $"Data Source={path}";
            Initialize();
        }

        private void Initialize()
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"CREATE TABLE IF NOT EXISTS Characters (ID TEXT PRIMARY KEY, MaxHealth REAL, MaxMana REAL, Force REAL, Resistance REAL, Speed REAL)";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public Stat? GetCharacterStat(CharacterType type)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT MaxHealth, MaxMana, Force, Resistance, Speed FROM Characters WHERE ID=@id";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", type.ToString());

            using SqliteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                float maxHealth = (float)reader.GetDouble(0);
                float maxMana = (float)reader.GetDouble(1);
                float force = (float)reader.GetDouble(2);
                float resistance = (float)reader.GetDouble(3);
                float speed = (float)reader.GetDouble(4);

                return new Stat
                {
                    MaxHealth = maxHealth,
                    MaxMana = maxMana,
                    Force = force,
                    Resistance = resistance,
                    Speed = speed
                };
            }

            return null;
        }
    }
}
