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

        private ProjectPanel mProjectPanel; //the parent of this control

        public event Events.Node.RequestToAddSiblingNodeHandler AddSiblingSection;
        public event Events.Node.RequestToRenameNodeHandler RenameSection;
        public event Events.Node.SetMediaHandler SetMedia;
        public event Events.Strip.RequestToImportAssetHandler ImportPhrase;
        public event Events.Strip.SelectedHandler SelectedStrip;
        public event Events.Strip.SelectedHandler SelectedAudioBlock;

        #region properties

        /// <summary>
        /// Get or set the currently selected section node.
        /// A null value means that no node is selected.
        /// When the new selection is set, the previous one (if any) is desselected.
        /// If there is a new selection, the node gets selected and the context menu is updated.
        /// An event informs listeners (e.g. this panel and the main form) about the current selection status.
        /// </summary>
        public CoreNode SelectedSectionNode
        {
            get
            {
                return mSelectedSection;
            }
            set
            {
                if (mSelectedSection != value)
                {
                    if (mSelectedSection != null) mSectionNodeMap[mSelectedSection].MarkDeselected();
                    mSelectedSection = value;
                    if (mSelectedSection != null)
                    {
                        mSectionNodeMap[mSelectedSection].MarkSelected();
                        SelectedStrip(this, new Events.Strip.SelectedEventArgs(true));
                    }
                    else
                    {
                        SelectedStrip(this, new Events.Strip.SelectedEventArgs(false));
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the currently selected phrase node.
        /// A null value means that no node is selected.
        /// When the new selection is set, the previous one (if any) is desselected.
        /// If there is a new selection, the node gets selected and the context menu is updated.
        /// The parent section node is selected as well.
        /// An event informs listeners (e.g. this panel and the main form) about the current selection status.
        /// </summary>
        public CoreNode SelectedPhraseNode
        {
            get
            {
                return mSelectedPhrase;
            }
            set
            {
                if (mSelectedPhrase != value)
                {
                    if (mSelectedPhrase != null) mPhraseNodeMap[mSelectedPhrase].MarkDeselected();
                    mSelectedPhrase = value;
                    if (mSelectedPhrase != null)
                    {
                        SelectedSectionNode = (CoreNode)mSelectedPhrase.getParent();
                        SelectedAudioBlock(this, new Events.Strip.SelectedEventArgs(true));
                    }
                    else
                    {
                        SelectedAudioBlock(this, new Events.Strip.SelectedEventArgs(false));
                    }
                }
            }
        }
        
        /// <summary>
        /// Get the SectionStrip that is currently seleced, or null if no current selection exists.
        /// </summary>
        // mg20060804
        internal SectionStrip SelectedSectionStrip
        {
            get
            {
                if(mSelectedSection!=null)
                    return mSectionNodeMap[mSelectedSection];
                return null;
            }
        }

        /// <summary>
        /// Get the control of the Block (phrase) that is currently seleced, or null if no current selection exists.
        /// </summary>
        // mg20060804
        internal AbstractBlock SelectedBlock
        {
            get
            {
                if (this.mSelectedPhrase != null)
                    return this.mPhraseNodeMap[mSelectedPhrase];
                return null;
            }
        }

        /// <summary>
        /// Get the context menu strip of this StripManagerPanel
        /// </summary>
        /// <remarks>mg: for access by sectionstrip that needs to override the textbox menu</remarks> 
        internal ContextMenuStrip PanelContextMenuStrip
        {
            get
            {
                return this.contextMenuStrip1;
            }
        }

        /// <summary>
        /// Get and set the parent ProjectPanel control 
        /// </summary>
        // mg 20060804
        internal ProjectPanel ProjectPanel
        {
            get 
            {
                return mProjectPanel;
            }
            set
            {
                mProjectPanel = value;
            }
        }

        #endregion

        #region instantiators

        /// <summary>
        /// Create a new manager panel. At first the panel is empty and nothing is selected.
        /// </summary>
        public StripManagerPanel()
        {
            InitializeComponent();
            // Listen to selected events to update the context menu.
            SelectedStrip += new Events.Strip.SelectedHandler(StripManagerPanel_SelectedStrip);
            SelectedAudioBlock += new Events.Strip.SelectedHandler(StripManagerPanel_SelectedAudioBlock);
            // The panel is empty and nothing is selected.
            mSectionNodeMap = new Dictionary<CoreNode, SectionStrip>();
            mSelectedSection = null;
            mPhraseNodeMap = new Dictionary<CoreNode, AudioBlock>();
            mSelectedPhrase = null;
        }

        #endregion

        /// <summary>
        /// Update the menus when a strip is selected.
        /// Affects "rename strip", "import audio file", "show in TOC view".
        /// </summary>
        private void StripManagerPanel_SelectedStrip(object sender, Events.Strip.SelectedEventArgs e)
        {
            mRenameStripToolStripMenuItem.Enabled = e.Selected;
            mImportAudioFileToolStripMenuItem.Enabled = e.Selected;
            mShowInTOCViewToolStripMenuItem.Enabled = e.Selected;
        }

        /// <summary>
        /// Update the menus when a block is selected.
        /// Affects "play audio block", "split audio block", "rename audio block"
        /// </summary>
        private void StripManagerPanel_SelectedAudioBlock(object sender, Events.Strip.SelectedEventArgs e)
        {
            mPlayAudioBlockToolStripMenuItem.Enabled = e.Selected;
            mSplitAudioBlockToolStripMenuItem.Enabled = e.Selected;
            mRenameAudioBlockToolStripMenuItem.Enabled = e.Selected;
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
            SelectedSectionNode = null;
            SelectedPhraseNode = null;
            root.acceptDepthFirst(this);
            if (mFlowLayoutPanel.Controls.Count > 0) this.ReflowTabOrder(mFlowLayoutPanel.Controls[0]);  // mg
            SelectedSectionNode = null;
            SelectedPhraseNode = null;
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
            if (Project.GetNodeType((CoreNode)node) == NodeType.Section)
            {
                SectionStrip strip = new SectionStrip();
                strip.Label = Project.GetTextMedia((CoreNode)node).getText();
                strip.Manager = this;
                strip.Node = (CoreNode)node;
                mSectionNodeMap[(CoreNode)node] = strip;
                mFlowLayoutPanel.Controls.Add(strip);
                SelectedSectionNode = strip.Node;
            }
            else if (Project.GetNodeType((CoreNode)node) == NodeType.Phrase)
            {
                SectionStrip strip = mSectionNodeMap[mSelectedSection];
                AudioBlock block = new AudioBlock();
                block.Manager = this;
                block.Node = (CoreNode)node;
                mPhraseNodeMap[(CoreNode)node] = block;
                TextMedia annotation = (TextMedia)Project.GetMediaForChannel((CoreNode)node, Project.AnnotationChannel);
                block.Label = annotation.getText();
                block.Time = (Math.Round(Project.GetAudioMediaAsset((CoreNode)node).LengthInMilliseconds / 1000)).ToString() + "s";
                strip.AppendAudioBlock(block);
                SelectedPhraseNode = block.Node;
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
            if (rename)
            {
                SelectedSectionNode = node;
                strip.StartRenaming();
            }
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
                Visitors.DescendantsVisitor visitor = new Visitors.DescendantsVisitor();
                e.Node.acceptDepthFirst(visitor);
                foreach (CoreNode node in visitor.Nodes)
                {
                    if (Project.GetNodeType(node) == NodeType.Section)
                    {
                        SectionStrip strip = mSectionNodeMap[node];
                        mFlowLayoutPanel.Controls.Remove(strip);
                    }
                }
                //mg:
                //this.ReflowTabOrder(mSectionNodeMap[e.Node]);
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
                block.Label = annotation.getText();
                block.Time = (Math.Round(Project.GetAudioMediaAsset(e.Node).LengthInMilliseconds / 1000)).ToString() + "s";
                strip.AppendAudioBlock(block);
                //mg:
                this.ReflowTabOrder(block);
            }
        }

        internal void SyncMovedNode(object sender, Events.Node.MovedNodeEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[e.Node];
            mFlowLayoutPanel.Controls.SetChildIndex(strip, e.Position);
            //mg:
            this.ReflowTabOrder(strip);
        }

        internal void SyncMediaSet(object sender, Events.Node.SetMediaEventArgs e)
        {
        }

        #endregion

        #region Menu items

        /// <summary>
        /// TODO:
        /// Adding a strip from the strip manager adds a new sibling strip right after the selected strip
        /// and reattaches the selected strip's children to the new strip. In effet, the new strip appears
        /// just below the selected strip.
        /// When no strip is selected, just add a new strip at the top of the tree.
        /// </summary>
        internal void mAddStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSiblingSection(this, new Events.Node.NodeEventArgs(this, mSelectedSection));
            // InsertSiblingSection(this, new Events.Node.NodeEventArgs(this, mSelectedSection));
        }

        internal void mRenameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedSection != null)
            {
                mSectionNodeMap[mSelectedSection].StartRenaming();
            }
        }

        internal void mImportAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedSectionNode != null)
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
        /// Play the currently selected audio block.
        /// </summary>
        /// <remarks>JQ</remarks>
        internal void mPlayAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                Dialogs.Play dialog = new Dialogs.Play(mSelectedPhrase);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Split the currently selected audio block.
        /// </summary>
        /// <remarks>JQ</remarks>
        internal void mSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                Dialogs.Split dialog = new Dialogs.Split(mSelectedPhrase, 0.0);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Rename the currently selected audio block (JQ)
        /// </summary>
        internal void mRenameAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                mPhraseNodeMap[mSelectedPhrase].StartRenaming();
            }
        }

        /// <summary>
        /// If a node is selected, set focus on that node in the TreeView.
        /// If the selected node is not a section node, move back to the
        /// section before commiting the select
        /// </summary>
        //  mg20060804
        internal void mShowInTOCViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedSection != null)
            {
                ProjectPanel.TOCPanel.SetSelectedSection(mSelectedSection);
                //since the tree can be hidden:
                mProjectPanel.ShowTOCPanel();
                ProjectPanel.TOCPanel.Focus();
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

        internal void RenamedAudioBlock(AudioBlock block)
        {
            TextMedia media = (TextMedia)block.Node.getPresentation().getMediaFactory().createMedia(MediaType.TEXT);
            media.setText(block.Label);
            SetMedia(this, new Events.Node.SetMediaEventArgs(this, block.Node, Project.AnnotationChannel, media));
        }

        private void mFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            SelectedSectionNode = null;
            SelectedPhraseNode = null;
        }

        private void mFlowLayoutPanel_Leave(object sender, EventArgs e)
        {
            SelectedSectionNode = null;
            SelectedPhraseNode = null;
        }

        /// <summary>
        /// Reflow the tab order (tabindex property)
        /// of strips and blocks in this StripManagerPanel
        /// starting from the inparam control,
        /// continuing to the the last block in the last strip.
        /// </summary>  
        /// <param name="startFrom">Either a SectionStrip or an AudioBlock </param>
        /// <returns>The last (highest) tabindex added</returns>
        //   added by mg 20060803
        internal int ReflowTabOrder(Control startFrom)
        {
            //get the previous tabindex, considered valid
            Control previous = getPreviousNodesControl(startFrom);
            int index;
            if (previous == startFrom)
            {
                index = -1;
            }
            else
            {
                index = previous.TabIndex;
            }
            System.Diagnostics.Debug.Print("Reflowing taborder from index " + index);

            //find out what strip to start from
            SectionStrip startStrip;
            if (startFrom is AudioBlock)
            {
                startStrip = mSectionNodeMap
                    [(CoreNode)((AudioBlock)startFrom).Node.getParent()];
            }
            else
            {
                startStrip = (SectionStrip)startFrom;
            }

            //proceed with the reflowing            
            bool firstIter = true;
            for (int i = 0; i < this.mFlowLayoutPanel.Controls.Count; i++)
            {
                Control c = this.mFlowLayoutPanel.Controls[i];
                if (c == startStrip || (!firstIter))
                {
                    if (c is SectionStrip)
                    {
                        SectionStrip ss = c as SectionStrip;
                        if (firstIter)
                        {
                            if (startFrom is SectionStrip)
                            {
                                ss.TabIndex = ++index;
                                index = ss.ReflowTabOrder(index);
                            }
                            else
                            {
                                index = ss.ReflowTabOrder(startFrom, index);
                            }
                            firstIter = false;
                        }
                        else
                        {
                            ss.TabIndex = ++index;
                            index = ss.ReflowTabOrder(index);
                        }
                    } //if (c is SectionStrip)
                    else
                    {
                        try
                        {
                            c.TabStop = false;
                        }
                        catch (Exception)
                        {
                            //instead of reflection
                        }
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Get the StripManagerPanel control corresponding to the previous Urakawa node
        /// </summary>  
        /// <returns>The control corresponding to the previous node in the tree,
        /// or the inparam control if no previous node exists</returns>
        // mg 20060803
        private Control getPreviousNodesControl(Control ctrl)
        {
            //we use the ukawa tree because it gets less messy than 
            //the windows.forms control hierarchy

            CoreNode node = null;

            if (ctrl is SectionStrip)
            {
                node = ((SectionStrip)ctrl).Node;
            }
            else if (ctrl is AudioBlock)
            {
                node = ((AudioBlock)ctrl).Node;
            }
            else
            {
                //TODO, when new types are added
                return ctrl;
            }

            if (ctrl.Controls.Count == 0) return ctrl;

            CoreNode previous = null;
            CoreNode cur = null;
            CoreNode parent = (CoreNode)node.getParent();

            if (parent != null && parent.getChildCount() > 0)
            {
                CoreNode prev = null;
                for (int i = 0; i < parent.getChildCount(); i++)
                {
                    cur = parent.getChild(i);
                    if (cur == node)
                    {
                        if (prev != null)
                        {
                            previous = prev;
                        }
                        else
                        {
                            previous = cur;
                        }
                        break;
                    }
                    prev = cur;
                }

            }
            else
            {
                previous = parent;
            }

            //now we have the prev node
            if (mPhraseNodeMap.ContainsKey(previous))
            {
                return this.mPhraseNodeMap[previous];
            }
            else
            {
                return this.mSectionNodeMap[previous];
            }

        }

    }
}