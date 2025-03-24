using HotelReservationLibrary;

namespace HotelReservationLibrary.Services
{
    public interface IRoomPricing
    {
        double GetPrice();
        double CalculateTotal(Reservation reservation);

    }

    public interface IReservationValidator
    {
        void Validate(Reservation reservation);
    }

}
