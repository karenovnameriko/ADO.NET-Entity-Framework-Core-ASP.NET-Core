
using KT7_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace KT7_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly Database _database;

    public StudentsController(Database database)
    {
        _database = database;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            "SELECT * FROM Students", connection);

        using var reader = command.ExecuteReader();

        var students = new List<Student>();

        while (reader.Read())
        {
            students.Add(new Student
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Age = reader.GetInt32(2),
                Email = reader.GetString(3)
            });
        }

        return Ok(students);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            "SELECT * FROM Students WHERE Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return NotFound();

        var student = new Student
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Age = reader.GetInt32(2),
            Email = reader.GetString(3)
        };

        return Ok(student);
    }

    [HttpPost]
    public IActionResult Create(Student student)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            """
            INSERT INTO Students (Name, Age, Email)
            VALUES (@Name, @Age, @Email);
            SELECT last_insert_rowid();
            """, connection);

        command.Parameters.AddWithValue("@Name", student.Name);
        command.Parameters.AddWithValue("@Age", student.Age);
        command.Parameters.AddWithValue("@Email", student.Email);

        student.Id = Convert.ToInt32(command.ExecuteScalar());

        return CreatedAtAction(nameof(GetById),
            new { id = student.Id }, student);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Student student)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            """
            UPDATE Students
            SET Name = @Name, Age = @Age, Email = @Email
            WHERE Id = @Id
            """, connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Name", student.Name);
        command.Parameters.AddWithValue("@Age", student.Age);
        command.Parameters.AddWithValue("@Email", student.Email);

        if (command.ExecuteNonQuery() == 0)
            return NotFound();

        return Ok("Данные студента обновлены.");
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand(
            "DELETE FROM Students WHERE Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", id);

        if (command.ExecuteNonQuery() == 0)
            return NotFound();

        return Ok("Студент удалён.");
    }
}
