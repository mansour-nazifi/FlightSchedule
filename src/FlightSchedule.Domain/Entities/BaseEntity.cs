using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Entities
{
    public interface BaseEntity
    {

    }

    public abstract class IntEntity : BaseEntity
    {
        public int Id { get; set; }
    }
}
