//эта функция обновляет дерево и отображение деталей задачи (если открыты) каждые 60 секунд
$(document).ready(
    setInterval(function () {
        refresh();
        setCollapseListeners();

        if (document.getElementById("taskid")) {
            var id = document.getElementById("taskid").innerHTML;
            document.getElementById("task" + id).click();
        }
    }, 60000)
);