using HotelReservationLibrary;
using Microsoft.Data.SqlClient;

namespace HotelReservationLibrary.Data
{
//we want a separate interface bc it's best to have separate database operations
public class ReservationDb : IReservationDb
{
    //this keeps it private (the connection string)
    private readonly string _connectionString;

    public ReservationDb(string connectionString)
    {
        _connectionString = connectionString;
    }

    //was static so hard to mock for unit tests
    public long AddReservation(Reservation reservation, double total)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Reservations (GuestFirstName, GuestLastName, GuestEmail, CheckInDate, CheckOutDate, NumberOfAdditionalGuests, RoomType, SmokingOrNonSmoking, Total) " +
                                  "VALUES (@GuestFirstName, @GuestLastName, @GuestEmail, @CheckInDate, @CheckOutDate, @NumberOfAdditionalGuests, @RoomType, @SmokingOrNonSmoking, @Total)";
            command.Parameters.AddWithValue("@GuestFirstName", reservation.GuestFirstName);
            command.Parameters.AddWithValue("@GuestLastName", reservation.GuestLastName);
            //needed to the capitalized bc that is what it's called in reservation class
            command.Parameters.AddWithValue("@GuestEmail", reservation.GuestEmail);
            command.Parameters.AddWithValue("@CheckInDate", reservation.CheckInDate);
            command.Parameters.AddWithValue("@CheckOutDate", reservation.CheckOutDate);
            command.Parameters.AddWithValue("@NumberOfAdditionalGuests", reservation.NumberOfAdditionalGuests);
            command.Parameters.AddWithValue("@RoomType", reservation.RoomType);
            command.Parameters.AddWithValue("@SmokingOrNonSmoking", reservation.SmokingOrNonSmoking);
            command.Parameters.AddWithValue("@Total", total);
            command.ExecuteNonQuery();
        }

        return DateTime.Now.Ticks;
    }
}
}