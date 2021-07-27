using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
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
        public void CalculatePlannedTime()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 1);
            TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 4);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("подзадача1 подзадачи1", "описание", "Вася, Петя", 3);
            taskNode2.AddSubtask(taskNode3);
            TaskNode taskNode4 = new TaskNode("подзадача1 подзадачи2", "описание", "Вася, Петя", 2);
            taskNode2.AddSubtask(taskNode4);
            TaskNode taskNode5 = new TaskNode("подзадача1 подзадачи1 подзадачи2", "описание", "Вася, Петя", 5);
            taskNode4.AddSubtask(taskNode5);
            TaskNode taskNode6 = new TaskNode("подзадача2", "описание", "Вася, Петя", 3);
            taskNode.AddSubtask(taskNode6);
            //taskNode
            //--taskNode2
            //----taskNode3
            //----taskNode4
            //------taskNode5
            //--taskNode6

            int timePlanned = taskNode.ExecutionTimePlanned;

            Assert.AreEqual(18, timePlanned);
        }

        [TestMethod]
        public void GetAllDemensions()
        {
          
            TaskNode taskNode = new TaskNode("1", "описание", "Вася, Петя", 1);
            TaskNode taskNode2 = new TaskNode("2", "описание", "Вася, Петя", 4);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("3", "описание", "Вася, Петя", 3);
            taskNode2.AddSubtask(taskNode3);
            TaskNode taskNode4 = new TaskNode("4", "описание", "Вася, Петя", 2);
            taskNode2.AddSubtask(taskNode4);
            TaskNode taskNode5 = new TaskNode("5", "описание", "Вася, Петя", 5);
            taskNode4.AddSubtask(taskNode5);
            TaskNode taskNode6 = new TaskNode("6", "описание", "Вася, Петя", 3);
            taskNode.AddSubtask(taskNode6);
            //taskNode
            //--taskNode2
            //----taskNode3
            //----taskNode4
            //------taskNode5
            //--taskNode6
            List<TaskNode> taskNodeList = new List<TaskNode>()
            {
                taskNode, taskNode2, taskNode3, taskNode4, taskNode5, taskNode6
            };
            //taskNodeList.Sort();
            List<TaskNode> allDemensionsList = taskNode.GetAllDemensions();
            //allDemensionsList.Sort();

            Assert.AreEqual(taskNodeList.SequenceEqual(allDemensionsList), true);
        }

        //[TestMethod]
        //public void SubtaskRemovingUnsuccess()
        //{
        //    TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);
        //    TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 1);
        //    taskNode.AddSubtask(taskNode2);
        //    TaskNode taskNode3 = new TaskNode("подзадача1 подзадачи 1", "описание", "Вася, Петя", 1);
        //    taskNode2.AddSubtask(taskNode3);

        //    //нельзя удалить незавёршенную задачу
        //    taskNode.Remove(taskNode2);

        //    Assert.AreEqual(taskNode.ChildrenList.Contains(taskNode2), true);
        //}

        //[TestMethod]
        //public void SubtaskRemovingSuccess()
        //{
        //    TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);
        //    TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 1);
        //    taskNode.AddSubtask(taskNode2);
        //    TaskNode taskNode3 = new TaskNode("подзадача1 подзадачи 1", "описание", "Вася, Петя", 1);
        //    taskNode2.AddSubtask(taskNode3);

        //    taskNode2.Execute();
        //    taskNode3.Execute();
        //    taskNode2.Complete();

        //    taskNode.Remove(taskNode2);

        //    Assert.AreEqual(taskNode.ChildrenList.Count, 0);
        //}
    }
}
