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

        Task EditAsync(TaskNode newTaskNode);

        Task SaveAsync();
        List<TaskNode> Load();

        Task<bool> Remove(int id, List<TaskNode> taskNodeListToRemove = null);

        Task<TaskNode> FindById(int id);
    }
}
