using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TaskManagement.Models;

namespace TaskManagementTests
{
    [TestClass]
    public class TaskNodeStateTests
    {
        [TestMethod]
        public void CompleteTestUnsuccess()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);
            TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 1);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("подзадача2", "описание", "Вася, Петя", 1);
            taskNode.AddSubtask(taskNode3);

            taskNode.Execute();
            taskNode2.Execute();
            //а taskNode3 осталась Assigned
            taskNode.Complete();

            //ни одна задача не должна стать Complete, потому что taskNode3 не может стать Complete
            //(нельзя перейти от Assigned к Complete, только от Executing)
            Assert.AreNotEqual(taskNode.TaskState == TaskNode.State.Complete &&
                            taskNode2.TaskState == TaskNode.State.Complete &&
                            taskNode3.TaskState == TaskNode.State.Complete, 
                            true);
        }

        [TestMethod]
        public void CompleteTestSuccess()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);
            TaskNode taskNode2 = new TaskNode("подзадача1", "описание", "Вася, Петя", 1);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("подзадача2", "описание", "Вася, Петя", 1);
            taskNode.AddSubtask(taskNode3);

            taskNode.Execute();
            taskNode2.Execute();
            taskNode3.Execute();
            taskNode.Complete();

            //завершаем одну задачу, должны завершиться все подзадачи
            Assert.AreEqual(taskNode.TaskState == TaskNode.State.Complete &&
                            taskNode2.TaskState == TaskNode.State.Complete &&
                            taskNode3.TaskState == TaskNode.State.Complete,
                            true);
        }

        [TestMethod]
        public void SuspendTestUnsuccess()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);

            taskNode.Suspend();

            //нельзя сделать задачи Suspend, если они не Executing
            Assert.AreNotEqual(taskNode.TaskState == TaskNode.State.Suspend, true);
        }

        [TestMethod]
        public void SuspendTestSuccess()
        {
            TaskNode taskNode = new TaskNode("задача1", "описание", "Вася, Петя", 2);

            //сначала задачу перевели в Executing, значит в Suspend можем перейти без проблем
            taskNode.Execute();
            taskNode.Suspend();

            Assert.AreEqual(taskNode.TaskState == TaskNode.State.Suspend, true);
        }
    }
}
