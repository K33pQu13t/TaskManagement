let getId = function () {
    if (document.getElementById("taskid")) {
        return document.getElementById("taskid").innerHTML;
    }
    return 0;
};

function setId() {
    document.querySelectorAll(".detail-button.lang").forEach(
        object => {
            let href = object.getAttribute('href').split("/");
            console.log(href);
            object.setAttribute('href',
                `/${href[1]}/${href[2]}/${getId()}?${href[3].split("?")[1]}`
            );
        }
    );
}
