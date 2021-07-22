using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagement.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<TaskNode> TaskNodeList { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //создаем базу данных при первом обращении
            Database.EnsureCreated();   
        }
    }
}
