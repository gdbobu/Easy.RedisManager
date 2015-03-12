using Easy.Common.Communication.SocketComm.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common.Communication.SocketComm.EventArg
{
    public class TcpChannelNotifyEventArgs : EventArgs
    {

        public EnumNotificationLevel Level { private set; get; }
        public string MethodName { private set; get; }
        public object Data { private set; get; }
        public Exception Ex { private set; get; }

        public TcpChannelNotifyEventArgs(EnumNotificationLevel notifLevel
            , string methodName, object data = null, Exception e = null)
        {
            Level = notifLevel;
            MethodName = methodName;
            Data = data;
            Ex = e;
        }
    }
}
