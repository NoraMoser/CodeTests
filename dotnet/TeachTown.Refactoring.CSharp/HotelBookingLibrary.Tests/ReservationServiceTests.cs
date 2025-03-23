using NUnit.Framework;
using Moq;
using FluentAssertions;
using HotelReservationLibrary;
using HotelReservationLibrary.Services;

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
            
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
        
                reservation.SetSmokingPreference("Non-Smoking");
            
            double expectedTotal = reservation.CalculateTotal();
            
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
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
        
                reservation.SetSmokingPreference("Non-Smoking");
           
            Assert.Throws<ArgumentException>(() => reservationService.BookReservation(reservation));
        }


        [Test]
        public void BookReservation_MissingLastName_ThrowsArgumentException()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
        
                reservation.SetSmokingPreference("Non-Smoking");
           
            Assert.Throws<ArgumentException>(() => reservationService.BookReservation(reservation));
        }

            [Test]
        public void BookReservation_IncorrectEmail_ThrowsArgumentException()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhiremeteachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
        
                reservation.SetSmokingPreference("Non-Smoking");
           
            Assert.Throws<ArgumentException>(() => reservationService.BookReservation(reservation));
        }

        [Test]
        public void BookReservation_CheckOutBeforeCheckIn_ThrowsArgumentException()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation("Single")
                    {
                        GuestFirstName = "Nora",
                        GuestLastName = "Moser",
                        GuestEmail = "youshouldhireme@teachtown.com",
                        CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                        CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                        NumberOfAdditionalGuests = 1,
                    };
        
                reservation.SetSmokingPreference("Non-Smoking");
    
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
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
                reservation.SetSmokingPreference("Non-Smoking");
    
            reservationService.BookReservation(reservation);

            reservationDbMock.Verify(db => db.AddReservation(reservation, It.Is<double>(total => total == reservation.CalculateTotal() * 1.2)), Times.Once);
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
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
                reservation.SetSmokingPreference("Non-Smoking");

            reservationService.BookReservation(reservation);

            reservationDbMock.Verify(db => db.AddReservation(reservation, It.Is<double>(total => total == reservation.CalculateTotal() * 1.2)), Times.Once);
        }

        [Test]
        public void BookReservation_WeatherApiThrowsException_ReturnsOriginalTotal()
        {
            var weatherApiMock = new Mock<IWeatherApi>();
            weatherApiMock.Setup(api => api.GetForecast(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Throws(new Exception("Weather API error"));

            var reservationDbMock = new Mock<IReservationDb>();
            var reservationService = new ReservationService(weatherApiMock.Object, reservationDbMock.Object);
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
                reservation.SetSmokingPreference("Non-Smoking");
          
            reservationService.BookReservation(reservation);

            reservationDbMock.Verify(db => db.AddReservation(reservation, It.Is<double>(total => total == reservation.CalculateTotal())), Times.Once);
        }



        [Test]
        public void SetRoomType_ShouldThrowException_WhenInvalidRoomType()
        {
            //have to do it right first so we don't get an exception before the test can run
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
            reservation.SetSmokingPreference("Non-Smoking");
        
            reservation.Invoking(r => r.SetRoomType("BigOne"))
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Invalid room type.");
        }

        [Test]
        public void SetSmokingPreference_ShouldThrowException_WhenInvalidPreference()
        {
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
            //same comment as above here
            reservation.SetSmokingPreference("Non-Smoking");

            reservation.Invoking(r => r.SetSmokingPreference("NiceAndSmelly"))
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Invalid smoking preference. Choose 'Smoking' or 'Non-Smoking'.");
        }


        [Test]
        public void CalculateTotal_ShouldReturnCorrectAmount()
        {
    
            var reservation = new Reservation("Single")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(3),
                NumberOfAdditionalGuests = 1,
            };
            reservation.SetSmokingPreference("Non-Smoking");

            double expectedTotal = 100 * 3;

            var total = reservation.CalculateTotal();

            total.Should().Be(expectedTotal);
        }

        [Test]
        public void CalculateTotal_ShouldAdjustBasedOnRoomzSizeandDates()
        {
            var reservation = new Reservation("Double")
            {
                GuestFirstName = "Nora",
                GuestLastName = "Moser",
                GuestEmail = "youshouldhireme@teachtown.com",
                CheckInDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local),
                CheckOutDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local).AddDays(4),
                NumberOfAdditionalGuests = 1,
            };
            reservation.SetSmokingPreference("Non-Smoking");

            double expectedTotal = 150 * 4;

            var total = reservation.CalculateTotal();

            total.Should().Be(expectedTotal);
        }


    }
}
