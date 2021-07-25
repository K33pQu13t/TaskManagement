using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {
        //IWebHostEnvironment _appEnvironment;
        ITaskNodeRepository _taskNodeRepository;

        readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger/*, IWebHostEnvironment environment*/)
        {
            _logger = logger;
            //_appEnvironment = environment;
            _taskNodeRepository = new TaskNodeRepository();
        }

        public IActionResult Index()
        {
            List<TaskNode> taskNodeList = _taskNodeRepository.Load();
            ViewBag.TaskNodeRecursivePartial = taskNodeList;

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>частичное представление просмотра конкретной задачи</returns>
        [HttpGet]
        public async Task<ActionResult> ShowTaskNodeDetails(int id)
        {
            TaskNode taskNode = await _taskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// добавляет задачу в бд
        /// </summary>
        /// <param name="parentId">id родительской задачи</param>
        /// <returns>частичное представление редактирования добавленной задачи</returns>
        [HttpGet]
        public async Task<ActionResult> AddTaskNodeShow(int parentId = default)
        {
            TaskNode taskNodeParent = await _taskNodeRepository.FindById(parentId);
            TaskNode taskNode = new TaskNode();

            //если добавляем новую задачу
       
            //if (taskNodeParent == null)
            //{
            //    //await _taskNodeRepository.AddTaskAsync(taskNode);
            //    return PartialView("TaskNodeCreatePartial", taskNode);
            //}
            ////если добавляем подзадачу
            //else
            //{
            //    //taskNodeParent.AddSubtask(taskNode);
            //    //await _taskNodeRepository.AddTaskAsync(taskNode);
            //    return PartialView("TaskNodeEditPartial", taskNode);
            //}
            if(taskNodeParent != null)
            {
                taskNodeParent.AddSubtask(taskNode);
            }
            return PartialView("TaskNodeCreatePartial", taskNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>частичное представление редактирования задачи</returns>
        [HttpGet]
        public async Task<ActionResult> EditTaskNodeDetailsShow(int id)
        {
            TaskNode taskNode = await _taskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodeEditPartial", null);
            }
            return PartialView("TaskNodeEditPartial", taskNode);
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
        public async Task<ActionResult> EditTaskNodeDetailsSave(int id, string title, string description, string executors, int timePlanned, int parentId = default)
        {
            TaskNode taskNode = await _taskNodeRepository.FindById(id);
            //если добавляем новую задачу/подзадачу
            if (taskNode == null)
            {
                taskNode = new TaskNode(title, description, executors, timePlanned);
                //если подзадача, то в родителя ещё добавляем ссылку
                if (parentId != default)
                {
                    TaskNode taskNodeParent = await _taskNodeRepository.FindById(parentId);
                    taskNodeParent.AddSubtask(taskNode);
                }

                await _taskNodeRepository.AddTaskAsync(taskNode);
            }
            //если редактируем имеющуюся задачу
            else
            {
                taskNode.Title = title;
                taskNode.Description = description;
                taskNode.Executors = executors;
                taskNode.ExecutionTimePlanned = timePlanned;

                await _taskNodeRepository.SaveAsync();
            }

           

            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// переводит задачу в статус Execute
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExecuteTask(int id)
        {
            TaskNode taskNode = await _taskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            taskNode.Execute();
            await _taskNodeRepository.SaveAsync();
            //_taskNodeRepository.EditAsync(taskNode);
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// переводит задачу в статус Suspend
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> SuspendTask(int id)
        {
            TaskNode taskNode = await _taskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            taskNode.Suspend();
            await _taskNodeRepository.SaveAsync();
            //_taskNodeRepository.EditAsync(taskNode);
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// переводит задачу в статус Complete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> CompleteTask(int id)
        {
            TaskNode taskNode = await _taskNodeRepository.FindById(id);
            if (taskNode == null)
            {
                //todo: надо бросить 404
                return PartialView("TaskNodePartial", null);
            }
            taskNode.Complete();
            await _taskNodeRepository.SaveAsync();
            //_taskNodeRepository.EditAsync(taskNode);
            return PartialView("TaskNodePartial", taskNode);
        }

        /// <summary>
        /// обновляет древовидное отображение задач
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RefreshTaskExplorer()
        {
            List<TaskNode> taskNodeList = _taskNodeRepository.Load();
            return PartialView("TaskNodeRecursiveCyclePartial", taskNodeList);
        }

        public async Task<IActionResult> RemoveTaskNode(int id)
        {
            await _taskNodeRepository.Remove(id);
            //чтобы удалить с области просмотра удалённую задачу
            return PartialView("TaskNodePartial", null);
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
