using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Helpers
{
    public class TaskNodeExecution
    {
        public TaskNode Node { get; set; }
        public int Milliseconds { get; set; }
    }
}
