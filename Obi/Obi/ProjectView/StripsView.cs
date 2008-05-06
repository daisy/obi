using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    /// <summary>
    /// Common interface for selection of strips and blocks
    /// </summary>
    public interface ISelectableInStripView
    {
        bool Selected { get; set; }               // set the selected state of the control
        ObiNode ObiNode { get; }                  // get the Obi node for the control
        NodeSelection SelectionFromView { set; }  // used by the parent view to set the selection 
    }

    public partial class StripsView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mView;                                   // parent project view
        private NodeSelection mSelection;                            // current selection
        private ISelectableInStripView mSelectedItem;                // the actual item for the selection
        private Dictionary<Keys, HandledShortcutKey> mShortcutKeys;  // list of all shortcuts

        // cursor stuff
        private AudioBlock mPlaybackBlock;
        private bool mFocusing;

        /// <summary>
        /// A new strips view.
        /// </summary>
        public StripsView()
        {
            InitializeComponent();
            InitializeShortcutKeys();
            mView = null;
            mSelection = null;
            mFocusing = false;
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

        private bool BlockSelected { get { return mSelectedItem is Block && mSelection.GetType() == typeof(NodeSelection); } }
        private bool BlockOrWaveformSelected { get { return mSelectedItem is Block; } }
        private bool StripSelected { get { return mSelectedItem is Strip && mSelection.GetType() == typeof(NodeSelection); } }

        private bool IsAudioRangeSelected { get { return mSelection is AudioSelection && !((AudioSelection)mSelection).AudioRange.HasCursor; } }

        public bool CanAddStrip { get { return StripSelected; } }

        public bool CanCopyAudio { get { return IsAudioRangeSelected; } }
        public bool CanCopyBlock { get { return BlockSelected; } }
        public bool CanCopyStrip { get { return StripSelected; } }
        public bool CanRemoveAudio { get { return IsAudioRangeSelected; } }
        public bool CanRemoveBlock { get { return BlockSelected; } }
        public bool CanRemoveStrip { get { return StripSelected; } }
        public bool CanRenameStrip { get { return StripSelected; } }

        public bool IsStripCursorSelected { get { return mSelection is StripCursorSelection; } }

        public bool CanSplitStrip
        {
            get
            {
                return (BlockSelected && SelectedEmptyNode.Index > 0)                            // block selected
                    || (IsStripCursorSelected && ((StripCursorSelection)mSelection).Index > 0);  // strip cursor selected
            }
        }

        public bool CanSetBlockUsedStatus { get { return BlockOrWaveformSelected && mSelection.Node.ParentAs<ObiNode>().Used; } }

        public bool CanSetStripUsedStatus { get { return StripSelected && mSelection.Node.SectionChildCount == 0; } }

        public bool CanMergeBlockWithNext
        {
            get
            {
                EmptyNode node = mSelectedItem is Block ? ((Block)mSelectedItem).Node : null;
                return node != null
                    // && node is PhraseNode && node.FollowingNode is PhraseNode 
                    && node.Index < node.ParentAs<ObiNode>().PhraseChildCount - 1;
            }
        }

        public bool CanMergeStripWithNext
        {
            get
            {
                return StripSelected && (((SectionNode)mSelection.Node).PhraseChildCount > 0 &&
                    mSelection.Node.Index < mSelection.Node.ParentAs<ObiNode>().SectionChildCount - 1);
            }
        }

        public bool Focusing { get { return mFocusing; } }

        public AudioBlock PlaybackBlock { get { return mPlaybackBlock; } }
        public Strip PlaybackStrip { get { return mPlaybackBlock == null ? null : mPlaybackBlock.Strip; } }

        public PhraseNode PlaybackPhrase
        {
            get { return mPlaybackBlock.Node as PhraseNode; }
            set
            {
                if (mPlaybackBlock != null) mPlaybackBlock.ClearCursor();
                mPlaybackBlock = value == null ? null : (AudioBlock)FindBlock(value);
                if (mPlaybackBlock != null)
                {
                    mLayoutPanel.ScrollControlIntoView(mPlaybackBlock);
                    mPlaybackBlock.InitCursor();
                }
            }
        }

        public void UpdateCursorPosition(double time) { mPlaybackBlock.UpdateCursorTime(time); }

        /// <summary>
        /// Create a command to delete the selected strip.
        /// </summary>
        public urakawa.undo.ICommand DeleteStripCommand()
        {
            return DeleteStripCommand(SelectedSection);
        }

        private urakawa.undo.ICommand DeleteStripCommand(SectionNode section)
        {
            Commands.Node.Delete delete = new Commands.Node.Delete(mView, section, Localizer.Message("delete_section_shallow"));
            if (section.SectionChildCount > 0)
            {
                urakawa.undo.CompositeCommand command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(delete.getShortDescription());
                for (int i = 0; i < section.SectionChildCount; ++i)
                {
                    command.append(new Commands.TOC.MoveSectionOut(mView, section.SectionChild(i)));
                }
                command.append(delete);
                return command;
            }
            else
            {
                return delete;
            }
        }

        /// <summary>
        /// True if a block is selected and it is used.
        /// </summary>
        public bool IsBlockUsed { get { return BlockOrWaveformSelected && mSelection.Node.Used; } }

        /// <summary>
        /// True if the strip where the selection is used.
        /// </summary>
        public bool IsStripUsed
        {
            get
            {
                return mSelection == null ? false :
                    mSelection.Node is SectionNode ? mSelection.Node.Used :
                        mSelection.Node.AncestorAs<SectionNode>().Used;
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
        /// Get a command to merge the selected strip with the next one. If the next strip is a child or a sibling, then
        /// its contents are appended to the selected strip and it is removed from the project; but if the next strip has
        /// a lower level, merging is not possible.
        /// </summary>
        public urakawa.undo.ICommand MergeSelectedStripWithNextCommand()
        {
            urakawa.undo.CompositeCommand command = null;
            if (CanMergeStripWithNext)
            {
                command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("merge_sections"));
                SectionNode section = SelectedSection;
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild(0);
                if (!section.Used) mView.AppendMakeUnused(command, next);
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
            mView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_changed);
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

        public EmptyNode SelectedEmptyNode { get { return BlockSelected ? ((Block)mSelectedItem).Node : null; } }
        public PhraseNode SelectedPhraseNode { get { return BlockSelected ? ((Block)mSelectedItem).Node as PhraseNode : null; } }
        public SectionNode SelectedSection { get { return StripSelected ? ((Strip)mSelectedItem).Node : null; } }
        public ObiNode SelectedNode { set { if (mView != null) mView.Selection = new NodeSelection(value, this); } }
        public NodeSelection SelectionFromStrip { set { if (mView != null) mView.Selection = value; } }

        /// <summary>
        /// Set the selection from the parent view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (value != mSelection)
                {
                    ISelectableInStripView s = value == null ? null : FindSelectable(value);
                    if (mSelectedItem != null) mSelectedItem.Selected = false;
                    mSelection = value;
                    mSelectedItem = s;
                    if (s != null)
                    {
                        s.SelectionFromView = mSelection;
                        mLayoutPanel.ScrollControlIntoView((Control)s);
                        SectionNode section = value.Node is SectionNode ? (SectionNode)value.Node :
                            value.Node.ParentAs<SectionNode>();
                        mView.MakeTreeNodeVisibleForSection(section);
                        mFocusing = true;
                        if (!((Control)s).Focused) ((Control)s).Focus();
                        mFocusing = false;
                    }
                }
            }
        }

        public void SetStripVisibilityForSection(SectionNode node, bool visible)
        {
            Strip s = FindStrip(node);
            if (s != null) s.Visible = visible;
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
        /// Split a strip at the selected block or cursor position; i.e. create a new sibling section which
        /// inherits the children of the split section except for the phrases before the selected block or
        /// position. Do not do anything if there are no phrases before.
        /// </summary>
        public urakawa.undo.CompositeCommand SplitStripCommand()
        {
            urakawa.undo.CompositeCommand command = null;
            if (CanSplitStrip)
            {
                EmptyNode node = IsStripCursorSelected ?
                    mSelection.Node.PhraseChild(((StripCursorSelection)mSelection).Index) : (EmptyNode)mSelection.Node;
                SectionNode section = node.ParentAs<SectionNode>();
                command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("split_section"));
                SectionNode sibling = mView.Presentation.CreateSectionNode();
                sibling.Label = section.Label;
                Commands.Node.AddNode add = new Commands.Node.AddNode(mView, sibling, section.ParentAs<ObiNode>(),
                    section.Index + 1);
                add.UpdateSelection = false;
                command.append(add);
                for (int i = 0; i < section.SectionChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, section.SectionChild(i), sibling));
                }
                for (int i = node.Index; i < section.PhraseChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, section.PhraseChild(i), sibling, node.Index));
                }
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
            if (mLayoutPanel.Controls.Count > 0)
            {
                Control last = mLayoutPanel.Controls[mLayoutPanel.Controls.Count - 1];
                int scrollbarW = last.Location.Y + last.Height > Height ? SystemInformation.VerticalScrollBarWidth : 0;
                foreach (Control c in mLayoutPanel.Controls)
                {
                    int w = mLayoutPanel.Width - c.Location.X - c.Margin.Right - scrollbarW;
                    c.Size = new Size(w, c.Height);
                }
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
            else if (e.Node is EmptyNode)
            {
                Block block = FindBlock((EmptyNode)e.Node);
                if (block != null) block.UpdateColors();
            }
        }

        // Listen to changes in the presentation
        private void Presentation_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (e is urakawa.events.core.ChildAddedEventArgs)
            {
                TreeNodeAdded((urakawa.events.core.ChildAddedEventArgs)e);
            }
            else if (e is urakawa.events.core.ChildRemovedEventArgs)
            {
                TreeNodeRemoved((urakawa.events.core.ChildRemovedEventArgs)e);
            }
        }

        private void TreeNodeAdded(urakawa.events.core.ChildAddedEventArgs e)
        {
            Control c = e.AddedChild is SectionNode ? (Control)AddStripForSection((SectionNode)e.AddedChild) :
                // TODO: in the future, the source node will not always be a section node!
                e.AddedChild is EmptyNode ? (Control)FindStrip((SectionNode)e.SourceTreeNode).AddBlockForNode((EmptyNode)e.AddedChild) :
                null;
            if (c != null)
            {
                mLayoutPanel.ScrollControlIntoView(c);
                UpdateTabIndex(c);
            }
        }

        // Remove the strip or block for the removed tree node
        private void TreeNodeRemoved(urakawa.events.core.ChildRemovedEventArgs e)
        {
            if (e.RemovedChild is SectionNode)
            {
                RemoveStripsForSection((SectionNode)e.RemovedChild);
            }
            else if (e.RemovedChild is EmptyNode)
            {
                // TODO in the future, the parent of a removed empty node
                // will not always be a section node!
                Strip strip = FindStrip((SectionNode)e.SourceTreeNode);
                if (strip != null) strip.RemoveBlock((EmptyNode)e.RemovedChild);
            }
        }

        // Add a new strip for a section and all of its subsections
        private Strip AddStripForSection(ObiNode node)
        {
            SuspendLayout();
            Strip strip = AddStripForSection_(node);
            ResumeLayout();
            return strip;
        }

        private Strip AddStripForSection_(ObiNode node)
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
            for (int i = 0; i < node.SectionChildCount; ++i) AddStripForSection_(node.SectionChild(i));
            for (int i = 0; i < node.PhraseChildCount; ++i) strip.AddBlockForNode(node.PhraseChild(i));
            return strip;
        }

        // Remove all strips for a section and its subsections
        private void RemoveStripsForSection(SectionNode section)
        {
            SuspendLayout();
            RemoveStripsForSection_(section);
            ResumeLayout();
        }

        private void RemoveStripsForSection_(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) RemoveStripsForSection_(section.SectionChild(i));
            Strip strip = FindStrip(section);
            mLayoutPanel.Controls.Remove(strip);
        }

        // Deselect everything when clicking the panel
        private void mLayoutPanel_Click(object sender, EventArgs e) { mView.Selection = null; }

        #endregion


        #region Utility functions

        // Find the block for the given node
        private Block FindBlock(EmptyNode node)
        {
            Strip strip = FindStrip(node.ParentAs<SectionNode>());
            return strip == null ? null : strip.FindBlock(node);
        }

        // Find the selectable item for this selection object
        private ISelectableInStripView FindSelectable(NodeSelection selection)
        {
            return selection is StripCursorSelection ? (ISelectableInStripView)
                    FindStrip((SectionNode)selection.Node).FindStripCursor((StripCursorSelection)selection) :
                selection == null ? null :
                selection.Node is SectionNode ? (ISelectableInStripView)FindStrip((SectionNode)selection.Node) :
                selection.Node is EmptyNode ? (ISelectableInStripView)FindBlock((EmptyNode)selection.Node) : null;
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
                mView.Selection = new NodeSelection(node, this);
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
                EventHandler<urakawa.events.DataModelChangedEventArgs> h =
                    delegate(object sender, urakawa.events.DataModelChangedEventArgs e) { };
                h = delegate(object sender, urakawa.events.DataModelChangedEventArgs e)
                {
                    if (e is urakawa.events.core.ChildAddedEventArgs &&
                        ((urakawa.events.core.ChildAddedEventArgs)e).AddedChild == node)
                    {
                        f();
                        mView.Presentation.changed -= h;
                    }
                };
                mView.Presentation.changed += h;
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
        /// Get all the searchable items (i.e. strips, blocks) in the control.  This does not support nested blocks right now.
        /// </summary>
        public List<ISearchable> Searchables
        {
            get
            {
                List<ISearchable> l = new List<ISearchable>();
                AddToSearchables(mLayoutPanel, l);
                return l;
            }
        }

        /// <summary>
        /// Recursive function to go through all the controls in-order and add the ISearchable ones to the list
        /// </summary>
        /// <param name="c"></param>
        /// <param name="searchables"></param>
        private void AddToSearchables(Control c, List<ISearchable> searchables)
        {
            if (c is ISearchable) searchables.Add((ISearchable)c);
            foreach (Control c2 in c.Controls) AddToSearchables(c2, searchables);
        }

        #region tabbing

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

        // Update tab index for all controls after a block or a strip
        private void UpdateTabIndex(Control c)
        {
            if (c is Block)
            {
                UpdateTabIndex(((Block)c).Strip);
            }
            else if (c is Strip)
            {
                UpdateTabIndex((Strip)c);
            }
        }

        #endregion

        #region shortcut keys

        public delegate bool HandledShortcutKey();  // key handling delegate

        private void InitializeShortcutKeys()
        {
            mShortcutKeys = new Dictionary<Keys, HandledShortcutKey>();

            mShortcutKeys[Keys.A] = MarkSelectionWholePhrase;
            //mShortcutKeys[Keys.Space] = TogglePlayPause;

            // playback shortcuts.
            mShortcutKeys[Keys.J] = NavigatePrevPhrase;
            mShortcutKeys[Keys.K] = NavigateNextPhrase;
            mShortcutKeys[Keys.Shift | Keys.H] = NavigatePrevSection;
            mShortcutKeys[Keys.H] = NavigateNextSection;
            mShortcutKeys[Keys.Shift | Keys.P] = NavigatePrevPage;
            mShortcutKeys[Keys.P] = NavigateNextPage;
            
            mShortcutKeys[Keys.S] = FastPlayRateStepDown;
            mShortcutKeys[Keys.F] = FastPlayRateStepUp;
            mShortcutKeys[Keys.D] = FastPlayRateNormalise;
            mShortcutKeys[Keys.E] = FastPlayNormaliseWithLapseBack;
            mShortcutKeys[Keys.OemOpenBrackets] = MarkSelectionBeginTime;
            mShortcutKeys[Keys.OemCloseBrackets] = MarkSelectionEndTime;
            mShortcutKeys[Keys.Shift | Keys.OemOpenBrackets] = MarkSelectionFromCursor;
            mShortcutKeys[Keys.Shift | Keys.OemCloseBrackets] = MarkSelectionToCursor;
            mShortcutKeys[Keys.X ] = PlayPreviewUptoCurrentPosition;
            mShortcutKeys[Keys.C] = PlayPreviewSelectedFragment;
            mShortcutKeys[Keys.V ] = PlayPreviewFromCurrentPosition;

            // Strips navigation
            mShortcutKeys[Keys.Left] = SelectPrecedingBlock;
            mShortcutKeys[Keys.Right] = SelectFollowingBlock;
            mShortcutKeys[Keys.End] = SelectLastBlockInStrip;
            mShortcutKeys[Keys.Home] = SelectFirstBlockInStrip;
            mShortcutKeys[ Keys.Control | Keys.PageDown] = SelectNextPageNode ;
            mShortcutKeys[Keys.Control |  Keys.PageUp] = SelectPrecedingPageNode;
            mShortcutKeys[Keys.F4] = SelectNextSpecialRoleNode ;
            mShortcutKeys[ Keys.Shift |  Keys.F4] = SelectPreviousSpecialRoleNode ;
            mShortcutKeys[Keys.Control | Keys.F9 ] = SelectNextTo_DoNode ;
            mShortcutKeys[Keys.Control | Keys.Shift |  Keys.F9] =  SelectPreviousTo_DoNode;

            mShortcutKeys[Keys.Up] = SelectPreviousStrip;
            mShortcutKeys[Keys.Down] = SelectNextStrip;
            mShortcutKeys[Keys.Control | Keys.Home] = SelectFirstStrip;
            mShortcutKeys[Keys.Control | Keys.End] = SelectLastStrip;

            mShortcutKeys[Keys.Escape] = SelectUp;

            // Control + arrows moves the strip cursor
            mShortcutKeys[Keys.Control | Keys.Left] = SelectPrecedingStripCursor;
            mShortcutKeys[Keys.Control | Keys.Right] = SelectFollowingStripCursor;
        }

        private static readonly int WM_KEYDOWN = 0x100;
        private static readonly int WM_SYSKEYDOWN = 0x104;

        private bool CanUseKeys { get { return mSelection == null || !(mSelection is TextSelection); } }

        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            if (CanUseKeys &&
                ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) &&
                mShortcutKeys.ContainsKey(key) && mShortcutKeys[key]()) return true;

            if (ProcessTabKeyInContentsView(key))
                return true;

            return base.ProcessCmdKey(ref msg, key);
        }

        // Get the strip for the currently selected component (i.e. the strip itself, or the parent strip
        // for a block.)
        private Strip StripFor(ISelectableInStripView item)
        {
            return item is Strip ? (Strip)item :
                   item is StripCursor ? ((StripCursor)item).Strip :
                   item is Block ? ((Block)item).Strip : null;
        }

        private delegate Block SelectBlockFunction(Strip strip, ISelectableInStripView item);

        private bool SelectBlockFor(SelectBlockFunction f)
        {
            Strip strip = StripFor(mSelectedItem);
            if (strip != null)
            {
                Block block = f(strip, mSelectedItem);
                if (block != null)
                {
                    mView.Selection = new NodeSelection(block.Node, this);
                    return true;
                }
            }
            return false;
        }

        private delegate int SelectStripCursorFunction(Strip strip, ISelectableInStripView item);

        private bool SelectStripCursorFor(SelectStripCursorFunction f)
        {
            System.Diagnostics.Debug.Print("SelectStripCursorFor {0} ...", mSelectedItem);
            Strip strip = StripFor(mSelectedItem);
            if (strip != null)
            {
                int index = f(strip, mSelectedItem);
                if (index >= 0)
                {
                    System.Diagnostics.Debug.Print("  ... got index {0}", index);
                    mView.Selection = new StripCursorSelection(strip.Node, this, index);
                    return true;
                }
            }
            return false;
        }

        private bool SelectPrecedingBlock()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.BlockBefore(item); });
        }

        private bool SelectPrecedingStripCursor()
        {
            return SelectStripCursorFor(delegate(Strip strip, ISelectableInStripView item) { return strip.StripCursorBefore(item); });
        }

        private bool SelectFollowingBlock()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.BlockAfter(item); });
        }

        private bool SelectFollowingStripCursor()
        {
            return SelectStripCursorFor(delegate(Strip strip, ISelectableInStripView item) { return strip.StripCursorAfter(item); });
        }

        private bool SelectLastBlockInStrip()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.LastBlock; });
        }

        private bool SelectFirstBlockInStrip()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.FirstBlock; });
        }

        private delegate Strip SelectStripFunction(Strip strip);

        private bool SelectStripFor(SelectStripFunction f)
        {
            Strip strip = f(StripFor(mSelectedItem) as Strip);
            if (strip != null)
            {
                mView.Selection = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }

        private bool SelectPreviousStrip()
        {
            Strip strip ;
            if ( mView.TransportBar.CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing
                &&    this.mPlaybackBlock.ObiNode.Index == 0 )  
            {
                 strip = StripBefore(StripFor(mSelectedItem)) ;
                            }
            else
            strip = mSelectedItem is Strip ? StripBefore(StripFor(mSelectedItem)) : StripFor(mSelectedItem);

            if (strip != null)
            {
                mView.Selection = new NodeSelection(strip.Node, this );
                strip.FocusStripLabel();
                return true;
            }
            return false;
        }

        private bool SelectNextStrip()
        {
            return SelectStripFor(StripAfter);
        }

        private bool SelectFirstStrip()
        {
            return SelectStripFor(delegate(Strip strip)
            {
                return mLayoutPanel.Controls.Count > 0 ? (Strip)mLayoutPanel.Controls[0] : null;
            });
        }

        private bool SelectLastStrip()
        {
            return SelectStripFor(delegate(Strip strip)
            {
                return mLayoutPanel.Controls.Count > 0 ? (Strip)mLayoutPanel.Controls[mLayoutPanel.Controls.Count - 1] :
                    null;
            });
        }

        // Select the item above the currently selected item.
        // E.g. from an audio selection a phrase, from a phrase a strip, from a strip nothing.
        private bool SelectUp()
        {
            if (mSelection is AudioSelection)
            {
                return SelectBlockFor(delegate(Strip s, ISelectableInStripView item) { return FindBlock((EmptyNode)mSelection.Node); });
            }
            else if (mSelectedItem is Block)
            {
                return SelectStripFor(delegate(Strip s) { return ((Block)mSelectedItem).Strip; });
            }
            else if (mSelectedItem is Strip)
            {
                mView.Selection = null;
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
        /// Moves keyboard focus to preceding page node.
        /// </summary>
        public bool SelectPrecedingPageNode()
        {
            if (mView.SelectedNodeAs<ObiNode>() != null)
            {
                for (ObiNode n = mView.SelectedNodeAs<ObiNode>().PrecedingNode; n != null; n = n.PrecedingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page)
                    {
                        mView.Selection = new NodeSelection(n, this);
                        return true;
                    }   
                }
            }
            return false;
        }

        /// <summary>
        /// Moves keyboard focus to the following page node.
        /// </summary>
        public bool SelectNextPageNode()
        {
            if (mView.SelectedNodeAs<EmptyNode>() != null)
            {
                for (ObiNode n = mView.SelectedNodeAs<EmptyNode>().FollowingNode; n != null; n = n.FollowingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page)
                    {
                        mView.Selection = new NodeSelection(n, this);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  Move keyboard focus to block with some special role
                /// </summary>
        /// <returns></returns>
        public bool SelectPreviousSpecialRoleNode()
        {
            if (mView.SelectedNodeAs<EmptyNode>() != null)
            {
                for (ObiNode n = mView.SelectedNodeAs<EmptyNode>().PrecedingNode; n != null; n = n.PrecedingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).NodeKind != EmptyNode.Kind.Plain)
                    {
                        mView.Selection = new NodeSelection(n, this);
                        return true;
                    }
                }
            } // check end for empty node
            return false;
        }


        /// <summary>
        /// Move keyboard focus to next block with special role 
                /// </summary>
        /// <returns></returns>
        public bool SelectNextSpecialRoleNode()
        {
            if (mView.SelectedNodeAs<EmptyNode>() != null)
            {
                for (ObiNode n = mView.SelectedNodeAs<EmptyNode>().FollowingNode; n != null; n = n.FollowingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).NodeKind != EmptyNode.Kind.Plain)
                    {
                        mView.Selection = new NodeSelection(n, this);
                        return true;
                    }
                }
            }// check ends for empty node
            return false;
        }


        /// <summary>
        ///  Select previous to do node in contents view
                /// </summary>
        /// <returns></returns>
        public bool SelectPreviousTo_DoNode ()
        {
            if (mView.SelectedNodeAs<EmptyNode>() != null)
            {
                for (ObiNode n = mView.SelectedNodeAs<EmptyNode>().PrecedingNode; n != null; n = n.PrecedingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).NodeKind  == EmptyNode.Kind.TODO)
                    {
                        mView.Selection = new NodeSelection(n, this);
                        return true;
                    }
                }
            } // check end for empty node
            return false;
        }

        /// <summary>
        ///  Select  next To_Do node in contents view
                /// </summary>
        /// <returns></returns>
        public bool SelectNextTo_DoNode ()
        {
            if (mView.SelectedNodeAs<EmptyNode>() != null)
            {
                for (ObiNode n = mView.SelectedNodeAs<EmptyNode>().FollowingNode; n != null; n = n.FollowingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.TODO )
                    {
                        mView.Selection = new NodeSelection(n, this);
                        return true;
                    }
                }
            }// check ends for empty node
            return false;
        }


        private bool NavigateNextPage()
        {
            mView.TransportBar.NextPage();
            return true;
        }

        private bool NavigateNextPhrase()
        {
            mView.TransportBar.NextPhrase();
            return true;
        }

        private bool NavigateNextSection()
        {
            mView.TransportBar.NextSection();
            return true;
        }

        private bool NavigatePrevPage()
        {
            mView.TransportBar.PrevPage();
            return true;
        }

        private bool NavigatePrevPhrase()
        {
            mView.TransportBar.PrevPhrase();
            return true;
        }

        private bool NavigatePrevSection()
        {
            mView.TransportBar.PrevSection();
            return true;
        }

        // Toggle play/pause in the transport bar
        public bool TogglePlayPause()
        {
            if (mView.TransportBar.CanPause)
            {
                mView.TransportBar.Pause();
                return true;
            }
            else if (mView.TransportBar.CanPlay || mView.TransportBar.CanResumePlayback)
            {
                mView.TransportBar.PlayOrResume();
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

        private bool MarkSelectionBeginTime()
        {
            return mView.TransportBar.MarkSelectionBeginTime();
        }

        private bool MarkSelectionEndTime()
        {
            return mView.TransportBar.MarkSelectionEndTime();
        }

        private bool MarkSelectionFromCursor()
        {
            return mView.TransportBar.MarkSelectionFromCursor();
        }

        private bool MarkSelectionToCursor()
        {
            return mView.TransportBar.MarkSelectionToCursor();
        }

        private bool MarkSelectionWholePhrase()
        {
            return mView.TransportBar.MarkSelectionWholePhrase();
        }

        private bool PlayPreviewFromCurrentPosition ()
        {
            return  mView.TransportBar.PlayPreviewFromCurrentPosition () ;
                                }

        private bool PlayPreviewSelectedFragment()
        {
            return mView.TransportBar.PlayPreviewSelectedFragment();
        }
        private bool PlayPreviewUptoCurrentPosition()
        {
            return mView.TransportBar.PlayPreviewUptoCurrentPosition () ;
                    }


               #endregion

        public void SelectAtCurrentTime() 
        { 
            if ( mPlaybackBlock != null )
            mPlaybackBlock.SelectAtCurrentTime(); 
        }
        
        public void GetFocus()
        {
            if (mSelection == null)
            {
                mView.Selection = new NodeSelection(mView.Presentation.FirstSection, this);
            }
            else
            {
                Focus();
            }
        }

        internal bool IsEnteringStripsView = false;
        private void StripsView_Enter(object sender, EventArgs e)
        {
            IsEnteringStripsView = true;
        }

/// <summary>
///  Function for processing tab key to preventing keyboard focus to move out of contents view with tabbing
/// </summary>
/// <param name="key"></param>
/// <returns></returns>
        private bool ProcessTabKeyInContentsView(Keys key)
        {
                        if (key == Keys.Tab)
            {
                if (this.ActiveControl != null)
                {
                    Strip s = ((Strip)mLayoutPanel.Controls[mLayoutPanel.Controls.Count - 1]);
                    if (s != null &&
                        ((s.ContainsFocus && s.LastBlock == null)
                                            || (s.LastBlock != null && s.LastBlock.ContainsFocus)))
                    {
                        SelectFirstStrip();
                        System.Media.SystemSounds.Beep.Play();
                        return true;
                    }
                }
            }
            else if (key == (Keys)(Keys.Shift | Keys.Tab))
            {
                if (this.ActiveControl != null)
                {
                    Strip s = ((Strip)mLayoutPanel.Controls[0]);
                    if (s != null && s.Controls[0].ContainsFocus)
                    {
                        Strip LastStrip = mLayoutPanel.Controls.Count > 0 ? (Strip)mLayoutPanel.Controls[mLayoutPanel.Controls.Count - 1] :
null;

                        if (LastStrip != null)
                        {
                            System.Media.SystemSounds.Beep.Play();
                            if (LastStrip.LastBlock != null)
                                return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return LastStrip.LastBlock; });
                            else
                                return SelectLastStrip();
                        }
                    }
                }
            }
            return false;
        }




        public void SelectNextPhrase(ObiNode node)
        {
            if (mSelection != null)
            {
                SelectFollowingBlock();
            }
            else if (node is SectionNode)
            {
                mSelectedItem = FindStrip((SectionNode)node);
                SelectFirstBlockInStrip();
            }
            else
            {
                SelectFirstStrip();
                SelectFirstBlockInStrip();
            }
        }
    }
}
