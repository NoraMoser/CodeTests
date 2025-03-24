using HotelReservationLibrary.Services;

public class RoomPricing : IRoomPricing
{
    private readonly string _roomType;

    public RoomPricing(string roomType)
    {
        _roomType = roomType;
    }

    public double GetPrice()
    {
        return _roomType switch
        {
            "Single" => 100,
            "Double" => 150,
            "Suite" => 250,
            _ => throw new ArgumentException("Invalid room type.")
        };
    }
}
