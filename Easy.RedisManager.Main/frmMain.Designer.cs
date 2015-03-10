namespace Easy.RedisManager.Main
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.spCLayout = new System.Windows.Forms.SplitContainer();
            this.tablLlPLeft = new System.Windows.Forms.TableLayoutPanel();
            this.tVDBList = new System.Windows.Forms.TreeView();
            this.tableLPConn = new System.Windows.Forms.TableLayoutPanel();
            this.btnManageConn = new System.Windows.Forms.Button();
            this.btnAddNewConn = new System.Windows.Forms.Button();
            this.tabCRight = new System.Windows.Forms.TabControl();
            this.tabPRedisManager = new System.Windows.Forms.TabPage();
            this.conMSManageConn = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.spCLayout)).BeginInit();
            this.spCLayout.Panel1.SuspendLayout();
            this.spCLayout.Panel2.SuspendLayout();
            this.spCLayout.SuspendLayout();
            this.tablLlPLeft.SuspendLayout();
            this.tableLPConn.SuspendLayout();
            this.tabCRight.SuspendLayout();
            this.conMSManageConn.SuspendLayout();
            this.SuspendLayout();
            // 
            // spCLayout
            // 
            this.spCLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spCLayout.Location = new System.Drawing.Point(0, 0);
            this.spCLayout.Name = "spCLayout";
            // 
            // spCLayout.Panel1
            // 
            this.spCLayout.Panel1.Controls.Add(this.tablLlPLeft);
            this.spCLayout.Panel1MinSize = 100;
            // 
            // spCLayout.Panel2
            // 
            this.spCLayout.Panel2.Controls.Add(this.tabCRight);
            this.spCLayout.Panel2MinSize = 100;
            this.spCLayout.Size = new System.Drawing.Size(2099, 1036);
            this.spCLayout.SplitterDistance = 699;
            this.spCLayout.SplitterWidth = 12;
            this.spCLayout.TabIndex = 0;
            // 
            // tablLlPLeft
            // 
            this.tablLlPLeft.ColumnCount = 1;
            this.tablLlPLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablLlPLeft.Controls.Add(this.tVDBList, 0, 1);
            this.tablLlPLeft.Controls.Add(this.tableLPConn, 0, 2);
            this.tablLlPLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablLlPLeft.Location = new System.Drawing.Point(0, 0);
            this.tablLlPLeft.Name = "tablLlPLeft";
            this.tablLlPLeft.RowCount = 3;
            this.tablLlPLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tablLlPLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablLlPLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tablLlPLeft.Size = new System.Drawing.Size(699, 1036);
            this.tablLlPLeft.TabIndex = 0;
            // 
            // tVDBList
            // 
            this.tVDBList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tVDBList.Location = new System.Drawing.Point(3, 63);
            this.tVDBList.Name = "tVDBList";
            this.tVDBList.Size = new System.Drawing.Size(693, 910);
            this.tVDBList.TabIndex = 0;
            // 
            // tableLPConn
            // 
            this.tableLPConn.ColumnCount = 2;
            this.tableLPConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLPConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLPConn.Controls.Add(this.btnAddNewConn, 1, 0);
            this.tableLPConn.Controls.Add(this.btnManageConn, 0, 0);
            this.tableLPConn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLPConn.Location = new System.Drawing.Point(3, 979);
            this.tableLPConn.Name = "tableLPConn";
            this.tableLPConn.RowCount = 1;
            this.tableLPConn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLPConn.Size = new System.Drawing.Size(693, 54);
            this.tableLPConn.TabIndex = 1;
            // 
            // btnManageConn
            // 
            this.btnManageConn.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnManageConn.Location = new System.Drawing.Point(73, 3);
            this.btnManageConn.Name = "btnManageConn";
            this.btnManageConn.Size = new System.Drawing.Size(270, 48);
            this.btnManageConn.TabIndex = 1;
            this.btnManageConn.Text = "Manage Connection";
            this.btnManageConn.UseVisualStyleBackColor = true;
            this.btnManageConn.Click += new System.EventHandler(this.btnManageConn_Click);
            // 
            // btnAddNewConn
            // 
            this.btnAddNewConn.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddNewConn.Location = new System.Drawing.Point(420, 3);
            this.btnAddNewConn.Name = "btnAddNewConn";
            this.btnAddNewConn.Size = new System.Drawing.Size(270, 48);
            this.btnAddNewConn.TabIndex = 2;
            this.btnAddNewConn.Text = "Add New Connection";
            this.btnAddNewConn.UseVisualStyleBackColor = true;
            // 
            // tabCRight
            // 
            this.tabCRight.Controls.Add(this.tabPRedisManager);
            this.tabCRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCRight.Location = new System.Drawing.Point(0, 0);
            this.tabCRight.Name = "tabCRight";
            this.tabCRight.SelectedIndex = 0;
            this.tabCRight.Size = new System.Drawing.Size(1388, 1036);
            this.tabCRight.TabIndex = 0;
            // 
            // tabPRedisManager
            // 
            this.tabPRedisManager.Location = new System.Drawing.Point(4, 34);
            this.tabPRedisManager.Name = "tabPRedisManager";
            this.tabPRedisManager.Padding = new System.Windows.Forms.Padding(3);
            this.tabPRedisManager.Size = new System.Drawing.Size(1380, 998);
            this.tabPRedisManager.TabIndex = 0;
            this.tabPRedisManager.Text = "Redis Manager";
            this.tabPRedisManager.UseVisualStyleBackColor = true;
            // 
            // conMSManageConn
            // 
            this.conMSManageConn.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importConnectionsToolStripMenuItem,
            this.exportConnectionsToolStripMenuItem});
            this.conMSManageConn.Name = "conMSManageConn";
            this.conMSManageConn.Size = new System.Drawing.Size(328, 76);
            // 
            // importConnectionsToolStripMenuItem
            // 
            this.importConnectionsToolStripMenuItem.Name = "importConnectionsToolStripMenuItem";
            this.importConnectionsToolStripMenuItem.Size = new System.Drawing.Size(327, 36);
            this.importConnectionsToolStripMenuItem.Text = "Import Connections";
            // 
            // exportConnectionsToolStripMenuItem
            // 
            this.exportConnectionsToolStripMenuItem.Name = "exportConnectionsToolStripMenuItem";
            this.exportConnectionsToolStripMenuItem.Size = new System.Drawing.Size(327, 36);
            this.exportConnectionsToolStripMenuItem.Text = "Export Connections";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2099, 1036);
            this.Controls.Add(this.spCLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "Redis Manager";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.spCLayout.Panel1.ResumeLayout(false);
            this.spCLayout.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spCLayout)).EndInit();
            this.spCLayout.ResumeLayout(false);
            this.tablLlPLeft.ResumeLayout(false);
            this.tableLPConn.ResumeLayout(false);
            this.tabCRight.ResumeLayout(false);
            this.conMSManageConn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spCLayout;
        private System.Windows.Forms.TableLayoutPanel tablLlPLeft;
        private System.Windows.Forms.TreeView tVDBList;
        private System.Windows.Forms.Button btnManageConn;
        private System.Windows.Forms.TableLayoutPanel tableLPConn;
        private System.Windows.Forms.Button btnAddNewConn;
        private System.Windows.Forms.TabControl tabCRight;
        private System.Windows.Forms.TabPage tabPRedisManager;
        private System.Windows.Forms.ContextMenuStrip conMSManageConn;
        private System.Windows.Forms.ToolStripMenuItem importConnectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportConnectionsToolStripMenuItem;
    }
}