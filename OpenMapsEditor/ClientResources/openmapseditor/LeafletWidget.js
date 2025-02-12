define([
    "epi/shell/widget/_ValueRequiredMixin", //Required for Optimizely to update the editor
    "dojo/on", //Event handler for DOM nodes
    "dojo/_base/declare", //Adds support to define Dojo classes for OOP
    "dojo/dom-construct", //Required to use DomNode for query-selection
    "dijit/_WidgetBase", //Required to instantiate a Dojo Widget
    "dijit/_TemplatedMixin", //Adds support for creating the DOM from a HTML Template
    "dojo/text!./WidgetTemplate.html", //HTML Template
    "https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.15.0/proj4.min.js", //PROJ4 coordinate-conversion.
    "xstyle/css!./WidgetTemplate.css", //CSS to load when widget is loaded
    "https://unpkg.com/leaflet@1.9.4/dist/leaflet.js" //Leaflet Map Engine
], function (_ValueRequiredMixin, on, declare, domConstruct, _Widget, _TemplatedMixin, _Template, proj4) {

    return declare("openmapseditor.LeafletWidget", [_ValueRequiredMixin, _Widget, _TemplatedMixin], {

        templateString: _Template, //HtmlTemplate in the Leaflet folder.

        postCreate: function () {
            this.inherited(arguments);
            this._loadLeafletCSS();
            this._initMap();
        },

        _loadLeafletCSS: function () { //Load Leaflet CSS when called
            //Leaflet CSS must be called before leaflet.js 
            domConstruct.create("link", {
                rel: "stylesheet",
                href: "https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"
            }, document.head);
        },

        _setValueAttr: function (value) { //Gets called on pageLoad and when this.set is called (On pageLoad the value is received from the CMS).
            if (value.latitude && value.latitude) {
                //Update the map & marker with the parsed coordinates 
                this._updateMap(value);
            }

            //Very important to ensure widget submits data to CMS.
            this.onFocus();

            //Set the coordinates in the CMS
            this._set("value", value);

            //Notifies the CMS of the changed values, must be done for the Editor to work correctly.
            this.onChange(value);
        },

        _onMapClick: function (event) {
            //EPSG:3006 = SWEREF99 TM, if needed for the future!
            //proj4.defs("EPSG:3006", "+proj=utm +zone=33 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs +type=crs");
            //let convertedCoordinates = proj4('WGS84', 'EPSG:3006', [event.latlng.lng, event.latlng.lat]).toString();
            //console.log(convertedCoordinates);

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

                L.tileLayer(this.apiUrl, {
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
