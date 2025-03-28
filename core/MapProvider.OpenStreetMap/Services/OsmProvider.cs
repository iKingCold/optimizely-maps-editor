using Microsoft.Extensions.Options;
using MapCore.Models;
using MapCore.Services;

namespace MapProvider.OpenStreetMap.Services
{
    public class OsmSettings : MapSettings { }

    public class OsmProvider : IMapProvider
    {
        private readonly OsmSettings _settings;

        public OsmProvider(IOptions<OsmSettings> options)
        {
            _settings = options.Value;
        }

        public bool RequiresAuthentication => false;

        public string GetAuthenticationHeader()
        {
            return string.Empty;
        }

        public string GetTileUrl(int z, int x, int y)
        {
            return $"{_settings.ApiTileUrl}/{z}/{y}/{x}.png";
        }
    }
} 