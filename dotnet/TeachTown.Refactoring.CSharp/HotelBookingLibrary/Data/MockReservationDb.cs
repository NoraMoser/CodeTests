using HotelReservationLibrary;
using HotelReservationLibrary.Data;

namespace HotelReservationLibrary.Data
{
    public class MockReservationDb : IReservationDb
    {
        public long AddReservation(Reservation reservation, double total)
        {
            Console.WriteLine("Mock DB: Reservation added successfully.");
            return 12345; // Return a fake reservation number
        }
    }

}
