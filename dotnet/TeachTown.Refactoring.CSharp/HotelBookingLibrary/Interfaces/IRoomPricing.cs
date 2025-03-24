using HotelReservationLibrary;

namespace HotelReservationLibrary.Services
{
    public interface IRoomPricing
    {
        double GetPrice();
    }

    public interface IReservationValidator
    {
        void Validate(Reservation reservation);
    }

}
