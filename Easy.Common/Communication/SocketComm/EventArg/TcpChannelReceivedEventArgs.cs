using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common.Communication.SocketComm.EventArg
{
    public class TcpChannelReceivedEventArgs : EventArgs
    {
        public byte[] BtsReceived { private set; get; }

        public TcpChannelReceivedEventArgs(byte[] btsReceived)
        {
            BtsReceived = btsReceived;
        }

        public bool AreBytesAvailable
        {
            get { return BtsReceived != null && BtsReceived.Length > 0; }
        }

        public string AsciiStringReceived
        {
            get { return AreBytesAvailable ? Encoding.ASCII.GetString(BtsReceived) : null; }
        }

        public string UnicodeStringReceived
        {
            get { return AreBytesAvailable ? Encoding.Unicode.GetString(BtsReceived) : null; }
        }
    }
}
