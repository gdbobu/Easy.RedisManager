using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Common.Enum
{
    /// <summary>
    /// The define type of RedisVersion
    /// </summary>
    public enum EnumRedisVersion
    {
        /// <summary>
        /// Redis 1.0
        /// </summary>
        [Description("Redis 1.0")]
        REDIS_1_0 = 1,
        /// <summary>
        /// Redis 1.2
        /// </summary>
        [Description("Redis 1.2")]
        REDIS_1_2 = 2,
        /// <summary>
        /// Redis 2.0
        /// </summary>
        [Description("Redis 2.0")]
        REDIS_2_0 = 3,
        /// <summary>
        /// Redis 2.2
        /// </summary>
        [Description("Redis 2.2")]
        REDIS_2_2 = 4,
        /// <summary>
        /// Redis 2.4
        /// </summary>
        [Description("Redis 2.4")]
        REDIS_2_4 = 5,
        /// <summary>
        /// Redis 2.6
        /// </summary>
        [Description("Redis 2.6")]
        REDIS_2_6 = 6,
        /// <summary>
        /// Redis 2.8
        /// </summary>
        [Description("Redis 2.8")]
        REDIS_2_8 = 7, 
        /// <summary>
        /// Redis 3.0
        /// </summary>
        [Description("Redis 3.0")]
        REDIS_3_0 = 8
    }
}
