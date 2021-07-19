using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Services;

namespace TaskManagement.Models
{
    public class TaskNode : Node
    {
        public TaskNode(string _title, string _description, 
            List<string> _executorList, 
            int _executionTimePlanned,
            List<Node> _childrenList = null,
            Node _parent = null) 
                :base(_title, _childrenList, _parent)
        {
            Description = _description;
            ExecutorList = _executorList;
            ExecutionTimePlanned = _executionTimePlanned;

            TaskState = State.Assigned;
            RegisterDate = DateTime.Now;
        }

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
        /// плановая трудоёмкость задачи в часах
        /// </summary>
        public int ExecutionTimePlanned 
        {
            get 
            {
                //сумма запланированного времени всех элементов
                int sum = executionTimePlanned;
                foreach (TaskNode node in ChildrenList)
                {
                    sum += node.ExecutionTimePlanned;
                }
                return sum;
            }
            private set
            {
                executionTimePlanned = value;
            }
        }
        private int executionTimePlanned;
        /// <summary>
        /// фактическое время выполнения в часах
        /// </summary>
        public int ExecutionTimeActual 
        {
            get
            {
                //сумма фактического времени всех элементов
                int sum = executionTimeActual;
                foreach (TaskNode node in ChildrenList)
                {
                    sum += node.ExecutionTimeActual;
                }
                return sum;
            }
            private set
            {
                executionTimeActual = value;
            }
        }
        private int executionTimeActual;
        /// <summary>
        /// дата завершения задачи
        /// </summary>
        public DateTime CompleteDate { get; set; }

        public override bool Remove(Node node)
        {
            if (node == this && node.IsTerminal())
            {
                //todo: добавить логику удаления из бд
                return true;
            }
            return false;
        }

        public override void Add(Node node)
        {
            node.Parent = this;
            ChildrenList.Add(node);
        }

        public enum State
        {
            Assigned,
            Executing,
            Suspend,
            Complete
        }
    }
}
