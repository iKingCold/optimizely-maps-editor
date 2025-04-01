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
using EPiServer.Async;

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
        public async Task<IEnumerable<AutoCompleteResult>> ParseAutoCompleteResults(string jsonResponse)
        {
            var addresses = JsonConvert.DeserializeObject<List<string>>(jsonResponse);

            return addresses.Select(address => new AutoCompleteResult
            {
                Address = address
            });
        }

        public async Task<SearchResult> ParseSearchResult(string searchJson)
        {
            //Implement SWEREF99 TO WGS84 conversion, then we may remove proj4 from leaflet-widget.
            var results = JsonConvert.DeserializeObject<List<dynamic>>(searchJson);
            Console.WriteLine(results);

            if (results?.Count <= 0)
            {
                //No results found, return empty.
            }

            var searchResult = results[0];
            var id = (string)searchResult?.objektidentitet;

            if (string.IsNullOrEmpty(id))
            {
                //No id found, return empty.
            }

            var coordinatesUrl = $"https://api.lantmateriet.se/distribution/produkter/belagenhetsadress/v4.2/{id}?includeData=basinformation";
            using (var httpClient = new HttpClient())
            { 
                var coordinatesResponse = await httpClient.GetAsync(coordinatesUrl);
                if (!coordinatesResponse.IsSuccessStatusCode)
                {

                    //Return error message
                    //return StatusCode((int)coordinatesResponse.StatusCode, $"API-call failed: {coordinatesResponse.ReasonPhrase}");
                }

                var coordinatesJson = await coordinatesResponse.Content.ReadAsStringAsync();
                var coordinatesResult = JsonConvert.DeserializeObject<dynamic>(coordinatesJson);

                var swePos = new SWEREF99Position(
                    (double)coordinatesResult.features[0].geometry.coordinates[0],
                    (double)coordinatesResult.features[0].geometry.coordinates[1]
                    );

                var wgsPos = swePos.ToWGS84();
            }

            return new SearchResult { Longitude = 0, Latitude = 0 };
        }
    }
}