using FlightsScraper.Helpers;
using FlightsScraper.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Services
{
    public enum Connection
    {
        Direct,
        NonDirect,
        All
    }
    public class JourneyService
    {

        private readonly FlightService service;
        private readonly JsonHelper helper;

        private readonly string inboundDirection = "V";
        private readonly string outboundDirection = "I";

        public JourneyService()
        {
            service = new FlightService();
            helper = new JsonHelper();
        }

        public List<JourneyModel> GetCheapestFlights(List<JourneyModel> roundtripFlights)
        {
            float lowestPrice = GetLowestPrice(roundtripFlights);
            var cheapestFlights = roundtripFlights.Where(x => x.Price == lowestPrice).ToList();
            return cheapestFlights;
        }

        public List<JourneyModel> GetRoundtripFlights(string fromDest, string toDest, string departDate, string returnDate, Connection connections = Connection.All, string connectionAirport = "")
        {
            List<JourneyModel> roundtripFlights = new List<JourneyModel>();
            try
            {
                JObject dataObj = service.GetFlightsData(fromDest, toDest, departDate, returnDate);
                var journeysJArr = helper.GetListByToken(dataObj, "journeys");

                List<Tuple<int, float>> availablePrices = GetAvailablePrices(dataObj);

                List<JourneyModel> outboundFlights = CreateJourneysByDirect(journeysJArr, outboundDirection, availablePrices, connections, connectionAirport);
                List<JourneyModel> inboundFlights = CreateJourneysByDirect(journeysJArr, inboundDirection, availablePrices, connections, connectionAirport);

                roundtripFlights = MakeRoundtripCombinations(outboundFlights, inboundFlights);
            }
            catch (Exception) { throw; }

            return roundtripFlights;
        }

        private List<Tuple<int, float>> GetAvailablePrices(JObject jsonObj)
        {
            var prices = new List<Tuple<int, float>>();

            var availabilities = helper.GetListByToken(jsonObj, "totalAvailabilities");

            foreach (var availability in availabilities)
            {
                var priceTuple = availability.Children();
                prices.Add(new Tuple<int, float>((int)priceTuple.ElementAt(0), (float)priceTuple.ElementAt(1)));
            }

            return prices;
        }

        private List<JourneyModel> CreateJourneysByDirect(List<JObject> journeys, string direction, List<Tuple<int, float>> availablePrices, Connection connections = Connection.All, string connectionAirport = "")
        {
            var journeysByDirect = journeys.Where(journey => (helper.GetValueByToken<string>(journey, "direction"))
            .Equals(direction)).ToList();

            List<JourneyModel> journeysList = new List<JourneyModel>();

            foreach (var journeyJObj in journeysByDirect)
            {
                var taxes = helper.GetValueByToken<float>(journeyJObj, "importTaxAdl");

                var recommendationId = helper.GetValueByToken<int>(journeyJObj, "recommendationId");
                var price = GetJourneyPrice(availablePrices, recommendationId);

                List<FlightModel> flights = service.CreateFlights(journeyJObj, connections, connectionAirport);

                if (flights.Any())
                {
                    JourneyModel journeyModel = new JourneyModel(flights, price, taxes);
                    journeysList.Add(journeyModel);
                }
            }
            return journeysList;
        }

        private List<JourneyModel> MakeRoundtripCombinations(List<JourneyModel> outboundFlights, List<JourneyModel> inboundFlights)
        {
            List<JourneyModel> roundtripFlights = new List<JourneyModel>();

            if (!outboundFlights.Any() || !inboundFlights.Any())
                return roundtripFlights;

            foreach (var outboundFlight in outboundFlights)
            {
                foreach (var inboundFlight in inboundFlights)
                {
                    roundtripFlights.Add(outboundFlight + inboundFlight);
                }
            }
            return roundtripFlights;
        }

        private float GetJourneyPrice(List<Tuple<int, float>> pricesList, int journeyId)
        {
            var journeyPrice = pricesList.Where(pair => pair.Item1 == journeyId).Select(pair => pair.Item2).FirstOrDefault();
            return journeyPrice;
        }

        private float GetLowestPrice(List<JourneyModel> journeys)
        {
            if (journeys.Count == 0)
                return 0;
            float minPrice = journeys.Min(x => x.Price);
            return minPrice;
        }

    }
}
