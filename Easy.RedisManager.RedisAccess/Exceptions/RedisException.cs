using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.RedisAccess.Exceptions
{
    /// <summary>
    /// Redis-specific exception. Thrown if unable to connect to Redis server due to socket exception, for example.
    /// </summary>
    public class RedisException
        : Exception
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="message"></param>
        public RedisException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RedisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
