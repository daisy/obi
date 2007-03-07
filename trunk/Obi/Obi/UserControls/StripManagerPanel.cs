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

        private bool mAllowShortcuts;  // allow handling of shortcut keys

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
                if (value is SectionNode)
                {
                    _SelectedPhraseNode = null;
                    _SelectedSectionNode = (SectionNode)value;
                }
                else if (value is PhraseNode)
                {
                    _SelectedSectionNode = null;
                    _SelectedPhraseNode = (PhraseNode)value;
                }
                else
                {
                    _SelectedSectionNode = null;
                    _SelectedPhraseNode = null;
                }
                if (mProjectPanel != null) mProjectPanel.SelectedNode = value;
            }
        }

        /// <summary>
        /// Get the section node for the currently selected strip.
        /// Return null if no strip is selected.
        /// </summary>
        public SectionNode SelectedSectionNode
        {
            get { return mSelectedSection; }
        }

        private SectionNode _SelectedSectionNode
        {
            set
            {
                if (value != mSelectedSection)
                {
                    Deselect();
                    mSelectedSection = value;
                    if (value != null)
                    {
                        mSectionNodeMap[mSelectedSection].Selected = true;
                        SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(true, mSectionNodeMap[mSelectedSection]));
                    }
                }
            }
        }

        /// <summary>
        /// The currently selected phrase (null if none.)
        /// </summary>
        public PhraseNode SelectedPhraseNode
        {
            get { return mSelectedPhrase; }
        }

        private PhraseNode _SelectedPhraseNode
        {
            set
            {
                if (value != mSelectedPhrase)
                {
                    Deselect();
                    mSelectedPhrase = value;
                    if (value != null)
                    {
                        mPhraseNodeMap[mSelectedPhrase].Selected = true;
                        SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(true, mPhraseNodeMap[mSelectedPhrase]));
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

        /// <summary>
        /// True if there is a selected block and it has no page number set.
        /// </summary>
        public bool CanSetPage
        {
            get { return mSelectedPhrase != null && mSelectedPhrase.PageProperty == null; }
        }

        /// <summary>
        /// True if there is a selected block and it has a page set.
        /// </summary>
        public bool CanRemovePage
        {
            get { return mSelectedPhrase != null && mSelectedPhrase.PageProperty != null; }
        }

        /// <summary>
        /// Allow handling of shortcut keys (arrows, delete, etc.)
        /// </summary>
        public bool AllowShortcuts
        {
            set
            {
                mAllowShortcuts = value;
                ((ObiForm)ParentForm).AllowDelete = value;
                mDeleteStripToolStripMenuItem.Enabled = value;
                mDeleteAudioBlockToolStripMenuItem.Enabled = value;
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
            InitializeShortcutKeys();
            // The panel is empty and nothing is selected.
            mSectionNodeMap = new Dictionary<SectionNode, SectionStrip>();
            mSelectedSection = null;
            mPhraseNodeMap = new Dictionary<PhraseNode, AudioBlock>();
            mSelectedPhrase = null;
            mAllowShortcuts = true;
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
            SelectedNode = null;
        }

        #region shortcut keys

        public delegate bool HandledShortcutKey();                   // for keyboard shortcuts
        private Dictionary<Keys, HandledShortcutKey> mShortcutKeys;  // list of all shortcuts

        private void InitializeShortcutKeys()
        {
            mShortcutKeys = new Dictionary<Keys, HandledShortcutKey>();

            // Transport bar
            mShortcutKeys[Keys.Space] = delegate() { mProjectPanel.TransportBar.Play(); return true; };
            mShortcutKeys[Keys.Escape] = delegate() { mProjectPanel.TransportBar.Stop(); return true; };

            // Strip manager navigation
            mShortcutKeys[Keys.Left] = delegate() { mProjectPanel.StripManager.PreviousPhrase(); return true; };
            mShortcutKeys[Keys.Right] = delegate() { mProjectPanel.StripManager.NextPhrase(); return true; };
            mShortcutKeys[Keys.Up] = delegate() { mProjectPanel.StripManager.PreviousSection(); return true; };
            mShortcutKeys[Keys.Down] = delegate() { mProjectPanel.StripManager.NextSection(); return true; };
        }

        private static readonly int WM_KEYDOWN = 0x100;
        private static readonly int WM_SYSKEYDOWN = 0x104;

        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                if (mAllowShortcuts && mShortcutKeys.ContainsKey(key) && mShortcutKeys[key]()) return true;
            }
            return base.ProcessCmdKey(ref msg, key);
        }

        #endregion

        /// <summary>
        /// Deselect all items (prior to setting the new selection.)
        /// </summary>
        private void Deselect()
        {
            // Deselect the currently selected item
            if (mSelectedSection != null)
            {
                mSectionNodeMap[mSelectedSection].Selected = false;
                SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(false, mSectionNodeMap[mSelectedSection]));
            }
            else if (mSelectedPhrase != null)
            {
                mPhraseNodeMap[mSelectedPhrase].Selected = false;
                SelectionChanged(this, new Obi.Events.Node.SelectedEventArgs(false, mPhraseNodeMap[mSelectedPhrase]));
            }
            else
            {
                mProjectPanel.DeselectEverythingElse(this);
            }
        }
    }
}
