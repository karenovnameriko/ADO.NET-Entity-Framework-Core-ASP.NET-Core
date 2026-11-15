using Microsoft.Data.Sqlite;

namespace KT6_UserUpdate;

public class Database
{
    private readonly string _connectionString = "Data Source=users.db";

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public void Initialize()
    {
        using var connection = GetConnection();
        connection.Open();

        string query = """
                       CREATE TABLE IF NOT EXISTS Users (
                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                           Name TEXT NOT NULL,
                           Email TEXT NOT NULL,
                           Age INTEGER NOT NULL
                       );

                       CREATE TABLE IF NOT EXISTS UpdateLogs (
                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                           UserId INTEGER NOT NULL,
                           ChangedAt TEXT NOT NULL,
                           Description TEXT NOT NULL
                       );
                       """;

        using var command = new SqliteCommand(query, connection);
        command.ExecuteNonQuery();

        string insertQuery = """
                             INSERT INTO Users (Name, Email, Age)
                             SELECT 'Ivan', 'ivan@example.com', 25
                             WHERE NOT EXISTS (SELECT 1 FROM Users);
                             """;

        using var insertCommand = new SqliteCommand(insertQuery, connection);
        insertCommand.ExecuteNonQuery();
    }
}