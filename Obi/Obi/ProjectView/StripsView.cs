using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;

namespace Obi.ProjectView
{
    /// <summary>
    /// Common interface for selection of strips and blocks
    /// </summary>
    public interface ISelectableInStripView
    {
        bool Selected { get; set; }
        bool Highlighted { get; set; }
        ObiNode ObiNode { get; }
    }

    public partial class StripsView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mView;                                   // parent project view
        private ISelectableInStripView mSelectedItem;                // selected strip or block
        private ISelectableInStripView mHighlightedItem;             // highlighted strip or block
        private Dictionary<Keys, HandledShortcutKey> mShortcutKeys;  // list of all shortcuts

        public delegate bool HandledShortcutKey();

        /// <summary>
        /// A new strips view.
        /// </summary>
        public StripsView()
        {
            InitializeComponent();
            InitializeShortcutKeys();
            mView = null;
            mSelectedItem = null;
        }


        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
            }
        }


        public bool CanAddStrip { get { return mSelectedItem is Strip; } }
        public bool CanRemoveBlock { get { return mSelectedItem is Block; } }
        public bool CanRemoveStrip { get { return mSelectedItem is Strip; } }
        public bool CanRenameStrip { get { return mSelectedItem is Strip; } }
        public bool CanSplitStrip { get { return SelectedPhrase != null && SelectedPhrase.Index > 0; } }

        public bool CanMergeWithNextStrip
        {
            get
            {
                return mSelectedItem is Strip &&
                    mLayoutPanel.Controls.IndexOf((Control)mSelectedItem) < mLayoutPanel.Controls.Count - 1;
            }
        }

        /// <summary>
        /// Create a command to delete the selected strip.
        /// </summary>
        public urakawa.undo.ICommand DeleteStripCommand()
        {
            return DeleteStripCommand(SelectedSection);
        }

        private urakawa.undo.ICommand DeleteStripCommand(SectionNode section)
        {
            Commands.Node.Delete delete = new Commands.Node.Delete(mView, section, Localizer.Message("delete_strip_command"));
            if (section.SectionChildCount > 0)
            {
                urakawa.undo.CompositeCommand command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(delete.getShortDescription());
                command.append(new Commands.TOC.MoveSectionOut(mView, section.SectionChild(0)));
                command.append(delete);
                return command;
            }
            else
            {
                return delete;
            }
        }

        /// <summary>
        /// Show the strip for this section node.
        /// </summary>
        public void MakeStripVisibleForSection(SectionNode section)
        {
            if (section != null) mLayoutPanel.ScrollControlIntoView(FindStrip(section));
        }

        /// <summary>
        /// Get a command to merge the selected strip with the next one.
        /// </summary>
        public urakawa.undo.ICommand MergeSelectedStripWithNextCommand()
        {
            urakawa.undo.CompositeCommand command = null;
            if (CanMergeWithNextStrip)
            {
                command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("merge_strips_command"));
                SectionNode section = SelectedSection;
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild(0);
                for (int i = 0; i < next.PhraseChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, next.PhraseChild(i), section)); 
                }
                command.append(DeleteStripCommand(next));
            }
            return command;
        }

        /// <summary>
        /// Set a new presentation for this view.
        /// </summary>
        public void NewPresentation()
        {
            mLayoutPanel.Controls.Clear();
            AddStripForSection(mView.Presentation.RootNode);
            mView.Presentation.treeNodeAdded += new TreeNodeAddedEventHandler(Presentation_treeNodeAdded);
            mView.Presentation.treeNodeRemoved += new TreeNodeRemovedEventHandler(Presentation_treeNodeRemoved);
            mView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
        }

        /// <summary>
        /// Rename a strip.
        /// </summary>
        public void RenameStrip(Strip strip)
        {
            mView.RenameSectionNode(strip.Node, strip.Label);
        }

        /// <summary>
        /// Set the selected phrase (null to deselect)
        /// </summary>
        public PhraseNode SelectedPhrase
        {
            get { return mSelectedItem != null && mSelectedItem is Block ? ((Block)mSelectedItem).Node : null; }
            set { if (mView != null) mView.Selection = new NodeSelection(value, this, false); }
        }

        /// <summary>
        /// Set the highlighted phrase (null to cancel)
        /// </summary>
        public PhraseNode HighlightedPhrase
        {
            get { return mHighlightedItem != null && mHighlightedItem is Block ? ((Block)mHighlightedItem).Node : null; }
            set { if (mView != null) mView.Highlight = new NodeSelection(value, this); }
        }

        /// <summary>
        /// Set the selected section (null to deselect)
        /// </summary>
        public SectionNode SelectedSection
        {
            get { return mSelectedItem != null && mSelectedItem is Strip ? ((Strip)mSelectedItem).Node : null; }
            set { if (mView != null) mView.Selection = new NodeSelection(value, this, false); }
        }

        /// <summary>
        /// Set the highlighted section (null to cancel highlight)
        /// </summary>
        public SectionNode HighlightedSection
        {
            get { return mHighlightedItem != null && mHighlightedItem is Strip ? ((Strip)mHighlightedItem).Node : null; }
            set { if (mView != null) mView.Highlight = new NodeSelection(value, this); }
        }

        /// <summary>
        /// Set the selection from the parent view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelectedItem == null ? null : new NodeSelection(mSelectedItem.ObiNode, this, false); }
            set
            {
                ISelectableInStripView s = value == null ? null : FindSelectable(value.Node);
                if (s != mSelectedItem)
                {
                    if (mSelectedItem != null) mSelectedItem.Selected = false;
                    mSelectedItem = s;
                    if (s != null)
                    {
                        s.Selected = true;
                        mLayoutPanel.ScrollControlIntoView((Control)s);
                        SectionNode section = value.Node is SectionNode ? (SectionNode)value.Node :
                            value.Node is PhraseNode ? ((PhraseNode)value.Node).ParentAs<SectionNode>() : null;
                        mView.MakeTreeNodeVisibleForSection(section);
                        if (!((Control)s).Focused) ((Control)s).Focus();
                    }
                }
            }
        }

        public NodeSelection Highlight
        {
            get { return mHighlightedItem == null ? null : new NodeSelection(mHighlightedItem.ObiNode, this); }
            set
            {
                ISelectableInStripView s = value == null ? null : FindSelectable(value.Node);
                if (s != mHighlightedItem)
                {
                    if (mHighlightedItem != null) mHighlightedItem.Highlighted = false;
                    mHighlightedItem = s;
                    if (s != null)
                    {
                        s.Highlighted = true;
                        mLayoutPanel.ScrollControlIntoView((Control)s);
                        SectionNode section = value.Node is SectionNode ? (SectionNode)value.Node :
                            value.Node is PhraseNode ? ((PhraseNode)value.Node).ParentAs<SectionNode>() : null;
                        mView.MakeStripVisibleForSection(section);
                        if (!((Control)s).Focused) ((Control)s).Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Show/hide strips under the one for which the section was collapsed or expanded.
        /// </summary>
        public void SetStripsVisibilityForSection(SectionNode section, bool visible)
        {
            for (int i = 0; i < section.SectionChildCount; ++i)
            {
                Strip s;
                SectionNode child = section.SectionChild(i);
                if ((s = FindStrip(child)) != null)
                {
                    s.Visible = visible;
                    if (mSelectedItem == s && !visible) mView.Selection = null;
                    SetStripsVisibilityForSection(section.SectionChild(i), visible);
                }
            }
        }

        /// <summary>
        /// Split a strip at the given block; i.e. create a new sibling section which inherits the children of
        /// the split section except for the phrases before the selected block. Do not do anything if there are
        /// no phrases before.
        /// </summary>
        public urakawa.undo.CompositeCommand SplitStripFromSelectedBlockCommand()
        {
            urakawa.undo.CompositeCommand command = null;
            if (CanSplitStrip)
            {
                PhraseNode phrase = SelectedPhrase;
                SectionNode section = phrase.ParentAs<SectionNode>();
                command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("split_strip_command"));
                SectionNode sibling = mView.Presentation.CreateSectionNode();
                sibling.Label = section.Label;
                for (int i = 0; i < section.SectionChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, section.SectionChild(i), sibling));
                }
                for (int i = phrase.Index; i < section.PhraseChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, section.PhraseChild(i), sibling, phrase.Index));
                }
                command.append(new Commands.Node.AddNode(mView, sibling, section.ParentAs<ObiNode>(),
                    section.Index + 1));
            }
            return command;
        }

        /// <summary>
        /// Views are not synchronized anymore, so make sure that all strips are visible.
        /// </summary>
        public void UnsyncViews()
        {
            foreach (Control c in mLayoutPanel.Controls) c.Visible = true;
        }


        #region Event handlers

        // Handle resizing of the layout panel: all strips are resized to be at least as wide.
        private void mLayoutPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in mLayoutPanel.Controls)
            {
                int w = mLayoutPanel.Width - c.Location.X - c.Margin.Right;
                c.Size = new Size(w, c.Height);
            }
        }

        // Handle section nodes renamed from the project: change the label of the corresponding strip.
        private void Presentation_RenamedSectionNode(object sender, NodeEventArgs<SectionNode> e)
        {
            Strip strip = FindStrip(e.Node);
            strip.Label = e.Node.Label;
        }

        // Handle change of used status
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (e.Node is SectionNode)
            {
                Strip strip = FindStrip((SectionNode)e.Node);
                if (strip != null) strip.UpdateColors();
            }
        }

        // Handle addition of tree nodes: add a new strip for new section nodes.
        private void Presentation_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                SectionNode section = (SectionNode)e.getTreeNode();
                if (section.IsRooted)
                {
                    Strip strip = AddStripForSection(section);
                    mLayoutPanel.ScrollControlIntoView(strip);
                    UpdateTabIndex(strip);
                }
            }
            else if (e.getTreeNode() is PhraseNode)
            {
                PhraseNode phrase = (PhraseNode)e.getTreeNode();
                if (phrase.IsRooted)
                {
                    Block block = FindStrip(phrase.ParentAs<SectionNode>()).AddBlockForPhrase(phrase);
                    mLayoutPanel.ScrollControlIntoView(block);
                    UpdateTabIndex(block);
                }
            }
        }

        // Add a new strip for a section and all of its subsections
        private Strip AddStripForSection(ObiNode node)
        {
            Strip strip = null;
            if (node is SectionNode)
            {
                strip = new Strip((SectionNode)node, this);
                mLayoutPanel.Controls.Add(strip);
                mLayoutPanel.Controls.SetChildIndex(strip, ((SectionNode)node).Position);
                int w = mLayoutPanel.Width - strip.Location.X - strip.Margin.Right;
                strip.Size = new Size(w, strip.Height);
            }
            for (int i = 0; i < node.SectionChildCount; ++i) AddStripForSection(node.SectionChild(i));
            for (int i = 0; i < node.PhraseChildCount; ++i) strip.AddBlockForPhrase(node.PhraseChild(i));
            return strip;
        }

        // Handle removal of tree nodes: remove a strip for a section node and all of its children.
        void Presentation_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                RemoveStripsForSection((SectionNode)e.getTreeNode());
            }
            else if (e.getTreeNode() is PhraseNode)
            {
                Strip strip = FindStrip((SectionNode)e.getFormerParent());
                if (strip != null) strip.RemoveBlock((PhraseNode)e.getTreeNode());
            }
        }

        // Remove all strips for a section and its subsections
        private void RemoveStripsForSection(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) RemoveStripsForSection(section.SectionChild(i));
            Strip strip = FindStrip(section);
            mLayoutPanel.Controls.Remove(strip);
        }


        // Deselect everything when clicking the panel
        private void mLayoutPanel_Click(object sender, EventArgs e)
        {
            mView.Selection = null;
        }

        #endregion


        #region Utility functions

        private Block FindBlock(PhraseNode node)
        {
            return FindStrip(node.ParentAs<SectionNode>()).FindBlock(node);
        }

        private ISelectableInStripView FindSelectable(ObiNode node)
        {
            return node is SectionNode ? (ISelectableInStripView)FindStrip((SectionNode)node) :
                node is PhraseNode ? (ISelectableInStripView)FindBlock((PhraseNode)node) : null;
        }

        /// <summary>
        /// Find the strip for the given section node.
        /// The strip must be present so an exception is thrown on failure.
        /// </summary>
        private Strip FindStrip(SectionNode section)
        {
            foreach (Control c in mLayoutPanel.Controls)
            {
                if (c is Strip && ((Strip)c).Node == section) return c as Strip;
            }
            //throw new Exception(String.Format("Could not find strip for section node labeled `{0}'", section.Label));
            return null;
        }

        #endregion

        #region IControlWithRenamableSelection Members

        public void SelectAndRename(ObiNode node)
        {
            DoToNewNode(node, delegate()
            {
                mView.Selection = new NodeSelection(node, this, false);
                FindStrip((SectionNode)node).StartRenaming();
            });
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the view.
        private void DoToNewNode(ObiNode node, DoToNewNodeDelegate f)
        {
            if (IsInView(node))
            {
                f();
            }
            else
            {
                TreeNodeAddedEventHandler h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e) { };
                h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
                {
                    if (e.getTreeNode() == node)
                    {
                        f();
                        mView.Presentation.treeNodeAdded -= h;
                    }
                };
                mView.Presentation.treeNodeAdded += h;
            }
        }

        private bool IsInView(ObiNode node)
        {
            return node is SectionNode && FindStrip((SectionNode)node) != null;
        }

        #endregion




        // temporary for search
        public FlowLayoutPanel LayoutPanel { get { return mLayoutPanel; } }

        /// <summary>
        /// Get all the searchable items (i.e. strips; later blocks) in the control
        /// </summary>
        public List<ISearchable> Searchables
        {
            get
            {
                List<ISearchable> l = new List<ISearchable>(mLayoutPanel.Controls.Count);
                foreach (Control c in mLayoutPanel.Controls) if (c is ISearchable) l.Add((ISearchable)c);
                return l;
            }
        }

        #region tabbing

        private void StripsView_Leave(object sender, EventArgs e) { mView.Highlight = null; }

        // Update tab index for all controls after a newly added strip
        private void UpdateTabIndex(Strip strip)
        {
            int stripIndex = mLayoutPanel.Controls.IndexOf(strip);
            int tabIndex = stripIndex > 0 ? ((Strip)mLayoutPanel.Controls[stripIndex - 1]).LastTabIndex : 0;
            for (int i = stripIndex; i < mLayoutPanel.Controls.Count; ++i)
            {
                tabIndex = ((Strip)mLayoutPanel.Controls[i]).UpdateTabIndex(tabIndex);
            }
        }

        // Update tab index for all controls after a newly added block
        private void UpdateTabIndex(Block block) { UpdateTabIndex(block.Strip); }

        #endregion

        #region shortcut keys

        private void InitializeShortcutKeys()
        {
            mShortcutKeys = new Dictionary<Keys, HandledShortcutKey>();

            // Transport bar
            // mShortcutKeys[Keys.Space] = delegate() { mProjectPanel.TransportBar.Play(); return true; };
            // mShortcutKeys[Keys.Escape] = delegate() { mProjectPanel.TransportBar.Stop(); return true; };

            // playback shortcuts.
            // Note: if these shortcuts are to be disabled till finalisation, just comment following five lines.
            mShortcutKeys[Keys.Space] = TogglePlayPause;
            mShortcutKeys[Keys.S] = FastPlayRateStepDown;
            mShortcutKeys[Keys.F] = FastPlayRateStepUp;
            mShortcutKeys[Keys.D] = FastPlayRateNormalise;
            mShortcutKeys[Keys.E] = FastPlayNormaliseWithLapseBack;


            mShortcutKeys[Keys.Return] = SelectHighlighted;

            // Strips navigation
            mShortcutKeys[Keys.Left] = HighlightPrecedingBlock;
            mShortcutKeys[Keys.Right] = HighlightFollowingBlock;
            mShortcutKeys[Keys.End] = HighlightLastBlockInStrip;
            mShortcutKeys[Keys.Home] = HighlightFirstBlockInStrip;

            mShortcutKeys[Keys.Up] = HighlightPreviousStrip;
            mShortcutKeys[Keys.Down] = HighlightNextStrip;
            mShortcutKeys[Keys.Control | Keys.Home] = HighlightFirstStrip;
            mShortcutKeys[Keys.Control | Keys.End] = HighlightLastStrip;
        }

        private static readonly int WM_KEYDOWN = 0x100;
        private static readonly int WM_SYSKEYDOWN = 0x104;

        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            if (((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) &&
                mShortcutKeys.ContainsKey(key) && mShortcutKeys[key]()) return true;
            return base.ProcessCmdKey(ref msg, key);
        }

        private bool SelectHighlighted()
        {
            if (mHighlightedItem != null)
            {
                mView.Selection = mView.Highlight;
                return true;
            }
            return false;
        }

        // Get the strip for the current highlighted component (i.e. the strip itself, or the parent strip
        // for a block.)
        private Strip StripForHighlight
        {
            get
            {
                return mHighlightedItem is Strip ? (Strip)mHighlightedItem :
                    mHighlightedItem is Block ? ((Block)mHighlightedItem).Strip : null;
            }
        }

        // Highlight the previous block; do not cross strip boundaries; must have a block already highlighted.
        // If a strip is highlighted, then the last block is highlighted (or none if it has no block.)
        private bool HighlightPrecedingBlock()
        {
            if (StripForHighlight != null)
            {
                Block block = StripForHighlight.BlockBefore(mHighlightedItem);
                if (block != null)
                {
                    mView.Highlight = new NodeSelection(block.Node, this);
                    return true;
                }
            }
            return false;
        }

        private bool HighlightFollowingBlock()
        {
            if (StripForHighlight != null)
            {
                Block block = StripForHighlight.BlockAfter(mHighlightedItem);
                if (block != null)
                {
                    mView.Highlight = new NodeSelection(block.Node, this);
                    return true;
                }
            }
            return false;
        }

        private bool HighlightLastBlockInStrip()
        {
            if (StripForHighlight != null)
            {
                Block block = StripForHighlight.BlockLast();
                if (block.Node != null)
                {
                    mView.Highlight = new NodeSelection(block.Node, this);
                    return true;
                }
            }
            return false;
        }

        private bool HighlightFirstBlockInStrip()
        {
            if (StripForHighlight != null)
            {
                Block block = StripForHighlight.BlockFirst();
                if (block.Node != null)
                {
                    mView.Highlight = new NodeSelection(block.Node, this);
                    return true;
                }
            }
            return false;
        }

        private bool HighlightPreviousStrip()
        {
            Strip strip = mHighlightedItem is Block ? StripForHighlight : StripBefore(StripForHighlight);
            if (strip != null)
            {
                mView.Highlight = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }

        private bool HighlightNextStrip()
        {
            Strip strip = StripAfter(StripForHighlight);
            if (strip != null)
            {
                mView.Highlight = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }

        private bool HighlightFirstStrip()
        {
            Strip strip = (Strip)mLayoutPanel.Controls[0];
            if (strip != null)
            {
                mView.Highlight = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }

        private bool HighlightLastStrip()
        {
            Strip strip = (Strip)mLayoutPanel.Controls[mLayoutPanel.Controls.Count - 1];
            if (strip != null)
            {
                mView.Highlight = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }


        private Strip StripAfter(Strip strip)
        {
            if (strip != null)
            {
                int count = mLayoutPanel.Controls.Count;
                int index = 1 + mLayoutPanel.Controls.IndexOf(strip);
                return index < count ? (Strip)mLayoutPanel.Controls[index] : null;
            }
            return null;
        }

        public Strip StripBefore(Strip strip)
        {
            if (strip != null)
            {
                int index = mLayoutPanel.Controls.IndexOf(strip);
                return index > 0 ? (Strip)mLayoutPanel.Controls[index - 1] : null;
            }
            return null;
        }


        /// <summary>
        /// Toggles play selection and pause with spacebar
        /// In this function Pause works both for play selection and Play all
        /// <see cref=""/>
        /// </summary>
        /// <returns></returns>
        private bool TogglePlayPause()
        {
            if (mView.TransportBar.CurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped
                || mView.TransportBar.CurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mView.TransportBar.Play(mView.Selection.Node);
                return true;
            }
            else if (mView.TransportBar.CurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
            {
                mView.TransportBar.Pause();
                return true;
            }
            return false;
        }


        private bool FastPlayRateStepDown()
        {
            return mView.TransportBar.FastPlayRateStepDown();
        }

        private bool FastPlayRateStepUp()
        {
            return mView.TransportBar.FastPlayRateStepUp();
        }

        private bool FastPlayRateNormalise()
        {
            return mView.TransportBar.FastPlayRateNormalise();
        }

        private bool FastPlayNormaliseWithLapseBack()
        {
            return mView.TransportBar.FastPlayNormaliseWithLapseBack();
        }

        #endregion
    }
}