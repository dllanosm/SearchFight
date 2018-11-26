using SearchFight.EngineProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SearchFight.ConsoleClient
{
    class Program
    {
        static List<ISearchEngine> engineItems = new List<ISearchEngine>();

        static void Main(string[] args)
        {
            try
            {
                LoadSearchEngines();

                Console.WriteLine("Welcome to SearchFight !!!");
                Console.WriteLine("*****************************");
                Console.WriteLine("Searchfight will determine the popularity of programming language on internet, based on Google / Bing results.");
                Console.WriteLine();
                Console.Write("Please enter your programming languages ");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("(Separated by Comma):");
                Console.ResetColor();

                String strSearchWords = Console.ReadLine();

                Console.WriteLine();
                RunAsync(strSearchWords).GetAwaiter().GetResult();

                Console.WriteLine("Please press any key to finish, Thank you for using SearchFight!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error ocurred during the procesing, please check the details:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static async Task RunAsync(string strSearchWords)
        {
            char[] charSeparators = new char[] { ',' };
            List<String> lstWords = strSearchWords.Split(charSeparators).Select(i => i.Trim()).ToList();
            List<ResultSummaryItem> lstResults = new List<ResultSummaryItem>();

            // Get Search results
            foreach (ISearchEngine searchEngine in engineItems)
            {
                ResultSummaryItem summaryItem = new ResultSummaryItem();
                summaryItem.SearchEngine = searchEngine.EngineName;

                foreach (string keyword in lstWords)
                {
                    long results = await searchEngine.GetSearchingResults(keyword);
                    summaryItem.KeyWordResults.Add(keyword, results);
                }

                lstResults.Add(summaryItem);
            }

            // Show results by Keywords
            Dictionary<string, long> totalsByKeyword = new Dictionary<string, long>();
            foreach (string keyword in lstWords)
            {
                long totalByKeyword = 0;
                Console.WriteLine(keyword);

                foreach (ResultSummaryItem item in lstResults)
                {
                    long nroResults = item.KeyWordResults[keyword];
                    Console.Write("Results in " + item.SearchEngine + ": ");
                    Console.WriteLine(nroResults.ToString());
                    totalByKeyword += nroResults;
                }

                totalsByKeyword.Add(keyword, totalByKeyword);
                Console.WriteLine();
            }

            // Show results by Engine
            foreach (ResultSummaryItem item in lstResults)
            {
                Console.Write(item.SearchEngine + " winner: ");

                var myList = item.KeyWordResults.ToList();
                myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                Console.WriteLine(myList[0].Key);
            }

            var myListTotal = totalsByKeyword.ToList();
            myListTotal.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            Console.WriteLine("Total Winner: " + myListTotal[0].Key);
            Console.WriteLine();
        }

        private static void LoadSearchEngines()
        {
            // Read Engines from JSon file
            string contentFile = System.IO.File.ReadAllText(@"SearchEngines.json");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var obj = serializer.Deserialize<EngineSetup>(contentFile);

            foreach(EngineItem item in obj.EngineList)
            {
                ISearchEngine engine = CreateInstance<ISearchEngine>(item.AssemblyName);
                engineItems.Add(engine);
            }
        }

        private static I CreateInstance<I>(string assemblyName) where I : class
        {
            Assembly assembly = Assembly.LoadFrom("SearchFight.EngineProxy.dll");
            Type type = assembly.GetType(assemblyName);
            return Activator.CreateInstance(type) as I;
        }
    }
}
