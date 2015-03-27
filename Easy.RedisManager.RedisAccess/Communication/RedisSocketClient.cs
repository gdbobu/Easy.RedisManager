using Easy.Common;
using Easy.RedisManager.RedisAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Easy.RedisManager.RedisAccess.Communication
{
    /// <summary>
    /// Redis通信客户端（Socket版）
    /// </summary>
    public class RedisSocketClient : IDisposable
    {
        #region Constant

        public const int DefaultPort = 6379;
        public const string DefaultHost = "localhost";
        public const int DefaultIdleTimeOutSecs = 240; //default on redis is 300

        #endregion

        #region Variable

        // 记录日志类
        private static readonly ILogger s_Logger = LogFactory.CreateLogger(typeof (RedisSocketClient));
        // 命令缓存
        private readonly IList<ArraySegment<byte>> _cmdBuffer = new List<ArraySegment<byte>>();
        // 结束符
        private readonly byte[] _endData = new[] { (byte)'\r', (byte)'\n' };

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
        // 当前缓存
        private byte[] _currentBuffer = BufferPool.GetBuffer();

        // 当前缓存的的索引
        private int _currentBufferIndex;
        // 最近一次连接的时间
        internal long _lastConnectedAtTimestamp;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public RedisSocketClient()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public RedisSocketClient(string host, int port)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public RedisSocketClient(string host, int port, string password)
            :this(host, port)
        {
            Password = password;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <param name="connectTimeout"></param>
        public RedisSocketClient(string host, int port, string password, int connectTimeout)
            : this(host, port, password)
        {
            ConnectTimeout = connectTimeout;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <param name="connectTimeout"></param>
        /// <param name="sendTimeout"></param>
        /// <param name="receiveTimeout"></param>
        /// <param name="idleTimeOutSecs"></param>
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
        /// 最近一次的命令
        /// </summary>
        public string LastCommand { get; private set; }

        /// <summary>
        /// 最近一次SocketException
        /// </summary>
        public SocketException LastSocketException { get; private set; }

        /// <summary>
        /// 是否释放资源
        /// </summary>
        internal bool IsDisposed { get; set; }

        #endregion

        #region Method

        #region Connect

        /// <summary>
        /// The function of Redis Connected
        /// </summary>
        public virtual void OnConnected()
        {
        }

        /// <summary>
        /// Connect
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

                // Init 16KB Buffer
                _buffStream = new BufferedStream(new NetworkStream(_redisSocket), 16 * 1024);

                var ipEndpoint = _redisSocket.LocalEndPoint as IPEndPoint;
                _clientPort = ipEndpoint != null ? ipEndpoint.Port : -1;
                LastCommand = null;
                LastSocketException = null;
                _lastConnectedAtTimestamp = Stopwatch.GetTimestamp();

                // Do after Redis Connected
                OnConnected();
            }
            catch (Exception ex)
            {
                s_Logger.Error("Error when trying to Connect().ErrorMessage:" + ex.Message);
                s_Logger.Error("Error when trying to Connect().ErrorMessage:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Assert Redis is Connected
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
        public bool ReConnect()
        {
            SafeConnectionClose();
            Connect(); 

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
                s_Logger.Error("Error when trying to BufferStaream Close().ErrorMessage:" + ex.Message);
                s_Logger.Error("Error when trying to BufferStaream Close().ErrorMessage:" + ex.StackTrace);
            }

            try
            {
                if (_redisSocket != null)
                    _redisSocket.Close();
            }
            catch (Exception ex)
            {
                s_Logger.Error("Error when trying to Socket Close().ErrorMessage:" + ex.Message);
                s_Logger.Error("Error when trying to Socket Close()..ErrorMessage:" + ex.StackTrace);
            }
            _buffStream = null;
            _redisSocket = null;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //dispose un managed resources
                DisposeConnection();
            }
        }

        /// <summary>
        /// Release resource
        /// </summary>
        internal void DisposeConnection()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            if (_redisSocket == null) return;

            try
            {
                SafeConnectionClose();
            }
            catch (Exception ex)
            {
                s_Logger.Error("Error when trying to Quit().ErrorMessage:" + ex.Message);
                s_Logger.Error("Error when trying to Quit().ErrorMessage:" + ex.StackTrace);
            }
            finally
            {
            }
        }

        ~RedisSocketClient()
        {
            Dispose(false);
        }
        #endregion

        #region Send
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
        private void PushCurrentBufferAndGetNewBuffer()
        {
            _cmdBuffer.Add(new ArraySegment<byte>(_currentBuffer, 0, _currentBufferIndex));
            _currentBuffer = BufferPool.GetBuffer();
            _currentBufferIndex = 0;
        }

        /// <summary>
        /// 写数据到发送缓冲区
        /// </summary>
        /// <param name="cmdBytes"></param>
        private void WriteToSendBuffer(byte[] cmdBytes)
        {
            if (CouldAddCurrentBuffer((cmdBytes))) return;

            PushCurrentBufferAndGetNewBuffer();

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
            // 把字节数组的长度写入缓存
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

        /// <summary>
        /// reset buffer index in send buffer
        /// </summary>
        private void ResetSendBuffer()
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
        /// 把缓存中的数据推送到Socket发送缓存
        /// </summary>
        /// <returns></returns>
        private bool FlushSendBuffer()
        {
            try
            {
                if (_currentBufferIndex > 0)
                    PushCurrentBufferAndGetNewBuffer();

                // Socket的Send方法，并非大家想象中的从一个端口发送消息到另一个端口，
                // 它仅仅是拷贝数据到基础系统的发送缓冲区，然后由基础系统将发送缓冲区的数据到连接的另一端口。
                // 值得一说的是，这里的拷贝数据与异步发送消息的拷贝是不一样的，
                // 同步发送的拷贝，是直接拷贝数据到基础系统缓冲区，拷贝完成后返回，在拷贝的过程中，执行线程会IO等待,
                // 此种拷贝与Socket自带的Buffer空间无关，
                // 但异步发送消息的拷贝，是将Socket自带的Buffer空间内的所有数据，拷贝到基础系统发送缓冲区，并立即返回，
                // 执行线程无需IO等待，所以异步发送在发送前必须执行SetBuffer方法，拷贝完成后，会触发你自定义回调函数ProcessSend，
                // 在ProcessSend方法中，调用SetBuffer方法，重新初始化Buffer空间。
                _redisSocket.Send(_cmdBuffer); // Optimized for Windows

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
        ///     Command to set multuple binary safe arguments
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        public bool SendCommand(params byte[][] cmdWithBinaryArgs)
        {
            if (!AssertConnectedSocket()) return false;

            Interlocked.Increment(ref _requestsPerHour);

            CmdLog(cmdWithBinaryArgs);

            //Total command lines count
            WriteAllToSendBuffer(cmdWithBinaryArgs);

            return FlushSendBuffer();
        }
        #endregion

        #region Read
        /// <summary>
        /// 安全模式读取一个字节
        /// </summary>
        /// <returns></returns>
        public int SafeReadByte()
        {
            return _buffStream.ReadByte();
        }

        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
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

            LastCommand = sb.ToString();
            if (LastCommand.Length > 100)
                LastCommand = LastCommand.Substring(0, 100) + "...";

            s_Logger.Debug("S: " + LastCommand);
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

            s_Logger.Error("SocketException: " + ex.Message);
            s_Logger.Error("SocketException: " + ex.StackTrace);

            LastSocketException = ex;

            // timeout?
            _redisSocket.Close();
            _redisSocket = null;

            return false;
        }

        #endregion

        #endregion
    }
}