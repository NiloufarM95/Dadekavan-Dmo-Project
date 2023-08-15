using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flight_Detection.Models
{
    public class RouteFlights
    {
        public int RoutId { get; set; }
        public List<int> FlightIds { get; set; }
    }
}
