using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.ViewModels
{
    public class TaskNodeAddTaskViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(64)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int ParentId { get; set; }
        public bool HasChildren { get; set; }
        [Required]
        public string Executors { get; set; }
        public TaskNode.State TaskState { get; set; }
        public DateTime RegisterDate { get; set; }
        [Required]
        public int ExecutionTimePlanned { get; set; }
        public int ExecutionTimePlannedThis { get; set; }
        public int ExecutionTimeActual { get; set; }
        public int ExecutionTimeActualThis { get; set; }

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
