using Microsoft.Extensions.Options;
using MapCore.Models;
using MapCore.Services;
using System;
using System.Text;

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
    }
} 