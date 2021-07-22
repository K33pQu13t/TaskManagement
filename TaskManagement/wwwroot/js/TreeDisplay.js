function display(obj, result) {
    if (obj == null)
        return result;
    var ul, li;
    for (var k in obj) {
        var value = obj[k];
        if (typeof (value) == "object") {
            li = result.appendChild(document.createElement("li"));
            li.innerHTML = k + ":";
            if (value != null) {
                ul = li.appendChild(document.createElement("ul"));
                display(value, ul);
            }
        } else {
            li = result.appendChild(document.createElement("li"));
            li.innerHTML = k + ": " + value;
        }
    }
    return result;
}
document.getElementById("displayDiv").appendChild(
    display(jsonObj, document.createElement("ul")));