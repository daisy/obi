using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    /// <summary>
    /// This control is a view of all the contents of a project
    /// </summary>
    public partial class StripManagerPanel : UserControl, ICoreNodeVisitor
    {
        private Dictionary<CoreNode, SectionStrip> mSectionNodeMap;  // find a section strip for a given node
        private CoreNode mSelectedSection;                           // the selected node

        private Dictionary<CoreNode, AudioBlock> mPhraseNodeMap;     // find an audio block for a given phrase node
        private CoreNode mSelectedPhrase;                            // the selected audio block

        public event Events.Node.RequestToAddSiblingNodeHandler AddSiblingSection;
        public event Events.Node.RequestToRenameNodeHandler RenameSection;
        public event Events.Strip.RequestToImportAssetHandler ImportPhrase;
        public event Events.Strip.SelectedHandler SelectedStrip;

        public CoreNode SelectedSection
        {
            get
            {
                return mSelectedSection;
            }
            set
            {
                if (mSelectedSection != null) mSectionNodeMap[mSelectedSection].MarkDeselected();
                mSelectedSection = value;
                if (mSelectedSection != null)
                {
                    mSectionNodeMap[mSelectedSection].MarkSelected();
                    renameStripToolStripMenuItem.Enabled = true;
                    importAssetToolStripMenuItem.Enabled = true;
                    SelectedStrip(this, new Events.Strip.SelectedEventArgs(true));
                }
                else
                {
                    renameStripToolStripMenuItem.Enabled = false;
                    importAssetToolStripMenuItem.Enabled = false;
                    SelectedStrip(this, new Events.Strip.SelectedEventArgs(false));
                }
            }
        }

        public CoreNode SelectedPhrase
        {
            get
            {
                return mSelectedPhrase;
            }
            set
            {
                if (mSelectedPhrase != null) mPhraseNodeMap[mSelectedPhrase].MarkDeselected();
                mSelectedPhrase = value;
                if (mSelectedPhrase != null)
                {
                    SelectedSection = (CoreNode)mSelectedPhrase.getParent();
                    mPhraseNodeMap[mSelectedPhrase].MarkSelected();
                    playAssetToolStripMenuItem.Enabled = true;
                }
                else
                {
                    playAssetToolStripMenuItem.Enabled = false;
                }
            }
        }

        public StripManagerPanel()
        {
            InitializeComponent();
            mSectionNodeMap = new Dictionary<CoreNode, SectionStrip>();
            mSelectedSection = null;
            mPhraseNodeMap = new Dictionary<CoreNode, AudioBlock>();
            mSelectedPhrase = null;
        }

        /// <summary>
        /// Synchronize the strips view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ICoreNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
        {
            mSectionNodeMap.Clear();
            mFlowLayoutPanel.Controls.Clear();
            root.acceptDepthFirst(this);
            SelectedSection = null;
            SelectedPhrase = null;
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
        /// Create a new strip for every core node.
        /// Skip the root node and nodes that do not have a text channel (i.e. phrase nodes.)
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True.</returns>
        public bool preVisit(ICoreNode node)
        {
            if (Project.GetNodeType((CoreNode)node) == NodeType.SectionNode)
            {
                SectionStrip strip = new SectionStrip();
                strip.Label = Project.GetTextMedia((CoreNode)node).getText();
                strip.Manager = this;
                strip.Node = (CoreNode)node;
                mSectionNodeMap[(CoreNode)node] = strip;
                mFlowLayoutPanel.Controls.Add(strip);
                SelectedSection = strip.Node;
            }
            else if (Project.GetNodeType((CoreNode)node) == NodeType.PhraseNode)
            {
                SectionStrip strip = mSectionNodeMap[mSelectedSection];
                AudioBlock block = new AudioBlock();
                block.Manager = this;
                block.Node = (CoreNode)node;
                mPhraseNodeMap[(CoreNode)node] = block;
                TextMedia annotation = (TextMedia)Project.GetMediaForChannel((CoreNode)node, Project.AnnotationChannel);
                block.Annotation = annotation.getText();
                block.Time = (Project.GetAudioMediaAsset((CoreNode)node).LengthInMilliseconds / 1000).ToString() + "s";
                strip.AppendAudioBlock(block);
                SelectedPhrase = block.Node;
            }
            return true;
        }

        #endregion

        #region Sync event handlers

        internal void SyncAddedSectionNode(object sender, Events.Node.AddedSectionNodeEventArgs e)
        {
            AddStripFromNode(e.Node, e.Position, e.Origin == this);
        }

        private void AddStripFromNode(CoreNode node, int position, bool rename)
        {
            SectionStrip strip = new SectionStrip();
            strip.Label = Project.GetTextMedia(node).getText();
            strip.Manager = this;
            strip.Node = node;
            mSectionNodeMap[node] = strip;
            mFlowLayoutPanel.Controls.Add(strip);
            mFlowLayoutPanel.Controls.SetChildIndex(strip, position);
            SelectedSection = node;
            if (rename) strip.StartRenaming();
        }

        internal void SyncRenamedNode(object sender, Events.Node.RenameNodeEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[e.Node];
            strip.Label = e.Label;
        }

        /// <summary>
        /// When deleting a node from the tree, all descendants are deleted as well.
        /// </summary>
        internal void SyncDeletedNode(object sender, Events.Node.NodeEventArgs e)
        {
            if (e.Node != null)
            {
                DescendantsVisitor visitor = new DescendantsVisitor();
                e.Node.acceptDepthFirst(visitor);
                foreach (CoreNode node in visitor.Nodes)
                {
                    SectionStrip strip = mSectionNodeMap[node];
                    mFlowLayoutPanel.Controls.Remove(strip);
                }
            }
        }

        internal void SyncImportedAsset(object sender, Events.Node.NodeEventArgs e)
        {
            if (e.Node != null && mSelectedSection != null)
            {
                SectionStrip strip = mSectionNodeMap[mSelectedSection];
                AudioBlock block = new AudioBlock();
                block.Manager = this;
                block.Node = e.Node;
                mPhraseNodeMap[e.Node] = block;
                TextMedia annotation = (TextMedia)Project.GetMediaForChannel(e.Node, Project.AnnotationChannel);
                block.Annotation = annotation.getText();
                block.Time = (Project.GetAudioMediaAsset(e.Node).LengthInMilliseconds / 1000).ToString() + "s";
                strip.AppendAudioBlock(block);
            }
        }

        internal void SyncMovedNode(object sender, Events.Node.MovedNodeEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[e.Node];
            mFlowLayoutPanel.Controls.SetChildIndex(strip, e.Position);
        }

        #endregion

        #region Menu items

        internal void addStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSiblingSection(this, new Events.Node.NodeEventArgs(this, mSelectedSection));
        }

        internal void renameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedSection != null)
            {
                mSectionNodeMap[mSelectedSection].StartRenaming();
            }
        }

        internal void importAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedSection != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "WAVE file (*.wav)|*.wav|Any file|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ImportPhrase(this, new Events.Strip.ImportAssetEventArgs(mSelectedSection, dialog.FileName));
                }
            }
        }

        /// <summary>
        /// Play the currently selected audio block. (JQ)
        /// </summary>
        private void playAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                Dialogs.Play dialog = new Dialogs.Play(mSelectedPhrase);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Rename the currently selected audio block (JQ)
        /// </summary>
        private void renameAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
            }
        }


        #endregion

        /// <summary>
        /// Convenience method to send the event that a strip was renamed, so that we don't have to track down
        /// all individual strips.
        /// </summary>
        /// <param name="strip">The renamed strip (with its new name as a label.)</param>
        internal void RenamedSectionStrip(SectionStrip strip)
        {
            RenameSection(this, new Events.Node.RenameNodeEventArgs(this, strip.Node, strip.Label));
        }

        private void mFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            SelectedSection = null;
            SelectedPhrase = null;
        }

        private void mFlowLayoutPanel_Leave(object sender, EventArgs e)
        {
            SelectedSection = null;
            SelectedPhrase = null;
        }
    }
}