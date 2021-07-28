using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Helpers
{
    /// <summary>
    /// вспомогательный класс, который содержит ссылку на объект класса TaskNode, 
    /// и поле Milliseconds, которое содержит время в миллисекундах,
    /// которое объёкт провёл в состоянии Executing
    /// </summary>
    public class TaskNodeExecution
    {
        public TaskNode Node { get; set; }
        /// <summary>
        /// сколько миллисекунд объект в состоянии Executing
        /// </summary>
        public int Milliseconds { get; set; }
    }
}
