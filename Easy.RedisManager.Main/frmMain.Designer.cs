namespace Easy.RedisManager.Main
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.spCLayout = new System.Windows.Forms.SplitContainer();
            this.tLPLeft = new System.Windows.Forms.TableLayoutPanel();
            this.treDBList = new System.Windows.Forms.TreeView();
            this.ilsTree = new System.Windows.Forms.ImageList(this.components);
            this.tLPConn = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddNewConn = new System.Windows.Forms.Button();
            this.btnManageConn = new System.Windows.Forms.Button();
            this.tLPSearch = new System.Windows.Forms.TableLayoutPanel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.tabCRight = new System.Windows.Forms.TabControl();
            this.tabPRedisManager = new System.Windows.Forms.TabPage();
            this.conMSManageConn = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.spCLayout)).BeginInit();
            this.spCLayout.Panel1.SuspendLayout();
            this.spCLayout.Panel2.SuspendLayout();
            this.spCLayout.SuspendLayout();
            this.tLPLeft.SuspendLayout();
            this.tLPConn.SuspendLayout();
            this.tLPSearch.SuspendLayout();
            this.tabCRight.SuspendLayout();
            this.conMSManageConn.SuspendLayout();
            this.SuspendLayout();
            // 
            // spCLayout
            // 
            this.spCLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spCLayout.Location = new System.Drawing.Point(0, 0);
            this.spCLayout.Margin = new System.Windows.Forms.Padding(2);
            this.spCLayout.Name = "spCLayout";
            // 
            // spCLayout.Panel1
            // 
            this.spCLayout.Panel1.Controls.Add(this.tLPLeft);
            this.spCLayout.Panel1MinSize = 100;
            // 
            // spCLayout.Panel2
            // 
            this.spCLayout.Panel2.Controls.Add(this.tabCRight);
            this.spCLayout.Panel2MinSize = 100;
            this.spCLayout.Size = new System.Drawing.Size(806, 635);
            this.spCLayout.SplitterDistance = 268;
            this.spCLayout.SplitterWidth = 6;
            this.spCLayout.TabIndex = 0;
            // 
            // tLPLeft
            // 
            this.tLPLeft.ColumnCount = 1;
            this.tLPLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPLeft.Controls.Add(this.treDBList, 0, 1);
            this.tLPLeft.Controls.Add(this.tLPConn, 0, 2);
            this.tLPLeft.Controls.Add(this.tLPSearch, 0, 0);
            this.tLPLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPLeft.Location = new System.Drawing.Point(0, 0);
            this.tLPLeft.Margin = new System.Windows.Forms.Padding(2);
            this.tLPLeft.Name = "tLPLeft";
            this.tLPLeft.RowCount = 3;
            this.tLPLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tLPLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tLPLeft.Size = new System.Drawing.Size(268, 635);
            this.tLPLeft.TabIndex = 0;
            // 
            // treDBList
            // 
            this.treDBList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treDBList.ImageIndex = 1;
            this.treDBList.ImageList = this.ilsTree;
            this.treDBList.Location = new System.Drawing.Point(2, 38);
            this.treDBList.Margin = new System.Windows.Forms.Padding(2);
            this.treDBList.Name = "treDBList";
            this.treDBList.SelectedImageIndex = 0;
            this.treDBList.Size = new System.Drawing.Size(264, 565);
            this.treDBList.TabIndex = 0;
            this.treDBList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treDBList_NodeMouseClick);
            // 
            // ilsTree
            // 
            this.ilsTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilsTree.ImageStream")));
            this.ilsTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ilsTree.Images.SetKeyName(0, "redis_small_icon.png");
            this.ilsTree.Images.SetKeyName(1, "redis_small_icon_offline.png");
            this.ilsTree.Images.SetKeyName(2, "db.png");
            this.ilsTree.Images.SetKeyName(3, "key.png");
            this.ilsTree.Images.SetKeyName(4, "namespace.png");
            this.ilsTree.Images.SetKeyName(5, "wait.png");
            // 
            // tLPConn
            // 
            this.tLPConn.ColumnCount = 2;
            this.tLPConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tLPConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tLPConn.Controls.Add(this.btnAddNewConn, 1, 0);
            this.tLPConn.Controls.Add(this.btnManageConn, 0, 0);
            this.tLPConn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPConn.Location = new System.Drawing.Point(2, 607);
            this.tLPConn.Margin = new System.Windows.Forms.Padding(2);
            this.tLPConn.Name = "tLPConn";
            this.tLPConn.RowCount = 1;
            this.tLPConn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPConn.Size = new System.Drawing.Size(264, 26);
            this.tLPConn.TabIndex = 1;
            // 
            // btnAddNewConn
            // 
            this.btnAddNewConn.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddNewConn.Location = new System.Drawing.Point(134, 2);
            this.btnAddNewConn.Margin = new System.Windows.Forms.Padding(2);
            this.btnAddNewConn.Name = "btnAddNewConn";
            this.btnAddNewConn.Size = new System.Drawing.Size(128, 22);
            this.btnAddNewConn.TabIndex = 2;
            this.btnAddNewConn.Text = "Add New Connection";
            this.btnAddNewConn.UseVisualStyleBackColor = true;
            this.btnAddNewConn.Click += new System.EventHandler(this.btnAddNewConn_Click);
            // 
            // btnManageConn
            // 
            this.btnManageConn.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnManageConn.Location = new System.Drawing.Point(2, 2);
            this.btnManageConn.Margin = new System.Windows.Forms.Padding(2);
            this.btnManageConn.Name = "btnManageConn";
            this.btnManageConn.Size = new System.Drawing.Size(128, 22);
            this.btnManageConn.TabIndex = 1;
            this.btnManageConn.Text = "Manage Connection";
            this.btnManageConn.UseVisualStyleBackColor = true;
            this.btnManageConn.Click += new System.EventHandler(this.btnManageConn_Click);
            // 
            // tLPSearch
            // 
            this.tLPSearch.ColumnCount = 3;
            this.tLPSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tLPSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tLPSearch.Controls.Add(this.txtSearch, 0, 0);
            this.tLPSearch.Controls.Add(this.btnOk, 1, 0);
            this.tLPSearch.Controls.Add(this.btnClear, 2, 0);
            this.tLPSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPSearch.Location = new System.Drawing.Point(3, 3);
            this.tLPSearch.Name = "tLPSearch";
            this.tLPSearch.RowCount = 1;
            this.tLPSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPSearch.Size = new System.Drawing.Size(262, 30);
            this.tLPSearch.TabIndex = 2;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtSearch.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSearch.Location = new System.Drawing.Point(3, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(196, 23);
            this.txtSearch.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOk.Image = global::Easy.RedisManager.Main.Properties.Resources.ok;
            this.btnOk.Location = new System.Drawing.Point(205, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(24, 24);
            this.btnOk.TabIndex = 1;
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.Image = global::Easy.RedisManager.Main.Properties.Resources.clear;
            this.btnClear.Location = new System.Drawing.Point(235, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.TabIndex = 2;
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // tabCRight
            // 
            this.tabCRight.Controls.Add(this.tabPRedisManager);
            this.tabCRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCRight.Location = new System.Drawing.Point(0, 0);
            this.tabCRight.Margin = new System.Windows.Forms.Padding(2);
            this.tabCRight.Name = "tabCRight";
            this.tabCRight.SelectedIndex = 0;
            this.tabCRight.Size = new System.Drawing.Size(532, 635);
            this.tabCRight.TabIndex = 0;
            // 
            // tabPRedisManager
            // 
            this.tabPRedisManager.Location = new System.Drawing.Point(4, 22);
            this.tabPRedisManager.Margin = new System.Windows.Forms.Padding(2);
            this.tabPRedisManager.Name = "tabPRedisManager";
            this.tabPRedisManager.Padding = new System.Windows.Forms.Padding(2);
            this.tabPRedisManager.Size = new System.Drawing.Size(524, 609);
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
            this.conMSManageConn.Size = new System.Drawing.Size(192, 48);
            // 
            // importConnectionsToolStripMenuItem
            // 
            this.importConnectionsToolStripMenuItem.Name = "importConnectionsToolStripMenuItem";
            this.importConnectionsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.importConnectionsToolStripMenuItem.Text = "Import Connections";
            // 
            // exportConnectionsToolStripMenuItem
            // 
            this.exportConnectionsToolStripMenuItem.Name = "exportConnectionsToolStripMenuItem";
            this.exportConnectionsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exportConnectionsToolStripMenuItem.Text = "Export Connections";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 635);
            this.Controls.Add(this.spCLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmMain";
            this.Text = "Redis Manager";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.spCLayout.Panel1.ResumeLayout(false);
            this.spCLayout.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spCLayout)).EndInit();
            this.spCLayout.ResumeLayout(false);
            this.tLPLeft.ResumeLayout(false);
            this.tLPConn.ResumeLayout(false);
            this.tLPSearch.ResumeLayout(false);
            this.tLPSearch.PerformLayout();
            this.tabCRight.ResumeLayout(false);
            this.conMSManageConn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spCLayout;
        private System.Windows.Forms.TableLayoutPanel tLPLeft;
        private System.Windows.Forms.TreeView treDBList;
        private System.Windows.Forms.Button btnManageConn;
        private System.Windows.Forms.TableLayoutPanel tLPConn;
        private System.Windows.Forms.Button btnAddNewConn;
        private System.Windows.Forms.TabControl tabCRight;
        private System.Windows.Forms.TabPage tabPRedisManager;
        private System.Windows.Forms.ContextMenuStrip conMSManageConn;
        private System.Windows.Forms.ToolStripMenuItem importConnectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportConnectionsToolStripMenuItem;
        private System.Windows.Forms.ImageList ilsTree;
        private System.Windows.Forms.TableLayoutPanel tLPSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnClear;
    }
}