using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    [Serializable]
    public class MockTaskRepository : ITaskRepository
    {
        List<TaskNode> _taskNodeList;
        string path;

        public MockTaskRepository(string _path)
        {
            path = _path;
            Load();
        }

        public void AddTask(TaskNode taskNode)
        {
            _taskNodeList.Add(taskNode);
            Save();
        }

        public void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path.Combine(path, "tasks.dat"), FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, this);
            }
        }

        public void Load()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string pathToData = Path.Combine(path, "tasks.dat");
            if (File.Exists(pathToData))
            {
                using (FileStream fs = new FileStream(pathToData, FileMode.OpenOrCreate))
                {
                    MockTaskRepository rep = (MockTaskRepository)formatter.Deserialize(fs);
                    _taskNodeList = rep._taskNodeList;
                }
            }
            else
            {
                _taskNodeList = new List<TaskNode>();
            }
        }
    }
}
