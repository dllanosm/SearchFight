using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight.ConsoleClient
{
    class ResultSummaryItem
    {
        public ResultSummaryItem()
        {
            KeyWordResults = new Dictionary<string, long>();
        }

        public string SearchEngine { get; set; }
        public Dictionary<String, long> KeyWordResults { get; set; }
    }

    class EngineSetup
    {
        public List<EngineItem> EngineList { get; set; }
    }

    class EngineItem
    {
        public string EngineName { get; set; }
        public string AssemblyName { get; set; }
    }
}
