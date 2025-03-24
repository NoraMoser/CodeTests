using HotelReservationLibrary.Services;

namespace HotelReservationLibrary
{
    public class Reservation
    {
        //dependency injection here makes it so class can easily be extended
        private IRoomPricing _pricingService;
        private string _roomType = "Single";
        private double _pricePerNight;          

        public required string GuestFirstName { get; set; }
        public required string GuestLastName { get; set; }        
        public required string GuestEmail { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfAdditionalGuests { get; set; }
        //this makes it backwards compatible with program.cs
        public Reservation() : this(new RoomPricing("Single")) { }
        // constructor for injecting pricing service
        public Reservation(RoomPricing pricingService)
        {
           _pricingService = pricingService;
           _pricePerNight = _pricingService.GetPrice(); 
        
        }
        public required string RoomType
        {
            get => _roomType;
            set
            {
                var validTypes = new[] { "Single", "Double", "Suite" };
                if (!validTypes.Contains(value))
                    throw new ArgumentException("Invalid room type.");

                _roomType = value;
                //this updates the room type before running the get price method
                _pricingService = new RoomPricing(_roomType);
                _pricePerNight = _pricingService.GetPrice();
            }
        }
        public double GetPricePerNight()
        {
            return _pricingService.GetPrice(); // Fetch price from RoomPricing service
        }
        public required string SmokingOrNonSmoking { get; set; }
        //Room Type and smoking preference are strings so we need to validate them to make sure they are valid valiues (below)
        public void SetSmokingPreference(string preference)
        {
            if (_pricingService is ISmokingPreference smokingPreference)
            {
                smokingPreference.SetSmokingPreference(preference);
            }
        }

        // Get smoking preference
        public string GetSmokingPreference()
        {
            if (_pricingService is ISmokingPreference smokingPreference)
            {
                return smokingPreference.GetSmokingPreference();
            }
            return "Not Set";
        }
        //(below)we had to put total in a method bc it was internal so exposed within assembly - this way it's not a settable prop 
       public double Total => _pricingService.CalculateTotal(this);
    }
}
