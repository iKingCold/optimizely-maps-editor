using System.Collections.Generic;
using System.Threading.Tasks;
using MapCore.Models;

namespace MapCore.Services
{
    public interface IMapProvider
    {
        string GetTileUrl(int z, int y, int x);
        string GetSearchParamName();
        string GetLimitParamName();
        bool RequiresAuthentication { get; }
        string GetAuthenticationHeader();
        Task<IEnumerable<AutoCompleteResult>> ParseAutoCompleteResults(string jsonResponse);
        Task<SearchResult> ParseSearchResult(string jsonResponse);
    }
} 