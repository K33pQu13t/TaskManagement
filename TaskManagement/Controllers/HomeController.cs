using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {
        public ITaskNodeRepository TaskNodeRepository { get; set; }
        readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IServiceProvider provider)
        {
            _logger = logger;

            TaskNodeRepository = provider.GetService<ITaskNodeRepository>();
        }

        public async Task<IActionResult> Index()
        {
            List<TaskNode> taskNodeList = await TaskNodeRepository.LoadAsync();
            //ViewBag.TaskNodeRecursivePartial = taskNodeList;

            return View(taskNodeList);
        }

        /// <summary>
        /// отражает детали указаной задачи
        /// </summary>
        /// <param name="id"></param>
        /// <returns>частичное представление просмотра конкретной задачи</returns>
        [HttpGet]
        public async Task<ActionResult> ShowTaskNodeDetails(int id)
        {
            TaskNode taskNode = await TaskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// отражает форму для добавления задачи
        /// </summary>
        /// <param name="parentId">id родительской задачи</param>
        /// <returns>частичное представление добавления задачи</returns>
        [HttpGet]
        public ActionResult AddTaskNodeShow(int parentId = default)
        {
            TaskNode taskNode = new TaskNode();
            return PartialView("TaskNodeCreatePartial", 
                //остальные поля дефолтно встанут
                new TaskNodeAddTaskViewModel 
                {
                     ParentId = parentId,
                     TaskState = taskNode.TaskState,
                     RegisterDate = taskNode.RegisterDate
                });
        }

        /// <summary>
        /// отражает форму редактирования задачи
        /// </summary>
        /// <param name="id"></param>
        /// <returns>частичное представление редактирования задачи</returns>
        [HttpGet]
        public async Task<ActionResult> EditTaskNodeDetailsShow(int id)
        {
            TaskNode taskNodeEditing = await TaskNodeRepository.FindById(id);
            if (taskNodeEditing == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodeEditPartial", null);
            }
            return PartialView("TaskNodeEditPartial",
                new TaskNodeAddTaskViewModel
                {
                    Id = taskNodeEditing.Id,
                    Title = taskNodeEditing.Title,
                    Description = taskNodeEditing.Description,
                    ParentId = taskNodeEditing.Parent == null ? default : taskNodeEditing.Parent.Id,
                    HasChildren = taskNodeEditing.ChildrenList.Count > 0,
                    Executors = taskNodeEditing.Executors,
                    TaskState = taskNodeEditing.TaskState,
                    RegisterDate = taskNodeEditing.RegisterDate,
                    ExecutionTimePlanned = taskNodeEditing.ExecutionTimePlanned,
                    ExecutionTimePlannedThis = taskNodeEditing.GetThisExecutionTimePlanned(),
                    ExecutionTimeActual = taskNodeEditing.ExecutionTimeActual,
                    ExecutionTimeActualThis = taskNodeEditing.GetThisExecutionTimeActual()
                }); ;
        }

        /// <summary>
        /// перезаписывает свойства задачи по её id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="executors"></param>
        /// <returns>частичное представление просмотра задачи</returns>
        [HttpPost]
        //public async Task<ActionResult> EditTaskNodeDetailsSave(int id, string title, string description, string executors, int executionTimePlanned, int parentId)
        public async Task<ActionResult> EditTaskNodeDetailsSave(TaskNodeAddTaskViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                TaskNode taskNode = await TaskNodeRepository.FindById(viewModel.Id);
                //если добавляем новую задачу/подзадачу
                if (taskNode == null)
                {
                    taskNode = new TaskNode(viewModel.Title, viewModel.Description, viewModel.Executors, viewModel.ExecutionTimePlanned);
                    //если подзадача, то в родителя ещё добавляем ссылку
                    if (viewModel.ParentId != default)
                    {
                        TaskNode taskNodeParent = await TaskNodeRepository.FindById(viewModel.ParentId);
                        taskNodeParent.AddSubtask(taskNode);
                    }

                    await TaskNodeRepository.AddTaskAsync(taskNode);
                }
                //если редактируем имеющуюся задачу
                else
                {
                    taskNode.Title = viewModel.Title;
                    taskNode.Description = viewModel.Description;
                    taskNode.Executors = viewModel.Executors;
                    taskNode.ExecutionTimePlanned = viewModel.ExecutionTimePlanned;

                    await TaskNodeRepository.SaveAsync();
                }

                return PartialView("TaskNodePartial", taskNode);
            }
            return StatusCode(400);
        }

        /// <summary>
        /// переводит задачу в статус Execute
        /// </summary>
        /// <param name="id"></param>
        /// <returns>частичное представление просмотра задачи</returns>
        public async Task<ActionResult> ExecuteTask(int id)
        {
            TaskNode taskNode = await TaskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            taskNode.Execute();
            await TaskNodeRepository.SaveAsync();
            //_taskNodeRepository.EditAsync(taskNode);
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// переводит задачу в статус Suspend
        /// </summary>
        /// <param name="id"></param>
        /// <returns>частичное представление просмотра задачи</returns>
        public async Task<ActionResult> SuspendTask(int id)
        {
            TaskNode taskNode = await TaskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            taskNode.Suspend();
            await TaskNodeRepository.SaveAsync();
            //_taskNodeRepository.EditAsync(taskNode);
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// переводит задачу в статус Complete
        /// </summary>
        /// <param name="id"></param>
        /// <returns>частичное представление просмотра задачи</returns>
        public async Task<ActionResult> CompleteTask(int id)
        {
            TaskNode taskNode = await TaskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            taskNode.Complete();
            await TaskNodeRepository.SaveAsync();
            //_taskNodeRepository.EditAsync(taskNode);
            return PartialView("TaskNodePartial", taskNode);
        }

        public async Task<IActionResult> RemoveTaskNode(int id)
        {
            await TaskNodeRepository.Remove(id);
            //чтобы удалить с области просмотра удалённую задачу
            return PartialView("TaskNodePartial", null);
        }

        /// <summary>
        /// обновляет древовидное отображение задач
        /// </summary>
        /// <returns>частичное представление отображения древовидной структуры задач</returns>
        [HttpGet]
        public async Task<IActionResult> RefreshTaskExplorer()
        {
            List<TaskNode> taskNodeList = await TaskNodeRepository.LoadAsync();
            return PartialView("TaskNodeRecursiveCyclePartial", taskNodeList);
        }

        /// <summary>
        /// перезаписывает куки указанной культуры для хранения культуры
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="returnUrl"></param>
        /// <returns>обновляет страницу</returns>
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
