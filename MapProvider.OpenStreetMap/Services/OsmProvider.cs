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

        public async Task<IEnumerable<SearchResult>> ParseAutoCompleteResults(string jsonResponse)
        {
            var osmResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
            var features = ((JArray)osmResponse.features).ToObject<List<dynamic>>();
            return features.Select(feature => new SearchResult
            {
                Address = feature.properties.street ?? feature.properties.name,
                City = feature.properties.city,
                PropertyNumber = feature.properties.housenumber ?? null,
                Longitude = feature.geometry.coordinates[0],
                Latitude = feature.geometry.coordinates[1]
            });
        }
    }
}