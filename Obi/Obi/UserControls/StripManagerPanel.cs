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
        private Dictionary<SectionNode, SectionStrip> mSectionNodeMap;  // find a section strip for a given node
        private SectionNode mSelectedSection;                           // the selected node

        private Dictionary<PhraseNode, AudioBlock> mPhraseNodeMap;     // find an audio block for a given phrase node
        private PhraseNode mSelectedPhrase;                            // the selected audio block

        private ProjectPanel mProjectPanel; //the parent of this control

        public event Events.SetMediaHandler SetMediaRequested;
        public event Events.SelectedHandler SelectionChanged;

        #region properties

        /// <summary>
        /// Selected node (phrase or section.)
        /// </summary>
        public ObiNode SelectedNode
        {
            get { return mSelectedPhrase == null ? (ObiNode)mSelectedSection : (ObiNode)mSelectedPhrase; }
            set
            {
                if (value != null)
                {
                    if (value.GetType() == Type.GetType("Obi.SectionNode"))
                    {
                        SelectedSectionNode = (SectionNode)value;
                    }
                    else if (value.GetType() == Type.GetType("Obi.PhraseNode"))
                    {
                        SelectedPhraseNode = (PhraseNode)value;
                    }
                }
                else
                {
                    SelectedPhraseNode = null;
                    SelectedSectionNode = null;
                }
            }
        }

        /// <summary>
        /// Get or set the currently selected section node.
        /// A null value means that no node is selected.
        /// When the new selection is set, the previous one (if any) is desselected.
        /// If there is a new selection, the node gets selected and the context menu is updated.
        /// An event informs listeners (e.g. this panel and the main form) about the current selection status.
        /// </summary>
        public SectionNode SelectedSectionNode
        {
            get { return mSelectedSection; }
            set
            {
                //make the new select
                if (value != null && mSelectedSection != value)
                {
                    //deselect everything will actually call this function and set the value to null
                    //therefore deselecting whatever is currently selected
                    mProjectPanel.DeselectEverything();

                    mSelectedSection = value;
                    mSectionNodeMap[mSelectedSection].Selected = true;
                    SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(true, mSectionNodeMap[mSelectedSection]));
                }
                //or deselect the old selection
                else if (value == null)
                {
                    if (mSelectedSection != null)
                    {
                        if (mSectionNodeMap.ContainsKey(mSelectedSection))
                        {
                            SectionStrip strip = mSectionNodeMap[mSelectedSection];
                            strip.Selected = false;
                            SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(false, strip));
                        }
                        mSelectedSection = null;
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
        public PhraseNode SelectedPhraseNode
        {
            get { return mSelectedPhrase; }
            set
            {
               //make the new select
                if (value != null && mSelectedPhrase != value)
                {
                    //deselect everything will actually call this function and set the value to null
                    //therefore deselecting whatever is currently selected
                    mProjectPanel.DeselectEverything();

                    mSelectedPhrase = value;
                    mPhraseNodeMap[mSelectedPhrase].Selected = true;
                    SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(true, mPhraseNodeMap[mSelectedPhrase]));
                }
                //or deselect the old selection
                else if (value == null)
                {
                    if (mSelectedPhrase != null)
                    {
                        AudioBlock block = mPhraseNodeMap[mSelectedPhrase];
                        block.Selected = false;
                        mSelectedPhrase = value; //null
                        SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(false, block));
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
            get { return mSelectedSection == null ? null : mSectionNodeMap[mSelectedSection]; }
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
                return this.mContextMenuStrip;
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
            InitializeEventHandlers();
            // The panel is empty and nothing is selected.
            mSectionNodeMap = new Dictionary<SectionNode, SectionStrip>();
            mSelectedSection = null;
            mPhraseNodeMap = new Dictionary<PhraseNode, AudioBlock>();
            mSelectedPhrase = null;
        }

        /// <summary>
        /// Initialize event handlers for the control.
        /// </summary>
        /// <remarks>The designer doesn't like those.</remarks>
        private void InitializeEventHandlers()
        {
            mInsertEmptyAudioblockToolStripMenuItem.Click +=
                new System.EventHandler(delegate(object _sender, System.EventArgs _e) { InsertEmptyAudioBlock(); });
        }

        #endregion

        /// <summary>
        /// Synchronize the strips view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ICoreNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
        {
            mFlowLayoutPanel.Controls.Clear();
            mSectionNodeMap.Clear();
            mSelectedSection = null;
            mSelectedPhrase = null;
            root.acceptDepthFirst(this);
            //if (mFlowLayoutPanel.Controls.Count > 0) this.ReflowTabOrder(mFlowLayoutPanel.Controls[0]);  // mg
        }

        #region Synchronization visitor

        private CoreNode parentSection;  // the current parent section to add phrases to
        private CoreNode parentPhrase;   // the current phrase node to add structure nodes to

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
           
            //if node is root
            if (node.GetType() == Type.GetType("urakawa.core.CoreNode"))
            { 
                parentSection = null;
            }
            else if (node.GetType() == Type.GetType("Obi.SectionNode"))
            {
                strip = new SectionStrip();
                strip.Label = Project.GetTextMedia((CoreNode)node).getText();
                strip.Manager = this;
                strip.Node = (SectionNode)node;
                mSectionNodeMap[(SectionNode)node] = strip;
                mFlowLayoutPanel.Controls.Add(strip);
                parentSection = ((SectionNode)node);
            }
            else if (node.GetType() == Type.GetType("Obi.PhraseNode"))
            {
                strip = mSectionNodeMap[(SectionNode)parentSection];
                AudioBlock block = SetupAudioBlockFromPhraseNode((PhraseNode)node);
                strip.AppendAudioBlock(block);
                parentPhrase = (PhraseNode)node;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Convenience method to send the event that a strip was renamed, so that we don't have to track down
        /// all individual strips.
        /// </summary>
        /// <param name="strip">The renamed strip (with its new name as a label.)</param>
        public void RenamedSectionStrip(SectionStrip strip)
        {
            mProjectPanel.Project.RenameSectionNodeWithCommand(strip.Node, strip.Label);
        }

        /// <summary>
        /// Clicking outside of a strip deselects any selected strip or block.
        /// </summary>
        private void mFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            SelectedSectionNode = null;
            SelectedPhraseNode = null;
        }
    }
}
