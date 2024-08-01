using FlightSchedule.Domain.Dto;
using FlightSchedule.Domain.Entities;
using FlightSchedule.Domain.Repositories;
using System.Collections.Generic;

namespace FlightSchedule.Application.Services
{
    public class FlightChangeDetectionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FlightChangeDetectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FlightChangeDto>> DetectChangesAsync1(DateTime startDate, DateTime endDate, int agencyId)
        {
            var changes = new List<FlightChangeDto>();

            var subscriptions = await _unitOfWork.Subscriptions.GetSubscriptionsByAgencyIdAsync(agencyId);
            var routes = await GetRoutesInRangeAsync(startDate, endDate, subscriptions);
            var flights = routes.SelectMany(x => x.Flights).ToList();

            var airlines = flights.GroupBy(x => x.AirlineId);

            var tolerance = TimeSpan.FromMinutes(30);
            var day = 7;

            object lockObj = new object();

            Parallel.ForEach(airlines, airline =>
            {
                Parallel.ForEach(airline, flight =>
                {
                    var hasPreviousWeek = HasFlight(airline, flight, -day, tolerance);
                    if (!hasPreviousWeek)
                    {
                        lock (lockObj)
                        {
                            changes.Add(FlightToChangeDto(flight, "New"));
                        }
                    }
                    else
                    {
                        var nextWeek = !HasFlight(airline, flight, day, tolerance);
                        if (nextWeek)
                        {
                            lock (lockObj)
                            {
                                changes.Add(FlightToChangeDto(flight, "Discontinued"));
                            }
                        }
                    }
                });
            });

            return changes;
        }

        public async Task<List<FlightChangeDto>> DetectChangesAsync(DateTime startDate, DateTime endDate, int agencyId)
        {
            var changes = new List<FlightChangeDto>();

            var subscriptions = await _unitOfWork.Subscriptions.GetSubscriptionsByAgencyIdAsync(agencyId);
            var routes = await GetRoutesInRangeAsync(startDate, endDate, subscriptions);
            var flights = routes.SelectMany(x => x.Flights).ToList();

            var airlines = flights.GroupBy(x => new { x.AirlineId });

            var tolerance = TimeSpan.FromMinutes(30);
            var day = 7;                                

            foreach (var airline in airlines)
            {
                var items = airline
                    .GroupBy(x => x.DepartureTime.Date)
                    .ToDictionary(x => x.Key);

                foreach (var item in items)
                {
                    var hasPreviousWeek = items.TryGetValue(item.Key.AddDays(-day), out var previousItems);
                    var hasNextWeek = items.TryGetValue(item.Key.AddDays(day), out var nextItems);

                    changes.AddRange(!hasPreviousWeek
                        ? item.Value.Select(x=> FlightToChangeDto(x, "New"))
                        : item.Value.Where(flight => !HasFlight(previousItems, flight, -day, tolerance)).Select(x => FlightToChangeDto(x, "New")));

                    changes.AddRange(!hasNextWeek
                        ? item.Value.Select(x => FlightToChangeDto(x, "Discontinued"))
                        : item.Value.Where(flight => !HasFlight(nextItems, flight, day ,tolerance)).Select(x => FlightToChangeDto(x, "Discontinued")));
                }
            } 

            return changes;
        }

        FlightChangeDto FlightToChangeDto(Flight flight, string status)
        {
            return new FlightChangeDto
            {
                flight_id = flight.Id,
                origin_city_id = flight.Route.OriginCityId,
                destination_city_id = flight.Route.DestinationCityId,
                departure_time = flight.DepartureTime,
                arrival_time = flight.ArrivalTime,
                airline_id = flight.AirlineId,
                status = status
            };
        }

        bool HasFlight(IEnumerable<Flight> flights, Flight current, int day, TimeSpan tolerance)
        {
            var date = current.DepartureTime.AddDays(day);

            var lowerBound = date - tolerance;
            var upperBound = date + tolerance;

            return flights.Any(f => f.DepartureTime >= lowerBound && f.DepartureTime <= upperBound);
        }

        //bool HasFlight(IEnumerable<Flight> flights, Flight current, int day, TimeSpan tolerance)
        //{
        //    var date = current.DepartureTime.AddDays(day);

        //    var lowerBound = date - tolerance;
        //    var upperBound = date + tolerance;

        //    return flights.Any(f => f != current && f.DepartureTime >= lowerBound && f.DepartureTime <= upperBound);
        //}

        async Task<IEnumerable<Route>> GetRoutesInRangeAsync(DateTime startDate, DateTime endDate, IEnumerable<Subscription> subscriptions)
        {
            var originCityIds = subscriptions.Select(s => s.OriginCityId).Distinct().ToArray();
            var destinationCityIds = subscriptions.Select(s => s.DestinationCityId).Distinct().ToArray();

            return await _unitOfWork.Routes.GetRoutesAsync(originCityIds, destinationCityIds, startDate, endDate);
        }
    }
}
