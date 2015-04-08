using Easy.Common;
using Easy.RedisManager.Common.Command;
using Easy.RedisManager.Common.Enum;
using Easy.RedisManager.Entity.Config;
using Easy.RedisManager.Entity.Dictionary;
using Easy.RedisManager.RedisAccess.Communication;
using Easy.RedisManager.RedisAccess.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.RedisAccess
{
    /// <summary>
    /// Redis Client
    /// </summary>
    public class RedisClient:IComparable<RedisClient>,IDisposable
    {
        // 记录日志类
        private static readonly ILogger s_Logger = LogFactory.CreateLogger(typeof(RedisClient));

        public const int MaxRedisDbCount = 1024;
        public const int UnKnown = -1;
        public const long DefaultDb = 0;

        protected RedisConnConfig _config;
        protected RedisSocketClient _socketClient;

        // 信息字典
        private ConcurrentDictionary<string, string> _redisInfo;

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool HadExceptions { get; protected set; }

        /// <summary>
        /// 获取数据库大小
        /// </summary>
        public long DbSize
        {
            get
            {
                return SendExpectLong(RedisCommands.DbSize);
            }

            private set { }
        }

        /// <summary>
        /// 获取数据库上次保存的时间
        /// </summary>
        public DateTime LastSave
        {
            get
            {
                var t = SendExpectLong(RedisCommands.LastSave);
                return t.FromUnixTime();
            }
        }

        /// <summary>
        /// Redis数据库信息字典
        /// </summary>
        public ConcurrentDictionary<string, string> RedisInfo
        {
            get
            {
                if (this._redisInfo == null)
                {
                    var lines = SendExpectString(RedisCommands.Info);
                    this._redisInfo = new ConcurrentDictionary<string, string>();

                    foreach (var line in lines
                        .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var p = line.IndexOf(':');
                        if (p == -1) continue;

                        this._redisInfo.TryAdd(line.Substring(0, p), line.Substring(p + 1));
                    }
                }
                return this._redisInfo;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public RedisClient(RedisConnConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("The Argument config is null.");

            this._config = config;
            this._socketClient = new RedisSocketClient(this._config.Host, this._config.Port,
                this._config.Auth, this._config.ConnectionTimeOut);

            if (this._socketClient == null)
                throw new ArgumentException("The RedisSocketClient is null.");
        }

        protected abstract void SendCommand();
        public abstract EnumRedisVersion GetSupportVersion();

        /// <summary>
        /// 发送命令，返回Long型数据
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected long SendExpectLong(params byte[][] cmdWithBinaryArgs)
        {
            if (!this._socketClient.SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            return ReadLong();
        }

        /// <summary>
        /// 发送命令，返回String
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected string SendExpectString(params byte[][] cmdWithBinaryArgs)
        {
            var bytes = SendExpectData(cmdWithBinaryArgs);
            return bytes.FromUtf8Bytes();
        }

        /// <summary>
        /// 发送命令，返回比特流
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected byte[] SendExpectData(params byte[][] cmdWithBinaryArgs)
        {
            if (!this._socketClient.SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            return ReadData();
        }

        /// <summary>
        /// 获取Redis版本号
        /// </summary>
        /// <returns></returns>
        protected EnumRedisVersion GetRedisVersion()
        {
            string version = "";
            if (!RedisInfo.TryGetValue("redis_version", out version))
                return EnumRedisVersion.REDIS_1_0;

            string[] versionInfos = version.Split(":".ToArray());
            if (versionInfos == null || versionInfos.Length == 0)
                return EnumRedisVersion.REDIS_1_0;

            version = versionInfos[1];

            if (version.StartsWith("3.0"))
                return EnumRedisVersion.REDIS_3_0;
            else if (version.StartsWith("2.8"))
                return EnumRedisVersion.REDIS_2_8;
            else if (version.StartsWith("2.6"))
                return EnumRedisVersion.REDIS_2_6;
            else if (version.StartsWith("2.4"))
                return EnumRedisVersion.REDIS_2_4;
            else if (version.StartsWith("2.2"))
                return EnumRedisVersion.REDIS_2_2;
            else if (version.StartsWith("2.0"))
                return EnumRedisVersion.REDIS_2_0;
            else if (version.StartsWith("1.2"))
                return EnumRedisVersion.REDIS_1_2;
            else
                return EnumRedisVersion.REDIS_1_0;
        }

        public int CompareTo(RedisClient other)
        {
            if (other == null)
                return -1;

            return this.GetSupportVersion().CompareTo(other.GetSupportVersion()) * -1;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读取Long
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            int c = this._socketClient.SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = this._socketClient.ReadLine();

            Log("R: {0}", s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

            if (c == ':' || c == '$')//really strange why ZRANK needs the '$' here
            {
                long i;
                if (long.TryParse(s, out i))
                    return i;
            }

            throw CreateResponseError("Unknown reply on integer response: " + c + s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private byte[] ParseSingleLine(string r)
        {
            Log("R: {0}", r);

            if (r.Length == 0)
                throw CreateResponseError("Zero length response");

            char c = r[0];
            if (c == '-')
                throw CreateResponseError(r.StartsWith("-ERR") ? r.Substring(5) : r.Substring(1));

            if (c == '$')
            {
                if (r == "$-1")
                    return null;
                int count;

                if (Int32.TryParse(r.Substring(1), out count))
                {
                    var retbuf = new byte[count];

                    var offset = 0;
                    while (count > 0)
                    {
                        var readCount = this._socketClient.SafeReadByte(retbuf, offset, count);
                        //var readCount = _buffStream.Read(retbuf, offset, count);
                        if (readCount <= 0)
                            throw CreateResponseError("Unexpected end of Stream");

                        offset += readCount;
                        count -= readCount;
                    }

                    if (this._socketClient.SafeReadByte() != '\r' 
                        || this._socketClient.SafeReadByte() != '\n')
                        throw CreateResponseError("Invalid termination");

                    return retbuf;
                }

                throw CreateResponseError("Invalid length");
            }

            if (c == ':')
            {
                //match the return value
                return r.Substring(1).ToUtf8Bytes();
            }
            throw CreateResponseError("Unexpected reply: " + r);
        }

        /// <summary>
        /// 读取字节流
        /// </summary>
        /// <returns></returns>
        private byte[] ReadData()
        {
            var r = this._socketClient.ReadLine();
            return ParseSingleLine(r);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        private void Log(string fmt, params object[] args)
        {
            s_Logger.Debug(string.Format("{0}", string.Format(fmt, args).Trim()));
        }

        /// <summary>
        /// 生成ResponseError
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private RedisResponseException CreateResponseError(string error)
        {
            HadExceptions = true;
            var throwEx = new RedisResponseException(
                string.Format("{0}, Port: {1}, LastCommand: {2}", error, this._config.Port, this._socketClient.LastCommand));
            s_Logger.Error(throwEx.Message);
            throw throwEx;
        }

        /// <summary>
        /// 生成连接错误
        /// </summary>
        /// <returns></returns>
        private RedisException CreateConnectionError()
        {
            HadExceptions = true;
            var throwEx = new RedisException(
                string.Format("Unable to Connect: Port: {0}", this._config.Port), this._socketClient.LastSocketException);
            s_Logger.Error(throwEx.Message);
            throw throwEx;
        }
    }
}
