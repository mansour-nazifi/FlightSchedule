using FlightSchedule.Domain.Entities;
using FlightSchedule.Domain.Repositories;
using FlightSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Infrastructure.Repositories
{
    public class RouteRepository : BaseRepository<Route>, IRouteRepository
    {
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<Route>> GetRoutesAsync(int originCityId, int destinationCityId, DateTime startDate, DateTime endDate)
        {
            return GetRoutesAsync([originCityId], [destinationCityId], startDate, endDate);
        }

        public Task<List<Route>> GetRoutesAsync(int[] originCityId, int[] destinationCityId, DateTime startDate, DateTime endDate)
        {
            return _context.Routes.AsNoTracking()
                .Include(x => x.Flights.Where(r => r.DepartureTime >= startDate && r.DepartureTime <= endDate))
                .Where(r =>
                    originCityId.Any(x => x == r.OriginCityId) &&
                    destinationCityId.Any(x => x == r.DestinationCityId) &&
                    r.DepartureDate >= startDate &&
                    r.DepartureDate <= endDate)
                .ToListAsync();
        }
    }
}
