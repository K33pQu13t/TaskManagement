using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{
    public class TaskNodeController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        ITaskNodeRepository _taskNodeRepository;

        public TaskNodeController(IWebHostEnvironment environment)
        {
            _appEnvironment = environment;
            _taskNodeRepository = new TaskNodeRepository();
        }

        
    }
}
