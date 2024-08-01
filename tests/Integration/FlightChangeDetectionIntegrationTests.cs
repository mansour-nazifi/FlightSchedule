using FlightSchedule.Application.Services;
using FlightSchedule.Domain.Entities;
using FlightSchedule.Domain.Repositories;
using FlightSchedule.Infrastructure.Data;
using FlightSchedule.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Integration
{
    [TestFixture]
    public class FlightChangeDetectionIntegrationTests
    {
        private ServiceProvider _serviceProvider;
        private ApplicationDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName: "FlightDb"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<FlightChangeDetectionService>();

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();

            SeedData();
        }

        private void SeedData()
        {
            _context.Routes.Add(new Route
            {
                Id = 1,
                OriginCityId = 1,
                DestinationCityId = 2,
                DepartureDate = DateTime.Now
            });

            _context.Flights.Add(new Flight
            {
                Id = 1,
                RouteId = 1,
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                AirlineId = 1
            });

            _context.Subscriptions.Add(new Subscription
            {
                AgencyId = 1,
                OriginCityId = 1,
                DestinationCityId = 2
            });
            _context.SaveChanges();
        }

        [Test]
        public async Task DetectChangesAsync_ShouldReturnChanges()
        {
            // Arrange
            var appService = _serviceProvider.GetRequiredService<FlightChangeDetectionService>();
            var startDate = DateTime.Now.AddDays(-10);
            var endDate = DateTime.Now.AddDays(7);
            var agencyId = 1;

            // Act
            var result = await appService.DetectChangesAsync(startDate, endDate, agencyId);

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _serviceProvider.Dispose();
        }
    }
}