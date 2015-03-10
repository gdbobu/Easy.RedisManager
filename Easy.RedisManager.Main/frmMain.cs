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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Manage Connections Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManageConn_Click(object sender, EventArgs e)
        {
            Point point = new Point();
            point.X = this.btnManageConn.Location.X;
            point.Y = this.btnManageConn.Location.Y - this.conMSManageConn.Size.Width;
            this.conMSManageConn.Show(this.btnManageConn, point);
        }
    }
}
