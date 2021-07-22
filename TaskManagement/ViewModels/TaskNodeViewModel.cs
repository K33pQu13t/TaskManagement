using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.ViewModels
{
    public class TaskNodeListViewModel
    {
        public List<TaskNode> TaskNodeList { get; set; }

        public TaskNodeListViewModel(List<TaskNode> _taskNodeList)
        {
            TaskNodeList = _taskNodeList;
        }

        //public int Id { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        //public TaskNodeViewModel Parent { get; set; }
        //public List<TaskNodeViewModel> Children { get; set; }
        //public string Executors { get; set; }
        //public int State { get; set; }
        //public DateTime RegisterDate { get; set; }
        //public int ExecutionTimePlanned { get; set; }
        //public int ExecutionTimeActual { get; set; }
        //public DateTime CompleteDate { get; set; }

        //public TaskNodeViewModel(TaskNode taskNode)
        //{
        //    if (taskNode == null)
        //        return;
        //    Id = taskNode.Id;
        //    Description = taskNode.Description;
        //    Parent = taskNode.Parent == null 
        //        ? null : new TaskNodeViewModel(taskNode.Parent);
        //    Children = new List<TaskNodeViewModel>();
        //    foreach(TaskNode child in taskNode.ChildrenList)
        //    {
        //        Children.Add(new TaskNodeViewModel(child));
        //    }
        //    Executors = taskNode.Executors;
        //    State = ((int)taskNode.TaskState);
        //    RegisterDate = taskNode.RegisterDate;
        //    ExecutionTimePlanned = taskNode.ExecutionTimePlanned;
        //    ExecutionTimeActual = taskNode.ExecutionTimeActual;
        //    CompleteDate = taskNode.CompleteDate;
        //}
    }
}
