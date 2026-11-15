
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using KT6_UserUpdate.Models;

namespace KT6_UserUpdate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly Database _database;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        Database database,
        ILogger<UsersController> logger)
    {
        _database = database;
        _logger = logger;
    }

    // Получение пользователя
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        string query = "SELECT Id, Name, Email, Age FROM Users WHERE Id = @Id";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return NotFound("Пользователь не найден.");

        var user = new User
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            Age = reader.GetInt32(3)
        };

        return Ok(user);
    }

    // Создание пользователя для тестирования
    [HttpPost]
    public IActionResult CreateUser(User user)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        string query = """
            INSERT INTO Users (Name, Email, Age)
            VALUES (@Name, @Email, @Age);
            SELECT last_insert_rowid();
            """;

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", user.Name);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Age", user.Age);

        int id = Convert.ToInt32(command.ExecuteScalar());

        user.Id = id;

        return CreatedAtAction(nameof(GetUser), new { id }, user);
    }

    // Задание 1 и 4: обновление нескольких полей и обработка исключений
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, User user)
    {
        try
        {
            using (var connection = _database.GetConnection())
            {
                string query = """
                    UPDATE Users
                    SET Name = @Name, Email = @Email, Age = @Age
                    WHERE Id = @Id
                    """;

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Age", user.Age);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result < 1)
                    {
                        LogUpdate("UpdateUser", id, false);
                        return NotFound("Пользователь не найден.");
                    }
                }
            }

            LogUpdate("UpdateUser", id, true);
            return Ok("Данные пользователя обновлены.");
        }
        catch (SqliteException ex)
        {
            LogUpdate("UpdateUser", id, false);
            _logger.LogError(ex, "Ошибка базы данных.");
            return StatusCode(500, "Ошибка базы данных.");
        }
        catch (Exception ex)
        {
            LogUpdate("UpdateUser", id, false);
            _logger.LogError(ex, "Ошибка сервера.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }

    // Задание 2: условное обновление email
    [HttpPut("{id}/email")]
    public IActionResult UpdateEmail(
        int id,
        [FromQuery] string name,
        [FromQuery] string email)
    {
        try
        {
            using (var connection = _database.GetConnection())
            {
                string query = """
                    UPDATE Users
                    SET Email = @Email
                    WHERE Id = @Id AND Name = @Name
                    """;

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result < 1)
                    {
                        LogUpdate("UpdateEmail", id, false);
                        return BadRequest("Имя не совпадает или пользователь не найден.");
                    }
                }
            }

            LogUpdate("UpdateEmail", id, true);
            return Ok("Email пользователя обновлён.");
        }
        catch (SqliteException ex)
        {
            LogUpdate("UpdateEmail", id, false);
            _logger.LogError(ex, "Ошибка базы данных.");
            return StatusCode(500, "Ошибка базы данных.");
        }
        catch (Exception ex)
        {
            LogUpdate("UpdateEmail", id, false);
            _logger.LogError(ex, "Ошибка сервера.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }

    // Задание 3: SQL-транзакция
    [HttpPut("{id}/transaction")]
    public IActionResult UpdateUserTransaction(int id, User user)
    {
        try
        {
            using (var connection = _database.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateQuery = """
                            UPDATE Users
                            SET Name = @Name, Email = @Email, Age = @Age
                            WHERE Id = @Id
                            """;

                        using (var updateCommand = new SqliteCommand(
                            updateQuery, connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@Id", id);
                            updateCommand.Parameters.AddWithValue("@Name", user.Name);
                            updateCommand.Parameters.AddWithValue("@Email", user.Email);
                            updateCommand.Parameters.AddWithValue("@Age", user.Age);

                            int result = updateCommand.ExecuteNonQuery();

                            if (result < 1)
                            {
                                transaction.Rollback();
                                LogUpdate("UpdateUserTransaction", id, false);
                                return NotFound("Пользователь не найден.");
                            }
                        }

                        string logQuery = """
                            INSERT INTO UpdateLogs (UserId, ChangedAt, Description)
                            VALUES (@UserId, @ChangedAt, @Description)
                            """;

                        using (var logCommand = new SqliteCommand(
                            logQuery, connection, transaction))
                        {
                            logCommand.Parameters.AddWithValue("@UserId", id);
                            logCommand.Parameters.AddWithValue(
                                "@ChangedAt", DateTimeOffset.UtcNow.ToString("O"));
                            logCommand.Parameters.AddWithValue(
                                "@Description", "Данные пользователя обновлены");

                            logCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            LogUpdate("UpdateUserTransaction", id, true);
            return Ok("Данные пользователя обновлены транзакцией.");
        }
        catch (SqliteException ex)
        {
            LogUpdate("UpdateUserTransaction", id, false);
            _logger.LogError(ex, "Ошибка базы данных при транзакции.");
            return StatusCode(500, "Ошибка базы данных.");
        }
        catch (Exception ex)
        {
            LogUpdate("UpdateUserTransaction", id, false);
            _logger.LogError(ex, "Ошибка транзакции.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }

    // Задание 5: логирование запросов
    private void LogUpdate(string operation, int id, bool success)
    {
        DateTimeOffset requestTime = DateTimeOffset.UtcNow;

        if (success)
        {
            _logger.LogInformation(
                "{RequestTime}: {Operation}, UserId={UserId}, Result=Success",
                requestTime, operation, id);
        }
        else
        {
            _logger.LogWarning(
                "{RequestTime}: {Operation}, UserId={UserId}, Result=Failed",
                requestTime, operation, id);
        }
    }
}
