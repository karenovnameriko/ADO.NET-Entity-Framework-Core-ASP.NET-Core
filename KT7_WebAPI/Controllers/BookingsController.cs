using KT7_WebAPI.Models;
using KT7_WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace KT7_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly Database _database;

    public BookingsController(Database database)
    {
        _database = database;
    }

    [HttpGet("available")]
    public IActionResult Available(
        string resourceType,
        DateTime start,
        DateTime end,
        int count = 10)
    {
        if (start >= end || count < 1)
            return BadRequest("Некорректный период или количество ресурсов.");

        var result = resourceType switch
        {
            "HotelRoom" => new BookingService<HotelRoom>(_database)
                .GetAvailable(start, end, count),
            "FlightSeat" => new BookingService<FlightSeat>(_database)
                .GetAvailable(start, end, count),
            "RestaurantTable" => new BookingService<RestaurantTable>(_database)
                .GetAvailable(start, end, count),
            _ => null
        };

        return result == null
            ? BadRequest("Неизвестный тип ресурса.")
            : Ok(result);
    }

    [HttpPost("{resourceType}")]
    public IActionResult Create(
        string resourceType,
        BookingRequest<HotelRoom> request)
    {
        try
        {
            int id = resourceType switch
            {
                "HotelRoom" => new BookingService<HotelRoom>(_database)
                    .Create(new BookingRequest<HotelRoom>
                    {
                        ResourceId = request.ResourceId,
                        StartAt = request.StartAt,
                        EndAt = request.EndAt
                    }),
                "FlightSeat" => new BookingService<FlightSeat>(_database)
                    .Create(new BookingRequest<FlightSeat>
                    {
                        ResourceId = request.ResourceId,
                        StartAt = request.StartAt,
                        EndAt = request.EndAt
                    }),
                "RestaurantTable" => new BookingService<RestaurantTable>(_database)
                    .Create(new BookingRequest<RestaurantTable>
                    {
                        ResourceId = request.ResourceId,
                        StartAt = request.StartAt,
                        EndAt = request.EndAt
                    }),
                _ => 0
            };

            return id == 0
                ? BadRequest("Неизвестный тип ресурса.")
                : Ok(new { Id = id, Message = "Бронирование создано." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{resourceType}/{id}")]
    public IActionResult Update(
        string resourceType, int id, BookingRequest<HotelRoom> request)
    {
        bool result = resourceType switch
        {
            "HotelRoom" => new BookingService<HotelRoom>(_database).Update(id,
                new BookingRequest<HotelRoom> { ResourceId = request.ResourceId, StartAt = request.StartAt, EndAt = request.EndAt }),
            "FlightSeat" => new BookingService<FlightSeat>(_database).Update(id,
                new BookingRequest<FlightSeat> { ResourceId = request.ResourceId, StartAt = request.StartAt, EndAt = request.EndAt }),
            "RestaurantTable" => new BookingService<RestaurantTable>(_database).Update(id,
                new BookingRequest<RestaurantTable> { ResourceId = request.ResourceId, StartAt = request.StartAt, EndAt = request.EndAt }),
            _ => false
        };

        return result ? Ok("Бронирование обновлено.") : NotFound();
    }

    [HttpDelete("{resourceType}/{id}")]
    public IActionResult Cancel(string resourceType, int id)
    {
        bool result = resourceType switch
        {
            "HotelRoom" => new BookingService<HotelRoom>(_database).Cancel(id),
            "FlightSeat" => new BookingService<FlightSeat>(_database).Cancel(id),
            "RestaurantTable" => new BookingService<RestaurantTable>(_database).Cancel(id),
            _ => false
        };

        return result ? Ok("Бронирование отменено.") : NotFound();
    }
}