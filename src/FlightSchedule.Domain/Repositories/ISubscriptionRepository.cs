using FlightSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Repositories
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<List<Subscription>> GetSubscriptionsByAgencyIdAsync(int agencyId);         
    }
}
