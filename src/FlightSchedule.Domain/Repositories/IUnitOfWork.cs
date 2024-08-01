using FlightSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IFlightRepository Flights { get; }
        IRouteRepository Routes { get; }
        ISubscriptionRepository Subscriptions { get; }

        Task<int> SaveChangesAsync();        
    }
}
