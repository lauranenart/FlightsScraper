using FlightsScraper.Helpers;
using FlightsScraper.Models;
using FlightsScraper.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Services
{
    public class FlightService
    {
        private readonly FlightRepository repo;
        private readonly JsonHelper helper;

        public FlightService()
        {
            repo = new FlightRepository();
            helper = new JsonHelper();
        }

        public JObject GetFlightsData(string fromDest, string toDest, string departDate, string returnDate)
        {
            var dataObj = repo.GetAsync($"from={fromDest}&to={toDest}&depart={departDate}&return={returnDate}").Result;
            return dataObj;
        }

        public List<FlightModel> CreateFlights(JObject journey, int connections = 1, string connectionAirport = "")
        {
            List<FlightModel> flights = new List<FlightModel>();
            var flightsJArr = helper.GetListByToken(journey, "flights");

            if (flightsJArr.Count == connections + 1 && connectionAirport != "")
            {
                AddFlightsWithConnAirport(flightsJArr, connectionAirport, ref flights);
            }
            else if (flightsJArr.Count <= connections + 1)
            {
                foreach (var flight in flightsJArr)
                {
                    FlightModel model = GetFlight(flight);
                    flights.Add(model);
                }
            }

            return flights;
        }

        private void AddFlightsWithConnAirport(List<JObject> flightsJArr, string connectionAirport, ref List<FlightModel> flights)
        {
            FlightModel flightFirst = GetFlight(flightsJArr.First());
            FlightModel flightLast = GetFlight(flightsJArr.Last());

            if (flightFirst.AirportArrivCode.Equals(flightLast.AirportDepartCode) && flightFirst.AirportArrivCode.Equals(connectionAirport))
            {
                flights.Add(flightFirst);
                flights.Add(flightLast);
            }
        }

        private FlightModel GetFlight(JObject flightJObj)
        {
            var departureCode = helper.GetValueByToken<string>(flightJObj, "airportDeparture.code");
            var dateDeparture = helper.GetValueByToken<string>(flightJObj, "dateDeparture");
            var arrivalCode = helper.GetValueByToken<string>(flightJObj, "airportArrival.code");
            var dateArrival = helper.GetValueByToken<string>(flightJObj, "dateArrival");

            var companyCode = helper.GetValueByToken<string>(flightJObj, "companyCode");
            var number = helper.GetValueByToken<string>(flightJObj, "number"); 

            var flightNumber = companyCode + number;
            
            FlightModel model = new FlightModel(flightNumber, departureCode, arrivalCode, Convert.ToDateTime(dateDeparture), Convert.ToDateTime(dateArrival));
            return model;
        }

    }
}
