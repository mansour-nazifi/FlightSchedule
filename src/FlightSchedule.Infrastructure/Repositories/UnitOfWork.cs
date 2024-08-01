using FlightSchedule.Domain.Repositories;
using FlightSchedule.Infrastructure.Data;

namespace FlightSchedule.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Flights = new FlightRepository(_context);
            Routes = new RouteRepository(_context);
            Subscriptions = new SubscriptionRepository(_context);
        }

        public IFlightRepository Flights { get; private set; }
        public IRouteRepository Routes { get; private set; }
        public ISubscriptionRepository Subscriptions { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
