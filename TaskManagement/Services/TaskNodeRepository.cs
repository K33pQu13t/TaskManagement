using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public class TaskNodeRepository : ITaskNodeRepository
    {
        ApplicationContext _db;


        public TaskNodeRepository()
        {
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            //builder.SetBasePath();
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;
            _db = new ApplicationContext(options);
        }

        public async Task AddTaskAsync(TaskNode taskNode)
        {
            await _db.TaskNodeList.AddAsync(taskNode);
            await SaveAsync();
        }

        public async Task AddSubtaskAsync(int parentId, TaskNode taskNodeChild)
        {
            TaskNode taskNodeParent = FindById(parentId);
            if (taskNodeParent != null)
            {
                taskNodeParent.AddSubtask(taskNodeChild);
                await AddTaskAsync(taskNodeChild);
            }
            //todo: else throw new exception?
        }

        public async Task EditAsync(TaskNode newTaskNode)
        {
            List<TaskNode> taskNodeList = Load();
            TaskNode taskNode = taskNodeList.FirstOrDefault(taskNode => 
                taskNode.Id == newTaskNode.Id);

            if (taskNode != null)
            {
                taskNode = newTaskNode;
                await SaveAsync();
            }
            //todo: else throw new exception?
        }

        public List<TaskNode> Load()
        {
            return _db.TaskNodeList.ToList();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        private TaskNode FindById(int id)
        {
            return Load().FirstOrDefault(taskNode => taskNode.Id == id);
        }
    }
}
