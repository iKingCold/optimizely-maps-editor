using Microsoft.Extensions.Options;
using MapCore.Models;
using MapCore.Services;

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
    }
} 