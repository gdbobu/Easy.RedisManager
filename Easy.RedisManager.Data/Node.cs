using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Data
{
    /// <summary>
    /// The define of TreeNode Data
    /// </summary>
    public class Node : IComparable, IComparable<Node>
    {
        public int Id { get; set; }
        public int Db { get; set; }

        public string Key { get; set; }

        public string DisplayText { get; set; }

        public Enum
        public int CompareTo(object obj)
        {
            Node node = obj as Node;
            if (node == null)
                return 0;

            return 1;
        }

        public int CompareTo(Node other)
        {
            if (other == null)
                return 0;

            return 1;
        }
    }
}
