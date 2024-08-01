using FlightSchedule.Domain.Entities;
using FlightSchedule.Domain.Repositories;
using FlightSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Infrastructure.Repositories
{
    public class FlightRepository : BaseRepository<Flight>, IFlightRepository
    {
        public FlightRepository(ApplicationDbContext context) : base(context)
        {

        } 
    }
}
