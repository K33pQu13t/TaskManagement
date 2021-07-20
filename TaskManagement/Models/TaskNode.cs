using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Services;

namespace TaskManagement.Models
{
    [Serializable]
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
            //_actualTimeUpdater = Task.Run(() => ActualTimeUpdater());
        }

        //private Task _actualTimeUpdater;
        //private int _minutesPassedFromLastHour;

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
                        //_actualTimeUpdater.Dispose();
                        _taskState = value;
                    }
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
                    List<TaskNode> _childrenList = CastListNodeToListTaskNode(ChildrenList);

                    //если этот объект имеет статус Executing и все его дочерние тоже
                    if (TaskState == State.Executing && !_childrenList.Any(node => node.TaskState != State.Executing))
                    {
                        //_actualTimeUpdater.Dispose();
                        //также завершаем все дочерние задачи
                        foreach (TaskNode taskNode in ChildrenList)
                        {
                            taskNode.TaskState = State.Complete;
                        }
                        _taskState = value;
                    }   
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns>true если задача успешно удалена</returns>
        public override bool Remove(Node node)
        {
            if (ChildrenList.Contains(node) &&
                ((TaskNode)node).TaskState == State.Complete)
            {
                //зануляем дочерние задачи
                for (int i = 0; i < node.ChildrenList.Count; i++)
                {
                    node.ChildrenList[i] = null;
                }
                node.ChildrenList = new List<Node>();

                //удаляем саму задачу
                ChildrenList.Remove((TaskNode)node);

                return true;
            }
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns>возвращает добавляемый элемент с изменённым родителем</returns>
        public override void Add(Node node)
        {
            if (this != ((TaskNode)node))
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
            CompleteDate = DateTime.Now;
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
