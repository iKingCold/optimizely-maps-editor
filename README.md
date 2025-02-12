# Open Maps Editor for Optimizely
Editor for setting coordinates in Optimizely CMS through map of choice with Leaflet.js.<br/>
The editor uses OpenStreetMaps by default. 

## Summary
The solution consists of two projects.<br/>
`OpenMapsEditor` contains the Map Editor add-on / plugin.<br/>
`DemoSite` contains a demo website for testing the Map Editor.

The project has been tested with **Optimizely Alloy**, **Optimizely Decoupled MusicFestival** & **Optimizely Empty Project**.
MusicFestival is a sample project that uses the Content Delivery API in a headless CMS structure. https://github.com/episerver/musicfestival-vue-template

# Option 1. Get started through NuGet
1. Install the `OpenMapsEditor` add-on through Nuget Package Manager to an existing Optimizely CMS project
1. Add Maps API Credentials & desired default values to `appsettings.json`
1. Add the custom `MapsCoordinates` property to a page- or blocktype
1. Start the project and browse to the Optimizely UI, default: http://localhost:8081/Util/Login
1. Create a new page or block of the type that has the `MapsCoordinates` property
1. Test the OpenMapsEditor in the editor view

# Option 2. Get started through the repository
1. Clone the repository
1. Run the `create-database.bat` to create an empty database
1. Open the solution and start the `DemoSite` project 
1. Create an admin user in the Optimizely UI: http://localhost:8081/Util/Login
1. Create a new page in the Optimizely UI Edit-panel
1. Test the OpenMapsEditor in the editor view

> Note: The add-on does not include any template rendering, for example to show a map to site visitors. It only focuses on the CMS editing experience.
