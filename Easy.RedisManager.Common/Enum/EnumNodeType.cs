using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Common.Enum
{
    /// <summary>
    /// The Type of the tree node
    /// </summary>
    public enum EnumNodeType:byte
    {
        /// <summary>
        /// Root
        /// </summary>
        [Description("Root")]
        Root,
        /// <summary>
        /// Server
        /// </summary>
        [Description("Server")]
        Server,
        /// <summary>
        /// DataBase
        /// </summary>
        [Description("DataBase")]
        DataBase,
        /// <summary>
        /// Container
        /// </summary>
        [Description("Container")]
        Container,
        /// <summary>
        /// String
        /// </summary>
        [Description("String")]
        String,
        /// <summary>
        /// Hash
        /// </summary>
        [Description("Hash")]
        Hash,
        /// <summary>
        /// List
        /// </summary>
        [Description("List")]
        List,
        /// <summary>
        /// Set
        /// </summary>
        [Description("Set")]
        Set,
        /// <summary>
        /// SortedSet
        /// </summary>
        [Description("SortedSet")]
        SortedSet
    }
}
