using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Models
{
    public class JourneyModel
    {
        public List<FlightModel> flights { get; set; }
        public float price { get; set; }
        public float taxes { get; set; }

        public JourneyModel(List<FlightModel> flights, float price, float taxes)
        {
            this.flights = flights;
            this.price = price;
            this.taxes = taxes;
        }

        public static JourneyModel operator +(JourneyModel journeyFirst, JourneyModel journeySecond)
        {
            var newPrice = journeyFirst.price > journeySecond.price? journeyFirst.price : journeySecond.price;
            var newTaxes = journeyFirst.taxes + journeySecond.taxes;
            
            List<FlightModel> newFlightsList = journeyFirst.flights.Concat(journeySecond.flights).ToList();

            return new JourneyModel(newFlightsList, newPrice, newTaxes);
        }

        public override string ToString()
        {
            string journeyString = price + "," + taxes.ToString("0.00"); ;
            foreach (FlightModel flight in flights)
            {
                journeyString += "," + flight.ToString();
            }

            return journeyString;
        }
    }
}
