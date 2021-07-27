//эта функция обновляет дерево каждые 10 секунд
$(document).ready(
    setInterval(function () {
        //alert("tree refresh");
        document.getElementById("refresher").click();
        if (document.getElementById("taskid")) {
            //alert("view refresh");
            var id = document.getElementById("taskid").innerHTML;
            document.getElementById("task" + id).click();
        }
    }, 10000)
);