using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using MapCore.Models;

namespace OpenMapsEditor
{
    [EditorDescriptorRegistration(TargetType = typeof(MapsCoordinates), EditorDescriptorBehavior = EditorDescriptorBehavior.Default)]
    public class LeafletWidgetEditorDescriptor : EditorDescriptor
    {
        public string? BaseUrl { get; set; } = ServiceCollectionExtensions.BaseUrl;
        public string[]? SearchPrefix { get; set; } = ServiceCollectionExtensions.SearchPrefix;
        public double DefaultLatitude { get; set; } = ServiceCollectionExtensions.DefaultLatitude;
        public double DefaultLongitude { get; set; } = ServiceCollectionExtensions.DefaultLongitude;
        public int DefaultZoom { get; set; } = ServiceCollectionExtensions.DefaultZoom;
        public int MaxZoom { get; set; } = ServiceCollectionExtensions.MaxZoom;
        public int MinZoom { get; set; } = ServiceCollectionExtensions.MinZoom;
        public string? MapProviderName { get; set; } = ServiceCollectionExtensions.MapProviderName;

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "openmapseditor/LeafletWidget";

            metadata.EditorConfiguration.Add("baseUrl", BaseUrl);
            metadata.EditorConfiguration.Add("searchPrefix", SearchPrefix);
            metadata.EditorConfiguration.Add("defaultLatitude", DefaultLatitude);
            metadata.EditorConfiguration.Add("defaultLongitude", DefaultLongitude);
            metadata.EditorConfiguration.Add("defaultZoom", DefaultZoom);
            metadata.EditorConfiguration.Add("maxZoom", MaxZoom);
            metadata.EditorConfiguration.Add("minZoom", MinZoom);
            metadata.EditorConfiguration.Add("mapProviderName", MapProviderName);


            base.ModifyMetadata(metadata, attributes);
        }
    }
}
