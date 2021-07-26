using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Models;
using TaskManagement.Helpers;

namespace TaskManagement.Services
{
    public class TaskNodeRepository : ITaskNodeRepository
    {
        readonly ApplicationContext _db;

        private List<TaskNodeExecution> _taskNodeExecutionList;

        private Task _timeUpdater;

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

            _taskNodeExecutionList = new List<TaskNodeExecution>();
            List<TaskNode> taskNodeList = _db.TaskNodeList.ToList();
            foreach (TaskNode taskNode in taskNodeList)
            {
                _taskNodeExecutionList.Add(new TaskNodeExecution { Node = taskNode });
            }

            //запускаем синхронизацию фактического времени выполнения
            _timeUpdater = Task.Run(() => ActualTimeUpdater());
        }

        public async Task AddTaskAsync(TaskNode taskNode)
        {
            await _db.TaskNodeList.AddAsync(taskNode);
            await SaveAsync();
        }

        public async Task AddSubtaskAsync(int parentId, TaskNode taskNodeChild)
        {
            TaskNode taskNodeParent = await FindById(parentId);
            if (taskNodeParent != null && taskNodeChild.Parent == null)
            {
                taskNodeParent.AddSubtask(taskNodeChild);
         
                await AddTaskAsync(taskNodeChild);
            }
            //todo: else throw new exception?
        }

        public async Task EditAsync(TaskNode newTaskNode)
        {
            TaskNode taskNode = await FindById(newTaskNode.Id);

            if (taskNode != null)
            {
                await SaveAsync();
            }
            //todo: else throw new exception?
        }

        public async Task<List<TaskNode>> LoadAsync()
        {
            return await _db.TaskNodeList.ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();

            //синхронизируем объекты из _taskNodeExecutionList с базой
            _taskNodeExecutionList = new List<TaskNodeExecution>();
            List<TaskNode> taskNodeList = _db.TaskNodeList.ToList();
            foreach (TaskNode taskNode in taskNodeList)
            {
                _taskNodeExecutionList.Add(new TaskNodeExecution { Node = taskNode });
            }
        }

        public async Task<TaskNode> FindById(int id)
        {
            List<TaskNode> taskNodeList = await LoadAsync();
            return taskNodeList.FirstOrDefault(taskNode => taskNode.Id == id);
        }

        /// <summary>
        /// удаляет задачу вместе со всеми её вложениями
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskNodeListToRemove"></param>
        /// <returns>true если база данных содержала залачу и она была успешно удалена вместе со всеми подзадачами</returns>
        public async Task<bool> Remove(int id, List<TaskNode> taskNodeListToRemove = null)
        {
            TaskNode taskNode = await FindById(id);
            if (taskNode != null)
            {
                //если первый вызов (с вьюхи), то инициализируем список
                if (taskNodeListToRemove == null)
                {
                    taskNodeListToRemove = new List<TaskNode>();
                    //добавляем сразу задачу, id которой был передан методу
                    taskNodeListToRemove.Add(taskNode);
                }

                foreach (TaskNode taskNodeChild in taskNode.ChildrenList)
                {
                    //рекурсивно добавляем ссылки на всё, что подлежит удалению
                    taskNodeListToRemove.Add(taskNodeChild);
                    await Remove(taskNodeChild.Id, taskNodeListToRemove);
                }
                
                //чтобы не произошёл выход из метода на первой самой вложенной задаче
                //сюда попадём только когда рекурсия вернётся к объекту, который её начал
                if (taskNode.Parent == null)
                {
                    _db.RemoveRange(taskNodeListToRemove);
                    await SaveAsync();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// запускает обновление фактической трудоёмкости. Трудоёмкость растёт только у задач с состоянием Executing
        /// </summary>
        /// <returns></returns>
        private async Task ActualTimeUpdater()
        {
            int oneMinute = 60000;
            int oneHour = 3600000;

          

            while (true)
            {
                //await Task.Delay(oneMinute);
                await Task.Delay(5000);  //для отладки чтоб быстрее смотреть как изменяется время
                foreach (TaskNodeExecution taskNodeExecution in _taskNodeExecutionList)
                {
                    if (taskNodeExecution.Node.TaskState == TaskNode.State.Executing)
                    {
                        //taskNodeExecution.Seconds += oneMinute;
                        taskNodeExecution.Seconds += oneHour; //для отладки чтоб быстрее смотреть как изменяется время
                    }
                    if(taskNodeExecution.Seconds >= oneHour)
                    {
                        taskNodeExecution.Seconds = 0;
                        taskNodeExecution.Node.AddExecutionTimeActual(1);
                        await SaveAsync();
                    }
                }
            }
        }
    }
}
