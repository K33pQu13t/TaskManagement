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
            TaskNode taskNode = new TaskNode("������1", "��������", "����, ����", 2);
            TaskNode taskNode2 = new TaskNode("���������1", "��������", "����, ����", 1);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("���������2", "��������", "����, ����", 1);
            taskNode.AddSubtask(taskNode3);

            taskNode.Execute();
            taskNode2.Execute();
            //� taskNode3 �������� Assigned
            taskNode.Complete();

            //�� ���� ������ �� ������ ����� Complete, ������ ��� taskNode3 �� ����� ����� Complete
            //(������ ������� �� Assigned � Complete, ������ �� Executing)
            Assert.AreNotEqual(taskNode.TaskState == TaskNode.State.Complete &&
                            taskNode2.TaskState == TaskNode.State.Complete &&
                            taskNode3.TaskState == TaskNode.State.Complete, 
                            true);
        }

        [TestMethod]
        public void CompleteTestSuccess()
        {
            TaskNode taskNode = new TaskNode("������1", "��������", "����, ����", 2);
            TaskNode taskNode2 = new TaskNode("���������1", "��������", "����, ����", 1);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("���������2", "��������", "����, ����", 1);
            taskNode.AddSubtask(taskNode3);

            taskNode.Execute();
            taskNode2.Execute();
            taskNode3.Execute();
            taskNode.Complete();

            //��������� ���� ������, ������ ����������� ��� ���������
            Assert.AreEqual(taskNode.TaskState == TaskNode.State.Complete &&
                            taskNode2.TaskState == TaskNode.State.Complete &&
                            taskNode3.TaskState == TaskNode.State.Complete,
                            true);
        }

        [TestMethod]
        public void SuspendTestUnsuccess()
        {
            TaskNode taskNode = new TaskNode("������1", "��������", "����, ����", 2);

            taskNode.Suspend();

            //������ ������� ������ Suspend, ���� ��� �� Executing
            Assert.AreNotEqual(taskNode.TaskState == TaskNode.State.Suspend, true);
        }

        [TestMethod]
        public void SuspendTestSuccess()
        {
            TaskNode taskNode = new TaskNode("������1", "��������", "����, ����", 2);

            //������� ������ �������� � Executing, ������ � Suspend ����� ������� ��� �������
            taskNode.Execute();
            taskNode.Suspend();

            Assert.AreEqual(taskNode.TaskState == TaskNode.State.Suspend, true);
        }
    }
}
