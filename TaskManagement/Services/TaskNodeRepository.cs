using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Helpers;
using TaskManagement.Models;

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
        /// <param name="taskNode">задача которую нужно добавить</param>
        /// <returns></returns>
        public async Task AddTaskAsync(TaskNode taskNode)
        {
            await _db.TaskNodeList.AddAsync(taskNode);
            await SaveAsync();
        }

        /// <summary>
        /// сохраняет любые изменения
        /// </summary>
        /// <returns></returns>
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
                //если в списке нет такого TaskNodeExecution, что у него Node это taskNode, то добавляем
                if (!_taskNodeExecutionList.Any(execution => execution.Node == taskNode))
                    _taskNodeExecutionList.Add(new TaskNodeExecution { Node = taskNode });
            }

            //там в процессе задачи могут стать Complete, убираем их чтоб оптимизировать foreach в ActualTimeUpdater
            _taskNodeExecutionList.RemoveAll(execution => execution.Node.TaskState == TaskNode.State.Complete);
        }

        /// <summary>
        /// получить все объекты из бд
        /// </summary>
        /// <returns>возвращает список объектов</returns>
        public async Task<List<TaskNode>> LoadAsync()
        {
            return await _db.TaskNodeList.ToListAsync();
        }

        /// <summary>
        /// удаляет задачу вместе со всеми её вложениями
        /// </summary>
        /// <param name="id">id удаляемой задачи</param>
        /// <returns>true если база данных содержала задачу и она была успешно удалена вместе со всеми подзадачами</returns>
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
        /// получить конкретный объект
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns>возвращает объект по его id</returns>
        public async Task<TaskNode> FindById(int id)
        {
            return await _db.TaskNodeList.FindAsync(id);
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
                await Task.Delay(oneMinute);
                //await Task.Delay(5000);  //для отладки чтоб быстрее смотреть как изменяется время
                foreach (TaskNodeExecution taskNodeExecution in _taskNodeExecutionList)
                {
                    if (taskNodeExecution.Node.TaskState == TaskNode.State.Executing)
                    {
                        taskNodeExecution.Milliseconds += oneMinute;
                        //taskNodeExecution.Milliseconds += oneHour; //для отладки чтоб быстрее смотреть как изменяется время
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
