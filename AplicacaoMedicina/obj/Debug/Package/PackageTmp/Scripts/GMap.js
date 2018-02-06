$(document).ready(function () {

    var geocoder = new google.maps.Geocoder();
    var lng;
    var lat;
    var map;
    var idInfoBoxAberto;
    var markers = [];
    var infowindow = new google.maps.InfoWindow();
    var autocomplete;
    var markerCluster;
    // var x = document.getElementById("warning");

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(successFunction, errorFunction);
    }

    function initAutoComplete() {
        autocomplete = new google.maps.places.Autocomplete(
            (document.getElementById('txtCidade')), {
                types: ['(cities)'],
                componentRestrictions: { 'country': 'br' }
            });

        places = new google.maps.places.PlacesService(map);

        autocomplete.addListener('place_changed', onPlaceChanged);
    }

    function onPlaceChanged() {
        var place = autocomplete.getPlace();

        if (place.geometry) {
            map.panTo(place.geometry.location);
            map.setZoom(15);
            search();
        }
    }

    function search() {
        var search = {
            bounds: map.getBounds()
        }
    }

    /*    $("#txtCidade").autocomplete({
            source: function (request, response) {
                geocoder.geocode({ 'address': request.term + ', Brasil,', 'region': 'BR' }, function (results, status) {
                    response($.map(results, function (item) {
                        return {
                            label: item.formatted_address,
                            value: item.formatted_address,
                            latitude: item.geometry.location.lat(),
                            longitude: item.geometry.location.lng()
                        }
                    }));
                })
            },
            select: function (event, ui) {

                var location = new google.maps.LatLng(ui.item.latitude, ui.item.longitude)
                map.setPosition(location);
                map.setCenter(location);
                map.setZoom(13);
            }
        }); */

    /*     function abrirInfoBox(id, marker) {
             if (typeof (idInfoBoxAberto) == 'number' &&
                 typeof (infoBox[idInfoBoxAberto]) == 'object') {
                 infoBox[idInfoBoxAberto].close();
             }

             infoBox[id].open(map, marker);
             idInfoBoxAberto = id;
         }*/

    //Get the latitude and the longitude;
    function successFunction(position) {
        lat = position.coords.latitude;
        lng = position.coords.longitude;
        codeLatLng(lat, lng)
    }

    function errorFunction() {
        alert("Erro na geolocalização");
    }


    function codeLatLng(lat, lng) {

        var latlng = new google.maps.LatLng(lat, lng);
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                //console.log(results)
                if (results[1]) {
                    //formatted address
                    //alert(results[0].formatted_address)
                    //find country name
                    for (var i = 0; i < results[0].address_components.length; i++) {
                        for (var b = 0; b < results[0].address_components[i].types.length; b++) {

                            //there are different types that might hold a city admin_area_lvl_1 usually does in come cases looking for sublocality type will be more appropriate
                            if (results[0].address_components[i].types[b] == "administrative_area_level_2") {
                                //this is the object you are looking for
                                city = results[0].address_components[i];
                                //console.log(city);
                                break;
                            }
                        }
                    }
                    //city data
                    //alert(city.short_name + " " + city.long_name)
                    document.getElementById("txtCidade").value = city.short_name;
                    initMap();

                } else {
                    alert("Não foi possivel pegar sua geolocalização.");
                }
            } else {
                alert("Não foi possivel pegar sua geolocalização.\nErro: " + status);
            }
        });
    }

    function initMap() {
        var center = { lat: lat, lng: lng };


        map = new google.maps.Map(document.getElementById("map"), {
            zoom: 13,
            center: center
        });

        initAutoComplete();

        setMarkers($("#txtCidade").val(), "", "", "Medico");
    }

    function setMarkers(cidad, espec, medic, filter) {

        $.ajax({
            type: "POST",
            url: '../Consultorio/GetEndereco',
            data: { Cidade: cidad, Especialidade: espec, Medico: medic, Filtro: filter },
            success: function (response) {

                if (response && response.success) {
                    address = response.address;
                    
                    for (var i = 0; i < address.length; ++i) {                            
                        (function (i) {

                            var userData = "https://maps.googleapis.com/maps/api/geocode/json?address="
                                + address[i].address + "&key=AIzaSyAU_tiKoaJXyYwl6CJqCS0V8z45pL2LE4w&libraries=places"

                            $.ajax({
                                type: "GET",
                                url: userData,
                                success: function (results, status) {

                                    if (status == "success") {

                                        var searchUrl;

                                        if (filter == "Medico") {
                                            searchUrl = "../Paciente/Confirma/" + address[i].id
                                        } else {
                                            searchUrl = "../Paciente/SelectDoc/" + address[i].id
                                        }

                                        var marker = new google.maps.Marker({
                                            position: {
                                                lat: results.results[0].geometry.location.lat,
                                                lng: results.results[0].geometry.location.lng
                                            },
                                            map: map,
                                            title: address[i].consult,
                                            url: searchUrl
                                        });

                                        markers.push(marker);

                                        var content = address[i].consult;

                                        google.maps.event.addListener(marker, 'mouseover', function (content) {
                                            return function () {
                                                infowindow.close();
                                                infowindow.setContent(content);
                                                infowindow.open(map, this);
                                            }
                                        }(content));

                                        google.maps.event.addListener(marker, 'mouseout', function () {
                                            infowindow.close();
                                        });

                                        if (filter == "Medico") {
                                            google.maps.event.addListener(marker, 'click', function () {
                                                window.location.href = this.url;
                                            });
                                        } else {
                                            google.maps.event.addListener(marker, 'click', function () {
                                                $('.inbox-body').load(this.url)
                                                $("#myModal").modal()
                                            });                                                  
                                        }


                                    } else {
                                        console.log(status);
                                    }
                                }

                            });
                        })(i);
                    }                            
                }


                markerCluster = [];

                markerCluster = new MarkerClusterer(map, markers,
                     { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });
            }
        });

       
    }



    function showPosition(position) {
        var latlon = position.coords.latitude + "," + position.coords.longitude;

        var img_url = "https://maps.googleapis.com/maps/api/staticmap?key=AIzaSyAU_tiKoaJXyYwl6CJqCS0V8z45pL2LE4w&zoom=14&size=400x300&sensor=false&center=" + latlon; //" + "

        document.getElementById("map").innerHTML = "<img src='" + img_url + "'>";
    }


    function getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition);
        } else {
            document.getElementById("warning").innerHTML = "Geolocation is not supported by this browser.";
        }
    }

    function showPosition(position) {
        document.getElementById("warning").innerHTML = "Latitude: " + position.coords.latitude +
            "<br>Longitude: " + position.coords.longitude;
    }

    function showError(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                $("#warning")
                    .html("O usuario não forneceu permissão de geolocalização.");
                break;
            case error.POSITION_UNAVAILABLE:
                $("#warning")
                    .html("Impossivel adquirir a informação.");
                break;
            case error.TIMEOUT:
                $("#warning")
                    .html("O tempo para a resposta excedeu.");
                break;
            case error.UNKNOWN_ERROR:
                $("#warning")
                    .html("Erro desconhecido.");
                break;
        }
    }

    function deleteMarkers() {

        for (var i = 0; i < markers.length; ++i) {
            markers[i].setMap(null);
        }
        markers = [];

    };

    $("#buscaMed").on("click", function () {
        event.preventDefault()
        deleteMarkers();

        setMarkers($('#txtCidade').val(), $('#txtEspecialidade').val(), $('#txtMedico').val(), $('#filtro').val());


    });

});
