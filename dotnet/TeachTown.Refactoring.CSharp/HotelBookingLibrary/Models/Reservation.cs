namespace HotelReservationLibrary
{
    public class Reservation
    {
        public required string GuestFirstName { get; set; }
        public required string GuestLastName { get; set; }        
        public required string GuestEmail { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfAdditionalGuests { get; set; }
        //(below) this is required but private setter so i'm using the constructor to enforce val
        public string RoomType { get; private set; }
        public Reservation(string roomType)
        {
            SetRoomType(roomType);
        }

        public string SmokingOrNonSmoking { get; private set; }

        private double _pricePerNight = 100;
        //(below)we had to put total in a method bc it was internal so exposed within assembly - this way it's not a settable prop 
        public double Total => CalculateTotal();
        //(below)Room Type and smoking preference are strings so we need to validate them to make sure they are valid valiues (below)
        public void SetRoomType(string roomType)
        {
            var validTypes = new[] { "Single", "Double", "Suite" };
            if (!validTypes.Contains(roomType))
                throw new ArgumentException("Invalid room type.");
            
            RoomType = roomType;
            _pricePerNight = GetPricePerNight();
        }

        public void SetSmokingPreference(string preference)
        {
            var validPreferences = new[] { "Smoking", "Non-Smoking" };
            if (!validPreferences.Contains(preference))
                throw new ArgumentException("Invalid smoking preference. Choose 'Smoking' or 'Non-Smoking'.");

            SmokingOrNonSmoking = preference;
        }
        //default value should change based on room type
        public double GetPricePerNight()
        {
            return RoomType switch
            {
                "Single" => 100,
                "Double" => 150,
                "Suite" => 250,
                _ => 100
            };
        }

        public double CalculateTotal()
        {
            int numberOfNights = (CheckOutDate - CheckInDate).Days;
            return numberOfNights * _pricePerNight;
        }
    }
}
