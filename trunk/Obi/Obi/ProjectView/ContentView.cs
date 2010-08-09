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
        private bool mScroll = false;
        private bool mEnableScrolling;  // enable scrolling to control to show it
        private Cursor mCursor;
        private List<Strip> m_VisibleStripsList;  // @phraseLimit
        private bool m_CreatingGUIForNewPresentation;
        private bool m_IsBlocksVisibilityProcessActive;
        //private Mutex m_BlocksVisibilityOperationMutex; //@phraseLimit
       
        private delegate Strip AddStripForObiNodeDelegate ( ObiNode node );
        private delegate void RemoveControlForSectionNodeDelegate ( SectionNode node );

        /// <summary>
        /// A new strips view.
        /// </summary>
        public ContentView ()
            {
            InitializeComponent ();
            InitializeShortcutKeys ();
            mProjectView = null;
            mSelection = null;
            mFocusing = false;
            mIsEnteringView = false;
            mWrapStripContents = true; //@singleSection
            mStrips = new Dictionary<SectionNode, Strip> ();
            mWaveformRenderQ = new PriorityQueue<Waveform, int> ();
            mWaveformRenderWorker = null;
            SetPlaybackPhraseAndTime ( null, 0.0 );
            mCornerPanel.BackColor = System.Drawing.SystemColors.Control;
            mEnableScrolling = true;

            m_VisibleStripsList = new List<Strip> (); // @phraseLimit
            m_IsBlocksVisibilityProcessActive = false;
            //m_BlocksVisibilityOperationMutex = new Mutex ();// @phraseLimit
            this.contentViewLabel1.contentView = this;
            verticleScrollPane1.contentView = this;
            mStripsPanel.ControlRemoved += new ControlEventHandler( mStripsPanel_ControlRemoved);
            this.MouseWheel += new MouseEventHandler(ContentView_MouseWheel);//@singleSection
            mStripsPanel.LocationChanged += new EventHandler ( mStripsPanel_LocationChanged  );//@singleSection
            
            }
        

        // Size of the borders
        private int BorderHeight { get { return Bounds.Height - ClientSize.Height; } }
        private int BorderWidth { get { return Bounds.Width - ClientSize.Width; } }
        private int VisibleHeight { get { return Height - BorderHeight; } }
        private int VisibleWidth { get { return Width - BorderWidth; } }

        // Add a new control (normally, a strip) at the given index.
        private void AddControlAt ( Control c, int index )
            {
            mStripsPanel.Controls.Add ( c );
            mStripsPanel.Controls.SetChildIndex ( c, index );
            ReflowFromControl ( c );
            c.SizeChanged += new EventHandler ( delegate ( object sender, EventArgs e ) { ReflowFromControl ( c ); } );
            UpdateSize ();
            }

        private void ReflowFromIndex ( int index )
            {
            for (int i = index; i >= 0 && i < mStripsPanel.Controls.Count; ++i)
                {
                int y_prev = i == 0 ? 0 : mStripsPanel.Controls[i - 1].Location.Y + mStripsPanel.Controls[i - 1].Height + mStripsPanel.Controls[i - 1].Margin.Bottom;
                mStripsPanel.Controls[i].Location = new Point ( mStripsPanel.Controls[i].Margin.Left, y_prev + mStripsPanel.Controls[i].Margin.Top );
                }
            }

        private void ReflowFromControl ( Control c ) { ReflowFromIndex ( mStripsPanel.Controls.IndexOf ( c ) ); }

        // Update size of the strips panel and the scrollbars.
        private void UpdateSize ()
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
            //mVScrollBar.Maximum = h - VisibleHeight + mVScrollBar.LargeChange - 1 + mVScrollBar.Width;//@singleSection: original
            mVScrollBar.Maximum = PredictedMaxStripsLayoutHeight - VisibleHeight + mVScrollBar.LargeChange - 1 + mVScrollBar.Width;//@singleSection: new
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
                            ((StripIndexSelection)mSelection).Section.PhraseChildCount) &&
                            mProjectView.IsPhraseCountWithinLimit; //@phraseLimit
                }
            }

        public bool CanSetBlockUsedStatus { get { return IsBlockOrWaveformSelected && mSelection.Node.ParentAs<ObiNode> ().Used; } }

        public bool CanMergeBlockWithNext
            {
            get
                {
                EmptyNode node = mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock.Node : mSelectedItem is Block ? ((Block)mSelectedItem).Node : null;
                return node != null
                    && node.Index < node.ParentAs<ObiNode> ().PhraseChildCount - 1;
                }
            }

        public bool CanMergeStripWithNext
            {
            get
                {
                return IsStripSelected &&
                    mSelection.Node.IsRooted && //@singleSection
                     (mSelection.Node.Index < mSelection.Node.ParentAs<ObiNode> ().SectionChildCount - 1 ||
                        ((SectionNode)mSelection.Node).SectionChildCount > 0);
                }
            }


        /// <summary>
        /// Current color settings used by the application.
        /// </summary>
        public ColorSettings ColorSettings
            {
            get { return mProjectView == null ? null : mProjectView.ColorSettings; }
            set { if (value != null) UpdateColors ( value ); }
            }

        /// <summary>
        /// Create a command to delete the selected strip.
        /// </summary>
        public ICommand DeleteStripCommand () { return DeleteStripCommand ( SelectedSection ); }

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
                        mSelection.Node.AncestorAs<SectionNode> ().Used;
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
        public void AddCustomRoleToContextMenu ( string name, ObiForm from )
            {
            from.AddCustomRoleToMenu ( name, Context_AssignRoleMenuItem.DropDownItems, Context_AssignRole_NewCustomRoleMenuItem );
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
            if (section != null) EnsureControlVisible ( FindStrip ( section ) );
            }

        /// <summary>
        /// Get a command to merge the selected strip with the next one. If the next strip is a child or a sibling, then
        /// its contents are appended to the selected strip and it is removed from the project; but if the next strip has
        /// a lower level, merging is not possible.
        /// </summary>
        public ICommand MergeSelectedStripWithNextCommand ()
            {
            CompositeCommand command = null;
            if (CanMergeStripWithNext)
                {
                command = mProjectView.Presentation.getCommandFactory ().createCompositeCommand ();
                command.setShortDescription ( Localizer.Message ( "merge_sections" ) );
                SectionNode section = SelectedSection;
                command.append ( new Commands.UpdateSelection ( mProjectView, new NodeSelection ( section, this ) ) );
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild ( 0 );
                if (!section.Used) mProjectView.AppendMakeUnused ( command, next );
                // Delete nodes in reverse order so that they are added back in the right order on redo
                // and remove the heading role if there is any in the next section
                for (int i = next.PhraseChildCount - 1; i >= 0; --i)
                    {
                    // Remove the role before removing the node because it needs to be attached to
                    // inform its parent that it is not a heading anymore.
                    if (next.PhraseChild ( i ).Role_ == EmptyNode.Role.Heading)
                        {
                        Commands.Node.AssignRole role =
                            new Commands.Node.AssignRole ( mProjectView, next.PhraseChild ( i ), EmptyNode.Role.Plain );
                        role.UpdateSelection = false;
                        command.append ( role );
                        }
                    Commands.Node.Delete delete = new Commands.Node.Delete ( mProjectView, next.PhraseChild ( i ) );
                    delete.UpdateSelection = false;
                    command.append ( delete );
                    }
                for (int i = 0; i < next.PhraseChildCount; ++i)
                    {
                    command.append ( new
                        Commands.Node.AddNode ( mProjectView, next.PhraseChild ( i ), section, section.PhraseChildCount + i , false) );
                    }
                command.append ( DeleteStripCommand ( next ) );
                }
            return command;
            }

        /// <summary>
        /// Set a new presentation for this view.
        /// </summary>
        public void NewPresentation ()
            {
            m_CreatingGUIForNewPresentation = true;
            ClearStripsPanel ();
            m_VisibleStripsList.Clear (); // @phraseLimit
            ClearWaveformRenderQueue ();
            SuspendLayout_All ();
            if (mWrapStripContents && mProjectView.Presentation.FirstSection != null)
                {
                AddStripForSection_Safe ( mProjectView.Presentation.FirstSection );
                mProjectView.SynchronizeViews = false;
                contentViewLabel1.Name_SectionDisplayed = mProjectView.Presentation.FirstSection.Label; //@singleSection
                verticleScrollPane1.CanScrollUp = false; //@singleSection
                m_IsScrollActive = false; //@singleSection
                }
            else
                {
                AddStripForSection_Safe ( mProjectView.Presentation.RootNode ); //this will not be called in single section
                }
            CreateBlocksForInitialStrips (); //@phraseLimit
            ResumeLayout_All ();
            mProjectView.Presentation.BeforeCommandExecuted +=
                new EventHandler<urakawa.events.command.CommandEventArgs> ( Presentation_BeforeCommandExecuted );
            mProjectView.Presentation.getUndoRedoManager ().commandDone +=
                new EventHandler<urakawa.events.undo.DoneEventArgs> ( ContentView_commandDone );
            EventsAreEnabled = true;
            UpdateSize ();
            mVScrollBar.Value = 0;
            mHScrollBar.Value = 0;

            m_CreatingGUIForNewPresentation = false;
            }

        private void ContentView_commandDone ( object sender, urakawa.events.undo.DoneEventArgs e )
            {
            ResumeLayout_All ();
            UpdateSize ();
            Cursor = mCursor;

            //UpdateBlocksLabelInSelectedNodeStrip ();
            }

        private void Presentation_BeforeCommandExecuted ( object sender, urakawa.events.command.CommandEventArgs e )
            {
            mCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            SuspendLayout_All ();
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
                    mProjectView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_changed );
                    mProjectView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode> ( Presentation_RenamedSectionNode );
                    mProjectView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode> ( Presentation_UsedStatusChanged );
                    }
                else
                    {
                    mProjectView.Presentation.changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_changed );
                    mProjectView.Presentation.RenamedSectionNode -= new NodeEventHandler<SectionNode> ( Presentation_RenamedSectionNode );
                    mProjectView.Presentation.UsedStatusChanged -= new NodeEventHandler<ObiNode> ( Presentation_UsedStatusChanged );
                    }
                }
            }


        public AudioBlock PlaybackBlock { get { return mPlaybackBlock; } }

        public void SetPlaybackPhraseAndTime ( PhraseNode node, double time )
            {
            if (mPlaybackBlock != null) mPlaybackBlock.ClearCursor ();
            mPlaybackBlock = node == null ? null : (AudioBlock)FindBlock ( node );
            if (mPlaybackBlock != null)
                {
                EnsureControlVisible ( mPlaybackBlock );
                mPlaybackBlock.InitCursor ( time );
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
                if (mProjectView != null) throw new Exception ( "Cannot set the project view again!" );
                mProjectView = value;
                mProjectView.SelectionChanged += new EventHandler ( ProjectView_SelectionChanged );//@singleSection
                }
            }

        /// <summary>
        /// Rename a strip.
        /// </summary>
        public void RenameStrip ( Strip strip ) { mProjectView.RenameSectionNode ( strip.Node, strip.Label ); }

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
        public void RenderWaveform ( Waveform w, int priority )
            {
            if (mWaveformRenderQ.Enqueued ( w, priority )) mProjectView.ObiForm.BackgroundOperation_AddItem ();
            RenderFirstWaveform ();
            }


        // Render the first waveform from the queue if no other rendering is in progress.
        private void RenderFirstWaveform ()
            {
            while (mWaveformRenderWorker == null && mWaveformRenderQ.Count > 0)
                {
                Waveform w = mWaveformRenderQ.Dequeue ();
                mWaveformRenderWorker = w.Render ();
                if (mWaveformRenderWorker != null)
                    {
                    mProjectView.ObiForm.BackgroundOperation_Step ();
                    }
                }
            if (mWaveformRenderQ.Count == 0) mProjectView.ObiForm.BackgroundOperation_Done ();
            }

        private void ClearWaveformRenderQueue ()
            {
            mWaveformRenderQ.Clear ();
            if (mProjectView != null && mProjectView.ObiForm != null) mProjectView.ObiForm.BackgroundOperation_Done ();
            }

        public void FinishedRendering ( Waveform w, bool renderedOK )
            {
            mWaveformRenderWorker = null;
            RenderFirstWaveform ();
            }

        /// <summary>
        /// Get all the searchable items (i.e. strips, blocks) in the control.
        /// This does not support nested blocks right now.
        /// </summary>
        public List<ISearchable> Searchables
            {
            get
                {
                List<ISearchable> l = new List<ISearchable> ();
                AddToSearchables ( this, l );
                return l;
                }
            }

        public EmptyNode SelectedEmptyNode { get { return IsBlockSelected ? ((Block)mSelectedItem).Node : null; } }
        public ObiNode SelectedNode { set { if (mProjectView != null) mProjectView.Selection = new NodeSelection ( value, this ); } }
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
                    if (value != null) CreateSelectedStripAndPhraseBlocks ( value );//@singleSection: creates strip to be selected
                    //if (value != null && (value.Node is EmptyNode || value is StripIndexSelection)) CreateBlocksInStrip ();//@sindleSection: temporary disabled for experiments
                    ISelectableInContentView s = value == null ? null : FindSelectable ( value );

                    if (s == null && IsBlockInvisibleButStripVisible ( value ) && this == null)//@singleSection: last check added to make it compulsory to go to else
                    { /* do nothing */ }
                    else
                        {
                        if (mSelectedItem != null) mSelectedItem.Highlighted = false;
                        mSelection = value;
                        mSelectedItem = s;
                        
                        }

                    if (s != null)
                        {
                        s.SetSelectionFromContentView ( mSelection );
                        SectionNode section = value.Node is SectionNode ? (SectionNode)value.Node :
                            value.Node.ParentAs<SectionNode> ();
                        mProjectView.MakeTreeNodeVisibleForSection ( section );
                        EnsureControlVisible ( (Control)s );
                        mFocusing = true;
                        if (!((Control)s).Focused) ((Control)s).Focus ();
                        mFocusing = false;

                        RemoveBlocksBelowContentViewVisibleArea ();//@singleSection: explicitly call remove after rearrangement of strip panel
                        }
                    }
                }
            }
        // @phraseLimit
        /// <summary>
        /// returns true if phrase block is  invisible but the strip of phrase exists
        /// </summary>
        /// <param name="sel"></param>
        /// <returns></returns>
        private bool IsBlockInvisibleButStripVisible ( NodeSelection sel )
            {
            if (sel == null)
                return false;

            ObiNode node = sel.Node;
            if (node is EmptyNode && node.IsRooted)
                {
                SectionNode parent = node.ParentAs<SectionNode> ();
                Strip s = FindStrip ( parent );

                if (s != null && s.FindBlock ( (EmptyNode)node ) == null)
                    {
                    return true;
                    }
                }
            return false;
            }


        // @phraseLimit
        /// <summary>
        /// Check if block for phrase passed as parameter is invisible while its strip is visible
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsBlockInvisibleButStripVisible ( EmptyNode node )
            {
            if (node != null && node is EmptyNode && node.IsRooted)
                {
                SectionNode parent = node.ParentAs<SectionNode> ();
                Strip s = FindStrip ( parent );

                if (s != null && s.FindBlock ( (EmptyNode)node ) == null)
                    {
                    return true;
                    }
                }
            return false;
            }


        /// <summary>
        /// Disable scrolling when clicking on an element. 
        /// </summary>
        public void DisableScrolling () { mEnableScrolling = false; }

        // Ensure that the playback cursor is visible given its position in the waveform.
        private void EnsureCursorVisible ( int x )
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
        private void EnsureControlVisible ( Control c )
            {
            if (mEnableScrolling)
                {
                // Find the parent strip
                if (!(c is Strip))
                    {
                    Control strip = c.Parent;
                    while (!(strip is Strip))
                        {
                        if (strip == null || strip.Parent == null) break;
                        strip = strip.Parent;
                        }
                    if (strip != null && strip is Strip)
                        {
                        //@singleSection: adding following if block to prevent useless shifting of strip
                        if (strip.Parent == mStripsPanel
                            && (mStripsPanel.Location.Y + strip.Bottom < 0 || mStripsPanel.Location.Y + strip.Top > mStripsPanel.Margin.Top + strip.Margin.Top))
                            {
                            Console.WriteLine ( "explicitly ensure visibility of parent strip " );
                            EnsureControlVisible ( strip );
                            }
                        }
                    }
                // Compute the location of the control relative to the strips panel
                // (Location is relative to its direct parent.)
                Point location = c.Location;
                Control parent = c.Parent;
                while (parent != null && parent != mStripsPanel)
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
                //int vh = VisibleHeight - mHScrollBar.Height; //@singleSection: original, replaced by below line
                int vh = mHScrollBar.Location.Y; //@singleSection : new
                int h_max = mHScrollBar.Maximum - mHScrollBar.LargeChange + 1;
                //int vw = VisibleWidth - mVScrollBar.Width; //@singleSection: original, replaced by following
                int vw = verticleScrollPane1.Location.X; //@singleSection : new

                // Vertical scrolling
                if (t < 0 || (b > vh && h > vh))
                    {
                    // Top of control is above the visible window, so scroll to the top
                    //@singleSection : we need to remove VScroll bar so this code should directly work on mStripsPanel
                    //mVScrollBar.Value = Math.Min ( top, v_max );
                    mStripsPanel.Location =new Point ( mStripsPanel.Location.X ,
                        Math.Min ( top, v_max )* -1);
                    }
                else if (b > vh)
                    {
                    // Top of control is below the visible window; scroll to align the
                    // bottom of the control to the bottom of the visible window; unless
                    // the control is taller than the visible window, in which case we
                    // want to see the top of the control in priority (this is handled
                    // above.)
                    //@singleSection : we need to remove VScroll bar so this code should directly work on mStripsPanel
                    //mVScrollBar.Value = Math.Min ( bottom - vh, v_max );
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                        Math.Min ( bottom - vh, v_max ) * -1);
                    }

                // Horizontal scrolling is the same
                if (l < 0 || (r > vw && w > vw))
                    {
                    mHScrollBar.Value = Math.Min ( left, h_max );
                    }
                else if (r > vw)
                    {
                    mHScrollBar.Value = Math.Min ( right - vw, h_max );
                    }
                }
            else
                {
                mEnableScrolling = true;
                }
            }

        public void SelectNextPhrase ( ObiNode node )
            {
            if (mSelection != null)
                {
                SelectFollowingBlock ();
                }
            else if (node is SectionNode)
                {
                mSelectedItem = FindStrip ( (SectionNode)node );
                SelectFirstBlockInStrip ();
                }
            else
                {
                SelectFirstStrip ();
                SelectFirstBlockInStrip ();
                }
            }

        /// <summary>
        /// Show/hide strips under the one for which the section was collapsed or expanded.
        /// </summary>
        public void SetStripsVisibilityForSection ( SectionNode section, bool visible )
            {
            for (int i = 0; i < section.SectionChildCount; ++i)
                {
                Strip s;
                SectionNode child = section.SectionChild ( i );
                if ((s = FindStrip ( child )) != null)
                    {
                    s.Visible = visible;
                    if (mSelectedItem == s && !visible) mProjectView.Selection = null;
                    SetStripsVisibilityForSection ( section.SectionChild ( i ), visible );
                    }
                }
            }

        public void SetStripVisibilityForSection ( SectionNode node, bool visible )
            {
            Strip s = FindStrip ( node );
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
        public CompositeCommand SplitStripCommand ()
            {
            CompositeCommand command = null;
            if (CanSplitStrip)
                {
                EmptyNode node = Selection.EmptyNodeForSelection;
                SectionNode section = node.ParentAs<SectionNode> ();
                command = mProjectView.Presentation.getCommandFactory ().createCompositeCommand ();
                command.setShortDescription ( Localizer.Message ( "split_section" ) );
                command.append ( new Commands.UpdateSelection ( mProjectView, new NodeSelection ( node, this ) ) );
                // Add a sibling with a new label
                SectionNode sibling = mProjectView.Presentation.CreateSectionNode ();
                sibling.Label = section.Label + "*";
                Commands.Node.AddNode add = new Commands.Node.AddNode ( mProjectView, sibling, section.ParentAs<ObiNode> (),
                    section.Index + 1 );
                add.UpdateSelection = false;
                command.append ( add );
                // Change parents of children to insert the section at the right position in strip order
                for (int i = section.SectionChildCount - 1; i >= 0; --i)
                    {
                    command.append ( new Commands.Node.Delete ( mProjectView, section.SectionChild ( i ), false ) );
                    }
                for (int i = 0; i < section.SectionChildCount; ++i)
                    {
                    command.append ( new Commands.Node.AddNode ( mProjectView, section.SectionChild ( i ), sibling, i, false ) );
                    }
                // Split the node if necessary
                PhraseNode splitNode = null;
                PhraseNode cropNode = null;
                if (mProjectView.CanSplitPhrase)
                    {
                    ICommand splitCommand = Commands.Node.SplitAudio.GetSplitCommand ( mProjectView );
                    if (splitCommand != null) command.append ( splitCommand );
                    splitNode = Commands.Node.SplitAudio.GetSplitNode ( splitCommand );
                    //@singleSection  work around to avoid triggering strip creation due to unknown selection of split phrase
                    if (splitNode != null) command.append ( new Commands.UpdateSelection ( mProjectView, new NodeSelection ( splitNode, this ) ) );
                    if (splitNode != null) cropNode = Commands.Node.SplitAudio.GetCropNode ( splitCommand, splitNode );
                    }
                // Move children from the context phrase to the new sibling
                int sectionOffset = node.Index + (splitNode != null ? 1 : 0);
                for (int i = section.PhraseChildCount - 1; i >= sectionOffset; --i)
                    {
                    command.append ( new Commands.Node.Delete ( mProjectView, section.PhraseChild ( i ), false ) );
                    }
                if (cropNode != null) command.append ( new Commands.Node.Delete ( mProjectView, cropNode, section, node.Index + 2, false ) );
                if (splitNode != null)
                    {
                    command.append ( new Commands.Node.Delete ( mProjectView, splitNode, section, node.Index + 1, false ) );
                    command.append ( new Commands.Node.AddNode ( mProjectView, splitNode, sibling, 0, false ) );
                    }
                if (cropNode != null) command.append ( new Commands.Node.AddNode ( mProjectView, cropNode, sibling, 1, false ) );
                int siblingOffset = node.Index - (cropNode != null ? 1 : 0);
                for (int i = sectionOffset; i < section.PhraseChildCount; ++i)
                    {
                    command.append ( new
                        Commands.Node.AddNode ( mProjectView, section.PhraseChild ( i ), sibling, i - siblingOffset, false ) );
                    }
                
                command.append ( new Commands.UpdateSelection ( mProjectView, new NodeSelection ( sibling, this ) ) );
                }
            return command;
            }

        /// <summary>
        /// String to be shown in the status bar.
        /// </summary>
        public override string ToString () { return Localizer.Message ( "strips_view_to_string" ); }

        /// <summary>
        /// Views are not synchronized anymore, so make sure that all strips are visible.
        /// </summary>
        public void UnsyncViews () { foreach (Control c in mStripsPanel.Controls) c.Visible = true; }

        public void UpdateCursorPosition ( double time )
            {
            if (PlaybackBlock == null && m_EnableFindPlaybackBlockDuringCursorUpdate && mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)//@singleSection
                {
                m_EnableFindPlaybackBlockDuringCursorUpdate = false;
                SetPlaybackPhraseAndTime ( mProjectView.TransportBar.CurrentPlaylist.CurrentPhrase, mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                }
            if (mPlaybackBlock != null) EnsureCursorVisible ( mPlaybackBlock.UpdateCursorTime ( time ) );
            }

        private void UpdateBlocksLabelInSelectedNodeStrip ()
            {
            if (mProjectView.Presentation != null && mProjectView.Selection != null)
                {
                if (mProjectView.Selection.Node is SectionNode) UpdateBlocksLabelInStrip ( (SectionNode)mProjectView.Selection.Node );
                else if (mProjectView.Selection.Node is EmptyNode) UpdateBlocksLabelInStrip ( mProjectView.Selection.Node.ParentAs<SectionNode> () );
                }
            }

        private void UpdateBlocksLabelInStrip ( SectionNode section )
            {
            Strip s = FindStrip ( section );
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
                if (mProjectView.Presentation == null) return;
                SectionNode selectedSection = mProjectView.GetSelectedPhraseSection;
                if (mProjectView.Presentation != null && selectedSection == null)
                    {
                    MessageBox.Show ( "A section or phrase should be select to wrap" );
                    return;
                    }
                
                //mWrapStripContents = value;
                mWrapStripContents = true; //@singleSection

                if (mWrapStripContents)
                    {
                    CreateStripForWrappedContent ();
                    mProjectView.SynchronizeViews = false;
                    
                    }
                else // is unwrap
                    {
                    RemoveStripsForSection_Safe ( selectedSection);
                    AddStripForSection_Safe ( mProjectView.Presentation.RootNode );
                    }
                UpdateSize ();
                }
            }

        private void CreateStripForWrappedContent ()
            {
            SectionNode selectedSection = mProjectView.GetSelectedPhraseSection;
            for (int i = mStripsPanel.Controls.Count - 1; i >= 0; --i)
                {
                Strip strip = mStripsPanel.Controls[i] as Strip;
                //@singleSection: adding check for mStripsPanel.Controls.Count because it is not necessary in unsync state that selected node is shown
                if (strip != null && mStripsPanel.Controls.Count >= 1)
                    {
                    if ( ( selectedSection == null && i == 0 )
                        || ( selectedSection != null &&  strip.Node == selectedSection) )
                        {
                        strip.WrapContents = mWrapStripContents;
                        }
                    else
                        {
                        //MessageBox.Show ( strip.Node.Label );
                        RemoveStripsForSection_Safe ( strip.Node );
                        }
                    }
                }

            }


        public float AudioScale
            {
            get { return mProjectView == null ? 0.01f : mProjectView.AudioScale; }
            set
                {
                foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).AudioScale = value;
                UpdateSize ();
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
                ClearWaveformRenderQueue ();
                foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).ZoomFactor = value;
                UpdateSize ();
                this.contentViewLabel1.contentView = this;
                this.contentViewLabel1.zoomFactor = ZoomFactor;
                mHScrollBar.Location = new Point(mHScrollBar.Location.X, this.Height - contentViewLabel1.Height - mHScrollBar.Height);
                mVScrollBar.Height = mVScrollBar.Location.Y + this.Height - contentViewLabel1.Height - mHScrollBar.Height;
                mCornerPanel.Location = new Point(mCornerPanel.Location.X, this.Height - contentViewLabel1.Height - mHScrollBar.Height);
                }
            }


        // Add a new strip for a section and all of its subsections
        private Strip AddStripForSection_Safe ( ObiNode node )
            {
            if (InvokeRequired)
                {
                return Invoke ( new AddStripForObiNodeDelegate ( AddStripForSection_Safe ), node ) as Strip;
                }
            else
                {
                SuspendLayout ();
                Strip strip = AddStripForSection ( node );
                ResumeLayout ();
                return strip;
                }
            }

        // Add a single strip for a section node
        private Strip AddStripForSection ( ObiNode node )
            {
            Strip strip = null;
            if (node is SectionNode)
                {
                if (mStrips.ContainsKey ( (SectionNode)node ))
                    {
                    //@singleSection : code change start
                    strip = new Strip ( (SectionNode)node, this );
                    mStrips[(SectionNode)node] = strip;
                    strip.WrapContents = mWrapStripContents;
                    strip.ColorSettings = ColorSettings;
                    //@singleSection : ends
                    // following commented temporarily to avoid bugs due to old strips
                    //strip = mStrips[(SectionNode)node];
                    //if (strip != null) strip.RefreshStrip ();
                    //if (strip != null && !strip.IsBlocksVisible && !m_CreatingGUIForNewPresentation)//@singleSection
                    if (strip != null && !m_CreatingGUIForNewPresentation)
                        {
                        CreateBlocksInStrip ( strip );
                        }
                    else if (strip != null && m_VisibleStripsList != null && !m_VisibleStripsList.Contains ( strip ))
                        {
                        // just add strip to visible strip list
                        AddStripToVisibleStripsList ( strip );
                        }
                    }
                else
                    {
                    strip = new Strip ( (SectionNode)node, this );
                    mStrips[(SectionNode)node] = strip;
                    strip.WrapContents = mWrapStripContents;
                    strip.ColorSettings = ColorSettings;
                    //for (int i = 0; i < node.PhraseChildCount; ++i) strip.AddBlockForNode(node.PhraseChild(i)); // @phraseLimit
                    if (!m_CreatingGUIForNewPresentation) // @phraseLimit
                        {
                        CreateBlocksInStrip ( strip );
                        }

                    }
                AddControlAt ( strip, ((SectionNode)node).Position );
                }
            if (!mWrapStripContents)
                {
                for (int i = 0; i < node.SectionChildCount; ++i) AddStripForSection ( node.SectionChild ( i ) ); // this will not be called in single section
                }
            return strip;
            }


        // @phraseLimit
        /// <summary>
        /// Remove all strips from strips panel and destroy their handle
        /// </summary>
        private void ClearStripsPanel ()
            {
            // destroy handles of all controls
            for (int i = 0; i < mStripsPanel.Controls.Count; i++)
                {
                mStripsPanel.Controls[i].Dispose ();
                }
            // dispose visible strips also
            for (int i = 0; i < m_VisibleStripsList.Count; i++)
                {
                if (m_VisibleStripsList[i] != null)
                    {
                    m_VisibleStripsList[i].Dispose ();
                    m_VisibleStripsList[i] = null;
                    }
                }
            mStripsPanel.Controls.Clear ();
            }

        // @phraseLimit
        private int VisibleBlocksCount
            {
            get
                {
                int count = 0;
                for (int i = 0; i < m_VisibleStripsList.Count; i++)
                    count += m_VisibleStripsList[i].Node.PhraseChildCount;

                return count;
                }
            }

        // @phraseLimit
        /// <summary>
        /// create prase blocks starting from begining of project such that visible blocks count is not more than MaxVisibleBlocksCount
        /// </summary>
        private void CreateBlocksForInitialStrips ()
            {
            Point prevPoint = new Point ( -100, -100 );
            int visibleStripsCount = 0;
            for (int i = 0; i < mStripsPanel.Controls.Count; i++)
                {

                if (mStripsPanel.Controls[i] is Strip)
                    {
                    Strip s = (Strip)mStripsPanel.Controls[i];
                    // make phrases in strip visible if visible phrase count is within limit, 
                    //strip lie well inside panel and visibleStripsCount is less or equal to 500
                    if (s.Node.PhraseChildCount <= mProjectView.MaxVisibleBlocksCount - VisibleBlocksCount
                        && prevPoint != s.Location
                        && visibleStripsCount <= 500)
                        {
                        CreateBlocksInStrip ( s );// uncomment for prev block loading
                        //s.LoadBlocksInLayoutIfRequired (); // //@singleSection : comment to restore old block loading
                        visibleStripsCount++;
                        }
                    else return;

                    prevPoint = s.Location;
                    }
                }

            }

        //@singleSection
        private void CreateSelectedStripAndPhraseBlocks ( NodeSelection selectionValue )
            {
            if (selectionValue == null) return;

            if (selectionValue.Node is SectionNode ||
                selectionValue.Node is EmptyNode ||
                selectionValue is StripIndexSelection)
                {
                Strip currentlyActiveStrip = ActiveStrip ;
                SectionNode sectionToBeSelected = selectionValue.Node is SectionNode ? (SectionNode)selectionValue.Node :
                                                selectionValue.Node.ParentAs<SectionNode> ();
                
                // remove irrelevant strips in case there are more than single strip in content view
                Strip requiredExistingStrip = null;

                if (mStripsPanel.Controls.Count > 1)
                    {
                    foreach (Control c in mStripsPanel.Controls)
                        {
                        if (c is Strip)
                            {
                            Strip iterationStrip = (Strip)c;
                            if (iterationStrip.Node == sectionToBeSelected)
                                {
                                requiredExistingStrip = iterationStrip;
                                contentViewLabel1.Name_SectionDisplayed = sectionToBeSelected.Label;
                                Console.WriteLine ("the required strip exists " + iterationStrip.Node.Label ) ;
                                }
                            else
                                {
                                RemoveStripsForSection_Safe ( iterationStrip.Node );
                                }
                            }
                        }
                    }
                if (requiredExistingStrip != null )
                    {
                    if (selectionValue.Node is EmptyNode || selectionValue is StripIndexSelection)
                        {
                        CreateLimitedBlocksInStrip ( requiredExistingStrip, selectionValue is StripIndexSelection ? ((StripIndexSelection)selectionValue).EmptyNodeForSelection : (EmptyNode)selectionValue.Node );
                        }
                    return;
                    }

                if (currentlyActiveStrip == null
                    || (currentlyActiveStrip != null
                    && sectionToBeSelected != currentlyActiveStrip.Node))
                    {
                    CreateStripForSelectedSection ( sectionToBeSelected,
                                                true);

                    
                    }
                if (selectionValue.Node is EmptyNode || selectionValue is StripIndexSelection)
                    {
                    CreateLimitedBlocksInStrip ( currentlyActiveStrip, selectionValue is StripIndexSelection ? ((StripIndexSelection)selectionValue).EmptyNodeForSelection : (EmptyNode)selectionValue.Node );
                    }
                }
            
            }

        public Strip CreateStripForAddedSectionNode ( SectionNode node, bool removeExisting )//@singleSection
            {
            if (ActiveStrip == null)
                {
                return CreateStripForSelectedSection ( node, removeExisting );
                }
            else
                {
                return null;
                }
            }


        public Strip CreateStripForSelectedSection ( SectionNode node, bool removeExisting )//@singleSection
            {
            if (IsStripVisible ( node )) return null;

            // first remove existing strip
            if (removeExisting)
                {
               Strip requiredExistingStrip = null;
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip && ((Strip)c).Node == node)
                        {
                        requiredExistingStrip = (Strip) c ;
                        }
                    else if (c is Strip )
                        {
                        RemoveStripsForSection_Safe ( ((Strip)c).Node );
                        }
                    }
                if (requiredExistingStrip != null)
                    {
                    contentViewLabel1.Name_SectionDisplayed = requiredExistingStrip.Node.Label;
                    return requiredExistingStrip;
                    }
                }
            //Console.WriteLine ("creating strip " + node.Label ) ;
            // now add strip for section in parameter
            contentViewLabel1.Name_SectionDisplayed = node.Label;
            verticleScrollPane1.CanScrollUp = false;
            return AddStripForSection ( node );
            }

        /// <summary>
        /// returns true if the single section shown in content view is the section passed as parameter
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool IsStripVisible ( SectionNode section )
            {
            foreach (Control c in mStripsPanel.Controls)
                {
                if (c is Strip && ((Strip)c).Node == section)
                    {
                    return true;
                    }
                }
            return false;
            }

        // @phraseLimit
        /// <summary>
        /// Make phrase block visible for selected strip
        /// </summary>
        /// <returns></returns>
        public bool CreateBlocksInStrip ()
            {
            if (mWrapStripContents) CreateStripForWrappedContent ();
            Strip s = StripForSelection;
            if (s == null && mProjectView.GetSelectedPhraseSection != null)
                s = FindStrip ( mProjectView.GetSelectedPhraseSection );

            //@singleSection : comment this code for restoring  old block loading
            //if (s != null)
                //{
                //s.LoadBlocksInLayoutIfRequired ();
                //return true;
                //}
            //else
                //{
                //return false;
                //}
            // commentting end for restoring

            return CreateBlocksInStrip ( s != null ? s : null ); // uncomment this for restoring old block behaviour
            }

        bool m_EnableFindPlaybackBlockDuringCursorUpdate = false;
        //@singleSection
        private bool CreateLimitedBlocksInStrip ( Strip stripControl, EmptyNode requiredEmptyNode )
            {
            if (stripControl != null && stripControl.Node.PhraseChildCount > 0)
                {
                

                int defaultVisibleCount = 40;
                int extraBlocksCount = 15;
                
                bool shouldRemoveBlocks = true;
                bool wasPlaybackOn = false;
                bool canMoveSelectionToPlaybackPhrase = mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase;
                try
                    {
                    if (mProjectView.Selection == null ||
                        stripControl.FirstBlock == null || //this means that no block is created in strip
                        (mProjectView.Selection != null && 
                        !(mProjectView.Selection.Node is EmptyNode) &&
                        !(mProjectView.Selection is StripIndexSelection) ))
                        {
                        
                        // check if block for defaultBlockCount index is there
                        Block v = stripControl.FindBlock ( stripControl.Node.PhraseChildCount < defaultVisibleCount ? stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 ) :
    stripControl.Node.PhraseChild ( defaultVisibleCount - 1 ) );

                        if (v == null)
                            {
                            shouldRemoveBlocks = false;
                            int maxCount = stripControl.Node.PhraseChildCount < defaultVisibleCount ? stripControl.Node.PhraseChildCount : defaultVisibleCount;
                            // pause playback if it is active.
                            if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                                {
                                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                                wasPlaybackOn = true;
                                mProjectView.TransportBar.Pause ();
                                }
                            for (int i = 0; i < 100 && !stripControl.IsContentViewFilledWithBlocks; ++i)
                                {
                                if ((maxCount < defaultVisibleCount && i >= maxCount )
                                    || i >= stripControl.Node.PhraseChildCount )
                                                                        {
                                    Console.WriteLine ( "Adding block stopped at " + i.ToString () );
                                    //MessageBox.Show ( maxCount.ToString ()  );
                                    break;
                                    }
                                stripControl.AddBlockForNode ( stripControl.Node.PhraseChild ( i ) );
                                }

                            if (requiredEmptyNode != null)
                                {
                                Block requiredBlock = stripControl.FindBlock ( requiredEmptyNode );
                                if (requiredBlock == null) CreateBlocksTillNodeInStrip ( stripControl, requiredEmptyNode, false );
                                }

                                                        }
                        }
                    else
                        {
                        ObiNode selectedNode = null;
                        if (mProjectView.Selection is StripIndexSelection)
                            {
                            selectedNode = ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection;
                            
                            }
                        if ( selectedNode == null )  selectedNode = mProjectView.Selection.Node;

                        Block lastBlockInStrip = stripControl.LastBlock;
                        if (lastBlockInStrip != null
                            && lastBlockInStrip.Node.IsRooted
                            && selectedNode.IsRooted
                                                        && ((lastBlockInStrip.Node.Index - selectedNode.Index >= 15
                                                        && requiredEmptyNode == null)
                                || (lastBlockInStrip.Node == stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 ))))
                            {
                            shouldRemoveBlocks = true;
                            }
                        else if ( selectedNode != null && selectedNode.IsRooted )
                            {//2

                            if (requiredEmptyNode != null && lastBlockInStrip != null && lastBlockInStrip.Node.IsRooted)
                                {
                                if (lastBlockInStrip.Node.Index < requiredEmptyNode.Index) CreateBlocksTillNodeInStrip ( stripControl, requiredEmptyNode, false );
                                }
                            //ObiNode currentNode = selectedNode.FollowingNode; // lets start from selected node
                            ObiNode currentNode = selectedNode;

                            // pause playback if it is active.
                            if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                                {
                                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                                wasPlaybackOn = true;
                                mProjectView.TransportBar.Pause ();
                                }

                            EmptyNode intendedFirstNodeAfterRemoval =  RemoveAllblocksInStripIfRequired ( stripControl, selectedNode, true);

                            if (intendedFirstNodeAfterRemoval != null)
                                {
                                int intermediateBlocksCount = selectedNode.Index - intendedFirstNodeAfterRemoval.Index;
                                Console.WriteLine ( "selection removal : extra block : intermediate count " + extraBlocksCount + " " + intermediateBlocksCount );
                                extraBlocksCount += intermediateBlocksCount;
                                currentNode = intendedFirstNodeAfterRemoval;
                                }

                            for (int i = 0; i < extraBlocksCount || !stripControl.IsContentViewFilledWithBlocks ; i++)
                                {//3
                                if (currentNode == null  ||
                                    !(currentNode is EmptyNode) ||
                                    currentNode.ParentAs<SectionNode> () != stripControl.Node)
                                    {//4
                                    Console.WriteLine ( "Adding extra blocks exit at " + i.ToString () );
                                    break;
                                    }//-4

                                Block currentNodeBlock = stripControl.FindBlock ( (EmptyNode)currentNode );
                                if (currentNodeBlock == null)
                                    {//4
                                    shouldRemoveBlocks = false;
                                    stripControl.AddBlockForNode ( (EmptyNode)currentNode );
                                    }//-4
                                currentNode = currentNode.FollowingNode;
                                }//-3
                            
                            }//-2
                        
                        UpdateSize ();
                        if (!shouldRemoveBlocks) stripControl.UpdateColors ();
                        }

                    if (shouldRemoveBlocks)
                        {
                        if (mProjectView.Selection != null && mProjectView.Selection.Node.IsRooted
                            && ( mProjectView.Selection.Node is EmptyNode || mProjectView.Selection is StripIndexSelection))
                            {
                            ObiNode currentPhraseNode = mProjectView.Selection is StripIndexSelection ? (( StripIndexSelection) mProjectView.Selection) .EmptyNodeForSelection: 
                                mProjectView.Selection.Node;

                            int currentPhraseIndex = (currentPhraseNode != null && currentPhraseNode.IsRooted) ? currentPhraseNode.Index : -1;
                            if (currentPhraseIndex == -1) return true;

                                                        if (mSelection != null && (mSelection.Node is EmptyNode || mSelection is StripIndexSelection)
                                                            && (mProjectView.Selection.Node.isSiblingOf (mSelection.Node)    ||    mProjectView.Selection.Node == mSelection.Node))
                                                            {
                                                            int contentViewSelectionIndex = mSelection is StripIndexSelection ? ((StripIndexSelection)mSelection).EmptyNodeForSelection.Index : mSelection.Node.Index;
                                                            if (currentPhraseIndex < contentViewSelectionIndex) currentPhraseIndex = contentViewSelectionIndex;
                                                            
                                                                                                                        }
                                                        if (stripControl.Node.PhraseChildCount <= currentPhraseIndex + 15) return true;
                                                        
                                                        if (currentPhraseIndex <= defaultVisibleCount) currentPhraseIndex = defaultVisibleCount-1 ;
                                                        
                                                        //System.Media.SystemSounds.Asterisk.Play ();
                            EmptyNode lastIntentedVisiblePhrase = stripControl.Node.PhraseChildCount > currentPhraseIndex + 15 ?   stripControl.Node.PhraseChild ( currentPhraseIndex + 15 ):
                                stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 );


                            if (stripControl.IsContentViewFilledWithBlocks)
                                {
                                // pause playback if it is active.
                                if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                                    {
                                    mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                                    wasPlaybackOn = true;
                                    mProjectView.TransportBar.Pause ();
                                    }
                                stripControl.RemoveAllFollowingBlocks ( lastIntentedVisiblePhrase, true, false );
                                UpdateSize ();
                                }
                            }
                        }
                    
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                if (wasPlaybackOn)
                    {
                    mProjectView.TransportBar.PlayOrResume ();
                    //SetPlaybackPhraseAndTime ( mProjectView.TransportBar.CurrentPlaylist.CurrentPhrase, mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                    m_EnableFindPlaybackBlockDuringCursorUpdate = true;
                    }
                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = canMoveSelectionToPlaybackPhrase;
                return true;

}
            
            return true;
            }

        //@singleSection
        private void RemoveBlocksBelowContentViewVisibleArea ()
            {
            if (mProjectView.Selection == null) return;
            
            ObiNode currentlySelectedNode = mProjectView.Selection is StripIndexSelection? ((StripIndexSelection) mProjectView.Selection).EmptyNodeForSelection: mProjectView.Selection.Node;

            if (currentlySelectedNode != null &&  currentlySelectedNode is SectionNode )
                {
                if (((SectionNode)currentlySelectedNode).PhraseChildCount > 0)
                    {
                    currentlySelectedNode = currentlySelectedNode.PhraseChildCount >40?  ((SectionNode)currentlySelectedNode).PhraseChild ( 40 ): 
                    currentlySelectedNode.PhraseChild ( currentlySelectedNode.PhraseChildCount - 1 );
                    }
                else 
                    {
                    return;
                    }
                }
            if ( currentlySelectedNode == null || ( currentlySelectedNode != null && !currentlySelectedNode.IsRooted )) return ;

            Strip stripControl = FindStrip ( currentlySelectedNode.ParentAs<SectionNode> () );

            if (stripControl != null    &&    stripControl.IsContentViewFilledWithBlocks)
                {
                
                bool wasPlaybackOn = false;
                bool canMoveSelectionToPlaybackPhrase = mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase ;
                // pause playback if it is active.
                if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                    {
                    mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                    wasPlaybackOn = true;
                    mProjectView.TransportBar.Pause ();
                    }

                try
                    {
                    EmptyNode lastIntentedVisiblePhrase = (EmptyNode)currentlySelectedNode;
                    stripControl.RemoveAllFollowingBlocks ( lastIntentedVisiblePhrase, true, false );
                    Console.WriteLine ( "remove explicitly  atmost till " + lastIntentedVisiblePhrase);
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }

                UpdateSize ();

                if (wasPlaybackOn)
                    {
                    mProjectView.TransportBar.PlayOrResume ();
                    //SetPlaybackPhraseAndTime ( mProjectView.TransportBar.CurrentPlaylist.CurrentPhrase, mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                    m_EnableFindPlaybackBlockDuringCursorUpdate = true;
                    }
                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = canMoveSelectionToPlaybackPhrase;
                }
            }

        //@singleSection
        public void CreateBlocksTillEndInStrip ( Strip stripControl )
            {
            CreateBlocksTillNodeInStrip ( stripControl, null , false);
            }
        
        //@singleSection
        public void CreateBlocksTillNodeInStrip ( Strip stripControl, EmptyNode nodeOfLastBlockToCreate, bool considerStripHaltFlag )
            {
            CreateBlocksTillNodeInStrip ( stripControl, nodeOfLastBlockToCreate, considerStripHaltFlag, 0 );
            }
        
        //@singleSection
        public void CreateBlocksTillNodeInStrip ( Strip stripControl, EmptyNode nodeOfLastBlockToCreate, bool considerStripHaltFlag, int pixelDepth)
            {
            Block firstBlock = stripControl.FirstBlock;
            Block lastBlock = stripControl.LastBlock;
            if (firstBlock != null  &&  lastBlock != null)
                {
                EmptyNode startNode = lastBlock.Node;
                int startNodeIndex = firstBlock.Node.Index;

                if ( nodeOfLastBlockToCreate == null ) nodeOfLastBlockToCreate = stripControl.Node.PhraseChild(stripControl.Node.PhraseChildCount -1 ) ;
                EmptyNode firstNodeAfterRemove = RemoveAllblocksInStripIfRequired ( stripControl, 
                    nodeOfLastBlockToCreate,
                    nodeOfLastBlockToCreate.Index >= firstBlock.Node.Index? true : false );
                
                if (firstNodeAfterRemove  != null)
                    {
                    startNode = firstNodeAfterRemove;
                    startNodeIndex = firstNodeAfterRemove.Index;
                    Console.WriteLine ( "Start node aftger removal " + startNode.Index );
                    }

                bool wasPlaybackOn = false;
                bool canMoveSelectionToPlaybackPhrase = mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase;
                if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                    {
                    mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                    wasPlaybackOn = true;
                    mProjectView.TransportBar.Pause ();
                    }
                if ( !considerStripHaltFlag && pixelDepth == 0) stripControl.AddsRangeOfBlocks ( startNode, nodeOfLastBlockToCreate != null ? nodeOfLastBlockToCreate : stripControl.Node.PhraseChild(stripControl.Node.PhraseChildCount - 1 ) );
                // start from beginning and create blocks for nodes for after the last block node.
                bool shouldStartCreating = stripControl.Node.PhraseChild ( startNodeIndex ) == startNode ? true : false;
                for (int i = startNodeIndex; i < stripControl.Node.PhraseChildCount; i++)
                    {
                    //System.Media.SystemSounds.Asterisk.Play ();
                    if (considerStripHaltFlag && stripControl.IsContentViewFilledWithBlocks
                        && (i % 5 == 0 || i <= 1))
                        {
                        Console.WriteLine ( "block creation quit index for scroll " + i.ToString () );
                        break;
                        }
                    else if ( pixelDepth > 0 && stripControl.IsContentViewFilledWithBlocksTillPixelDepth ( pixelDepth ))
                        {
                        Console.WriteLine ( "block creation quit index for scroll for pixcel depth" + i.ToString () );
                        break;
                        }

                    EmptyNode node = stripControl.Node.PhraseChild ( i );
                    if (shouldStartCreating)
                        {
                        stripControl.AddBlockForNode ( node );
                        }

                    if (node != null && node == nodeOfLastBlockToCreate)
                        {
                        // if nod is null then keep on creating block till end of strip
                        break;
                        }

                    if (node == startNode)
                        {
                        shouldStartCreating = true;
                        }
                    }
                UpdateSize ();
                stripControl.UpdateColors();
                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = canMoveSelectionToPlaybackPhrase;
                if (wasPlaybackOn) mProjectView.TransportBar.PlayOrResume ();
                }
            }

        public void CreatePhraseBlocksForFillingContentView ( Strip stripControl)
            {
            Block lastBlock = stripControl.LastBlock;
            if (lastBlock != null)
                {
                for (int i = lastBlock.Node.Index + 1;
                    i < stripControl.Node.PhraseChildCount;
                    i++)
                    {
                    if (stripControl.IsContentViewFilledWithBlocks)
                        {
                        Console.WriteLine ( " content view fill while fill up " + i );
                        break;
                        }
                    stripControl.AddBlockForNode ( stripControl.Node.PhraseChild ( i ) );
                    }
                UpdateSize ();
                stripControl.UpdateColors ();
                }
            }

        //@singleSection
        private EmptyNode RemoveAllblocksInStripIfRequired ( Strip stripControl, ObiNode node  , bool isScrollDown)
            {
            //int phraseBlocksLotInterval = PhraseCountInLot ( stripControl,true ) ;
            //if (stripControl.Node.PhraseChildCount <= 300) phraseBlocksLotInterval = stripControl.Node.PhraseChildCount;
            return RemoveAllblocksInStripIfRequired ( stripControl, node, PhraseCountInLot ( stripControl, isScrollDown));
            }


        //@singleSection
        private EmptyNode RemoveAllblocksInStripIfRequired ( Strip stripControl, ObiNode node, int phraseBlocksLotInterval )
            {
            Block firstBlock = stripControl.FirstBlock;
            Block lastBlock = stripControl.LastBlock;
            EmptyNode startNode = null;
            if (firstBlock != null && lastBlock != null)
                {


                //check if 
                // if difference between currently selected node and target node is more than phrase block lot interval defined above
                EmptyNode currentlySelectedNode = null;
                if (mProjectView.Selection != null)
                    {
                    currentlySelectedNode = mProjectView.Selection is StripIndexSelection ? (EmptyNode)((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection :
                        mProjectView.Selection.Node is EmptyNode ? (EmptyNode)mProjectView.Selection.Node : null;
                    }
Console.WriteLine ("offset difference is : " + Math.Abs ( node.Index - firstBlock.Node.Index)  ) ;
                if (((Math.Abs ( node.Index - firstBlock.Node.Index ) >= phraseBlocksLotInterval)
                                       || node.Index < firstBlock.Node.Index) )
                    //&&
                    //stripControl.FindBlock ( (EmptyNode)node ) == null)
                    {
                    int startNodeIndex = 0;
                    // see if last block and target nodes lie on either side of 250 threshold
                    if (firstBlock.Node.Index > node.Index)
                        {
                        startNodeIndex = Convert.ToInt32 ( node.Index / phraseBlocksLotInterval) * phraseBlocksLotInterval;
                        startNode = stripControl.Node.PhraseChild ( startNodeIndex );
                        Console.WriteLine ( "required node less than first block : " + startNodeIndex );
                        }
                    else if (node.Index - firstBlock.Node.Index >= phraseBlocksLotInterval)
                        {

                        int thresholdAboveLastNode = 0;
                        for (int i = 1; thresholdAboveLastNode <= firstBlock.Node.Index; ++i)
                            {
                            thresholdAboveLastNode = i * phraseBlocksLotInterval;
                            }
                        Console.WriteLine ( "Threshold index " + thresholdAboveLastNode );

                        startNode = stripControl.Node.PhraseChild ( thresholdAboveLastNode );
                        startNodeIndex = thresholdAboveLastNode;

                        }

                    System.Media.SystemSounds.Asterisk.Play ();
                    stripControl.RemoveAllBlocks ( false );
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, stripControl.BlocksLayoutTopPosition * -1 );
                    Console.WriteLine ( "Remove aall executed " );
                    }
                else
                    {
                    Console.WriteLine ( "Remove aall skipped" );
                    }
                }
            return startNode;
            }


//@ssingleSection :  small increment up or scroll
        public bool ScrollUp_SmallIncrement ()
            {
            int scrollIncrement = Convert.ToInt32 ( mHScrollBar.Location.Y / 5 ) * -1;
            ScrollMStripsPanel ( scrollIncrement);
            return true;
            }

        //@ssingleSection :  small increment down or scroll
        public bool ScrollDown_SmallIncrement ()
            {
            int scrollIncrement = Convert.ToInt32 ( mHScrollBar.Location.Y / 5 );
            ScrollMStripsPanel ( scrollIncrement);
            return true;
            }

        //@ssingleSection :  large increment up or scroll
        public bool ScrollUp_LargeIncrement ()
            {
            ScrollMStripsPanel ( mHScrollBar.Location.Y * -1 );
            return true;
            }

        //@ssingleSection :  large increment down for scroll
        public bool ScrollDown_LargeIncrement ()
            {
            ScrollMStripsPanel ( mHScrollBar.Location.Y);
            return true;
            }


        //@singleSection : base function for strips panel scroll
        public void ScrollMStripsPanel (int interval)
            {
            Strip currentlyActiveStrip = ActiveStrip;

            if (currentlyActiveStrip != null)
                {
                Block firstBlock = currentlyActiveStrip.FirstBlock ;
                Block lastBlock = currentlyActiveStrip.LastBlock ;
                if( firstBlock != null && lastBlock != null )
                    {
                    mProjectView.ObiForm.Cursor = Cursors.WaitCursor;
                    m_IsScrollActive = true;

                    int contentViewVisibleHeight = mHScrollBar.Location.Y;

                    if (interval > 0)
                        {
                        // check if the section is too small and the last block is less than mid of strips panel
                                if ( firstBlock.Node.Index == 0 && lastBlock.Node.Index == currentlyActiveStrip.Node.PhraseChildCount - 1
                                    && lastBlock.Location.Y -firstBlock.Location.Y + currentlyActiveStrip.BlocksLayoutTopPosition
                                    < contentViewVisibleHeight )
                                    {
                                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 0 );
                                    mProjectView.ObiForm.Cursor = Cursors.Default;
                                    m_IsScrollActive = false;
                                    return;
                                    }

                                int indexIncrement_PhraseLot = PhraseCountInLot ( currentlyActiveStrip, true ) -1 ;
                                int nextThresholdIndex = firstBlock.Node.Index + indexIncrement_PhraseLot;
                        bool setStripsPanelToInitialPosition = false;

                        Console.WriteLine ( "strips panel space " + (mStripsPanel.Height + mStripsPanel.Location.Y) );
                        if (nextThresholdIndex >= currentlyActiveStrip.Node.PhraseChildCount)
                            {
                            nextThresholdIndex = currentlyActiveStrip.Node.PhraseChildCount - 1;
                            
                            }
                        else if (nextThresholdIndex <= lastBlock.Node.Index
                            && currentlyActiveStrip.IsContentViewFilledWithBlocks 
                            && lastBlock.Node.Index < currentlyActiveStrip.Node.PhraseChildCount - 1)
                            //&& mStripsPanel.Height + mStripsPanel.Location.Y <= contentViewVisibleHeight + 1)
                            {

                            nextThresholdIndex = nextThresholdIndex + indexIncrement_PhraseLot;
                            if (nextThresholdIndex >= currentlyActiveStrip.Node.PhraseChildCount) nextThresholdIndex = currentlyActiveStrip.Node.PhraseChildCount - 1;
                            setStripsPanelToInitialPosition = true;
                            }
                        Console.WriteLine ( "threshold index : " + nextThresholdIndex );
                        // create blocks for additional interval
                        CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                            currentlyActiveStrip.Node.PhraseChild ( nextThresholdIndex ),
                            false,
                           contentViewVisibleHeight + interval );
                        
                        if (!setStripsPanelToInitialPosition)
                            {
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                mStripsPanel.Location.Y - interval );

                            CreatePhraseBlocksForFillingContentView ( currentlyActiveStrip );

                            if (Math.Abs ( mStripsPanel.Location.Y ) > mStripsPanel.Height - contentViewVisibleHeight)
                                {
                                
                                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                        (mStripsPanel.Height - (contentViewVisibleHeight / 2)) * -1 );
                                    
                                }
                            }
                        Console.WriteLine ( "Strips panel location after scroll " + mStripsPanel.Location );
                        }
                    else if ( interval < 0 ) // move strips panel down
                        {
                        if (mStripsPanel.Location.Y  > interval)
                            {//2
                            //MessageBox.Show ( "inside " );
                            if (mStripsPanel.Location.Y >= currentlyActiveStrip.BlocksLayoutTopPosition * -1)
                                {//3
                                Console.WriteLine ( "Scroll while creating previous phrases " );
                                if (firstBlock.Node.Index > 0)
                                    {//4
                                    int prevThreshold = firstBlock.Node.Index - 1;
                                    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch ();
                                    stopWatch.Start ();

                                    int requiredPhraseCount =  currentlyActiveStrip.GetPhraseCountForContentViewVisibleHeight (mHScrollBar.Location.Y, verticleScrollPane1.Location.X, 
                                        currentlyActiveStrip.Node.PhraseChild ( prevThreshold ), true );

                                    //currentlyActiveStrip.RemoveAllBlocks ( false );
                                    //currentlyActiveStrip.AddsRangeOfBlocks ( currentlyActiveStrip.Node.PhraseChild ( prevThreshold - requiredPhraseCount ),
                                        //currentlyActiveStrip.Node.PhraseChild ( prevThreshold ) );
                                    //UpdateSize ();
                                    //currentlyActiveStrip.UpdateColors ();
                                    CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                            currentlyActiveStrip.Node.PhraseChild ( prevThreshold ),
                            false );

                                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                        (mStripsPanel.Height - contentViewVisibleHeight) * -1 );
                                    stopWatch.Stop ();
                                    Console.WriteLine ( "stop watch " + stopWatch.Elapsed.TotalMilliseconds );
                                    Console.WriteLine ( "previous blocks created " );
                                    }//-4
                                else
                                    {
                                    //strips panel contains initial phrases and is near top so take it to top to show label of strip
                                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 0 );
                                    }
                                }//-3
                            else
                                {//3
                                if (firstBlock.Node.Index == 0)
                                    {

                                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 0 );
                                    }
                                else
                                    {
                                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, currentlyActiveStrip.BlocksLayoutTopPosition * -1 );
                                    //remove blocks below the content view
                                    currentlyActiveStrip.RemoveAllFollowingBlocks ( true, true) ;
                                    }
                                Console.WriteLine ( "adjusted upto label " );
                                }//-3
                            }//-2
                        else // just move strips panel down
                            {//2
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                        mStripsPanel.Location.Y - interval );//interval is negetive
                            Console.WriteLine ( "just moved strips panel down " );
                            }//-2
                        Console.WriteLine ( "Strips panel location while moving up " + mStripsPanel.Location.Y );
                        }

                    m_IsScrollActive = false;
                    mProjectView.ObiForm.Cursor = Cursors.Default;
                    }
                verticleScrollPane1.TrackBarValueInPercentage = EstimateScrollPercentage ( currentlyActiveStrip );
                }// check ends for currently active strip

            }

        private int PhraseCountInLot ( Strip currentlyActiveStrip, bool isScrollDown )
            {
            int phraseLotSize = 250;

            if (ZoomFactor < 1.0f)
                {
                phraseLotSize = 300;
                }
            else if (ZoomFactor >= 1.0f && ZoomFactor <= 1.5)
                {
                phraseLotSize = 250;
                }
            else if (ZoomFactor > 1.5)
                {
                phraseLotSize = 126;
                }
            if (!isScrollDown)
                {
                phraseLotSize = (phraseLotSize / 2) ;
                }

            
                if (currentlyActiveStrip.Node.PhraseChildCount <= (phraseLotSize * 6 / 5))
                    {
                phraseLotSize = currentlyActiveStrip.Node.PhraseChildCount;
                }
            Console.WriteLine ("Calculated phrase lot size " + phraseLotSize ) ;
            return phraseLotSize;
            }

        //@singleSection
        private int EstimateScrollPercentage (Strip currentlyActiveStrip)
            {
            int startY = Math.Abs( mStripsPanel.Location.Y ) ;
            int endY = startY + mHScrollBar.Location.Y;

            List<int> boundaryPhraseIndexes = currentlyActiveStrip.GetBoundaryPhrasesIndexForVisiblePhrases ( startY, endY );
            if (boundaryPhraseIndexes == null || boundaryPhraseIndexes.Count == 0) return 0 ;

            int percentageValue = 0;
            if (boundaryPhraseIndexes[0] == 0)
                {
                percentageValue =  0;
                }
            else if (boundaryPhraseIndexes.Count == 2 && currentlyActiveStrip.Node.PhraseChildCount > 0
                && boundaryPhraseIndexes[1] == currentlyActiveStrip.Node.PhraseChildCount - 1)
                {
                percentageValue = 100;
                }
            else if ( boundaryPhraseIndexes.Count == 2  && currentlyActiveStrip.Node.PhraseChildCount > 0 )
                {
                int midIndexVisible = Convert.ToInt32 ( (boundaryPhraseIndexes[0] + boundaryPhraseIndexes[1]) / 2 );
                percentageValue = Convert.ToInt32 ( (midIndexVisible * 100) / currentlyActiveStrip.Node.PhraseChildCount );
                }
            Console.WriteLine ( "estimated percentage of scroll " + percentageValue );
            return percentageValue ;
            }

        //@singleSection : Scroll to top
        public bool ScrollStripsPanel_Top ()
            {
            Strip currentlyActiveStrip = ActiveStrip;

            if (currentlyActiveStrip != null && currentlyActiveStrip.Node.PhraseChildCount > 0)
                {
                mProjectView.ObiForm.Cursor = Cursors.WaitCursor;
                m_IsScrollActive = true ;

                CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                                currentlyActiveStrip.Node.PhraseChild ( 0 ),
                                false );

                mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 0 );

                CreatePhraseBlocksForFillingContentView ( currentlyActiveStrip );
                verticleScrollPane1.TrackBarValueInPercentage = 0;

                m_IsScrollActive = false;
                mProjectView.ObiForm.Cursor = Cursors.Default;
                return true;
                }
            return false;
            }

        //@singleSection : Scroll to bottom
        public bool ScrollStripsPanel_Bottom ()
            {
            Strip currentlyActiveStrip = ActiveStrip;

            if (currentlyActiveStrip != null && currentlyActiveStrip.Node.PhraseChildCount > 0 )
                {
                mProjectView.ObiForm.Cursor = Cursors.WaitCursor;
                m_IsScrollActive = true;
                CreateBlocksTillEndInStrip ( currentlyActiveStrip );
                mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 
                    (mStripsPanel.Height - (mHScrollBar.Location.Y - 10 )) * -1  );
                verticleScrollPane1.TrackBarValueInPercentage = 100;
                m_IsScrollActive = false;
                mProjectView.ObiForm.Cursor = Cursors.Default;
                return true;
                
                }
            return false;
            }

        //@singleSection
        private bool m_IsScrollActive = false;
        public bool IsScrollActive { get { return m_IsScrollActive; } }
            
            
        //@singleSection
        public void mStripsPanel_LocationChanged ( object sender, EventArgs e )
            {
            if (mStripsPanel.Location.Y >= 0)
                {
                Strip currentlyActiveStrip = ActiveStrip;

                if (currentlyActiveStrip != null )
                    {
                    if (currentlyActiveStrip.FirstBlock == null ||
                        (currentlyActiveStrip.FirstBlock != null && currentlyActiveStrip.FirstBlock.Node.Index == 0))
                        {
                        verticleScrollPane1.CanScrollUp = false;
                        }
                    else if ( Math.Abs ( mStripsPanel.Location.Y ) > currentlyActiveStrip.BlocksLayoutTopPosition 
                        && currentlyActiveStrip.FirstBlock != null && currentlyActiveStrip.FirstBlock.Node.Index > 0)
                        {
                        // set position of strip panel to hide label -- for precaution
                        mStripsPanel.Location = new Point ( mStripsPanel.Location.X, currentlyActiveStrip.BlocksLayoutTopPosition * -1 );
                        Console.WriteLine ( "precautionary setting of strips label for threshold index " );
                        }
                    }
                }
            else if (mStripsPanel.Location.Y < 0)
                {
                verticleScrollPane1.CanScrollUp = true;
                }

            if (mStripsPanel.Location.Y + mStripsPanel.Height <= mHScrollBar.Location.Y)
                {
                Strip currentlyActiveStrip = ActiveStrip;
                if (currentlyActiveStrip != null)
                    {
                    if (currentlyActiveStrip.LastBlock == null
                        || (currentlyActiveStrip.LastBlock != null && currentlyActiveStrip.LastBlock.Node.Index == currentlyActiveStrip.Node.PhraseChildCount - 1))
                        {
                        verticleScrollPane1.CanScrollDown = false;
                        }
                    }
                }
            else
                {
                verticleScrollPane1.CanScrollDown = true;
                }
            }

        public void RecreateContentsWhileInitializingRecording ( EmptyNode recordingResumePhrase)
            {
            
            if ( recordingResumePhrase != null
                || (mProjectView.Selection != null && mProjectView.Selection.Node is SectionNode) )
                {
                SectionNode section = recordingResumePhrase != null ? recordingResumePhrase.ParentAs<SectionNode> ():
                    (SectionNode)mProjectView.Selection.Node;

                Strip stripControl = FindStrip ( section );

                if (stripControl == null) return;
                if (recordingResumePhrase != null && stripControl.FindBlock ( recordingResumePhrase ) != null) return;

                Block firstBlock = stripControl.FirstBlock;
                Block lastBlock = stripControl.LastBlock;

                if (firstBlock != null && lastBlock != null
                    && ( lastBlock.Node.Index < stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 ).Index    
                    ||    ( recordingResumePhrase != null &&  lastBlock.Node.Index < recordingResumePhrase.Index ) ) )
                    {
                    EmptyNode lastVisiblePhraseIntended = recordingResumePhrase != null ? recordingResumePhrase:
                        stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 );

                    if (lastVisiblePhraseIntended.Index < lastBlock.Node.Index + 10)
                        {
                        CreateBlocksTillNodeInStrip ( stripControl, lastVisiblePhraseIntended, false );
                        if (recordingResumePhrase != null) CreatePhraseBlocksForFillingContentView ( stripControl );
                        return;
                        }
                    System.Media.SystemSounds.Asterisk.Play ();
                    stripControl.RemoveAllBlocks ( false );
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, stripControl.BlocksLayoutTopPosition * -1 );

                    // now create some blocks before recording phrase 
                    if (recordingResumePhrase != null)
                        {
                        stripControl.AddBlockForNode ( recordingResumePhrase );
                        CreatePhraseBlocksForFillingContentView ( stripControl );
                        return;
                        }

                    if (stripControl.Node.PhraseChildCount > 2)
                        {
                        for (int i = stripControl.Node.PhraseChildCount - 3; i < stripControl.Node.PhraseChildCount; ++i)
                            {
                            stripControl.AddBlockForNode ( section.PhraseChild ( i ) );
                            }

                        }
                    }
                }
                }

        // @phraseLimit
        /// <summary>
        /// Make phrase blocks visible for strip  passed as parameter
        /// </summary>
        /// <param name="stripControl"></param>
        /// <returns></returns>
        private bool CreateBlocksInStrip ( Strip stripControl )
            {
            return CreateLimitedBlocksInStrip ( stripControl, null );
            if (stripControl != null && 
stripControl.Node.PhraseChildCount > 0)
                {
                // pause playback if it is active.
                if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Pause ();

                ChangeVisibilityProcessState ( true );
                // make blocks visible w.r.t. over limit, remove blocks only if new blocks take count even above over limit
                if (!m_CreatingGUIForNewPresentation &&
                    (VisibleBlocksCount + stripControl.Node.PhraseChildCount) > (mProjectView.MaxVisibleBlocksCount + mProjectView.MaxOverLimitForPhraseVisibility))
                    MakeOldStripsBlocksInvisible ( stripControl.Node.PhraseChildCount, true, 0 );

                int indexAddition = -1;
                bool IsAllBlocksCreated = true;

                try
                    {
                    // if any block of target invisible strip is visible, first make it invisible then make blocks for whole strip visible
                    if (!m_CreatingGUIForNewPresentation)
                        RemoveBlocksInStrip ( stripControl );

                    // create blocks for whole strip
                    if (stripControl.Node.PhraseChildCount <= mProjectView.MaxVisibleBlocksCount)
                        {
                        for (int i = 0; i < stripControl.Node.PhraseChildCount; ++i)
                            stripControl.AddBlockForNode ( stripControl.Node.PhraseChild ( i ) );
                        }
                    else
                        {
                        for (int i = 0; i < mProjectView.MaxVisibleBlocksCount; ++i)
                            stripControl.AddBlockForNode ( stripControl.Node.PhraseChild ( i ) );

                        IsAllBlocksCreated = false;
                        }

                    stripControl.SetAccessibleName ();
                    indexAddition = AddStripToVisibleStripsList ( stripControl );
                    } // try ends
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }

                if (!m_CreatingGUIForNewPresentation && VisibleBlocksCount > mProjectView.MaxVisibleBlocksCount && IsAllBlocksCreated)
                    MakeOldStripsBlocksInvisible ( indexAddition );

                if (!m_CreatingGUIForNewPresentation) UpdateSize ();

                if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();

                if (!IsAllBlocksCreated) MessageBox.Show ( string.Format ( Localizer.Message ( "ContentHidden_SectionHasOverlimitPhrases" ), stripControl.Node.Label, mProjectView.MaxVisibleBlocksCount ), Localizer.Message ( "Caption_Warning" ) );

                ChangeVisibilityProcessState ( false );
                if (!m_CreatingGUIForNewPresentation && mProjectView.ObiForm.Settings.AudioClues && stripControl.Node.PhraseChildCount > 0)
                    PlayShowBlocksCompletedSound ();
                return true;
                }
            ChangeVisibilityProcessState ( false );
            return false;
            }

        // @phraseLimit
        /// <summary>
        /// Add a strip to list of visible strips list according to order of their arrangement in stripPanel
        /// </summary>
        /// <param name="newStrip"></param>
        /// <returns></returns>
        private int AddStripToVisibleStripsList ( Strip newStrip )
            {
            if (m_VisibleStripsList.Contains ( newStrip )) return m_VisibleStripsList.IndexOf ( newStrip );

            if (m_VisibleStripsList.Count > 0)
                {
                if (newStrip.Node.Position < m_VisibleStripsList[0].Node.Position)
                    {
                    m_VisibleStripsList.Insert ( 0, newStrip );
                    return 0;
                    }

                if (newStrip.Node.Position > m_VisibleStripsList[m_VisibleStripsList.Count - 1].Node.Position)
                    {
                    m_VisibleStripsList.Add ( newStrip );
                    return m_VisibleStripsList.Count - 1;
                    }

                for (int i = 0; i < m_VisibleStripsList.Count - 1; i++)
                    {
                    if (newStrip.Node.Position > m_VisibleStripsList[i].Node.Position && newStrip.Node.Position < m_VisibleStripsList[i + 1].Node.Position)
                        {
                        m_VisibleStripsList.Insert ( i + 1, newStrip );
                        return i + 1;
                        }
                    }
                }
            m_VisibleStripsList.Add ( newStrip );
            return m_VisibleStripsList.Count - 1;
            }

        // @phraseLimit
        /// <summary>
        /// Indicate change in state of  blocks visibility process
        /// </summary>
        /// <param name="active"></param>
        private void ChangeVisibilityProcessState ( bool active )
            {
            m_IsBlocksVisibilityProcessActive = active;
            if (!m_CreatingGUIForNewPresentation) mProjectView.ChangeVisibilityProcessState ( active );
            }

        // @phraseLimit
        /// <summary>
        /// Make blocks of strips invisible. For making invisible, the function selects farthest strip w.r.t. strip index passed as parameter
        /// </summary>
        /// <param name="countRequired"></param>
        /// <param name="tillOverLimit"></param>
        /// <param name="newStripIndex"></param>
        private void MakeOldStripsBlocksInvisible ( int countRequired, bool tillOverLimit, int newStripIndex )
            {
            if (m_VisibleStripsList.Count == 0)
                return;

            //m_BlocksVisibilityOperationMutex.WaitOne ();
            int maxVisiblePhraseCountConsidered;

            if (tillOverLimit == false) // consider only normal visibility limit and no over limit. this is normal operation and can be used through threads
                maxVisiblePhraseCountConsidered = mProjectView.MaxVisibleBlocksCount;
            else// overlimit is true, operate in overlimit band, called when we visible phrases are more than even over limit. generally called imidiately 
                maxVisiblePhraseCountConsidered = mProjectView.MaxVisibleBlocksCount + mProjectView.MaxOverLimitForPhraseVisibility;

            Strip newStrip = m_VisibleStripsList[newStripIndex];
            // first clear blocks in partially visible strips
            for (int i = 0; i < m_VisibleStripsList.Count; i++)
                {
                if (maxVisiblePhraseCountConsidered - VisibleBlocksCount < countRequired)
                    {
                    try
                        {
                        int removeIndex = PartiallyVisibleStripIndexToMakeInvisible ( m_VisibleStripsList.IndexOf ( newStrip ) );
                        if (removeIndex != -1)
                            {
                            int blocksRemoved = RemoveBlocksInStrip ( m_VisibleStripsList[removeIndex] );
                            countRequired = blocksRemoved > 0 ? countRequired - blocksRemoved : countRequired;
                            }
                        else break;
                        }
                    catch (System.Exception ex)
                        {
                        MessageBox.Show ( ex.ToString () );
                        }
                    }
                }


            // after removing all blocks in partially visible strips, start removing blocks fully visible strips
            for (int i = 0; i < m_VisibleStripsList.Count; i++)
                {
                if (maxVisiblePhraseCountConsidered - VisibleBlocksCount < countRequired)
                    {
                    try
                        {
                        int blocksRemoved = RemoveBlocksInStrip ( m_VisibleStripsList[VisibleStripIndexToMakeInvisible ( m_VisibleStripsList.IndexOf ( newStrip ) )] );
                        countRequired = blocksRemoved > 0 ? countRequired - blocksRemoved : countRequired;
                        }
                    catch (System.Exception ex)
                        {
                        MessageBox.Show ( ex.ToString () );
                        }
                    }
                else
                    {
                    //m_BlocksVisibilityOperationMutex.ReleaseMutex ();
                    return;
                    }
                }
            //m_BlocksVisibilityOperationMutex.ReleaseMutex ();
            }

        // @phraseLimit
        /// <summary>
        /// Make blocks of strips invisible. For making invisible, the function selects farthest strip w.r.t. strip index passed as parameter
        /// </summary>
        /// <param name="newStripIndex"></param>
        private void MakeOldStripsBlocksInvisible ( int newStripIndex )
            {
            int countRequired = VisibleBlocksCount - mProjectView.MaxVisibleBlocksCount;
            if (countRequired > 0) MakeOldStripsBlocksInvisible ( countRequired, false, newStripIndex );
            }


        // @phraseLimit
        /// <summary>
        /// Make blocks invisible for strips, starting from the selected strip. Useful in removing partially visible blocks from selected strip
        /// </summary>
        /// <param name="removeFromSelected"></param>
        public void MakeOldStripsBlocksInvisible ( bool removeFromSelected )
            {
            ChangeVisibilityProcessState ( true );
            int countRequired = VisibleBlocksCount - mProjectView.MaxVisibleBlocksCount;

            if (removeFromSelected && countRequired > 0) RemoveBlocksFromSelectedPartiallyVisibleStrip ( countRequired );

            countRequired = VisibleBlocksCount - mProjectView.MaxVisibleBlocksCount;
            if (countRequired > 0)
                MakeOldStripsBlocksInvisible ( countRequired, false, 0 );
            ChangeVisibilityProcessState ( false );
            }


        // @phraseLimit
        /// <summary>
        /// Make blocks invisible for strips, starting from the selected strip. Useful in removing partially visible blocks from selected strip
        /// </summary>
        /// <param name="countRequired"></param>
        private void RemoveBlocksFromSelectedPartiallyVisibleStrip ( int countRequired )
            {
            if (mProjectView.GetSelectedPhraseSection != null && countRequired > 0 && m_VisibleStripsList.Count > 0)
                {
                try
                    {
                    int stripIndex = GetStripIndexInVisibleStripList ( (SectionNode)mProjectView.GetSelectedPhraseSection );
                    if (stripIndex > 0 && !m_VisibleStripsList[stripIndex].IsBlocksVisible)
                        {
                        countRequired = m_VisibleStripsList[stripIndex].Node.PhraseChildCount;
                        RemoveBlocksInStrip ( m_VisibleStripsList[stripIndex] );
                        }
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                }
            }

        // @phraseLimit
        /// <summary>
        /// returns index of section node  passed as parameter in visible strips list, returns -1 if the parameter node do not lie in visible strips list
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int GetStripIndexInVisibleStripList ( SectionNode node )
            {
            if (node != null && node is SectionNode)
                {

                for (int i = 0; i < m_VisibleStripsList.Count; i++)
                    if (m_VisibleStripsList[i].Node == node) return i;

                }
            return -1;
            }

        /// <summary>
        /// Make all phrase blocks invisible in  strip of parameter  section node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int RemoveBlocksInStrip ( SectionNode node )
            {
            if (node != null)
                {
                Strip s = FindStrip ( node );

                if (s != null)
                    {
                    try
                        {
                        int retVal = RemoveBlocksInStrip ( s );
                        return retVal;
                        }
                    catch (System.Exception ex)
                        {
                        MessageBox.Show ( ex.ToString () );
                        return 0;
                        }

                    }
                }
            return 0;
            }

        // @phraseLimit
        /// <summary>
        /// Make all phrase blocks invisible in  parameter strip
        /// </summary>
        /// <param name="stripControl"></param>
        /// <returns></returns>
        private int RemoveBlocksInStrip ( Strip stripControl )
            {
            int blocksRemoved = 0;
            if (stripControl != null && stripControl.Node.PhraseChildCount > 0)
                {
                blocksRemoved = stripControl.RemoveAllBlocks ( true );
                stripControl.SetAccessibleName ();
                if (!stripControl.IsBlocksVisible) m_VisibleStripsList.Remove ( stripControl );
                }
            return blocksRemoved;
            }


        // @phraseLimit
        /// <summary>
        /// Remove only required number of phrase blocks from strip
        /// </summary>
        /// <param name="stripControl"></param>
        /// <param name="countRequired"></param>
        /// <returns></returns>
        private int RemoveBlocksInStrip ( Strip stripControl, int countRequired )
            {
            if (stripControl != null && stripControl.Node.PhraseChildCount > 0)
                {
                int upperBound = countRequired < 15 ? countRequired * 2 : countRequired;
                if (countRequired < 0 && countRequired > stripControl.Node.PhraseChildCount)
                    upperBound = stripControl.Node.PhraseChildCount;
                for (int i = 0; i < upperBound; i++)
                    {
                    if (i == stripControl.Node.PhraseChildCount - 1)
                        stripControl.RemoveBlock ( stripControl.Node.PhraseChild ( i ) );
                    else
                        stripControl.RemoveBlock ( stripControl.Node.PhraseChild ( i ), false );

                    if (countRequired < 0 && countRequired > stripControl.Node.PhraseChildCount)
                        upperBound = stripControl.Node.PhraseChildCount;
                    }

                stripControl.SetAccessibleName ();
                if (!stripControl.IsBlocksVisible) m_VisibleStripsList.Remove ( stripControl );
                return countRequired;
                }
            return 0;
            }

        // @phraseLimit
        /// <summary>
        /// find the index of strip in visible strips list for making its blocks invisible, currently it searches the farthest index with respect to parameter index
        /// </summary>
        /// <param name="newSectionIndex"></param>
        /// <returns></returns>
        private int VisibleStripIndexToMakeInvisible ( int newSectionIndex )
            {
            if (m_VisibleStripsList.Count == 0) return -1;

            int midIndex = m_VisibleStripsList.Count / 2;
            bool startFromZeroIndex = (newSectionIndex > midIndex);

            int upperPositionDiff = Math.Abs ( m_VisibleStripsList[newSectionIndex].Node.Position - m_VisibleStripsList[0].Node.Position );
            int lowerPositionDiff = Math.Abs ( m_VisibleStripsList[m_VisibleStripsList.Count - 1].Node.Position - m_VisibleStripsList[newSectionIndex].Node.Position );

            if (upperPositionDiff >= lowerPositionDiff) startFromZeroIndex = true;
            else startFromZeroIndex = false;

            //if (newSectionIndex > m_VisibleStripsList.Count / 2)
            if (startFromZeroIndex)
                {
                for (int i = 0; i < m_VisibleStripsList.Count; i++)
                    {
                    if (mProjectView.GetSelectedPhraseSection != null && mProjectView.GetSelectedPhraseSection != m_VisibleStripsList[i].Node)
                        return i;

                    }
                }
            else
                {
                for (int i = m_VisibleStripsList.Count - 1; i >= 0; i--)
                    {
                    if (mProjectView.GetSelectedPhraseSection != null && mProjectView.GetSelectedPhraseSection != m_VisibleStripsList[i].Node)
                        return i;

                    }
                }

            return 0;
            }

        // @phraseLimit
        /// <summary>
        /// Find the intex of partially visible strip in visible strip list for making its blocks invisible, currently it searches farthest index w.r.t. parameter index. 
        /// Returns -1 if no partially visible strip is found.
        /// </summary>
        /// <param name="newSectionIndex"></param>
        /// <returns></returns>
        private int PartiallyVisibleStripIndexToMakeInvisible ( int newSectionIndex )
            {
            if (m_VisibleStripsList.Count == 0) return -1;

            if (newSectionIndex > m_VisibleStripsList.Count / 2)
                {
                for (int i = 0; i < m_VisibleStripsList.Count; i++)
                    {
                    if (mProjectView.GetSelectedPhraseSection != null && mProjectView.GetSelectedPhraseSection != m_VisibleStripsList[i].Node
                    && !m_VisibleStripsList[i].IsBlocksVisible)
                        return i;

                    }
                }
            else
                {
                for (int i = m_VisibleStripsList.Count - 1; i >= 0; i--)
                    {
                    if (mProjectView.GetSelectedPhraseSection != null && mProjectView.GetSelectedPhraseSection != m_VisibleStripsList[i].Node
                        && !m_VisibleStripsList[i].IsBlocksVisible)
                        return i;

                    }
                }

            return -1;
            }


        // @phraseLimit
        /// <summary>
        /// Returns a string indicating that strip of parameter node has invisible blocks , else if blocks are visible or node is not section node, returns empty string
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string InvisibleStripString ( ObiNode node )
            {
            if (IsSectionPhrasesVisible ( node ))
                return "";
            else if (mProjectView.GetSelectedPhraseSection != null)
                return Localizer.Message ( "ContentsHidden_StatusMessage" );
            else
                return "";
            }

        // @phraseLimit
        /// <summary>
        /// Returns true if phrase blocks of parameter section node are visible else return false
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsSectionPhrasesVisible ( ObiNode node )
            {
            if (node != null && node is SectionNode)
                {
                if (((SectionNode)node).PhraseChildCount == 0) return true;

                if (m_VisibleStripsList.Count > 0)
                    {
                    for (int i = 0; i < m_VisibleStripsList.Count; i++)
                        {
                        if (m_VisibleStripsList[i].Node == node)
                            {
                            if (m_VisibleStripsList[i].IsBlocksVisible) return true;
                            else return false;
                            }
                        }
                    }
                }
            return false;
            }
        // @phraseLimit
        /// <summary>
        /// if audioclue is enabled,  plays a sound when process of creating blocks is complete.
        /// </summary>
        private void PlayShowBlocksCompletedSound ()
            {
            string FilePath = System.IO.Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "ShowBlocksCompleted.wav" );
            if (System.IO.File.Exists ( FilePath ) && mProjectView.ObiForm.Settings.AudioClues)
                {
                System.Media.SoundPlayer showBlocksPlayer = new System.Media.SoundPlayer ( FilePath );
                showBlocksPlayer.Play ();
                }
            }


        // @phraseLimit
        public Clipboard clipboard { get { return mProjectView.Clipboard; } }



        // Recursive function to go through all the controls in-order and add the ISearchable ones to the list
        private void AddToSearchables ( Control c, List<ISearchable> searchables )
            {
            if (c is ISearchable) searchables.Add ( (ISearchable)c );
            foreach (Control c_ in c.Controls) AddToSearchables ( c_, searchables );
            }

        // Deselect everything when clicking the panel
        private void ContentView_Click ( object sender, EventArgs e ) { mProjectView.Selection = null; }

        // Create a command (possibly composite) to delete a strip for the given section node.
        private ICommand DeleteStripCommand ( SectionNode section )
            {
            Commands.Node.Delete delete = new Commands.Node.Delete ( mProjectView, section, Localizer.Message ( "delete_section_shallow" ) );
            if (section.SectionChildCount > 0)
                {
                CompositeCommand command = mProjectView.Presentation.getCommandFactory ().createCompositeCommand ();
                command.setShortDescription ( delete.getShortDescription () );
                for (int i = 0; i < section.SectionChildCount; ++i)
                    {
                    command.append ( new Commands.TOC.MoveSectionOut ( mProjectView, section.SectionChild ( i ) ) );
                    }
                command.append ( delete );
                return command;
                }
            else
                {
                return delete;
                }
            }

        // Find the block for the given node; return null if not found (be careful!)
        private Block FindBlock ( EmptyNode node )
            {
            Strip strip = FindStrip ( node.ParentAs<SectionNode> () );
            return strip == null ? null : strip.FindBlock ( node );
            }

        // Find the strip cursor for a strip index selection
        private StripCursor FindStripCursor ( StripIndexSelection selection )
            {
            Strip strip = FindStrip ( selection.Node as SectionNode );
            return strip == null ? null : strip.FindStripCursor ( selection.Index );
            }

        // Find the strip for the given section node; return null if not found (be careful!)
        private Strip FindStrip ( SectionNode section )
            {
            foreach (Control c in mStripsPanel.Controls)
                {
                if (c is Strip && ((Strip)c).Node == section) return (Strip)c;
                }
            return null;
            }

        // Find the selectable item for this selection object (block, strip or strip cursor.)
        private ISelectableInContentView FindSelectable ( NodeSelection selection )
            {
            return
                selection == null ? null :
                selection is StripIndexSelection ? (ISelectableInContentView)FindStripCursor ( (StripIndexSelection)selection ) :
                selection.Node is SectionNode ? (ISelectableInContentView)FindStrip ( (SectionNode)selection.Node ) :
                selection.Node is EmptyNode ? (ISelectableInContentView)FindBlock ( (EmptyNode)selection.Node ) : null;
            }

        private bool IsAudioRangeSelected { get { return mSelection != null && mSelection is AudioSelection && ((AudioSelection)mSelection).AudioRange != null && !((AudioSelection)mSelection).AudioRange.HasCursor; } }
        private bool IsBlockSelected { get { return mSelectedItem != null && mSelectedItem is Block && mSelection.GetType () == typeof ( NodeSelection ); } }
        private bool IsBlockOrWaveformSelected { get { return mSelectedItem != null && mSelectedItem is Block; } }
        private bool IsInView ( ObiNode node ) { return node is SectionNode && FindStrip ( (SectionNode)node ) != null; }
        private bool IsStripCursorSelected { get { return mSelection != null && mSelection is StripIndexSelection; } }
        private bool IsStripSelected { get { return mSelectedItem != null && mSelectedItem is Strip && mSelection.GetType () == typeof ( NodeSelection ); } }

        // Listen to changes in the presentation (new nodes being added or removed)
        private void Presentation_changed ( object sender, urakawa.events.DataModelChangedEventArgs e )
            {
            if (e is urakawa.events.core.ChildAddedEventArgs)
                {
                TreeNodeAdded ( (urakawa.events.core.ChildAddedEventArgs)e );
                }
            else if (e is urakawa.events.core.ChildRemovedEventArgs)
                {
                TreeNodeRemoved ( (urakawa.events.core.ChildRemovedEventArgs)e );
                }
            }

        // Handle section nodes renamed from the project: change the label of the corresponding strip.
        private void Presentation_RenamedSectionNode ( object sender, NodeEventArgs<SectionNode> e )
            {
            Strip strip = FindStrip ( e.Node );
            if (strip != null)
                {
                strip.Label = e.Node.Label;
                contentViewLabel1.Name_SectionDisplayed = strip.Label;//@singleSection
                }
            }

        // Handle change of used status
        private void Presentation_UsedStatusChanged ( object sender, NodeEventArgs<ObiNode> e )
            {
            if (e.Node is SectionNode)
                {
                Strip strip = FindStrip ( (SectionNode)e.Node );
                if (strip != null) strip.UpdateColors ();
                }
            else if (e.Node is EmptyNode)
                {
                Block block = FindBlock ( (EmptyNode)e.Node );
                if (block != null) block.UpdateColors ();
                }
            }

        // Remove all strips for a section and its subsections
        private void RemoveStripsForSection_Safe ( SectionNode section )
            {
            if (InvokeRequired)
                {
                Invoke ( new RemoveControlForSectionNodeDelegate ( RemoveStripsForSection_Safe ), section );
                }
            else
                {
                SuspendLayout ();
                RemoveStripsForSection ( section );
                ResumeLayout ();
                }
            }

        private void RemoveStripsForSection ( SectionNode section )
            {
            for (int i = 0; i < section.SectionChildCount; ++i) RemoveStripsForSection ( section.SectionChild ( i ) );
            Strip strip = FindStrip ( section );
            int index = mStripsPanel.Controls.IndexOf ( strip );
            mStripsPanel.Controls.Remove ( strip );
            ReflowFromIndex ( index );
            if (m_VisibleStripsList.Contains ( strip )) m_VisibleStripsList.Remove ( strip ); // @phraseLimit

            if (clipboard == null || 
                (clipboard != null && strip != null && clipboard.Node != strip.Node)) // @phraseLimit
                {
                if ( strip != null )  strip.DestroyStripHandle ();
                strip = null;
                }
            else if (strip != null)
                {
                strip.RemoveAllBlocks ( false );
                }

            }

        // Remove the strip or block for the removed tree node
        private void TreeNodeRemoved ( urakawa.events.core.ChildRemovedEventArgs e )
            {
            if (e.RemovedChild is SectionNode)
                {
                
                    
                    RemoveStripsForSection_Safe ( (SectionNode)e.RemovedChild );
                      
                }
            else if (e.RemovedChild is EmptyNode)
                {
                // TODO in the future, the parent of a removed empty node
                // will not always be a section node!
                Strip strip = FindStrip ( (SectionNode)e.SourceTreeNode );
                if (strip != null) strip.RemoveBlock ( (EmptyNode)e.RemovedChild );
                }
            }

        // Add a new strip for a newly added section node or a new block for a newly added empty node.
        private void TreeNodeAdded ( urakawa.events.core.ChildAddedEventArgs e )
            {
            
            //@singleSection : AddStripForSection_Safe replaced by CreateStripForAddedSectionNode
            // this will remove existing strips before creating new strip in content view
            Control c = e.AddedChild is SectionNode ? (Control)CreateStripForAddedSectionNode( (SectionNode)e.AddedChild , true) :
                // TODO: in the future, the source node will not always be a section node!
                e.AddedChild is EmptyNode ? AddBlockForNodeConsideringPhraseLimit ( (Strip)FindStrip ( (SectionNode)e.SourceTreeNode ), ((EmptyNode)e.AddedChild) ) : // @phraseLimit
                null;
            UpdateNewControl ( c );
            }

        // @phraseLimit
        private Block AddBlockForNodeConsideringPhraseLimit ( Strip stripControl, EmptyNode node )
            {
            // if the node is above max phrase limit per section, do not add block and return
            if (node.Index > mProjectView.MaxVisibleBlocksCount
                || stripControl == null)//@singleSection: this null check shuld surely be replaced by strip creation code
                {
                //@singleSection : if no strip is visible in content view, make the parent strip of empty node visible 
                if (ActiveStrip == null)
                    {
                    stripControl =  CreateStripForSelectedSection ( node.ParentAs<SectionNode> (), true );
                    }
                else
                    {
                    return null;
                    }
                }
            Block lastBlock = stripControl.LastBlock;
            if (lastBlock != null )
                                {
                                int phraseLotSize = PhraseCountInLot ( stripControl, true );
                                int nextThreshold = (Convert.ToInt32 ( lastBlock.Node.Index / phraseLotSize ) + 1) * phraseLotSize;

                                if (node.Index > nextThreshold
                                    || (stripControl.IsContentViewFilledWithBlocks && node.Index >  lastBlock.Node.Index ))
                                    {
                                    //here no need for applying check for nodes before threshold as it is handled in add blok for node function in strip
                                    //Console.WriteLine ( "exiting before making block " );
                                    return null;
                                    }
                }
            // else add block
            Block b = stripControl.AddBlockForNode ( node );
            
            // change colors if added phrase is first phrase of section and recorder is active.
            if (b != null && b.Node.Index == 0 && mProjectView.TransportBar.IsRecorderActive)
                stripControl.UpdateColors ();


            int indexOfNewStrip = 0;
            // if strip is visible but not included in visible strips list, include it
            if (!m_VisibleStripsList.Contains ( stripControl ))
                indexOfNewStrip = AddStripToVisibleStripsList ( stripControl );


            int blocksCountInVisibleStrip = VisibleBlocksCount;

            // remove blocks in old strips if  blocks exceed max. blocks limit and recorder is not active
            // else remove imidiately if  if visible blocks exceed even extra limit  even if recorder is active
            if (blocksCountInVisibleStrip > mProjectView.MaxVisibleBlocksCount && !mProjectView.TransportBar.IsRecorderActive)
                {
                //MakeOldStripsBlocksInvisible ( 1, false, indexOfNewStrip );
                }
            else if (blocksCountInVisibleStrip > (mProjectView.MaxVisibleBlocksCount + mProjectView.MaxOverLimitForPhraseVisibility))
                {
                //MakeOldStripsBlocksInvisible ( 1, true, indexOfNewStrip );
                }

            
            return b;
            }

        private delegate void ControlInvokation ( Control c );

        private void UpdateNewControl ( Control c )
            {
            if (InvokeRequired)
                {
                Invoke ( new ControlInvokation ( UpdateNewControl ), c );
                }
            else if (c != null)
                {
                EnsureControlVisible ( c );
                UpdateTabIndex ( c );
                }
            }

        // Update the colors of the view and its components.
        private void UpdateColors ( ColorSettings settings )
            {
            mStripsPanel.BackColor = settings.ContentViewBackColor;
            foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).ColorSettings = settings;
            UpdateWaveforms ();
            contentViewLabel1.invertColor = SystemInformation.HighContrast;
            }

        // Update all waveforms after colors have been set
        private void UpdateWaveforms ()
            {
            ClearWaveformRenderQueue ();
            foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).UpdateWaveforms ( AudioBlock.NORMAL_PRIORITY );
            }

        #region IControlWithRenamableSelection Members

        public void SelectAndRename ( ObiNode node )
            {
            SectionNode section = node as SectionNode;
            if (section != null)
                {
                DoToNewNode ( node, delegate ()
                {
                    mProjectView.Selection = new NodeSelection ( section, this );
                    FindStrip ( section ).StartRenaming ();
                } );
                }
            }

        private delegate void DoToNewNodeDelegate ();

        // Do f() to a section node that may not yet be in the view.
        private void DoToNewNode ( ObiNode node, DoToNewNodeDelegate f )
            {
            if (IsInView ( node ))
                {
                f ();
                }
            else
                {
                EventHandler<urakawa.events.DataModelChangedEventArgs> h =
                    delegate ( object sender, urakawa.events.DataModelChangedEventArgs e ) { };
                h = delegate ( object sender, urakawa.events.DataModelChangedEventArgs e )
                {
                    if (e is urakawa.events.core.ChildAddedEventArgs &&
                        ((urakawa.events.core.ChildAddedEventArgs)e).AddedChild == node)
                        {
                        f ();
                        mProjectView.Presentation.changed -= h;
                        }
                };
                mProjectView.Presentation.changed += h;
                }
            }

        #endregion



        #region tabbing

        // Update tab index for all controls after a newly added strip
        private void UpdateTabIndex ( Strip strip )
            {
            int stripIndex = mStripsPanel.Controls.IndexOf ( strip );
            int tabIndex = stripIndex > 0 ? ((Strip)mStripsPanel.Controls[stripIndex - 1]).LastTabIndex : 0;
            for (int i = stripIndex; i < mStripsPanel.Controls.Count; ++i)
                {
                tabIndex = ((Strip)mStripsPanel.Controls[i]).UpdateTabIndex ( tabIndex );
                }
            }

        // Update tab index for all controls after a block or a strip
        private void UpdateTabIndex ( Control c )
            {
            if (c is Block)
                {
                UpdateTabIndex ( ((Block)c).Strip );
                }
            else if (c is Strip)
                {
                UpdateTabIndex ( (Strip)c );
                }
            }

        #endregion

        #region shortcut keys

        private void InitializeShortcutKeys ()
            {
            mShortcutKeys = new Dictionary<Keys, ProjectView.HandledShortcutKey> ();

            mShortcutKeys[Keys.A] = delegate () { return mProjectView.TransportBar.MarkSelectionWholePhrase (); };
            mShortcutKeys[Keys.C] = delegate () { return mProjectView.TransportBar.PreviewAudioSelection (); };
            mShortcutKeys[Keys.H] = delegate () { return mProjectView.TransportBar.NextSection (); };
            mShortcutKeys[Keys.Shift | Keys.H] = delegate () { return mProjectView.TransportBar.PrevSection (); };
            mShortcutKeys[Keys.J] = delegate () { return mProjectView.TransportBar.PrevPhrase (); };
            mShortcutKeys[Keys.K] = delegate () { return mProjectView.TransportBar.NextPhrase (); };
            mShortcutKeys[Keys.N] = delegate () { return mProjectView.TransportBar.Nudge ( TransportBar.Forward ); };
            mShortcutKeys[Keys.Shift | Keys.N] = delegate () { return mProjectView.TransportBar.Nudge ( TransportBar.Backward ); };
            mShortcutKeys[Keys.OemOpenBrackets] = delegate () { return mProjectView.TransportBar.MarkSelectionBeginTime (); };
            mShortcutKeys[Keys.OemCloseBrackets] = delegate () { return mProjectView.TransportBar.MarkSelectionEndTime (); };
            mShortcutKeys[Keys.P] = delegate () { return mProjectView.TransportBar.NextPage (); };
            mShortcutKeys[Keys.Shift | Keys.P] = delegate () { return mProjectView.TransportBar.PrevPage (); };
            mShortcutKeys[Keys.V] = delegate () { return mProjectView.TransportBar.Preview ( TransportBar.From, TransportBar.UseAudioCursor ); };
            mShortcutKeys[Keys.Shift | Keys.V] = delegate () { return mProjectView.TransportBar.Preview ( TransportBar.From, TransportBar.UseSelection ); };
            mShortcutKeys[Keys.X] = delegate () { return mProjectView.TransportBar.Preview ( TransportBar.Upto, TransportBar.UseAudioCursor ); };
            mShortcutKeys[Keys.Shift | Keys.X] = delegate () { return mProjectView.TransportBar.Preview ( TransportBar.Upto, TransportBar.UseSelection ); };

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

            mShortcutKeys[Keys.PageDown] = ScrollDown_LargeIncrement;
            mShortcutKeys[Keys.PageUp] = ScrollUp_LargeIncrement;

            }

        private bool CanUseKeys { get { return (mSelection == null || !(mSelection is TextSelection)) && !m_IsBlocksVisibilityProcessActive; } }

        protected override bool ProcessCmdKey ( ref Message msg, Keys key )
            {
            if (CanUseKeys &&
                ((msg.Msg == ProjectView.WM_KEYDOWN) || (msg.Msg == ProjectView.WM_SYSKEYDOWN)) &&
                mShortcutKeys.ContainsKey ( key ) && mShortcutKeys[key] ()) return true;
            if (ProcessTabKeyInContentsView ( key )) return true;
            return base.ProcessCmdKey ( ref msg, key );
            }

        // Get the strip for the currently selected component (i.e. the strip itself, or the parent strip
        // for a block.)
        private Strip StripFor ( ISelectableInContentView item )
            {
            return item is Strip ? (Strip)item :
                   item is StripCursor ? ((StripCursor)item).Strip :
                   item is Block ? ((Block)item).Strip : null;
            }

        private delegate Block SelectBlockFunction ( Strip strip, ISelectableInContentView item );

        private bool SelectBlockFor ( SelectBlockFunction f )
            {
            Strip strip = StripFor ( mSelectedItem );
            if (strip != null)
                {
                Block block = f ( strip, mSelectedItem );
                if (block != null)
                    {
                    mProjectView.Selection = new NodeSelection ( block.Node, this );
                    return true;
                    }
                }
            return false;
            }

        private delegate int SelectStripCursorFunction ( Strip strip, ISelectableInContentView item );

        private bool SelectStripCursorFor ( SelectStripCursorFunction f )
            {
            Strip strip = StripFor ( mSelectedItem );
            if (strip != null)
                {
                int index = f ( strip, mSelectedItem );
                if (index >= 0)
                    {
                    //mProjectView.Selection = new StripIndexSelection ( strip.Node, this, index ); //@singleSection: original
                    mProjectView.Selection = new StripIndexSelection ( strip.Node, this, index + strip.OffsetForFirstPhrase);//@singleSection: new
                    return true;
                    }
                }
            return false;
            }

        private bool SelectPrecedingBlock ()
            {
            CreateBlocksInPreviousThresholdsSlot ();//@singleSection

            return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.BlockBefore ( mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock : item ); } );
            }

        private bool SelectPrecedingStripCursor ()
            {
            bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
            Block PlaybackBlock = null;
            if (mProjectView.TransportBar.CanUsePlaybackSelection)
                {
                PlaybackBlock = mPlaybackBlock;
                mProjectView.TransportBar.Stop ();
                }
            CreateBlocksInPreviousThresholdsSlot ();//@singleSection

            bool ReturnVal = SelectStripCursorFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.StripIndexBefore ( PlaybackBlock != null ? PlaybackBlock : item ); } );
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;

            return ReturnVal;
            }

        //@singleSection
        private void CreateBlocksInPreviousThresholdsSlot ()
            {
            EmptyNode currentlySelectedNode = mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock.Node :
                mProjectView.Selection != null && mProjectView.Selection.Node is EmptyNode ? (EmptyNode)mProjectView.Selection.Node : 
                mProjectView.Selection != null && mProjectView.Selection is StripIndexSelection ? ((StripIndexSelection) mProjectView.Selection).EmptyNodeForSelection :null;

            if (currentlySelectedNode != null)
                {
                Console.WriteLine ( "currently selected node in blocks : " + currentlySelectedNode );
                Strip s = FindStrip ( currentlySelectedNode.ParentAs<SectionNode> () );
                if (currentlySelectedNode.Index > 0 && s != null && s.OffsetForFirstPhrase == currentlySelectedNode.Index)
                    {
                    if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Pause ();

                    CreateBlocksTillNodeInStrip ( s, (EmptyNode)currentlySelectedNode.PrecedingNode, false );
                    Console.WriteLine ( "creating node till : " + currentlySelectedNode.PrecedingNode.Index );
                    
                    }
                }
            }

        private bool SelectFollowingBlock ()
            {
            return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.BlockAfter ( mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock : item ); } );
            }

        private bool SelectFollowingStripCursor ()
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

        private bool SelectLastBlockInStrip ()
            {
            if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
            if (mProjectView.Selection != null) CreateBlocksTillEndInStrip ( mStrips[mProjectView.GetSelectedPhraseSection] );//@singleSection

            return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.LastBlock; } );
            }

        private bool SelectFirstBlockInStrip ()
            {
            if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
            //if (mProjectView.Selection != null && mProjectView.GetSelectedPhraseSection.PhraseChildCount >0) CreateBlocksTillNodeInStrip( mStrips[mProjectView.GetSelectedPhraseSection], mProjectView.GetSelectedPhraseSection.PhraseChild(0),true );//@singleSection
            if (mProjectView.Selection != null && mProjectView.GetSelectedPhraseSection.PhraseChildCount > 0) //@singleSection
                {
                SelectPhraseBlockOrStrip ( mProjectView.GetSelectedPhraseSection.PhraseChild ( 0 ) );
                return true;
                }
            return false;//@singleSection
            //if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Stop();
            //return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.FirstBlock; } );//@singleSection: commented
            }

        private delegate Strip SelectStripFunction ( Strip strip );

        private bool SelectStripFor ( SelectStripFunction f )
            {
            Strip strip = f ( StripFor ( mSelectedItem ) as Strip );
            if (strip != null)
                {
                mProjectView.Selection = new NodeSelection ( strip.Node, this );
                return true;
                }
            return false;
            }

        private bool SelectPreviousStrip ()
            {
            bool WasPlaying = mProjectView.TransportBar.CurrentState == TransportBar.State.Playing;
            if (mProjectView.TransportBar.IsPlayerActive 
                && 
                (mProjectView.Selection == null
                || (mProjectView.Selection != null && !(mProjectView.Selection.Node is SectionNode )))) 
                {
                mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
            }

            if (mProjectView.GetSelectedPhraseSection == null) return false;
            SectionNode previousSection = mProjectView.GetSelectedPhraseSection.PrecedingSection ; //@singleSection
            if (previousSection != null && mProjectView.Selection.Node is SectionNode) CreateStripForSelectedSection ( previousSection , true); //@singleSection

            Strip strip;
            if (WasPlaying
                && PlaybackBlock != null && (this.mPlaybackBlock.ObiNode.Index == 0 || mPlaybackBlock.Node.Role_ == EmptyNode.Role.Heading))
                {
                strip = StripBefore ( StripFor ( mSelectedItem ) );
                }
            else
                strip = mSelectedItem is Strip ? StripBefore ( StripFor ( mSelectedItem ) ) : StripFor ( mSelectedItem );

            if (strip == null) strip = ActiveStrip;//@singleSection
            if (strip != null)
                {
                mProjectView.Selection = new NodeSelection ( strip.Node, this );
                strip.FocusStripLabel ();
                
                return true;
                }
            return false;
            }

        private bool SelectNextStrip ()
            {
            //@singleSection : starts
            SectionNode currentlySelectedSection = mProjectView.TransportBar.IsPlayerActive ? mProjectView.TransportBar.CurrentPlaylist.CurrentPhrase.ParentAs<SectionNode> () :
                mProjectView.GetSelectedPhraseSection;

            if (currentlySelectedSection  == null) return false;

            SectionNode nextSection = currentlySelectedSection.FollowingSection; 
            if (mProjectView.TransportBar.IsPlayerActive && nextSection != null) mProjectView.TransportBar.Stop ();

            if (nextSection != null &&
                (mProjectView.Selection == null || 
                (mProjectView.Selection != null && (mProjectView.Selection.Node is EmptyNode || mProjectView.Selection is StripIndexSelection ) ) ) )
                {
                mProjectView.Selection = new NodeSelection ( currentlySelectedSection, this );
                if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Stop ();
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip )
                        {
                        if ( ((Strip)c).Node == currentlySelectedSection )  ((Strip)c).FocusStripLabel ();
                        }
                    }
                }
                        if (nextSection != null) CreateStripForSelectedSection ( nextSection , true); //@singleSection: ends
                        
            Strip strip = StripAfter ( StripFor ( mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock : mSelectedItem ) );
            if (strip != null)
                {
                mProjectView.Selection = new NodeSelection ( strip.Node, this );
                strip.FocusStripLabel ();
                return true;
                }
            else if (mSelectedItem != null && StripFor ( mSelectedItem ) != (Strip)mStripsPanel.Controls[mStripsPanel.Controls.Count - 1] || Selection is TextSelection)
                {
                // allow base to process the key if  current strip is not last strip or some text is selected
                return false;
                }
            else
                {
                return true;
                }
            }

        private bool SelectFirstStrip ()
            {
            SectionNode section = mProjectView.Presentation.FirstSection; //@singleSection
            if (section != null ) CreateStripForSelectedSection ( section, true); //@singleSection

            return SelectStripFor ( delegate ( Strip strip )
{
    return mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[0] : null;
} );
            }

        private bool SelectLastStrip ()
            {
            
                ObiNode n = null;
                for (n = mProjectView.Presentation.RootNode.LastLeaf;
                    !(n is SectionNode);
                    n = n.PrecedingNode)
                { }

                SectionNode section = (SectionNode) n;
                if (mProjectView.TransportBar.IsPlayerActive && section != null) mProjectView.TransportBar.Stop ();
                if (mProjectView.Selection.Node is PhraseNode && section != null)
                    {
                    mProjectView.Selection = new NodeSelection ( mProjectView.GetSelectedPhraseSection, this );
                    foreach (Control c in mStripsPanel.Controls)
                        {
                        if (c is Strip)
                            {
                            if (((Strip)c).Node == mProjectView.GetSelectedPhraseSection) ((Strip)c).FocusStripLabel ();
                            }
                        }
                    }
                if (section != null) CreateStripForSelectedSection ( section , true); //@singleSection: ends
                
            return SelectStripFor ( delegate ( Strip strip )
{
    return mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[mStripsPanel.Controls.Count - 1] : null;
} );
            }

        // Select the item above the currently selected item.
        // E.g. from an audio selection a phrase, from a phrase a strip, from a strip nothing.
        private bool SelectUp ()
            {
            if (mSelection is AudioSelection)
                {
                return SelectBlockFor ( delegate ( Strip s, ISelectableInContentView item )
                    { return FindBlock ( (EmptyNode)mSelection.Node ); } );
                }
            else if (mSelectedItem is Block)
                {
                return SelectStripFor ( delegate ( Strip s ) { return ((Block)mSelectedItem).Strip; } );
                }
            else if (mSelectedItem is Strip)
                {
                mProjectView.Selection = null;
                return true;
                }
            return false;
            }

        private Strip StripAfter ( Strip strip )
            {
            if (strip != null)
                {
                int count = mStripsPanel.Controls.Count;
                int index = 1 + mStripsPanel.Controls.IndexOf ( strip );
                return index < count ? (Strip)mStripsPanel.Controls[index] : null;
                }
            return null;
            }

        public Strip StripBefore ( Strip strip )
            {
            if (strip != null)
                {
                int index = mStripsPanel.Controls.IndexOf ( strip );
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
                    return mPlaybackBlock != null ? mPlaybackBlock.Node: null;
                else
                    return mProjectView.SelectedNodeAs<ObiNode> ();
                }
            }


        /// <summary>
        /// Moves keyboard focus to preceding page node.
        /// </summary>
        public bool SelectPrecedingPageNode ()
            {
            if (SelectedNodeInTransportbarOrProjectview != null)
                {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.PrecedingNode; n != null; n = n.PrecedingNode)
                    {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                        {
                        //mProjectView.Selection = new NodeSelection(n, this);
                        SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                        return true;
                        }
                    }
                }
            return false;
            }

        /// <summary>
        /// Moves keyboard focus to the following page node.
        /// </summary>
        public bool SelectNextPageNode ()
            {
            if (SelectedNodeInTransportbarOrProjectview != null)
                {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.FollowingNode; n != null; n = n.FollowingNode)
                    {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                        {
                        //mProjectView.Selection = new NodeSelection(n, this);
                        SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
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
        public bool SelectPreviousSpecialRoleNode ()
            {
            if (SelectedNodeInTransportbarOrProjectview != null)
                {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.PrecedingNode; n != null; n = n.PrecedingNode)
                    {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ != EmptyNode.Role.Plain)
                        {
                        //mProjectView.Selection = new NodeSelection(n, this);
                        SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
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
        public bool SelectNextSpecialRoleNode ()
            {
            if (SelectedNodeInTransportbarOrProjectview != null)
                {
                for (ObiNode n = SelectedNodeInTransportbarOrProjectview.FollowingNode; n != null; n = n.FollowingNode)
                    {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ != EmptyNode.Role.Plain)
                        {
                        //mProjectView.Selection = new NodeSelection(n, this);
                        SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                        return true;
                        }
                    }
                }// check ends for empty node
            return false;
            }


        /// <summary>
        /// Select previous to do node in contents view
        /// </summary>
        public void SelectNextTODONode ()
            {
            if (mProjectView.Presentation != null)
                {
                if (SelectedNodeInTransportbarOrProjectview != null)
                    {
                    for (ObiNode n = SelectedNodeInTransportbarOrProjectview.FollowingNode; n != null; n = n.FollowingNode)
                        {
                        if (n is EmptyNode && ((EmptyNode)n).TODO)
                            {
                            //mProjectView.Selection = new NodeSelection ( n, this );
                            SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                            return;
                            }
                        }
                    }
                for (ObiNode n = mProjectView.Presentation.RootNode.FirstLeaf; n != null; n = n.FollowingNode)
                    {
                    if (n is EmptyNode && ((EmptyNode)n).TODO)
                        {
                        //mProjectView.Selection = new NodeSelection ( n, this );
                        SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                        return;
                        }
                    }
                } // check for null presentation ends
            }

        /// <summary>
        /// Select previous to do node in contents view
        /// </summary>
        public void SelectPrecedingTODONode ()
            {
            if (mProjectView.Presentation != null)
                {
                if (SelectedNodeInTransportbarOrProjectview != null)
                    {
                    for (ObiNode n = SelectedNodeInTransportbarOrProjectview.PrecedingNode; n != null; n = n.PrecedingNode)
                        {
                        if (n is EmptyNode && ((EmptyNode)n).TODO)
                            {
                            //mProjectView.Selection = new NodeSelection(n, this);
                            SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                            return;
                            }
                        }
                    }
                for (ObiNode n = mProjectView.Presentation.RootNode.LastLeaf; n != null; n = n.PrecedingNode)
                    {
                    if (n is EmptyNode && ((EmptyNode)n).TODO)
                        {
                        //mProjectView.Selection = new NodeSelection(n, this);
                        SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
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
                            if (n is EmptyNode && !(n is PhraseNode))
                                {
                                //mProjectView.Selection = new NodeSelection ( n, this );
                                SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                                return true;
                                }
                            }
                        }
                    for (ObiNode n = mProjectView.Presentation.RootNode.FirstLeaf; n != null; n = n.FollowingNode)
                        {
                        if (n is EmptyNode && !(n is PhraseNode))
                            {
                            //mProjectView.Selection = new NodeSelection ( n, this );
                            SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                            return true;
                            }
                        }
                    } // check for null presentation ends

                } // try ends
            catch (System.Exception)
                {
                MessageBox.Show ( Localizer.Message ( "Caption_Error" ), Localizer.Message ( "Caption_Error" ) );
                }
            return false;
            }


        // @phraseLimit
        public void SelectPhraseBlockOrStrip ( EmptyNode node )
            {
            if (node != null)
                {
                //@singleSection: all of the part in this if block refactored, old part is commented below
                // no need to change anything in functions like next / previous page, todo, special node etc. changing this function did the behaviour of single section
                SectionNode parentSection = node.ParentAs<SectionNode> ();
                bool isParentSectionVisible = false;
                Strip strip = null ;

                //check if strip layout contains this section strip
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip)
                        {
                        strip = ((Strip)c) ;
                        if (strip.Node == parentSection) isParentSectionVisible = true;
                        }
                    }

                if (!isParentSectionVisible)
                    {
                    if (MessageBox.Show ( "The required phrase is not in current section. Will you like to show the section containing the phrase?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes)
                        {
                        CreateStripForSelectedSection ( parentSection, true );
                        }
                    else
                        {
                        return;
                        }
                    }
                
                if ( node != null && strip != null &&  strip.FindBlock ( node ) == null)
                    {
                    
                    // if parent section is visible, then check if target phrase is visible
                    // if not, create phrases till it.
                    ObiNode targetNode = node;
                    
                    if (strip.OffsetForFirstPhrase < node.Index)
                        {

                        ObiNode iterationNode = node;
                        for (int i = 0; i < 15; i++)
                            {
                            if (!(iterationNode is EmptyNode)
                                || iterationNode.ParentAs<SectionNode> () != node.ParentAs<SectionNode> ())
                                {
                                break;
                                }
                            else
                                {
                                targetNode = iterationNode;
                                }
                            iterationNode = iterationNode.FollowingNode;
                            }
                        }
                    
                    if (strip != null) CreateBlocksTillNodeInStrip ( strip,(EmptyNode)  node, false );
                    }

                if ( node != null )  mProjectView.Selection = new NodeSelection ( node, this );
                /*
                if (IsBlockInvisibleButStripVisible ( node ))
                    {
                    mProjectView.Selection = new NodeSelection ( node.ParentAs<SectionNode> (), this );
                    }
                else
                    {
                    mProjectView.Selection = new NodeSelection ( node, this );
                    }
                 */ 
                }

            }

        //@singleSection
        public Strip ActiveStrip
            {
            get
                {
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip)
                        {
                        return ((Strip) c) ;
                        }
                    }
                return null;
                }
            }

        //@singleSection
        private int PredictedMaxStripsLayoutHeight
            {
            get
                {
                int height = 0;
                foreach (Control c in mStripsPanel.Controls)
                    {
                    height += c is Strip ? ((Strip)c).PredictedStripHeight : 0;
                    }
                //Console.WriteLine ( "predicted scroll height " + height + " " + mStripsPanel.Height.ToString () );
                return height > mStripsPanel.Height ? height : mStripsPanel.Height; 
                }
            }


        // Toggle play/pause in the transport bar
        public bool TogglePlayPause ()
            {
            if (mProjectView.TransportBar.CanPause)
                {
                mProjectView.TransportBar.Pause ();
                return true;
                }
            else if (mProjectView.TransportBar.CanPlay || mProjectView.TransportBar.CanResumePlayback)
                {
                mProjectView.TransportBar.PlayOrResume ();
                return true;
                }
            return false;
            }


        private bool FastPlayRateStepDown ()
            {
            return mProjectView.TransportBar.FastPlayRateStepDown ();
            }

        private bool FastPlayRateStepUp ()
            {
            return mProjectView.TransportBar.FastPlayRateStepUp ();
            }

        private bool FastPlayRateNormalise ()
            {
            return mProjectView.TransportBar.FastPlayRateNormalise ();
            }

        private bool FastPlayNormaliseWithLapseBack ()
            {
            return mProjectView.TransportBar.FastPlayNormaliseWithLapseBack ();
            }

        private bool MarkSelectionFromCursor ()
            {
            return mProjectView.TransportBar.MarkSelectionFromCursor ();
            }

        private bool MarkSelectionToCursor ()
            {
            return mProjectView.TransportBar.MarkSelectionToCursor ();
            }



        #endregion

        public void SelectAtCurrentTime ()
            {
            if (mPlaybackBlock != null)
                mPlaybackBlock.SelectAtCurrentTime ();
            }

        public void GetFocus ()
            {
            if (mSelection == null)
                {
                mProjectView.Selection = new NodeSelection ( mProjectView.Presentation.FirstSection, this );
                }
            else
                {
                Focus ();
                }
            }

        private void StripsView_Enter ( object sender, EventArgs e )
            {
            mIsEnteringView = true;
            }

        /// <summary>
        ///  Function for processing tab key to preventing keyboard focus to move out of contents view with tabbing
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool ProcessTabKeyInContentsView ( Keys key )
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
                        SelectFirstStrip ();
                        System.Media.SystemSounds.Beep.Play ();
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
                            System.Media.SystemSounds.Beep.Play ();
                            if (LastStrip.LastBlock != null)
                                {
                                return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return LastStrip.LastBlock; } );
                                }
                            else
                                {
                                return SelectLastStrip ();
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
        public void UpdateContextMenu ()
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
            //Context_PhraseIsTODOMenuItem.Enabled = mProjectView.CanSetTODOStatus && !mProjectView.TransportBar.IsActive;
            Context_PhraseIsTODOMenuItem.Enabled = mProjectView.CanSetTODOStatus ; // made consistent with drop down menu. if not suitable the commented lines around can be restored.
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
            Context_CutMenuItem.Enabled = (CanRemoveAudio || CanRemoveBlock || CanRemoveStrip) && !mProjectView.TransportBar.IsRecorderActive;
            Context_CopyMenuItem.Enabled = CanCopyAudio || CanCopyBlock || CanCopyStrip;
            Context_PasteMenuItem.Enabled = mProjectView.CanPaste;
            Context_PasteBeforeMenuItem.Enabled = mProjectView.CanPasteBefore;
            Context_PasteInsideMenuItem.Enabled = mProjectView.CanPasteInside;
            Context_DeleteMenuItem.Enabled = (CanRemoveAudio || CanRemoveBlock || CanRemoveStrip) && !mProjectView.TransportBar.IsRecorderActive;
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
                return IsBlockSelected && SelectedEmptyNode.AncestorAs<SectionNode> ().Used;
                }
            }

        // Add section context menu item
        private void Context_AddSectionMenuItem_Click ( object sender, EventArgs e ) { mProjectView.AddStrip (); }

        // Insert section context menu item
        private void Context_InsertSectionMenuItem_Click ( object sender, EventArgs e ) { mProjectView.InsertSection (); }

        // Split section context menu item
        private void Context_SplitSectionMenuItem_Click ( object sender, EventArgs e ) { mProjectView.SplitStrip (); }

        // Merge section with next context menu item
        private void Context_MergeSectionWithNextMenuItem_Click ( object sender, EventArgs e ) { mProjectView.MergeStrips (); }

        // Add blank phrase context menu item
        private void Context_AddBlankPhraseMenuItem_Click ( object sender, EventArgs e ) { mProjectView.AddEmptyBlock (); }

        // Add empty pages context menu item
        private void Context_AddEmptyPagesMenuItem_Click ( object sender, EventArgs e ) { mProjectView.AddEmptyPages (); }

        // Import audio files context menu item
        private void Context_ImportAudioFilesMenuItem_Click ( object sender, EventArgs e ) { mProjectView.ImportPhrases (); }

        // Split phrase context context menu item
        private void Context_SplitPhraseMenuItem_Click ( object sender, EventArgs e ) { mProjectView.SplitPhrase (); }

        // Merge phrase with next context menu item
        private void Context_MergePhraseWithNextMenuItem_Click ( object sender, EventArgs e ) { mProjectView.MergeBlockWithNext (); }

        // Crop audio context menu item
        private void Context_CropAudioMenuItem_Click ( object sender, EventArgs e ) { mProjectView.CropPhrase (); }

        // Phrase is TODO context menu item
        private void Context_PhraseIsTODOMenuItem_Click ( object sender, EventArgs e )
            {
            Context_PhraseIsTODOMenuItem.Checked = !Context_PhraseIsTODOMenuItem.Checked;
            mProjectView.ToggleTODOForPhrase ();
            }

        // Phrase is used context menu item
        private void Context_PhraseIsUsedMenuItem_Click ( object sender, EventArgs e )
            {
            Context_PhraseIsUsedMenuItem.Checked = !Context_PhraseIsUsedMenuItem.Checked;
            mProjectView.SetSelectedNodeUsedStatus ( Context_PhraseIsUsedMenuItem.Checked );
            }

        // Assign role > Plain context menu item
        private void Context_AssignRole_PlainMenuItem_Click ( object sender, EventArgs e )
            {
            if (mProjectView.CanAssignPlainRole) mProjectView.SetRoleForSelectedBlock ( EmptyNode.Role.Plain, null );
            }

        private void Context_AssignRole_HeadingMenuItem_Click ( object sender, EventArgs e )
            {
            if (mProjectView.CanAssignHeadingRole) mProjectView.SetRoleForSelectedBlock ( EmptyNode.Role.Heading, null );
            }

        private void Context_AssignRole_PageMenuItem_Click ( object sender, EventArgs e )
            {
            if (mProjectView.CanAssignARole) mProjectView.SetPageNumberOnSelection ();
            }

        private void Context_AssignRole_SilenceMenuItem_Click ( object sender, EventArgs e )
            {
            if (mProjectView.CanAssignSilenceRole) mProjectView.SetSilenceRoleForSelectedPhrase ();
            }

        private void Context_AssignRole_NewCustomRoleMenuItem_Click ( object sender, EventArgs e )
            {
            if (mProjectView.CanAssignARole) mProjectView.ShowPhrasePropertiesDialog ( true );
            }

        // Clear role context menu item
        private void Context_ClearRoleMenuItem_Click ( object sender, EventArgs e )
            {
            mProjectView.ClearRoleOfSelectedPhrase ();
            }

        // Apply phrase detection context menu item
        private void Context_ApplyPhraseDetectionMenuItem_Click ( object sender, EventArgs e )
            {
            mProjectView.ApplyPhraseDetection ();
            }

        private void Context_AudioSelection_BeginMenuItem_Click ( object sender, EventArgs e )
            {
            mProjectView.TransportBar.MarkSelectionBeginTime ();
            }

        private void Context_AudioSelection_EndMenuItem_Click ( object sender, EventArgs e )
            {
            mProjectView.TransportBar.MarkSelectionEndTime ();
            }


        // Cut context menu item
        private void Context_CutMenuItem_Click ( object sender, EventArgs e ) { mProjectView.Cut (); }

        // Copy context menu item
        private void Context_CopyMenuItem_Click ( object sender, EventArgs e ) { mProjectView.Copy (); }

        // Paste context menu item
        private void Context_PasteMenuItem_Click ( object sender, EventArgs e ) { mProjectView.Paste (); }

        // Paste before context menu item
        private void Context_PasteBeforeMenuItem_Click ( object sender, EventArgs e ) { mProjectView.PasteBefore (); }

        // Paste inside context menu item
        private void Context_PasteInsideMenuItem_Click ( object sender, EventArgs e ) { mProjectView.PasteInside (); }

        // Delete context menu item
        private void Context_DeleteMenuItem_Click ( object sender, EventArgs e ) { mProjectView.Delete (); }

        // Properties context menu item
        private void Context_PropertiesMenuItem_Click ( object sender, EventArgs e )
            {
            if (mProjectView.CanShowPhrasePropertiesDialog)
                {
                mProjectView.ShowPhrasePropertiesDialog ( false );
                }
            else if (mProjectView.CanShowSectionPropertiesDialog)
                {
                mProjectView.ShowSectionPropertiesDialog ();
                }
            else
                {
                mProjectView.ShowProjectPropertiesDialog ();
                }
            }

        public void SuspendLayout_All ()
            {
            Invalidate ();
            foreach (Control c in mStripsPanel.Controls) c.SuspendLayout ();
            }

        public void ResumeLayout_All ()
            {
            foreach (Control c in mStripsPanel.Controls)
                {
                c.ResumeLayout ();
                if (c is Strip) ((Strip)c).Resize_All ();
                }
            }

        private void mHScrollBar_ValueChanged ( object sender, EventArgs e )
            {
            mStripsPanel.Location = new Point ( -mHScrollBar.Value, mStripsPanel.Location.Y );
            System.Diagnostics.Debug.Print ( "X = {0}/{1}, W = {2}/{3}",
                mStripsPanel.Location, mHScrollBar.Maximum, mStripsPanel.Size, VisibleWidth );
            }

        private void mVScrollBar_ValueChanged ( object sender, EventArgs e )
            {
                //mStripsPanel.Location = new Point ( mStripsPanel.Location.X, -mVScrollBar.Value );      
            }

        //@singleSection
        private void CreatePhrasesAccordingToVScrollBarValue ( int scrollValue )
            {
            Strip currentlyActiveStrip = ActiveStrip ;
            if (currentlyActiveStrip != null)
                {
                int indexOfPhraseToBeShown = Convert.ToInt32 ( (scrollValue * currentlyActiveStrip.Node.PhraseChildCount) / currentlyActiveStrip.PredictedStripHeight );
                Console.WriteLine ( "Index of phrase to be shown for verticle scroll " + indexOfPhraseToBeShown );
                CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                    currentlyActiveStrip.Node.PhraseChild ( indexOfPhraseToBeShown ),
                    false);
                // adjust location of strips panel such that the phrase blocks at end are shown.
                if ( mStripsPanel.Height > (this.Height - mHScrollBar.Location.Y ))
                    {
                    int stripsPanelYLocation = this.Height  - mStripsPanel.Height;
                    if (indexOfPhraseToBeShown == 0)
                        {
                        stripsPanelYLocation = 0;
                        
                        }
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, stripsPanelYLocation );
                    Console.WriteLine ( "Y location of strip panel after scroll is " + stripsPanelYLocation + " " + mStripsPanel.Size  );
                    CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                    currentlyActiveStrip.Node.PhraseChild ( indexOfPhraseToBeShown ),
                    true );
                    }
                }
            }

        BackgroundWorker m_ScrolBackgroundWorker = new BackgroundWorker ();//@singleSection

        //@singleSection
        private void StartCreatingBlockForScroll ()
            {
            if (m_ScrolBackgroundWorker.IsBusy) return;
            m_ScrolBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_ScrolBackgroundWorker_RunWorkerCompleted);
            m_ScrolBackgroundWorker.RunWorkerAsync ();
            }

        //@singleSection
        private void m_ScrolBackgroundWorker_RunWorkerCompleted ( object sender, EventArgs e )
            {
            Strip s = null;
            foreach (Strip c in mStripsPanel.Controls)
                {
                if (c is Strip) s = (Strip)c;
                }
                        
            CreateBlocksTillNodeInStrip ( s, null, true ) ;
            }

        //@singleSection
        private void mVScrollBar_Scroll ( object sender, ScrollEventArgs e )
            {
                mScroll = true;
                this.mVScrollBar.Maximum = PredictedMaxStripsLayoutHeight;
                int height;
                timer1.Start();
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll
                    && e.OldValue < e.NewValue)
                {// StartCreatingBlockForScroll (); 
                }

                height = mStripsPanel.Location.Y - PredictedMaxStripsLayoutHeight;
                if (PredictedMaxStripsLayoutHeight < this.Size.Height)
                {
                    this.mStripsPanel.Location = new Point(mStripsPanel.Location.X, this.Size.Height + height);
                }                
            }

        //@singleSection
        private void ContentView_Resize ( object sender, EventArgs e )
            {
            if (ActiveStrip != null )
                {
                CreateLimitedBlocksInStrip ( ActiveStrip, null );
                }
                //this.contentViewLabel1.Size = new Size(this.Size.Width + this.mVScrollBar.Width, 22);
            this.verticleScrollPane1.Location =new Point ( this.Width - verticleScrollPane1.Width , 0 ) ;
            this.verticleScrollPane1.Size = new Size ( verticleScrollPane1.Width, mHScrollBar.Location.Y );
            mHScrollBar.Size = new Size ( verticleScrollPane1.Location.X, mHScrollBar.Height );
            }

        //@singleSection
        private void ProjectView_SelectionChanged ( object sender, EventArgs e ) 
            {
            if (mProjectView.GetSelectedPhraseSection == null)
                {
                contentViewLabel1.sectionSelected = false;
                return;
                }
            Strip currentlyActiveStrip = ActiveStrip;

            if (currentlyActiveStrip == null) return;

            if (mProjectView.GetSelectedPhraseSection == currentlyActiveStrip.Node )
                {
                
                contentViewLabel1.sectionSelected = true;
                
                }
            else
                {
                contentViewLabel1.sectionSelected = false;
                
                }
            }
        

        private void mStripsPanel_ControlRemoved ( object sender, EventArgs e )
            {
            if (mStripsPanel.Controls.Count == 0)
                {
                contentViewLabel1.Name_SectionDisplayed = Localizer.Message ( "ContentViewLabel_NoSection" );
                }
            }

        private void ContentView_MouseWheel(object sender, MouseEventArgs e)
        {
            int increment = Convert.ToInt32(mHScrollBar.Location.Y * 0.4);
            if (e.Delta > 0)
                increment = increment * (-1);
            ScrollMStripsPanel(increment);
            Console.WriteLine ( "mouse wheel scrolling " + increment );
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int interval;
            int mid;
            interval = this.mStripsPanel.Height / this.mVScrollBar.Height;
            mid = this.mVScrollBar.Height / 2;
            for (int i = 1; i <= interval; i++)
            {
                if ((mVScrollBar.Value > (i - 1) * mVScrollBar.Height) && (mVScrollBar.Value < (mVScrollBar.Height * i)))
                {
                    if (i == interval)
                        break;
                    if (mVScrollBar.Value < ((mVScrollBar.Height * (i - 1) + mid)))
                    {
                      //  mStripsPanel.Location = new Point(mStripsPanel.Location.X, (-mVScrollBar.Height * (i - 1)));
                        this.mVScrollBar.Value = mVScrollBar.Height * (i - 1);
                    }
                    else if (mVScrollBar.Value > ((mVScrollBar.Height * (i - 1) + mid)))
                    {
                     //   mStripsPanel.Location = new Point(mStripsPanel.Location.X, (-mVScrollBar.Height * i));
                        this.mVScrollBar.Value = mVScrollBar.Height * (i);
                    }
                }
            }      
            timer1.Stop();
            if (mScroll)
            {
            mEnableScrolling = false;
                CreatePhrasesAccordingToVScrollBarValue(mVScrollBar.Value); }
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
        void SetSelectionFromContentView ( NodeSelection selection );  // set the selection from the parent view
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
