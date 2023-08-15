using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flight_Detection.Enums;
using Flight_Detection.Interfaces;
using Flight_Detection.Models;

namespace Flight_Detection.Services
{
    public class FlightDetectionService : IFlightDetectionService
    {
        public List<FlightDetectionResult> GetRoutesByAgencyIdAndDuration(InputParameters inputParameters)
        {
            List<FlightDetectionResult> lstFlightDetectionResults = new List<FlightDetectionResult>();

            using (AppDbContext context = new AppDbContext())
            {
                var lstRoute = context.Route
                .Join(context.Subscription,
                    r => new { r.DestinationCityId, r.OriginCityId },
                    s => new { s.DestinationCityId, s.OriginCityId },
                    (r, s) => new { Route = r, Subscription = s })
                .Where(sr => sr.Subscription.AgencyId == inputParameters.AgencyId &&
                             (sr.Route.DepartureDate.Date >= inputParameters.StartDate.Date &&
                              sr.Route.DepartureDate.Date <= inputParameters.EndDate.Date))
                .Select(sr => sr.Route).Distinct().ToList();

                if (lstRoute.Count == 0)
                {
                    return new List<FlightDetectionResult>();
                }

                var lstRoutId = lstRoute.Select(p => p.RouteId).ToList();

                var filteredFlights = context.Flight
                    .Where(p => lstRoutId.Contains(p.RouteId))
                .ToList();

                lstFlightDetectionResults = GetFlightDetectionResults(filteredFlights, lstFlightDetectionResults, lstRoute);
            }

            return lstFlightDetectionResults;
        }

        private static List<FlightDetectionResult> GetFlightDetectionResults(List<Flight> filteredFlights, List<FlightDetectionResult> lstFlightDetectionResults, List<Route> lstRoute)
        {
            var airlineFlights = filteredFlights.GroupBy(p => p.AirlineId);

            DetectFlightType(lstFlightDetectionResults, lstRoute, airlineFlights);

            lstFlightDetectionResults = lstFlightDetectionResults.OrderBy(p => p.DepartureDate).ToList();

            return lstFlightDetectionResults;
        }

        private static void DetectFlightType(List<FlightDetectionResult> lstFlightDetectionResults, List<Route> lstRoute, IEnumerable<IGrouping<int, Flight>> airlineFlights)
        {
            TimeSpan beforeTimeSpan = new TimeSpan(6, 23, 30, 0, 0);
            TimeSpan afterTimeSpan = new TimeSpan(7, 0, 30, 0, 0);

            foreach (var airlineFlight in airlineFlights)
            {
                foreach (var flight in airlineFlight)
                {
                    if (!airlineFlight.Any(p =>
                        p.DepartureTime <= flight.DepartureTime.Subtract(beforeTimeSpan) &&
                        p.DepartureTime >= flight.DepartureTime.Subtract(afterTimeSpan)))
                    {
                        GetFlightType(lstFlightDetectionResults, lstRoute, flight, FlightStatusEnum.New.ToString());
                    }
                    else if (!airlineFlight.Any(p =>
                        p.DepartureTime >= flight.DepartureTime.Add(beforeTimeSpan) &&
                        p.DepartureTime <= flight.DepartureTime.Add(afterTimeSpan)))
                    {
                        GetFlightType(lstFlightDetectionResults, lstRoute, flight, FlightStatusEnum.Discontinued.ToString());
                    }
                    else
                    {
                        GetFlightType(lstFlightDetectionResults, lstRoute, flight, "");
                    }
                }
            }
        }

        private static void GetFlightType(List<FlightDetectionResult> lstFlightDetectionResults, List<Route> lstRoute, Flight flight, string status)
        {
            lstFlightDetectionResults.Add(new FlightDetectionResult
            {
                ArrivalTime = flight.ArrivalTime,
                AirlineId = flight.AirlineId,
                OriginCityId = lstRoute.FirstOrDefault(p => p.RouteId == flight.RouteId)?.OriginCityId,
                DepartureDate = flight.DepartureTime,
                DestinationCityId = lstRoute.FirstOrDefault(p => p.RouteId == flight.RouteId)?.DestinationCityId,
                FlightId = flight.FlightId,
                Status = status
            });
        }
    }
}
