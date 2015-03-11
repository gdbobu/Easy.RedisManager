using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.RedisManager.Common.Log
{
    /// <summary>
    /// Redis Log
    /// </summary>
    public class RedisLog:ILogger
    {
        private ILog m_Logger;//日志记录器

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loggerName">记录器对象的名称</param>
        public RedisLog(string loggerName)
        {
            m_Logger = LogManager.GetLogger(loggerName);
        }

        /// <summary>
        /// 输出Debug级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        public void Debug(object message)
        {
            m_Logger.Debug(message);
        }

        /// <summary>
        /// 输出Debug级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        public void Debug(object message, Exception ex)
        {
            m_Logger.Debug(message,ex);
        }

        /// <summary>
        /// 输出Info级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        public void Info(object message)
        {
            m_Logger.Info(message);
        }

        /// <summary>
        /// 输出Info级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        public void Info(object message, Exception ex)
        {
            m_Logger.Info(message,ex);
        }

        /// <summary>
        /// 输出Error级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        public void Error(object message)
        {
            m_Logger.Error(message);
        }

        /// <summary>
        /// 输出Error级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        public void Error(object message, Exception ex)
        {
            m_Logger.Error(message,ex);
        }

        /// <summary>
        /// 输出Warn级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        public void Warn(object message)
        {
            m_Logger.Warn(message);
        }

        /// <summary>
        /// 输出Warn级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        public void Warn(object message, Exception ex)
        {
            m_Logger.Warn(message,ex);
        }

        /// <summary>
        /// 输出Fatal级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        public void Fatal(object message)
        {
            m_Logger.Fatal(message);
        }

        /// <summary>
        /// 输出Fatal级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        public void Fatal(object message, Exception ex)
        {
            m_Logger.Fatal(message,ex);
        }
    }
}
