define([
    "epi/shell/widget/_ValueRequiredMixin", //Required for Optimizely to update the editor
    "dojo/on", //Event handler for DOM nodes
    "dojo/_base/declare", //Adds support to define Dojo classes for OOP
    "dojo/request",
    "dijit/_WidgetBase", //Required to instantiate a Dojo Widget
    "dijit/_TemplatedMixin", //Adds support for creating the DOM from a HTML Template
    "dojo/text!./WidgetTemplate.html", //HTML Template
    "https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.15.0/proj4.min.js", //PROJ4 coordinate-conversion.
    "xstyle/css!./WidgetTemplate.css", //Widget CSS
    "xstyle/css!./leaflet/leaflet.css", //Leaflet CSS
    "openmapseditor./leaflet/leaflet.js"
], function (_ValueRequiredMixin, on, declare, request, _Widget, _TemplatedMixin, _Template, proj4) {

    return declare("openmapseditor.LeafletWidget", [_ValueRequiredMixin, _Widget, _TemplatedMixin], {

        templateString: _Template, //HtmlTemplate in the Leaflet folder.

        postCreate: function () {
            this.inherited(arguments);
            this._setupSearch(); //Creates search-functionality
            this._initMap(); // Initialize the map once Leaflet is loaded
        },

        _setValueAttr: function (value) { //Gets called on pageLoad and when this.set is called (On pageLoad the value is received from the CMS).
            if (value.latitude && value.latitude) {
                //Update the map & marker with the parsed coordinates 
                this._updateMap(value);
            }

            //This is insanely important
            this.onFocus();

            //Set the coordinates in the CMS
            this._set("value", value);

            //Notifies the CMS of the changed values, must be done for the Editor to work correctly.
            this.onChange(value);
        },

        _setupSearch: function () {
            on(this.searchbox, "change", (e) => {
                const address = e.target.value.trim();
                if (address) {
                    this._searchAddress(address);
                }
            });
        },

        _searchAddress: function (address) {
            // Calls the SearchAddress API.
            //The encode URI - component makes sure the url isn't broken with special characters or blank spaces, etc.
            request.get(`${this.apiSearchUrl}?address=${encodeURIComponent(address)}`, {
                handleAs: "json"
            }).then((response) => {
                if (response.coordinatesData) {

                    const lngX = response.coordinatesData.longitude;
                    const latY = response.coordinatesData.latitude;

                    //Converting the coordinates
                    proj4.defs("EPSG:3006", "+proj=utm +zone=33 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs +type=crs");
                    let convertedCoordinates = proj4('EPSG:3006', 'WGS84', [lngX, latY])

                    const lng = convertedCoordinates[0];
                    const lat = convertedCoordinates[1];

                    this.set("value", { latitude: lat, longitude: lng });
                } else {
                    alert("No address or coordinates found");
                }
            }, (error) => {
                console.error("Error with api-call: ", error);
                alert("could not get address (and coordinates)");
            });
        },

        _onMapClick: function (event) {
            //Calls _setValueAttr & updates values in the CMS editor
            this.set("value", { latitude: event.latlng.lat, longitude: event.latlng.lng });
        },

        _updateMap: function (value) {
            this.latTextbox.value = value.latitude; //Set latitude in latTextbox
            this.lngTextbox.value = value.longitude; //Set longitude in lngTextbox

            this.marker.setLatLng([value.latitude, value.longitude]).addTo(this.map); //Set marker on click-coordinates & add it to presentation layer
            this.map.setView([value.latitude, value.longitude], this.map.getZoom(), { animate: true }); //Pan center of view to click-coordinates
        },

        _mapClear: function () {
            this.latTextbox.value = ""; //Clear latitude from latTextbox
            this.lngTextbox.value = ""; //Clear longitude from lngTextbox
            this.marker.remove(); //Remove the marker

            //Update value in the CMS editor
            this.set("value", { latitude: null, longitude: null }); //Set values to null (still an object for local block properties)
        },

        _initMap: function () {
            let mapContainer = this.domNode.querySelector(".leaflet-map");

            if (mapContainer) {
                this.map = L.map(mapContainer, {
                    zoomControl: true,   //Ensure zoom buttons are visible
                    attributionControl: true, //Ensure attribution is visible
                    doubleClickZoom: false //Disable zoom when double-clicking

                }).setView([this.defaultLatitude, this.defaultLongitude], this.defaultZoom);

                L.tileLayer(`${this.apiTileUrl}?z={z}&y={y}&x={x}`, {
                    attribution: '&copy; Lantmäteriet',
                    maxZoom: this.maxZoom,
                    minZoom: this.minZoom
                }).addTo(this.map);

                //Subscribe to (LMB)onClick event on leaflet map & trigger function
                //Pass map object so that we can manipulate it based on the click event
                this.marker = new L.marker([0, 0]);
                this.map.on('click', (event) => this._onMapClick(event));

                //Clear coordinates & remove marker when clearIcon is clicked
                on(this.clearIcon, 'click', () => this._mapClear());

                // Force the map to recalculate size after the container is ready
                setTimeout(() => {
                    this.map.invalidateSize(); // Arrow function retains "this" context
                }, 500);
            }
        },
    });
});
