using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity.Redis
{
    /// <summary>
    /// Redis数据库节点信息
    /// </summary>
    public class RedisDbNodeInfo
    {
        /// <summary>
        /// Host
        /// </summary>
        public string ParentHost { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        public int ParentPort { get; set; }
        /// <summary>
        /// 所属数据库
        /// </summary>
        public long Db { get; set; }
    }
}
