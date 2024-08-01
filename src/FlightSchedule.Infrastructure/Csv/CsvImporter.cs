using CsvHelper;
using FlightSchedule.Domain.Entities;
using FlightSchedule.Domain.Repositories;
using FlightSchedule.Domain.Services;
using System.Globalization;
namespace FlightSchedule.Infrastructure.Csv
{
    public class CsvImporter
    {
        private readonly IBulkService<Route> _bulkRouteService;
        private readonly IBulkService<Flight> _bulkFlightService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBulkService<Subscription> _bulkSubscriptionService;

        public CsvImporter(IUnitOfWork unitOfWork, IBulkService<Subscription> bulkSubscriptionService, IBulkService<Route> bulkRouteService, IBulkService<Flight> bulkFlightService)
        {
            _unitOfWork = unitOfWork;
            _bulkSubscriptionService = bulkSubscriptionService;
            _bulkRouteService = bulkRouteService;
            _bulkFlightService = bulkFlightService;
        }
        public async Task Import()
        {
            Console.WriteLine("Importing routes.csv...");
            await ImportRoutesAsync("Data/CSV/routes.csv");

            Console.WriteLine("Importing flights.csv...");
            await ImportFlightsAsync("Data/CSV/flights.csv");

            Console.WriteLine("Importing subscriptions.csv...");
            await ImportSubscriptionsAsync("Data/CSV/subscriptions.csv");
        }

        async Task ImportRoutesAsync(string filePath)
        {
            if (await _unitOfWork.Routes.CountAsync() > 0)
                return;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var items = csv.GetRecords<Domain.Dto.Csv.RouteDto>()
                    .Select(record => new Route
                    {
                        Id = record.route_id,
                        OriginCityId = record.origin_city_id,
                        DestinationCityId = record.destination_city_id,
                        DepartureDate = record.departure_date
                    }).ToList();

                await _bulkRouteService.BulkInsertAsync(items);
            }
        }
        async Task ImportFlightsAsync(string filePath)
        {
            if (await _unitOfWork.Flights.CountAsync() > 0)
                return;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {                 
                var items = csv.GetRecords<Domain.Dto.Csv.FlightDto>()
                    .Select(record => new Flight
                    {
                        Id = record.flight_id,
                        RouteId = record.route_id,
                        DepartureTime = record.departure_time,
                        ArrivalTime = record.arrival_time,
                        AirlineId = record.airline_id
                    }).ToList();

                await _bulkFlightService.BulkInsertAsync(items);
            }
        }
        async Task ImportSubscriptionsAsync(string filePath)
        {
            if (await _unitOfWork.Subscriptions.CountAsync() > 0)
                return;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var items = csv.GetRecords<Domain.Dto.Csv.SubscriptionDto>()
                    .Select(record => new Subscription
                    {
                        AgencyId = record.agency_id,
                        OriginCityId = record.origin_city_id,
                        DestinationCityId = record.destination_city_id
                    }).ToList();

                await _bulkSubscriptionService.BulkInsertAsync(items);
            }
        }
    }
}