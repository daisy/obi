using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using urakawa.core;
using urakawa.core.visitor;
using urakawa.media;

namespace Obi.UserControls
{
    /// <summary>
    /// This control is a view of all the contents of a project
    /// </summary>
    public partial class StripManagerPanel : UserControl, ITreeNodeVisitor, IControlWithSelection
    {
        private ProjectPanel mProjectPanel;                             //the parent of this control
        private Dictionary<SectionNode, SectionStrip> mSectionNodeMap;  // find a section strip for a given node
        private Dictionary<PhraseNode, AudioBlock> mPhraseNodeMap;      // find an audio block for a given phrase node
        private bool mAllowShortcuts;                                   // allow handling of shortcut keys

        /// <summary>
        /// Create an empty manager panel.
        /// </summary>
        public StripManagerPanel()
        {
            InitializeComponent();
            InitializeShortcutKeys();
            mSectionNodeMap = new Dictionary<SectionNode, SectionStrip>();
            mPhraseNodeMap = new Dictionary<PhraseNode, AudioBlock>();
            mAllowShortcuts = true;
        }


        #region selection

        public ObiNode Selection
        {
            get
            {
                return mProjectPanel.CurrentSelection != null &&
                    mProjectPanel.CurrentSelection.Control == this ?
                    mProjectPanel.CurrentSelection.Node : null;
            }
            set
            {
                if (value == null)
                {
                    Deselect();
                }
                else if (value is PhraseNode)
                {
                    mPhraseNodeMap[(PhraseNode)value].Selected = true;
                }
                else if (value is SectionNode)
                {
                    mSectionNodeMap[(SectionNode)value].Selected = true;
                }
            }
        }

        private void Deselect()
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null &&
                mPhraseNodeMap.ContainsKey((PhraseNode)mProjectPanel.CurrentSelectedAudioBlock))
            {
                mPhraseNodeMap[(PhraseNode)mProjectPanel.CurrentSelectedAudioBlock].Selected = false;
            }
            else if (mProjectPanel.CurrentSelectedStrip != null &&
                mSectionNodeMap.ContainsKey((SectionNode)mProjectPanel.CurrentSelectedStrip))
            {
                mSectionNodeMap[(SectionNode)mProjectPanel.CurrentSelectedStrip].Selected = false;
            }
        }

        public SectionNode SelectedSectionNode
        {
            get
            {
                return mProjectPanel.CurrentSelection != null &&
                    mProjectPanel.CurrentSelection.Control == this ?
                    mProjectPanel.CurrentSelection.Node as SectionNode : null;
            }
        }

        public PhraseNode SelectedPhraseNode
        {
            get
            {
                return mProjectPanel.CurrentSelection != null &&
                    mProjectPanel.CurrentSelection.Control == this ?
                    mProjectPanel.CurrentSelection.Node as PhraseNode : null;
            }
        }

        #endregion


        #region properties

        /// <summary>
        /// Get the context menu strip of this StripManagerPanel
        /// </summary>
        /// <remarks>mg: for access by sectionstrip that needs to override the textbox menu</remarks> 
        public ContextMenuStrip PanelContextMenuStrip
        {
            get { return this.mContextMenuStrip; }
        }

        /// <summary>
        /// Get and set the parent ProjectPanel control 
        /// </summary>
        // mg 20060804
        internal ProjectPanel ProjectPanel
        {
            get { return mProjectPanel; }
            set { mProjectPanel = value; }
        }

        /// <summary>
        /// True if there is a selected block and it has no page number set.
        /// </summary>
        public bool CanSetPage
        {
            get
            {
                return mProjectPanel.CurrentSelectedAudioBlock != null &&
                    mProjectPanel.CurrentSelectedAudioBlock.PageProperty == null;
            }
        }

        /// <summary>
        /// True if there is a selected block and it has a page set.
        /// </summary>
        public bool CanRemovePage
        {
            get
            {
                return mProjectPanel.CurrentSelectedAudioBlock != null &&
                    mProjectPanel.CurrentSelectedAudioBlock.PageProperty != null;
            }
        }

        /// <summary>
        /// Allow handling of shortcut keys (arrows, delete, etc.)
        /// </summary>
        public bool AllowShortcuts
        {
            get
            {
                return  mAllowShortcuts; // Avn: Getter added to use this property outside this class
            }
            set
            {
                if (ParentForm != null) //won't set anything untill we have a parent form!
                {
                    mAllowShortcuts = value;
                    ((ObiForm)ParentForm).AllowDelete = value;
                    mDeleteStripToolStripMenuItem.Enabled = value;
                    mDeleteAudioBlockToolStripMenuItem.Enabled = value;
                }
            }
        }

        private bool mEnabledTooltips;

        public bool EnableTooltips
        {
            get { return mEnabledTooltips; }
            set
            {
                mEnabledTooltips = value;
                foreach (SectionStrip strip in mSectionNodeMap.Values) strip.EnableTooltips = value;
                foreach (AudioBlock block in mPhraseNodeMap.Values) block.EnableTooltips = value;
            }
        }

        #endregion

        /// <summary>
        /// Synchronize the strips view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ITreeNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(urakawa.core.TreeNode root)
        {
            mFlowLayoutPanel.Controls.Clear();
            mSectionNodeMap.Clear();
            root.acceptDepthFirst(this);
        }

        #region Synchronization visitor

        private urakawa.core.TreeNode parentSection;  // the current parent section to add phrases to
        private urakawa.core.TreeNode parentPhrase;   // the current phrase node to add structure nodes to

        /// <summary>
        /// Update the parent section to attach phrase nodes to.
        /// </summary>
        /// <param name="node">The node to do nothing with.</param>
        public void postVisit(urakawa.core.TreeNode node)
        {
            parentSection = (urakawa.core.TreeNode)node.getParent();
        }

        /// <summary>
        /// Create a new strip for every core node.
        /// Skip the root node and nodes that do not have a text channel (i.e. phrase nodes.)
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True.</returns>
        public bool preVisit(urakawa.core.TreeNode node)
        {
            SectionStrip strip = null;
           
            //if node is root
            if (node.GetType() == Type.GetType("urakawa.core.TreeNode"))
            { 
                parentSection = null;
            }
            else if (node.GetType() == Type.GetType("Obi.SectionNode"))
            {
                strip = new SectionStrip();
                strip.Label = Project.GetTextMedia(node).getText();
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
            if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
                mProjectPanel.CurrentSelection = null;
            }
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
                UpdateEnabledItemsForContextMenu();
            }
            return base.ProcessCmdKey(ref msg, key);
        }

        #endregion
    }
}
