using Microsoft.EntityFrameworkCore;

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
