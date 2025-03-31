define([
    "epi/shell/widget/_ValueRequiredMixin", //Required for Optimizely to update the editor
    "dojo/on", //Event handler for DOM nodes
    "dojo/_base/declare", //Adds support to define Dojo classes for OOP
    "dijit/_WidgetBase", //Required to instantiate a Dojo Widget
    "dijit/_TemplatedMixin", //Adds support for creating the DOM from a HTML Template
    "dojo/text!./WidgetTemplate.html", //HTML Template
    "openmapseditor/proj/proj4", //Load the local PROJ4 coordinate-conversion script
    "dojo/query", //Used to query for DOM-nodes, get all nodes of specified name or tag. 
    "dojo/_base/event", //Handle DOM Events
    "xstyle/css!./WidgetTemplate.css", //Widget CSS
    "xstyle/css!./leaflet/leaflet.css", //Leaflet CSS
    "openmapseditor/leaflet/leaflet" //Load the local Leaflet script.   
], function (_ValueRequiredMixin, on, declare, _Widget, _TemplatedMixin, _Template, proj4, query) {

    return declare("openmapseditor.LeafletWidget", [_ValueRequiredMixin, _Widget, _TemplatedMixin], {

        templateString: _Template, //HtmlTemplate in the Leaflet folder.
        intermediateChanges: true,

        postCreate: function () {
            this.inherited(arguments);
            this._setupPrefix(); //Initialize prefix functionality
            this._setupSearch(); //Initialize search functionality
            this._initMap(); // Initialize the map once Leaflet is loaded
        },

        _debounce: function (func, delay) {
            let timeout; //Initialize a time

            return function () { //Return a new function that will execute the passed param function
                clearTimeout(timeout); //Clear previous existing timers

                timeout = setTimeout(() => { //Start the new timer
                    func(); //Execute function after the timer reaches the param delay value
                }, delay);
            };
        },

        _setValueAttr: function (value) { //Gets called on pageLoad and when this.set is called (On pageLoad the value is received from the CMS).
            if (value.latitude && value.latitude) {
                //Update the map & marker with the parsed coordinates 
                this._updateMap(value);
            }

            //Important for the CMS to bind correctly
            this.onFocus();

            //Set the coordinates in the CMS
            this._set("value", value);

            //Notifies the CMS of the changed values, must be done for the Editor to work correctly.
            this.onChange(value);
        },

        _setupPrefix: function () {
            //No prefix supplied, return without rendering prefixInput / dropdown
            if (!this.searchPrefix) {
                return;
            }

            if (this.searchPrefix.length > 1) {
                //We received an array of prefixes, create options & display prefix dropdown.
                this.searchPrefix.forEach(prefix => {
                    const option = document.createElement("option");
                    option.textContent = prefix;
                    this.prefixDropdown.appendChild(option);
                });
                this.prefixDropdown.classList.remove("hidden");
            }
            else if (this.searchPrefix.length === 1) {
                //We received one search prefix, display prefixInput.
                this.prefixInput.value = this.searchPrefix;
                this.prefixInput.classList.remove("hidden");
            }
        },

        _setupSearch: function () {
            //On searchbox keyup, call debounce for searchAddress method with 300ms delay, if key is enter, directly search.
            const debouncedSearch = this._debounce(() => this._searchAutoComplete(this.searchbox.value), 300);
            on(this.searchbox, 'keyup', (e) => {
                if (e.key === "Enter") {
                    this._searchAddress(this._appendPrefix(this.searchbox.value)); //Direct search
                }
                else if (e.key === "ArrowDown") { //Set focus on first address-result to navigate with arrow-keys.
                    e.preventDefault(); //Prevent page scrolling

                    let firstItem = query(".address-result")[0];
                    if (firstItem) {
                        firstItem.focus();
                    }
                }
                else {
                    debouncedSearch(); //calling the function that is returned from debouce
                }
            });

            on(this.searchButton, 'click', () => {
                this._searchAddress(this._appendPrefix(this.searchbox.value));
            });

            on(this.searchForm, "submit", (e) => {
                e.preventDefault(); //Prevent default behavior (Refresh)
            });
        },

        _searchAutoComplete: function (address) {
            if (!address) {
                this.resultDropdown.classList.add("hidden");
                return;
            }

            //Append the search prefix
            address = this._appendPrefix(address);

            fetch(`${this.baseUrl}/SearchAutoComplete?address=${encodeURIComponent(address)}`)
                .then(response => response.json())
                .then(data => {
                    this._updateDropdown(data); //Features contains a list of results.
                });
        },

        _updateDropdown: function (results) {
            this.resultDropdown.classList.remove("hidden"); //Show dropdown

            //Remove all existing children
            while (this.resultDropdown.firstChild) {
                this.resultDropdown.removeChild(this.resultDropdown.firstChild);
            }

            //No results found
            if (results.length === 0) {
                const li = document.createElement("li");
                li.textContent = "Hittar inga adresser för angivet sökord";
                this.resultDropdown.appendChild(li);
                return;
            }

            //Received results, create a list item for each result.
            results.forEach(result => {

                console.log(result);

                if (result.propertyNumber) { result.address += " " + result.propertyNumber };

                //Photon/OSM autocomplete frequently returns results for other cities, fix by compairing result city & prefix. 
                if (this.searchPrefix && this.prefixDropdown.value !== "") {
                    if (this.searchPrefix.length > 1 && result.city !== null && result.city !== this.prefixDropdown.value) {
                        return;
                    }
                    else if (this.searchPrefix.length === 1 && result.city !== null && result.city !== this.searchPrefix) {
                        return;
                    }
                }

                const li = document.createElement("li")
                li.setAttribute("tabindex", "0");
                li.setAttribute("class", "address-result")
                li.textContent = this._removePrefix(result.address);

                on(li, "click", function () {
                    if (result.longitude && result.latitude) { this._autoCompleteSelect(result); }
                    else { this._searchAddress(result.address); }
                }.bind(this));

                on(li, "keydown", function (e) { //Could potentially be replaced by using HTML Select, but that requires alot of work.
                    if (e.key === "Enter") {
                        if (result.longitude && result.latitude) { this._autoCompleteSelect(result); }
                        else { this._searchAddress(result.address); }
                    }
                    else if (e.key === " ") { //e.key "Spacebar" doesn't work, identifier for spacebar is a blankspace for some reason?
                        this.searchbox.value = li.textContent;
                        this.searchbox.focus();
                    }
                    else if (e.key === "ArrowDown" || e.key === "ArrowUp") {
                        e.preventDefault(); //Prevent page scrolling

                        let items = query(".address-result"); //Get all list items in the dropdown
                        let currentIndex = items.indexOf(e.target);

                        if (e.key === "ArrowDown" && currentIndex < items.length - 1) {
                            items[currentIndex + 1].focus(); //Move to next item if not at last pos
                        } else if (e.key === "ArrowUp" && currentIndex > 0) {
                            items[currentIndex - 1].focus(); //Move to previous item if not at first pos
                        } else if (e.key === "ArrowUp" && currentIndex === 0) {
                            this.searchbox.focus(); //Focus the searchbox again if on first pos
                        }
                    }
                }.bind(this));

                this.resultDropdown.appendChild(li);
            });
        },

        _searchAddress: function (address) {
            //Calls the SearchAddress API.
            //The encode URI - component makes sure the url isn't broken with special characters or blank spaces, etc.
            fetch(`${this.baseUrl}/SearchAddress?address=${encodeURIComponent(address)}`)
                .then(response => {
                    if (response.status === 204) { //Backend returned 204 (No Content)
                        this.resultDropdown.classList.remove("hidden"); //Show the user that no addresses were found
                        return;
                    }
                    return response.json();
                })
                .then(data => {
                    if (data != null) {
                        this.resultDropdown.classList.add("hidden"); //Remove dropdown
                        const lng = data[0].lon;
                        const lat = data[0].lat;

                        this.set("value", { latitude: lat, longitude: lng });
                    } 
                }, (error) => {
                    console.error("Error with api-call: ", error);
                });
        },

        _autoCompleteSelect: function (data) {
            //Hide the dropdown, populate searchBox with address & update CMS with coordinates. 
            this.resultDropdown.classList.add("hidden");
            this.searchbox.value = this._removePrefix(data.address);
            this.set("value", { latitude: data.latitude, longitude: data.longitude });
        },

        _appendPrefix: function (address) {
            if (!this.searchPrefix) {
                return address;
            }

            if (this.searchPrefix.length > 1) {
                //User selected empty prefix in dropdown, return address without prefix
                if (this.prefixDropdown.value === "") { return address }

                //Return the selected option from the dropdown
                return `${this.prefixDropdown.value} ${address}`;
            }
            else {
                //Return the specified prefix
                return `${this.searchPrefix} ${address}`;
            }
        },

        _removePrefix: function (address) {
            if (!this.searchPrefix) {
                return address;
            }

            if (this.searchPrefix.length > 1) {
                if (this.prefixDropdown.value === "") { return address }

                return address.replace(`${this.prefixDropdown.value} `, "")
            }
            else {
                return address.replace(`${this.searchPrefix} `, "")
            }
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

                L.tileLayer(`${this.baseUrl}/GetTileImage?z={z}&y={y}&x={x}`, {
                    attribution: '&copy; OpenStreetMap',
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
                    this.map.invalidateSize(); //Arrow function retains "this" context
                }, 500);
            }
        },
    });
});
