using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Models
{
    public class FlightModel
    {
        [DisplayName("airport departure")]
        public string AirportDepartCode { get; set; }
        [DisplayName("airport arrival")]
        public string AirportArrivCode { get; set; }
        [DisplayName("time departure")]
        public DateTime DateDeparture { get; set; }
        [DisplayName("time arrival")]
        public DateTime DateArrival { get; set; }
        [DisplayName("flight number")]
        public string FlightNumber { get; set; }

        public FlightModel(string flightNumber, string airportDepartCode, string airportArrivCode, DateTime dateDeparture, DateTime dateArrival)
        {
            FlightNumber = flightNumber;
            AirportDepartCode = airportDepartCode;
            AirportArrivCode = airportArrivCode;
            DateDeparture = dateDeparture;
            DateArrival = dateArrival;
        }

        public override string ToString()
        {
            return AirportDepartCode + "," + AirportArrivCode + ","
                + DateDeparture.ToString("yyyy-MM-dd HH:mm") + "," + DateArrival.ToString("yyyy-MM-dd HH:mm") + "," + FlightNumber;
        }
    }
}
