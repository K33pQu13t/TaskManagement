using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Helpers;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {
        public ITaskNodeRepository TaskNodeRepository { get; set; }
        private readonly IStringLocalizer<HomeController> _localizer;

        readonly ILogger<HomeController> _logger;

        private KeyNotFoundException _exceptionTaskNotFound;
        private AppException _exceptionFieldsIsInvalid;
        private AppException _exceptionCantExecute;
        private AppException _exceptionCantRemove;
        private AppException _exceptionCantSuspend;
        private AppException _exceptionCantComplete;

        public HomeController(IStringLocalizer<HomeController> localizer, ILogger<HomeController> logger, IServiceProvider provider)
        {
            _localizer = localizer;
            _logger = logger;

            _exceptionTaskNotFound = new KeyNotFoundException(_localizer["TaskNotFound"]);
            _exceptionFieldsIsInvalid = new AppException(_localizer["FieldsIsInvalid"]);
            _exceptionCantExecute = new AppException(_localizer["CantExecute"]);
            _exceptionCantRemove = new AppException(_localizer["CantRemove"]);
            _exceptionCantSuspend = new AppException(_localizer["CantSuspend"]);
            _exceptionCantComplete = new AppException(_localizer["CantComplete"]);

            TaskNodeRepository = provider.GetService<ITaskNodeRepository>();
        }

        public async Task<IActionResult> Index()
        {
            List<TaskNode> taskNodeList = await TaskNodeRepository.LoadAsync();

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
                throw _exceptionTaskNotFound;
            }
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// отражает форму для добавления задачи
        /// </summary>
        /// <param name="parentId">id родительской задачи. 0 значит новая задача</param>
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
        /// <param name="id">id задачи которую надо дотредактировать</param>
        /// <returns>частичное представление редактирования задачи</returns>
        [HttpGet]
        public async Task<ActionResult> EditTaskNodeDetailsShow(int id)
        {
            TaskNode taskNodeEditing = await TaskNodeRepository.FindById(id);
            if (taskNodeEditing == null)
            {
                throw _exceptionTaskNotFound;
            }
            return PartialView("TaskNodeEditPartial",
                new TaskNodeAddTaskViewModel
                {
                    Id = taskNodeEditing.Id,
                    Title = taskNodeEditing.Title,
                    Description = taskNodeEditing.Description,
                    ParentId = taskNodeEditing.Parent == null ? default : taskNodeEditing.Parent.Id,
                    IsHavingChildren = taskNodeEditing.IsHavingChildren(),
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

                    await TaskNodeRepository.SaveAsync();
                }

                return PartialView("TaskNodePartial", taskNode);
            }
            throw _exceptionFieldsIsInvalid;
        }

        /// <summary>
        /// переводит задачу в статус Execute
        /// </summary>
        /// <param name="id">id задачи</param>
        /// <returns>частичное представление просмотра задачи</returns>
        public async Task<ActionResult> ExecuteTask(int id)
        {
            TaskNode taskNode = await TaskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                throw _exceptionTaskNotFound;
            }

            if (taskNode.CanBeExecuted())
            {
                taskNode.Execute();
                await TaskNodeRepository.SaveAsync();
                return PartialView("TaskNodePartial", taskNode);
            }
                throw _exceptionCantExecute;
        }

        /// <summary>
        /// переводит задачу в статус Suspend
        /// </summary>
        /// <param name="id">id задачи</param>
        /// <returns>частичное представление просмотра задачи</returns>
        public async Task<ActionResult> SuspendTask(int id)
        {
            TaskNode taskNode = await TaskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                throw _exceptionTaskNotFound;
            }

            if (taskNode.CanBeSuspended())
            {
                taskNode.Suspend();
                await TaskNodeRepository.SaveAsync();
                return await ShowTaskNodeDetails(id);
            }
            throw _exceptionCantSuspend;
        }

        /// <summary>
        /// переводит задачу в статус Complete
        /// </summary>
        /// <param name="id">id задачи</param>
        /// <returns>частичное представление просмотра задачи</returns>
        public async Task<ActionResult> CompleteTask(int id)
        {
            TaskNode taskNode = await TaskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                throw _exceptionFieldsIsInvalid;
            }

            if (taskNode.CanBeCompleted())
            {
                taskNode.Complete();
                await TaskNodeRepository.SaveAsync();
                return await ShowTaskNodeDetails(id);
            }
            throw _exceptionCantComplete;
        }

        /// <summary>
        /// удаляет задачу и все её подзадачи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveTaskNode(int id)
        {
            //если успешно удалили
            if (await TaskNodeRepository.Remove(id))
            {
                //чтобы удалить с области просмотра удалённую задачу
                return PartialView("TaskNodePartial", null);
            }
            throw _exceptionCantRemove;
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
        public async Task<IActionResult> SetLanguage(string culture, int id = default)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            //отразить пустоту вместо деталей о задаче
            //по-хорошему, сюда бы передать id открытой задачи чтоб у неё язык сразу сменился, но я не осилил js
            return PartialView("TaskNodePartial", await TaskNodeRepository.FindById(id));
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
