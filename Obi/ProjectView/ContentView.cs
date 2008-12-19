using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using urakawa.command;

namespace Obi.ProjectView
{
    /// <summary>
    /// The content view shows the strips and blocks of the project.
    /// </summary>
    public partial class ContentView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mProjectView;                            // parent project view
        private NodeSelection mSelection;                            // current selection
        private ISelectableInContentView mSelectedItem;              // the actual item for the selection
        private Dictionary<Keys, ProjectView.HandledShortcutKey> mShortcutKeys;  // list of all shortcuts
        private bool mWrapStripContents;                             // wrapping of strip contents
        private bool mIsEnteringView;                                // flag set when entering the  view

        private Dictionary<SectionNode, Strip> mStrips;              // strips for sections (reuse old strips instead of redrawing)
        private PriorityQueue<Waveform, int> mWaveformRenderQ;       // queue of waveforms to render
        private BackgroundWorker mWaveformRenderWorker;              // current waveform rendering worker

        // cursor stuff
        private AudioBlock mPlaybackBlock;
        private bool mFocusing;

        private bool mEnableScrolling;  // enable scrolling to control to show it
        private Cursor mCursor;

        
        private readonly int m_MaxVisiblePhraseCount;
        private bool m_CreatingGUIForNewPresentation;

        private delegate Strip AddStripForObiNodeDelegate(ObiNode node);
        private delegate void RemoveControlForSectionNodeDelegate(SectionNode node);

        /// <summary>
        /// A new strips view.
        /// </summary>
        public ContentView()
        {
            InitializeComponent();
            InitializeShortcutKeys();
            mProjectView = null;
            mSelection = null;
            mFocusing = false;
            mIsEnteringView = false;
            mWrapStripContents = false;
            mStrips = new Dictionary<SectionNode, Strip>();
            mWaveformRenderQ = new PriorityQueue<Waveform, int>();
            mWaveformRenderWorker = null;
            SetPlaybackPhraseAndTime(null, 0.0);
            mCornerPanel.BackColor = System.Drawing.SystemColors.Control;
            mEnableScrolling = true;

            m_VisibleStripsList = new List<Strip> ();
            m_MaxVisiblePhraseCount = 300;
        }


        // Size of the borders
        private int BorderHeight { get { return Bounds.Height - ClientSize.Height; } }
        private int BorderWidth { get { return Bounds.Width - ClientSize.Width; } }
        private int VisibleHeight { get { return Height - BorderHeight; } }
        private int VisibleWidth { get { return Width - BorderWidth; } }

        // Add a new control (normally, a strip) at the given index.
        private void AddControlAt(Control c, int index)
        {
            mStripsPanel.Controls.Add(c);
            mStripsPanel.Controls.SetChildIndex(c, index);
            ReflowFromControl(c);
            c.SizeChanged += new EventHandler(delegate(object sender, EventArgs e) { ReflowFromControl(c); });
            UpdateSize();
        }

        private void ReflowFromIndex(int index)
        {
            for (int i = index; i >= 0 && i < mStripsPanel.Controls.Count; ++i)
            {
                int y_prev = i == 0 ? 0 : mStripsPanel.Controls[i - 1].Location.Y + mStripsPanel.Controls[i - 1].Height + mStripsPanel.Controls[i - 1].Margin.Bottom;
                mStripsPanel.Controls[i].Location = new Point(mStripsPanel.Controls[i].Margin.Left, y_prev + mStripsPanel.Controls[i].Margin.Top);
            }
        }

        private void ReflowFromControl(Control c) { ReflowFromIndex(mStripsPanel.Controls.IndexOf(c)); }

        // Update size of the strips panel and the scrollbars.
        private void UpdateSize()
        {
            int h = VisibleHeight;
            if (mStripsPanel.Controls.Count > 0)
            {
                Control last = mStripsPanel.Controls[mStripsPanel.Controls.Count - 1];
                int h_ = last.Location.Y + last.Height + last.Margin.Bottom;
                if (h_ > h) h = h_;
            }
            int w_max = VisibleWidth;
            foreach (Control c in mStripsPanel.Controls)
            {
                int w = c.Width + c.Margin.Horizontal;
                if (w > w_max) w_max = w;
            }
            mStripsPanel.Width = w_max;
            mStripsPanel.Height = h;
            mVScrollBar.Maximum = h - VisibleHeight + mVScrollBar.LargeChange - 1 + mVScrollBar.Width;
            int v_max = mVScrollBar.Maximum - mVScrollBar.LargeChange + 1;
            if (mVScrollBar.Value > v_max) mVScrollBar.Value = v_max;
            mHScrollBar.Maximum = w_max - VisibleWidth + mHScrollBar.LargeChange - 1 + mHScrollBar.Height;
            int h_max = mHScrollBar.Maximum - mHScrollBar.LargeChange + 1;
            if (mHScrollBar.Value > h_max) mHScrollBar.Value = h_max;
        }


        public bool CanAddStrip { get { return IsStripSelected || IsBlockOrWaveformSelected || Selection is StripIndexSelection; } }
        public bool CanCopyAudio { get { return IsAudioRangeSelected; } }
        public bool CanCopyBlock { get { return IsBlockSelected; } }
        public bool CanCopyStrip { get { return IsStripSelected; } }
        public bool CanRemoveAudio { get { return IsAudioRangeSelected; } }
        public bool CanRemoveBlock { get { return IsBlockSelected; } }
        public bool CanRemoveStrip { get { return IsStripSelected; } }
        public bool CanRenameStrip { get { return IsStripSelected; } }

        /// <summary>
        /// Can split strip when a phrase is selected (but not the first), or at a strip index
        /// (but neither first nor last.)
        /// </summary>
        public bool CanSplitStrip
        {
            get
            {
                return (IsBlockSelected && SelectedEmptyNode.Index >= 0)
                    || mSelection is AudioSelection
                    || (IsStripCursorSelected && ((StripIndexSelection)mSelection).Index > 0 &&
                        ((StripIndexSelection)mSelection).Index <
                            ((StripIndexSelection)mSelection).Section.PhraseChildCount); 
            }
        }

        public bool CanSetBlockUsedStatus { get { return IsBlockOrWaveformSelected && mSelection.Node.ParentAs<ObiNode>().Used; } }

        public bool CanMergeBlockWithNext
        {
            get
            {
                EmptyNode node = mProjectView.TransportBar.IsPlayerActive ? mPlaybackBlock.Node: mSelectedItem is Block ? ((Block)mSelectedItem).Node : null;
                return node != null
                    && node.Index < node.ParentAs<ObiNode>().PhraseChildCount - 1;
            }
        }

        public bool CanMergeStripWithNext
        {
            get
            {
                return IsStripSelected &&
                     (mSelection.Node.Index < mSelection.Node.ParentAs<ObiNode>().SectionChildCount - 1 || 
                        ((SectionNode)mSelection.Node).SectionChildCount > 0);
            }
        }


        /// <summary>
        /// Current color settings used by the application.
        /// </summary>
        public ColorSettings ColorSettings
        {
            get { return mProjectView == null ? null : mProjectView.ColorSettings; }
            set { if (value != null) UpdateColors(value); }
        }

        /// <summary>
        /// Create a command to delete the selected strip.
        /// </summary>
        public ICommand DeleteStripCommand() { return DeleteStripCommand(SelectedSection); }

        public bool Focusing { get { return mFocusing; } }

        /// <summary>
        /// True if a block is selected and it is used.
        /// </summary>
        public bool IsBlockUsed { get { return IsBlockOrWaveformSelected && mSelection.Node.Used; } }

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
        /// Get the entering flag; then turn down the flag immediatly.
        /// </summary>
        public bool IsEnteringView
        {
            get
            {
                bool isEntering = mIsEnteringView;
                mIsEnteringView = false;
                return isEntering;
            }
        }

        /// <summary>
        /// Add a custom class to the context menu.
        /// </summary>
        public void AddCustomRoleToContextMenu(string name, ObiForm from)
        {
            from.AddCustomRoleToMenu(name, Context_AssignRoleMenuItem.DropDownItems, Context_AssignRole_NewCustomRoleMenuItem);
        }

