using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

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
        public event Events.Node.SetMediaHandler SetMediaRequested;
        public event Events.Strip.RequestToImportAssetHandler ImportAudioAssetRequested;
        public event Events.Node.RequestToDeleteBlockHandler DeleteBlockRequested;
        public event Events.Node.RequestToMoveBlockHandler MoveAudioBlockForwardRequested;
        public event Events.Node.RequestToMoveBlockHandler MoveAudioBlockBackwardRequested;
        public event Events.Node.SplitNodeHandler SplitAudioBlockRequested;
        public event Events.Node.MergeNodesHandler MergeNodes;
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
                        mPhraseNodeMap[mSelectedPhrase].MarkSelected();
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
                if (mSelectedSection != null)
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
            mEditAudioBlockLabelToolStripMenuItem.Enabled = e.Selected;
            mDeleteAudioBlockToolStripMenuItem.Enabled = e.Selected;
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
            mSelectedSection = null;
            mSelectedPhrase = null;
            root.acceptDepthFirst(this);
            if (mFlowLayoutPanel.Controls.Count > 0) this.ReflowTabOrder(mFlowLayoutPanel.Controls[0]);  // mg
            SelectedSectionNode = null;
            SelectedPhraseNode = null;
        }

        #region Synchronization visitor

        private CoreNode parentSection;

        /// <summary>
        /// Update the parent section to attach phrase nodes to.
        /// </summary>
        /// <param name="node">The node to do nothing with.</param>
        public void postVisit(ICoreNode node)
        {
            parentSection = (CoreNode)node.getParent();
        }

        /// <summary>
        /// Create a new strip for every core node.
        /// Skip the root node and nodes that do not have a text channel (i.e. phrase nodes.)
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True.</returns>
        public bool preVisit(ICoreNode node)
        {
            SectionStrip strip = null;
            switch (Project.GetNodeType((CoreNode)node))
            {
                case NodeType.Root:
                    parentSection = null;
                    break;
                case NodeType.Section:
                    strip = new SectionStrip();
                    strip.Label = Project.GetTextMedia((CoreNode)node).getText();
                    strip.Manager = this;
                    strip.Node = (CoreNode)node;
                    mSectionNodeMap[(CoreNode)node] = strip;
                    mFlowLayoutPanel.Controls.Add(strip);
                    parentSection = ((CoreNode)node);
                    break;
                case NodeType.Phrase:
                    strip = mSectionNodeMap[parentSection];
                    AudioBlock block = new AudioBlock();
                    block.Manager = this;
                    block.Node = (CoreNode)node;
                    mPhraseNodeMap[(CoreNode)node] = block;
                    TextMedia annotation = (TextMedia)Project.GetMediaForChannel((CoreNode)node, Project.AnnotationChannel);
                    block.Label = annotation.getText();
                    block.Time = (Math.Round(Project.GetAudioMediaAsset((CoreNode)node).LengthInMilliseconds / 1000)).ToString() + "s";
                    strip.AppendAudioBlock(block);
                    break;
                default:
                    break;
            }
            return true;
        }

        #endregion

        #region Sync event handlers

        
        internal void SyncBlockChangedTime(object sender, Events.Node.NodeEventArgs e)
        {
            AudioBlock block = mPhraseNodeMap[e.Node];
            block.Time = (Math.Round(Project.GetAudioMediaAsset(e.Node).LengthInMilliseconds / 1000)).ToString() + "s";
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

        /// <summary>
        /// Enable/disable items depending on what is currently available.
        /// </summary>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            mShowInTOCViewToolStripMenuItem.Enabled = mSelectedSection != null;
        }
    }
}
