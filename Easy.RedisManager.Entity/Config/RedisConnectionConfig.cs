using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity.Config
{
    /// <summary>
    /// RedisConnectionConfig Define
    /// </summary>
    public class RedisConnectionConfig
    {
        /// <summary>
        /// 是否已经连接
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Auth
        /// </summary>
        public string Auth { get; set; }
        /// <summary>
        /// Connection TimeOut(Second)
        /// </summary>
        public int ConnectionTimeOut { get; set; }
        /// <summary>
        /// Command Execution TimeOut(Second)
        /// </summary>
        public int CommandExecutionTimeOut { get; set; }
    }
}
