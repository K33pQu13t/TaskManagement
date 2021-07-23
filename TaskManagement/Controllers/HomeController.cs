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

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger/*, IWebHostEnvironment environment*/)
        {
            _logger = logger;
            //_appEnvironment = environment;
            _taskNodeRepository = new TaskNodeRepository();
        }

        public IActionResult Index()
        {
            List<TaskNode> taskNodeList = _taskNodeRepository.Load();

            ViewBag.TaskNodeRecursive = taskNodeList;

            return View();
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

        public IActionResult AddNewTaskNode(string title, string description, string executors, int executionTimePlanned)
        {
            TaskNode taskNode = new TaskNode(title, description, executors, executionTimePlanned);
            _taskNodeRepository.AddTaskAsync(taskNode);
            return View();
        }

        public async Task<IActionResult> AddNewSubtaskNode(string title, string description, string executors, int executionTimePlanned, int parentId)
        {
            await ((TaskNodeRepository)_taskNodeRepository).AddSubtaskAsync(parentId, new TaskNode(title, description, executors, executionTimePlanned));
            return View();
        }

        //[HttpGet]
        //public JsonResult GetTreeNodes()
        //{
        //    // Tree nodes from db
        //    List<TaskNode> taskNodeList = _taskNodeRepository.Load();
        //    // Tree nodes view model
        //    List<TaskNodeViewModel> taskNodeViewModelList;


        //    taskNodeViewModelList = taskNodeList.Where(taskNode => taskNode.Parent == null)
        //            .Select(taskNode => new TaskNodeViewModel(taskNode)).ToList();


        //    return Json(taskNodeList);
        //}


        //private List<TaskNodeViewModel> GetChildren(List<TaskNode> taskNodeList, int parentId)
        //{
        //    return taskNodeList.Where(l => l.Parent.Id == parentId)
        //        .Select(l => new TaskNodeViewModel
        //        {
        //            Id = l.Id,
        //            Title = l.Title,
        //            Description = l.Description,
        //            Parent = new TaskNodeViewModel(l.Parent),
        //            Children = GetChildren(taskNodeList, l.Id),
        //            State = ((int)l.TaskState),
        //            RegisterDate = l.RegisterDate,
        //            ExecutionTimePlanned = l.ExecutionTimePlanned,
        //            ExecutionTimeActual = l.ExecutionTimeActual,
        //            CompleteDate = l.CompleteDate
        //        }).ToList();
        //}
    }
}
