using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.RedisAccess.Exceptions
{
    /// <summary>
    /// Redis-response exception. Thrown if unable to accept Redis server respose.
    /// </summary>
    public class RedisResponseException
        : RedisException
    {
        public RedisResponseException(string message)
            : base(message)
        {
        }

        public RedisResponseException(string message, string code)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; private set; }
    }
}
