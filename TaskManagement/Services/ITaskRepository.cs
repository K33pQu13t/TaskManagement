using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public interface ITaskRepository
    {
        void AddTask(TaskNode taskNode);

        void Save();
        void Load();
    }
}
