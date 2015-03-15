using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Easy.Common
{
    public class EasyTcpClient : TcpChannel
    {
        #region Const

        protected const int DEFAULT_RECONNECTION_ATTEMPTS = 0;
        protected const int DEFAULT_RECONNECTION_DELAY_IN_SEC = 15;

        #endregion // Const

        #region Var

        protected IPAddress m_RemoteIpAddress = null;
        protected int m_RemotePort = -1;

        protected int m_ReconnectionAttempts;
        protected TimeSpan m_DelayBetweenReconnectionAttempts;

        #endregion // Var

        #region Delegates

        public delegate bool TryReconnectDelegate(string id);
        public delegate bool IsReconnectionRequiredDelegate();

        #endregion // Delegates

        #region Events

        public event TcpChannelEventHandler<TcpChannelNotifyEventArgs> OnSocketClosureFailed;
        public event TcpChannelEventHandler<TcpChannelNotifyEventArgs> OnSocketConnectionFailed;
        public event TcpChannelEventHandler<TcpChannelNotifyEventArgs> OnConnectionAttempt;

        #endregion // Events

        #region Constructor

        public EasyTcpClient(TcpChannelEventHandler<TcpChannelReceivedEventArgs> onReceived,
                         string id = null,
                         int receiveTimeoutInSec = DEFAULT_RECEIVE_TIMEOUT_IN_SEC,
                         int reconnectionAttempts = DEFAULT_RECONNECTION_ATTEMPTS,
                         int delayBetweenReconnectionAttemptsInSec = DEFAULT_RECONNECTION_DELAY_IN_SEC,
                         int socketReceiveTimeoutInSec = DEFAULT_RECEIVE_TIMEOUT_IN_SEC,
                         int socketSendTimeoutInSec = DEFAULT_SEND_TIMEOUT_IN_SEC,
                         int socketReceiveBufferSize = DEFAULT_RECEIVE_BUFFER_SIZE,
                         int socketSendBufferSize = DEFAULT_SEND_BUFFER_SIZE) :
            base(onReceived, id, socketReceiveTimeoutInSec, socketSendTimeoutInSec, socketReceiveBufferSize, socketSendBufferSize)
        {
            dlgtReconnect = new ReconnectDelegate(Connect);

            this.m_ReconnectionAttempts = reconnectionAttempts;
            if (this.m_ReconnectionAttempts <= 0)
                this.m_ReconnectionAttempts = 1;

            this.m_DelayBetweenReconnectionAttempts = new TimeSpan(0, 0, delayBetweenReconnectionAttemptsInSec);
        }

        #endregion // Constructors

        #region Connect

        public bool Connect(string url, int remotePort)
        {
            return Connect(GetIPAddress(url), remotePort);
        }

        public bool Connect(IPAddress remoteIpAddress, int remotePort)
        {
            this.m_RemoteIpAddress = remoteIpAddress;
            this.m_RemotePort = remotePort;
            return Connect();
        }

        public bool Connect()
        {
            if (m_RemoteIpAddress != null && m_RemotePort > 0)
            {
                if (socket != null)
                {
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception e)
                    {
                        if (OnSocketClosureFailed != null)
                            OnSocketClosureFailed(this, new TcpChannelNotifyEventArgs(EnumNotificationLevel.Error, "Connect", null, e));
                    }

                    socket = null;
                }

                for (int i = 0; !IsSocketConnected && i < m_ReconnectionAttempts && !IsFinishing; i++)
                {
                    if (OnConnectionAttempt != null)
                        OnConnectionAttempt(this, new TcpChannelNotifyEventArgs(
                                    EnumNotificationLevel.Info, "Connect", i));

                    try
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        SetSocketParams();
                        socket.Connect(m_RemoteIpAddress, m_RemotePort);

                        RemoteEP = socket.RemoteEndPoint as IPEndPoint;
                    }
                    catch (Exception e)
                    {
                        socket = null;

                        if (OnSocketConnectionFailed != null)
                            OnSocketConnectionFailed(this, new TcpChannelNotifyEventArgs(EnumNotificationLevel.Error, "Connect", null, e));

                        evForReconnect.WaitOne(m_DelayBetweenReconnectionAttempts);
                    }
                }

                if (IsSocketConnected)
                    Receive();
            }

            return IsSocketConnected;
        }

        #endregion // Connect
    }
}
