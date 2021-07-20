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
            //MockTaskRepository repository = new MockTaskRepository(@"C:\Users\sisurity\Desktop");
            //TaskNode taskNode = new TaskNode("задача1", "описание", new List<string> { "Вася", "Петя" }, 2);
            //repository.AddTask(taskNode);

            //TaskNode taskNode2 = new TaskNode("подзадача1", "описание", new List<string> { "Вася", "Петя" }, 1);
            //taskNode.Add(taskNode2);
            //repository.AddTask(taskNode2);

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
