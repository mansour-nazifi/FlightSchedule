using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Dto.Csv
{
    public class FlightDto
    {
        public int flight_id { get;   set; }
        public int route_id { get;   set; }
        public DateTime departure_time { get;   set; }
        public DateTime arrival_time { get;   set; }
        public int airline_id { get;   set; }
    }
}
