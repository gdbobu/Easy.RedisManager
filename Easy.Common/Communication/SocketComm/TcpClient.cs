using Easy.Common.Communication.SocketComm.Enums;
using Easy.Common.Communication.SocketComm.EventArg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Easy.Common.Communication.SocketComm
{
    public class TcpClient : TcpChannel
    {
        #region Const

        protected const int DEFAULT_RECONNECTION_ATTEMPTS = 0;
        protected const int DEFAULT_RECONNECTION_DELAY_IN_SEC = 15;

        #endregion // Const

        #region Var

        protected IPAddress remoteIpAddress = null;
        protected int remotePort = -1;

        protected int reconnectionAttempts;
        protected TimeSpan delayBetweenReconnectionAttempts;

        #endregion // Var

        #region Delegates

        public delegate bool TryReconnectDelegate(string id);
        public delegate bool IsReconnectionRequiredDelegate();

        #endregion // Delegates

        #region Events

        public event TcpChannelEventHandler<TcpChannelNotifyEventArgs> onSocketClosureFailed;
        public event TcpChannelEventHandler<TcpChannelNotifyEventArgs> onSocketConnectionFailed;
        public event TcpChannelEventHandler<TcpChannelNotifyEventArgs> onConnectionAttempt;

        #endregion // Events

        #region Constructor

        public TcpClient(TcpChannelEventHandler<TcpChannelReceivedEventArgs> onReceived,
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

            this.reconnectionAttempts = reconnectionAttempts;
            if (this.reconnectionAttempts <= 0)
                this.reconnectionAttempts = 1;

            this.delayBetweenReconnectionAttempts = new TimeSpan(0, 0, delayBetweenReconnectionAttemptsInSec);
        }

        #endregion // Constructors

        #region Connect

        public bool Connect(string url, int remotePort)
        {
            return Connect(GetIPAddress(url), remotePort);
        }

        public bool Connect(IPAddress remoteIpAddress, int remotePort)
        {
            this.remoteIpAddress = remoteIpAddress;
            this.remotePort = remotePort;
            return Connect();
        }

        public bool Connect()
        {
            if (remoteIpAddress != null && remotePort > 0)
            {
                if (socket != null)
                {
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception e)
                    {
                        if (onSocketClosureFailed != null)
                            onSocketClosureFailed(this, new TcpChannelNotifyEventArgs(EnumNotificationLevel.Error, "Connect", null, e));
                    }

                    socket = null;
                }

                for (int i = 0; !IsSocketConnected && i < reconnectionAttempts && !IsFinishing; i++)
                {
                    if (onConnectionAttempt != null)
                        onConnectionAttempt(this, new TcpChannelNotifyEventArgs(
                                    EnumNotificationLevel.Info, "Connect", i));

                    try
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        SetSocketParams();
                        socket.Connect(remoteIpAddress, remotePort);

                        RemoteEP = socket.RemoteEndPoint as IPEndPoint;
                    }
                    catch (Exception e)
                    {
                        socket = null;

                        if (onSocketConnectionFailed != null)
                            onSocketConnectionFailed(this, new TcpChannelNotifyEventArgs(EnumNotificationLevel.Error, "Connect", null, e));

                        evForReconnect.WaitOne(delayBetweenReconnectionAttempts);
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
