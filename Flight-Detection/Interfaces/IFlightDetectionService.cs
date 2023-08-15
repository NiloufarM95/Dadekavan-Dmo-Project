using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flight_Detection.Models;

namespace Flight_Detection.Interfaces
{
    public interface IFlightDetectionService
    {
        List<FlightDetectionResult> GetRoutesByAgencyIdAndDuration(InputParameters inputParameters);
    }
}
