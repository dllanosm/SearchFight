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
    public class BingEngine : ISearchEngine
    {
        public string EngineName
        {
            get
            {
                return "Bing";
            }
        }

        public async Task<long> GetSearchingResults(string keyword)
        {
            try
            {
                var builder = new UriBuilder(ConfigurationManager.AppSettings["BingEngine.URI"]);
                builder.Port = -1;

                var query = HttpUtility.ParseQueryString(builder.Query);
                query["q"] = keyword;

                builder.Query = query.ToString();
                string url = builder.ToString();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["BingEngine.SubscriptionKey"]);

                    HttpResponseMessage response = await client.GetAsync(url);
                    string content = await response.Content.ReadAsStringAsync();

                    JavaScriptSerializer jsonSerial = new JavaScriptSerializer();
                    var obj = jsonSerial.Deserialize<BingResult>(content);
                    long resultsCount = obj.WebPages.TotalEstimatedMatches;

                    return resultsCount;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    class BingResult
    {
        public object QueryContext { get; set; }
        public BingSearchContent WebPages { get; set; }
        public object RelatedSearches { get; set; }
        public object Videos { get; set; }
        public object RankingResponse { get; set; }
    }

    class BingSearchContent
    {
        public string WebSearchUrl { get; set; }
        public long TotalEstimatedMatches { get; set; }
        public object Value { get; set; }
    }
}
