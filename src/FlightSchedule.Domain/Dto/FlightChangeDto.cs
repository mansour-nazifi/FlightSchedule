using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Dto
{
    public class FlightChangeDto
    {
        public int flight_id { get; set; }
        public int origin_city_id { get; set; }
        public int destination_city_id { get; set; }
        public DateTime departure_time { get; set; }
        public DateTime arrival_time { get; set; }
        public int airline_id { get; set; }
        public string status { get; set; }
    }
}
