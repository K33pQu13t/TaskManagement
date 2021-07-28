const debug = true;

function log(message) {
    if (!debug) { return }
    console.log('Degub:', message)
}

//обновляет дереро
function refresh() {
    document.getElementById("refresher").click();
    setId();
    log("refresh");
};

function setCollapseListeners() {
    let categoryTitleObjects = document.getElementsByClassName('collapse-button');
    let toggleView = function () {
        console.log(this);
        this.innerHTML = this.innerHTML == '&gt;' ? 'v' : '&gt;';
        this.parentNode.parentNode.classList.toggle('collapsed');
    }
    Array.from(categoryTitleObjects).forEach(object => {
        object.addEventListener('click', toggleView);
    });
}

let getId = function () {
    log("getId");
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
    log("setId");
}