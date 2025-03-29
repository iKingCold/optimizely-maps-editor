using Microsoft.Extensions.DependencyInjection;
using MapCore.Services;
using MapProvider.OpenStreetMap.Services;

namespace MapProvider.OpenStreetMap
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenStreetMapProvider(this IServiceCollection services)
        {
            services.AddScoped<IMapProvider, OsmProvider>();
            return services;
        }
    }
} 