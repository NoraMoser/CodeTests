using NUnit.Framework;
using Moq;
using FluentAssertions;
using HotelReservationLibrary;

namespace HotelReservationLibrary.Tests
{
    [TestFixture]
    public class ReservationServiceTests
    {
        private Mock<IWeatherApi>? _mockWeatherApi;
        private Mock<IReservationDb>? _mockReservationDb;
        private ReservationService? _reservationService;

        [SetUp]
        public void ReservationServiceTestsSetup()
        {
            _mockWeatherApi = new Mock<IWeatherApi>();
            _mockReservationDb = new Mock<IReservationDb>();
            _reservationService = new ReservationService(_mockWeatherApi.Object, _mockReservationDb.Object);
        }

      [Test]
        public void BookReservation_ShouldReturnValidReservationNumber()
        {
            
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };
            
            double expectedTotal = reservation.Total;
            
            _mockReservationDb?
                .Setup(db => db.AddReservation(reservation, expectedTotal))
                .Returns(456435463);

            var reservationNumber = _reservationService?.BookReservation(reservation);

            reservationNumber.Should().BeGreaterThan(0);
            reservationNumber.Should().Be(456435463);
        }

        [Test]
        public void BookReservation_MissingFirstName_ThrowsArgumentException()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
          var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };
           
            Assert.Throws<ArgumentException>(() => reservationService.BookReservation(reservation));
        }


        [Test]
        public void BookReservation_MissingLastName_ThrowsArgumentException()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
          var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };
           
            Assert.Throws<ArgumentException>(() => reservationService.BookReservation(reservation));
        }

            [Test]
        public void BookReservation_IncorrectEmail_ThrowsArgumentException()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhiremeteachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };
           
            Assert.Throws<ArgumentException>(() => reservationService.BookReservation(reservation));
        }

        [Test]
        public void BookReservation_CheckOutBeforeCheckIn_ThrowsArgumentException()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };
    
            Assert.Throws<ArgumentException>(() => reservationService.BookReservation(reservation));
        }

        [Test]
        public void BookReservation_WeatherFreezing_AdjustsTotalPrice()
        {
            // Arrange
            var weatherApiMock = new Mock<IWeatherApi>();
            weatherApiMock.Setup(api => api.GetForecast(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(new WeatherForecast
            {
                Summary = "Freezing",
                TemperatureC = -10
            });

            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };
    
            reservationService.BookReservation(reservation);

            reservationDbMock.Verify(db => db.AddReservation(reservation, It.Is<double>(total => total == reservation.Total * 1.2)), Times.Once);
        }

        [Test]
        public void BookReservation_WeatherSweltering_AdjustsTotalPrice()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            weatherApiMock.Setup(api => api.GetForecast(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(new WeatherForecast
            {
                Summary = "Sweltering",
                TemperatureC = 40
            });

            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };

            reservationService.BookReservation(reservation);

            reservationDbMock.Verify(db => db.AddReservation(reservation, It.Is<double>(total => total == reservation.Total * 1.2)), Times.Once);
        }

        [Test]
        public void BookReservation_WeatherApiThrowsException_ReturnsOriginalTotal()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            weatherApiMock.Setup(api => api.GetForecast(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Throws(new Exception("Weather API error"));

            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@eachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };
          
            reservationService.BookReservation(reservation);

            reservationDbMock.Verify(db => db.AddReservation(reservation, It.Is<double>(total => total == reservation.Total)), Times.Once);
        }

        [Test]
        public void Reservation_ShouldThrowException_WhenInvalidRoomTypeIsSetInObjectInitialization()
        {
            // directly initialize the reservation with an invalid RoomType
            Action act = () => new Reservation()
            {
                RoomType = "BigOne",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1),
                CheckOutDate = new DateTime(2025, 1, 1).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };

            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid room type.");
        }


        [Test]
        public void SetSmokingPreference_ShouldThrowException_WhenInvalidPreference()
        {
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };

            reservation.Invoking(r => r.SetSmokingPreference("NiceAndSmelly"))
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Invalid smoking preference. Choose 'Smoking' or 'Non-Smoking'.");
        }


        [Test]
        public void CalculateTotal_ShouldReturnCorrectAmount()
        {
    
            var reservation = new Reservation()
            {
                RoomType = "Single",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(3),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };

            double expectedTotal = 100 * 3;

            var total = reservation.Total;

            total.Should().Be(expectedTotal);
        }

        [Test]
        public void CalculateTotal_ShouldAdjustBasedOnRoomzSizeandDates()
        {
            var reservation = new Reservation()
            {
                RoomType = "Double",
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhiremet@eachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
                SmokingOrNonSmoking = "Non-Smoking"
            };

            double expectedTotal = 150 * 4;

            var total = reservation.Total;

            total.Should().Be(expectedTotal);
        }


    }
}
