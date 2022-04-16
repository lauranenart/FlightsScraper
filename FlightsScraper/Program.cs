using FlightsScraper.Helpers;
using FlightsScraper.Services;

class Program
{
    static void Main(string[] args)
    {
        CreateCustomCulture();

        ExtractRoundtripFlights("MAD", "FUE", "2022-05-09", "2022-05-16");
        ExtractRoundtripFlights("MAD", "FUE", "2023-02-09", "2023-02-16");
        ExtractRoundtripFlights("MAD", "AUH", "2023-05-09", "2023-05-16", Connection.Direct);
        ExtractRoundtripFlights("CPH", "MAD", "2022-12-30", "2023-03-11", Connection.NonDirect, "AMS");
        ExtractRoundtripFlights("JFK", "FUE", "2022-07-19", "2022-08-16");
        ExtractRoundtripFlights("MAD", "AUH", "2022-04-22", "2023-04-30", Connection.Direct);
        ExtractRoundtripFlights("JFK", "FUE", "2022-07-21", "2023-08-10");
        ExtractRoundtripFlights("CPH", "MAD", "2022-07-21", "2022-08-15", Connection.NonDirect);
        ExtractRoundtripFlights("MAD", "AUH", "2022-06-15", "2022-07-07", Connection.Direct);
        ExtractRoundtripFlights("MAD", "AUH", "2022-11-04", "2022-11-16", Connection.Direct);
    }
    private static void CreateCustomCulture()
    {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        Thread.CurrentThread.CurrentCulture = customCulture;
    }

    private static void ExtractRoundtripFlights(string fromDest, string toDest, string departDate, string returnDate, Connection connections = Connection.All, string connectionAirport = "")
    {
        JourneyService service = new JourneyService();

        try
        {
            var roundtripFlights = service.GetRoundtripFlights(fromDest, toDest, departDate, returnDate, connections, connectionAirport);
            var cheapestFlights = service.GetCheapestFlights(roundtripFlights);

            int count = connections is Connection.Direct ? 0 : 1;
            FileHelper.ExportToCsvFile(cheapestFlights, fromDest, toDest, departDate, returnDate, count);
        }
        catch (Exception ex) {
            Console.WriteLine($"With input: {fromDest} {toDest} {departDate} {returnDate}, {ex.Message}"); 
        }
        
    }
}




