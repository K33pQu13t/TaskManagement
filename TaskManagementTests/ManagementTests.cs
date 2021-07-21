using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TaskManagement.Models;

namespace TaskManagementTests
{
    [TestClass]
    public class ManagementTests
    {
        [TestMethod]
        public void SubtaskCreation()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);
            TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 1);
            taskNode.AddSubtask(taskNode2);
            //если у второй задачи родитель - это первая задача,
            //и среди наследников у первой задачи есть вторая,
            //то значит связь между ними есть
            Assert.AreEqual(taskNode2.Parent == taskNode && 
                               taskNode.ChildrenList.Contains(taskNode2), 
                               true);
        }

        [TestMethod]
        public void SubtaskRemovingUnsuccess()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);
            TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 1);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("подзадача1 подзадачи 1", "описание", "Вася, Петя", 1);
            taskNode2.AddSubtask(taskNode3);

            //нельзя удалить незавёршенную задачу
            taskNode.Remove(taskNode2);

            Assert.AreEqual(taskNode.ChildrenList.Contains(taskNode2), true);
        }

        [TestMethod]
        public void SubtaskRemovingSuccess()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);
            TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 1);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("подзадача1 подзадачи 1", "описание", "Вася, Петя", 1);
            taskNode2.AddSubtask(taskNode3);

            taskNode2.Execute();
            taskNode3.Execute();
            taskNode2.Complete();

            taskNode.Remove(taskNode2);

            Assert.AreEqual(taskNode.ChildrenList.Count, 0);
        }
    }
}
