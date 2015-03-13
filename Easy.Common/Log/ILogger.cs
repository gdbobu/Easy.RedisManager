using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Common
{
    /// <summary>
    /// 日志记录器对外接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 输出Debug级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        void Debug(object message);

        /// <summary>
        /// 输出Debug级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        void Debug(object message, Exception ex);

        /// <summary>
        /// 输出Info级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        void Info(object message);

        /// <summary>
        /// 输出Info级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        void Info(object message, Exception ex);

        /// <summary>
        /// 输出Error级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        void Error(object message);

        /// <summary>
        /// 输出Error级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        void Error(object message, Exception ex);

        /// <summary>
        /// 输出Warn级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        void Warn(object message);

        /// <summary>
        /// 输出Warn级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        void Warn(object message, Exception ex);

        /// <summary>
        /// 输出Fatal级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        void Fatal(object message);

        /// <summary>
        /// 输出Fatal级别的日志
        /// </summary>
        /// <param name="message">要输出的日志信息</param>
        /// <param name="ex">要输出的异常信息</param>
        void Fatal(object message, Exception ex);
    }
}
