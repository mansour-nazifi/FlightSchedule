using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Entities
{
    public class Route : IntEntity
    {
        public int OriginCityId { get; set; }
        public int DestinationCityId { get; set; }
        public DateTime DepartureDate { get; set; }

        public IList<Flight> Flights { get; set; }
    }
}
