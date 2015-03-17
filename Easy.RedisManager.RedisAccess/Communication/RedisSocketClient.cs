using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Easy.Common;
using Easy.RedisManager.RedisAccess.Exceptions;
using System.Collections.Generic;
using Easy.RedisManager.Entity.Redis;
using Easy.RedisManager.Common.Command;
using System.Net;
using System.Runtime.CompilerServices;
using Easy.RedisManager.Common.Enum;
using System.Globalization;

namespace Easy.RedisManager.RedisAccess.Communication
{
    /// <summary>
    ///     Redis通信客户端（Socket版）
    /// </summary>
    public class RedisSocketClient : IDisposable
    {
        #region Constant

        public const int UnKnown = -1;
        public const long DefaultDb = 0;
        public const int DefaultPort = 6379;
        public const string DefaultHost = "localhost";
        public const int DefaultIdleTimeOutSecs = 240; //default on redis is 300

        #endregion

        #region Variable

        // 记录日志类
        private static readonly ILogger SLogger = LogFactory.CreateLogger(typeof (RedisSocketClient));
        // 命令缓存
        private readonly IList<ArraySegment<byte>> _cmdBuffer = new List<ArraySegment<byte>>();
        // 结束符
        private readonly byte[] _endData = new[] { (byte)'\r', (byte)'\n' };
        internal const int Success = 1;
        internal const int OneGb = 1073741824;

        // Socket
        protected Socket _redisSocket;
        // 接收数据的缓存
        protected BufferedStream _buffStream;
        // Timer类的实例
        private Timer _usageTimer;
        // 每小时请求的次数
        private int _requestsPerHour;
        // 客户端的端口号
        private int _clientPort;
        // 最近一次的命令
        private string _lastCommand;
        // 最近一次SocketException
        private SocketException _lastSocketException;
        // 当前缓存
        private byte[] _currentBuffer = BufferPool.GetBuffer();
        private IRedisPipelineShared _pipeline;
        // 信息字典
        private Dictionary<string, string> _info;

        // 当前缓存的的索引
        private int _currentBufferIndex;
        // 最近一次连接的时间
        internal long _lastConnectedAtTimestamp;
        //public Action<IRedisNativeClient> ConnectionFilter { get; set; }

        #endregion

        #region Constructor

        public RedisSocketClient()
        {
        }

