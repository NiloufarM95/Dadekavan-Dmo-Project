using System;
using Flight_Detection.Enums;

namespace Flight_Detection.Models
{
    public class FlightDetectionResult
    {
        public int FlightId { get; set; }
        public int? OriginCityId { get; set; }
        public int? DestinationCityId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int AirlineId { get; set; }
        public string Status { get; set; } = "";
    }
}
