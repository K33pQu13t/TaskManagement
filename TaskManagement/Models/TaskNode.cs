using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagement.Models
{
    public class TaskNode
    {
        /// <summary>
        /// наименование задачи
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// описание задачи
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// список исполнителей
        /// </summary>
        public List<string> ExecutorList { get; set; }

        /// <summary>
        /// статус задачи
        /// </summary>
        public State TaskState { get; set; }

        /// <summary>
        /// дата регистрации задачи в системе
        /// </summary>
        public DateTime RegisterDate { get; set; }
        /// <summary>
        /// плановая трудоёмкость задачи
        /// </summary>
        public int ExecutionTimePlanned { get; set; }
        /// <summary>
        /// фактическое время выполнения
        /// </summary>
        public int ExecutionTimeActual { get; set; }
        /// <summary>
        /// дата завершения задачи
        /// </summary>
        public DateTime CompleteDate { get; set; }



        public enum State
        {
            Assigned,
            Executing,
            Suspend,
            Complete
        }
    }
}
