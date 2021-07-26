using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Services;

namespace TaskManagement.Models
{
    [Serializable]
    public class TaskNode
    {
        /// <summary>
        /// уникальный идентификатор задачи
        /// </summary>
        public int Id { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// описание задачи
        /// </summary>
        public string Description { get; set; }

        public TaskNode Parent { get; set; }
        public ICollection<TaskNode> ChildrenList { get; set; }

        /// <summary>
        /// список исполнителей
        /// </summary>
        public string Executors { get; set; }

        /// <summary>
        /// статус задачи
        /// </summary>
        public State TaskState 
        {
            get
            {
                return _taskState;
            }
            private set
            {
                //нельзя изменить статус завершённой задачи
                if (_taskState == State.Complete)
                    return;
                else if (value == State.Suspend)
                {
                    //если эта задача выполняется и нет ни одной задачи среди её дочерних, которая была бы не на паузе
                    if (TaskState == State.Executing)
                    {
                        _taskState = value;
                    }
                    //todo: else exception
                }
                else if (value == State.Complete && CompleteChildrenIfPossible(this))
                {
                    _taskState = value;
                }
                else
                {
                    _taskState = value;
                }
            }
        }
        private State _taskState;

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
                //сумма планового времени всех элементов
                int sum = _executionTimePlanned;
                foreach(TaskNode taskNode in this.ChildrenList)
                {
                    sum += taskNode.ExecutionTimePlanned;
                }
                return sum;
            }
            set
            {
                _executionTimePlanned = value;
            }
        }
        private int _executionTimePlanned;
        /// <summary>
        /// фактическое трудоёмкость задачи в часах
        /// </summary>
        public int ExecutionTimeActual 
        {
            get
            {
                //сумма фактического времени всех элементов
                int sum = _executionTimeActual;
                foreach (TaskNode taskNode in this.ChildrenList)
                {
                    sum += taskNode.ExecutionTimeActual;
                }
                return sum;
            }
            private set
            {
                _executionTimeActual = value;
            }
        }
        private int _executionTimeActual;

        /// <summary>
        /// дата завершения задачи
        /// </summary>
        public DateTime CompleteDate { get; private set; }

        public TaskNode() 
        {
            TaskState = State.Assigned;
            RegisterDate = DateTime.Now;

            ChildrenList = new List<TaskNode>();
        }

        public TaskNode(string _title, string _description,
            string _executors,
            int _executionTimePlanned)
        {
            Title = _title;
            Description = _description;
            Executors = _executors;
            ExecutionTimePlanned = _executionTimePlanned;

            TaskState = State.Assigned;
            RegisterDate = DateTime.Now;

            ChildrenList = new List<TaskNode>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns>true если задача успешно удалена</returns>
        //public bool Remove(TaskNode node)
        //{
        //    if (ChildrenList.Contains(node))
        //    {
        //        //зануляем дочерние задачи
        //        for (int i = 0; i < node.ChildrenList.Count; i++)
        //        {
        //            //рекурсивно вызываем этот метод на каждом дочернем объекте
        //            node.Remove(((List<TaskNode>)node.ChildrenList)[i]);
        //            node = null;
        //        }
        //        node.ChildrenList = new List<TaskNode>();

        //        ((List<TaskNode>)ChildrenList).RemoveAll(node => node == null);

        //        return true;
        //    }
        //    else return false;
        //}

        /// <summary>
        /// добавляет подзадачу к задаче
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public void AddSubtask(TaskNode node)
        {
            if (this != node)
            {
                node.Parent = this;
                ChildrenList.Add(node);
            }
        }

        /// <summary>
        /// Переводит все дочерние объекты в статус Complete, если это возможно
        /// </summary>
        /// <param name="taskNode"></param>
        /// <param name="taskNodeCommonChildren"></param>
        /// <returns>true если эту задачу можно перевести в состояние Complete (возможно только если все дочерние элементы в состоянии Executing)</returns>
        static public bool CompleteChildrenIfPossible(TaskNode taskNode, TaskNode taskNodeStart = null, List<TaskNode> taskNodeCommonChildren = null)
        {
            if (taskNodeStart == null)
                taskNodeStart = taskNode;

            if (taskNodeCommonChildren == null)
                taskNodeCommonChildren = new List<TaskNode>();


            //если хотя бы один не Executing или Complete, то сразу ясно что не получится
            if (taskNode.TaskState != State.Executing &&
                taskNode.TaskState != State.Complete)
                return false;

            //тот объект который вызвали самым первым не добавляю, он сам после вызова в сеттере поменяет состояние
            if (taskNode != taskNodeStart)
                taskNodeCommonChildren.Add(taskNode);

            foreach(TaskNode node in taskNode.ChildrenList)
            {
                if (!CompleteChildrenIfPossible(node, taskNodeStart, taskNodeCommonChildren))
                    return false;
            }

            foreach(TaskNode node in taskNodeCommonChildren)
            {
                node.Complete();
            }
            return true;
        }

        /// <summary>
        /// Получить плановую трудоёмкость конкретно этой задачи
        /// </summary>
        /// <returns>запланированное время на выполнения для этой задачи (без учёта подзадач)</returns>
        public int GetThisExecutionTimePlanned()
        {
            return _executionTimePlanned;
        }

        /// <summary>
        /// Получить фактическую трудоёмкость конкретно этой задачи
        /// </summary>
        /// <returns>фактическое время на выполнения для этой задачи (без учёта подзадач)</returns>
        public int GetThisExecutionTimeActual()
        {
            return _executionTimeActual;
        }

        public void AddExecutionTimeActual(int hours)
        {
            _executionTimeActual += hours;
        }

        /// <summary>
        /// перевести задачу в статус "выполняется"
        /// </summary>
        public void Execute()
        {
            TaskState = State.Executing;
        }

        /// <summary>
        /// перевести задачу в статус "приостановлена"
        /// </summary>
        public void Suspend()
        {
            TaskState = State.Suspend;
        }

        /// <summary>
        /// перевести задачу в статус "завершена"
        /// </summary>
        public void Complete()
        {
            TaskState = State.Complete;
            CompleteDate = DateTime.Now;
        }

        public enum State
        {
            /// <summary>
            /// задача назначена
            /// </summary>
            Assigned,
            /// <summary>
            /// задача выполняется
            /// </summary>
            Executing,
            /// <summary>
            /// задача приостановлена
            /// </summary>
            Suspend,
            /// <summary>
            /// задача завершена
            /// </summary>
            Complete
        }
    }
}
