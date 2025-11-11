using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace SpellBreakers_Server.DB
{
    public class UserRepository
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

            CreateUsers(connection);
            CreateUserTokens(connection);
        }

        private static void CreateUsers(SqliteConnection connection)
        {
            string query = @"CREATE TABLE IF NOT EXISTS Users (ID TEXT PRIMARY KEY, Nickname TEXT NOT NULL UNIQUE, Password TEXT NOT NULL)";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        private static void CreateUserTokens(SqliteConnection connection)
        {
            string query = @"CREATE TABLE IF NOT EXISTS UserTokens (ID INTEGER PRIMARY KEY AUTOINCREMENT, UserID TEXT NOT NULL UNIQUE, Token TEXT NOT NULL, Expiry DATETIME NOT NULL,
                                FOREIGN KEY(UserID) REFERENCES Users(ID))";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public bool TryRegister(string id, string nickname, string password)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO Users (ID, Nickname, Password) Values (@id, @nickname, @password)";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
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

        public string? TryLogin(string id, string password, ref string message)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT Password FROM Users WHERE ID=@id";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            object? result = cmd.ExecuteScalar();
            if (result == null)
            {
                message = "유저를 찾을 수 없습니다!";

                return null;
            }

            string? storedHash = result.ToString();
            if (HashPassword(password) != storedHash)
            {
                message = "아이디 혹은 비밀번호가 틀렸습니다!";

                return null;
            }

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddDays(30);

            string insert = @"INSERT INTO UserTokens (UserID, Token, Expiry) VALUES (@id, @token, @expiry) ON CONFLICT (UserID) DO UPDATE SET Token=@token, Expiry=@expiry";

            using SqliteCommand insertCmd = new SqliteCommand(insert, connection);
            insertCmd.Parameters.AddWithValue("@id", id);
            insertCmd.Parameters.AddWithValue("@token", token);
            insertCmd.Parameters.AddWithValue("@expiry", expiry);
            insertCmd.ExecuteNonQuery();

            return token;
        }

        public string? AutoLogin(string token)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT Expiry From UserTokens WHERE Token=@token";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@token", token);

            object? result = cmd.ExecuteScalar();
            if(result == null) return null;

            if(!DateTime.TryParse(result.ToString(), out DateTime expiry)) return null;

            if(expiry < DateTime.UtcNow)
            {
                string delete = @"DELETE FROM UserTokens WHERE Token=@token";

                using SqliteCommand deleteCmd = new SqliteCommand(delete, connection);
                deleteCmd.Parameters.AddWithValue(@"token", token);
                deleteCmd.ExecuteNonQuery();

                return null;
            }

            string newToken = Guid.NewGuid().ToString();
            DateTime newExpiry = DateTime.UtcNow.AddDays(30);

            string update = @"UPDATE UserTokens SET Token=@newToken, Expiry=@expiry WHERE Token=@token";

            using SqliteCommand updateCmd = new SqliteCommand(update, connection);
            updateCmd.Parameters.AddWithValue("@newToken", newToken);
            updateCmd.Parameters.AddWithValue("@expiry", newExpiry);
            updateCmd.Parameters.AddWithValue("@token", token);
            updateCmd.ExecuteNonQuery();

            return newToken;
        }

        private static string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public string? GetIDByToken(string token)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT UserID FROM UserTokens WHERE Token=@token";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@token", token);
            
            object? result = cmd.ExecuteScalar();

            return result?.ToString();
        }

        public string? GetNicknameByID(string id)
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = @"SELECT Nickname FROM Users WHERE ID=@id";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            object? result = cmd.ExecuteScalar();

            return result?.ToString();
        }
    }
}
