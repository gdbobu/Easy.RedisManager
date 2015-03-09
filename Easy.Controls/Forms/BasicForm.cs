using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Easy.Controls;
using Easy.Controls.Properties;

namespace Easy.Controls
{
    public partial class BasicForm : Form
    {
        private Graphics g = null;
        private Bitmap Bmp = null;
        private Bitmap closeBmp = null;
        private Bitmap minBmp = null;
        private Bitmap maxBmp = null;
        private ImageAttributes imageAttr = null;
        private Brush titleColor = Brushes.White;
        private ContextMenu taskMenu = null;
        private MenuItem item0, item1, item2, item3, item4, item5, item6 = null;
        private int xwidth = 0;

        private bool m_ShowColorButton = true;
        public int colorA = 0;
        public int lightB = 0;
        public int grayC = 0;

        public BasicForm()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            closeBmp = ResClass.GetResObj("btn_close_normal");
            minBmp = ResClass.GetResObj("btn_mini_normal");
            maxBmp = ResClass.GetResObj("btn_max_normal");
            if (this.WindowState == FormWindowState.Maximized)
            {
                maxBmp = ResClass.GetResObj("btn_restore_normal");
            }
        }

        [Description("是否显示换色按钮"), Category("Appearance")]
        public bool ShowColorButton
        {
            get
            {
                return m_ShowColorButton;
            }
            set
            {
                m_ShowColorButton = value;
                colour.Visible = value;
            }
        }

        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();

        protected override void OnPaint(PaintEventArgs e)
        {
            g = e.Graphics;
            Bmp = ResClass.GetResObj("window");
            Bmp = (Bitmap)Util.ProcImage(Bmp, colorA, grayC, lightB);
            imageAttr = new ImageAttributes();
            imageAttr.SetColorKey(Bmp.GetPixel(1, 1), Bmp.GetPixel(1, 1));
            g.DrawImage(Bmp, new Rectangle(1, 1, 5, 31), 0, 0, 5, 31, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(6, 1, this.Width-12, 31), 5, 0, Bmp.Width-10, 31, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(this.Width - 6, 1, 5, 31), Bmp.Width-5, 0, 5, 31, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(1, 32, 5, this.Height - 38), 0, 31, 5, Bmp.Height - 36, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(6, 32, this.Width - 10, this.Height - 38), 5, 31, 5, Bmp.Height - 36, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(this.Width - 6, 32, 5, this.Height - 38), Bmp.Width - 5, 31, 5, Bmp.Height - 36, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(1, this.Height - 6, 5, 5), 0,Bmp.Height - 5, 5, 5, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(6, this.Height - 6, this.Width-12, 5), 5, Bmp.Height - 5, Bmp.Width-10, 5, GraphicsUnit.Pixel);
            g.DrawImage(Bmp, new Rectangle(this.Width - 6, this.Height - 6, 5, 5), Bmp.Width - 5, Bmp.Height - 5, 5, 5, GraphicsUnit.Pixel);
            //g.DrawImage(closeBmp, new Rectangle(this.Width - closeBmp.Width-1, 0, closeBmp.Width, closeBmp.Height), 0, 0, closeBmp.Width, closeBmp.Height, GraphicsUnit.Pixel);
            g.DrawIcon(this.Icon, new Rectangle(7, 7, 18, 18));
            g.DrawString(this.Text, new Font("微软雅黑", 10F, FontStyle.Bold), titleColor, 26, 7);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int Rgn = Win32.CreateRoundRectRgn(1, 1, this.Width, this.Height, 9, 8);
            Win32.SetWindowRgn(this.Handle, Rgn, true);

            if (this.ControlBox)
            {
                this.ButtonClose.Left = this.Width - ButtonClose.Width - 2;
            }

            if (this.MinimizeBox)
            {
                if (this.MaximizeBox)
                {
                    this.ButtonMax.Left = this.Width - ButtonClose.Width - ButtonMax.Width - 1;
                    this.ButtonMin.Left = this.Width - ButtonClose.Width - ButtonMax.Width - ButtonMin.Width;
                    xwidth = this.Width - ButtonClose.Width - ButtonMax.Width - ButtonMin.Width;
                }
                else
                {
                    this.ButtonMax.Enabled = false;
                    xwidth = this.Width - ButtonClose.Width - ButtonMin.Width;
                    this.ButtonMin.Left = this.Width - ButtonClose.Width - ButtonMin.Width;
                }
                colour.Visible = true;
                colour.Left = ButtonMin.Left - 25;
            }
            else
            {

                colour.Visible = false;
                xwidth = this.Width - ButtonClose.Width;
            }
            colour.Visible = ShowColorButton;
            this.Invalidate();
        }

