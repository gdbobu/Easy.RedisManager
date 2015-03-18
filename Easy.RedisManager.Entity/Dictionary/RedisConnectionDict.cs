using Easy.RedisManager.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Easy.Common;

namespace Easy.RedisManager.Entity.Dictionary
{
    /// <summary>
    /// Redis连接字典表
    /// 存放Redis配置信息
    /// </summary>
    public class RedisConnectionDict:BaseDict
    {
        /// <summary>
        /// 文档内容
        /// </summary>
        public XElement ConfigDoc { get; private set; }

        /// <summary>
        /// Redis配置信息字典
        /// </summary>
        public Dictionary<string, RedisConnectionConfig> RedisConnDict { get; set; }

        public RedisConnectionDict()
        {
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileFullPath">配置文件的路径</param>
        public RedisConnectionDict(string fileFullPath)
        {
            ConfigDoc = XElement.Load(fileFullPath);
            RedisConnDict = new Dictionary<string, RedisConnectionConfig>();
            RedisConnectionConfig data = null;

            IEnumerator<XElement> enumerator = ConfigDoc.Elements("RedisConnection").GetEnumerator();
            while (enumerator.MoveNext())
            {
                XElement element = enumerator.Current;
                string host = element.Attribute("Host").Value;
                if (RedisConnDict.ContainsKey(host))
                    continue;

                data = new RedisConnectionConfig();
                RedisConnDict.Add(host, data);
                data.Host = host;
                data.Name = GetAttributeValue(element, "Name");
                data.Port = GetAttributeValue(element, "Port").ToInt();
                data.Auth = GetAttributeValue(element, "Auth");
                data.ConnectionTimeOut = GetAttributeValue(element, "ConnectionTimeOut").ToInt();
                data.CommandExecutionTimeOut = GetAttributeValue(element, "CommandExecutionTimeOut").ToInt();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileFullPath"></param>
        public void Init(string fileFullPath)
        {
            ConfigDoc = XElement.Load(fileFullPath);
            RedisConnDict = new Dictionary<string, RedisConnectionConfig>();
            RedisConnectionConfig config = null;

            IEnumerator<XElement> enumerator = ConfigDoc.Elements("RedisConnection").GetEnumerator();
            while (enumerator.MoveNext())
            {
                XElement element = enumerator.Current;
                string host = element.Attribute("Host").Value;
                if (RedisConnDict.ContainsKey(host))
                    continue;

                config = new RedisConnectionConfig();
                RedisConnDict.Add(host, config);
                config.Host = host;
                config.Name = GetAttributeValue(element, "Name");
                config.Port = GetAttributeValue(element, "Port").ToInt();
                config.Auth = GetAttributeValue(element, "Auth");
                config.ConnectionTimeOut = GetAttributeValue(element, "ConnectionTimeOut").ToInt();
                config.CommandExecutionTimeOut = GetAttributeValue(element, "CommandExecutionTimeOut").ToInt();
            }

        }

    }
}
