using HotelReservationLibrary;

public interface IReservationDb
{
    long AddReservation(Reservation reservation, double total);
}
