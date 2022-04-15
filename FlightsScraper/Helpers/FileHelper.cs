using CsvHelper;
using FlightsScraper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Helpers
{
    public class FileHelper
    {
        public static void ExportToCsvFile(List<JourneyModel> roundtripFlights, string fromDest, string toDest, string departDate, string returnDate, int connections = 1)
        {
            var filename = $"{fromDest}-{toDest}_({departDate})-({returnDate}).csv";
            var path = GetFullPath(filename);

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var header = GetHeader(connections);
                csv.WriteField(header);
                csv.NextRecord();

                foreach (JourneyModel roundtrip in roundtripFlights)
                {
                    List<string> roundtripProp = roundtrip.ToString().Split(',').ToList();
                    csv.WriteField(roundtripProp);
                    csv.NextRecord();
                }
            }
        }

        private static string GetDirectory()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            var dir = new DirectoryInfo(Environment.CurrentDirectory);
            while (dir.Name != appName)
            {
                dir = Directory.GetParent(dir.FullName);
            }
            return dir.FullName;
        }

        private static string GetFullPath(string filename)
        {
            string solutionDir = GetDirectory();
            string folderPath = Path.Combine(solutionDir, "Files");

            string fullPathString = Path.Combine(folderPath, filename);
            return fullPathString;
        }

        private static List<string> GetHeader(int connections)
        {
            var header = new List<string>() { "Price", "Taxes" };
            AddFields(ref header, "outbound", connections);
            AddFields(ref header, "inbound", connections);

            return header;
        }
        
        private static void AddFields(ref List<string> header, string direction, int connections)
        {
            var names = GetDisplayNames();

            for (int i = 1; i <= connections + 1; ++i)
            {
                foreach (var name in names)
                {
                    header.Add(direction + $" {i} {name}");
                }
            }
        }

        private static List<string> GetDisplayNames()
        {
            List<string> displayNames = new List<string>();

            PropertyInfo[] propertyInfos = typeof(FlightModel).GetProperties(BindingFlags.Instance |
                       BindingFlags.Static |
                       BindingFlags.NonPublic |
                       BindingFlags.Public);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var name = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                    .Cast<DisplayNameAttribute>().Single().DisplayName;
                displayNames.Add(name);
            }

            return displayNames;
        }
    }
}
