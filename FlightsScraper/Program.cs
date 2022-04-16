using FlightsScraper.Helpers;
using FlightsScraper.Services;

class Program
{
    static void Main(string[] args)
    {
        CreateCustomCulture();

        ExtractRoundtripFlights("MAD", "FUE", "2022-05-09", "2022-05-16");
        ExtractRoundtripFlights("MAD", "FUE", "2023-02-09", "2023-02-16");
        ExtractRoundtripFlights("MAD", "AUH", "2023-05-09", "2023-05-16", 0);
        ExtractRoundtripFlights("CPH", "MAD", "2022-12-30", "2023-03-11", 1, "AMS");
        ExtractRoundtripFlights("CPH", "FUE", "2022-12-30", "2023-03-11");
    }
    private static void CreateCustomCulture()
    {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        Thread.CurrentThread.CurrentCulture = customCulture;
    }

    private static void ExtractRoundtripFlights(string fromDest, string toDest, string departDate, string returnDate, int connections = 1, string connectionAirport = "")
    {
        JourneyService service = new JourneyService();

        try
        {
            var roundtripFlights = service.GetRoundtripFlights(fromDest, toDest, departDate, returnDate, connections, connectionAirport);
            var cheapestFlights = service.GetCheapestFlights(roundtripFlights);
            FileHelper.ExportToCsvFile(cheapestFlights, fromDest, toDest, departDate, returnDate, connections);
        }
        catch (Exception ex) {
            Console.WriteLine($"With input: {fromDest} {toDest} {departDate} {returnDate}, {ex.Message}"); 
        }
        
    }
}




