
using KT7_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace KT7_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly Database _database;

    public TasksController(Database database)
    {
        _database = database;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? status)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        string query = string.IsNullOrWhiteSpace(status)
            ? "SELECT * FROM Tasks"
            : "SELECT * FROM Tasks WHERE Status = @Status";

        using var command = new SqliteCommand(query, connection);

        if (!string.IsNullOrWhiteSpace(status))
            command.Parameters.AddWithValue("@Status", status);

        using var reader = command.ExecuteReader();
        var tasks = new List<TaskItem>();

        while (reader.Read())
        {
            tasks.Add(new TaskItem
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Status = reader.GetString(3)
            });
        }

        return Ok(tasks);
    }

    [HttpPost]
    public IActionResult Create(TaskItem task)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            """
            INSERT INTO Tasks (Title, Description, Status)
            VALUES (@Title, @Description, @Status);
            SELECT last_insert_rowid();
            """, connection);

        command.Parameters.AddWithValue("@Title", task.Title);
        command.Parameters.AddWithValue("@Description", task.Description);
        command.Parameters.AddWithValue("@Status", "Новая");

        task.Id = Convert.ToInt32(command.ExecuteScalar());
        task.Status = "Новая";

        return Ok(task);
    }

    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromQuery] string status)
    {
        var allowedStatuses = new[] { "Новая", "В работе", "Выполнена" };

        if (!allowedStatuses.Contains(status))
            return BadRequest("Недопустимый статус.");

        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            "UPDATE Tasks SET Status = @Status WHERE Id = @Id",
            connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Status", status);

        if (command.ExecuteNonQuery() == 0)
            return NotFound();

        return Ok("Статус задачи обновлён.");
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            "DELETE FROM Tasks WHERE Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", id);

        if (command.ExecuteNonQuery() == 0)
            return NotFound();

        return Ok("Задача удалена.");
    }
}
