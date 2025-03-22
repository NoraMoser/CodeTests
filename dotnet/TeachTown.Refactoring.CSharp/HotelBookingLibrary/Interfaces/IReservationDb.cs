using HotelReservationLibrary;

public interface IReservationDb
{
//i think this is out ot the scope of this exercise, but you can add more methods here and and then logic in ReservationsDb so like delete, edit, etc 
    long AddReservation(Reservation reservation, double total);
}
