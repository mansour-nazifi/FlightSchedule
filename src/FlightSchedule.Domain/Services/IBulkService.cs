using FlightSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Services
{
    public interface IBulkService<TEntity> where TEntity : class
    {
        Task BulkInsertAsync(IEnumerable<TEntity> entities);
    }
}
