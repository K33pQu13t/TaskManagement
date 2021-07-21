using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //TaskNodeRepository repository = new TaskNodeRepository();
            //TaskNode taskNode = new TaskNode("������1", "��������", "����, ����", 2);
            //TaskNode taskNode2 = new TaskNode("���������1", "��������", "����, ����", 1);
            //taskNode.AddSubtask(taskNode2);
            //repository.AddTaskAsync(taskNode);
            //repository.AddTaskAsync(taskNode2);
            //List<TaskNode> taskNodes = repository.Load();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