        public void RemoveCustomRoleFromContextMenu ( string name, ObiForm from )
            {
            for (int i = 3; i < Context_AssignRoleMenuItem.DropDownItems.Count - 1; i++)
                {
                if (Context_AssignRoleMenuItem.DropDownItems[i].Text == name)
                    {
                                        Context_AssignRoleMenuItem.DropDownItems.RemoveAt ( i );
                                        }
                }
              
            }
        /// <summary>
        /// Show the strip for this section node.
        /// </summary>
        public void MakeStripVisibleForSection ( SectionNode section )
        {
            if (section != null) EnsureControlVisible(FindStrip(section));
        }

        /// <summary>
        /// Get a command to merge the selected strip with the next one. If the next strip is a child or a sibling, then
        /// its contents are appended to the selected strip and it is removed from the project; but if the next strip has
        /// a lower level, merging is not possible.
        /// </summary>
        public ICommand MergeSelectedStripWithNextCommand()
        {
            CompositeCommand command = null;
            if (CanMergeStripWithNext)
            {
                command = mProjectView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("merge_sections"));
                SectionNode section = SelectedSection;
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild(0);
                if (!section.Used) mProjectView.AppendMakeUnused(command, next);
                // Delete nodes in reverse order so that they are added back in the right order on redo
                // and remove the heading role if there is any in the next section
                for (int i = next.PhraseChildCount - 1; i >= 0; --i)
                {
                    // Remove the role before removing the node because it needs to be attached to
                    // inform its parent that it is not a heading anymore.
                    if (next.PhraseChild(i).Role_ == EmptyNode.Role.Heading)
                    {
                        Commands.Node.AssignRole role =
                            new Commands.Node.AssignRole(mProjectView, next.PhraseChild(i), EmptyNode.Role.Plain);
                        role.UpdateSelection = false;
                        command.append(role);
                    }
                    Commands.Node.Delete delete = new Commands.Node.Delete(mProjectView, next.PhraseChild(i));
                    delete.UpdateSelection = false;
                    command.append(delete);
                }
                for (int i = 0; i < next.PhraseChildCount; ++i)
                {
                    command.append(new
                        Commands.Node.AddNode(mProjectView, next.PhraseChild(i), section, section.PhraseChildCount + i));
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
        m_CreatingGUIForNewPresentation = true;
            mStripsPanel.Controls.Clear();
            m_VisibleStripsList.Clear ();
            ClearWaveformRenderQueue();
            SuspendLayout_All();
            AddStripForSection_Safe(mProjectView.Presentation.RootNode);
            ResumeLayout_All();
            mProjectView.Presentation.BeforeCommandExecuted +=
                new EventHandler<urakawa.events.command.CommandEventArgs>(Presentation_BeforeCommandExecuted);
            mProjectView.Presentation.getUndoRedoManager().commandDone +=
                new EventHandler<urakawa.events.undo.DoneEventArgs>(ContentView_commandDone);
            EventsAreEnabled = true;
            UpdateSize();
            mVScrollBar.Value = 0;
            mHScrollBar.Value = 0;

            CreateBlocksForInitialStrips ();
            m_CreatingGUIForNewPresentation = false;
        }

        private void ContentView_commandDone(object sender, urakawa.events.undo.DoneEventArgs e)
        {
            ResumeLayout_All();
            UpdateSize();
            Cursor = mCursor;

            //UpdateBlocksLabelInSelectedNodeStrip ();
        }

        private void Presentation_BeforeCommandExecuted(object sender, urakawa.events.command.CommandEventArgs e)
        {
            mCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            SuspendLayout_All();
        }

        /// <summary>
        /// Ignore/unignore events.
        /// </summary>
        public bool EventsAreEnabled
        {
            set
            {
                if (value)
                {
                    mProjectView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_changed);
                    mProjectView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
                    mProjectView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
                }
                else
                {
                    mProjectView.Presentation.changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_changed);
                    mProjectView.Presentation.RenamedSectionNode -= new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
                    mProjectView.Presentation.UsedStatusChanged -= new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
                }
            }
        }


        public AudioBlock PlaybackBlock { get { return mPlaybackBlock; } }

        public void SetPlaybackPhraseAndTime(PhraseNode node, double time)
        {
            if (mPlaybackBlock != null) mPlaybackBlock.ClearCursor();
            mPlaybackBlock = node == null ? null : (AudioBlock)FindBlock(node);
            if (mPlaybackBlock != null)
            {
                EnsureControlVisible(mPlaybackBlock);
                mPlaybackBlock.InitCursor(time);
            }
        }

        public PhraseNode PlaybackPhrase
        {
            get { return mPlaybackBlock == null ? null : mPlaybackBlock.Node as PhraseNode; }
        }
        
        public Strip PlaybackStrip { get { return mPlaybackBlock == null ? null : mPlaybackBlock.Strip; } }

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mProjectView != null) throw new Exception("Cannot set the project view again!");
                mProjectView = value;
            }
        }

        /// <summary>
        /// Rename a strip.
        /// </summary>
        public void RenameStrip(Strip strip) { mProjectView.RenameSectionNode(strip.Node, strip.Label); }

        /// <summary>
        /// Get the strip that the selection is in, or null if there is no applicable selection.
        /// </summary>
        public Strip StripForSelection
        {
            get
            {
                return mSelectedItem is Strip ? (Strip)mSelectedItem :
                    mSelectedItem is Block ? ((Block)mSelectedItem).Strip :
                    null;
            }
        }


        /// <summary>
        /// Add a waveform to the queue of waveforms to render.
        /// </summary>
        public void RenderWaveform(Waveform w, int priority)
        {
            if (mWaveformRenderQ.Enqueued(w, priority)) mProjectView.ObiForm.BackgroundOperation_AddItem();
            RenderFirstWaveform();
        }


        // Render the first waveform from the queue if no other rendering is in progress.
        private void RenderFirstWaveform()
        {
            while (mWaveformRenderWorker == null && mWaveformRenderQ.Count > 0)
            {
                Waveform w = mWaveformRenderQ.Dequeue();
                mWaveformRenderWorker = w.Render();
                if (mWaveformRenderWorker != null)
                {
                    mProjectView.ObiForm.BackgroundOperation_Step();
                }
            }
            if (mWaveformRenderQ.Count == 0) mProjectView.ObiForm.BackgroundOperation_Done();
        }

        private void ClearWaveformRenderQueue()
        {
            mWaveformRenderQ.Clear();
            if (mProjectView != null && mProjectView.ObiForm != null) mProjectView.ObiForm.BackgroundOperation_Done();
        }

        public void FinishedRendering(Waveform w, bool renderedOK)
        {
            mWaveformRenderWorker = null;
            RenderFirstWaveform();
        }

        /// <summary>
        /// Get all the searchable items (i.e. strips, blocks) in the control.
        /// This does not support nested blocks right now.
        /// </summary>
        public List<ISearchable> Searchables
        {
            get
            {
                List<ISearchable> l = new List<ISearchable>();
                AddToSearchables(this, l);
                return l;
            }
        }

        public EmptyNode SelectedEmptyNode { get { return IsBlockSelected ? ((Block)mSelectedItem).Node : null; } }
        public ObiNode SelectedNode { set { if (mProjectView != null) mProjectView.Selection = new NodeSelection(value, this); } }
        public PhraseNode SelectedPhraseNode { get { return IsBlockSelected ? ((Block)mSelectedItem).Node as PhraseNode : null; } }
        public SectionNode SelectedSection { get { return IsStripSelected ? ((Strip)mSelectedItem).Node : null; } }
        public NodeSelection SelectionFromStrip { set { if (mProjectView != null) mProjectView.Selection = value; } }

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
                    ISelectableInContentView s = value == null ? null : FindSelectable(value);
                    if (mSelectedItem != null) mSelectedItem.Highlighted = false;
                    mSelection = value;
                    mSelectedItem = s;
                    if (s != null)
                    {
                        s.SetSelectionFromContentView(mSelection);
                        SectionNode section = value.Node is SectionNode ? (SectionNode)value.Node :
                            value.Node.ParentAs<SectionNode>();
                        mProjectView.MakeTreeNodeVisibleForSection(section);
                        EnsureControlVisible((Control)s);
                        mFocusing = true;
                        if (!((Control)s).Focused) ((Control)s).Focus();
                        mFocusing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Disable scrolling when clicking on an element. 
        /// </summary>
        public void DisableScrolling() { mEnableScrolling = false; }

        // Ensure that the playback cursor is visible given its position in the waveform.
        private void EnsureCursorVisible(int x)
        {
            /*int x_cursor = x;
            for (Control parent = mPlaybackBlock.Parent; parent != mStripsPanel; parent = parent.Parent)
            {
                x_cursor += parent.Location.X;
            }
            int x_ = x_cursor + mStripsPanel.Location.X;
            int h_max = mHScrollBar.Maximum - mHScrollBar.LargeChange + 1;
            int vw = VisibleWidth - mVScrollBar.Width - mVScrollBar.LargeChange;
            if (x_ > vw)
            {
                // EnsureControlVisible(mPlaybackBlock);
                mHScrollBar.Value = Math.Min(x_ - vw, h_max);
            }*/
        }

        // Scroll to the control to make sure that it is shown.
        private void EnsureControlVisible(Control c)
        {
            if (mEnableScrolling)
            {
                // Find the parent strip
                if (!(c is Strip))
                {
                    Control strip = c.Parent;
                    while (!(strip is Strip)) strip = strip.Parent;
                    EnsureControlVisible(strip);
                }
                // Compute the location of the control relative to the strips panel
                // (Location is relative to its direct parent.)
                Point location = c.Location;
                Control parent = c.Parent;
                while (parent != mStripsPanel)
                {
                    location.X += parent.Location.X;
                    location.Y += parent.Location.Y;
                    parent = parent.Parent;
                }
                // Compute the four corners of the control, including margins
                int top = location.Y - c.Margin.Top;
                int bottom = location.Y + c.Height + c.Margin.Bottom;
                int left = location.X - c.Margin.Left;
                int right = location.X + c.Width + c.Margin.Right;
                // Four corners relative to the current strips panel location
                int t = top + mStripsPanel.Location.Y;
                int b = bottom + mStripsPanel.Location.Y;
                int h = bottom - top;
                int l = left + mStripsPanel.Location.X;
                int r = right + mStripsPanel.Location.X;
                int w = c.Width + c.Margin.Horizontal;
                // Maximum values of the scrollbars (for some reason the scrollbar
                // stops one "large change" short of the maximum...)
                int v_max = mVScrollBar.Maximum - mVScrollBar.LargeChange + 1;
                int vh = VisibleHeight - mHScrollBar.Height;
                int h_max = mHScrollBar.Maximum - mHScrollBar.LargeChange + 1;
                int vw = VisibleWidth - mVScrollBar.Width;

                // Vertical scrolling
                if (t < 0 || (b > vh && h > vh))
                {
                    // Top of control is above the visible window, so scroll to the top
                    mVScrollBar.Value = Math.Min(top, v_max);
                }
                else if (b > vh)
                {
                    // Top of control is below the visible window; scroll to align the
                    // bottom of the control to the bottom of the visible window; unless
                    // the control is taller than the visible window, in which case we
                    // want to see the top of the control in priority (this is handled
                    // above.)
                    mVScrollBar.Value = Math.Min(bottom - vh, v_max);
                }

                // Horizontal scrolling is the same
                if (l < 0 || (r > vw && w > vw))
                {
                    mHScrollBar.Value = Math.Min(left, h_max);
                }
                else if (r > vw)
                {
                    mHScrollBar.Value = Math.Min(right - vw, h_max);
                }
            }
            else
            {
                mEnableScrolling = true;
            }
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
                    if (mSelectedItem == s && !visible) mProjectView.Selection = null;
                    SetStripsVisibilityForSection(section.SectionChild(i), visible);
                }
            }
        }

        public void SetStripVisibilityForSection(SectionNode node, bool visible)
        {
            Strip s = FindStrip(node);
            if (s != null) s.Visible = visible;
        }

        /// <summary>
        /// Split a strip at the selected block or cursor position; i.e. create a new sibling section which
        /// inherits the children of the split section except for the phrases before the selected block or
        /// position. Do not do anything if there are no phrases before.
        /// In case of an audio selection, split the phrase normally and use this position as the split
        /// point (i.e. audio before becomes the last block of the first strip, audio after is the beginning
        /// of the new strip.)
        /// </summary>
        public CompositeCommand SplitStripCommand()
        {
            CompositeCommand command = null;
            if (CanSplitStrip)
            {
                EmptyNode node = Selection.EmptyNodeForSelection;
                SectionNode section = node.ParentAs<SectionNode>();
                command = mProjectView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("split_section"));
                // Add a sibling with a new label
                SectionNode sibling = mProjectView.Presentation.CreateSectionNode();
                sibling.Label = section.Label + "*";
                Commands.Node.AddNode add = new Commands.Node.AddNode(mProjectView, sibling, section.ParentAs<ObiNode>(),
                    section.Index + 1);
                add.UpdateSelection = false;
                command.append(add);
                // Change parents of children to insert the section at the right position in strip order
                for (int i = section.SectionChildCount - 1; i >= 0; --i)
                {
                    command.append(new Commands.Node.Delete(mProjectView, section.SectionChild(i), false));
                }
                for (int i = 0; i < section.SectionChildCount; ++i)
                {
                    command.append(new Commands.Node.AddNode(mProjectView, section.SectionChild(i), sibling, i, false));
                }
                // Split the node if necessary
                PhraseNode splitNode = null;
                PhraseNode cropNode = null;
                if (mProjectView.CanSplitPhrase)
                {
                    ICommand splitCommand = Commands.Node.SplitAudio.GetSplitCommand(mProjectView);
                    if (splitCommand != null ) command.append(splitCommand);
                    splitNode = Commands.Node.SplitAudio.GetSplitNode(splitCommand);
                    if (splitNode != null) cropNode = Commands.Node.SplitAudio.GetCropNode(splitCommand, splitNode);
                }
                // Move children from the context phrase to the new sibling
                int sectionOffset = node.Index + (splitNode != null ? 1 : 0);
                for (int i = section.PhraseChildCount - 1; i >= sectionOffset; --i)
                {
                    command.append(new Commands.Node.Delete(mProjectView, section.PhraseChild(i), false));
                }
                if (cropNode != null) command.append(new Commands.Node.Delete(mProjectView, cropNode, section, node.Index + 2, false));
                if (splitNode != null)
                {
                    command.append(new Commands.Node.Delete(mProjectView, splitNode, section, node.Index + 1, false));
                    command.append(new Commands.Node.AddNode(mProjectView, splitNode, sibling, 0, false));
                }
                if (cropNode != null) command.append(new Commands.Node.AddNode(mProjectView, cropNode, sibling, 1, false));
                int siblingOffset = node.Index - (cropNode != null ? 1 : 0);
                for (int i = sectionOffset; i < section.PhraseChildCount; ++i)
                {
                    command.append(new
                        Commands.Node.AddNode(mProjectView, section.PhraseChild(i), sibling, i - siblingOffset, false));
                }
                command.append(new Commands.UpdateSelection(mProjectView, new NodeSelection(sibling, this)));
            }
            return command;
        }

        /// <summary>
        /// String to be shown in the status bar.
        /// </summary>
        public override string ToString() { return Localizer.Message("strips_view_to_string"); }

        /// <summary>
        /// Views are not synchronized anymore, so make sure that all strips are visible.
        /// </summary>
        public void UnsyncViews() { foreach (Control c in mStripsPanel.Controls) c.Visible = true; }

        public void UpdateCursorPosition(double time)
        {
            if (mPlaybackBlock != null ) EnsureCursorVisible(mPlaybackBlock.UpdateCursorTime(time));
        }

        private void UpdateBlocksLabelInSelectedNodeStrip ()
            {
            if (mProjectView.Presentation != null && mProjectView.Selection != null)
                {
                if (mProjectView.Selection.Node is SectionNode) UpdateBlocksLabelInStrip ( (SectionNode)mProjectView.Selection.Node );
                else if (mProjectView.Selection.Node is EmptyNode) UpdateBlocksLabelInStrip ( mProjectView.Selection.Node.ParentAs<SectionNode> () );
                                }
            }

        private void UpdateBlocksLabelInStrip(SectionNode section)
        {
                    Strip s = FindStrip(section);
            if (s != null)
            {
                try
                {
                    //BackgroundWorker UpdateStripThread = new BackgroundWorker();
                    //UpdateStripThread.DoWork += new DoWorkEventHandler(s.UpdateBlockLabelsInStrip);
                    //UpdateStripThread.RunWorkerAsync();
                //s.UpdateBlockLabelsInStrip ();
                                }
                catch (System.Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Set the flag to wrap contents inside a strip.
        /// </summary>
        public bool WrapStripContents
        {
            set
            {
                mWrapStripContents = value;
                for (int i = mStripsPanel.Controls.Count - 1; i >= 0; --i)
                {
                    Strip strip = mStripsPanel.Controls[i] as Strip;
                    if (strip != null) strip.WrapContents = mWrapStripContents;
                }
                UpdateSize();
            }
        }

        public float AudioScale
        {
            get { return mProjectView == null ? 0.01f : mProjectView.AudioScale; }
            set
            {
                foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).AudioScale = value;
                UpdateSize();
            }
        }

        /// <summary>
        /// Set the zoom factor for the control and its components.
        /// </summary>
        public float ZoomFactor
        {
            get { return mProjectView == null ? 1.0f : mProjectView.ZoomFactor; }
            set
            {
                ClearWaveformRenderQueue();
                foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).ZoomFactor = value;
                UpdateSize();
            }
        }


        // Add a new strip for a section and all of its subsections
        private Strip AddStripForSection_Safe(ObiNode node)
        {
            if (InvokeRequired)
            {
                return Invoke(new AddStripForObiNodeDelegate(AddStripForSection_Safe), node) as Strip;
            }
            else
            {
                SuspendLayout();
                Strip strip = AddStripForSection(node);
                ResumeLayout();
                return strip;
            }
        }

        // Add a single strip for a section node
        private Strip AddStripForSection(ObiNode node)
        {
            Strip strip = null;
            if (node is SectionNode)
            {
                if (mStrips.ContainsKey((SectionNode)node))
                {
                    strip = mStrips[(SectionNode)node];
                }
                else
                {
                    strip = new Strip((SectionNode)node, this);
                    mStrips[(SectionNode)node] = strip;
                    strip.WrapContents = mWrapStripContents;
                    strip.ColorSettings = ColorSettings;
                    //for (int i = 0; i < node.PhraseChildCount; ++i) strip.AddBlockForNode(node.PhraseChild(i));
                    if (!m_CreatingGUIForNewPresentation)
                        {
                                                    CreateBlocksInStrip ( strip );
                        }

                    }
                AddControlAt(strip, ((SectionNode)node).Position);
            }
            for (int i = 0; i < node.SectionChildCount; ++i) AddStripForSection(node.SectionChild(i));
            return strip;
        }

        private List<Strip> m_VisibleStripsList;

        public int VisibleBlocksCount
            {
            get
                {
                int count= 0 ;
                for (int i = 0; i < m_VisibleStripsList.Count; i++)
                    count += m_VisibleStripsList[i].Node.PhraseChildCount;

                return count;
                }
            }

        private void CreateBlocksForInitialStrips ()
            {

            for (int i = 0; i < mStripsPanel.Controls.Count; i++)
                {

                if (mStripsPanel.Controls[i] is Strip)
                    {
                    Strip s = (Strip)mStripsPanel.Controls[i] ;
                    if ( s.Node.PhraseChildCount < m_MaxVisiblePhraseCount -  VisibleBlocksCount  )
                        {
                    CreateBlocksInStrip (s) ;
                                                //MessageBox.Show ( s.Node.Label );
                        }
                    }

                }

            }



        public bool CreateBlocksInStrip ()
            {
            return CreateBlocksInStrip ( StripForSelection != null ? StripForSelection : null );
            }

        private  bool CreateBlocksInStrip ( Strip stripControl )
            {
                                    if (stripControl != null && stripControl.Node.PhraseChildCount > 0 )
                {
                MakeOldStripsBlocksInvisible ( stripControl.Node.PhraseChildCount );

                for (int i = 0; i < stripControl.Node.PhraseChildCount; ++i) 
                    stripControl.AddBlockForNode ( stripControl.Node.PhraseChild ( i ) );

                m_VisibleStripsList.Add ( stripControl );
                                return true;
                }
            return false;
            }

        private void MakeOldStripsBlocksInvisible ( int countRequired )
            {
            if (m_VisibleStripsList.Count > 0)
            {
            for (int i = 0; i < m_VisibleStripsList.Count; i++)
                {
                                if (m_MaxVisiblePhraseCount - VisibleBlocksCount < countRequired)
                    {
                    RemoveBlocksInStrip ( m_VisibleStripsList[0] );
                    //MessageBox.Show ( "Removed  " + m_VisibleStripsList[0].Label );
                    m_VisibleStripsList.RemoveAt ( 0 );
                    
                    }
                else
                    return;
                }
                }
                        }

        private bool RemoveBlocksInStrip ( Strip stripControl )
            {
            if (stripControl != null && stripControl.Node.PhraseChildCount > 0)
                {
                for ( int i = 0 ; i < stripControl.Node.PhraseChildCount ; i++ )
                    stripControl.RemoveBlock ( stripControl.Node.PhraseChild (i) ) ;

                return true;
                }
            return false;
            }

        // Recursive function to go through all the controls in-order and add the ISearchable ones to the list
        private void AddToSearchables ( Control c, List<ISearchable> searchables )
        {
            if (c is ISearchable) searchables.Add((ISearchable)c);
            foreach (Control c_ in c.Controls) AddToSearchables(c_, searchables);
        }

        // Deselect everything when clicking the panel
        private void ContentView_Click(object sender, EventArgs e) { mProjectView.Selection = null; }

        // Create a command (possibly composite) to delete a strip for the given section node.
        private ICommand DeleteStripCommand(SectionNode section)
        {
            Commands.Node.Delete delete = new Commands.Node.Delete(mProjectView, section, Localizer.Message("delete_section_shallow"));
            if (section.SectionChildCount > 0)
            {
                CompositeCommand command = mProjectView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(delete.getShortDescription());
                for (int i = 0; i < section.SectionChildCount; ++i)
                {
                    command.append(new Commands.TOC.MoveSectionOut(mProjectView, section.SectionChild(i)));
                }
                command.append(delete);
                return command;
            }
            else
            {
                return delete;
            }
        }

        // Find the block for the given node; return null if not found (be careful!)
        private Block FindBlock(EmptyNode node)
        {
            Strip strip = FindStrip(node.ParentAs<SectionNode>());
            return strip == null ? null : strip.FindBlock(node);
        }

        // Find the strip cursor for a strip index selection
        private StripCursor FindStripCursor(StripIndexSelection selection)
        {
            Strip strip = FindStrip(selection.Node as SectionNode);
            return strip == null ? null : strip.FindStripCursor(selection.Index);
        }

        // Find the strip for the given section node; return null if not found (be careful!)
        private Strip FindStrip(SectionNode section)
        {
            foreach (Control c in mStripsPanel.Controls)
            {
                if (c is Strip && ((Strip)c).Node == section) return (Strip)c;
            }
            return null;
        }

        // Find the selectable item for this selection object (block, strip or strip cursor.)
        private ISelectableInContentView FindSelectable(NodeSelection selection)
        {
            return
                selection == null ? null :
                selection is StripIndexSelection ? (ISelectableInContentView)FindStripCursor((StripIndexSelection)selection) :
                selection.Node is SectionNode ? (ISelectableInContentView)FindStrip((SectionNode)selection.Node) :
                selection.Node is EmptyNode ? (ISelectableInContentView)FindBlock((EmptyNode)selection.Node) : null;
        }

        private bool IsAudioRangeSelected { get { return mSelection is AudioSelection && !((AudioSelection)mSelection).AudioRange.HasCursor; } }
        private bool IsBlockSelected { get { return mSelectedItem is Block && mSelection.GetType() == typeof(NodeSelection); } }
        private bool IsBlockOrWaveformSelected { get { return mSelectedItem is Block; } }
        private bool IsInView(ObiNode node) { return node is SectionNode && FindStrip((SectionNode)node) != null; }
        private bool IsStripCursorSelected { get { return mSelection is StripIndexSelection; } }
        private bool IsStripSelected { get { return mSelectedItem is Strip && mSelection.GetType() == typeof(NodeSelection); } }

        // Listen to changes in the presentation (new nodes being added or removed)
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

        // Handle section nodes renamed from the project: change the label of the corresponding strip.
        private void Presentation_RenamedSectionNode(object sender, NodeEventArgs<SectionNode> e)
        {
            Strip strip = FindStrip(e.Node);
            if (strip != null) strip.Label = e.Node.Label;
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

        // Remove all strips for a section and its subsections
        private void RemoveStripsForSection_Safe(SectionNode section)
        {
            if (InvokeRequired)
            {
                Invoke(new RemoveControlForSectionNodeDelegate(RemoveStripsForSection_Safe), section);
            }
            else
            {
                SuspendLayout();
                RemoveStripsForSection(section);
                ResumeLayout();
            }
        }

        private void RemoveStripsForSection(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) RemoveStripsForSection(section.SectionChild(i));
            Strip strip = FindStrip(section);
            int index = mStripsPanel.Controls.IndexOf(strip);
            mStripsPanel.Controls.Remove(strip);
            ReflowFromIndex(index);
        }

        // Remove the strip or block for the removed tree node
        private void TreeNodeRemoved(urakawa.events.core.ChildRemovedEventArgs e)
        {
            if (e.RemovedChild is SectionNode)
            {
                RemoveStripsForSection_Safe((SectionNode)e.RemovedChild);
            }
            else if (e.RemovedChild is EmptyNode)
            {
                // TODO in the future, the parent of a removed empty node
                // will not always be a section node!
                Strip strip = FindStrip((SectionNode)e.SourceTreeNode);
                if (strip != null) strip.RemoveBlock((EmptyNode)e.RemovedChild);
            }
        }

        // Add a new strip for a newly added section node or a new block for a newly added empty node.
        private void TreeNodeAdded(urakawa.events.core.ChildAddedEventArgs e)
        {
            Control c = e.AddedChild is SectionNode ? (Control)AddStripForSection_Safe((SectionNode)e.AddedChild) :
                // TODO: in the future, the source node will not always be a section node!
                e.AddedChild is EmptyNode ? (Control)FindStrip((SectionNode)e.SourceTreeNode).AddBlockForNode((EmptyNode)e.AddedChild) :
                null;
            UpdateNewControl(c);
        }

        private delegate void ControlInvokation(Control c);

        private void UpdateNewControl(Control c)
        {
            if (InvokeRequired)
            {
                Invoke(new ControlInvokation(UpdateNewControl), c);
            }
            else if (c != null)
            {
                EnsureControlVisible(c);
                UpdateTabIndex(c);
            }
        }

        // Update the colors of the view and its components.
        private void UpdateColors(ColorSettings settings)
        {
            mStripsPanel.BackColor = settings.ContentViewBackColor;
            foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).ColorSettings = settings;
            UpdateWaveforms();
        }

        // Update all waveforms after colors have been set
        private void UpdateWaveforms()
        {
            ClearWaveformRenderQueue();
            foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).UpdateWaveforms(AudioBlock.NORMAL_PRIORITY);
        }

        #region IControlWithRenamableSelection Members

        public void SelectAndRename(ObiNode node)
        {
            SectionNode section = node as SectionNode;
            if (section != null)
            {
                DoToNewNode(node, delegate()
                {
                    mProjectView.Selection = new NodeSelection(section, this);
                    FindStrip(section).StartRenaming();
                });
            }
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
                        mProjectView.Presentation.changed -= h;
                    }
                };
                mProjectView.Presentation.changed += h;
            }
        }

        #endregion



        #region tabbing

        // Update tab index for all controls after a newly added strip
        private void UpdateTabIndex(Strip strip)
        {
            int stripIndex = mStripsPanel.Controls.IndexOf(strip);
            int tabIndex = stripIndex > 0 ? ((Strip)mStripsPanel.Controls[stripIndex - 1]).LastTabIndex : 0;
            for (int i = stripIndex; i < mStripsPanel.Controls.Count; ++i)
            {
                tabIndex = ((Strip)mStripsPanel.Controls[i]).UpdateTabIndex(tabIndex);
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

        private void InitializeShortcutKeys()
        {
            mShortcutKeys = new Dictionary<Keys, ProjectView.HandledShortcutKey>();

            mShortcutKeys[Keys.A] = delegate() { return mProjectView.TransportBar.MarkSelectionWholePhrase(); };
            mShortcutKeys[Keys.C] = delegate() { return mProjectView.TransportBar.PreviewAudioSelection(); };
            mShortcutKeys[Keys.H] = delegate() { return mProjectView.TransportBar.NextSection(); };
            mShortcutKeys[Keys.Shift | Keys.H] = delegate() { return mProjectView.TransportBar.PrevSection(); };
            mShortcutKeys[Keys.J] = delegate() { return mProjectView.TransportBar.PrevPhrase(); };
            mShortcutKeys[Keys.K] = delegate() { return mProjectView.TransportBar.NextPhrase(); };
            mShortcutKeys[Keys.N] = delegate() { return mProjectView.TransportBar.Nudge(TransportBar.Forward); };
            mShortcutKeys[Keys.Shift | Keys.N] = delegate() { return mProjectView.TransportBar.Nudge(TransportBar.Backward); };
            mShortcutKeys[Keys.OemOpenBrackets] = delegate() { return mProjectView.TransportBar.MarkSelectionBeginTime(); };
            mShortcutKeys[Keys.OemCloseBrackets] = delegate() { return mProjectView.TransportBar.MarkSelectionEndTime(); };
            mShortcutKeys[Keys.P] = delegate() { return mProjectView.TransportBar.NextPage(); };
            mShortcutKeys[Keys.Shift | Keys.P] = delegate() { return mProjectView.TransportBar.PrevPage(); };
            mShortcutKeys[Keys.V] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.From, TransportBar.UseAudioCursor); };
            mShortcutKeys[Keys.Shift | Keys.V] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.From, TransportBar.UseSelection); };
            mShortcutKeys[Keys.X] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.Upto, TransportBar.UseAudioCursor); };
            mShortcutKeys[Keys.Shift | Keys.X] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.Upto, TransportBar.UseSelection); };

            // playback shortcuts.

            mShortcutKeys[Keys.S] = FastPlayRateStepDown;
            mShortcutKeys[Keys.F] = FastPlayRateStepUp;
            mShortcutKeys[Keys.D] = FastPlayRateNormalise;
            mShortcutKeys[Keys.E] = FastPlayNormaliseWithLapseBack;
            mShortcutKeys[Keys.Shift | Keys.OemOpenBrackets] = MarkSelectionFromCursor;
            mShortcutKeys[Keys.Shift | Keys.OemCloseBrackets] = MarkSelectionToCursor;


            // Strips navigation
            mShortcutKeys[Keys.Left] = SelectPrecedingBlock;
            mShortcutKeys[Keys.Right] = SelectFollowingBlock;
            mShortcutKeys[Keys.End] = SelectLastBlockInStrip;
            mShortcutKeys[Keys.Home] = SelectFirstBlockInStrip;
            mShortcutKeys[Keys.Control | Keys.PageDown] = SelectNextPageNode;
            mShortcutKeys[Keys.Control | Keys.PageUp] = SelectPrecedingPageNode;
            mShortcutKeys[Keys.F4] = SelectNextSpecialRoleNode;
            mShortcutKeys[Keys.Shift | Keys.F4] = SelectPreviousSpecialRoleNode;
            mShortcutKeys[Keys.Control | Keys.Alt | Keys.F4] = SelectNextEmptyNode;

            mShortcutKeys[Keys.Up] = SelectPreviousStrip;
            mShortcutKeys[Keys.Down] = SelectNextStrip;
            mShortcutKeys[Keys.Control | Keys.Home] = SelectFirstStrip;
            mShortcutKeys[Keys.Control | Keys.End] = SelectLastStrip;

            mShortcutKeys[Keys.Escape] = SelectUp;

            // Control + arrows moves the strip cursor
            mShortcutKeys[Keys.Control | Keys.Left] = SelectPrecedingStripCursor;
            mShortcutKeys[Keys.Control | Keys.Right] = SelectFollowingStripCursor;

            mShortcutKeys[Keys.F5] = CreateBlocksInStrip ;
        }

        private bool CanUseKeys { get { return mSelection == null || !(mSelection is TextSelection); } }

        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            if (CanUseKeys &&
                ((msg.Msg == ProjectView.WM_KEYDOWN) || (msg.Msg == ProjectView.WM_SYSKEYDOWN)) &&
                mShortcutKeys.ContainsKey(key) && mShortcutKeys[key]()) return true;
            if (ProcessTabKeyInContentsView(key)) return true;
            return base.ProcessCmdKey(ref msg, key);
        }

        // Get the strip for the currently selected component (i.e. the strip itself, or the parent strip
        // for a block.)
        private Strip StripFor(ISelectableInContentView item)
        {
            return item is Strip ? (Strip)item :
                   item is StripCursor ? ((StripCursor)item).Strip :
                   item is Block ? ((Block)item).Strip : null;
        }

        private delegate Block SelectBlockFunction(Strip strip, ISelectableInContentView item);

        private bool SelectBlockFor(SelectBlockFunction f)
        {
            Strip strip = StripFor(mSelectedItem);
            if (strip != null)
            {
                Block block = f(strip, mSelectedItem);
                if (block != null)
                {
                    mProjectView.Selection = new NodeSelection(block.Node, this);
                    return true;
                }
            }
            return false;
        }

        private delegate int SelectStripCursorFunction(Strip strip, ISelectableInContentView item);

        private bool SelectStripCursorFor(SelectStripCursorFunction f)
        {
            Strip strip = StripFor(mSelectedItem);
            if (strip != null)
            {
                int index = f(strip, mSelectedItem);
                if (index >= 0)
                {
                    mProjectView.Selection = new StripIndexSelection(strip.Node, this, index);
                    return true;
                }
            }
            return false;
        }

        private bool SelectPrecedingBlock()
        {
                    return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.BlockBefore ( mProjectView.TransportBar.IsPlayerActive? mPlaybackBlock : item ); } );
        }

        private bool SelectPrecedingStripCursor()
        {
        bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
        mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
        Block PlaybackBlock = null;
        if (mProjectView.TransportBar.CanUsePlaybackSelection)
            {
            PlaybackBlock = mPlaybackBlock;
            mProjectView.TransportBar.Stop ();
            }

            bool ReturnVal = SelectStripCursorFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.StripIndexBefore ( PlaybackBlock != null ? PlaybackBlock : item ); } );
        mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;

        return ReturnVal;
        }

        private bool SelectFollowingBlock()
        {
                        return SelectBlockFor(delegate(Strip strip, ISelectableInContentView item) { return strip.BlockAfter( mProjectView.TransportBar.IsPlayerActive ? mPlaybackBlock : item); });
        }

        private bool SelectFollowingStripCursor()
        {
       bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
       mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
       Block PlaybackBlock = null;
        if (mProjectView.TransportBar.CanUsePlaybackSelection)
        {
            PlaybackBlock = mPlaybackBlock;
            mProjectView.TransportBar.Stop ();
        }

            bool ReturnVal = SelectStripCursorFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.StripIndexAfter ( PlaybackBlock != null ? PlaybackBlock : item ); } );
       mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
       return ReturnVal;
        }

        private bool SelectLastBlockInStrip()
        {
        if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
                     return SelectBlockFor(delegate(Strip strip, ISelectableInContentView item) { return strip.LastBlock; });
        }

        private bool SelectFirstBlockInStrip()
        {
        if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
            
            //if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Stop();
            return SelectBlockFor(delegate(Strip strip, ISelectableInContentView item) { return strip.FirstBlock; });
        }

        private delegate Strip SelectStripFunction(Strip strip);

        private bool SelectStripFor(SelectStripFunction f)
        {
            Strip strip = f(StripFor(mSelectedItem) as Strip);
            if (strip != null)
            {
                mProjectView.Selection = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }

        private bool SelectPreviousStrip()
        {
            bool WasPlaying = mProjectView.TransportBar.CurrentState == TransportBar.State.Playing ;
            if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
            
            Strip strip;
            if (WasPlaying
                && (this.mPlaybackBlock.ObiNode.Index == 0 || mPlaybackBlock.Node.Role_ == EmptyNode.Role.Heading ))
            {
            strip = StripBefore ( StripFor (  mSelectedItem ) );
            }
            else
            strip = mSelectedItem is Strip ? StripBefore ( StripFor ( mSelectedItem ) ) : StripFor ( mSelectedItem );

            if (strip != null)
            {
                mProjectView.Selection = new NodeSelection(strip.Node, this);
                strip.FocusStripLabel();
                return true;
            }
            return false;
        }

        private bool SelectNextStrip()
        {
                                    Strip strip = StripAfter(StripFor( mProjectView.TransportBar.IsPlayerActive ? mPlaybackBlock : mSelectedItem));
            if (strip != null)
            {
                mProjectView.Selection = new NodeSelection(strip.Node, this);
                strip.FocusStripLabel();
                return true;
            }
        else if (mSelectedItem != null &&  StripFor ( mSelectedItem ) != (Strip)mStripsPanel.Controls[mStripsPanel.Controls.Count - 1] || Selection is TextSelection)
            {
                // allow base to process the key if  current strip is not last strip or some text is selected
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool SelectFirstStrip()
        {
                        return SelectStripFor(delegate(Strip strip)
            {
                return mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[0] : null;
            });
        }

        private bool SelectLastStrip()
        {
                        return SelectStripFor(delegate(Strip strip)
            {
                return mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[mStripsPanel.Controls.Count - 1] : null;
            });
        }

        // Select the item above the currently selected item.
        // E.g. from an audio selection a phrase, from a phrase a strip, from a strip nothing.
        private bool SelectUp()
        {
            if (mSelection is AudioSelection)
            {
                return SelectBlockFor(delegate(Strip s, ISelectableInContentView item)
                    { return FindBlock((EmptyNode)mSelection.Node); });
            }
            else if (mSelectedItem is Block)
            {
                return SelectStripFor(delegate(Strip s) { return ((Block)mSelectedItem).Strip; });
            }
            else if (mSelectedItem is Strip)
            {
                mProjectView.Selection = null;
                return true;
            }
            return false;
        }

        private Strip StripAfter(Strip strip)
        {
            if (strip != null)
            {
                int count = mStripsPanel.Controls.Count;
                int index = 1 + mStripsPanel.Controls.IndexOf(strip);
                return index < count ? (Strip)mStripsPanel.Controls[index] : null;
            }
            return null;
        }

        public Strip StripBefore(Strip strip)
        {
            if (strip != null)
            {
                int index = mStripsPanel.Controls.IndexOf(strip);
                return index > 0 ? (Strip)mStripsPanel.Controls[index - 1] : null;
            }
            return null;
        }


        /// <summary>
        /// returns active current node from transport bar if player is active else return selected node from project view
                /// </summary>
        private ObiNode SelectedNodeInTransportbarOrProjectview
            {
            get
                {
                if (mProjectView.TransportBar.IsPlayerActive)
                    return mPlaybackBlock.Node;
                else
                    return mProjectView.SelectedNodeAs<ObiNode> ();
                }
            }


        /// <summary>
        /// Moves keyboard focus to preceding page node.
        /// </summary>
        public bool SelectPrecedingPageNode()
        {
            if (SelectedNodeInTransportbarOrProjectview != null)
            {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.PrecedingNode; n != null; n = n.PrecedingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                    {
                        mProjectView.Selection = new NodeSelection(n, this);
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
            if (SelectedNodeInTransportbarOrProjectview != null)
            {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.FollowingNode; n != null; n = n.FollowingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                    {
                        mProjectView.Selection = new NodeSelection(n, this);
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
            if (SelectedNodeInTransportbarOrProjectview != null)
            {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.PrecedingNode; n != null; n = n.PrecedingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ != EmptyNode.Role.Plain)
                    {
                        mProjectView.Selection = new NodeSelection(n, this);
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
            if (SelectedNodeInTransportbarOrProjectview != null)
            {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.FollowingNode; n != null; n = n.FollowingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ != EmptyNode.Role.Plain)
                    {
                        mProjectView.Selection = new NodeSelection(n, this);
                        return true;
                    }
                }
            }// check ends for empty node
            return false;
        }


        /// <summary>
        /// Select previous to do node in contents view
        /// </summary>
        public void SelectNextTODONode()
        {
        if (mProjectView.Presentation != null)
            {
            if (SelectedNodeInTransportbarOrProjectview != null)
                {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.FollowingNode; n != null; n = n.FollowingNode)
                    {
                    if (n is EmptyNode && ((EmptyNode)n).TODO)
                        {
                        mProjectView.Selection = new NodeSelection ( n, this );
                        return;
                        }
                    }
                }
            for (ObiNode n = mProjectView.Presentation.RootNode.FirstLeaf; n != null; n = n.FollowingNode)
                {
                if (n is EmptyNode && ((EmptyNode)n).TODO)
                    {
                    mProjectView.Selection = new NodeSelection ( n, this );
                    return;
                    }
                }
            } // check for null presentation ends
                    }

        /// <summary>
        /// Select previous to do node in contents view
        /// </summary>
        public void SelectPrecedingTODONode()
        {
            if (mProjectView.Presentation != null)
            {
                if (SelectedNodeInTransportbarOrProjectview != null)
                {
                    for (ObiNode n = SelectedNodeInTransportbarOrProjectview.PrecedingNode; n != null; n = n.PrecedingNode)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).TODO)
                        {
                            mProjectView.Selection = new NodeSelection(n, this);
                            return;
                        }
                    }
                }
                for (ObiNode n = mProjectView.Presentation.RootNode.LastLeaf; n != null; n = n.PrecedingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).TODO)
                    {
                        mProjectView.Selection = new NodeSelection(n, this);
                        return;
                    }
                }
            } // check for null presentation ends
        }

        private bool SelectNextEmptyNode ()
            {
            try
                {
                if (mProjectView.Presentation != null)
            {
            if (SelectedNodeInTransportbarOrProjectview != null)
                {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.FollowingNode; n != null; n = n.FollowingNode)
                    {
                    if (n is EmptyNode && !(n is PhraseNode) )
                        {
                        mProjectView.Selection = new NodeSelection ( n, this );
                        return true;
                        }
                    }
                }
            for (ObiNode n = mProjectView.Presentation.RootNode.FirstLeaf; n != null; n = n.FollowingNode)
                {
                if (n is EmptyNode && !(n is PhraseNode))
                    {
                    mProjectView.Selection = new NodeSelection ( n, this );
                    return true;
                    }
                }
                            } // check for null presentation ends
                
                                } // try ends
            catch (System.Exception )
                {
                MessageBox.Show ( Localizer.Message ( "Caption_Error" ), Localizer.Message ( "Caption_Error" ) );
                }
            return false;
                        }






        // Toggle play/pause in the transport bar
        public bool TogglePlayPause()
        {
            if (mProjectView.TransportBar.CanPause)
            {
                mProjectView.TransportBar.Pause();
                return true;
            }
            else if (mProjectView.TransportBar.CanPlay || mProjectView.TransportBar.CanResumePlayback)
            {
                mProjectView.TransportBar.PlayOrResume();
                return true;
            }
            return false;
        }


        private bool FastPlayRateStepDown()
        {
            return mProjectView.TransportBar.FastPlayRateStepDown();
        }

        private bool FastPlayRateStepUp()
        {
            return mProjectView.TransportBar.FastPlayRateStepUp();
        }

        private bool FastPlayRateNormalise()
        {
            return mProjectView.TransportBar.FastPlayRateNormalise();
        }

        private bool FastPlayNormaliseWithLapseBack()
        {
            return mProjectView.TransportBar.FastPlayNormaliseWithLapseBack();
        }

        private bool MarkSelectionFromCursor()
        {
            return mProjectView.TransportBar.MarkSelectionFromCursor();
        }

        private bool MarkSelectionToCursor()
        {
            return mProjectView.TransportBar.MarkSelectionToCursor();
        }



        #endregion

        public void SelectAtCurrentTime()
        {
            if (mPlaybackBlock != null)
                mPlaybackBlock.SelectAtCurrentTime();
        }

        public void GetFocus()
        {
            if (mSelection == null)
            {
                mProjectView.Selection = new NodeSelection(mProjectView.Presentation.FirstSection, this);
            }
            else
            {
                Focus();
            }
        }

        private void StripsView_Enter(object sender, EventArgs e)
        {
            mIsEnteringView = true;
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
                if (this.ContainsFocus)
                {
                    Strip s = mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[mStripsPanel.Controls.Count - 1] :
                        null;
                    if (s != null &&
                        ((s.ContainsFocus && s.LastBlock == null) || (s.LastBlock != null && s.LastBlock.ContainsFocus)))
                    {
                        SelectFirstStrip();
                        System.Media.SystemSounds.Beep.Play();
                        return true;
                    }
                }
            }
            else if (key == (Keys)(Keys.Shift | Keys.Tab))
            {
                if (this.ContainsFocus)
                {
                    Strip s = mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[0] : null;
                    if (s != null && s.Controls[1].ContainsFocus)
                    {
                        Strip LastStrip = mStripsPanel.Controls.Count > 0 ?
                            (Strip)mStripsPanel.Controls[mStripsPanel.Controls.Count - 1] : null;
                        if (LastStrip != null)
                        {
                            System.Media.SystemSounds.Beep.Play();
                            if (LastStrip.LastBlock != null)
                            {
                                return SelectBlockFor(delegate(Strip strip, ISelectableInContentView item) { return LastStrip.LastBlock; });
                            }
                            else
                            {
                                return SelectLastStrip();
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Update the context menu items.
        /// </summary>
        public void UpdateContextMenu()
        {
            Context_AddSectionMenuItem.Enabled = mProjectView.CanAddSection;
            Context_InsertSectionMenuItem.Enabled = mProjectView.CanInsertSection;
            Context_SplitSectionMenuItem.Enabled = CanSplitStrip;
            Context_MergeSectionWithNextMenuItem.Enabled = CanMergeStripWithNext;
            Context_AddBlankPhraseMenuItem.Enabled = mProjectView.CanAddEmptyBlock;
            Context_AddEmptyPagesMenuItem.Enabled = mProjectView.CanAddEmptyBlock;
            Context_ImportAudioFilesMenuItem.Enabled = mProjectView.CanImportPhrases;
            Context_SplitPhraseMenuItem.Enabled = mProjectView.CanSplitPhrase;
            Context_MergePhraseWithNextMenuItem.Enabled = CanMergeBlockWithNext;
            Context_CropAudioMenuItem.Enabled = mProjectView.CanCropPhrase;
            Context_PhraseIsTODOMenuItem.Enabled = mProjectView.CanSetTODOStatus && !mProjectView.TransportBar.IsActive;
            Context_PhraseIsTODOMenuItem.Checked = mProjectView.IsCurrentBlockTODO;
            Context_PhraseIsUsedMenuItem.Enabled = CanSetSelectedPhraseUsedStatus;
            Context_PhraseIsUsedMenuItem.Checked = mProjectView.IsBlockUsed;
            Context_AssignRoleMenuItem.Enabled = mProjectView.CanAssignARole;
            Context_AssignRole_PlainMenuItem.Enabled = mProjectView.CanAssignPlainRole;
            Context_AssignRole_HeadingMenuItem.Enabled = mProjectView.CanAssignHeadingRole;
            Context_AssignRole_PageMenuItem.Enabled = mProjectView.CanAssignARole;
            Context_AssignRole_SilenceMenuItem.Enabled = mProjectView.CanAssignSilenceRole;
            Context_AssignRole_NewCustomRoleMenuItem.Enabled = mProjectView.CanAssignARole;
            Context_ClearRoleMenuItem.Enabled = mProjectView.CanAssignPlainRole;
            Context_ApplyPhraseDetectionMenuItem.Enabled = mProjectView.CanApplyPhraseDetection;
            Context_CutMenuItem.Enabled = CanRemoveAudio || CanRemoveBlock || CanRemoveStrip;
            Context_CopyMenuItem.Enabled = CanCopyAudio || CanCopyBlock || CanCopyStrip;
            Context_PasteMenuItem.Enabled = mProjectView.CanPaste;
            Context_PasteBeforeMenuItem.Enabled = mProjectView.CanPasteBefore;
            Context_PasteInsideMenuItem.Enabled = mProjectView.CanPasteInside;
            Context_DeleteMenuItem.Enabled = CanRemoveAudio || CanRemoveBlock || CanRemoveStrip;
            Context_AudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
            Context_AudioSelection_BeginMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
            Context_AudioSelection_EndMenuItem.Enabled = mProjectView.CanMarkSelectionEnd;
            Context_PropertiesMenuItem.Enabled = mProjectView.CanShowSectionPropertiesDialog ||
                mProjectView.CanShowPhrasePropertiesDialog || mProjectView.CanShowProjectPropertiesDialog;
        }

        private bool CanSetSelectedPhraseUsedStatus
        {
            get
            {
                return IsBlockSelected && SelectedEmptyNode.AncestorAs<SectionNode>().Used;
            }
        }

        // Add section context menu item
        private void Context_AddSectionMenuItem_Click(object sender, EventArgs e) { mProjectView.AddStrip(); }

        // Insert section context menu item
        private void Context_InsertSectionMenuItem_Click(object sender, EventArgs e) { mProjectView.InsertSection(); }

        // Split section context menu item
        private void Context_SplitSectionMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitStrip(); }

        // Merge section with next context menu item
        private void Context_MergeSectionWithNextMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeStrips(); }

        // Add blank phrase context menu item
        private void Context_AddBlankPhraseMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyBlock(); }

        // Add empty pages context menu item
        private void Context_AddEmptyPagesMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyPages(); }

        // Import audio files context menu item
        private void Context_ImportAudioFilesMenuItem_Click(object sender, EventArgs e) { mProjectView.ImportPhrases(); }

        // Split phrase context context menu item
        private void Context_SplitPhraseMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitPhrase(); }

        // Merge phrase with next context menu item
        private void Context_MergePhraseWithNextMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeBlockWithNext(); }

        // Crop audio context menu item
        private void Context_CropAudioMenuItem_Click(object sender, EventArgs e) { mProjectView.CropPhrase(); }

        // Phrase is TODO context menu item
        private void Context_PhraseIsTODOMenuItem_Click(object sender, EventArgs e)
        {
            Context_PhraseIsTODOMenuItem.Checked = !Context_PhraseIsTODOMenuItem.Checked;
            mProjectView.ToggleTODOForPhrase();
        }

        // Phrase is used context menu item
        private void Context_PhraseIsUsedMenuItem_Click(object sender, EventArgs e)
        {
            Context_PhraseIsUsedMenuItem.Checked = !Context_PhraseIsUsedMenuItem.Checked;
            mProjectView.SetSelectedNodeUsedStatus(Context_PhraseIsUsedMenuItem.Checked);
        }

        // Assign role > Plain context menu item
        private void Context_AssignRole_PlainMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanAssignPlainRole) mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Plain, null);
        }

        private void Context_AssignRole_HeadingMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanAssignHeadingRole) mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Heading, null);
        }

        private void Context_AssignRole_PageMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanAssignARole) mProjectView.SetPageNumberOnSelection();
        }

        private void Context_AssignRole_SilenceMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanAssignSilenceRole) mProjectView.SetSilenceRoleForSelectedPhrase();
        }

        private void Context_AssignRole_NewCustomRoleMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanAssignARole) mProjectView.ShowPhrasePropertiesDialog(true);
        }

        // Clear role context menu item
        private void Context_ClearRoleMenuItem_Click(object sender, EventArgs e)
        {
        mProjectView.ClearRoleOfSelectedPhrase ();
        }

        // Apply phrase detection context menu item
        private void Context_ApplyPhraseDetectionMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ApplyPhraseDetection();
        }

        private void Context_AudioSelection_BeginMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.MarkSelectionBeginTime();
        }

        private void Context_AudioSelection_EndMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.MarkSelectionEndTime();
        }


        // Cut context menu item
        private void Context_CutMenuItem_Click(object sender, EventArgs e) { mProjectView.Cut(); }

        // Copy context menu item
        private void Context_CopyMenuItem_Click(object sender, EventArgs e) { mProjectView.Copy(); }

        // Paste context menu item
        private void Context_PasteMenuItem_Click(object sender, EventArgs e) { mProjectView.Paste(); }

        // Paste before context menu item
        private void Context_PasteBeforeMenuItem_Click(object sender, EventArgs e) { mProjectView.PasteBefore(); }

        // Paste inside context menu item
        private void Context_PasteInsideMenuItem_Click(object sender, EventArgs e) { mProjectView.PasteInside(); }

        // Delete context menu item
        private void Context_DeleteMenuItem_Click(object sender, EventArgs e) { mProjectView.Delete(); }

        // Properties context menu item
        private void Context_PropertiesMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanShowPhrasePropertiesDialog)
            {
                mProjectView.ShowPhrasePropertiesDialog(false);
            }
            else if (mProjectView.CanShowSectionPropertiesDialog)
            {
                mProjectView.ShowSectionPropertiesDialog();
            }
            else
            {
                mProjectView.ShowProjectPropertiesDialog();
            }
        }

        public void SuspendLayout_All()
        {
            Invalidate();
            foreach (Control c in mStripsPanel.Controls) c.SuspendLayout();
        }

        public void ResumeLayout_All()
        {
            foreach (Control c in mStripsPanel.Controls)
            {
                c.ResumeLayout();
                if (c is Strip) ((Strip)c).Resize_All();
            }
        }

        private void mHScrollBar_ValueChanged(object sender, EventArgs e)
        {
            mStripsPanel.Location = new Point(-mHScrollBar.Value, mStripsPanel.Location.Y);
            System.Diagnostics.Debug.Print("X = {0}/{1}, W = {2}/{3}",
                mStripsPanel.Location, mHScrollBar.Maximum, mStripsPanel.Size, VisibleWidth);
        }

        private void mVScrollBar_ValueChanged(object sender, EventArgs e)
        {
            mStripsPanel.Location = new Point(mStripsPanel.Location.X, -mVScrollBar.Value);
        }

    //@ShowSingleSection
        /*
    /// <summary>
    /// Show only the selected section.
    /// </summary>
    public void ShowOnlySelectedSection ( ObiNode node )
        {
        // Show only one strip
        SectionNode section = node is SectionNode ? (SectionNode)node : node.AncestorAs<SectionNode> ();
                foreach (Control c in mStripsPanel.Controls)
            {
            if (c is Strip ) c.Visible = ((Strip)c).Node == section;
            }
        }
        */


    }

    /// <summary>
    /// Common interface for selection of strips and blocks.
    /// </summary>
    public interface ISelectableInContentView
    {
        bool Highlighted { get; set; }                              // get or set the highlighted state of the control
        ObiNode ObiNode { get; }                                    // get the Obi node for the control
        void SetSelectionFromContentView(NodeSelection selection);  // set the selection from the parent view
    }


    /// <summary>
    /// Common interface to selectables (in the content view) that also have customizable colors.
    /// </summary>
    public interface ISelectableInContentViewWithColors : ISelectableInContentView
    {
        ContentView ContentView { get; }
        ColorSettings ColorSettings { get; }
    }
}
