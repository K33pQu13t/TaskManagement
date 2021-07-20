using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagement.Services
{
    [Serializable]
    abstract public class Node
    {
        /// <summary>
        /// наименование компонента
        /// </summary>
        public string Title { get; set; }

        public Node Parent { get; set; }
        public List<Node> ChildrenList { get; set; }
        public Node(string _title, List<Node> _childrenList, Node _parent)
        {
            Title = _title;
            ChildrenList = _childrenList != null 
                ? _childrenList 
                : new List<Node>();

            Parent = _parent;
        }

        public abstract bool Remove(Node node);
        public abstract void Add(Node node);

    }
}
