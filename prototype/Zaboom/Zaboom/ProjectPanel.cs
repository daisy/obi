using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;
using urakawa.undo;

namespace Zaboom
{
    public partial class ProjectPanel : UserControl
    {
        private Project project;
        private CommandManager commandManager;
        private Dictionary<urakawa.core.TreeNode, Control> nodeMap;
        private int pixelsPerSecond;

        public ProjectPanel()
        {
            InitializeComponent();
            project = null;
            commandManager = new CommandManager();
            nodeMap = new Dictionary<urakawa.core.TreeNode, Control>();
            pixelsPerSecond = WaveformPanel.DEFAULT_PIXELS_PER_SECOND;
            transportBar.Enabled = false;  // let's wait for a working audio player
        }

        public Project Project
        {
            get { return project; }
            set
            {
                if (project == null && value != null)
                {
                    project = value;
                    project.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(project_treeNodeAdded);
                    project.getPresentation().treeNodeRemoved += new TreeNodeRemovedEventHandler(project_treeNodeRemoved);
                    flowLayout.Controls.Add(new DummyBlock());
                }
            }
        }

        public CommandManager CommandManager { get { return commandManager; } }

        public int PixelsPerSecond
        {
            get { return pixelsPerSecond; }
            set
            {
                pixelsPerSecond = value;
                foreach (Control c in flowLayout.Controls)
                {
                    WaveformPanel panel = c as WaveformPanel;
                    if (panel != null) panel.PixelsPerSecond = pixelsPerSecond;
                }
            }
        }

        /// <summary>
        /// Handle addition of a node.
        /// </summary>
        private void project_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            WaveformPanel panel = new WaveformPanel(project, e.getTreeNode(), pixelsPerSecond);
            flowLayout.Controls.Add(panel);
            flowLayout.Controls.SetChildIndex(panel, flowLayout.Controls.Count - 2);
            nodeMap[e.getTreeNode()] = panel;
        }

        void project_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            flowLayout.Controls.Remove(nodeMap[e.getTreeNode()]);
            nodeMap.Remove(e.getTreeNode());
        }
    }
}
