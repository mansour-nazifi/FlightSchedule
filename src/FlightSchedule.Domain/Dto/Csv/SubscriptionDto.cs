﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSchedule.Domain.Dto.Csv
{
    public class SubscriptionDto
    {
        public int agency_id { get;   set; }
        public int origin_city_id { get;   set; }
        public int destination_city_id { get;   set; }
    }
}
