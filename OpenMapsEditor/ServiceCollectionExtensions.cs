using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using EPiServer.Shell.Modules;

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
        public static string? ApiUrl { get; set; }
        public static double DefaultLatitude { get; set; }
        public static double DefaultLongitude { get; set; }
        public static int DefaultZoom { get; set; }
        public static int MaxZoom { get; set; }
        public static int MinZoom { get; set; }

        private const string ADDON_NAME = "OpenMapsEditor";

        /// <summary>
        /// Enables the Open Maps Editor
        /// </summary>
        /// <param name="apiUrl">API Url for fetching map tiles</param>
        /// <param name="defaultZoom">Default zoom level from 1 (least) to 20 (most)</param>
        /// <param name="defaultLongitude">Default longitude coordinate when no property value is se.</param>
        /// <param name="defaultLatitude">Default latitude coordinate when no property value is set</param>
        /// <param name="maxZoom">Maximum zoom restriction for the map</param>
        /// <param name="minZoom">Minimum zoom restriction for the map</param>
        /// <param name="services"></param>
        public static IServiceCollection AddOpenMapsEditor(this IServiceCollection services, string apiUrl, double defaultLatitude, 
            double defaultLongitude, int defaultZoom, int maxZoom, int minZoom)
        {
            ApiUrl = apiUrl;
            DefaultLatitude = defaultLatitude;
            DefaultLongitude = defaultLongitude;
            DefaultZoom = defaultZoom;
            MaxZoom = maxZoom;
            MinZoom = minZoom;

            services.Configure<ProtectedModuleOptions>(
                    pm =>
                    {
                        if (!pm.Items.Any(i => i.Name.Equals(ADDON_NAME, StringComparison.OrdinalIgnoreCase)))
                        {
                            pm.Items.Add(new ModuleDetails
                            {
                                Name = ADDON_NAME
                            });
                        }
                    });

            return services;
        }
    }
}
