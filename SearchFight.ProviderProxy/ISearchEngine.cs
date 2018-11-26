using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFight.EngineProxy
{
    public interface ISearchEngine
    {
        string EngineName { get; }
        Task<long> GetSearchingResults(string keyword);
    }
}
