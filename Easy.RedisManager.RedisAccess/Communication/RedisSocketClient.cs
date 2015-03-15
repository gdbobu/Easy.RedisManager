using Easy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Easy.RedisManager.RedisAccess.Communication
{
    /// <summary>
    /// Redis通信客户端（Socket版）
    /// </summary>
    public class RedisSocketClient:IDisposable
    {
        #region Constant
        #endregion

        #region Variable
        private EasyTcpClient m_TcpClient = null;
        #endregion

        #region Method
        public void Start()
        {
        }

        public void Close()
        {
        }

        public void Send()
        {
            
        }

        public void Dispose()
        {
        }
        #endregion

    }
}
