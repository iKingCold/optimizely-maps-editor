using Microsoft.Extensions.Options;
using MapCore.Models;
using MapCore.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MapProvider.OpenStreetMap.Services
{
    public class OsmProvider : IMapProvider
    {
        private readonly MapSettings _settings;

        public OsmProvider(IOptions<MapSettings> options)
        {
            _settings = options.Value;
        }

        public bool RequiresAuthentication => false;

        public string GetAuthenticationHeader()
        {
            return string.Empty;
        }

        public string GetTileUrl(int z, int y, int x)
        {
            return $"{_settings.ApiTileUrl}/{z}/{x}/{y}.png";
        }

        public string GetSearchParamName()
        {
            return "q";
        }

        public string GetLimitParamName()
        {
            return "limit";
        }

        public string GetAdditionalParams()
        {
            return "format=jsonv2";
        }

        public async Task<IEnumerable<AutoCompleteResult>?> ParseAutoCompleteResults(string jsonResponse)
        {
            var addresses = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if(addresses == null)
            {
                return null;
            }

            var features = ((JArray)addresses.features).ToObject<List<dynamic>>();

            if(features == null || features.Count == 0)
            {
                return null;
            }

            return features.Select(feature => new AutoCompleteResult
            {
                Address = feature.properties.street ?? feature.properties.name,
                City = feature.properties.city,
                PropertyNumber = feature.properties.housenumber ?? null,
                Longitude = feature.geometry.coordinates[0],
                Latitude = feature.geometry.coordinates[1]
            });
        }

        public async Task<SearchResult?> ParseSearchResult(string searchJson)
        {
            var results = JsonConvert.DeserializeObject<List<dynamic>>(searchJson);

            if (results == null || results.Count == 0)
            {
                return null;
            }

            var searchResult = results[0];

            return new SearchResult
            {
                Longitude = searchResult.lon,
                Latitude = searchResult.lat
            };
        }
    }
}