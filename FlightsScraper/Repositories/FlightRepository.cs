using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Repositories
{
    public class FlightRepository
    {
        private string baseUrl = "http://homeworktask.infare.lt/";
        public async Task<JObject> GetAsync(string url)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
             
                var jsonBody = new JObject();
                try
                {
                    var response = await client.GetAsync("search.php?" + url);
                    var httpStatus = response.StatusCode;

                    if (httpStatus == System.Net.HttpStatusCode.OK)
                    {
                        var content = await response.Content.ReadAsStringAsync(); 
                        var jsonHeaderBody = JObject.Parse(content);

                        jsonBody = (JObject)jsonHeaderBody.SelectToken("body.data");
                    }
                    return jsonBody;
                }
                catch(InvalidOperationException) { throw; }
                catch(HttpRequestException) { throw; }
                catch(TaskCanceledException) { throw; }
                catch(Exception){ throw; }
            }
        }
    }
}
