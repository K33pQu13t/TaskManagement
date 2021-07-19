using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            //запускет обновление фактического времени
            actualTimeUpdater = Task.Run(() => ActualTimeUpdater());
        }

        private Task actualTimeUpdater;
        private int minutesPassedFromLastHour;

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
                    List<TaskNode> _childrenList = CastListNodeToListTaskNode(ChildrenList);
                    if (TaskState == State.Executing && !_childrenList.Any(node => node.TaskState != State.Executing))
                    {
                        actualTimeUpdater.Dispose();
                        _taskState = value;
                    }
                }
                //если задача стояла на паузе, но пазу сняли, то запускаем таймер обратно
                else if (_taskState == State.Suspend && 
                        value != State.Suspend && 
                        value != State.Complete)
                {
                    actualTimeUpdater = Task.Run(() => ActualTimeUpdater());
                    _taskState = value;
                }
                else if (value == State.Complete)
                {
                    List<TaskNode> _childrenList = CastListNodeToListTaskNode(ChildrenList);

                    //если этот объект имеет статус Executing и все его дочерние тоже
                    if (TaskState == State.Executing && !_childrenList.Any(node => node.TaskState != State.Executing))
                    {
                        actualTimeUpdater.Dispose();
                        //также завершаем все дочерние задачи
                        foreach (TaskNode node in ChildrenList)
                        {
                            node.TaskState = State.Complete;
                        }
                        _taskState = value;
                    }   
                }
                //todo: мне кажется, этот else никогда не сработает, потому что верхние условия описывают все сценарии,
                //но пусть будет пока
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
            if (node == this && this.TaskState == State.Complete)
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

        private async void ActualTimeUpdater()
        {
            while (true)
            {
                //ждём 60 минут и 5 мс (для гарантии)
                Thread.Sleep(60000);

                minutesPassedFromLastHour++;

                if (minutesPassedFromLastHour >= 60)
                {
                    executionTimeActual++;
                    minutesPassedFromLastHour = 0;
                }
            }
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

        //todo: мб можно как-то красивее привести список объектов типа Node к списку объектов типа TaskNode
        /// <summary>
        /// преобразует List<Node> в List<TaskNode>
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private List<TaskNode> CastListNodeToListTaskNode(List<Node> nodeList)
        {
            List<TaskNode> taskNodeList = new List<TaskNode>();
            foreach (TaskNode taskNode in nodeList)
            {
                taskNodeList.Add(taskNode);
            }
            return taskNodeList;
        }
    }
}
