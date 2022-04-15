using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Models
{
    public class JourneyModel
    {
        public List<FlightModel> Flights { get; set; }
        public float Price { get; set; }
        public float Taxes { get; set; }

        public JourneyModel(List<FlightModel> flights, float price, float taxes)
        {
            Flights = flights;
            Price = price;
            Taxes = taxes;
        }

        public static JourneyModel operator +(JourneyModel journeyFirst, JourneyModel journeySecond)
        {
            var newPrice = journeyFirst.Price > journeySecond.Price ? journeyFirst.Price : journeySecond.Price;
            var newTaxes = journeyFirst.Taxes + journeySecond.Taxes;
            
            List<FlightModel> newFlightsList = journeyFirst.Flights.Concat(journeySecond.Flights).ToList();

            return new JourneyModel(newFlightsList, newPrice, newTaxes);
        }

        public override string ToString()
        {
            string journeyString = Price + "," + Taxes.ToString("0.00"); ;
            foreach (FlightModel flight in Flights)
            {
                journeyString += "," + flight.ToString();
            }

            return journeyString;
        }
    }
}
