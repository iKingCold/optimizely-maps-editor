using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using EPiServer.Shell.Modules;
using MapCore.Models;
using Microsoft.IdentityModel.Tokens;

namespace OpenMapsEditor
{
    /// <summary>
    /// Provides extension method for adding a flexible maps editor to an Optimizely website.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Gets or sets the API Settings
        /// </summary>
        public static string? BaseUrl { get; set; }
        public static string? ApiTileUrl { get; set; }
        public static string? ApiAutoCompleteUrl { get; set; }
        public static string? ApiSearchUrl { get; set; }
        public static string[]? SearchPrefix { get; set; }
        public static string? Identifier { get; set; }
        public static string? Username { get; set; }
        public static string? Password { get; set; }
        public static string? AuthType { get; set; }
        public static double DefaultLatitude { get; set; }
        public static double DefaultLongitude { get; set; }
        public static int DefaultZoom { get; set; }
        public static int MaxZoom { get; set; }
        public static int MinZoom { get; set; }
        public static string? MapProviderName { get; set; }

        public const string Plugin = "MapCore";

        /// <summary>
        /// Enables the Open Maps Editor
        /// </summary>
        /// <param name="baseUrl">Base URL for localhost</param>
        /// <param name="apiTileUrl">API Url for fetching map tiles</param>
        /// <param name="apiAutoCompleteUrl">API Url for the autocomplete endpoint</param>
        /// <param name="apiSearchUrl">API Url for searching for coordinates on specified address</param>
        /// <param name="searchPrefix">Prefix for specifying search address results to a defined county, can be left empty</param>
        /// <param name="identifier">Map type identifier</param>
        /// <param name="username">Maps Username (If required)</param>
        /// <param name="password">Maps Password (If required)</param>
        /// <param name="authType">Authentication type (Basic, bearer etc.)</param>
        /// <param name="defaultZoom">Default zoom level from 1 (least) to 20 (most).</param>
        /// <param name="defaultLongitude">Default longitude coordinate when no property value is set.</param>
        /// <param name="defaultLatitude">Default latitude coordinate when no property value is set.</param>
        /// <param name="maxZoom">Maximum zoom restriction for the map</param>
        /// <param name="minZoom">Minimum zoom restriction for the map</param>
        /// <param name="mapProviderName">Name of the map provider will be displayed in the bottom right of the map</param>
        /// <param name="services"></param>
        public static IServiceCollection AddOpenMapsEditor(this IServiceCollection services, MapSettings mapSettings)
        {
            BaseUrl = mapSettings.BaseUrl;
            ApiTileUrl = mapSettings.ApiTileUrl;
            ApiAutoCompleteUrl = mapSettings.ApiAutoCompleteUrl;
            ApiSearchUrl = mapSettings.ApiSearchUrl;
            SearchPrefix = mapSettings.SearchPrefix;
            Identifier = mapSettings.Identifier;
            Username = mapSettings.Username;
            Password = mapSettings.Password;
            AuthType = mapSettings.AuthType;
            DefaultLatitude = mapSettings.DefaultLatitude;
            DefaultLongitude = mapSettings.DefaultLongitude;
            DefaultZoom = mapSettings.DefaultZoom;
            MaxZoom = mapSettings.MaxZoom;
            MinZoom = mapSettings.MinZoom;
            MapProviderName = mapSettings.MapProviderName;

            services.Configure<ProtectedModuleOptions>(
                    pm =>
                    {
                        if (!pm.Items.Any(i => i.Name.Equals(Plugin, StringComparison.OrdinalIgnoreCase)))
                        {
                            pm.Items.Add(new ModuleDetails
                            {
                                Name = Plugin
                            });
                        }
                    });

            return services;
        }
    }
}
