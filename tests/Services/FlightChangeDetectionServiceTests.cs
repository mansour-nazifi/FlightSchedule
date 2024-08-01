using FlightSchedule.Application.Services;
using FlightSchedule.Domain.Entities;
using FlightSchedule.Domain.Repositories;
using Moq;

namespace Services
{
    [TestFixture]
    public class FlightChangeDetectionServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private FlightChangeDetectionService _flightChangeDetectionService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _flightChangeDetectionService = new FlightChangeDetectionService(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task DetectChangesAsync_ShouldReturnChanges()
        {
            //// Arrange
            //var agencyId = 1;
            //var startDate = DateTime.Now;
            //var endDate = DateTime.Now.AddDays(7);

            //var subscriptions = new List<Subscription>
            //{
            //    new Subscription{ AgencyId=agencyId,OriginCityId= 1,DestinationCityId= 2 }
            //};

            //var routes = new List<Route>
            //{
            //    new Route{ Id = 1, DestinationCityId = 2, OriginCityId = 1, DepartureDate = startDate }
            //};

            //var flights = new List<Flight>
            //{
            //    new Flight{ Id = 1, RouteId = 1, DepartureTime = startDate.AddHours(1), ArrivalTime = startDate.AddHours(2), AirlineId = 1 }
            //};

            //_unitOfWorkMock.Setup(u => u.Subscriptions.GetSubscriptionsByAgencyIdAsync(agencyId))
            //               .ReturnsAsync(subscriptions);

            //_unitOfWorkMock.Setup(u => u.Routes.GetRoutesAsync(1, 2, startDate, endDate))
            //               .ReturnsAsync(routes);         

            //// Act
            //var result = await _flightChangeDetectionService.DetectChangesAsync(startDate, endDate, agencyId);

            //// Assert
            //Assert.AreEqual(1, result.Count);
        }
    }
}