        public RedisSocketClient(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public RedisSocketClient(string host, int port, string password)
            :this(host, port)
        {
            Password = password;
        }

        public RedisSocketClient(string host, int port, string password
            , int connectTimeout, int sendTimeout, int receiveTimeout, int idleTimeOutSecs)
            :this(host, port, password)
        {
            ConnectTimeout = connectTimeout;
            SendTimeout = sendTimeout;
            ReceiveTimeout = receiveTimeout;
            IdleTimeOutSecs = idleTimeOutSecs;
        }

        #endregion

        #region Property
        /// <summary>
        /// 服务器版本号
        /// </summary>
        public int ServerVersionNumber { get; set; }

        /// <summary>
        /// 服务器的IP地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 服务器的端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 连接Redis数据库的密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 当前连接的Redis数据库的数据库编号
        /// </summary>
        public int DB { get; set; }

        /// <summary>
        /// 连接服务器超时时间
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// 发送数据超时时间
        /// </summary>
        public int SendTimeout { get; set; }

        /// <summary>
        /// 接收数据的超时时间
        /// </summary>
        public int ReceiveTimeout { get; set; }

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool HadExceptions { get; protected set; }

        /// <summary>
        /// 空闲时间
        /// </summary>
        public int IdleTimeOutSecs { get; set; }

        /// <summary>
        /// 选择的数据库
        /// </summary>
        public long Db { get; set; }

        /// <summary>
        /// 获取数据库大小
        /// </summary>
        public long DbSize
        {
            get
            {
                return SendExpectLong(RedisCommands.DbSize);
            }
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
        /// 信息字典
        /// </summary>
        public Dictionary<string, string> Info
        {
            get
            {
                if (this._info == null)
                {
                    var lines = SendExpectString(RedisCommands.Info);
                    this._info = new Dictionary<string, string>();

                    foreach (var line in lines
                        .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var p = line.IndexOf(':');
                        if (p == -1) continue;

                        this._info.Add(line.Substring(0, p), line.Substring(p + 1));
                    }
                }
                return this._info;
            }
        }

        /// <summary>
        /// 是否释放资源
        /// </summary>
        internal bool IsDisposed { get; set; }
        internal IRedisPipelineShared Pipeline
        {
            get
            {
                return _pipeline;
            }
            set
            {
                if (value != null)
                    AssertConnectedSocket();
                _pipeline = value;
            }
        }

        /// <summary>
        /// 服务器版本号
        /// </summary>
        public string ServerVersion
        {
            get
            {
                string version;
                this.Info.TryGetValue("redis_version", out version);
                return version;
            }
        }

        #endregion

        #region Method

        #region Connect

        public virtual void OnConnected()
        {
        }

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            // 初始化 Timer 类的新实例，使用 TimeSpan 值来度量时间间隔。
            // callback
            // 参数: 
            // 1.callback 类型：System.Threading.TimerCallback 一个 TimerCallback 委托，表示要执行的方法。
            // 2.state 类型：System.Object 一个包含回调方法要使用的信息的对象，或者为 null。
            // 3.dueTime 类型：System.TimeSpan TimeSpan ，表示在 callback 参数调用它的方法之前延迟的时间量。 
            //         指定 -1 毫秒以防止启动计时器。 指定零 (0) 可立即启动计时器。 
            // 4.period 类型：System.TimeSpan 在调用 callback 所引用的方法之间的时间间隔。 
            //         指定 -1 毫秒可以禁用定期终止。 
            // 在超过 dueTime 后及此后每隔 period 时间间隔，都会调用一次由 callback 参数指定的委托。
            // 如果 dueTime 为零 (0)，则立即调用 callback。 如果 dueTime 是 -1 毫秒，则不会调用 callback；计时器将被禁用，但通过调用 Change 方法可以重新启用计时器。
            // 如果 period 为零 (0) 或 -1 毫秒，而且 dueTime 为正，则只会调用一次 callback；计时器的定期行为将被禁用，但通过使用 Change 方法可以重新启用该行为。
            if (_usageTimer == null)
            {
                _usageTimer = new Timer(delegate { this._requestsPerHour = 0; }, null, TimeSpan.FromMilliseconds(0),
                    TimeSpan.FromHours(1));
            }

            // 初始化Socket
            _redisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = SendTimeout,
                ReceiveTimeout = ReceiveTimeout
            };

            try
            {
                if (ConnectTimeout == 0)
                {
                    _redisSocket.Connect(Host, Port);
                }
                else
                {
                    var connectResult = _redisSocket.BeginConnect(Host, Port, null, null);
                    // 阻塞当前进程，直到收到信号位置，或者到了超时时间后，退出
                    connectResult.AsyncWaitHandle.WaitOne(ConnectTimeout, true);
                }

                if (!_redisSocket.Connected)
                {
                    _redisSocket.Close();
                    _redisSocket.Dispose();
                    _redisSocket = null;
                    HadExceptions = true;
                    return;
                }

                // 初始化16KB的缓存
                _buffStream = new BufferedStream(new NetworkStream(_redisSocket), 16 * 1024);

                if (!string.IsNullOrEmpty(Password))
                    SendExpectSuccess(RedisCommands.Auth, Password.ToUtf8Bytes());

                if (Db != 0)
                    SendExpectSuccess(RedisCommands.Select, Db.ToUtf8Bytes());

                try
                {
                    if (ServerVersionNumber == 0)
                    {
                        var parts = ServerVersion.Split('.');
                        var version = int.Parse(parts[0]) * 1000;
                        if (parts.Length > 1)
                            version += int.Parse(parts[1]) * 100;
                        if (parts.Length > 2)
                            version += int.Parse(parts[2]);

                        ServerVersionNumber = version;
                    }
                }
                catch (Exception)
                {
                    //Twemproxy doesn't support the INFO command so automatically closes the socket
                    //Fallback to ServerVersionNumber=Unknown then try re-connecting
                    ServerVersionNumber = UnKnown;
                    Connect();
                    return;
                }

                var ipEndpoint = _redisSocket.LocalEndPoint as IPEndPoint;
                _clientPort = ipEndpoint != null ? ipEndpoint.Port : -1;
                _lastCommand = null;
                _lastSocketException = null;
                _lastConnectedAtTimestamp = Stopwatch.GetTimestamp();

                OnConnected();

                //if (ConnectionFilter != null)
                //{
                //    ConnectionFilter(this);
                //}

            }
            catch (Exception ex)
            {
                SLogger.Error("Error when trying to Connect().ErrorMessage:" + ex.Message);
                SLogger.Error("Error when trying to Connect().ErrorMessage:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 确保Socket是否连接
        /// </summary>
        /// <returns></returns>
        private bool AssertConnectedSocket()
        {
            if (_lastConnectedAtTimestamp > 0)
            {
                var now = Stopwatch.GetTimestamp();
                // 获取间隔的秒数
                var elapsedSecs = (now - _lastConnectedAtTimestamp) / Stopwatch.Frequency;

                if (_redisSocket == null || (elapsedSecs > IdleTimeOutSecs && !_redisSocket.IsConnected()))
                {
                    return ReConnect();
                }

                _lastConnectedAtTimestamp = now;
            }

            if (_redisSocket == null)
            {
                Connect();
            }

            return _redisSocket != null;
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <returns></returns>
        private bool ReConnect()
        {
            var previousDb = Db;

            SafeConnectionClose();
            Connect(); //sets db to 0

            if (previousDb != DefaultDb) Db = previousDb;
            return _redisSocket != null;
        }
        #endregion

        #region Close
        /// <summary>
        /// 以安全的方式关闭链接
        /// </summary>
        private void SafeConnectionClose()
        {
            try
            {
                // workaround for a .net bug: http://support.microsoft.com/kb/821625
                if (_buffStream != null)
                    _buffStream.Close();
            }
            catch (Exception ex)
            {
                SLogger.Error("Error when trying to BufferStaream Close().ErrorMessage:" + ex.Message);
                SLogger.Error("Error when trying to BufferStaream Close().ErrorMessage:" + ex.StackTrace);
            }

            try
            {
                if (_redisSocket != null)
                    _redisSocket.Close();
            }
            catch (Exception ex)
            {
                SLogger.Error("Error when trying to Socket Close().ErrorMessage:" + ex.Message);
                SLogger.Error("Error when trying to Socket Close()..ErrorMessage:" + ex.StackTrace);
            }
            _buffStream = null;
            _redisSocket = null;
        }

        public void Quit()
        {
            SendCommand(RedisCommands.Quit);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (ClientManager != null)
            //{
            //    ClientManager.DisposeClient(this);
            //    return;
            //}

            if (disposing)
            {
                //dispose un managed resources
                DisposeConnection();
            }
        }

        ~RedisSocketClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        internal void DisposeConnection()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            if (_redisSocket == null) return;

            try
            {
                Quit();
            }
            catch (Exception ex)
            {
                SLogger.Error("Error when trying to Quit().ErrorMessage:" + ex.Message);
                SLogger.Error("Error when trying to Quit().ErrorMessage:" + ex.StackTrace);
            }
            finally
            {
                SafeConnectionClose();
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// reset buffer index in send buffer
        /// </summary>
        public void ResetSendBuffer()
        {
            _currentBufferIndex = 0;
            for (int i = _cmdBuffer.Count - 1; i >= 0; i--)
            {
                var buffer = _cmdBuffer[i].Array;
                BufferPool.ReleaseBufferToPool(ref buffer);
                _cmdBuffer.RemoveAt(i);
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        public bool FlushSendBuffer()
        {
            try
            {
                if (_currentBufferIndex > 0)
                    PushCurrentBuffer();

                if (!Env.IsMono)
                {
                    _redisSocket.Send(_cmdBuffer); //Optimized for Windows
                }
                else
                {
                    //Sendling IList<ArraySegment> Throws 'Message to Large' SocketException in Mono
                    foreach (var segment in _cmdBuffer)
                    {
                        var buffer = segment.Array;
                        _redisSocket.Send(buffer, segment.Offset, segment.Count, SocketFlags.None);
                    }
                }
                ResetSendBuffer();
            }
            catch (SocketException ex)
            {
                _cmdBuffer.Clear();
                return HandleSocketException(ex);
            }
            return true;
        }

        /// <summary>
        /// 期待返回成功
        /// </summary>
        protected void ExpectSuccess()
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();

            Log((char)c + s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") && s.Length >= 4 ? s.Substring(4) : s);
        }

        private string ExpectCode()
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();

            Log((char)c + s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

            return s;
        }
        private void ExpectWord(string word)
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();

            Log((char)c + s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);

            if (s != word)
                throw CreateResponseError(string.Format("Expected '{0}' got '{1}'", word, s));
        }

        /// <summary>
        /// 发送命令，期待返回成功
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        private void SendExpectSuccess(params byte[][] cmdWithBinaryArgs)
        {
            if (!SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            if (Pipeline != null)
            {
                Pipeline.CompleteVoidQueuedCommand(ExpectSuccess);
                return;
            }

            ExpectSuccess();
        }

        /// <summary>
        /// 发送命令，返回Long型数据
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected long SendExpectLong(params byte[][] cmdWithBinaryArgs)
        {
            if (!SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            if (Pipeline != null)
            {
                Pipeline.CompleteLongQueuedCommand(ReadInt);
                return default(long);
            }
            return ReadLong();
        }

        /// <summary>
        /// 发送命令，返回比特流
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected byte[] SendExpectData(params byte[][] cmdWithBinaryArgs)
        {
            if (!SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            if (Pipeline != null)
            {
                Pipeline.CompleteBytesQueuedCommand(ReadData);
                return null;
            }
            return ReadData();
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

        protected string SendExpectCode(params byte[][] cmdWithBinaryArgs)
        {
            if (!SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            if (Pipeline != null)
            {
                Pipeline.CompleteStringQueuedCommand(ExpectCode);
                return null;
            }

            return ExpectCode();
        }
        protected double SendExpectDouble(params byte[][] cmdWithBinaryArgs)
        {
            if (!SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            if (Pipeline != null)
            {
                Pipeline.CompleteDoubleQueuedCommand(ReadDouble);
                return Double.NaN;
            }

            return ReadDouble();
        }

        protected byte[][] SendExpectMultiData(params byte[][] cmdWithBinaryArgs)
        {
            if (!SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            if (Pipeline != null)
            {
                Pipeline.CompleteMultiBytesQueuedCommand(ReadMultiData);
                return new byte[0][];
            }
            return ReadMultiData();
        }

        protected object[] SendExpectDeeplyNestedMultiData(params byte[][] cmdWithBinaryArgs)
        {
            if (!SendCommand(cmdWithBinaryArgs))
                throw CreateConnectionError();

            if (Pipeline != null)
            {
                throw new NotSupportedException("Pipeline is not supported.");
            }

            return ReadDeeplyNestedMultiData();
        }

        /// <summary>
        ///     Command to set multuple binary safe arguments
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        private bool SendCommand(params byte[][] cmdWithBinaryArgs)
        {
            if (!AssertConnectedSocket()) return false;

            Interlocked.Increment(ref _requestsPerHour);
            //if (_requestsPerHour % 20 == 0)
            //    LicenseUtils.AssertValidUsage(LicenseFeature.Redis, QuotaType.RequestsPerHour, __requestsPerHour);

            CmdLog(cmdWithBinaryArgs);

            //Total command lines count
            WriteAllToSendBuffer(cmdWithBinaryArgs);

            //pipeline will handle flush, if pipelining is turned on
            if (Pipeline == null)
                return FlushSendBuffer();

            return true;
        }

        /// <summary>
        /// 是否可以添加到当前的缓存
        /// </summary>
        /// <param name="cmdBytes"></param>
        /// <returns></returns>
        private bool CouldAddCurrentBuffer(byte[] cmdBytes)
        {
            if (_currentBufferIndex + cmdBytes.Length < BufferPool.BufferLength)
            {
                Buffer.BlockCopy(cmdBytes, 0, _currentBuffer, _currentBufferIndex, cmdBytes.Length);
                _currentBufferIndex += cmdBytes.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 保存现在的Buffer到命令Buffer队列
        /// 获取一个新的Buffer
        /// </summary>
        private void PushCurrentBuffer()
        {
            _cmdBuffer.Add(new ArraySegment<byte>(_currentBuffer, 0, _currentBufferIndex));
            _currentBuffer = BufferPool.GetBuffer();
            _currentBufferIndex = 0;
        }

        /// <summary>
        /// 写数据到发送缓冲区
        /// </summary>
        /// <param name="cmdBytes"></param>
        public void WriteToSendBuffer(byte[] cmdBytes)
        {
            if (CouldAddCurrentBuffer((cmdBytes))) return;

            PushCurrentBuffer();

            if (CouldAddCurrentBuffer((cmdBytes))) return;

            var bytesCopied = 0;
            while (bytesCopied < cmdBytes.Length)
            {
                var copyOfBytes = BufferPool.GetBuffer();
                var bytesToCopy = Math.Min(cmdBytes.Length - bytesCopied, copyOfBytes.Length);
                Buffer.BlockCopy(cmdBytes, bytesCopied, copyOfBytes, 0, bytesToCopy);
                _cmdBuffer.Add(new ArraySegment<byte>(copyOfBytes, 0, bytesToCopy));
                bytesCopied += bytesToCopy;
            }

        }

        /// <summary>
        /// 写数据到发送缓存
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        private void WriteAllToSendBuffer(params byte[][] cmdWithBinaryArgs)
        {
            WriteToSendBuffer(GetCmdBytes('*', cmdWithBinaryArgs.Length));

            foreach (var safeBinaryValue in cmdWithBinaryArgs)
            {
                WriteToSendBuffer(GetCmdBytes('$', safeBinaryValue.Length));
                WriteToSendBuffer(safeBinaryValue);
                WriteToSendBuffer(_endData);
            }
        }

        /// <summary>
        /// 获取发送数据的字节流
        /// </summary>
        /// <param name="cmdPrefix"></param>
        /// <param name="numOfLines"></param>
        /// <returns></returns>
        private byte[] GetCmdBytes(char cmdPrefix, int numOfLines)
        {
            var strLines = numOfLines.ToString();
            var strLinesLength = strLines.Length;

            var cmdBytes = new byte[1 + strLinesLength + 2];
            cmdBytes[0] = (byte)cmdPrefix;

            for (var i = 0; i < strLinesLength; i++)
                cmdBytes[i + 1] = (byte)strLines[i];

            cmdBytes[1 + strLinesLength] = 0x0D; // \r
            cmdBytes[2 + strLinesLength] = 0x0A; // \n

            return cmdBytes;
        }
        #endregion

        #region Read
        /// <summary>
        /// 安全模式读取一个字节
        /// </summary>
        /// <returns></returns>
        private int SafeReadByte()
        {
            return _buffStream.ReadByte();
        }

        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        protected string ReadLine()
        {
            var sb = new StringBuilder();

            int c;
            while ((c = _buffStream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
                sb.Append((char)c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 读取Int
        /// </summary>
        /// <returns></returns>
        public long ReadInt()
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();

            Log("R: {0}", s);

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
        /// 读取Long
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();

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
        /// 读取字节流
        /// </summary>
        /// <returns></returns>
        private byte[] ReadData()
        {
            var r = ReadLine();
            return ParseSingleLine(r);
        }

        public double ReadDouble()
        {
            var bytes = ReadData();
            return (bytes == null) ? double.NaN : ParseDouble(bytes);
        }

        public static double ParseDouble(byte[] doubleBytes)
        {
            var doubleString = Encoding.UTF8.GetString(doubleBytes);

            double d;
            double.TryParse(doubleString, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out d);

            return d;
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
                        var readCount = _buffStream.Read(retbuf, offset, count);
                        if (readCount <= 0)
                            throw CreateResponseError("Unexpected end of Stream");

                        offset += readCount;
                        count -= readCount;
                    }

                    if (_buffStream.ReadByte() != '\r' || _buffStream.ReadByte() != '\n')
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

        private byte[][] ReadMultiData()
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();
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

        private object[] ReadDeeplyNestedMultiData()
        {
            return (object[])ReadDeeplyNestedMultiDataItem();
        }

        private object ReadDeeplyNestedMultiDataItem()
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();
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

        internal int ReadMultiDataResultCount()
        {
            int c = SafeReadByte();
            if (c == -1)
                throw CreateResponseError("No more data");

            var s = ReadLine();
            Log("R: {0}", s);

            if (c == '-')
                throw CreateResponseError(s.StartsWith("ERR") ? s.Substring(4) : s);
            if (c == '*')
            {
                int count;
                if (int.TryParse(s, out count))
                {
                    return count;
                }
            }
            throw CreateResponseError("Unknown reply on multi-request: " + c + s);
        }

        private static byte[][] MergeCommandWithKeysAndValues(byte[] cmd, byte[][] keys, byte[][] values)
        {
            var firstParams = new[] { cmd };
            return MergeCommandWithKeysAndValues(firstParams, keys, values);
        }

        private static byte[][] MergeCommandWithKeysAndValues(byte[] cmd, byte[] firstArg, byte[][] keys, byte[][] values)
        {
            var firstParams = new[] { cmd, firstArg };
            return MergeCommandWithKeysAndValues(firstParams, keys, values);
        }

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

        private static byte[][] MergeCommandWithArgs(byte[] cmd, params string[] args)
        {
            var byteArgs = args.ToMultiByteArray();
            return MergeCommandWithArgs(cmd, byteArgs);
        }

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
        #endregion

        #region Log Method

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="args"></param>
        private void CmdLog(byte[][] args)
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(arg.FromUtf8Bytes());
            }
            _lastCommand = sb.ToString();
            if (_lastCommand.Length > 100)
                _lastCommand = _lastCommand.Substring(0, 100) + "...";

            SLogger.Debug("S: " + _lastCommand);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        private void Log(string fmt, params object[] args)
        {
            SLogger.Debug(string.Format("{0}", string.Format(fmt, args).Trim()));
        }

        #endregion

        #region Exception
        /// <summary>
        /// 处理Socket异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool HandleSocketException(SocketException ex)
        {
            HadExceptions = true;

            SLogger.Error("SocketException: " + ex.Message);
            SLogger.Error("SocketException: " + ex.StackTrace);

            _lastSocketException = ex;

            // timeout?
            _redisSocket.Close();
            _redisSocket = null;

            return false;
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
                string.Format("{0}, Port: {1}, LastCommand: {2}", error, _clientPort, _lastCommand));
            SLogger.Error(throwEx.Message);
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
                string.Format("Unable to Connect: Port: {0}", _clientPort), _lastSocketException);
            SLogger.Error(throwEx.Message);
            throw throwEx;
        }

        #endregion

        #region Common Operations
        public bool Ping()
        {
            return SendExpectCode(RedisCommands.Ping) == "PONG";
        }

        public string Echo(string text)
        {
            return SendExpectData(RedisCommands.Echo, text.ToUtf8Bytes()).FromUtf8Bytes();
        }

        public void SlaveOf(string hostname, int port)
        {
            SendExpectSuccess(RedisCommands.SlaveOf, hostname.ToUtf8Bytes(), port.ToUtf8Bytes());
        }

        public void SlaveOfNoOne()
        {
            SendExpectSuccess(RedisCommands.SlaveOf, RedisCommands.No, RedisCommands.One);
        }

        public byte[][] ConfigGet(string pattern)
        {
            return SendExpectMultiData(RedisCommands.Config, RedisCommands.Get, pattern.ToUtf8Bytes());
        }

        public void ConfigSet(string item, byte[] value)
        {
            SendExpectSuccess(RedisCommands.Config, RedisCommands.Set, item.ToUtf8Bytes(), value);
        }

        public void ConfigResetStat()
        {
            SendExpectSuccess(RedisCommands.Config, RedisCommands.ResetStat);
        }

        public byte[][] Time()
        {
            return SendExpectMultiData(RedisCommands.Time);
        }

        public void DebugSegfault()
        {
            SendExpectSuccess(RedisCommands.Debug, RedisCommands.Segfault);
        }

        public byte[] Dump(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectData(RedisCommands.Dump);
        }

        public byte[] Restore(string key, long expireMs, byte[] dumpValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectData(RedisCommands.Restore, key.ToUtf8Bytes(), expireMs.ToUtf8Bytes(), dumpValue);
        }

        public void Migrate(string host, int port, int destinationDb, long timeoutMs)
        {
            SendExpectSuccess(RedisCommands.Migrate, host.ToUtf8Bytes(), port.ToUtf8Bytes(), destinationDb.ToUtf8Bytes(), timeoutMs.ToUtf8Bytes());
        }

        public bool Move(string key, int db)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Move, key.ToUtf8Bytes(), db.ToUtf8Bytes()) == Success;
        }

        public long ObjectIdleTime(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Object, RedisCommands.IdleTime, key.ToUtf8Bytes());
        }

        public string Type(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectCode(RedisCommands.Type, key.ToUtf8Bytes());
        }

        public EnumRedisKeyType GetEntryType(string key)
        {
            switch (Type(key))
            {
                case "none":
                    return EnumRedisKeyType.None;
                case "string":
                    return EnumRedisKeyType.String;
                case "set":
                    return EnumRedisKeyType.Set;
                case "list":
                    return EnumRedisKeyType.List;
                case "zset":
                    return EnumRedisKeyType.SortedSet;
                case "hash":
                    return EnumRedisKeyType.Hash;
            }
            throw CreateResponseError("Invalid value");
        }

        public long StrLen(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.StrLen, key.ToUtf8Bytes());
        }

        public void Set(string key, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            value = value ?? new byte[0];

            if (value.Length > OneGb)
                throw new ArgumentException("value exceeds 1G", "value");

            SendExpectSuccess(RedisCommands.Set, key.ToUtf8Bytes(), value);
        }

        public void Set(string key, byte[] value, int expirySeconds, long expiryMs = 0, bool? exists = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            value = value ?? new byte[0];

            if (value.Length > OneGb)
                throw new ArgumentException("value exceeds 1G", "value");

            if (exists == null)
            {
                if (expirySeconds > 0)
                    SendExpectSuccess(RedisCommands.Set, key.ToUtf8Bytes(), value, RedisCommands.Ex, expirySeconds.ToUtf8Bytes());
                else if (expiryMs > 0)
                    SendExpectSuccess(RedisCommands.Set, key.ToUtf8Bytes(), value, RedisCommands.Px, expiryMs.ToUtf8Bytes());
                else
                    SendExpectSuccess(RedisCommands.Set, key.ToUtf8Bytes(), value);
            }
            else
            {
                var entryExists = exists.Value ? RedisCommands.Xx : RedisCommands.Nx;

                if (expirySeconds > 0)
                    SendExpectSuccess(RedisCommands.Set, key.ToUtf8Bytes(), value, RedisCommands.Ex, expirySeconds.ToUtf8Bytes(), entryExists);
                else if (expiryMs > 0)
                    SendExpectSuccess(RedisCommands.Set, key.ToUtf8Bytes(), value, RedisCommands.Px, expiryMs.ToUtf8Bytes(), entryExists);
                else
                    SendExpectSuccess(RedisCommands.Set, key.ToUtf8Bytes(), value, entryExists);
            }
        }

        public void SetEx(string key, int expireInSeconds, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            value = value ?? new byte[0];

            if (value.Length > OneGb)
                throw new ArgumentException("value exceeds 1G", "value");

            SendExpectSuccess(RedisCommands.SetEx, key.ToUtf8Bytes(), expireInSeconds.ToUtf8Bytes(), value);
        }

        public bool Persist(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Persist, key.ToUtf8Bytes()) == Success;
        }

        public void PSetEx(string key, long expireInMs, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            SendExpectSuccess(RedisCommands.PSetEx, key.ToUtf8Bytes(), expireInMs.ToUtf8Bytes(), value);
        }

        public long SetNX(string key, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            value = value ?? new byte[0];

            if (value.Length > OneGb)
                throw new ArgumentException("value exceeds 1G", "value");

            return SendExpectLong(RedisCommands.SetNx, key.ToUtf8Bytes(), value);
        }

        public void MSet(byte[][] keys, byte[][] values)
        {
            var keysAndValues = MergeCommandWithKeysAndValues(RedisCommands.MSet, keys, values);

            SendExpectSuccess(keysAndValues);
        }

        public void MSet(string[] keys, byte[][] values)
        {
            MSet(keys.ToMultiByteArray(), values);
        }

        public bool MSetNx(byte[][] keys, byte[][] values)
        {
            var keysAndValues = MergeCommandWithKeysAndValues(RedisCommands.MSet, keys, values);

            return SendExpectLong(keysAndValues) == Success;
        }

        public bool MSetNx(string[] keys, byte[][] values)
        {
            return MSetNx(keys.ToMultiByteArray(), values);
        }

        public byte[] Get(string key)
        {
            return GetBytes(key);
        }

        public object[] Slowlog(int? top)
        {
            if (top.HasValue)
                return SendExpectDeeplyNestedMultiData(RedisCommands.Slowlog, RedisCommands.Get, top.Value.ToUtf8Bytes());
            else
                return SendExpectDeeplyNestedMultiData(RedisCommands.Slowlog, RedisCommands.Get);
        }

        public void SlowlogReset()
        {
            SendExpectSuccess(RedisCommands.Slowlog, "RESET".ToUtf8Bytes());
        }

        public byte[] GetBytes(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectData(RedisCommands.Get, key.ToUtf8Bytes());
        }

        public byte[] GetSet(string key, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            value = value ?? new byte[0];

            if (value.Length > OneGb)
                throw new ArgumentException("value exceeds 1G", "value");

            return SendExpectData(RedisCommands.GetSet, key.ToUtf8Bytes(), value);
        }

        public long Exists(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Exists, key.ToUtf8Bytes());
        }

        public long Del(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Del, key.ToUtf8Bytes());
        }

        public long Del(params string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");

            var cmdWithArgs = MergeCommandWithArgs(RedisCommands.Del, keys);
            return SendExpectLong(cmdWithArgs);
        }

        public long Incr(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Incr, key.ToUtf8Bytes());
        }

        public long IncrBy(string key, int count)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.IncrBy, key.ToUtf8Bytes(), count.ToUtf8Bytes());
        }

        public long IncrBy(string key, long count)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return SendExpectLong(RedisCommands.IncrBy, key.ToUtf8Bytes(), count.ToUtf8Bytes());
        }

        public double IncrByFloat(string key, double incrBy)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectDouble(RedisCommands.IncrByFloat, key.ToUtf8Bytes(), incrBy.ToUtf8Bytes());
        }

        public long Decr(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Decr, key.ToUtf8Bytes());
        }

        public long DecrBy(string key, int count)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.DecrBy, key.ToUtf8Bytes(), count.ToUtf8Bytes());
        }

        public long Append(string key, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Append, key.ToUtf8Bytes(), value);
        }

