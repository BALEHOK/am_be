<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleMapsControl.ascx.cs" Inherits="AssetSite.Controls.GoogleMapsControl" %>

<div id="<% =this.UniqueID %>" style="width:300px; height:300px"></div>
<table>
    <tr>
        <td>Source</td>
        <td>Destination</td>
    </tr>
    <tr>
        <td><input type="text" id="SRC_ADDR" runat="server"/></td>
        <td><input type="text" id="DST_ADDR" runat="server"/></td>
    </tr>
    <%if (this.Editable){ %>
    <tr>
        <td colspan="2"><input type="button" id="showPath" onclick="codeAddress();" title="Show" value="Show"></td>
    </tr>
    <%} %>
    <tr>
        <td colspan="2">
            <div id="directions-panel" style="overflow: scroll; height: 200px;"></div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            Additional info
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" Columns="40" ID="additionalInfo" />
        </td>
    </tr>
</table>

<script type="text/javascript">
    var map;
    var directionsDisplay;
    var geocoder;
    var directionsService;
    var geodesic;
    var poly;
    var src_latlng;
    var dst_latlng;
    var src_address;
    var dst_address;
    var myLatlng;
    var marker;
    var sPos;
    var dPos;
    var markersArray = [];

    function clearOverlays() {
        for (var i = 0; i < markersArray.length; i++) {
            markersArray[i].setMap(null);
        }

        markersArray = [];

        if (poly) {
            poly.setMap(null);
        }
    }

    function calcRoute() {
        var start = src_latlng;
        var end = dst_latlng;

        var request = {
            origin: start,
            destination: end,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        };

        directionsService.route(request, function (result, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(result);
            } else {
                alert(status);
            }
        });
    }

    function codeAddress() {
        clearOverlays();

        src_address = document.getElementById("<%=SRC_ADDR.ClientID%>").value;
        dst_address = document.getElementById("<%=DST_ADDR.ClientID%>").value;

        var polyOptions = {
            strokeColor: '#FF0000',
            strokeOpacity: 1.0,
            strokeWeight: 3
        }
        poly = new google.maps.Polyline(polyOptions);
        poly.setMap(map);
        directionsDisplay.setMap(map);

        geocoder.geocode({ 'address': src_address, 'latLng': myLatlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                var path = poly.getPath();
                path.push(results[0].geometry.location);
                src_latlng = results[0].geometry.location;
                marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
                markersArray.push(marker);
            }
        });

        geocoder.geocode({ 'address': dst_address, 'latLng': myLatlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                map.setZoom(8);

                var path = poly.getPath();
                path.push(results[0].geometry.location);
                dst_latlng = results[0].geometry.location;
                marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
                markersArray.push(marker);
                calcRoute();
            }
            else {
                alert(status);
            }
        });
    }

    function notsuccess() {
        showDefault();
    }

    function success(position) {
        var lat = position.coords.latitude;
        var long = position.coords.longitude;
        myLatlng = new google.maps.LatLng(lat, long);

        var myOptions = {
            center: myLatlng,
            zoom: 12,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            disableDefaultUI: false
        };

        map = new google.maps.Map(document.getElementById("<% =this.UniqueID %>"), myOptions);
    }

    function showDefault() {
        var lat = 50.8411;
        var long = 4.3564;
        myLatlng = new google.maps.LatLng(lat, long);
        var myOptions = {
            center: myLatlng,
            zoom: 12,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            disableDefaultUI: false
        };

        map = new google.maps.Map(document.getElementById("<% =this.UniqueID %>"), myOptions);
    }

    $(document).ready(function () {

        directionsService = new google.maps.DirectionsService();
        directionsDisplay = new google.maps.DirectionsRenderer();
        geocoder = new google.maps.Geocoder();

        directionsDisplay.setPanel(document.getElementById("directions-panel"));

        src_address = document.getElementById("<%=SRC_ADDR.ClientID%>").value;
        dst_address = document.getElementById("<%=DST_ADDR.ClientID%>").value;

        if (src_address && dst_address) {
            showDefault();
            codeAddress();
        }
        else {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(success, notsuccess, { timeout: 10000 });
            } else {
                showDefault();
            }
        }
    });
</script>