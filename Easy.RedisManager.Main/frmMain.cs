using Easy.Common;
using Easy.RedisManager.Entity.Config;
using Easy.RedisManager.Entity.Dictionary;
using Easy.RedisManager.Entity.Redis;
using Easy.RedisManager.RedisAccess.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Easy.RedisManager.Main
{
    public partial class FrmMain : Form
    {
        public delegate void RefreshTreeListNodeStatusDelegate(TreeNode node, int imageIndex);
        #region Const
        private const int Index_RedisOnline = 0;
        private const int Index_RedisOffline = 1;
        private const int Index_RedisDb = 2;
        private const int Index_RedisKey = 3;
        private const int Index_RedisNameSpace = 4;
        private const int Index_RedisWait = 5;
        #endregion

        /// <summary>
        /// 日志记录
        /// </summary>
        private readonly ILogger _logger = null;

        public FrmMain()
        {
            InitializeComponent();

            // 初始化日志记录
            _logger = LogFactory.CreateLogger(typeof(FrmMain)); 
        }

        #region Method

        /// <summary>
        /// 绑定配置文件节点
        /// </summary>
        private void BindConfigNodeToTree()
        {
            string redisConnFullPath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory,
                "Config\\RedisConnections.xml");
            _logger.Debug("RedisConnectionConfig FilePath:" + redisConnFullPath);

            // 初始化Redis连接信息
            Singleton<RedisConnectionDict>.Instance.Init(redisConnFullPath);
            _logger.Debug("初始化Redis连接信息结束");

            // 添加节点
            foreach (var config in Singleton<RedisConnectionDict>.Instance.RedisConnDict.Values)
            {
                var node = new TreeNode(config.Name);
                node.Tag = config;
                node.ImageIndex = Index_RedisOnline;
                node.ImageIndex = Index_RedisOffline;
                treDBList.Nodes.Add(node);
            }
            treDBList.Refresh();
        }

        /// <summary>
        /// 绑定数据库
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="dicDbInfo"></param>
        /// <param name="config"></param>
        private void BindDbToTree(TreeNode parentNode, Dictionary<long, long> dicDbInfo
            , RedisConnConfig config)
        {
            if (parentNode == null)
                return;

            parentNode.Nodes.Clear();
            // 添加节点
            foreach (var info in dicDbInfo)
            {
                var nodeInfo = new RedisDbNodeInfo();
                nodeInfo.ParentHost = config.Host;
                nodeInfo.ParentPort = config.Port;
                nodeInfo.Db = info.Key;

                string nodeName = string.Format("db{0}({1})", info.Key, info.Value);

                var node = new TreeNode(nodeName);
                node.Tag = nodeInfo;
                node.ImageIndex = Index_RedisDb;
                parentNode.Nodes.Add(node);
                parentNode.Expand();
            }
            treDBList.Refresh();
        }

        /// <summary>
        /// 绑定Keys
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="keys"></param>
        private void BindKeysToTree(TreeNode parentNode, List<string> keys)
        {
            if (parentNode == null)
                return;

            parentNode.Nodes.Clear();
            // 添加节点
            foreach (var key in keys)
            {
                var node = new TreeNode(key);
                node.ImageIndex = Index_RedisKey;
                parentNode.Nodes.Add(node);
                parentNode.Expand();
            }
            treDBList.Refresh();
        }

        /// <summary>
        /// 更新节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="imageIndex"></param>
        private void RefreshTreeListNodeStatus(TreeNode node, int imageIndex)
        {
            node.ImageIndex = imageIndex;
            node.SelectedImageIndex = imageIndex;
            //treDBList.SelectedNode = node;
            treDBList.Refresh();
        }

        #endregion

        #region Event
        /// <summary>
        /// Manage Connections Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManageConn_Click(object sender, EventArgs e)
        {
            _logger.Debug("btnManageConn_Click Start.");

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
            _logger.Debug("btnManageConn_Click End.");
        }

        /// <summary>
        /// Add a new Connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNewConn_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Main Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            _logger.Debug("FrmMain_Load Start.");

            try
            {
                BindConfigNodeToTree();
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                _logger.Error("FrmMain_Load Error. Message:" + ex.Message);
                _logger.Error("FrmMain_Load Error. StackTrace:" + ex.StackTrace);
            }

            _logger.Debug("FrmMain_Load End.");

        }

        /// <summary>
        /// Node Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treDBList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                var connNode = e.Node;
                if (connNode == null)
                    return;

                this.BeginInvoke((MethodInvoker)delegate
                {
                    RefreshTreeListNodeStatus(connNode, Index_RedisWait);
                });

                var connNodeTag = connNode.Tag as RedisConnConfig;
                if (connNodeTag != null)
                {
                    using (var redisSocket = new RedisSocketClient(connNodeTag.Host, connNodeTag.Port))
                    {
                        var dictDbInfo = redisSocket.GetRedisDbAndDbSize();
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            BindDbToTree(connNode, dictDbInfo, connNodeTag);
                        });

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            RefreshTreeListNodeStatus(connNode, Index_RedisOnline);
                        });
                    }
                }
                else
                {
                    var nodeInfo = connNode.Tag as RedisDbNodeInfo;
                    if (nodeInfo != null)
                    {
                        using (var redisSocket = new RedisSocketClient(nodeInfo.ParentHost, nodeInfo.ParentPort))
                        {
                            var keys = redisSocket.GetAllKeysByDb(nodeInfo.Db).OrderBy(x => x).ToList();
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                BindKeysToTree(connNode, keys);
                            });

                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                RefreshTreeListNodeStatus(connNode, Index_RedisDb);
                            });
                        }
                    }
                }
            });

            thread.Start();
        }
        #endregion
    }
}
