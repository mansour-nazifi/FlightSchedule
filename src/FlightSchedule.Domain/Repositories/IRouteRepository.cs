using FlightSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Repositories
{
    public interface IRouteRepository : IRepository<Route>
    {
        Task<List<Route>> GetRoutesAsync(int originCityId, int destinationCityId, DateTime startDate, DateTime endDate);
        Task<List<Route>> GetRoutesAsync(int[] originCityId, int[] destinationCityId, DateTime startDate, DateTime endDate);
    }
}
