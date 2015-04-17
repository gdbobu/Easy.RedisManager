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
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Easy.RedisManager.RedisAccess
{
    /// <summary>
    /// Redis Client
    /// 网络层
    /// Redis在TCP端口6379上监听到来的连接，客户端连接到来时，Redis服务器为此创建一个TCP连接。
    /// 在客户端与服务器端之间传输的每个Redis命令或者数据都以\r\n结尾。
    /// 
    /// 请求
    /// Redis接收由不同参数组成的命令。一旦收到命令，将会立刻被处理，并回复给客户端。
    /// 
    /// 新的统一请求协议
    /// 新的统一协议已在Redis 1.2中引入，但是在Redis 2.0中，这就成为了与Redis服务器通讯的标准方式。
    /// 在这个统一协议里，发送给Redis服务端的所有参数都是二进制安全的。以下是通用形式：
    /// *<number of arguments> CR LF
    /// $<number of bytes of argument 1> CR LF
    /// <argument data> CR LF
    /// ...
    /// $<number of bytes of argument N> CR LF
    /// <argument data> CR LF
    /// 
    /// 例子如下：
    /// *3
    /// $3
    /// SET
    /// $5
    /// mykey
    /// $7
    /// myvalue
    /// 上面的命令看上去像是单引号字符串，所以可以在查询中看到每个字节的准确值：
    /// "*3\r\n$3\r\nSET\r\n$5\r\nmykey\r\n$7\r\nmyvalue\r\n"
    /// 在Redis的回复中也使用这样的格式。批量回复时，这种格式用于每个参数$6\r\nmydata\r\n。 
    /// 实际的统一请求协议是Redis用于返回列表项，并调用 Multi-bulk回复。 仅仅是N个以以*<argc>\r\n为前缀的不同批量回复，<argc>是紧随的参数（批量回复）数目。
    /// 
    /// 回复
    /// Redis用不同的回复类型回复命令。它可能从服务器发送的第一个字节开始校验回复类型：
    /// 用单行回复，回复的第一个字节将是“+”
    /// 错误消息，回复的第一个字节将是“-”
    /// 整型数字，回复的第一个字节将是“:”
    /// 批量回复，回复的第一个字节将是“$”
    /// 多个批量回复，回复的第一个字节将是“*”
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

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected abstract void SendCommand();

        public abstract EnumRedisVersion GetSupportVersion();

        #region 返回处理
        /// 状态回复
        /// 状态回复（或者单行回复）以“+”开始以“\r\n”结尾的单行字符串形式。例如：+OK
        /// 客户端库将在“+”后面返回所有数据，正如上例中字符串“OK”一样。
        /// 
        /// 错误回复
        /// 错误回复发送类似于状态回复。唯一的不同是第一个字节用“-”代替“+”。
        /// 错误回复仅仅在一些意料之外的事情发生时发送，例如：如果你试图执行一个操作来应付错误的数据类型，
        /// 或者如果命令不存在等等。所以当收到一个错误回复时，客户端将会出现一个异常。

        /// <summary>
        /// 期待返回成功
        /// </summary>
        protected void ExpectSuccess()
        {
            int c = this._socketClient.SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = this._socketClient.ReadLine();
            Log((char)c + s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") && s.Length >= 4 ? s.Substring(4) : s);
        }
       
        /// <summary>
        /// 状态代码回复
        /// </summary>
        /// <returns></returns>
        private string ExpectCode()
        {
            int c = this._socketClient.SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = this._socketClient.ReadLine();
            Log((char)c + s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

            return s;
        }

        /// <summary>
        /// 返回的预期值
        /// </summary>
        /// <param name="word"></param>
        private void ExpectWord(string word)
        {
            int c = this._socketClient.SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = this._socketClient.ReadLine();
            Log((char)c + s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

            if (s != word)
                throw CreateResponseError(string.Format("Expected '{0}' got '{1}'", word, s));
        }

        /// <summary>
        /// 期望成功
        /// </summary>
        internal void ExpectOk()
        {
            ExpectWord("OK");
        }

        /// <summary>
        /// 期望入到事务队列
        /// </summary>
        internal void ExpectQueued()
        {
            ExpectWord("QUEUED");
        }
        #endregion

        #region SendCommand
        /// <summary>
        /// 发送命令，返回成功
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        protected void SendExpectSuccess(params byte[][] cmdWithBinaryArgs)
        {
            if (!this._socketClient.SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            ExpectSuccess();
        }

        /// <summary>
        /// 发送命令，返回整型回复
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
        /// 发送命令，返回Double型回复
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected double SendExpectDouble(params byte[][] cmdWithBinaryArgs)
        {
            if (!this._socketClient.SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            return ReadDouble();
        }

        /// <summary>
        /// 发送命令，返回String回复
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected string SendExpectString(params byte[][] cmdWithBinaryArgs)
        {
            var bytes = SendExpectData(cmdWithBinaryArgs);
            return bytes.FromUtf8Bytes();
        }

        /// <summary>
        /// 发送命令，返回状态回复
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected string SendExpectCode(params byte[][] cmdWithBinaryArgs)
        {
            if (!this._socketClient.SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            return ExpectCode();
        }

        /// <summary>
        /// 发送命令，返回批量或者多批量回复
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected byte[][] SendExpectMultiData(params byte[][] cmdWithBinaryArgs)
        {
            if (!this._socketClient.SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            return ReadMultiData();
        }

        /// <summary>
        /// 发送命令，返回批量或者多批量回复
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected object[] SendExpectDeeplyNestedMultiData(params byte[][] cmdWithBinaryArgs)
        {
            if (!this._socketClient.SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            return ReadDeeplyNestedMultiData();
        }

        /// <summary>
        /// 发送命令，返回一个单二进制安全字符串。
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
        /// 合并命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static byte[][] MergeCommandWithArgs(byte[] cmd, params string[] args)
        {
            var byteArgs = args.ToMultiByteArray();
            return MergeCommandWithArgs(cmd, byteArgs);
        }
        
        /// <summary>
        /// 合并命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static byte[][] MergeCommandWithArgs(byte[] cmd, params byte[][] args)
        {
            var mergedBytes = new byte[1 + args.Length][];
            mergedBytes[0] = cmd;
            for (var i = 0; i < args.Length; i++)
            {
                mergedBytes[i + 1] = args[i];
            }
            return mergedBytes;
        }

        /// <summary>
        /// 合并命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="firstArg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static byte[][] MergeCommandWithArgs(byte[] cmd, byte[] firstArg, params byte[][] args)
        {
            var mergedBytes = new byte[2 + args.Length][];
            mergedBytes[0] = cmd;
            mergedBytes[1] = firstArg;
            for (var i = 0; i < args.Length; i++)
            {
                mergedBytes[i + 2] = args[i];
            }
            return mergedBytes;
        }

        /// <summary>
        /// 合并命令和Key，参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static byte[][] MergeCommandWithKeysAndValues(byte[] cmd, byte[][] keys, byte[][] values)
        {
            var firstParams = new[] { cmd };
            return MergeCommandWithKeysAndValues(firstParams, keys, values);
        }

        /// <summary>
        /// 合并命令和Key，参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="firstArg"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static byte[][] MergeCommandWithKeysAndValues(byte[] cmd, byte[] firstArg, byte[][] keys, byte[][] values)
        {
            var firstParams = new[] { cmd, firstArg };
            return MergeCommandWithKeysAndValues(firstParams, keys, values);
        }

        /// <summary>
        /// 合并命令和Key，参数
        /// </summary>
        /// <param name="firstParams"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static byte[][] MergeCommandWithKeysAndValues(byte[][] firstParams,
            byte[][] keys, byte[][] values)
        {
            if (keys == null || keys.Length == 0)
                throw new ArgumentNullException("keys");
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");
            if (keys.Length != values.Length)
                throw new ArgumentException("The number of values must be equal to the number of keys");

            var keyValueStartIndex = (firstParams != null) ? firstParams.Length : 0;

            var keysAndValuesLength = keys.Length * 2 + keyValueStartIndex;
            var keysAndValues = new byte[keysAndValuesLength][];

            for (var i = 0; i < keyValueStartIndex; i++)
            {
                keysAndValues[i] = firstParams[i];
            }

            var j = 0;
            for (var i = keyValueStartIndex; i < keysAndValuesLength; i += 2)
            {
                keysAndValues[i] = keys[j];
                keysAndValues[i + 1] = values[j];
                j++;
            }
            return keysAndValues;
        }

        /// <summary>
        /// 合并和转换成Byte数组
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected byte[][] MergeAndConvertToBytes(string[] keys, string[] args)
        {
            if (keys == null)
                keys = new string[0];
            if (args == null)
                args = new string[0];

            var keysLength = keys.Length;
            var merged = new string[keysLength + args.Length];
            for (var i = 0; i < merged.Length; i++)
            {
                merged[i] = i < keysLength ? keys[i] : args[i - keysLength];
            }

            return ConvertToBytes(merged);
        }
        #endregion

        #region Read data
        /// <summary>
        /// 整形状态回复
        /// 这种回复类型只是用CRLF结尾字符串来表示整型，用一个字节的“：”作为前缀。
        /// 例如：“：0\r\n”，或者“:1000\r\n”是整型回复。
        /// 像INCR或者LASTAVE命令用整型回复作为实际回复值，此时对于返回的整型没有特殊的意思。
        /// 它仅仅是为INCR、LASTSAVE的UNIX时间等增加数值。
        /// 一些命令像EXISTS将为true返回1，为false返回0。
        /// 其它命令像SADD、SREM和SETNX如果操作实际完成了的话将返回1，否则返回0。
        /// 接下来的命令将回复一个整型回复：SETNX、DEL、EXISTS、INCR、INCRBY、DECR、DECRBY、
        /// DBSIZE、LASTSAVE、RENAMENX、MOVE、LLEN、SADD、SREM、SISMEMBER、SCARD。
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            int c = this._socketClient.SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data.");

            var s = this._socketClient.ReadLine();

            Log("R:{0}", s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

            if (c == ':' || c == '$')//really strange why ZRANK needs the '$' here
            {
                int i;
                if (int.TryParse(s, out i))
                    return i;
            }
            throw CreateResponseError("Unknown reply on integer response: " + c + s);

        }

        /// <summary>
        /// 整形状态回复
        /// 这种回复类型只是用CRLF结尾字符串来表示整型，用一个字节的“：”作为前缀。
        /// 例如：“：0\r\n”，或者“:1000\r\n”是整型回复。
        /// 像INCR或者LASTAVE命令用整型回复作为实际回复值，此时对于返回的整型没有特殊的意思。
        /// 它仅仅是为INCR、LASTSAVE的UNIX时间等增加数值。
        /// 一些命令像EXISTS将为true返回1，为false返回0。
        /// 其它命令像SADD、SREM和SETNX如果操作实际完成了的话将返回1，否则返回0。
        /// 接下来的命令将回复一个整型回复：SETNX、DEL、EXISTS、INCR、INCRBY、DECR、DECRBY、
        /// DBSIZE、LASTSAVE、RENAMENX、MOVE、LLEN、SADD、SREM、SISMEMBER、SCARD。
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
        /// 读取Double类型的回复
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            var bytes = ReadData();
            return (bytes == null) ? double.NaN : ParseDouble(bytes);
        }

        /// <summary>
        /// 读取一个单二进制安全字符串。
        /// </summary>
        /// <returns></returns>
        private byte[] ReadData()
        {
            var r = this._socketClient.ReadLine();
            return ParseSingleLine(r);
        }

        /// <summary>
        /// 批量回复（Bulk replies）
        /// 批量回复被服务器用于返回一个单二进制安全字符串。
        /// C: GET mykey
        /// S: $6\r\nfoobar\r\n
        /// 服务器发送第一行回复，该行以“$”开始后面跟随实际要发送的字节数，随后是CRLF，
        /// 然后发送实际数据，随后是2个字节的额外数据用于最后的CRLF。
        /// 
        /// 服务器发送的准确序列如下：
        /// "$6\r\nfoobar\r\n"
        /// 
        /// 如果请求的值不存在，批量回复将使用特殊的值-1来作为数据长度，例如：
        /// C: GET nonexistingkey
        /// S: $-1
        /// 当请求的对象不存在时，客户端库API不会返回空字符串，而会返回空对象。
        /// 例如：Ruby库返回‘nil’，而C库返回NULL（或者在回复的对象里设置指定的标志）等等。
        /// 
        /// 多批量回复（Multi-bulk replies）
        /// 像命令LRNGE需要返回多个值（列表的每个元素是一个值，而LRANGE需要返回多于一个单元素）。
        /// 使用多批量写是有技巧的，用一个初始行作为前缀来指示多少个批量写紧随其后。
        /// 批量回复的第一个字节总是*，例如：
        /// C: LRANGE mylist 0 3
        /// s: *4
        /// s: $3
        /// s: foo
        /// s: $3
        /// s: bar
        /// s: $5
        /// s: Hello
        /// s: $5
        /// s: World
        /// 正如您可以看到的多批量回复是以完全相同的格式使用Redis统一协议将命令发送给服务器。
        /// 服务器发送的第一行是*4\r\n，用于指定紧随着4个批量回复。然后传送每个批量写。
        /// 如果指定的键不存在，则该键被认为是持有一个空的列表，且数值0被当作多批量计数值来发送，例如：
        /// C: LRANGE nokey 0 1
        /// S: *0
        /// 当BLPOP命令超时时，它返回nil多批量回复。这种类型多批量回复的计数器是-1，且值被当作nil来解释。例如：
        /// C: BLPOP key 1
        /// S: *-1
        /// 当这种情况发生时，客户端库API将返回空nil对象，且不是一个空列表。
        /// 这必须有别于空列表和错误条件（例如：BLPOP命令的超时条件）。
        /// 
        /// 多批量回复中的Nil元素
        /// 多批量回复的单元素长度可能是-1，为了发出信号这个元素被丢失且不是空字符串。
        /// 这种情况发送在SORT命令时，此时使用GET模式选项且指定的键丢失。一个多批量回复包含一个空元素的例子如下：
        /// S: *3
        /// S: $3
        /// S: foo
        /// S: $-1
        /// S: $3
        /// S: bar
        /// 第二个元素是空。客户端库返回如下:
        /// ["foo",nil,"bar"]
        /// </summary>
        /// <returns></returns>
        private byte[][] ReadMultiData()
        {
            int c = this._socketClient.SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = this._socketClient.ReadLine();
            Log("R: {0}", s);

            switch (c)
            {
                // Some commands like BRPOPLPUSH may return Bulk Reply instead of Multi-bulk
                case '$':
                    var t = new byte[2][];
                    t[1] = ParseSingleLine(string.Concat(char.ToString((char)c), s));
                    return t;

                case '-':
                    throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

                case '*':
                    int count;
                    if (int.TryParse(s, out count))
                    {
                        if (count == -1)
                        {
                            //redis is in an invalid state
                            return new byte[0][];
                        }

                        var result = new byte[count][];

                        for (int i = 0; i < count; i++)
                            result[i] = ReadData();

                        return result;
                    }
                    break;
            }

            throw CreateResponseError("Unknown reply on multi-request: " + c + s);
        }

        /// <summary>
        /// 读取多个类型的数据
        /// </summary>
        /// <returns></returns>
        private object[] ReadDeeplyNestedMultiData()
        {
            return (object[])ReadDeeplyNestedMultiDataItem();
        }

        /// <summary>
        /// 批量回复（Bulk replies）
        /// 批量回复被服务器用于返回一个单二进制安全字符串。
        /// C: GET mykey
        /// S: $6\r\nfoobar\r\n
        /// 服务器发送第一行回复，该行以“$”开始后面跟随实际要发送的字节数，随后是CRLF，然后发送实际数据，随后是2个字节的额外数据用于最后的CRLF。
        /// 
        /// 服务器发送的准确序列如下：
        /// "$6\r\nfoobar\r\n"
        /// 
        /// 如果请求的值不存在，批量回复将使用特殊的值-1来作为数据长度，例如：
        /// C: GET nonexistingkey
        /// S: $-1
        /// 当请求的对象不存在时，客户端库API不会返回空字符串，而会返回空对象。
        /// 例如：Ruby库返回‘nil’，而C库返回NULL（或者在回复的对象里设置指定的标志）等等。
        /// 
        /// 多批量回复（Multi-bulk replies）
        /// 像命令LRNGE需要返回多个值（列表的每个元素是一个值，而LRANGE需要返回多于一个单元素）。
        /// 使用多批量写是有技巧的，用一个初始行作为前缀来指示多少个批量写紧随其后。批量回复的第一个字节总是*，例如：
        /// C: LRANGE mylist 0 3
        /// s: *4
        /// s: $3
        /// s: foo
        /// s: $3
        /// s: bar
        /// s: $5
        /// s: Hello
        /// s: $5
        /// s: World
        /// 正如您可以看到的多批量回复是以完全相同的格式使用Redis统一协议将命令发送给服务器。
        /// 服务器发送的第一行是*4\r\n，用于指定紧随着4个批量回复。然后传送每个批量写。
        /// 如果指定的键不存在，则该键被认为是持有一个空的列表，且数值0被当作多批量计数值来发送，例如：
        /// C: LRANGE nokey 0 1
        /// S: *0
        /// 当BLPOP命令超时时，它返回nil多批量回复。这种类型多批量回复的计数器是-1，且值被当作nil来解释。例如：
        /// C: BLPOP key 1
        /// S: *-1
        /// 当这种情况发生时，客户端库API将返回空nil对象，且不是一个空列表。这必须有别于空列表和错误条件（例如：BLPOP命令的超时条件）。
        /// 
        /// 多批量回复中的Nil元素
        /// 多批量回复的单元素长度可能是-1，为了发出信号这个元素被丢失且不是空字符串。
        /// 这种情况发送在SORT命令时，此时使用GET模式选项且指定的键丢失。一个多批量回复包含一个空元素的例子如下：
        /// S: *3
        /// S: $3
        /// S: foo
        /// S: $-1
        /// S: $3
        /// S: bar
        /// 第二个元素是空。客户端库返回如下:
        /// ["foo",nil,"bar"]
        /// </summary>
        /// <returns></returns>
        private object ReadDeeplyNestedMultiDataItem()
        {
            int c = this._socketClient.SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = this._socketClient.ReadLine();
            Log("R: {0}", s);

            switch (c)
            {
                case '$':
                    return ParseSingleLine(string.Concat(char.ToString((char)c), s));

                case '-':
                    throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

                case '*':
                    int count;
                    if (int.TryParse(s, out count))
                    {
                        var array = new object[count];
                        for (int i = 0; i < count; i++)
                        {
                            array[i] = ReadDeeplyNestedMultiDataItem();
                        }

                        return array;
                    }
                    break;

                default:
                    return s;
            }

            throw CreateResponseError("Unknown reply on multi-request: " + c + s);
        }
        #endregion

        #region Type Convert
        /// <summary>
        /// 字节流转换Double
        /// </summary>
        /// <param name="doubleBytes"></param>
        /// <returns></returns>
        public static double ParseDouble(byte[] doubleBytes)
        {
            var doubleString = Encoding.UTF8.GetString(doubleBytes);

            double d;
            double.TryParse(doubleString, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out d);

            return d;
        }

        /// <summary>
        /// 读取批量回复的数据
        /// 批量回复（Bulk replies）
        /// 批量回复被服务器用于返回一个单二进制安全字符串。
        /// 服务器发送第一行回复，该行以“$”开始后面跟随实际要发送的字节数，随后是CRLF，
        /// 然后发送实际数据，随后是2个字节的额外数据用于最后的CRLF。
        /// 
        /// 服务器发送的准确序列如下：
        /// "$6\r\nfoobar\r\n"
        /// 
        /// 如果请求的值不存在，批量回复将使用特殊的值-1来作为数据长度，例如：
        /// C: GET nonexistingkey
        /// S: $-1
        /// 
        /// 当请求的对象不存在时，客户端库API不会返回空字符串，而会返回空对象。
        /// 例如：Ruby库返回‘nil’，而C库返回NULL（或者在回复的对象里设置指定的标志）等等。
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
        /// 抓换成Byte数组
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected byte[][] ConvertToBytes(string[] keys)
        {
            var keyBytes = new byte[keys.Length][];
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                keyBytes[i] = key != null ? key.ToUtf8Bytes() : new byte[0];
            }
            return keyBytes;
        }
        #endregion

        #region Script（脚本）
        /// <summary>
        /// EVAL script numkeys key [key ...] arg [arg ...]
        /// EVAL 和 EVALSHA 命令是从 Redis 2.6.0 版本开始的，使用内置的 Lua 解释器，可以对 Lua 脚本进行求值。
        /// </summary>
        /// <param name="luaBody"></param>
        /// <param name="numberKeysInArgs"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public long EvalInt(string luaBody, int numberKeysInArgs, params byte[][] keys)
        {
            if (luaBody == null)
                throw new ArgumentNullException("luaBody");

            var cmdArgs = MergeCommandWithArgs(RedisCommands.Eval, luaBody.ToUtf8Bytes(), keys.PrependInt(numberKeysInArgs));
            return SendExpectLong(cmdArgs);
        }

        /// <summary>
        /// EVALSHA sha1 numkeys key [key ...] arg [arg ...]
        /// 根据给定的 sha1 校验码，对缓存在服务器中的脚本进行求值。
        /// 将脚本缓存到服务器的操作可以通过 SCRIPT LOAD 命令进行。
        /// 这个命令的其他地方，比如参数的传入方式，都和 EVAL 命令一样。
        /// </summary>
        /// <param name="sha1"></param>
        /// <param name="numberKeysInArgs"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public long EvalShaInt(string sha1, int numberKeysInArgs, params byte[][] keys)
        {
            if (sha1 == null)
                throw new ArgumentNullException("sha1");

            var cmdArgs = MergeCommandWithArgs(RedisCommands.EvalSha, sha1.ToUtf8Bytes(), keys.PrependInt(numberKeysInArgs));
            return SendExpectLong(cmdArgs);
        }

        /// <summary>
        /// EVAL script numkeys key [key ...] arg [arg ...]
        /// EVAL 和 EVALSHA 命令是从 Redis 2.6.0 版本开始的，使用内置的 Lua 解释器，可以对 Lua 脚本进行求值。
        /// </summary>
        /// <param name="luaBody"></param>
        /// <param name="numberKeysInArgs"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string EvalStr(string luaBody, int numberKeysInArgs, params byte[][] keys)
        {
            if (luaBody == null)
                throw new ArgumentNullException("luaBody");

            var cmdArgs = MergeCommandWithArgs(RedisCommands.Eval, luaBody.ToUtf8Bytes(), keys.PrependInt(numberKeysInArgs));
            return SendExpectData(cmdArgs).FromUtf8Bytes();
        }

        /// <summary>
        /// EVALSHA sha1 numkeys key [key ...] arg [arg ...]
        /// 根据给定的 sha1 校验码，对缓存在服务器中的脚本进行求值。
        /// 将脚本缓存到服务器的操作可以通过 SCRIPT LOAD 命令进行。
        /// 这个命令的其他地方，比如参数的传入方式，都和 EVAL 命令一样。
        /// </summary>
        /// <param name="sha1"></param>
        /// <param name="numberKeysInArgs"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string EvalShaStr(string sha1, int numberKeysInArgs, params byte[][] keys)
        {
            if (sha1 == null)
                throw new ArgumentNullException("sha1");

            var cmdArgs = MergeCommandWithArgs(RedisCommands.EvalSha, sha1.ToUtf8Bytes(), keys.PrependInt(numberKeysInArgs));
            return SendExpectData(cmdArgs).FromUtf8Bytes();
        }

        /// <summary>
        /// EVAL script numkeys key [key ...] arg [arg ...]
        /// EVAL 和 EVALSHA 命令是从 Redis 2.6.0 版本开始的，使用内置的 Lua 解释器，可以对 Lua 脚本进行求值。
        /// </summary>
        /// <param name="luaBody"></param>
        /// <param name="numberKeysInArgs"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public byte[][] Eval(string luaBody, int numberKeysInArgs, params byte[][] keys)
        {
            if (luaBody == null)
                throw new ArgumentNullException("luaBody");

            var cmdArgs = MergeCommandWithArgs(RedisCommands.Eval, luaBody.ToUtf8Bytes(), keys.PrependInt(numberKeysInArgs));
            return SendExpectMultiData(cmdArgs);
        }

        /// <summary>
        /// EVALSHA sha1 numkeys key [key ...] arg [arg ...]
        /// 根据给定的 sha1 校验码，对缓存在服务器中的脚本进行求值。
        /// 将脚本缓存到服务器的操作可以通过 SCRIPT LOAD 命令进行。
        /// 这个命令的其他地方，比如参数的传入方式，都和 EVAL 命令一样。
        /// </summary>
        /// <param name="sha1"></param>
        /// <param name="numberKeysInArgs"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public byte[][] EvalSha(string sha1, int numberKeysInArgs, params byte[][] keys)
        {
            if (sha1 == null)
                throw new ArgumentNullException("sha1");

            var cmdArgs = MergeCommandWithArgs(RedisCommands.EvalSha, sha1.ToUtf8Bytes(), keys.PrependInt(numberKeysInArgs));
            return SendExpectMultiData(cmdArgs);
        }

        /// <summary>
        /// 计算SHA
        /// </summary>
        /// <param name="luaBody"></param>
        /// <returns></returns>
        public string CalculateSha1(string luaBody)
        {
            if (luaBody == null)
                throw new ArgumentNullException("luaBody");

            byte[] buffer = Encoding.UTF8.GetBytes(luaBody);
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
        }

        /// <summary>
        /// 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本。
        /// </summary>
        /// <param name="luaBody"></param>
        /// <returns></returns>
        public byte[] ScriptLoad(string luaBody)
        {
            if (luaBody == null)
                throw new ArgumentNullException("luaBody");

            var cmdArgs = MergeCommandWithArgs(RedisCommands.Script, RedisCommands.Load, luaBody.ToUtf8Bytes());
            return SendExpectData(cmdArgs);
        }

        /// <summary>
        /// SCRIPT EXISTS script [script ...]
        /// 给定一个或多个脚本的 SHA1 校验和，返回一个包含 0 和 1 的列表，
        /// 表示校验和所指定的脚本是否已经被保存在缓存当中。
        /// </summary>
        /// <param name="sha1Refs"></param>
        /// <returns></returns>
        public byte[][] ScriptExists(params byte[][] sha1Refs)
        {
            var keysAndValues = MergeCommandWithArgs(RedisCommands.Script, RedisCommands.Exists, sha1Refs);
            return SendExpectMultiData(keysAndValues);
        }

        /// <summary>
        /// SCRIPT FLUSH
        /// 清除所有 Lua 脚本缓存。
        /// </summary>
        public void ScriptFlush()
        {
            SendExpectSuccess(RedisCommands.Script, RedisCommands.Flush);
        }

        /// <summary>
        /// SCRIPT KILL
        /// 杀死当前正在运行的 Lua 脚本，当且仅当这个脚本没有执行过任何写操作时，这个命令才生效。
        /// 这个命令主要用于终止运行时间过长的脚本，比如一个因为 BUG 而发生无限 loop 的脚本，诸如此类。
        /// SCRIPT KILL 执行之后，当前正在运行的脚本会被杀死，执行这个脚本的客户端会从 EVAL 命令的阻塞当中退出，并收到一个错误作为返回值。
        /// 另一方面，假如当前正在运行的脚本已经执行过写操作，那么即使执行 SCRIPT KILL ，也无法将它杀死，因为这是违反 Lua 脚本的原子性执行原则的。
        /// 在这种情况下，唯一可行的办法是使用 SHUTDOWN NOSAVE 命令，通过停止整个 Redis 进程来停止脚本的运行，并防止不完整(half-written)的信息被写入数据库中。
        /// 关于使用 Redis 对 Lua 脚本进行求值的更多信息，请参见 EVAL 命令。
        /// </summary>
        public void ScriptKill()
        {
            SendExpectSuccess(RedisCommands.Script, RedisCommands.Kill);
        }
        #endregion

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

        /// <summary>
        /// 将此实例与指定对象进行比较并返回一个对二者的相对值的指示。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(RedisClient other)
        {
            if (other == null)
                return -1;

            return this.GetSupportVersion().CompareTo(other.GetSupportVersion()) * -1;
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
