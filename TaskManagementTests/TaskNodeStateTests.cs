using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            TaskNode taskNode3 = new TaskNode("���������1 ���������1", "��������", "����, ����", 1);
            taskNode2.AddSubtask(taskNode3);
            TaskNode taskNode4 = new TaskNode("���������1 ���������2", "��������", "����, ����", 1);
            taskNode2.AddSubtask(taskNode4);
            TaskNode taskNode5 = new TaskNode("���������1 ���������1 ���������2", "��������", "����, ����", 1);
            taskNode4.AddSubtask(taskNode5);
            TaskNode taskNode6 = new TaskNode("���������2", "��������", "����, ����", 1);
            taskNode.AddSubtask(taskNode6);
            //taskNode
            //--taskNode2
            //----taskNode3
            //----taskNode4
            //------taskNode5
            //--taskNode6

            taskNode.Execute();
            taskNode2.Execute();
            taskNode4.Execute();
            taskNode6.Execute();
            //� taskNode3 � taskNode5 �������� Assigned
            taskNode.Complete();

            //�� ���� ������ �� ������ ����� Complete, ������ ��� taskNode3 �� ����� ����� Complete
            //(������ ������� �� Assigned � Complete, ������ �� Executing)
            Assert.AreNotEqual(taskNode.TaskState == TaskNode.State.Complete &&
                            taskNode2.TaskState == TaskNode.State.Complete &&
                            taskNode3.TaskState == TaskNode.State.Complete &&
                            taskNode4.TaskState == TaskNode.State.Complete &&
                            taskNode5.TaskState == TaskNode.State.Complete &&
                            taskNode6.TaskState == TaskNode.State.Complete, 
                            true);
        }

        [TestMethod]
        public void CompleteTestSuccess()
        {
            TaskNode taskNode = new TaskNode("������1", "��������", "����, ����", 2);
            TaskNode taskNode2 = new TaskNode("���������1", "��������", "����, ����", 1);
            taskNode.AddSubtask(taskNode2);
            TaskNode taskNode3 = new TaskNode("���������1 ���������1", "��������", "����, ����", 1);
            taskNode2.AddSubtask(taskNode3);
            TaskNode taskNode4 = new TaskNode("���������1 ���������2", "��������", "����, ����", 1);
            taskNode2.AddSubtask(taskNode4);
            TaskNode taskNode5 = new TaskNode("���������1 ���������1 ���������2", "��������", "����, ����", 1);
            taskNode4.AddSubtask(taskNode5);
            TaskNode taskNode6 = new TaskNode("���������2", "��������", "����, ����", 1);
            taskNode.AddSubtask(taskNode6);
            //taskNode
            //--taskNode2
            //----taskNode3
            //----taskNode4
            //------taskNode5
            //--taskNode6

            taskNode.Execute();
            taskNode2.Execute();
            taskNode3.Execute();
            taskNode4.Execute();
            taskNode5.Execute();
            taskNode6.Execute();

            taskNode.Complete();

            //��� ������ ����� Complete
            Assert.AreEqual(taskNode.TaskState == TaskNode.State.Complete &&
                            taskNode2.TaskState == TaskNode.State.Complete &&
                            taskNode3.TaskState == TaskNode.State.Complete &&
                            taskNode4.TaskState == TaskNode.State.Complete &&
                            taskNode5.TaskState == TaskNode.State.Complete &&
                            taskNode6.TaskState == TaskNode.State.Complete,
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
