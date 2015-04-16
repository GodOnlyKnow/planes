var ZoomControl = function () {
    this.anchor = BMAP_ANCHOR_TOP_LEFT;
    this.offset = new BMap.Size(20, 10);
}

ZoomControl.prototype = new BMap.Control();
ZoomControl.prototype.initialize = function (map) {
    var div = document.createElement("div");
    div.appendChild(document.createTextNode("自动定位中..."));
    div.style.border = "1px solid gray";
    div.style.backgroundColor = "white";
    map.getContainer().appendChild(div);
    return div;
}

function addMarker(map) {
    map.clearOverlays();
    var marker = new BMap.Marker(pt);
    map.addOverlay(marker);
    marker.setAnimation(BMAP_ANIMATION_BOUNCE);
}