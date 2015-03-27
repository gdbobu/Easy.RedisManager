using Easy.RedisManager.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Easy.Common;
using System.Threading;
using System.Collections.Concurrent;

namespace Easy.RedisManager.Entity.Dictionary
{
    /// <summary>
    /// Redis连接字典表
    /// 存放Redis配置信息
    /// </summary>
    public class RedisConnectionDict:BaseDict
    {
        private int _index = 0;
        /// <summary>
        /// 文档内容
        /// </summary>
        private XElement _configDoc;

        /// <summary>
        /// Redis配置信息字典
        /// </summary>
        private ConcurrentDictionary<int, RedisConnConfig> _redisConnDict;

        public RedisConnectionDict()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileFullPath">配置文件的路径</param>
        public RedisConnectionDict(string fileFullPath)
        {
            _configDoc = XElement.Load(fileFullPath);
            _redisConnDict = new ConcurrentDictionary<int, RedisConnConfig>();
            RedisConnConfig data = null;

            IEnumerator<XElement> enumerator = _configDoc.Elements("RedisConnection").GetEnumerator();
            while (enumerator.MoveNext())
            {
                XElement element = enumerator.Current;

                Interlocked.Increment(ref _index);
                if (_redisConnDict.ContainsKey(_index))
                    continue;

                data = new RedisConnConfig();
                data.Id = _index;
                data.Host = GetAttributeValue(element, "Host");
                data.Name = GetAttributeValue(element, "Name");
                data.Port = GetAttributeValue(element, "Port").ToInt();
                data.Auth = GetAttributeValue(element, "Auth");
                data.ConnectionTimeOut = GetAttributeValue(element, "ConnectionTimeOut").ToInt();
                data.CommandExecutionTimeOut = GetAttributeValue(element, "CommandExecutionTimeOut").ToInt();

                _redisConnDict.TryAdd(_index, data);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileFullPath"></param>
        public void Init(string fileFullPath)
        {
            _configDoc = XElement.Load(fileFullPath);
            _redisConnDict = new ConcurrentDictionary<int, RedisConnConfig>();
            RedisConnConfig config = null;
            _index = 0;

            IEnumerator<XElement> enumerator = _configDoc.Elements("RedisConnection").GetEnumerator();
            while (enumerator.MoveNext())
            {
                XElement element = enumerator.Current;
                Interlocked.Increment(ref _index);

                string host = element.Attribute("Host").Value;
                if (_redisConnDict.ContainsKey(_index))
                    continue;

                config = new RedisConnConfig();
                config.Id = _index;
                config.Host = GetAttributeValue(element, "Host");
                config.Name = GetAttributeValue(element, "Name");
                config.Port = GetAttributeValue(element, "Port").ToInt();
                config.Auth = GetAttributeValue(element, "Auth");
                config.ConnectionTimeOut = GetAttributeValue(element, "ConnectionTimeOut").ToInt();
                config.CommandExecutionTimeOut = GetAttributeValue(element, "CommandExecutionTimeOut").ToInt();

                _redisConnDict.TryAdd(_index, config);
            }
        }

        /// <summary>
        /// 根据ID获取ConnectionConfig
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RedisConnConfig GetConnConfigById(int id)
        {
            RedisConnConfig config = null;
            _redisConnDict.TryGetValue(id, out config);
            return config;
        }
    }
}
