using Microsoft.Extensions.Options;
using MapCore.Models;
using MapCore.Services;
using System;
using System.Text;

namespace MapProvider.Lantmateriet.Services
{
    public class LantmaterietSettings : MapSettings
    {
        public string Identifier { get; set; }
    }

    public class LantmaterietProvider : IMapProvider
    {
        private readonly LantmaterietSettings _settings;

        public LantmaterietProvider(IOptions<LantmaterietSettings> options)
        {
            _settings = options.Value;
        }

        public bool RequiresAuthentication => true;

        public string GetAuthenticationHeader()
        {
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.Username}:{_settings.Password}"));
            return $"{_settings.AuthType} {authToken}";
        }

        public string GetTileUrl(int z, int x, int y)
        {
            return $"{_settings.ApiTileUrl}{_settings.Identifier}/default/3857/{z}/{y}/{x}.png";
        }
    }
} 