using KT7_WebAPI.Models;
using Microsoft.Data.Sqlite;

namespace KT7_WebAPI.Services;

public class BookingService<TResource>
{
    private readonly Database _database;
    private readonly string _resourceType = typeof(TResource).Name;

    public BookingService(Database database)
    {
        _database = database;
    }

    public List<int> GetAvailable(DateTime start, DateTime end, int count)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand("""
            SELECT ResourceId FROM Reservations
            WHERE ResourceType = @Type
            AND StartAt < @End AND EndAt > @Start
            """, connection);

        command.Parameters.AddWithValue("@Type", _resourceType);
        command.Parameters.AddWithValue("@Start", start.ToString("O"));
        command.Parameters.AddWithValue("@End", end.ToString("O"));

        using var reader = command.ExecuteReader();
        var occupied = new HashSet<int>();

        while (reader.Read())
            occupied.Add(reader.GetInt32(0));

        return Enumerable.Range(1, count)
            .Where(id => !occupied.Contains(id))
            .ToList();
    }

    public int Create(BookingRequest<TResource> request)
    {
        if (request.StartAt >= request.EndAt)
            throw new ArgumentException("Некорректный период бронирования.");

        using var connection = _database.GetConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            using var check = new SqliteCommand("""
                SELECT COUNT(*) FROM Reservations
                WHERE ResourceType = @Type
                AND ResourceId = @ResourceId
                AND StartAt < @End AND EndAt > @Start
                """, connection, transaction);

            AddPeriodParameters(check, request.ResourceId,
                request.StartAt, request.EndAt);

            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                throw new InvalidOperationException("Ресурс уже забронирован.");

            using var command = new SqliteCommand("""
                INSERT INTO Reservations
                (ResourceType, ResourceId, StartAt, EndAt)
                VALUES (@Type, @ResourceId, @Start, @End);
                SELECT last_insert_rowid();
                """, connection, transaction);

            AddPeriodParameters(command, request.ResourceId,
                request.StartAt, request.EndAt);

            int id = Convert.ToInt32(command.ExecuteScalar());
            transaction.Commit();

            return id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public bool Update(int id, BookingRequest<TResource> request)
    {
        if (request.StartAt >= request.EndAt)
            throw new ArgumentException("Некорректный период бронирования.");

        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand("""
            UPDATE Reservations
            SET ResourceId = @ResourceId,
                StartAt = @Start,
                EndAt = @End
            WHERE Id = @Id AND ResourceType = @Type
            AND NOT EXISTS (
                SELECT 1 FROM Reservations
                WHERE ResourceType = @Type
                AND ResourceId = @ResourceId
                AND StartAt < @End AND EndAt > @Start
                AND Id != @Id
            )
            """, connection);

        AddPeriodParameters(command, request.ResourceId,
            request.StartAt, request.EndAt);
        command.Parameters.AddWithValue("@Id", id);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Cancel(int id)
    {
        using var connection = _database.GetConnection();
        connection.Open();

        using var command = new SqliteCommand("""
            DELETE FROM Reservations
            WHERE Id = @Id AND ResourceType = @Type
            """, connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Type", _resourceType);

        return command.ExecuteNonQuery() > 0;
    }

    private void AddPeriodParameters(
        SqliteCommand command,
        int resourceId,
        DateTime start,
        DateTime end)
    {
        command.Parameters.AddWithValue("@Type", _resourceType);
        command.Parameters.AddWithValue("@ResourceId", resourceId);
        command.Parameters.AddWithValue("@Start", start.ToString("O"));
        command.Parameters.AddWithValue("@End", end.ToString("O"));
    }
}