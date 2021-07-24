using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public interface ITaskNodeRepository
    {
        Task AddTaskAsync(TaskNode taskNode);

        Task SaveAsync();
        List<TaskNode> Load();

        Task EditAsync(TaskNode newTaskNode);

        TaskNode FindById(int id);
    }
}
