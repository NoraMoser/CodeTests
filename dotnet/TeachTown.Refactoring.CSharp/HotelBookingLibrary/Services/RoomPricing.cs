using HotelReservationLibrary;
using HotelReservationLibrary.Services;

namespace HotelReservationLibrary.Services {
public class RoomPricing : IRoomPricing, ISmokingPreference
{
    private readonly string _roomType;
    private string _smokingOrNonSmoking = "Not Set";

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

        public double CalculateTotal(Reservation reservation)
        {
            double _pricePerNight = GetPrice();
            int numberOfNights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
            return numberOfNights * _pricePerNight;
        }
        public void SetSmokingPreference(string preference)
        {
            var validPreferences = new[] { "Smoking", "Non-Smoking" };
            if (!validPreferences.Contains(preference))
                throw new ArgumentException("Invalid smoking preference. Choose 'Smoking' or 'Non-Smoking'.");

            _smokingOrNonSmoking = preference;
        }

        // getter for the smoking preference if needed elsewhere
        public string GetSmokingPreference()
        {
            return _smokingOrNonSmoking ?? "Not Set";
        }

}
}
