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
                    if (TaskState == State.Executing && !ChildrenList.Any(node => ((TaskNode)node).TaskState != State.Executing))
                    {
                        //_actualTimeUpdater.Dispose();
                        _taskState = value;
                    }
                    //todo: else exception
                }
                //если задача стояла на паузе, но пазу сняли, то запускаем таймер обратно
                else if (_taskState == State.Suspend && 
                        value != State.Suspend && 
                        value != State.Complete)
                {
                    //_actualTimeUpdater = Task.Run(() => ActualTimeUpdater());
                    _taskState = value;
                }
                else if (value == State.Complete)
                {
                    //если этот объект имеет статус Executing и все его дочерние тоже
                    if (TaskState == State.Executing && !ChildrenList.Any(node => ((TaskNode)node).TaskState != State.Executing))
                    {
                        //_actualTimeUpdater.Dispose();
                        //также завершаем все дочерние задачи
                        foreach (TaskNode taskNode in ChildrenList)
                        {
                            taskNode.TaskState = State.Complete;
                        }
                        _taskState = value;
                    }
                    //todo: else exception
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
                //сумма запланированного времени всех элементов
                int sum = _executionTimePlanned;
                foreach (TaskNode taskNode in ChildrenList)
                {
                    sum += taskNode.ExecutionTimePlanned;
                }
                return sum;
            }
            private set
            {
                _executionTimePlanned = value;
            }
        }
        private int _executionTimePlanned;
        /// <summary>
        /// фактическое время выполнения в часах
        /// </summary>
        public int ExecutionTimeActual 
        {
            get
            {
                //сумма фактического времени всех элементов
                int sum = _executionTimeActual;
                foreach (TaskNode taskNode in ChildrenList)
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

        //private Task _actualTimeUpdater;
        //private int _minutesPassedFromLastHour;

        //без пустого конструктора не хочет собираться бд EF Core,
        //не до конца понимаю почему
        private TaskNode() 
        {
            TaskState = State.Assigned;
            RegisterDate = DateTime.Now;

            ChildrenList = new List<TaskNode>();
        }

        public TaskNode(string _title, string _description,
            string _executors,
            int _executionTimePlanned
            //,List<TaskNode> _childrenList = null,
            //TaskNode _parent = null
            )
        {
            Title = _title;
            Description = _description;
            Executors = _executors;
            ExecutionTimePlanned = _executionTimePlanned;

            TaskState = State.Assigned;
            RegisterDate = DateTime.Now;

            ChildrenList = new List<TaskNode>();

            //запускет обновление фактического времени
            //_actualTimeUpdater = Task.Run(() => ActualTimeUpdater());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns>true если задача успешно удалена</returns>
        public bool Remove(TaskNode node)
        {
            if (ChildrenList.Contains(node) &&
                (node).TaskState == State.Complete)
            {
                //зануляем дочерние задачи
                for (int i = 0; i < node.ChildrenList.Count; i++)
                {
                    ((List<TaskNode>)node.ChildrenList)[i] = null;
                }
                node.ChildrenList = new List<TaskNode>();

                //удаляем саму задачу
                ChildrenList.Remove(node);

                return true;
            }
            else return false;
        }

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

        //private async void ActualTimeUpdater()
        //{
        //    while (true)
        //    {
        //        //ждём 60 минут и 5 мс (для гарантии)
        //        Thread.Sleep(60000);

        //        _minutesPassedFromLastHour++;

        //        if (_minutesPassedFromLastHour >= 60)
        //        {
        //            _executionTimeActual++;
        //            _minutesPassedFromLastHour = 0;
        //        }
        //    }
        //}

       

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
