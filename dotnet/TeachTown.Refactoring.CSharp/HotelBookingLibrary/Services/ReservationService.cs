using HotelReservationLibrary.Data;

namespace HotelReservationLibrary
{
    public class ReservationService
    {
        //(below) better to put these in a constructor to make it more testable, etc
        private readonly IWeatherApi _weatherApi;
        private readonly IReservationDb _reservationDb;

        // constructor inject dependencies - doing it this way for backwards compatibility
        public ReservationService(IWeatherApi weatherApi, IReservationDb reservationDb)
        {
            _weatherApi = weatherApi ?? throw new ArgumentNullException(nameof(weatherApi));
            _reservationDb = reservationDb ?? throw new ArgumentNullException(nameof(reservationDb));
        }

        // Default constructor for backward compatibility
        // note I used the Mock db here so I could test it but for you, use the regular ReservationDb with a connection string
        public ReservationService() : this(new ExternalWeatherApi(), new ReservationDb("whateverthestringis"))
        {
        }

        //reservation was spelled reservashin which doesn't affect functionality but makes it more confusing for other teammates to read
        public long BookReservation(Reservation reservation)
        {
            ValidateReservation(reservation);

            reservation.GetPricePerNight();

            double total = reservation.Total;

            total = ChangeForWeather(reservation, total);

            return _reservationDb.AddReservation(reservation, total);
        }

        //separating validation and booking so each method is just doing one thing so easier to read and test. also instead of returning 0 for null, we throw an error to make it easier to debug.
        private static void ValidateReservation(Reservation reservation)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            if (string.IsNullOrEmpty(reservation.GuestFirstName))
                throw new ArgumentException("First name is required.");
            
            if (string.IsNullOrEmpty(reservation.GuestLastName))
                throw new ArgumentException("Last name is required.");

            if (!reservation.GuestEmail.Contains('@'))
                throw new ArgumentException("Invalid email address.");
            
            if (reservation.CheckOutDate <= reservation.CheckInDate)
                throw new ArgumentException("Check-out date must be later than check-in date.");

            if (reservation.NumberOfAdditionalGuests > 2)
                throw new ArgumentException("Number of additional guests cannot exceed 2.");
        }

        private double ChangeForWeather(Reservation reservation, double total)
        {
            try
            {
                var forecast = _weatherApi.GetForecast(DateOnly.FromDateTime(reservation.CheckInDate), DateOnly.FromDateTime(reservation.CheckOutDate));

                if (forecast.Summary == "Freezing" || forecast.Summary == "Sweltering")
                {
                    total *= 1.2;
                }
            }
            catch (Exception ex)
            {
               //do something with the ex like log - i would add a logging library but feels out of scope
               File.AppendAllText("errors.log", $"{DateTime.Now}: {ex}\n");
            }
            return total;
        } 
    }  
}
