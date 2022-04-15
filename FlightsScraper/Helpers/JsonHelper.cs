using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsScraper.Helpers
{
    public class JsonHelper
    {
        public List<JObject> GetListByToken(JObject obj, string filter)
        {
            List<JObject> list = new List<JObject>();
            var token = obj.SelectToken(filter);
            if (token is not null)
                list = ((JArray)token).Children().OfType<JObject>().ToList();

            return list;
        }

        public T GetValueByToken<T>(JObject obj, string filter)
        {
            var token = obj.SelectToken(filter);
            return (T)Convert.ChangeType(token, typeof(T));
        }
    }
}
