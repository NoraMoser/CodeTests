using HotelReservationLibrary;
using HotelReservationLibrary.Services;
using HotelReservationLibrary.Data;

namespace HotelReservationClient
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IWeatherApi weatherApi = new ExternalWeatherApi();
            IReservationDb reservationDb = new ReservationDb("MyConnectionString");

            var reservationService = new ReservationService(weatherApi, reservationDb);
            //RoomType is required in constructor now
            var reservation = new Reservation("Single") {
                GuestFirstName = "Bobby",
                GuestLastName = "Tables",
                GuestEmail = "wearehiring@teachtown.com",
                CheckInDate = new DateTime(2022, 1, 1),
                CheckOutDate = new DateTime(2022, 1, 8),
                NumberOfAdditionalGuests = 1,
            };
            //this is private so gotta pass it
            reservation.SetSmokingPreference("Non-Smoking");
            var resevationNumber = reservationService.BookReservation(reservation);
            Console.WriteLine("Reservation number: " + resevationNumber);
        }
    }
}
