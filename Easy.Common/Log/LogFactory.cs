using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common
{
    /// <summary>
    /// 日志工厂类，管理所有的ILogger对象
    /// </summary>
    public class LogFactory
    {
        /// <summary>
        /// 根据字符串创建日志记录器对象
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns>记录器接口实例</returns>
        public static ILogger CreateLogger(string loggerName)
        {
            return new Log4Log(loggerName);
        }

        /// <summary>
        /// 根据类型的名称创建日志记录器对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>记录器接口实例</returns>
        public static ILogger CreateLogger(Type type)
        {
            return new Log4Log(type.ToString());
        }
    }
}
