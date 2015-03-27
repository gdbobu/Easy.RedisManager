using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity.Config
{
    /// <summary>
    /// RedisConnectionConfig Define
    /// </summary>
    public class RedisConnConfig
    {
        /// <summary>
        /// Id
        /// </summary>
        [Description("Id")]
        public int Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [Description("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        [Description("Host")]
        public string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        [Description("Port")]
        public int Port { get; set; }
        /// <summary>
        /// Auth
        /// </summary>
        [Description("Auth")]
        public string Auth { get; set; }
        /// <summary>
        /// Connection TimeOut(Second)
        /// </summary>
        [Description("Connection TimeOut(Second)")]
        public int ConnectionTimeOut { get; set; }
        /// <summary>
        /// Command Execution TimeOut(Second)
        /// </summary>
        [Description("Command Execution TimeOut(Second)")]
        public int CommandExecutionTimeOut { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RedisConnConfig()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="auth"></param>
        /// <param name="connectionTimeOut"></param>
        /// <param name="commandExecutionTimeOut"></param>
        public RedisConnConfig(int id, string name, string host,
            int port, string auth, int connectionTimeOut, int commandExecutionTimeOut)
        {
            Id = id;
            Name = name;
            Host = host;
            Port = port;
            Auth = auth;
            ConnectionTimeOut = connectionTimeOut;
            CommandExecutionTimeOut = CommandExecutionTimeOut;
        }
    }
}
