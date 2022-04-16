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
                        var contentType = response.Content.Headers.ContentType.MediaType;
                        var content = await response.Content.ReadAsStringAsync();

                        if (contentType.Contains("application/json"))
                        {
                            var jsonHeaderBody = JObject.Parse(content);
                            jsonBody = (JObject)jsonHeaderBody.SelectToken("body.data");
                        }
                    }
                    else
                        throw new HttpRequestException($"Http request failed with status: {httpStatus}");
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
