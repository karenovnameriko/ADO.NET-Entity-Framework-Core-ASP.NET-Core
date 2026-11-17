namespace KT7_WebAPI.Models;

public class HotelRoom { }
public class FlightSeat { }
public class RestaurantTable { }

public class BookingRequest<TResource>
{
    public int ResourceId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}

public class Booking<TResource>
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}