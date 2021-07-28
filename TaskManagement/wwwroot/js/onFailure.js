//не срабатывает, функция не вызывается
function onFailure(XMLHttpRequest, textStatus, errorThrown) {
    document.getElementById("#task-viewer").innerHTML = textStatus;
    alert("failure");
};