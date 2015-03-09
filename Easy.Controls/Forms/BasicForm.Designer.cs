namespace Easy.Controls
{
    partial class BasicForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (imageAttr != null) {
                imageAttr.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.basicPictureBox8 = new EasyPictureBox();
            this.basicPictureBox9 = new EasyPictureBox();
            this.basicPictureBox4 = new EasyPictureBox();
            this.basicPictureBox5 = new EasyPictureBox();
            this.basicPictureBox6 = new EasyPictureBox();
            this.basicPictureBox3 = new EasyPictureBox();
            this.basicPictureBox2 = new EasyPictureBox();
            this.basicPictureBox1 = new EasyPictureBox();
            this.ButtonMax = new System.Windows.Forms.PictureBox();
            this.ButtonClose = new System.Windows.Forms.PictureBox();
            this.ButtonMin = new System.Windows.Forms.PictureBox();
            this.colour = new System.Windows.Forms.PictureBox();
            this.skinPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colour)).BeginInit();
            this.skinPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 4000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // trackBar3
            // 
            this.trackBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar3.AutoSize = false;
            this.trackBar3.Location = new System.Drawing.Point(1, 142);
            this.trackBar3.Maximum = 200;
            this.trackBar3.Minimum = -180;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(149, 22);
            this.trackBar3.TabIndex = 2;
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.toolTip1.SetToolTip(this.trackBar3, "灰度");
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar2.AutoSize = false;
            this.trackBar2.Location = new System.Drawing.Point(1, 112);
            this.trackBar2.Maximum = 80;
            this.trackBar2.Minimum = -50;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(149, 22);
            this.trackBar2.TabIndex = 1;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.toolTip1.SetToolTip(this.trackBar2, "亮度");
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.AutoSize = false;
            this.trackBar1.Location = new System.Drawing.Point(1, 82);
            this.trackBar1.Maximum = 170;
            this.trackBar1.Minimum = -185;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(149, 22);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.toolTip1.SetToolTip(this.trackBar1, "颜色");
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // basicPictureBox8
            // 
            this.basicPictureBox8.Image = global::Easy.Controls.Properties.Resources.s7;
            this.basicPictureBox8.Location = new System.Drawing.Point(114, 43);
            this.basicPictureBox8.Name = "basicPictureBox8";
            this.basicPictureBox8.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basicPictureBox8.TabIndex = 25;
            this.basicPictureBox8.TabStop = false;
            this.basicPictureBox8.Texts = "黑";
            this.toolTip1.SetToolTip(this.basicPictureBox8, "黑色");
            this.basicPictureBox8.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // basicPictureBox9
            // 
            this.basicPictureBox9.Image = global::Easy.Controls.Properties.Resources.s6;
            this.basicPictureBox9.Location = new System.Drawing.Point(78, 43);
            this.basicPictureBox9.Name = "basicPictureBox9";
            this.basicPictureBox9.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basicPictureBox9.TabIndex = 24;
            this.basicPictureBox9.TabStop = false;
            this.basicPictureBox9.Texts = "绿";
            this.toolTip1.SetToolTip(this.basicPictureBox9, "绿色");
            this.basicPictureBox9.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // basicPictureBox4
            // 
            this.basicPictureBox4.Image = global::Easy.Controls.Properties.Resources.s5;
            this.basicPictureBox4.Location = new System.Drawing.Point(42, 43);
            this.basicPictureBox4.Name = "basicPictureBox4";
            this.basicPictureBox4.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basicPictureBox4.TabIndex = 23;
            this.basicPictureBox4.TabStop = false;
            this.basicPictureBox4.Texts = "黄";
            this.toolTip1.SetToolTip(this.basicPictureBox4, "黄色");
            this.basicPictureBox4.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // basicPictureBox5
            // 
            this.basicPictureBox5.Image = global::Easy.Controls.Properties.Resources.s4;
            this.basicPictureBox5.Location = new System.Drawing.Point(6, 43);
            this.basicPictureBox5.Name = "basicPictureBox5";
            this.basicPictureBox5.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basicPictureBox5.TabIndex = 22;
            this.basicPictureBox5.TabStop = false;
            this.basicPictureBox5.Texts = "橙";
            this.toolTip1.SetToolTip(this.basicPictureBox5, "橙色");
            this.basicPictureBox5.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // basicPictureBox6
            // 
            this.basicPictureBox6.Image = global::Easy.Controls.Properties.Resources.s3;
            this.basicPictureBox6.Location = new System.Drawing.Point(114, 7);
            this.basicPictureBox6.Name = "basicPictureBox6";
            this.basicPictureBox6.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basicPictureBox6.TabIndex = 21;
            this.basicPictureBox6.TabStop = false;
            this.basicPictureBox6.Texts = "红";
            this.toolTip1.SetToolTip(this.basicPictureBox6, "红色");
            this.basicPictureBox6.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // basicPictureBox3
            // 
            this.basicPictureBox3.Image = global::Easy.Controls.Properties.Resources.s2;
            this.basicPictureBox3.Location = new System.Drawing.Point(78, 7);
            this.basicPictureBox3.Name = "basicPictureBox3";
            this.basicPictureBox3.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basicPictureBox3.TabIndex = 20;
            this.basicPictureBox3.TabStop = false;
            this.basicPictureBox3.Texts = "紫";
            this.toolTip1.SetToolTip(this.basicPictureBox3, "紫色");
            this.basicPictureBox3.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // basicPictureBox2
            // 
            this.basicPictureBox2.Image = global::Easy.Controls.Properties.Resources.s1;
            this.basicPictureBox2.Location = new System.Drawing.Point(42, 7);
            this.basicPictureBox2.Name = "basicPictureBox2";
            this.basicPictureBox2.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basicPictureBox2.TabIndex = 19;
            this.basicPictureBox2.TabStop = false;
            this.basicPictureBox2.Texts = "蓝";
            this.toolTip1.SetToolTip(this.basicPictureBox2, "蓝色");
            this.basicPictureBox2.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // basicPictureBox1
            // 
            this.basicPictureBox1.Image = global::Easy.Controls.Properties.Resources._null;
            this.basicPictureBox1.Location = new System.Drawing.Point(6, 7);
            this.basicPictureBox1.Name = "basicPictureBox1";
            this.basicPictureBox1.Size = new System.Drawing.Size(30, 30);
            this.basicPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.basicPictureBox1.TabIndex = 18;
            this.basicPictureBox1.TabStop = false;
            this.basicPictureBox1.Texts = "默认";
            this.toolTip1.SetToolTip(this.basicPictureBox1, "默认色");
            this.basicPictureBox1.Click += new System.EventHandler(this.basicPictureBox1_Click);
            // 
            // ButtonMax
            // 
            this.ButtonMax.BackColor = System.Drawing.Color.Transparent;
            this.ButtonMax.Location = new System.Drawing.Point(258, 1);
            this.ButtonMax.Name = "ButtonMax";
            this.ButtonMax.Size = new System.Drawing.Size(26, 18);
            this.ButtonMax.TabIndex = 23;
            this.ButtonMax.TabStop = false;
            this.ButtonMax.MouseLeave += new System.EventHandler(this.ButtonMax_MouseLeave);
            this.ButtonMax.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ButtonMax_MouseClick);
            this.ButtonMax.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonMax_MouseDown);
            this.ButtonMax.Paint += new System.Windows.Forms.PaintEventHandler(this.ButtonMax_Paint);
            this.ButtonMax.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonMax_MouseUp);
            this.ButtonMax.MouseEnter += new System.EventHandler(this.ButtonMax_MouseEnter);
            // 
            // ButtonClose
            // 
            this.ButtonClose.BackColor = System.Drawing.Color.Transparent;
            this.ButtonClose.Location = new System.Drawing.Point(283, 1);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(38, 18);
            this.ButtonClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ButtonClose.TabIndex = 21;
            this.ButtonClose.TabStop = false;
            this.ButtonClose.MouseLeave += new System.EventHandler(this.ButtonClose_MouseLeave);
            this.ButtonClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ButtonClose_MouseClick);
            this.ButtonClose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonClose_MouseDown);
            this.ButtonClose.Paint += new System.Windows.Forms.PaintEventHandler(this.ButtonClose_Paint);
            this.ButtonClose.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonClose_MouseUp);
            this.ButtonClose.MouseEnter += new System.EventHandler(this.ButtonClose_MouseEnter);
            // 
            // ButtonMin
            // 
            this.ButtonMin.BackColor = System.Drawing.Color.Transparent;
            this.ButtonMin.Location = new System.Drawing.Point(234, 1);
            this.ButtonMin.Name = "ButtonMin";
            this.ButtonMin.Size = new System.Drawing.Size(25, 18);
            this.ButtonMin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ButtonMin.TabIndex = 20;
            this.ButtonMin.TabStop = false;
            this.ButtonMin.MouseLeave += new System.EventHandler(this.ButtonMin_MouseLeave);
            this.ButtonMin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ButtonMin_MouseClick);
            this.ButtonMin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonMin_MouseDown);
            this.ButtonMin.Paint += new System.Windows.Forms.PaintEventHandler(this.ButtonMin_Paint);
            this.ButtonMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ButtonMin_MouseUp);
            this.ButtonMin.MouseEnter += new System.EventHandler(this.ButtonMin_MouseEnter);
            // 
            // colour
            // 
            this.colour.BackColor = System.Drawing.Color.Transparent;
            this.colour.Location = new System.Drawing.Point(209, 1);
            this.colour.Name = "colour";
            this.colour.Size = new System.Drawing.Size(19, 19);
            this.colour.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.colour.TabIndex = 24;
            this.colour.TabStop = false;
            this.colour.MouseLeave += new System.EventHandler(this.colour_MouseLeave);
            this.colour.Click += new System.EventHandler(this.colour_Click);
            this.colour.Paint += new System.Windows.Forms.PaintEventHandler(this.colour_Paint);
            this.colour.MouseEnter += new System.EventHandler(this.colour_MouseEnter);
            // 
            // skinPanel
            // 
            this.skinPanel.BackColor = System.Drawing.Color.AliceBlue;
            this.skinPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.skinPanel.Controls.Add(this.basicPictureBox8);
            this.skinPanel.Controls.Add(this.basicPictureBox9);
            this.skinPanel.Controls.Add(this.basicPictureBox4);
            this.skinPanel.Controls.Add(this.basicPictureBox5);
            this.skinPanel.Controls.Add(this.basicPictureBox6);
            this.skinPanel.Controls.Add(this.basicPictureBox3);
            this.skinPanel.Controls.Add(this.basicPictureBox2);
            this.skinPanel.Controls.Add(this.basicPictureBox1);
            this.skinPanel.Controls.Add(this.trackBar3);
            this.skinPanel.Controls.Add(this.trackBar2);
            this.skinPanel.Controls.Add(this.trackBar1);
            this.skinPanel.Location = new System.Drawing.Point(168, 26);
            this.skinPanel.Name = "skinPanel";
            this.skinPanel.Size = new System.Drawing.Size(153, 0);
            this.skinPanel.TabIndex = 28;
            this.skinPanel.Visible = false;
            this.skinPanel.Leave += new System.EventHandler(this.skinPanel_Leave);
            // 
            // BasicForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(323, 241);
            this.Controls.Add(this.skinPanel);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.ButtonMax);
            this.Controls.Add(this.ButtonMin);
            this.Controls.Add(this.colour);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(205, 40);
            this.Name = "BasicForm";
            this.Text = "INewForm";
            this.Load += new System.EventHandler(this.IForm_Load);
            this.TextChanged += new System.EventHandler(this.IForm_TextChanged);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basicPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colour)).EndInit();
            this.skinPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox ButtonClose;
        private System.Windows.Forms.PictureBox ButtonMin;
        private System.Windows.Forms.PictureBox ButtonMax;
        private System.Windows.Forms.PictureBox colour;
        private System.Windows.Forms.Panel skinPanel;
        private EasyPictureBox basicPictureBox8;
        private EasyPictureBox basicPictureBox9;
        private EasyPictureBox basicPictureBox4;
        private EasyPictureBox basicPictureBox5;
        private EasyPictureBox basicPictureBox6;
        private EasyPictureBox basicPictureBox3;
        private EasyPictureBox basicPictureBox2;
        private EasyPictureBox basicPictureBox1;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar1;

    }
}