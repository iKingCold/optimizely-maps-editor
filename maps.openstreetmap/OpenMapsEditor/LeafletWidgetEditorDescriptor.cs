using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace OpenMapsEditor
{
    [EditorDescriptorRegistration(TargetType = typeof(MapsCoordinates), EditorDescriptorBehavior = EditorDescriptorBehavior.Default)]
    public class LeafletWidgetEditorDescriptor : EditorDescriptor
    {
        public string ApiUrl { get; set; } = ServiceCollectionExtensions.ApiUrl;
        public string[]? SearchPrefix { get; set; } = ServiceCollectionExtensions.SearchPrefix;
        public double DefaultLatitude { get; set; } = ServiceCollectionExtensions.DefaultLatitude;
        public double DefaultLongitude { get; set; } = ServiceCollectionExtensions.DefaultLongitude;
        public int DefaultZoom { get; set; } = ServiceCollectionExtensions.DefaultZoom;
        public int MaxZoom { get; set; } = ServiceCollectionExtensions.MaxZoom;
        public int MinZoom { get; set; } = ServiceCollectionExtensions.MinZoom;

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "openmapseditor/LeafletWidget";

            metadata.EditorConfiguration.Add("apiUrl", ApiUrl);
            metadata.EditorConfiguration.Add("searchPrefix", SearchPrefix);
            metadata.EditorConfiguration.Add("defaultLatitude", DefaultLatitude);
            metadata.EditorConfiguration.Add("defaultLongitude", DefaultLongitude);
            metadata.EditorConfiguration.Add("defaultZoom", DefaultZoom);
            metadata.EditorConfiguration.Add("maxZoom", MaxZoom);
            metadata.EditorConfiguration.Add("minZoom", MinZoom);

            base.ModifyMetadata(metadata, attributes);
        }
    }
}