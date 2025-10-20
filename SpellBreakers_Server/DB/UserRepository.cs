using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace SpellBreakers_Server.DB
{
    class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string path)
        {
            _connectionString = $"Data Source={path}";
            Initialize();
        }

        private void Initialize()
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nickname TEXT NOT NULL UNIQUE, Password TEXT NOT NULL, Token TEXT, TokenIssuedAt DATETIME)";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public bool TryRegister(string nickname, string password)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO Users (Nickname, Password) Values (@nickname, @password)";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@nickname", nickname);
            cmd.Parameters.AddWithValue("@password", HashPassword(password));

            try
            {
                cmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string? TryLogin(string nickname, string password)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT Id FROM Users WHERE Nickname=@nickname AND Password=@password";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@nickname", nickname);
            cmd.Parameters.AddWithValue("@password", HashPassword(password));

            object? result = cmd.ExecuteScalar();
            if (result == null) return null;

            int id = Convert.ToInt32(result);
            string token = Guid.NewGuid().ToString();

            string update = @"UPDATE Users Set Token=@token, TokenIssuedAt=@tokenIssuedAt WHERE Id=@id";

            using SqliteCommand updateCmd = new SqliteCommand(update, connection);
            updateCmd.Parameters.AddWithValue("@token", token);
            updateCmd.Parameters.AddWithValue("@tokenIssuedAt", DateTime.UtcNow);
            updateCmd.Parameters.AddWithValue("@id", id);
            updateCmd.ExecuteNonQuery();

            return token;
        }

        private static string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool IsValidToken(string token, int validitySeconds = 86400)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT TokenIssuedAt FROM Users WHERE Token=@token";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@token", token);

            object? result = cmd.ExecuteScalar();
            if (result == null) return false;

            DateTime tokenIssuedAt = DateTime.Parse(result.ToString()!);

            return (DateTime.UtcNow - tokenIssuedAt).TotalSeconds <= validitySeconds;
        }
    }
}