        public byte[] GetRange(string key, int fromIndex, int toIndex)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectData(RedisCommands.GetRange, key.ToUtf8Bytes(), fromIndex.ToUtf8Bytes(), toIndex.ToUtf8Bytes());
        }

        public long SetRange(string key, int offset, byte[] value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.SetRange, key.ToUtf8Bytes(), offset.ToUtf8Bytes(), value);
        }

        public long GetBit(string key, int offset)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.GetBit, key.ToUtf8Bytes(), offset.ToUtf8Bytes());
        }

        public long SetBit(string key, int offset, int value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value > 1 || value < 0)
                throw new ArgumentException("value is out of range");

            return SendExpectLong(RedisCommands.SetBit, key.ToUtf8Bytes(), offset.ToUtf8Bytes(), value.ToUtf8Bytes());
        }

        public long BitCount(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.BitCount, key.ToUtf8Bytes());
        }

        public string RandomKey()
        {
            return SendExpectData(RedisCommands.RandomKey).FromUtf8Bytes();
        }

        public void Rename(string oldKeyname, string newKeyname)
        {
            if (oldKeyname == null)
                throw new ArgumentNullException("oldKeyname");
            if (newKeyname == null)
                throw new ArgumentNullException("newKeyname");

            SendExpectSuccess(RedisCommands.Rename, oldKeyname.ToUtf8Bytes(), newKeyname.ToUtf8Bytes());
        }

        public bool RenameNx(string oldKeyname, string newKeyname)
        {
            if (oldKeyname == null)
                throw new ArgumentNullException("oldKeyname");
            if (newKeyname == null)
                throw new ArgumentNullException("newKeyname");

            return SendExpectLong(RedisCommands.RenameNx, oldKeyname.ToUtf8Bytes(), newKeyname.ToUtf8Bytes()) == Success;
        }

        public bool Expire(string key, int seconds)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Expire, key.ToUtf8Bytes(), seconds.ToUtf8Bytes()) == Success;
        }

        public bool PExpire(string key, long ttlMs)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.PExpire, key.ToUtf8Bytes(), ttlMs.ToUtf8Bytes()) == Success;
        }

        public bool ExpireAt(string key, long unixTime)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.ExpireAt, key.ToUtf8Bytes(), unixTime.ToUtf8Bytes()) == Success;
        }

        public bool PExpireAt(string key, long unixTimeMs)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.PExpireAt, key.ToUtf8Bytes(), unixTimeMs.ToUtf8Bytes()) == Success;
        }

        public long Ttl(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.Ttl, key.ToUtf8Bytes());
        }

        public long PTtl(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return SendExpectLong(RedisCommands.PTtl, key.ToUtf8Bytes());
        }

        public void Save()
        {
            SendExpectSuccess(RedisCommands.Save);
        }

        public void SaveAsync()
        {
            BgSave();
        }

        public void BgSave()
        {
            SendExpectSuccess(RedisCommands.BgSave);
        }

        public void Shutdown()
        {
            SendCommand(RedisCommands.Shutdown);
        }

        public void BgRewriteAof()
        {
            SendExpectSuccess(RedisCommands.BgRewriteAof);
        }

        public void FlushDb()
        {
            SendExpectSuccess(RedisCommands.FlushDb);
        }

        public void FlushAll()
        {
            SendExpectSuccess(RedisCommands.FlushAll);
        }

        public string ClientGetName()
        {
            return SendExpectString(RedisCommands.Client, RedisCommands.GetName);
        }

        public void ClientSetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty");

            if (name.Contains(" "))
                throw new ArgumentException("Name cannot contain spaces");

            SendExpectSuccess(RedisCommands.Client, RedisCommands.SetName, name.ToUtf8Bytes());
        }

        public byte[] ClientList()
        {
            return SendExpectData(RedisCommands.Client, RedisCommands.List);
        }

        public void ClientKill(string clientAddr)
        {
            SendExpectSuccess(RedisCommands.Client, RedisCommands.Kill, clientAddr.ToUtf8Bytes());
        }

        public byte[][] Keys(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            return SendExpectMultiData(RedisCommands.Keys, pattern.ToUtf8Bytes());
        }

        public byte[][] MGet(params byte[][] keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");
            if (keys.Length == 0)
                throw new ArgumentException("keys");

            var cmdWithArgs = MergeCommandWithArgs(RedisCommands.MGet, keys);

            return SendExpectMultiData(cmdWithArgs);
        }

        public byte[][] MGet(params string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");
            if (keys.Length == 0)
                throw new ArgumentException("keys");

            var cmdWithArgs = MergeCommandWithArgs(RedisCommands.MGet, keys);

            return SendExpectMultiData(cmdWithArgs);
        }

        public void Watch(params string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");
            if (keys.Length == 0)
                throw new ArgumentException("keys");

            var cmdWithArgs = MergeCommandWithArgs(RedisCommands.Watch, keys);

            SendExpectCode(cmdWithArgs);
        }

        public void UnWatch()
        {
            SendExpectCode(RedisCommands.UnWatch);
        }

        internal void Multi()
        {
            //make sure socket is connected. Otherwise, fetch of server info will interfere
            //with pipeline
            AssertConnectedSocket();
            if (!SendCommand(RedisCommands.Multi))
                throw CreateConnectionError();
        }

        /// <summary>
        /// Requires custom result parsing
        /// </summary>
        /// <returns>Number of results</returns>
        internal void Exec()
        {
            if (!SendCommand(RedisCommands.Exec))
                throw CreateConnectionError();

        }

        internal void Discard()
        {
            SendExpectSuccess(RedisCommands.Discard);
        }

        public ScanResult Scan(ulong cursor, int count = 10, string match = null)
        {
            if (match == null)
                return SendExpectScanResult(RedisCommands.Scan, cursor.ToUtf8Bytes(),
                                            RedisCommands.Count, count.ToUtf8Bytes());

            return SendExpectScanResult(RedisCommands.Scan, cursor.ToUtf8Bytes(),
                                        RedisCommands.Match, match.ToUtf8Bytes(),
                                        RedisCommands.Count, count.ToUtf8Bytes());
        }

        public ScanResult SScan(string setId, ulong cursor, int count = 10, string match = null)
        {
            if (match == null)
            {
                return SendExpectScanResult(RedisCommands.SScan, setId.ToUtf8Bytes(), cursor.ToUtf8Bytes(),
                                            RedisCommands.Count, count.ToUtf8Bytes());
            }

            return SendExpectScanResult(RedisCommands.SScan, setId.ToUtf8Bytes(), cursor.ToUtf8Bytes(),
                                        RedisCommands.Match, match.ToUtf8Bytes(),
                                        RedisCommands.Count, count.ToUtf8Bytes());
        }

        public ScanResult ZScan(string setId, ulong cursor, int count = 10, string match = null)
        {
            if (match == null)
            {
                return SendExpectScanResult(RedisCommands.ZScan, setId.ToUtf8Bytes(), cursor.ToUtf8Bytes(),
                                            RedisCommands.Count, count.ToUtf8Bytes());
            }

            return SendExpectScanResult(RedisCommands.ZScan, setId.ToUtf8Bytes(), cursor.ToUtf8Bytes(),
                                        RedisCommands.Match, match.ToUtf8Bytes(),
                                        RedisCommands.Count, count.ToUtf8Bytes());
        }

        public ScanResult HScan(string hashId, ulong cursor, int count = 10, string match = null)
        {
            if (match == null)
            {
                return SendExpectScanResult(RedisCommands.HScan, hashId.ToUtf8Bytes(), cursor.ToUtf8Bytes(),
                                            RedisCommands.Count, count.ToUtf8Bytes());
            }

            return SendExpectScanResult(RedisCommands.HScan, hashId.ToUtf8Bytes(), cursor.ToUtf8Bytes(),
                                        RedisCommands.Match, match.ToUtf8Bytes(),
                                        RedisCommands.Count, count.ToUtf8Bytes());
        }


        internal ScanResult SendExpectScanResult(byte[] cmd, params byte[][] args)
        {
            var cmdWithArgs = MergeCommandWithArgs(cmd, args);
            var multiData = SendExpectDeeplyNestedMultiData(cmdWithArgs);
            var counterBytes = (byte[])multiData[0];

            var ret = new ScanResult
            {
                Cursor = ulong.Parse(counterBytes.FromUtf8Bytes()),
                Results = new List<byte[]>()
            };
            var keysBytes = (object[])multiData[1];

            foreach (var keyBytes in keysBytes)
            {
                ret.Results.Add((byte[])keyBytes);
            }

            return ret;
        }

        public bool PfAdd(string key, params byte[][] elements)
        {
            var cmdWithArgs = MergeCommandWithArgs(RedisCommands.PfAdd, key.ToUtf8Bytes(), elements);
            return SendExpectLong(cmdWithArgs) == 1;
        }

        public long PfCount(string key)
        {
            var cmdWithArgs = MergeCommandWithArgs(RedisCommands.PfCount, key.ToUtf8Bytes());
            return SendExpectLong(cmdWithArgs);
        }

        public void PfMerge(string toKeyId, params string[] fromKeys)
        {
            var fromKeyBytes = fromKeys.Map(x => x.ToUtf8Bytes()).ToArray();
            var cmdWithArgs = MergeCommandWithArgs(RedisCommands.PfMerge, toKeyId.ToUtf8Bytes(), fromKeyBytes);
            SendExpectSuccess(cmdWithArgs);
        }
        #endregion

        #endregion
    }
}