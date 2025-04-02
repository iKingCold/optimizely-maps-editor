using System.Linq;
using Microsoft.Extensions.Options;
using MapCore.Models;
using MapCore.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using MightyLittleGeodesy.Positions;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace MapProvider.Lantmateriet.Services
{
    public class LantmaterietProvider : IMapProvider
    {
        private readonly MapSettings _settings;

        public LantmaterietProvider(IOptions<MapSettings> options)
        {
            _settings = options.Value;
        }

        public bool RequiresAuthentication => true;

        public string GetAuthenticationHeader()
        {
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.Username}:{_settings.Password}"));
            return $"{_settings.AuthType} {authToken}";
        }

        public string GetTileUrl(int z, int y, int x)
        {
            return $"{_settings.ApiTileUrl}{_settings.Identifier}/default/3857/{z}/{y}/{x}.png";
        }

        public string GetSearchParamName()
        {
            return "adress";
        }

        public string GetLimitParamName()
        {
            return "maxHits";
        }

        public string GetAdditionalParams()
        {
            return string.Empty;
        }

        public async Task<IEnumerable<AutoCompleteResult>?> ParseAutoCompleteResults(string jsonResponse)
        {
            var addresses = JsonConvert.DeserializeObject<List<string>>(jsonResponse);

            if (addresses == null || addresses.Count == 0)
            {
                return null;
            }

            return addresses.Select(address => new AutoCompleteResult
            {
                Address = address
            });
        }

        public async Task<SearchResult?> ParseSearchResult(string searchJson)
        {
            //Implement SWEREF99 TO WGS84 conversion, then we may remove proj4 from leaflet-widget.
            var results = JsonConvert.DeserializeObject<List<dynamic>>(searchJson);

            if (results == null || results.Count == 0)
            {
                return null;
            }

            var searchResult = results[0];
            var id = (string)searchResult?.objektidentitet;

            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var coordinatesUrl = $"https://api.lantmateriet.se/distribution/produkter/belagenhetsadress/v4.2/{id}?includeData=basinformation";
            using (var httpClient = new HttpClient())
            {
                var authHeader = GetAuthenticationHeader();
                var parts = authHeader.Split(' ');
                if (parts.Length == 2) //First part is authType, second is authToken
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
                }

                var coordinatesResponse = await httpClient.GetAsync(coordinatesUrl);
                if (!coordinatesResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                var coordinatesJson = await coordinatesResponse.Content.ReadAsStringAsync();
                var coordinatesResult = JsonConvert.DeserializeObject<dynamic>(coordinatesJson);

                var swePos = new SWEREF99Position(
                    (double)coordinatesResult.features[0].geometry.coordinates[1],
                    (double)coordinatesResult.features[0].geometry.coordinates[0]
                    );

                var wgsPos = swePos.ToWGS84();
                return new SearchResult { Longitude = wgsPos.Longitude,  Latitude = wgsPos.Latitude };
            }
        }
    }
}