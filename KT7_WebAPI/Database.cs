using Microsoft.Data.Sqlite;

namespace KT7_WebAPI;

public class Database
{
    private readonly string _connectionString = "Data Source=kt7.db";

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public void Initialize()
    {
        using var connection = GetConnection();
        connection.Open();

        string query = """
                       CREATE TABLE IF NOT EXISTS Students (
                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                           Name TEXT NOT NULL,
                           Age INTEGER NOT NULL,
                           Email TEXT NOT NULL
                       );

                       CREATE TABLE IF NOT EXISTS Tasks (
                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                           Title TEXT NOT NULL,
                           Description TEXT NOT NULL,
                           Status TEXT NOT NULL
                       );

                       CREATE TABLE IF NOT EXISTS Reservations (
                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                           ResourceType TEXT NOT NULL,
                           ResourceId INTEGER NOT NULL,
                           StartAt TEXT NOT NULL,
                           EndAt TEXT NOT NULL
                       );
                       """;
        

        using var command = new SqliteCommand(query, connection);
        command.ExecuteNonQuery();
    }
}