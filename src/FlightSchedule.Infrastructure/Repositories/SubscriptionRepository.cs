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
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<Subscription>> GetSubscriptionsByAgencyIdAsync(int agencyId)
        {
            return _context.Subscriptions.AsNoTracking().Where(x => x.AgencyId == agencyId).ToListAsync();
        }
    }
}
