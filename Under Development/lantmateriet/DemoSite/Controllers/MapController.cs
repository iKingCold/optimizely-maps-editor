using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusicFestival.Backend.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DemoSite.Controllers
{
    public class MapController : Controller
    {
        private readonly ApiSettings _settings; //Used to read APISettings from appsettings.json
        public MapController(IOptions<ApiSettings> settings)
        {
            _settings = settings.Value;
        }

        [Authorize] //Only signed-in editors can access the endpoint
        [HttpGet("GetTileImage")]
        public async Task<IActionResult> GetTileImage(int z, int y, int x)
        {
            var authToken = Base64Encode($"{_settings.Username}:{_settings.Password}");
            var apiUrl = $"https://maps.lantmateriet.se/topowebb/v1.1/wmts/1.0.0/{_settings.Identifier}/default/3857/{z}/{y}/{x}.png";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    //Lägg på authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settings.AuthType, authToken);

                    var response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var imageBytes = await response.Content.ReadAsByteArrayAsync();
                        return File(imageBytes, "image/png");
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Error fetching tile from API.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }

        [Authorize] //Only signed-in editors can access the endpoint
        [HttpGet("SearchAddress")]
        public async Task<IActionResult> SearchAddress(string address, string includeData = "basinformation")
        {

            if (string.IsNullOrWhiteSpace(address))
            {
                return BadRequest("Enter something to search...");
            }

            var authToken = Base64Encode($"{_settings.Username}:{_settings.Password}");
            var searchUrl = $"https://api.lantmateriet.se/distribution/produkter/belagenhetsadress/v4.2/referens/fritext?adress={address}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    //Lägg på authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settings.AuthType, authToken);

                    var searchResponse = await httpClient.GetAsync(searchUrl);

                    if (!searchResponse.IsSuccessStatusCode)
                    {
                        return StatusCode((int)searchResponse.StatusCode, $"API-call failed: {searchResponse.ReasonPhrase}");
                    }

                    //converts the response from json.
                    var searchJson = await searchResponse.Content.ReadAsStringAsync();
                    var searchResultArray = JsonConvert.DeserializeObject<List<dynamic>>(searchJson);

                    if (searchResultArray == null || searchResultArray.Count == 0)
                    {
                        return NotFound("No address found");
                    }

                    //Checks for an id
                    var searchResult = searchResultArray[0];

                    //objektidentitet är motsvarande ID i api:et
                    var id = (string)searchResult?.objektidentitet;
                    if (string.IsNullOrEmpty(id))
                    {
                        return NotFound("No id found");
                    }

                    var coordinatesUrl = $"https://api.lantmateriet.se/distribution/produkter/belagenhetsadress/v4.2/{id}?includeData={includeData}";

                    var coordinatesResponse = await httpClient.GetAsync(coordinatesUrl);
                    if (!coordinatesResponse.IsSuccessStatusCode)
                    {
                        return StatusCode((int)coordinatesResponse.StatusCode, $"API-call failed: {coordinatesResponse.ReasonPhrase}");
                    }

                    var coordinatesJson = await coordinatesResponse.Content.ReadAsStringAsync();
                    var coordinatesResult = JsonConvert.DeserializeObject<dynamic>(coordinatesJson);

                    var longitude = (double)coordinatesResult.features[0].geometry.coordinates[0];
                    var latitude = (double)coordinatesResult.features[0].geometry.coordinates[1];

                    return Ok(new
                    {
                        AddressData = searchResult,
                        CoordinatesData = new
                        {
                            longitude,
                            latitude
                        }
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        private static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}
