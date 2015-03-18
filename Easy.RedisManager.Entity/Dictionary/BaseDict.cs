using Easy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Easy.RedisManager.Entity.Dictionary
{
    /// <summary>
    /// 基础字典类
    /// </summary>
    public class BaseDict
    {
		// 日志记录
		protected ILogger _logger;

		/// <summary>
		/// 初始化
		/// </summary>
        public BaseDict()
		{
            _logger = LogFactory.CreateLogger(this.GetType());
		}

        /// <summary>
        /// 得到属性的值
        /// </summary>
        /// <param name="xElement">节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>属性值</returns>
        public string GetAttributeValue(XElement xElement, string attributeName)
        {
            if (xElement == null)
                return string.Empty;

            XAttribute xAttribute = xElement.Attribute(attributeName);
            if (xAttribute == null)
                return string.Empty;

            return xAttribute.Value;
        }
    }
}
