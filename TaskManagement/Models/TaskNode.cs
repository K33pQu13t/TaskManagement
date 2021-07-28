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
                    if (CanBeSuspended())
                        _taskState = value;
                    //todo: else exception
                }
                else if (value == State.Complete)
                {
                    //если можем завершить, то ставить дату завершения
                    CompleteTaskAndChildrenIfPossible();
                }
                else if (value == State.Executing)
                { 
                    if (CanBeExecuted())
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
        /// добавляет подзадачу к задаче
        /// </summary>
        /// <param name="node">подзадача</param>
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
        /// Переводит объект и все его дочерние объекты в статус Complete, если это возможно
        /// </summary>
        /// <returns>true если задача перешла в Complete</returns>
        private bool CompleteTaskAndChildrenIfPossible()
        {
            if (CanBeCompleted())
            {
                var allSubtaskList = _GetAllDemensions(this);
                foreach (var node in allSubtaskList)
                {
                    node._taskState = State.Complete;
                    node.CompleteDate = DateTime.Now;
                } 
                return true;
            }
            return false;
        }

        /// <summary>
        /// получить все подзадачи во всех измерениях
        /// </summary>
        /// <returns>список, в который входит эта задача и все подзадачи всех подзадач</returns>
        public List<TaskNode> GetAllDemensions()
        {
            return _GetAllDemensions(this);
        }

        //этот метод нужен чтобы скрыть его извне, чтоб обращались через обычный GetAllDemensions
        /// <summary>
        /// получить задачу + все подзадачи всех подзадач
        /// </summary>
        /// <param name="taskNode"></param>
        /// <param name="taskNodeListAllDemensions"></param>
        /// <returns>список задач, который состоит из исходной задачи и всех её подзадач во всех измерениях</returns>
        private List<TaskNode> _GetAllDemensions(TaskNode taskNode, List<TaskNode> taskNodeListAllDemensions = null)
        {
            if (taskNodeListAllDemensions == null)
                taskNodeListAllDemensions = new List<TaskNode>();

            taskNodeListAllDemensions.Add(taskNode);

            foreach (TaskNode node in taskNode.ChildrenList)
            {
                _GetAllDemensions(node, taskNodeListAllDemensions);
            }
           
            return taskNodeListAllDemensions;
        }

        /// <summary>
        /// true если задача может перейти в Completed
        /// </summary>
        /// <returns></returns>
        public bool CanBeCompleted()
        {
            return _GetAllDemensions(this)
                .All(node => node.TaskState == State.Complete || node.TaskState == State.Executing);
        }

        /// <summary>
        /// true если задача может перейти в Executed
        /// </summary>
        /// <returns></returns>
        public bool CanBeExecuted()
        {
            return this.TaskState == State.Assigned || this.TaskState == State.Suspend;
        }

        /// <summary>
        /// true если задача может перейти в Suspend
        /// </summary>
        /// <returns></returns>
        public bool CanBeSuspended()
        {
            return this.TaskState == State.Executing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true если задача Complete</returns>
        public bool IsCompleted()
        {
            return TaskState == State.Complete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true если у задачи есть подзадачи</returns>
        public bool IsHavingChildren()
        {
            return ChildrenList.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true если задачу можно удалить</returns>
        public bool CanBeDeleted()
        {
            //удалить можно только терминальную задачу
            //false если хоть одна задача не Complete
            return _GetAllDemensions(this)
                .All(node => node.TaskState == State.Complete);
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

        /// <summary>
        /// добавляет задаче фактические часы выполнения
        /// </summary>
        /// <param name="hours">сколько часов добавить</param>
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
