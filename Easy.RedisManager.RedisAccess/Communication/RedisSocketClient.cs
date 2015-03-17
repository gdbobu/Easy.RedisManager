using Easy.Common;
using Easy.RedisManager.RedisAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Easy.RedisManager.Entity.Config;

namespace Easy.RedisManager.RedisAccess.Communication
{
    /// <summary>
    /// Redis通信客户端（Socket版）
    /// </summary>
    public class RedisSocketClient : IDisposable
    {
        #region Constant

        private const int UnKnown = -1;
        public const long DefaultDb = 0;
        public const int DefaultPort = 6379;
        public const string DefaultHost = "localhost";
        public const int DefaultIdleTimeOutSecs = 240; //default on redis is 300

        #endregion

        #region Variable
        // Socket实例
        protected Socket _redisSocket = null;
        // 接收数据的缓存
        protected BufferedStream _buffStream = null;
        // Timer类的实例
        private Timer _usageTimer = null;
        // 每小时请求的次数
        private int _requestsPerHour = 0;
        // 记录日志类
        private static readonly ILogger s_logger = LogFactory.CreateLogger(typeof(RedisSocketClient));
        // 客户端的端口号
        private int _clientPort;
        // 最近一次的命令
        private string _lastCommand;
        // 最近一次SocketException
        private SocketException _lastSocketException;
        // 最近一次连接的时间
        internal long _lastConnectedAtTimestamp;
        #endregion

        #region Property
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

        private long _db;
        public long Db
        {
            get
            {
                return _db;
            }

            set
            {
                _db = value;
            }
        }

        #endregion

        #region Method

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
            if (this._usageTimer == null)
            {
                this._usageTimer = new Timer(delegate
                {
                    this._requestsPerHour = 0;
                }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromHours(1));
            }

            // 初始化Socket
            this._redisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
               SendTimeout =  SendTimeout,
               ReceiveTimeout =  ReceiveTimeout
            };

            try
            {
                if (ConnectTimeout == 0)
                {
                    this._redisSocket.Connect(Host, Port);
                }
                else
                {
                    var connectResult = this._redisSocket.BeginConnect(Host, Port, null, null);
                    // 阻塞当前进程，直到收到信号位置，或者到了超时时间后，退出
                    connectResult.AsyncWaitHandle.WaitOne(ConnectTimeout, true);
                }

                if (!this._redisSocket.Connected)
                {
                    this._redisSocket.Close();
                    this._redisSocket.Dispose();
                    this._redisSocket = null;
                    HadExceptions = true;
                    return;
                }

                // 初始化16KB的缓存
                _buffStream = new BufferedStream(new NetworkStream(this._redisSocket), 16 * 1024);

                if(!string.IsNullOrEmpty(Password))

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 确保Socket是否连接
        /// </summary>
        /// <returns></returns>
        private bool AssertConnectedSocket()
        {
            if (this._lastConnectedAtTimestamp > 0)
            {
                var now = Stopwatch.GetTimestamp();
                // 获取间隔的秒数
                var elapsedSecs = (now - this._lastConnectedAtTimestamp)/Stopwatch.Frequency;

                if (this._redisSocket == null || (elapsedSecs > IdleTimeOutSecs && !this._redisSocket.IsConnected()))
                {
                    return ReConnect();
                }

                this._lastConnectedAtTimestamp = now;
            }

            if (this._redisSocket == null)
            {
                Connect();
            }

            return this._redisSocket != null;
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <returns></returns>
        private bool ReConnect()
        {
            var previousDb = this._db;

            SafeConnectionClose();
            Connect(); //sets db to 0

            if (previousDb != DefaultDb) this._db = previousDb;
            return this._redisSocket != null;
        }

        public void Start()
        {
        }

        public void Close()
        {
        }
        private void SafeConnectionClose()
        {
            try
            {
                // workaround for a .net bug: http://support.microsoft.com/kb/821625
                if (this._buffStream != null)
                    _buffStream.Close();
            }
            catch { }

            try
            {
                if (this._redisSocket != null)
                    this._redisSocket.Close();
            }
            catch { }
            this._buffStream = null;
            this._redisSocket = null;
        }

        public void Send()
        {
            
        }

        /// <summary>
        /// Command to set multuple binary safe arguments
        /// </summary>
        /// <param name="cmdWithBinaryArgs"></param>
        /// <returns></returns>
        protected bool SendCommand(params byte[][] cmdWithBinaryArgs)
        {
            if (!AssertConnectedSocket()) return false;

            Interlocked.Increment(ref _requestsPerHour);
            //if (_requestsPerHour % 20 == 0)
            //    LicenseUtils.AssertValidUsage(LicenseFeature.Redis, QuotaType.RequestsPerHour, __requestsPerHour);

            CmdLog(cmdWithBinaryArgs);

            //Total command lines count
            WriteAllToSendBuffer(cmdWithBinaryArgs);

            //pipeline will handle flush, if pipelining is turned on
            //if (Pipeline == null)
            //    return FlushSendBuffer();

            return true;
        }
        
        internal bool IsDisposed { get; set; }

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

        internal void DisposeConnection()
        {
			if (IsDisposed) return;
            IsDisposed = true;

            if (this._redisSocket == null) return;

            try
            {
                Quit();
            }
            catch (Exception ex)
            {
                s_logger.Error("Error when trying to Quit()", ex);
            }
            finally
            {
                SafeConnectionClose();
            }
        }

        #region Business Method
        #endregion

        #region Log Method
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="args"></param>
        protected void CmdLog(byte[][] args)
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(arg.FromUtf8Bytes());
            }
            this._lastCommand = sb.ToString();
            if (this._lastCommand.Length > 100)
                this._lastCommand = this._lastCommand.Substring(0, 100) + "...";

            s_logger.Debug("S: " + this._lastCommand);
        }
        #endregion

        #region Exception
        /// <summary>
        /// 生成ResponseError
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private RedisResponseException CreateResponseError(string error)
        {
            HadExceptions = true;
            var throwEx = new RedisResponseException(
                string.Format("{0}, Port: {1}, LastCommand: {2}", error, this._clientPort, this._lastCommand));
            s_logger.Error(throwEx.Message);
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
                string.Format("Unable to Connect: Port: {0}", this._clientPort), this._lastSocketException);
            s_logger.Error(throwEx.Message);
            throw throwEx;
        }
        #endregion

        #endregion
    }
}
