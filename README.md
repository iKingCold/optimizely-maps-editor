# Open Maps Editor for Optimizely
Editor for setting coordinates in Optimizely CMS through map of choice with Leaflet.js.<br/>
The editor uses OpenStreetMap by default. 

## Pictures
<div>
  <img src="https://github.com/iKingCold/optimizely-maps-editor/blob/main/Pictures/Lantmateriet-AutoComplete-1.png" width="400">
  <img src="https://github.com/iKingCold/optimizely-maps-editor/blob/main/Pictures/OSM-AutoComplete-1.png" width="400">
</div>

## Summary
The solution consists of 3 different project types.<br/>
`MapCore` contains the Core Map files required to render the Leaflet map.<br/>
`MapProvider.*provider*` contains a ProviderService that implements provider-specific settings.<br/>
`MapDemo` contains a demo website for testing the Map Editor.<br/>

The project has been tested with **Optimizely Alloy**, **Optimizely Decoupled MusicFestival** & **Optimizely Empty Project**.
MusicFestival is a sample project that uses the Content Delivery API in a headless CMS structure. https://github.com/episerver/musicfestival-vue-template

# Option 1. Get started through NuGet
1. Install the desired Map Provider `OpenMapsEditor.*provider*` through Nuget Package Manager to an existing Optimizely CMS project
1. Add Maps API Credentials & desired default values to `appsettings.json`
1. Add the custom `MapsCoordinates` property to a page- or blocktype
1. Start the project and browse to the Optimizely UI, default: http://localhost:8081/Util/Login
1. Create a new page or block of the type that has the `MapsCoordinates` property
1. Test the OpenMapsEditor in the editor view

# Option 2. Get started through the repository
1. Clone the repository
1. Run the `create-database.bat` to create an empty database
1. Open the solution and start the `MapDemo` project 
1. Create an admin user in the Optimizely UI: http://localhost:8081/Util/Login
1. Create a new page in the Optimizely UI Edit-panel
1. Test the OpenMapsEditor in the editor view

# Switch MapProvider: 
1. Scope the desired MapProvider in MapDemo/Startup.cs <br/>
example: .AddScoped<IMapProvider, LantmaterietProvider>();
2. Configure the MapSettings with desired ApiUrls in MapDemo/appsettings.json.

> Note: The add-on does not include any template rendering, for example to show a map to site visitors. It only focuses on the CMS editing experience.
