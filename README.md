# Open Maps Editor for Optimizely
Editor for specifying coordinates in Optimizely CMS through map of choice with Leaflet.js.

## Summary
Solution consists of one project for the add-on (`OpenMapsEditor` folder).

The project has been tested with both **Optimizely Alloy** & **Optimizely Decoupled MusicFestival**.
MusicFestival is a sample project that uses the Content Delivery API in a headless CMS structure. https://github.com/episerver/musicfestival-vue-template

# Get started
1. Install the `OpenMapsEditor` add-on through Nuget Package Manager to an existing Optimizely CMS project
1. Add Maps API Credentials & desired default values to `appsettings.json`
1. Add the custom `MapsCoordinates` property to a page- or blocktype
1. Start the project and browse to the Optimizely UI, default: http://localhost:8081/Util/Login
1. Create a new page or block of the type that has the `MapsCoordinates` property
1. Test the OpenMapsEditor

> Note: The add-on does not include any template rendering, for example to show a map to site visitors. It only focuses on the CMS editing experience.