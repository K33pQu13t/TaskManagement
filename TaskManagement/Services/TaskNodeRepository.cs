﻿using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationContext _db;
        /// <summary>
        /// список задач которые сейчас выполняются. Хранит время выполнения в миллисекундах
        /// </summary>
        private List<TaskNodeExecution> _taskNodeExecutionList;

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
                if (taskNode.TaskState == TaskNode.State.Executing)
                    _taskNodeExecutionList.Add(new TaskNodeExecution { Node = taskNode });
            }

            //запускаем синхронизацию фактического времени выполнения
            Task.Run(() => ActualTimeUpdater());
        }

        /// <summary>
        /// добавляет задачу
        /// </summary>
        /// <param name="taskNode"></param>
        /// <returns></returns>
        public async Task AddTaskAsync(TaskNode taskNode)
        {
            await _db.TaskNodeList.AddAsync(taskNode);
            await SaveAsync();
        }

        /// <summary>
        /// добавляет к задаче подзадачу
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="taskNodeChild"></param>
        /// <returns></returns>
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

        /// <summary>
        /// сохраняет изменения в объекте в бд
        /// </summary>
        /// <param name="taskNodeEdited">изменённый объект</param>
        /// <returns></returns>
        public async Task EditAsync(TaskNode taskNodeEdited)
        {
            TaskNode taskNode = await FindById(taskNodeEdited.Id);

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

        //сохраняет любые изменения
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();

            //синхронизируем объекты из _taskNodeExecutionList с базой
            List<TaskNode> taskNodeList = await _db.TaskNodeList.ToListAsync();
            //удаляем все которые не Executing
            taskNodeList.RemoveAll(node => node.TaskState != TaskNode.State.Executing);
            //создаём из них объекты TaskNodeExecution и добавляем 
            foreach (TaskNode taskNode in taskNodeList)
            {
                _taskNodeExecutionList.Add(new TaskNodeExecution { Node = taskNode });
            }
            //оставляем только те которые Executing
            //(там в процессе задачи могут стать Suspend или Complete, убираем их чтоб оптимизировать foreach в апдейтере)
            _taskNodeExecutionList.RemoveAll(execution => execution.Node.TaskState != TaskNode.State.Executing);
        }

        public async Task<TaskNode> FindById(int id)
        {
            return await _db.TaskNodeList.FindAsync(id);
        }

        /// <summary>
        /// удаляет задачу вместе со всеми её вложениями
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskNodeListToRemove"></param>
        /// <returns>true если база данных содержала залачу и она была успешно удалена вместе со всеми подзадачами</returns>
        public async Task<bool> Remove(int id)
        {
            TaskNode taskNode = await FindById(id);
            if (taskNode != null && taskNode.CanBeDeleted())
            {
                List<TaskNode> taskNodeListToRemove = taskNode.GetAllDemensions();

                _db.TaskNodeList.RemoveRange(taskNodeListToRemove);
                await SaveAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// запускает обновление фактической трудоёмкости. Трудоёмкость растёт только у задач с состоянием Executing
        /// </summary>
        /// <returns></returns>
        private async Task ActualTimeUpdater()
        {
            //эта система не считает время, если код не запущен. Не знаю, недостаток ли это, просто факт
            int oneMinute = 60000;
            int oneHour = 3600000;

            while (true)
            {
                //await Task.Delay(oneMinute);
                await Task.Delay(5000);  //для отладки чтоб быстрее смотреть как изменяется время
                foreach (TaskNodeExecution taskNodeExecution in _taskNodeExecutionList)
                {
                    //по-идее, в методе SaveAsync висит логика, которая оставляет в _taskNodeExecutionList только те объекты,
                    //которые Executing, но на всякий случай
                    if (taskNodeExecution.Node.TaskState == TaskNode.State.Executing)
                    {
                        //taskNodeExecution.Seconds += oneMinute;
                        taskNodeExecution.Milliseconds += oneHour; //для отладки чтоб быстрее смотреть как изменяется время
                    }
                    if(taskNodeExecution.Milliseconds >= oneHour)
                    {
                        taskNodeExecution.Milliseconds = 0;
                        taskNodeExecution.Node.AddExecutionTimeActual(1);
                        await SaveAsync();
                    }
                }
            }
        }
    }
}
