using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MapCore.Models;
using MapCore.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace MapDemo.Controllers
{
    public class MapController : Controller
    {
        private readonly IMapProvider _mapProvider;
        private readonly MapSettings _settings;

        public MapController(IMapProvider mapProvider, IOptions<MapSettings> settings)
        {
            _mapProvider = mapProvider;
            _settings = settings.Value;
        }

        [Authorize] //Only signed-in editors can access the endpoint
        [HttpGet("GetTileImage")]
        public async Task<IActionResult> GetTileImage(int z, int y, int x)
        {
            var apiUrl = _mapProvider.GetTileUrl(z, y, x);

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("OpenMapsEditor/1.0"); //OSM requires a user-agent

                    if (_mapProvider.RequiresAuthentication)
                    {
                        var authHeader = _mapProvider.GetAuthenticationHeader();
                        var parts = authHeader.Split(' ');
                        if (parts.Length == 2)
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
                        }
                    }

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
        [HttpGet("SearchAutoComplete")]
        public async Task<IActionResult> SearchAutoComplete(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return BadRequest("Enter something to search...");
            }

            var uriAddress = HttpUtility.UrlEncode(address);
            var searchUrl = $"{_settings.ApiAutoCompleteUrl}?{_mapProvider.GetSearchParamName()}={uriAddress}&{_mapProvider.GetLimitParamName()}=5";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("OpenMapsEditor/1.0"); //OSM requires a user-agent

                    if (_mapProvider.RequiresAuthentication)
                    {
                        var authHeader = _mapProvider.GetAuthenticationHeader();
                        var parts = authHeader.Split(' ');
                        if (parts.Length == 2)
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
                        }
                    }

                    var searchResponse = await httpClient.GetAsync(searchUrl);

                    if (!searchResponse.IsSuccessStatusCode)
                    {
                        return StatusCode((int)searchResponse.StatusCode, $"API-call failed: {searchResponse.ReasonPhrase}");
                    }

                    var searchJson = await searchResponse.Content.ReadAsStringAsync();
                    var results = await _mapProvider.ParseAutoCompleteResults(searchJson);

                    if (results == null)
                    {
                        return NoContent();
                    }

                    return Ok(results);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
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

            var searchUrl = $"{_settings.ApiSearchUrl}?{_mapProvider.GetSearchParamName()}={address}&{_mapProvider.GetLimitParamName()}=1&{_mapProvider.GetAdditionalParams()}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("OpenMapsEditor/1.0"); //OSM requires a user-agent

                    if (_mapProvider.RequiresAuthentication)
                    {
                        var authHeader = _mapProvider.GetAuthenticationHeader();
                        var parts = authHeader.Split(' ');
                        if (parts.Length == 2) //First part is authType, second is authToken
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]); 
                        }
                    }

                    var searchResponse = await httpClient.GetAsync(searchUrl);

                    if (!searchResponse.IsSuccessStatusCode)
                    {
                        return StatusCode((int)searchResponse.StatusCode, $"API-call failed: {searchResponse.ReasonPhrase}");
                    }

                    var searchJson = await searchResponse.Content.ReadAsStringAsync();
                    var result = await _mapProvider.ParseSearchResult(searchJson);

                    if(result == null)
                    {
                        return NoContent();
                    }

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
