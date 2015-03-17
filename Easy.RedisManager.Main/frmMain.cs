using Easy.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Easy.RedisManager.Main
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        private readonly ILogger m_Logger = null;

        public FrmMain()
        {
            InitializeComponent();

            // 初始化日志记录
            m_Logger = LogFactory.CreateLogger(typeof(FrmMain)); 
        }

        /// <summary>
        /// Manage Connections Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManageConn_Click(object sender, EventArgs e)
        {
            m_Logger.Debug("btnManageConn_Click Start.");

            var pScreen = this.btnManageConn.PointToScreen(this.btnManageConn.Location);
            var point = new Point();
            point.X = pScreen.X;
            point.Y = pScreen.Y + this.btnManageConn.Size.Height;

            // The height of the WorkingArea
            int iActulaHeight = SystemInformation.WorkingArea.Height;
            if (point.Y + this.conMSManageConn.Size.Height > iActulaHeight)
            {
                point.X = pScreen.X - this.btnManageConn.Size.Width + 50;
                point.Y = pScreen.Y - this.btnManageConn.Size.Height - 30;
            }

            this.conMSManageConn.Show(point);
            m_Logger.Debug("btnManageConn_Click End.");
        }
    }
}
