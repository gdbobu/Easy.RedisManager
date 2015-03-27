using Easy.RedisManager.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity
{
    /// <summary>
    /// The define of TreeNode Data
    /// </summary>
    public class Node : IComparable, IComparable<Node>
    {
        /// <summary>
        /// ID
        /// </summary>
        [Description("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Db
        /// </summary>
        [Description("Db")]
        public int Db { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        [Description("Key")]
        public string Key { get; set; }
        /// <summary>
        /// DisplayText
        /// </summary>
        [Description("DisplayText")]
        public string DisplayText { get; set; }
        /// <summary>
        /// NodeType
        /// </summary>
        [Description("NodeType")]
        public EnumNodeType NodeType { get; set; }
        /// <summary>
        /// Order
        /// </summary>
        [Description("Order")]
        public EnumOrderType Order { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="displayText"></param>
        /// <param name="nodeType"></param>
        /// <param name="order"></param>
        public Node(int id, int db, string key, 
            string displayText, EnumNodeType nodeType, EnumOrderType order)
        {
            Id = id;
            Db = db;
            Key = key;
            DisplayText = displayText;
            NodeType = nodeType;
            Order = order;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="displayText"></param>
        /// <param name="nodeType"></param>
        public Node(int id, int db, string key,
            string displayText, EnumNodeType nodeType)
            : this(id, db, key, displayText, nodeType, EnumOrderType.Ascend)
        {
        }

        /// <summary>
        /// Override the function of Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Node node = obj as Node;
            if (node == null)
                return false;

            return Id == node.Id
                && Db == node.Db
                && Key == node.Key
                && DisplayText == node.DisplayText
                && NodeType == node.NodeType
                && Order == node.Order;
        }

        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Node node = obj as Node;
            if (node == null)
                return 0;

            return CompareTo(node);
        }

        /// <summary>
        /// CompareTo
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Node other)
        {
            if (other == null)
            {
                if (Order == EnumOrderType.Ascend)
                    return 1;
                return -1;
            }

            int result = Id.CompareTo(other.Id);
            if(result == 0)
            {
                result = Db.CompareTo(other.Db);
                if (result == 0)
                    result = Key.CompareTo(other.Key);
            }

            if (Order == EnumOrderType.Ascend)
                return result;

            return result * -1;
        }
    }
}
