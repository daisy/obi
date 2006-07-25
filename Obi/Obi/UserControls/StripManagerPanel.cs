using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// This control is a view of all the contents of a project
    /// </summary>
    public partial class StripManagerPanel : UserControl, ICoreNodeVisitor
    {
        private Dictionary<CoreNode, SectionStrip> mNodeMap;  // find a strip for a given node
        private CoreNode mSelectedNode;                       // the selected node

        public event Events.Node.AddSiblingSectionHandler AddSiblingSection;
        public event Events.Node.RenameSectionHandler RenameSection;

        public CoreNode SelectedNode
        {
            get
            {
                return mSelectedNode;
            }
            set
            {
                if (mSelectedNode != null) mNodeMap[mSelectedNode].MarkDeselected();
                mSelectedNode = value;
                if (mSelectedNode != null) mNodeMap[mSelectedNode].MarkSelected();
            }
        }

        public StripManagerPanel()
        {
            InitializeComponent();
            mNodeMap = new Dictionary<CoreNode, SectionStrip>();
            mSelectedNode = null;
        }

        /// <summary>
        /// Synchronize the strips view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ICoreNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
        {
            mNodeMap.Clear();
            mFlowLayoutPanel.Controls.Clear();
            root.acceptDepthFirst(this);
        }

        #region Synchronization visitor

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="node">The node to do nothing with.</param>
        public void postVisit(ICoreNode node)
        {
        }

        /// <summary>
        /// Create a new strip for every core node. Skip the root node.
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True.</returns>
        public bool preVisit(ICoreNode node)
        {
            if (node.getParent() != null)
            {
                SectionStrip strip = new SectionStrip();
                strip.Label = Project.GetTextMedia((CoreNode)node).getText();
                strip.Manager = this;
                strip.Node = (CoreNode)node;
                mNodeMap[(CoreNode)node] = strip;
                mFlowLayoutPanel.Controls.Add(strip);
            }
            return true;
        }

        #endregion

        #region Sync event handlers

        internal void SyncAddedSiblingNode(object sender, Events.Sync.AddedSiblingNodeEventArgs e)
        {
            AddStripFromNode(e.Node, e.Position);
            if (e.Node != null) SelectedNode = e.Node;
        }

        internal void SyncAddedChildNode(object sender, Events.Sync.AddedChildNodeEventArgs e)
        {
            AddStripFromNode(e.Node, e.Position);
            if (e.Node != null) SelectedNode = e.Node;
        }

        private void AddStripFromNode(CoreNode node, int position)
        {
            SectionStrip strip = new SectionStrip();
            strip.Label = Project.GetTextMedia(node).getText();
            strip.Manager = this;
            strip.Node = node;
            mNodeMap[node] = strip;
            mFlowLayoutPanel.Controls.Add(strip);
            mFlowLayoutPanel.Controls.SetChildIndex(strip, position);
        }

        internal void SyncRenamedNode(object sender, Events.Sync.RenamedNodeEventArgs e)
        {
            SectionStrip strip = mNodeMap[e.Node];
            strip.Label = e.Label;
        }

        /// <summary>
        /// When deleting a node from the tree, all descendants are deleted as well.
        /// </summary>
        internal void SyncDeletedNode(object sender, Events.Sync.DeletedNodeEventArgs e)
        {
            if (e.Node != null)
            {
                DescendantsVisitor visitor = new DescendantsVisitor();
                e.Node.acceptDepthFirst(visitor);
                foreach (CoreNode node in visitor.Nodes)
                {
                    SectionStrip strip = mNodeMap[node];
                    mFlowLayoutPanel.Controls.Remove(strip);
                }
            }
        }

        #endregion

        private void mFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            SelectedNode = null;
        }

        #region Menu items

        internal void addStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSiblingSection(this, new Events.Node.AddSiblingSectionEventArgs(mSelectedNode));
        }

        internal void renameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedNode != null)
            {
                mNodeMap[mSelectedNode].StartRenaming();
            }
        }

        #endregion

        internal void RenamedSectionStrip(SectionStrip strip)
        {
            RenameSection(this, new Events.Node.RenameSectionEventArgs(strip.Node, strip.Label));
        }
    }
}