        private void ButtonClose_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.Close();
        }

        private void ButtonClose_MouseDown(object sender, MouseEventArgs e)
        {
            closeBmp = ResClass.GetResObj("btn_close_down");
            ButtonClose.Invalidate();
        }

        private void ButtonClose_MouseUp(object sender, MouseEventArgs e)
        {
            if (!ButtonClose.IsDisposed)
            {
                closeBmp = ResClass.GetResObj("btn_close_normal");
                ButtonClose.Invalidate();
            }
        }

        private void ButtonClose_MouseLeave(object sender, EventArgs e)
        {
            closeBmp = ResClass.GetResObj("btn_close_normal");
            ButtonClose.Invalidate();
        }

        private void ButtonClose_MouseEnter(object sender, EventArgs e)
        {
            closeBmp = ResClass.GetResObj("btn_close_highlight");
            toolTip1.SetToolTip(ButtonClose, "关闭");
            ButtonClose.Invalidate();
        }

        private void ButtonMin_MouseEnter(object sender, EventArgs e)
        {
            minBmp = ResClass.GetResObj("btn_mini_highlight");
            toolTip1.SetToolTip(ButtonMin, "最小化");
            ButtonMin.Invalidate();
        }

        private void ButtonMin_MouseDown(object sender, MouseEventArgs e)
        {
            minBmp = ResClass.GetResObj("btn_mini_down");
            ButtonMin.Invalidate();
        }

        private void ButtonMin_MouseUp(object sender, MouseEventArgs e)
        {
            minBmp = ResClass.GetResObj("btn_close_normal");
            ButtonMin.Invalidate();
        }

        private void ButtonMin_MouseLeave(object sender, EventArgs e)
        {
            minBmp = ResClass.GetResObj("btn_mini_normal");
            ButtonMin.Invalidate();
        }

        private void ButtonMin_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.WindowState = FormWindowState.Minimized;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (skinPanel.Visible)
                {
                    skinPanel.Hide();
                    colour_MouseLeave(null, null);
                }
                this.Focus();
                //if (e.Y <= 31 && e.X < xwidth)
                //{
                    if (e.Clicks >= 2 && this.MaximizeBox)
                    {
                        ButtonMax_MouseClick(null, new MouseEventArgs(MouseButtons.Left, 1, 2, 2, 0));
                    }
                    else
                    {
                        ReleaseCapture();
                        SendMessage(Handle, 0x00A1, 2, 0);
                    }
                //}
            }
        }

        private void ButtonMin_Paint(object sender, PaintEventArgs e)
        {
            if (this.MinimizeBox)
            {
                g = e.Graphics;
                g.DrawImage(minBmp, new Rectangle(0, 0, minBmp.Width, minBmp.Height), 0, 0, minBmp.Width, minBmp.Height, GraphicsUnit.Pixel, imageAttr);
            }
        }

        private void ButtonClose_Paint(object sender, PaintEventArgs e)
        {
            if (this.ControlBox)
            {
                g = e.Graphics;
                g.DrawImage(closeBmp, new Rectangle(0, 0, closeBmp.Width, closeBmp.Height), 0, 0, closeBmp.Width, closeBmp.Height, GraphicsUnit.Pixel, imageAttr);
            }
        }

        private void ButtonMax_MouseDown(object sender, MouseEventArgs e)
        {
            g = ButtonMax.CreateGraphics();
            maxBmp = ResClass.GetResObj("btn_max_down");
            if (this.WindowState == FormWindowState.Maximized)
            {
                maxBmp = ResClass.GetResObj("btn_restore_down");
            }
            g.DrawImage(maxBmp, new Rectangle(0, 0, maxBmp.Width, maxBmp.Height), 0, 0, maxBmp.Width, maxBmp.Height, GraphicsUnit.Pixel);
        }

        private void ButtonMax_MouseEnter(object sender, EventArgs e)
        {
            g = ButtonMax.CreateGraphics();
            maxBmp = ResClass.GetResObj("btn_max_highlight");
            toolTip1.SetToolTip(ButtonMax, "最大化");
            if (this.WindowState == FormWindowState.Maximized)
            {
                maxBmp = ResClass.GetResObj("btn_restore_highlight");
                toolTip1.SetToolTip(ButtonMax, "还原");
            }
            g.DrawImage(maxBmp, new Rectangle(0, 0, maxBmp.Width, maxBmp.Height), 0, 0, maxBmp.Width, maxBmp.Height, GraphicsUnit.Pixel);
        }

        private void ButtonMax_MouseLeave(object sender, EventArgs e)
        {
            maxBmp = ResClass.GetResObj("btn_max_normal");
            if (this.WindowState == FormWindowState.Maximized)
            {
                maxBmp = ResClass.GetResObj("btn_restore_normal");
            }
            ButtonMax.Invalidate();
        }

        private void ButtonMax_MouseUp(object sender, MouseEventArgs e)
        {
            maxBmp = ResClass.GetResObj("btn_max_normal");
            if (this.WindowState == FormWindowState.Maximized)
            {
                maxBmp = ResClass.GetResObj("btn_restore_normal");
            }
            ButtonMax.Invalidate();
        }

        private void ButtonMax_Paint(object sender, PaintEventArgs e)
        {
            if (this.MaximizeBox)
            {
                g = e.Graphics;
                maxBmp = ResClass.GetResObj("btn_max_normal");
                if (this.WindowState == FormWindowState.Maximized)
                {
                    maxBmp = ResClass.GetResObj("btn_restore_normal");
                }
                g.DrawImage(maxBmp, new Rectangle(0, 0, maxBmp.Width, maxBmp.Height), 0, 0, maxBmp.Width, maxBmp.Height, GraphicsUnit.Pixel);
            }
        }

        private void IForm_Load(object sender, EventArgs e)
        {
            if (this.ControlBox)
            {
                this.ButtonClose.Visible = true;
                if (!this.MinimizeBox)
                    this.ButtonMin.Visible = false;
                else
                    this.ButtonMin.Visible = true;
                if (!this.MaximizeBox)
                    this.ButtonMax.Visible = false;
                else
                    this.ButtonMax.Visible = true;
            }
            else
            {
                this.ButtonClose.Visible = false;
                this.ButtonMin.Visible = false;
                this.ButtonMax.Visible = false;
            }
        }

        private void IForm_TextChanged(object sender, EventArgs e)
        {
            this.Invalidate(new Rectangle(26, 7,300,31));
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            titleColor = Brushes.White;
            this.Invalidate(new Rectangle(26, 7, 300, 31));
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            titleColor = Brushes.WhiteSmoke;
            this.Invalidate(new Rectangle(26, 7, 300, 31));
        }
        
        private void topPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                if (taskMenu == null)
                {
                    taskMenu = new ContextMenu();
                    taskMenu.Name = "taskMenu";

                    item0 = new MenuItem("还原(&R)");
                    item0.Name = "resetMenu";
                    item0.Index = 1;
                    item0.Click += new EventHandler(item0_Click);

                    item1 = new MenuItem("最小化(&N)");
                    item1.Name = "minMenu";
                    item1.Index = 2;
                    item1.Click += new EventHandler(item1_Click);

                    item2 = new MenuItem("最大化(&X)");
                    item2.Name = "maxMenu";
                    item2.Index = 3;
                    item2.Click += new EventHandler(item2_Click);

                    item3 = new MenuItem("关闭(&C)");
                    item3.Name = "closeMenu";
                    item3.Index = 5;
                    item3.ShowShortcut = true;
                    item3.Shortcut = Shortcut.AltF4;
                    item3.Click += new EventHandler(item3_Click);

                    item4 = new MenuItem();
                    item4.Name = "fgf";
                    item4.Index = 4;
                    item4.Text = "-";

                    item5 = new MenuItem("大小(&S)");
                    item5.Name = "sizeMenu";
                    item5.Enabled = false;
                    item5.Index = 6;
                    item5.Click += new EventHandler(item4_Click);

                    item6 = new MenuItem("移动(&M)");
                    item6.Name = "moveMenu";
                    item6.Enabled = false;
                    item6.Index = 7;
                    item6.Click += new EventHandler(item5_Click);

                    item3.DefaultItem = true;

                    if (!this.MinimizeBox)
                    {
                        item1.Enabled = false;
                    }
                    if (!this.MaximizeBox)
                    {
                        item2.Enabled = false;
                        item0.Enabled = false;
                    }
                    else
                    {
                        if (this.WindowState == FormWindowState.Normal)
                        {
                            item2.Enabled = true;
                            item0.Enabled = false;
                        }
                        else
                        {
                            item2.Enabled = false;
                            item0.Enabled = true;
                        }
                    }

                    taskMenu.MenuItems.AddRange(new MenuItem[] { item0, item1, item2, item4, item3 });
                    taskMenu.Show(this, new Point(e.X+5, e.Y), LeftRightAlignment.Right);
                }
                else 
                {
                    if (!this.MinimizeBox)
                    {
                        item1.Enabled = false;
                    }
                    if (!this.MaximizeBox)
                    {
                        item2.Enabled = false;
                        item0.Enabled = false;
                    }
                    else {
                        if (this.WindowState == FormWindowState.Normal)
                        {
                            item2.Enabled = true;
                            item0.Enabled = false;
                        }
                        else
                        {
                            item2.Enabled = false;
                            item0.Enabled = true;
                        }
                    }
                    taskMenu.Show(this, new Point(e.X+5, e.Y), LeftRightAlignment.Right);
                }
            }
        }

        private void item5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void item4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void item3_Click(object sender, EventArgs e)
        {
            this.taskMenu.Dispose();
            this.Close();
        }

        private void item2_Click(object sender, EventArgs e)
        {
            this.taskMenu.Dispose();
            this.taskMenu = null;
            this.WindowState = FormWindowState.Maximized;
        }

        private void item1_Click(object sender, EventArgs e)
        {
            this.taskMenu.Dispose();
            this.taskMenu = null;
            this.WindowState = FormWindowState.Minimized;
        }

        private void item0_Click(object sender, EventArgs e)
        {
            this.taskMenu.Dispose();
            this.taskMenu = null;
            this.WindowState = FormWindowState.Normal;
        }

        private void ButtonMax_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState==FormWindowState.Normal)
                    this.WindowState = FormWindowState.Maximized;
                else
                    this.WindowState = FormWindowState.Normal;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Focus();
            this.Activate();
            if (e.Button == MouseButtons.Right)
            {
                if (e.Y <= 31 && e.X < xwidth)
                {
                    topPanel_MouseClick(null, e);
                }
            }
        }

        private void colour_Click(object sender, EventArgs e)
        {
            if (skinPanel.Visible)
            {
                skinPanel.Hide();
            }
            else 
            {
                skinPanel.Left = this.Width - skinPanel.Width - 3;
                skinPanel.Top = colour.Top + colour.Height + 3;
                skinPanel.Height = 177;
                skinPanel.BringToFront();
                skinPanel.Show();
                skinPanel.Focus();
            }
        }

        private void colour_MouseEnter(object sender, EventArgs e)
        {
            if (!skinPanel.Visible)
                s_bmp = Resources.All_iconbutton_highlightBackground;
            colour.Invalidate();
        }

        private Bitmap s_bmp = null;

        private void colour_MouseLeave(object sender, EventArgs e)
        {
            if (!skinPanel.Visible)
                s_bmp = null;
            colour.Invalidate();
        }

        private void colour_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            if(s_bmp!=null)
                g.DrawImage(Util.ProcImage(s_bmp, colorA, lightB, grayC), new Rectangle(0, 1, 18, 18), 0, 0, 20, 20, GraphicsUnit.Pixel);
            g.DrawImage(Easy.Controls.Properties.Resources.colour, new Rectangle(0, 1, 18, 18), 0, 0, 18, 18, GraphicsUnit.Pixel);
        }

        private void skinPanel_Leave(object sender, EventArgs e)
        {
            if (skinPanel.Visible)
            {
                skinPanel.Hide();
                colour_MouseLeave(null, null);
            }
        }

        private void basicPictureBox1_Click(object sender, EventArgs e)
        {
            EasyPictureBox pic = sender as EasyPictureBox;
            switch (pic.Texts)
            {
                case "蓝":
                    trackBar1.Value = -20;
                    trackBar2.Value = 0;
                    trackBar3.Value = 0;
                    break;
                case "黄":
                    trackBar1.Value = -130;
                    trackBar2.Value = -5;
                    trackBar3.Value = 40;
                    break;
                case "橙":
                    trackBar1.Value = -175;
                    trackBar2.Value = 0;
                    trackBar3.Value = 0;
                    break;
                case "绿":
                    trackBar1.Value = -60;
                    trackBar2.Value = -10;
                    trackBar3.Value = 0;
                    break;
                case "黑":
                    trackBar1.Value = 50;
                    trackBar2.Value = -50;
                    trackBar3.Value = -180;
                    break;
                case "紫":
                    trackBar1.Value = 100;
                    trackBar2.Value = -20;
                    trackBar3.Value = 0;
                    break;
                case "红":
                    trackBar1.Value = -185;
                    trackBar2.Value = -5;
                    trackBar3.Value = 10;
                    break;
                case "透明":
                    trackBar1.Value = 0;
                    trackBar2.Value = 0;
                    trackBar3.Value = 0;
                    break;
                case "默认":
                    trackBar1.Value = 0;
                    trackBar2.Value = 0;
                    trackBar3.Value = 0;
                    break;
            }
            trackBar1_Scroll(null, null);
            trackBar2_Scroll(null, null);
            trackBar3_Scroll(null, null);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            colorA = trackBar1.Value;
            ButtonMin.Invalidate();
            ButtonMax.Invalidate();
            ButtonClose.Invalidate();
            Invalidate();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            lightB = trackBar2.Value;
            ButtonMin.Invalidate();
            ButtonMax.Invalidate();
            ButtonClose.Invalidate();
            Invalidate();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            grayC = trackBar3.Value;
            Invalidate();
            ButtonMin.Invalidate();
            ButtonMax.Invalidate();
            ButtonClose.Invalidate();
        }
    }
}
