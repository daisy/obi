using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;
using urakawa.undo;

namespace Zaboom.UserControls
{
    public partial class ProjectPanel : UserControl
    {
        private Project project;
        private CommandManager commandManager;
        private Dictionary<urakawa.core.TreeNode, Control> nodeMap;
        private List<Selectable> selected;
        private int pixelsPerSecond;

        public ProjectPanel()
        {
            InitializeComponent();
            project = null;
            commandManager = new CommandManager();
            nodeMap = new Dictionary<urakawa.core.TreeNode, Control>();
            selected = new List<Selectable>();
            pixelsPerSecond = WaveformPanel.DEFAULT_PIXELS_PER_SECOND;
            transportBar.Enabled = false;
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
                    AudioBlock block = c as AudioBlock;
                    if (block != null) block.PixelsPerSecond = pixelsPerSecond;
                }
            }
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
                    DummyBlock dummy = new DummyBlock();
                    dummy.Panel = this;
                    flowLayout.Controls.Add(dummy);
                }
            }
        }

        public void ReplaceSelection(UserControls.Selectable s)
        {
            Deselect();
            selected.Add(s);
        }

        public List<Selectable> Selected { get { return selected; } }

        public void SelectionChanged(Selectable s)
        {
            if (s.Selected)
            {
                selected.Add(s);
            }
            else
            {
                selected.Remove(s);
            }
        }

        /// <summary>
        /// Deselect all selected items.
        /// </summary>
        private void Deselect()
        {
            foreach (UserControls.Selectable s in selected) s.Selected = false;
        }

        private void flowLayout_Click(object sender, EventArgs e)
        {
            Deselect();
        }

        /// <summary>
        /// Handle addition of a node.
        /// </summary>
        private void project_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            AudioBlock block = new AudioBlock();
            block.Panel = this;
            block.Project = project;
            block.Node = e.getTreeNode();
            block.PixelsPerSecond = pixelsPerSecond;
            flowLayout.Controls.Add(block);
            flowLayout.Controls.SetChildIndex(block, flowLayout.Controls.Count - 2);
            nodeMap[e.getTreeNode()] = block;
        }

        /// <summary>
        /// Handle deletion of a node.
        /// </summary>
        private void project_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            flowLayout.Controls.Remove(nodeMap[e.getTreeNode()]);
            nodeMap.Remove(e.getTreeNode());
        }
    }
}
