using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace SearchFight.EngineProxy
{
    public class GoogleEngine : ISearchEngine
    {
        public string EngineName
        {
            get
            {
                return "Google";
            }
        }

        public async Task<long> GetSearchingResults(string keyword)
        {
            try
            {
                var builder = new UriBuilder(ConfigurationManager.AppSettings["GoogleEngine.URI"]);
                builder.Port = -1;

                var query = HttpUtility.ParseQueryString(builder.Query);
                query["key"] = ConfigurationManager.AppSettings["GoogleEngine.Key"];
                query["cx"] = ConfigurationManager.AppSettings["GoogleEngine.Cx"];
                query["q"] = keyword;

                builder.Query = query.ToString();
                string url = builder.ToString();

                using (HttpClient client = new HttpClient())
                {
                    
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(url);
                    string content = await response.Content.ReadAsStringAsync();

                    JavaScriptSerializer jsonSerial = new JavaScriptSerializer();
                    var objResponse = jsonSerial.Deserialize<GoogleResult>(content);
                    string resultsCount = objResponse.SearchInformation.TotalResults;

                    return long.Parse(resultsCount);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    class GoogleResult
    {
        public string Kind { get; set; }
        public dynamic Url { get; set; }
        public dynamic Queries { get; set; }
        public dynamic Context { get; set; }
        public GoogleSearchInfo SearchInformation { get; set; }
        public dynamic Items { get; set; }
    }

    class GoogleSearchInfo
    {
        public decimal SearchTime { get; set; }
        public string FormattedSearchTime { get; set; }
        public string TotalResults { get; set; }
        public string FormattedTotalResults { get; set; }
    }
}
