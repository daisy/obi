using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using urakawa;
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

        private bool m_CreatingGUIForNewPresentation;
        private bool m_IsBlocksVisibilityProcessActive;
        private NodeSelection m_PreviousSelectionForScroll; //caches previous selection for restore while scroll
        //private Mutex m_BlocksVisibilityOperationMutex; //@phraseLimit
        
        private delegate Strip AddStripForObiNodeDelegate ( ObiNode node );
        private delegate void RemoveControlForSectionNodeDelegate ( SectionNode node );
        private bool m_IsWaveformRenderingPaused;
        private Waveform m_RenderingWaveform = null;
        private EmptyNode m_BeginNote = null; //@AssociateNode
        private EmptyNode m_EndNote = null;  //@AssociateNode
        private Waveform_Recording waveform_recording_control;
        private ZoomWaveform m_ZoomWaveformPanel;
        private Toolbar_EditAudio m_Edit;
        private double m_timeElapsed = 0.0;
        private Color m_ColorBackgroundBeforeFlicker;

        /// <summary>
        /// A new strips view.
        /// </summary>
        public ContentView ()
            {
            InitializeComponent ();
            //InitializeShortcutKeys ();
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


            m_IsBlocksVisibilityProcessActive = false;

            this.contentViewLabel1.contentView = this;
            verticalScrollToolStripContainer1.contentView = this;
            waveform_recording_control = new Waveform_Recording();
            this.Controls.Add(waveform_recording_control);
            this.waveform_recording_control.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.waveform_recording_control.BackColor = System.Drawing.SystemColors.HighlightText;
            
            waveform_recording_control.contentView = this;    //@Onthefly
            mStripsPanel.ControlRemoved += new ControlEventHandler ( mStripsPanel_ControlRemoved );
            this.MouseWheel += new MouseEventHandler ( ContentView_MouseWheel );//@singleSection
            mStripsPanel.LocationChanged += new EventHandler ( mStripsPanel_LocationChanged );//@singleSection
            mStripsPanel.Resize += new EventHandler ( mStripsPanel_Resize );
            m_PreviousSelectionForScroll = null;
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
         public void UpdateSize ()
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
            //mHScrollBar.Maximum = w_max - VisibleWidth + mHScrollBar.LargeChange - 1 + mHScrollBar.Height; //@singleSection:commented
            int verticalScrollStripAllowance = w_max < 3000 ? verticalScrollToolStripContainer1.Width : 0; //@singleSection: workaround
            mHScrollBar.Maximum = w_max - (VisibleWidth - verticalScrollStripAllowance) + mHScrollBar.LargeChange - 1 + mHScrollBar.Height;
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
        public bool CanSetStripUsedStatus { get { return IsStripSelected && mSelection != null && mSelection.Node.IsRooted &&  mSelection.Node.ParentAs<ObiNode>().Used; } }
        public EmptyNode RecordingNode { get { return mProjectView.TransportBar.CurrentState == TransportBar.State.Recording? mProjectView.TransportBar.RecordingPhrase: null; } }

        public bool CanMergeBlockWithNext
            {
            get
                {
                EmptyNode node = mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock.Node : (mSelectedItem is Block ) ? ((Block)mSelectedItem).Node : (mSelectedItem is ZoomWaveform) ? ((ZoomWaveform)mSelectedItem).ZoomPanelNode : null ;
                return node != null && node.IsRooted
                    && node.Index < node.ParentAs<ObiNode> ().PhraseChildCount - 1;
                }
            }

        /*//@singleSection: moved to project view to enable merge in TOC
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
        */

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
        public urakawa.command.Command DeleteStripCommand () { return DeleteStripCommand ( SelectedSection ); }

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

        public EmptyNode BeginSpecialNode   //@AssociateNode
            { 
                get { return m_BeginNote; }
                set 
                { 
                    m_BeginNote = value;
                    UpdateContextMenu();
                } 
            }

        public EmptyNode EndSpecialNode     //@AssociateNode
        {
            get { return m_EndNote; }
            set { m_EndNote = value; }
        }
        /// <summary>
        /// Add a custom class to the context menu.
        /// </summary>
        /// 
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

            /*//@singleSection: moved to project view to enable merge in toc
            /// <summary>
            /// Get a command to merge the selected strip with the next one. If the next strip is a child or a sibling, then
            /// its contents are appended to the selected strip and it is removed from the project; but if the next strip has
            /// a lower level, merging is not possible.
            /// </summary>
            public urakawa.command.Command MergeSelectedStripWithNextCommand ()
                {
                CompositeCommand command = null;
                if (CanMergeStripWithNext)
                    {
                    command = mProjectView.Presentation.getCommandFactory ().createCompositeCommand ();
                    command.setShortDescription ( Localizer.Message ( "merge_sections" ) );
                    SectionNode section = SelectedSection;
                    command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.UpdateSelection ( mProjectView, new NodeSelection ( section, this ) ) );
                    SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild ( 0 );
                    //if (!section.Used) mProjectView.AppendMakeUnused ( command, next );
                    // Delete nodes in reverse order so that they are added back in the right order on redo
                    // and remove the heading role if there is any in the next section
                    //for (int i = next.PhraseChildCount - 1; i >= 0; --i)
                        //{
                        // Remove the role before removing the node because it needs to be attached to
                        // inform its parent that it is not a heading anymore.
                        //if (next.PhraseChild ( i ).Role_ == EmptyNode.Role.Heading)
                            //{
                            //Commands.Node.AssignRole role =
                                //new Commands.Node.AssignRole ( mProjectView, next.PhraseChild ( i ), EmptyNode.Role.Plain );
                            //role.UpdateSelection = false;
                            //command.ChildCommands.Insert ( role );
                            //}
                        //Commands.Node.Delete delete = new Commands.Node.Delete ( mProjectView, next.PhraseChild ( i ) );
                        //delete.UpdateSelection = false;
                        //command.ChildCommands.Insert ( delete );
                        //}
                    for (int i = 0; i < next.PhraseChildCount; ++i)
                        {
                        EmptyNode newPhraseNode = (EmptyNode) next.PhraseChild ( i ).copy ( false, true );
                        if (newPhraseNode.Role_ == EmptyNode.Role.Heading)
                            {
                            newPhraseNode.Role_ = EmptyNode.Role.Plain;
                            }
                        if (!section.Used && newPhraseNode.Used)
                            {
                            newPhraseNode.Used = section.Used;
                            }
                        command.ChildCommands.Insert (command.ChildCommands.Count, new
                            Commands.Node.AddNode ( mProjectView, newPhraseNode, section, section.PhraseChildCount + i, false ) );
                        }
                    command.ChildCommands.Insert (command.ChildCommands.Count,  DeleteStripCommand ( next ) );
                    }
                return command;
                }
                     */

            /// <summary>
        /// Set a new presentation for this view.
        /// </summary>
        public void NewPresentation ()
            {
            m_CreatingGUIForNewPresentation = true;
            ClearStripsPanel ();
            m_PreviousSelectionForScroll = null;
            mStripsPanel.Location = new Point(0,0 );

            if (mProjectView != null)
            {
                this.waveform_recording_control.projectView = mProjectView;  //@Onthefly
                this.waveform_recording_control.VUMeter = mProjectView.TransportBar.VuMeter;  //@Onthefly
            }

            ClearWaveformRenderQueue ();
            SuspendLayout_All ();
            ObiNode bookMarkedNode = ((ObiRootNode)mProjectView.Presentation.RootNode).BookmarkNode;
            if (mWrapStripContents && mProjectView.Presentation.FirstSection != null)
            {
                SectionNode sectionToDisplay = mProjectView.Presentation.FirstSection;
                if (bookMarkedNode != null) Console.WriteLine("Bookmark node is " + bookMarkedNode.ToString());
                if (mProjectView.ObiForm.Settings.Project_OpenBookmarkNodeOnReopeningProject && bookMarkedNode != null)
                {
                    sectionToDisplay = bookMarkedNode is SectionNode ?
                        (SectionNode)bookMarkedNode :
                        bookMarkedNode is EmptyNode ? bookMarkedNode.ParentAs<SectionNode>() : sectionToDisplay;
                }
                AddStripForSection_Safe(sectionToDisplay);
                mProjectView.SynchronizeViews = false;
                contentViewLabel1.Name_SectionDisplayed = sectionToDisplay.Label; //@singleSection
                IsScrollActive = false; //@singleSection
                
            }
            else
                {
                AddStripForSection_Safe ((ObiNode)  mProjectView.Presentation.RootNode ); //this will not be called in single section//sdk2 :root node casted
                }
                m_DisablePhraseCreationWhileSelectionRestore = false;
                if (mProjectView.ObiForm.Settings.Project_OpenBookmarkNodeOnReopeningProject && bookMarkedNode != null )
                {
                    bool statusForSelectionChangedPlaybackEnabled = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                    if (bookMarkedNode is SectionNode)
                    {
                        mProjectView.Selection = new NodeSelection(bookMarkedNode, this);
                    }
                    else if (bookMarkedNode is EmptyNode)
                    {
                        SelectPhraseBlockOrStrip((EmptyNode)bookMarkedNode);
                    }
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = statusForSelectionChangedPlaybackEnabled;
                }
                else
                {
                    CreateBlocksForInitialStrips(); //@phraseLimit
                }
            ResumeLayout_All ();
            mProjectView.Presentation.BeforeCommandExecuted +=
                new EventHandler<urakawa.events.command.CommandEventArgs> ( Presentation_BeforeCommandExecuted );
            mProjectView.Presentation.UndoRedoManager.CommandDone+=
                new EventHandler<urakawa.events.undo.DoneEventArgs> ( ContentView_commandDone );
            mProjectView.Presentation.UndoRedoManager.CommandReDone+= new EventHandler<urakawa.events.undo.ReDoneEventArgs>(ContentView_commandReDone);
            mProjectView.Presentation.UndoRedoManager.CommandUnDone += new EventHandler<urakawa.events.undo.UnDoneEventArgs>(ContentView_commandUndone);
            mProjectView.TransportBar.Recorder.StateChanged += new AudioLib.AudioRecorder.StateChangedHandler(Recorder_StateChanged);
            EventsAreEnabled = true;
            UpdateSize ();
            mVScrollBar.Value = 0;
            mHScrollBar.Value = 0;

            m_CreatingGUIForNewPresentation = false;
            }

        private void ContentView_commandDone ( object sender, urakawa.events.undo.DoneEventArgs e )
            {
                ResizeForCommands();
                
            // explicit toolstrip enabling for merge preceding, it is not so important, can be allowed to work like other commands but will be helpful to user in this operation
                if (e.Command is CompositeCommand
                        && ((CompositeCommand)e.Command).ShortDescription == Localizer.Message("Merge_RangeOfPhrases"))
                {
                    if (mProjectView.Selection != null && mProjectView.Selection.Node is EmptyNode)
                    {
                        EmptyNode currentlySelectedEmptyNode = (EmptyNode)mProjectView.Selection.Node;
                        if (currentlySelectedEmptyNode.IsRooted && currentlySelectedEmptyNode.Index == 0 && mStripsPanel.Location.Y >= -5)
                        {
                            verticalScrollToolStripContainer1.CanScrollUp = true;
                            
                        }
                    }
                    
                }
            }

        private void ContentView_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e)
        {
            ResizeForCommands();
            if (e.Command is CompositeCommand
                && mProjectView.Selection != null && !(mProjectView.Selection is AudioSelection) )
                            {
                Control c = mSelectedItem != null && (mSelectedItem is Block || mSelectedItem is StripCursor) ? (Control)mSelectedItem : null;
                if (c != null) EnsureControlVisible(c);
            }
        }

        private void ContentView_commandUndone (object sender, urakawa.events.undo.UnDoneEventArgs e)
        {
            ResizeForCommands();
            // workaround for making selection visible in some complex, large volume commands
            if (e.Command is CompositeCommand  && (((CompositeCommand)e.Command ).ShortDescription == Localizer.Message("split_section") || ((CompositeCommand)e.Command).ShortDescription == Localizer.Message("phrase_detection")
                || ((CompositeCommand)e.Command ).ShortDescription == Localizer.Message("Merge_RangeOfPhrases") || ((CompositeCommand)e.Command ).ShortDescription ==  Localizer.Message("Delete_RangeOfPhrases")
                || ((CompositeCommand)e.Command ).ShortDescription ==  Localizer.Message("import_phrases")))
            {
                Control c = mSelectedItem != null && (mSelectedItem is Block || mSelectedItem is StripCursor) ? (Control)mSelectedItem : null;
                if (c != null)
                {
                    EnsureControlVisible(c);
                    Strip currentlyActiveStrip = ActiveStrip ;
                    if (currentlyActiveStrip != null) CreatePhraseBlocksForFillingContentView(currentlyActiveStrip);
                }
                
            }
            if ( e.Command is CompositeCommand && mProjectView.Selection == null && ActiveStrip == null
                &&  e.Command.ShortDescription == Localizer.Message("recording_command") )
            {
                SectionNode sectionLast = mProjectView.Presentation.LastSection ;
                if ( sectionLast != null ) CreateStripForAddedSectionNode(sectionLast, false) ;
            }
            UpdateVerticalScrolPanelButtons();
        }

        private void ResizeForCommands()
        {
            if (mProjectView.Selection != null && mProjectView.Selection.Node is SectionNode && mProjectView.Selection.Control is TOCView
                && ActiveStrip == null)
            {
                CreateStripForSelectedSection(mProjectView.GetSelectedPhraseSection, true);
            }
            ResumeLayout_All();
            UpdateSize();
            if (!UpdateScrollTrackBarAccordingToSelectedNode()) 
            {
                Strip currentlyActiveStrip = ActiveStrip;
                if ( currentlyActiveStrip != null )  verticalScrollToolStripContainer1.TrackBarValueInPercentage = EstimateScrollPercentage(currentlyActiveStrip);
            }
            Cursor = mCursor;
            //Console.WriteLine("horizontal bar size " + mHScrollBar.Maximum);
            //UpdateBlocksLabelInSelectedNodeStrip ();
        }

        //@singleSection
        /// <summary>
        /// returns true if limited phrases in a strip are created and content view is not completely filled
        /// </summary>
        /// <returns></returns>
        public bool IsLimitedPhraseBlocksCreatedAfterCommand()
        {
            //if (mStripsPanel.Location.Y + (mStripsPanel.Height + mStripsPanel.Margin.Bottom * 2) < mHScrollBar.Location.Y)
            if (mStripsPanel.Location.Y + (mStripsPanel.Height + mStripsPanel.Margin.Bottom * 2) < mHScrollBar.Location.Y
                || (mStripsPanel.Height <= this.VisibleHeight && mStripsPanel.Width > VisibleWidth * 3 && mStripsPanel.Width > 1000 ))
            {
                Strip currentlyActiveStrip = ActiveStrip;
                if (currentlyActiveStrip != null)
                {
                    Block lastBlock = currentlyActiveStrip.LastBlock;
                    if (lastBlock != null && lastBlock.Node != currentlyActiveStrip.Node.PhraseChild(currentlyActiveStrip.Node.PhraseChildCount - 1))
                    {
                        return true;
                    }
                }
            }
            return false;
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
                    mProjectView.Presentation.Changed  += new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_changed );
                    mProjectView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode> ( Presentation_RenamedSectionNode );
                    mProjectView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode> ( Presentation_UsedStatusChanged );
                    }
                else
                    {
                    mProjectView.Presentation.Changed  -= new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_changed );
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

        public void ClearCursor()
        {
            if (mPlaybackBlock != null) mPlaybackBlock.ClearCursor();
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
                m_RenderingWaveform = mWaveformRenderQ.Dequeue ();
                mWaveformRenderWorker = m_RenderingWaveform.Render ();
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
            m_IsWaveformRenderingPaused = false;
            }

        public bool IsWaveformRendering { get { return mWaveformRenderQ.Count > 0; } }

        public void FinishedRendering ( Waveform w, bool renderedOK )
            {
            mWaveformRenderWorker = null;
            if ( !m_IsWaveformRenderingPaused) RenderFirstWaveform ();
            if (!m_IsWaveformRenderingPaused) m_RenderingWaveform = null;
            }

        public void WaveformRendering_PauseOrResume(bool pause)
        {
            if (pause)
            {
                m_IsWaveformRenderingPaused = true;
                if (m_RenderingWaveform != null && m_RenderingWaveform.IsRenderingWaveform) m_RenderingWaveform.CancelRendering = true;
            }
            else
            {
                m_IsWaveformRenderingPaused = false;
                if(m_RenderingWaveform != null )  mWaveformRenderWorker = m_RenderingWaveform.Render();
                if (mWaveformRenderWorker != null)
                {
                    mProjectView.ObiForm.BackgroundOperation_Step();
                }
                //RenderFirstWaveform();
            }
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
        public NodeSelection SelectionFromStrip 
        { 
            set
            {
                if (mProjectView != null)
                {
                    mProjectView.Selection = value;
                    Console.WriteLine("Content View After mProjectView.Selection = value" + mProjectView.Selection);
                }
            }
        }

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

                    //@singleSection: removed if block
                        if (mSelectedItem != null) mSelectedItem.Highlighted = false;
                        NodeSelection previousSelection = mSelection;
                        mSelection = value;
                        mSelectedItem = s;

                    if (s != null)
                        {
                        s.SetSelectionFromContentView ( mSelection );
                        SectionNode section = value.Node is SectionNode ? (SectionNode)value.Node :
                            value.Node.ParentAs<SectionNode> ();
                        mProjectView.MakeTreeNodeVisibleForSection ( section );
                        if (mSelection != null && previousSelection != null && previousSelection.Node is EmptyNode && mSelection.Node is SectionNode
                            && previousSelection.Node.ParentAs<SectionNode>() == mSelection.Node)
                        {
                            // if section comes in selection after its own phrase, no neet to ensure its visibility.
                        }
                        else
                        {
                            EnsureControlVisible((Control)s);
                            UpdateScrollTrackBarAccordingToSelectedNode();
                        }
                        
                        mFocusing = true;
                        if (!((Control)s).Focused) ((Control)s).Focus ();
                        mFocusing = false;

                        // we do not need following condition for avoiding strip selection as it is handled in projectView.Selection disable section selection flag
                        //if (!(mSelectedItem is Strip) ) m_PreviousSelectionForScroll = null;//if section is not selected, it means that user has intentionally selected something else so selection should not restore
                        m_PreviousSelectionForScroll = null;
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
        
 void EnsureCursorVisible ( int x )
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
                //@singleSection: take care that strip label is not visible if first block is not first phrase of section
                int stripLabelOffset = 0;
                if (c is Strip || c is Block || c is StripCursor)
                    {
                    Strip c_Strip = c is Strip ? (Strip)c : 
                        c is Block? ((Block)c).Strip:
                        ((StripCursor)c).Strip;
                    if (c_Strip == null) return;
                    if (c_Strip.OffsetForFirstPhrase > 0) stripLabelOffset = c_Strip.BlocksLayoutTopPosition;
                    //Console.WriteLine ( "adjusting cordinates : " + stripLabelOffset );
                    }
                    int audioCursorPosition;
                    int selectionBeginPosition;
                    int selectionEndPosition;
                    int cursorPosition = GetSelectionOrAudioCursorLocationX(c, out audioCursorPosition, out selectionBeginPosition, out selectionEndPosition);

                // Compute the four corners of the control, including margins
                int top = location.Y - c.Margin.Top;
                if (top < stripLabelOffset) top = stripLabelOffset;//@singleSection
                int bottom = location.Y + c.Height + c.Margin.Bottom;
                //int left = location.X - c.Margin.Left + GetSelectionOrAudioCursorLocationX (c, out audioCursorPosition, out selectionBeginPosition, out selectionEndPosition);
                int left = location.X - c.Margin.Left ;
                // following if block added on oct 15, 2011
                if (cursorPosition > 0 && selectionEndPosition <= 0)
                {
                    int cursorPosContentView = left + cursorPosition;
                    if (cursorPosContentView < 0 || cursorPosContentView > verticalScrollToolStripContainer1.Location.X) left = cursorPosContentView;
                }
                int right =selectionEndPosition > 0? location.X + selectionEndPosition: location.X + c.Width + c.Margin.Right;
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
                int vw = verticalScrollToolStripContainer1.Location.X; //@singleSection : new

                
                // Vertical scrolling
                if (t < 0 || (b > vh && h > vh))
                    {
                    // Top of control is above the visible window, so scroll to the top
                    //@singleSection : we need to remove VScroll bar so this code should directly work on mStripsPanel
                    //mVScrollBar.Value = Math.Min ( top, v_max );
                        int cordY = Math.Min(top, v_max);
                        if (stripLabelOffset > 0 && cordY < stripLabelOffset) cordY = stripLabelOffset;
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                        cordY * -1 );
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
                        int cordY = Math.Min(bottom - vh, v_max);
                        if (stripLabelOffset > 0 && cordY < stripLabelOffset) cordY = stripLabelOffset;
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                        cordY * -1 );
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

        private int GetSelectionOrAudioCursorLocationX(Control c, out int audioCursorPosition, out int selectionBeginPosition, out int selectionEndPosition)
        {
            audioCursorPosition = -1;
            selectionBeginPosition = -1;
            selectionEndPosition = -1;
            if (c != null && c is AudioBlock)
            {
                AudioBlock block = (AudioBlock)c;
                
                block.GetLocationXForAudioCursorAndSelection(out audioCursorPosition, out selectionBeginPosition, out selectionEndPosition);

                if (audioCursorPosition == -1 && selectionBeginPosition > 0 )
                {
                    return selectionBeginPosition;
                    }
                    else if (audioCursorPosition > 0 && selectionBeginPosition == -1 )
                    {
                        return audioCursorPosition;
                    }
                    else if (audioCursorPosition > 0 && selectionBeginPosition > 0)
                    {
                        return audioCursorPosition < selectionBeginPosition ? audioCursorPosition : selectionBeginPosition;
                    }
            }
            return 0;
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
                command = mProjectView.Presentation.CommandFactory.CreateCompositeCommand ();
                command.ShortDescription = Localizer.Message ( "split_section" ) ;

                Commands.UpdateSelection initialSelection = new Commands.UpdateSelection ( mProjectView, new NodeSelection ( node, this ) ) ;
                initialSelection.RefreshSelectionForUnexecute = true;
                command.ChildCommands.Insert (command.ChildCommands.Count, initialSelection );
                // Add a sibling with a new label
                SectionNode sibling = mProjectView.Presentation.CreateSectionNode ();
                sibling.Label = section.Label + "*";
                Commands.Node.AddNode add = new Commands.Node.AddNode ( mProjectView, sibling, section.ParentAs<ObiNode> (),
                    section.Index + 1 );
                add.UpdateSelection = false;
                add.ProgressPercentage = 0;
                command.ChildCommands.Insert (command.ChildCommands.Count,  add );
                if (!section.Used) command.ChildCommands.Insert(command.ChildCommands.Count,new Commands.Node.ToggleNodeUsed(mProjectView,sibling));

                // Change parents of children to insert the section at the right position in strip order
                for (int i = section.SectionChildCount - 1; i >= 0; --i)
                    {
                    command.ChildCommands.Insert (command.ChildCommands.Count, new Commands.Node.Delete ( mProjectView, section.SectionChild ( i ), false ) );
                    }
                for (int i = 0; i < section.SectionChildCount; ++i)
                    {
                    command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.Node.AddNode ( mProjectView, section.SectionChild ( i ), sibling, i, false ) );
                    }
                // Split the node if necessary
                PhraseNode splitNode = null;
                PhraseNode cropNode = null;
                if (mProjectView.CanSplitPhrase)
                    {
                    urakawa.command.Command splitCommand = Commands.Node.SplitAudio.GetSplitCommand ( mProjectView );
                    if (splitCommand != null) command.ChildCommands.Insert (command.ChildCommands.Count,  splitCommand );
                    splitNode = Commands.Node.SplitAudio.GetSplitNode ( splitCommand );
                    //@singleSection  work around to avoid triggering strip creation due to unknown selection of split phrase
                    if (splitNode != null) command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.UpdateSelection ( mProjectView, new NodeSelection ( splitNode, this ) ) );
                    if (splitNode != null) cropNode = Commands.Node.SplitAudio.GetCropNode ( splitCommand, splitNode );
                    }
                // Move children from the context phrase to the new sibling
                int sectionOffset = node.Index + (splitNode != null ? 1 : 0);
                int progressPercent = 0;
                int progressInterval = (section.PhraseChildCount - sectionOffset) > 40 ? (section.PhraseChildCount - sectionOffset) / 40 : 1;
                for (int i = section.PhraseChildCount - 1; i >= sectionOffset; --i)
                    {
                    Commands.Command delete = new Commands.Node.Delete ( mProjectView, section.PhraseChild ( i ), false );

                    if (i % progressInterval == 0 && progressPercent < 100) delete.ProgressPercentage = ++progressPercent;

                    command.ChildCommands.Insert (command.ChildCommands.Count,  delete );
                    }
                progressInterval = 45;
                if (cropNode != null) command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.Node.Delete ( mProjectView, cropNode, section, node.Index + 2, false ) );
                if (splitNode != null)
                    {
                    command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.Node.Delete ( mProjectView, splitNode, section, node.Index + 1, false ) );
                    command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.Node.AddNode ( mProjectView, splitNode, sibling, 0, false ) );
                    }
                if (cropNode != null) command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.Node.AddNode ( mProjectView, cropNode, sibling, 1, false ) );
                int siblingOffset = node.Index - (cropNode != null ? 1 : 0);

                progressInterval = (section.PhraseChildCount - sectionOffset) > 45 ? (section.PhraseChildCount - sectionOffset) * 2 / 45 : 2;//multiplied by 2 to report progress with increment of 2
                for (int i = sectionOffset; i < section.PhraseChildCount; ++i)
                    {
                    Commands.Command addCmd = new
                        Commands.Node.AddNode ( mProjectView, section.PhraseChild ( i ), sibling, i - siblingOffset, false );
                    if (i % progressInterval == 0 && progressPercent < 98) addCmd.ProgressPercentage = progressPercent += 2;
                    command.ChildCommands.Insert (command.ChildCommands.Count, addCmd );
                    }
                progressPercent = 100;
                Commands.Command updateSelectionCmd = new Commands.UpdateSelection ( mProjectView, new NodeSelection ( sibling, this ) );
                updateSelectionCmd.ProgressPercentage = progressPercent;
                command.ChildCommands.Insert (command.ChildCommands.Count,  updateSelectionCmd );
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

        public void UpdateCursorPosition(double time)
        {
            if (mProjectView.TransportBar.PreviewBeforeRecordingActive)
            {
                ColorFlicker(time);
            }
            else if (m_ColorBackgroundBeforeFlicker.Name != "0" && m_ColorBackgroundBeforeFlicker.Name != mProjectView.ColorSettings.BlockBackColor_Selected.Name)
            {
                mPlaybackBlock.ColorSettings.BlockBackColor_Selected = m_ColorBackgroundBeforeFlicker;
               // m_ColorBackgroundBeforeFlicker.Name = "0";
            }        
          
            if (m_ZoomWaveformPanel != null && mProjectView.TransportBar.IsPlayerActive)//@zoomwaveform
            {
                m_ZoomWaveformPanel.UpdateCursorTime(time);
                return;
            }

            if (PlaybackBlock == null && m_EnableFindPlaybackBlockDuringCursorUpdate && mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)//@singleSection
            {
                m_EnableFindPlaybackBlockDuringCursorUpdate = false;
                SetPlaybackPhraseAndTime(mProjectView.TransportBar.CurrentPlaylist.CurrentPhrase, mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset);
            }
            if (mPlaybackBlock != null) EnsureCursorVisible(mPlaybackBlock.UpdateCursorTime(time));

            /* int audioCursorPosition;
             int beginPosition = 0;
             int endPosition = 0;
             int getCursorPosition = GetSelectionOrAudioCursorLocationX(mPlaybackBlock,out audioCursorPosition, out beginPosition,out endPosition);
             int horizontalSizeLeftInStripPanel = audioCursorPosition - (mStripsPanel.Location.X - this.Location.X);
             int cursorPositionInContentView = this.Size.Width - horizontalSizeLeftInStripPanel - mVScrollBar.Size.Width;
             Console.WriteLine(" M - NEW - SIZE " + cursorPositionInContentView);
             if (cursorPositionInContentView < this.Size.Width) EnsureControlVisible(mPlaybackBlock);
             */
        }

        private void ColorFlicker(double time)
        {
            mProjectView.Selection = new NodeSelection(mPlaybackBlock.Node, this);
            if (m_timeElapsed == 0)
            {
                m_timeElapsed = time;
                m_ColorBackgroundBeforeFlicker = mProjectView.ObiForm.Settings.ColorSettings.BlockBackColor_Selected;
            }

            if (mPlaybackBlock.ColorSettings.BlockBackColor_Selected == Color.Red)
            {
                mPlaybackBlock.ColorSettings.BlockBackColor_Selected = m_ColorBackgroundBeforeFlicker;
            }
            else
            {
                mPlaybackBlock.ColorSettings.BlockBackColor_Selected = Color.Red;
            }
            //  m_timeElapsed = time;

            if (!mProjectView.TransportBar.IsPlayerActive)
            {
                //mPlaybackBlock.ColorSettings.BlockBackColor_Selected = m_ColorBackgroundBeforeFlicker;
                //m_timeElapsed = 0;
                //mProjectView.TransportBar.PreviewBeforeRecordingActive = false;
            }
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
                    MessageBox.Show (Localizer.Message("ContentView_wrap"));
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
                    RemoveStripsForSection_Safe ( selectedSection );
                    AddStripForSection_Safe ((ObiNode)  mProjectView.Presentation.RootNode );
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
                    if ((selectedSection == null && i == 0)
                        || (selectedSection != null && strip.Node == selectedSection))
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
                    Strip currentlyActiveStrip = ActiveStrip;
                    int previousBlockLayoutTopPosition = currentlyActiveStrip != null ? currentlyActiveStrip.BlocksLayoutTopPosition : -1;//this cannot be negative for right value
                ClearWaveformRenderQueue ();
                foreach (Control c in mStripsPanel.Controls) if (c is Strip) ((Strip)c).ZoomFactor = value;
                UpdateSize ();
                // reposition strips panel so that zoom is from 0 position of content view and not from 0 position of stripspanel
                if (contentViewLabel1 != null && contentViewLabel1.zoomFactor > 0 &&  mStripsPanel.Location.Y < 0)
                {
                    if (ActiveStrip != null && previousBlockLayoutTopPosition == Math.Abs(mStripsPanel.Location.Y))
                    {
                        mStripsPanel.Location = new Point(mStripsPanel.Location.X, -(currentlyActiveStrip.BlocksLayoutTopPosition));
                    }
                    else
                    {
                        int cordY = mStripsPanel.Location.Y;
                        cordY = Convert.ToInt16((cordY * value) / contentViewLabel1.zoomFactor);
                        mStripsPanel.Location = new Point(mStripsPanel.Location.X, (cordY - 1));
                    }
                                    }
                this.contentViewLabel1.contentView = this;
                this.contentViewLabel1.zoomFactor = ZoomFactor;
                this.waveform_recording_control.contentView = this;   //@Onthefly
                this.waveform_recording_control.zoomFactor = ZoomFactor;   //@Onthefly
                mHScrollBar.Location = new Point ( mHScrollBar.Location.X, this.Height - contentViewLabel1.Height - mHScrollBar.Height );
                mVScrollBar.Height = mVScrollBar.Location.Y + this.Height - contentViewLabel1.Height - mHScrollBar.Height;
                mCornerPanel.Location = new Point ( mCornerPanel.Location.X, this.Height - contentViewLabel1.Height - mHScrollBar.Height );
                waveform_recording_control.Location = new Point(waveform_recording_control.Location.X, this.Height - contentViewLabel1.Height - mHScrollBar.Height - waveform_recording_control.Height);  //@Onthefly
                // ensure visibility of selected node
                if (mProjectView != null &&  mProjectView.Selection != null && (mProjectView.Selection is StripIndexSelection || mProjectView.Selection.Node is EmptyNode))
                    {
                    EmptyNode currentlySelectedEmptyNode = mProjectView.Selection is StripIndexSelection && ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection != null? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection :
                        mProjectView.Selection.Node is EmptyNode ? (EmptyNode)mProjectView.Selection.Node : null;

                    Block currentlySelectedBlock = null;
                    if (currentlySelectedEmptyNode == null && mProjectView.Selection is StripIndexSelection && currentlyActiveStrip != null)
                    {
                        currentlySelectedBlock = currentlyActiveStrip.BlockBefore(mSelectedItem);
                    }
                    else
                    {
                        currentlySelectedBlock = FindBlock(currentlySelectedEmptyNode) ;
                    }
                    if (currentlySelectedBlock != null) EnsureControlVisible ( currentlySelectedBlock );
                    }
                    //@zoomwaveform
                    if (m_ZoomWaveformPanel != null)
                    {
                        m_ZoomWaveformPanel.ZoomFactor = value;
                    }

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

            mStripsPanel.Controls.Clear ();
            }

        // @phraseLimit
        private int VisibleBlocksCount
            {
            get
                {
                int count = 0;
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip)
                        {
                        count += ((Strip)c).Node.PhraseChildCount;
                        }
                    }
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
                    //if (s.Node.PhraseChildCount <= mProjectView.MaxVisibleBlocksCount - VisibleBlocksCount
                        //&& prevPoint != s.Location
                        //&& visibleStripsCount <= 500)
                        //{
                        CreateBlocksInStrip ( s );// uncomment for prev block loading
                                                visibleStripsCount++;
                                                break;
                        //}
                    //else return;

                    prevPoint = s.Location;
                    }
                }

            }

        //@singleSection
        private void CreateSelectedStripAndPhraseBlocks ( NodeSelection selectionValue )
            {
            if (selectionValue == null || selectionValue.Node == null || !selectionValue.Node.IsRooted) return;

            // if selection restore phrase lie in next phrase lot but phrase is alreaty created with lot before, no need to refresh screen in this case
            // the calling code should check if the phrase is created or not
            if (m_DisablePhraseCreationWhileSelectionRestore) return;

            // explicitly handle audio cursor selection, will add universal approach later.
            if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing &&  mSelection != null && selectionValue.Node is PhraseNode && selectionValue.Node == mSelection.Node && selectionValue is AudioSelection) return;

            // it is important to prevent creation of initial phrases when the section is selected after phrase selection
            //  this will avoid creation of useless intermediate phrases
            // but need to explore negative effects.
            Strip currentlyActiveStrip = null;
            if (mSelection != null && selectionValue != null )
            {
                EmptyNode previouslySelectedNode = mSelection.Node is EmptyNode?(EmptyNode) mSelection.Node:
                    mSelection is StripIndexSelection && ((StripIndexSelection)mSelection).EmptyNodeForSelection != null?((StripIndexSelection)mSelection).EmptyNodeForSelection: null ;
                if (previouslySelectedNode != null &&  (selectionValue.Node is SectionNode && !(selectionValue is StripIndexSelection))
            && previouslySelectedNode.ParentAs<SectionNode>() == selectionValue.Node
                    && (currentlyActiveStrip = ActiveStrip) != null && currentlyActiveStrip.Node == selectionValue.Node)
                {
                    return;
                }
            }           

            if (selectionValue.Node is SectionNode ||
                selectionValue.Node is EmptyNode ||
                selectionValue is StripIndexSelection)
                {
                if(currentlyActiveStrip == null)  currentlyActiveStrip = ActiveStrip;
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
                                //Console.WriteLine ( "the required strip exists " + iterationStrip.Node.Label );
                                }
                            else
                                {
                                RemoveStripsForSection_Safe ( iterationStrip.Node );
                                }
                            }
                        }
                    }
                if (requiredExistingStrip != null)
                    {
                    if (selectionValue.Node is EmptyNode || selectionValue is StripIndexSelection)
                        {
                        EmptyNode eNode = selectionValue is StripIndexSelection ? (((StripIndexSelection)selectionValue).Index < requiredExistingStrip.Node.PhraseChildCount ? ((StripIndexSelection)selectionValue).EmptyNodeForSelection :
                            requiredExistingStrip.Node.PhraseChild ( requiredExistingStrip.Node.PhraseChildCount - 1 )) :
                                (EmptyNode)selectionValue.Node;
                        CreateLimitedBlocksInStrip ( requiredExistingStrip, eNode );
                        }
                    return;
                    }

                if (currentlyActiveStrip == null
                    || (currentlyActiveStrip != null
                    && sectionToBeSelected != currentlyActiveStrip.Node))
                    {
                    currentlyActiveStrip = CreateStripForSelectedSection ( sectionToBeSelected,
                                                true );


                    }
                if (selectionValue.Node is EmptyNode || selectionValue is StripIndexSelection)
                    {
                    //if (currentlyActiveStrip == null) MessageBox.Show ( "active is null " );
                    //if (selectionValue is StripIndexSelection ) MessageBox.Show ("selection is  " + (( StripIndexSelection) selectionValue).Index.ToString () );
                        
                    EmptyNode eNode = selectionValue is StripIndexSelection ? (((StripIndexSelection)selectionValue).Index < currentlyActiveStrip.Node.PhraseChildCount ? ((StripIndexSelection)selectionValue).EmptyNodeForSelection :
                        currentlyActiveStrip.Node.PhraseChildCount > 0? currentlyActiveStrip.Node.PhraseChild ( currentlyActiveStrip.Node.PhraseChildCount - 1 ): null) :
                                (EmptyNode)selectionValue.Node;
                    CreateLimitedBlocksInStrip ( currentlyActiveStrip, eNode );
                    // enable upper toolstrip in case first block is not 0 index, this is because new strip is created with top toolstrip disabled
                    if (currentlyActiveStrip.OffsetForFirstPhrase > 0) verticalScrollToolStripContainer1.CanScrollUp = true;
                    }
                else if (currentlyActiveStrip != null && selectionValue.Node is SectionNode
                    && currentlyActiveStrip.FirstBlock == null && currentlyActiveStrip.Node.PhraseChildCount > 0)
                    {
                    CreateLimitedBlocksInStrip ( currentlyActiveStrip, null );
                    // enable upper toolstrip in case first block is not 0 index, this is because new strip is created with top toolstrip disabled
                    if (currentlyActiveStrip.OffsetForFirstPhrase > 0) verticalScrollToolStripContainer1.CanScrollUp = true;
                    }

                }

            }

        private delegate Strip CreateStripForAddedSectionNodeInvokation ( SectionNode node, bool removeExisting );

        public Strip CreateStripForAddedSectionNode ( SectionNode node, bool removeExisting )//@singleSection
            {
            if (ActiveStrip == null)
                {
                if (InvokeRequired)
                    {
                    return (Strip)Invoke ( new CreateStripForAddedSectionNodeInvokation ( CreateStripForAddedSectionNode ), node, removeExisting );
                    }
                else
                    {
                    return CreateStripForSelectedSection ( node, removeExisting );
                    }
                }
            else
                {
                return null;
                }
            }


        public Strip CreateStripForSelectedSection ( SectionNode node, bool removeExisting )//@singleSection
            {
            //if (IsStripVisible ( node )) return null;
            if (node == null ||  !node.IsRooted) return null;

            //Check for recording, return if section to be created is not recording section
            if (mProjectView.TransportBar.CurrentState == TransportBar.State.Recording && mProjectView.TransportBar.RecordingPhrase != null)
                {

                //if (mProjectView.TransportBar.RecordingPhrase.ParentAs<SectionNode> () == node) return null;
                }
   
            // first remove existing strip
            if (removeExisting)
                {
                Strip requiredExistingStrip = null;
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip && ((Strip)c).Node == node)
                        {
                        requiredExistingStrip = (Strip)c;
                        }
                    else if (c is Strip)
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
            mStripsPanel.Location = new Point ( 0, 0 );
            verticalScrollToolStripContainer1.CanScrollUp = false;
            //Console.WriteLine("disabling upper toolstrip during initializing section");
            bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
            Strip newStrip = AddStripForSection ( node );
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
            return newStrip;
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
                int phraseLotSize = PhraseCountInLot ( stripControl, true );
                try
                    {
                    //check for recording restriction
                    if (RestrictDynamicLoadingForRecording ( stripControl.Node ) && stripControl.FindBlock ( mProjectView.TransportBar.RecordingPhrase ) != null) return false;

                    if (mProjectView.Selection == null ||
                        stripControl.FirstBlock == null || //this means that no block is created in strip
                        (mProjectView.Selection != null &&
                        !(mProjectView.Selection.Node is EmptyNode) &&
                        !(mProjectView.Selection is StripIndexSelection)))
                        {
                        if (requiredEmptyNode != null)
                            {
                            Block requiredBlock = stripControl.FindBlock ( requiredEmptyNode );
                            if (requiredBlock == null)
                                {
                                CreateBlocksTillNodeInStrip ( stripControl, requiredEmptyNode, false );
                                shouldRemoveBlocks = false;
                                }
                            }
                        else
                            {
                            // check if block for defaultBlockCount index is there
                            Block v = stripControl.FindBlock ( stripControl.Node.PhraseChildCount < defaultVisibleCount ? stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 ) :
        stripControl.Node.PhraseChild ( defaultVisibleCount - 1 ) );

                            if (v == null || !stripControl.IsContentViewFilledWithBlocks)
                                {
                                shouldRemoveBlocks = false;
                                int maxCount = stripControl.Node.PhraseChildCount < defaultVisibleCount ? stripControl.Node.PhraseChildCount : defaultVisibleCount;
                                bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                                // pause playback if it is active.
                                if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                                    {
                                    mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                                    wasPlaybackOn = true;
                                    mProjectView.TransportBar.Pause ();
                                    }
                                for (int i = 0; i < phraseLotSize && !stripControl.IsContentViewFilledWithBlocks; ++i)
                                    {
                                    if ((maxCount < defaultVisibleCount && i >= maxCount)
                                        || i >= stripControl.Node.PhraseChildCount)
                                        {
                                        Console.WriteLine ( "Adding block stopped at " + i.ToString () );
                                        //MessageBox.Show ( maxCount.ToString ()  );
                                        break;
                                        }
                                    stripControl.AddBlockForNode ( stripControl.Node.PhraseChild ( i ) );
                                    }
                                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
                                }


                            }
                        }
                    else
                        {
                        ObiNode selectedNode = null;
                        if (mProjectView.Selection is StripIndexSelection)
                            {
                            selectedNode = ((StripIndexSelection)mProjectView.Selection).Index < stripControl.Node.PhraseChildCount ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection :
                               stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 );

                            }
                        if (selectedNode == null) selectedNode = mProjectView.Selection.Node;

                        Block lastBlockInStrip = stripControl.LastBlock;
                        if (lastBlockInStrip != null
                            && lastBlockInStrip.Node.IsRooted
                            && selectedNode.IsRooted
                                                        && ((lastBlockInStrip.Node.Index - selectedNode.Index >= 15
                                                        && requiredEmptyNode == null)
                                || (lastBlockInStrip.Node == stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 ) && requiredEmptyNode == null)))
                            {
                            shouldRemoveBlocks = true;
                            }
                        else if (selectedNode != null && selectedNode.IsRooted)
                            {//2
                            //Console.WriteLine ( "required node " + requiredEmptyNode );
                            if (requiredEmptyNode != null && lastBlockInStrip != null && lastBlockInStrip.Node.IsRooted)
                                {
                                if (lastBlockInStrip.Node.Index < requiredEmptyNode.Index || stripControl.OffsetForFirstPhrase > requiredEmptyNode.Index)
                                    CreateBlocksTillNodeInStrip ( stripControl, requiredEmptyNode, false );
                                }
                            //ObiNode currentNode = selectedNode.FollowingNode; // lets start from selected node
                            ObiNode currentNode = selectedNode;

                            bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                            
                            ObiNode nodeForRemoveReference = requiredEmptyNode != null && requiredEmptyNode.Index > selectedNode.Index ? requiredEmptyNode : selectedNode;
                            EmptyNode intendedFirstNodeAfterRemoval = null;
                            Block firstBlock = stripControl.FirstBlock;
                            // do not allow removal of block layout till phrase to be selected is  100 more than last phrase index of lot
                            // this will prevent refreshing of layout on clicking phrase beyond 250 phrases lot size. But a better way is preffered than this.
                            if (firstBlock != null && nodeForRemoveReference.Index - firstBlock.Node.Index >= phraseLotSize + 100)
                                {
                                    // pause playback if it is active.
                                    if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                                    {
                                        mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                                        wasPlaybackOn = true;
                                        mProjectView.TransportBar.Pause();
                                    }
                                intendedFirstNodeAfterRemoval = RemoveAllblocksInStripIfRequired ( stripControl, nodeForRemoveReference, true );
                                }
                            if (intendedFirstNodeAfterRemoval != null)
                                {
                                int intermediateBlocksCount = selectedNode.Index - intendedFirstNodeAfterRemoval.Index;
                                //Console.WriteLine ( "selection removal : extra block : intermediate count " + extraBlocksCount + " " + intermediateBlocksCount );
                                extraBlocksCount += intermediateBlocksCount;
                                currentNode = intendedFirstNodeAfterRemoval;
                                }

                            for (int i = 0; (i < extraBlocksCount || !stripControl.IsContentViewFilledWithBlocks ) && i < 350; i++)
                                {//3
                                if (currentNode == null ||
                                    !(currentNode is EmptyNode) ||
                                    currentNode.ParentAs<SectionNode> () != stripControl.Node)
                                    {//4
                                    Console.WriteLine ( "Adding extra blocks exit at " + i.ToString () );
                                    break;
                                    }//-4

                                Block currentNodeBlock = stripControl.FindBlock ( (EmptyNode)currentNode );
                                if (currentNodeBlock == null)
                                    {//4
                                        // pause playback if it is active.
                                        if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                                        {
                                            mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                                            wasPlaybackOn = true;
                                            mProjectView.TransportBar.Pause();
                                        }
                                    shouldRemoveBlocks = false;
                                    stripControl.AddBlockForNode ( (EmptyNode)currentNode );
                                    }//-4
                                currentNode = currentNode.FollowingNode;
                                }//-3
                            SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(stripControl, false);
                            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
                            }//-2

                        UpdateSize ();
                        if (!shouldRemoveBlocks) stripControl.UpdateColors ();
                        }

                        if (shouldRemoveBlocks)
                        {
                            if (mProjectView.Selection != null && mProjectView.Selection.Node.IsRooted
                                && (mProjectView.Selection.Node is EmptyNode || mProjectView.Selection is StripIndexSelection))
                            {
                                ObiNode currentPhraseNode = mProjectView.Selection is StripIndexSelection ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection :
                                    mProjectView.Selection.Node;

                                int currentPhraseIndex = (currentPhraseNode != null && currentPhraseNode.IsRooted) ? currentPhraseNode.Index : -1;
                                if (currentPhraseIndex == -1) return true;

                                if (mSelection != null && (mSelection.Node is EmptyNode || mSelection is StripIndexSelection)
                                    && (mProjectView.Selection.Node.IsSiblingOf(mSelection.Node) || mProjectView.Selection.Node == mSelection.Node))
                                {
                                    int contentViewSelectionIndex = mSelection is StripIndexSelection ? (((StripIndexSelection)mSelection).EmptyNodeForSelection != null ? ((StripIndexSelection)mSelection).EmptyNodeForSelection.Index : ((StripIndexSelection)mSelection).Index - 1) :
                                        mSelection.Node.Index;
                                    if (currentPhraseIndex < contentViewSelectionIndex) currentPhraseIndex = contentViewSelectionIndex;

                                }
                                if (requiredEmptyNode != null && currentPhraseIndex < requiredEmptyNode.Index)
                                {
                                    currentPhraseIndex = requiredEmptyNode.Index;
                                }
                                // replace following return condition by bypass type if block
                                //if (stripControl.Node.PhraseChildCount <= currentPhraseIndex + 15) return true;
                                if (stripControl.Node.PhraseChildCount > currentPhraseIndex + 15)
                                {
                                    if (currentPhraseIndex <= defaultVisibleCount) currentPhraseIndex = defaultVisibleCount - 1;

                                    //System.Media.SystemSounds.Asterisk.Play ();
                                    EmptyNode lastIntentedVisiblePhrase = stripControl.Node.PhraseChildCount > currentPhraseIndex + 15 ? stripControl.Node.PhraseChild(currentPhraseIndex + 15) :
                                        stripControl.Node.PhraseChild(stripControl.Node.PhraseChildCount - 1);


                                    if (stripControl.IsContentViewFilledWithBlocks)
                                    {
                                        // pause playback if it is active.
                                        if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                                        {
                                            mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                                            wasPlaybackOn = true;
                                            mProjectView.TransportBar.Pause();
                                        }
                                        stripControl.RemoveAllFollowingBlocks(lastIntentedVisiblePhrase, true, false);
                                        UpdateSize();
                                    }
                                }
                            }
                        }
                    }
                catch (System.Exception ex)
                    {
                    mProjectView.WriteToLogFile(ex.ToString());
                    MessageBox.Show ( ex.ToString () );
                    }
                if (wasPlaybackOn)
                    {
                    //mProjectView.TransportBar.PlayOrResume ();
                        ResumePlaybackAfterDynamicLoading();
                    //SetPlaybackPhraseAndTime ( mProjectView.TransportBar.CurrentPlaylist.CurrentPhrase, mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                    m_EnableFindPlaybackBlockDuringCursorUpdate = true;
                    }
                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = canMoveSelectionToPlaybackPhrase;
                return true;

                }

            return true;
            }

        //@singleSection
        private void ResumePlaybackAfterDynamicLoading()
        {
            if ( mProjectView.TransportBar.CurrentState == TransportBar.State.Paused )
            {
                if (mProjectView.TransportBar.CurrentPlaylist == mProjectView.TransportBar.MasterPlaylist)
                {
                    mProjectView.TransportBar.PlayAll();
                }
                else
                {
                    mProjectView.TransportBar.PlayOrResume();
                }
            }
        }

        //@singleSection
        private void RemoveBlocksBelowContentViewVisibleArea ( EmptyNode nodeSelected )
            {
            if (mProjectView.Selection == null) return;

            ObiNode currentlySelectedNode = mProjectView.Selection is StripIndexSelection ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection : mProjectView.Selection.Node;

            if (currentlySelectedNode == null
                || (currentlySelectedNode != null && nodeSelected != null
                && nodeSelected.Index > currentlySelectedNode.Index))
                {
                currentlySelectedNode = nodeSelected;
                }

            //Console.WriteLine ( "currently selected node while removal " + currentlySelectedNode );
            if (currentlySelectedNode != null && currentlySelectedNode is SectionNode)
                {
                if (((SectionNode)currentlySelectedNode).PhraseChildCount > 0)
                    {
                    currentlySelectedNode = currentlySelectedNode.PhraseChildCount > 40 ? ((SectionNode)currentlySelectedNode).PhraseChild ( 40 ) :
                    currentlySelectedNode.PhraseChild ( currentlySelectedNode.PhraseChildCount - 1 );
                    }
                else
                    {
                    return;
                    }
                }
            if (currentlySelectedNode == null || (currentlySelectedNode != null && !currentlySelectedNode.IsRooted)) return;

            Strip stripControl = FindStrip ( currentlySelectedNode.ParentAs<SectionNode> () );

            if (stripControl != null && stripControl.IsContentViewFilledWithBlocks && !RestrictDynamicLoadingForRecording ( stripControl.Node ))
                {

                bool wasPlaybackOn = false;
                bool canMoveSelectionToPlaybackPhrase = mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase;
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
                    //Console.WriteLine ( "remove explicitly  atmost till " + lastIntentedVisiblePhrase );
                    }
                catch (System.Exception ex)
                    {
                    mProjectView.WriteToLogFile(ex.ToString());
                    MessageBox.Show (Localizer.Message("Operation_Cancelled") + "\n\n" + ex.ToString () );
                    }

                UpdateSize ();

                if (wasPlaybackOn)
                    {
                    //mProjectView.TransportBar.PlayOrResume ();
                        ResumePlaybackAfterDynamicLoading();
                    //SetPlaybackPhraseAndTime ( mProjectView.TransportBar.CurrentPlaylist.CurrentPhrase, mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                    m_EnableFindPlaybackBlockDuringCursorUpdate = true;
                    }
                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = canMoveSelectionToPlaybackPhrase;
                }
            }

        //@singleSection
        public void CreateBlocksTillNodeInStrip ( Strip stripControl, EmptyNode nodeOfLastBlockToCreate, bool considerStripHaltFlag )
            {
            CreateBlocksTillNodeInStrip ( stripControl, nodeOfLastBlockToCreate, considerStripHaltFlag, 0 );
            }

        //@singleSection
        public void CreateBlocksTillNodeInStrip ( Strip stripControl, EmptyNode nodeOfLastBlockToCreate, bool considerStripHaltFlag, int pixelDepth )
            {
            Block firstBlock = stripControl.FirstBlock;
            Block lastBlock = stripControl.LastBlock;
            if ((firstBlock != null && lastBlock != null)
                || (stripControl.Node.PhraseChildCount > 0 && nodeOfLastBlockToCreate.Index < stripControl.Node.PhraseChildCount))
                {
                //recording restriction check
                if (RestrictDynamicLoadingForRecording ( stripControl.Node ) && stripControl.FindBlock ( mProjectView.TransportBar.RecordingPhrase ) != null) return;

                int phraseLotSize = PhraseCountInLot ( stripControl, true ); //used only in case when strip has no blocks
                int startThreshold = Convert.ToInt32 ( nodeOfLastBlockToCreate.Index / phraseLotSize ) * phraseLotSize;//used only in case when strip has no blocks
                EmptyNode startNode = lastBlock != null ? lastBlock.Node : stripControl.Node.PhraseChild ( startThreshold );
                int startNodeIndex = firstBlock != null ? firstBlock.Node.Index : startThreshold;

                if (nodeOfLastBlockToCreate == null) nodeOfLastBlockToCreate = stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 );
                EmptyNode firstNodeAfterRemove = RemoveAllblocksInStripIfRequired ( stripControl,
                    nodeOfLastBlockToCreate,
                    firstBlock != null && nodeOfLastBlockToCreate.Index >= firstBlock.Node.Index ? true : false );

                if (firstNodeAfterRemove != null || firstBlock == null)
                    {
                    if ((firstBlock == null || firstNodeAfterRemove.Index < firstBlock.Node.Index)
                        && stripControl.DisplayPreviousLayout ( nodeOfLastBlockToCreate ))
                        {
                        UpdateSize ();

                        return;
                        }
                    else if (firstNodeAfterRemove != null)
                        {
                        startNode = firstNodeAfterRemove;
                        startNodeIndex = firstNodeAfterRemove.Index;
                        //Console.WriteLine ( "Start node aftger removal " + startNode.Index );
                        }
                    }

                bool wasPlaybackOn = false;
                bool canMoveSelectionToPlaybackPhrase = mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase;
                bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                    {
                    mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                    wasPlaybackOn = true;
                    mProjectView.TransportBar.Pause ();
                    }
                if (!considerStripHaltFlag && pixelDepth == 0)
                    {
                    // add some extra blocks to avoid irregular look at bottom of strip
                    int intendedLastNodeIndex = nodeOfLastBlockToCreate.Index + 10 >= stripControl.Node.PhraseChildCount ? stripControl.Node.PhraseChildCount - 1 :
                            nodeOfLastBlockToCreate.Index + 10;
                    nodeOfLastBlockToCreate = stripControl.Node.PhraseChild ( intendedLastNodeIndex );

                    stripControl.AddsRangeOfBlocks ( startNode, nodeOfLastBlockToCreate );
                    startNode = nodeOfLastBlockToCreate;
                    startNodeIndex = nodeOfLastBlockToCreate.Index;
                    }
                // start from beginning and create blocks for nodes for after the last block node.
                bool shouldStartCreating = stripControl.Node.PhraseChild ( startNodeIndex ) == startNode ? true : false;
                for (int i = startNodeIndex; i < stripControl.Node.PhraseChildCount; i++)
                    {
                    //System.Media.SystemSounds.Asterisk.Play ();
                    if (considerStripHaltFlag && stripControl.IsContentViewFilledWithBlocks
                        && (i % 5 == 0 || i <= 1))
                        {
                        //Console.WriteLine ( "block creation quit index for scroll " + i.ToString () );
                        break;
                        }
                    else if (pixelDepth > 0 && stripControl.IsContentViewFilledWithBlocksTillPixelDepth ( pixelDepth ))
                        {
                        //Console.WriteLine ( "block creation quit index for scroll for pixcel depth" + i + " depth " + pixelDepth );
                        break;
                        }

                    EmptyNode node = stripControl.Node.PhraseChild ( i );
                    if (shouldStartCreating)
                        {
                        stripControl.AddBlockForNode ( node );
                        }

                    if (node != null && node == nodeOfLastBlockToCreate)
                        {
                        // if node is null then keep on creating block till end of strip
                        if (node.Index != stripControl.Node.PhraseChildCount - 1 && firstBlock != null && node.Index - firstBlock.Node.Index < 350
                            && !considerStripHaltFlag && pixelDepth > 0)
                            {
                            int nextLastIndex = node.Index + 100 >= stripControl.Node.PhraseChildCount ? stripControl.Node.PhraseChildCount - 1 :
                                node.Index + 100;
                            nodeOfLastBlockToCreate = stripControl.Node.PhraseChild ( nextLastIndex );
                            }
                        else
                            {

                            break;
                            }
                        }

                    if (node == startNode)
                        {
                        shouldStartCreating = true;
                        }
                    }
                UpdateSize ();
                stripControl.UpdateColors ();
                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = canMoveSelectionToPlaybackPhrase;
                if (wasPlaybackOn)
                    {
                    // if IScrollActive flag is true, it will not allow playback
                    bool isScrollActiveStatus = IsScrollActive;
                    if (IsScrollActive) IsScrollActive = false;
                    //mProjectView.TransportBar.PlayOrResume ();
                    ResumePlaybackAfterDynamicLoading();
                    IsScrollActive = isScrollActiveStatus;
                    }
                }
            }

        public void CreatePhraseBlocksForFillingContentView ( Strip stripControl )
            {
            if (stripControl.IsContentViewFilledWithBlocks) return;
            Block lastBlock = stripControl.LastBlock;
            if (lastBlock != null)
                {
                bool wasPlaybackOn = false;
                bool canMoveSelectionToPlaybackPhrase = mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase;
                if (mProjectView.TransportBar.CurrentState == TransportBar.State.Playing)
                    {
                    mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = false;
                    wasPlaybackOn = true;
                    mProjectView.TransportBar.Pause ();
                    }
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
                mProjectView.TransportBar.CanMoveSelectionToPlaybackPhrase = canMoveSelectionToPlaybackPhrase;
                if (wasPlaybackOn)
                    {
                    // if IScrollActive flag is true, it will not allow playback
                    bool isScrollActiveStatus = IsScrollActive;
                    if (IsScrollActive) IsScrollActive = false;
                    //mProjectView.TransportBar.PlayOrResume ();
                    ResumePlaybackAfterDynamicLoading();
                    IsScrollActive = isScrollActiveStatus;
                    }
                }
            }

        //@singleSection
        private EmptyNode RemoveAllblocksInStripIfRequired ( Strip stripControl, ObiNode node, bool isScrollDown )
            {
            //int phraseBlocksLotInterval = PhraseCountInLot ( stripControl,true ) ;
            //if (stripControl.Node.PhraseChildCount <= 300) phraseBlocksLotInterval = stripControl.Node.PhraseChildCount;
            return RemoveAllblocksInStripIfRequired ( stripControl, node, PhraseCountInLot ( stripControl, isScrollDown ) );
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
                //Console.WriteLine ( "offset difference is : " + Math.Abs ( node.Index - firstBlock.Node.Index ) );
                if (((Math.Abs ( node.Index - firstBlock.Node.Index ) >= phraseBlocksLotInterval)
                                       || node.Index < firstBlock.Node.Index))
                    {
                    int startNodeIndex = 0;
                    // see if last block and target nodes lie on either side of 250 threshold
                    if (firstBlock.Node.Index > node.Index)
                        {
                        startNodeIndex = Convert.ToInt32 ( node.Index / phraseBlocksLotInterval ) * phraseBlocksLotInterval;
                        startNode = stripControl.Node.PhraseChild ( startNodeIndex );
                        //Console.WriteLine ( "required node less than first block : " + startNodeIndex );
                        }
                    else if (node.Index - firstBlock.Node.Index >= phraseBlocksLotInterval)
                        {
int thresholdAboveLastNode = Convert.ToInt32 ( node.Index / phraseBlocksLotInterval ) * phraseBlocksLotInterval;
if (thresholdAboveLastNode >= stripControl.Node.PhraseChildCount) thresholdAboveLastNode = stripControl.Node.PhraseChildCount - 1;
                        //Console.WriteLine ( "Threshold index " + thresholdAboveLastNode );

                        startNode = stripControl.Node.PhraseChild ( thresholdAboveLastNode );
                        startNodeIndex = thresholdAboveLastNode;

                        }
                    }

                if (startNode != null)
                    {

                    //System.Media.SystemSounds.Asterisk.Play ();
                    //stripControl.RemoveAllBlocks ( false );
                    if (startNode.Index > firstBlock.Node.Index)
                        {
                        // if next nodes are to be created, current nodes are backed up
                            if (!( mSelectedItem is Strip )) mProjectView.DisableSectionSelection();
                            stripControl.MoveCurrentBlocklayoutToBackground();
                        }
                    else
                        {
                            if (!(mSelectedItem is Strip)) mProjectView.DisableSectionSelection();
                        stripControl.CreateNewLayout ( false );
                        }
                    UpdateSize ();
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, stripControl.BlocksLayoutTopPosition * -1 );
                    Console.WriteLine ( "Remove block layout executed " );
                    }
                }
            else
                {
                Console.WriteLine ( "Remove aall skipped" );
                }

            return startNode;
            }


        //@ssingleSection :  small increment up or scroll
        public bool ScrollUp_SmallIncrement ( bool updateSelection )
            {
            int scrollIncrement = Convert.ToInt32 ( mHScrollBar.Location.Y / 5 ) * -1;
            ScrollMStripsPanel ( scrollIncrement, updateSelection );
            return true;
            }

        //@ssingleSection :  small increment down or scroll
        public bool ScrollDown_SmallIncrement ( bool updateSelection )
            {
            int scrollIncrement = Convert.ToInt32 ( mHScrollBar.Location.Y / 5 );
            ScrollMStripsPanel ( scrollIncrement, updateSelection );
            return true;
            }

        //@singleSection
        public bool ScrollUp_SmallIncrementWithSelection ()
            {
                if (mProjectView.Selection != null && mProjectView.Selection.Node is SectionNode) return false;
                if (!mProjectView.TransportBar.IsPlayerActive && !mProjectView.TransportBar.IsRecorderActive && m_PreviousSelectionForScroll != null) SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(null, true);
            return MoveToNextOrPreviousLineInStrip ( false );
            }

        //@singleSection
        public bool ScrollDown_SmallIncrementWithSelection ()
            {
                if (!mProjectView.TransportBar.IsPlayerActive && !mProjectView.TransportBar.IsRecorderActive && m_PreviousSelectionForScroll != null) SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(null, true);
            return MoveToNextOrPreviousLineInStrip ( true );
            }

        //@singleSection
        private bool MoveToNextOrPreviousLineInStrip ( bool nextLine )
            {
            Strip currentlyActiveStrip = ActiveStrip;
            if (currentlyActiveStrip != null && currentlyActiveStrip.Node.PhraseChildCount > 0)
                {
                    if (ScrollRestrictedWhileRecording(currentlyActiveStrip)) return false;

                if (Selection != null)
                    {
                    EmptyNode currentlySelectedEmptyNode = mProjectView.Selection is StripIndexSelection && ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection != null ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection :
                        mProjectView.Selection.Node is SectionNode ? (currentlyActiveStrip.Node.PhraseChildCount > 0? currentlyActiveStrip.Node.PhraseChild(0): null):
                        mProjectView.Selection.Node is EmptyNode ? (EmptyNode)mProjectView.Selection.Node : null;
                    if (currentlySelectedEmptyNode != null)
                        {
                        // if the section is selected and first phrase block is not in created phrase lot, create it
                            //if (mProjectView.Selection.Node is SectionNode && currentlyActiveStrip.FindBlock(currentlySelectedEmptyNode) == null)
                        if (mProjectView.Selection.Node is SectionNode )
                            {
                                SelectPhraseBlockOrStrip(currentlySelectedEmptyNode);
                                return true;
                            }
                        Block blockToBeSelected = currentlyActiveStrip.FirstBlockInNextLineOrPrevious ( currentlySelectedEmptyNode, nextLine );
                        if (nextLine)
                            {
                            if (blockToBeSelected != null && LocationOfBlockInStripPanel ( blockToBeSelected ).Y + mStripsPanel.Location.Y < mHScrollBar.Location.Y - blockToBeSelected.Height)
                                {
                                mProjectView.SelectedBlockNode = blockToBeSelected.Node;
                                return true;
                                }
                            else
                                {
                                EmptyNode firstNode = currentlyActiveStrip.FirstBlock != null ? currentlyActiveStrip.FirstBlock.Node : null;
                                ScrollDown_SmallIncrement ( false );
                                //blockToBeSelected = currentlyActiveStrip.FirstBlockInNextLineOrPrevious ( currentlySelectedEmptyNode, nextLine );
                                // if new lot is created, select first block in lot
                                if (firstNode != null && currentlyActiveStrip.FirstBlock != null && firstNode != currentlyActiveStrip.FirstBlock.Node)
                                    {
                                    mProjectView.SelectedBlockNode = currentlyActiveStrip.FirstBlock.Node;
                                    }
                                else if (blockToBeSelected != null)
                                    {
                                    mProjectView.SelectedBlockNode = blockToBeSelected.Node;
                                    }

                                if (mProjectView.ObiForm.Settings.PlayOnNavigate && mProjectView.Selection != null
                                    && currentlySelectedEmptyNode != mProjectView.Selection.Node && mProjectView.TransportBar.CurrentState != TransportBar.State.Playing)
                                {
                                    //mProjectView.TransportBar.PlayOrResume ();
                                }
                                }
                            return true;
                            }
                        else
                            {
                            if (blockToBeSelected != null &&
    LocationOfBlockInStripPanel ( blockToBeSelected ).Y + mStripsPanel.Location.Y > 0 && blockToBeSelected.Node != currentlySelectedEmptyNode)
                                {
                                mProjectView.SelectedBlockNode = blockToBeSelected.Node;
                                return true;
                                }
                            else
                                {
                                ScrollUp_SmallIncrement ( false );
                                Block newBlockToBeSelected = currentlyActiveStrip.FirstBlockInNextLineOrPrevious ( currentlySelectedEmptyNode, nextLine );
                                if (blockToBeSelected != null && newBlockToBeSelected != null
                                    && blockToBeSelected == newBlockToBeSelected)
                                    {
                                    ScrollUp_SmallIncrement ( false );
                                    newBlockToBeSelected = currentlyActiveStrip.FirstBlockInNextLineOrPrevious ( currentlySelectedEmptyNode, nextLine );
                                    }
                                if (newBlockToBeSelected != null)
                                    {
                                    mProjectView.SelectedBlockNode = newBlockToBeSelected.Node;
                                    }
                                else if (currentlyActiveStrip.LastBlock != null && currentlyActiveStrip.LastBlock.Node.Index < currentlySelectedEmptyNode.Index)
                                    {
                                    newBlockToBeSelected = currentlyActiveStrip.FirstBlockInNextLineOrPrevious ( currentlyActiveStrip.LastBlock.Node, nextLine );
                                    mProjectView.SelectedBlockNode = newBlockToBeSelected != null ? newBlockToBeSelected.Node : currentlyActiveStrip.LastBlock.Node;
                                    }
                                // workaround for avoiding distortion after very rapid line ups
                                    if (currentlyActiveStrip != null && Math.Abs(mStripsPanel.Location.Y) <= currentlyActiveStrip.BlocksLayoutTopPosition && !currentlyActiveStrip.IsContentViewFilledWithBlocks) CreatePhraseBlocksForFillingContentView(currentlyActiveStrip);
                                    if (mProjectView.ObiForm.Settings.PlayOnNavigate && mProjectView.Selection != null
                                        && currentlySelectedEmptyNode != mProjectView.Selection.Node && mProjectView.TransportBar.CurrentState != TransportBar.State.Playing)
                                    {
                                        //mProjectView.TransportBar.PlayOrResume ();
                                    }

                                }
                            }

                        }
                    }
                }

            return true;
            }

        public bool NudgeIntervalIncrement(bool increment)
        {
            double interval = mProjectView.ObiForm.Settings.Audio_NudgeTimeMs;
            if (increment && interval < 1000)
            {
                interval += 50;
            }
            else if (!increment &&  interval > 100 )
            {
                interval -= 50;
            }

            if (interval != mProjectView.ObiForm.Settings.Audio_NudgeTimeMs)
            {
                mProjectView.ObiForm.Settings.Audio_NudgeTimeMs = interval;
                if (mProjectView.ObiForm.Settings.Audio_AudioClues) Audio.AudioFormatConverter.Speak(interval.ToString () , null, mProjectView.ObiForm.Settings, mProjectView.Presentation.MediaDataManager.DefaultPCMFormat.Data);
                return true;
            }
            return false;
        }

        private bool ArrowKey_Up()
        {
            if (mProjectView.TransportBar.FineNavigationModeForPhrase)
            {
                NudgeIntervalIncrement(true);
                return true;
            }
            else
            {
                return ScrollUp_SmallIncrementWithSelection();
            }
        }

        private bool ArrowKey_Down()
        {
            if (mProjectView.TransportBar.FineNavigationModeForPhrase)
            {
                NudgeIntervalIncrement(false);
                return true;
            }
            else
            {
                return ScrollDown_SmallIncrementWithSelection();
            }
        }


        //@singleSection
        public bool ScrollUp_LargeIncrementWithSelection () { return ScrollUp_LargeIncrement ( true ); }

        //@ssingleSection :  large increment up or scroll
        public bool ScrollUp_LargeIncrement ( bool updateSelection )
            {
            ScrollMStripsPanel ( mHScrollBar.Location.Y * -1, updateSelection );
            return true;
            }

        //@singleSection
        public bool ScrollDown_LargeIncrementWithSelection () { return ScrollDown_LargeIncrement ( true ); }
        //@ssingleSection :  large increment down for scroll
        public bool ScrollDown_LargeIncrement ( bool updateSelection )
            {
            ScrollMStripsPanel ( mHScrollBar.Location.Y, updateSelection );
            return true;
            }


        //@singleSection
        public int ContentViewDepthForCreatingBlocks { get { return this.Height + Convert.ToInt32 ( ZoomFactor * 100 ); } }

        //@singleSection : base function for strips panel scroll
        public void ScrollMStripsPanel ( int interval, bool updateBlockSelection )
            {
            Strip currentlyActiveStrip = ActiveStrip;

            if (currentlyActiveStrip != null)
                {
                    DeselectStripRenameTextSelection(currentlyActiveStrip);
                    if (ScrollRestrictedWhileRecording(currentlyActiveStrip))
                    {
                        if ( mProjectView.TransportBar.RecordingPhrase != null )
                        {
                            Block b = currentlyActiveStrip.FindBlock(mProjectView.TransportBar.RecordingPhrase);
                            if (b != null) EnsureControlVisible(b);
                        }
                        return;
                    }
                Block firstBlock = currentlyActiveStrip.FirstBlock;
                Block lastBlock = currentlyActiveStrip.LastBlock;
                if (firstBlock != null && lastBlock != null)
                    {
                    int selectedItemDepthFromContentViewOrigin = -1;
                    if (mProjectView.Selection != null && (mProjectView.Selection.Node is EmptyNode || mProjectView.Selection is StripIndexSelection))
                        {
                        // compute the depth of selected item from content view origin.
                        EmptyNode currentlySelectedEmptyNode = mProjectView.Selection is StripIndexSelection ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection : (EmptyNode)mProjectView.Selection.Node;
                        if (currentlySelectedEmptyNode != null)
                            {
                            Block currentlySelectedBlock = currentlyActiveStrip.FindBlock ( currentlySelectedEmptyNode );
                            if (currentlySelectedBlock != null)
                                {
                                int selectedBlockDepthInsideStripsPanel = LocationOfBlockInStripPanel ( currentlySelectedBlock ).Y;
                                selectedItemDepthFromContentViewOrigin = mStripsPanel.Location.Y + selectedBlockDepthInsideStripsPanel;
                                //Console.WriteLine ( " depth of selected item in content view " + selectedItemDepthFromContentViewOrigin );
                                }
                            }
                        }

                        NodeSelection previousSelection = null;
                        if (!updateBlockSelection && mProjectView.Selection != null && mProjectView.Selection.Control is ContentView) previousSelection = mProjectView.Selection;
                    mProjectView.ObiForm.Cursor = Cursors.WaitCursor;
                    IsScrollActive = true;

                    int contentViewVisibleHeight = mHScrollBar.Location.Y;


                    if (interval > 0)
                        {
                        // check if the section is too small and the last block is less than mid of strips panel
                        if (firstBlock.Node.Index == 0 && lastBlock.Node.Index == currentlyActiveStrip.Node.PhraseChildCount - 1
                            && lastBlock.Location.Y - firstBlock.Location.Y + currentlyActiveStrip.BlocksLayoutTopPosition
                            < contentViewVisibleHeight)
                            {
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 0 );
                            mProjectView.ObiForm.Cursor = Cursors.Default;
                            IsScrollActive = false;
                            return;
                            }

                        int indexIncrement_PhraseLot = PhraseCountInLot ( currentlyActiveStrip, true ) - 1;
                        int nextThresholdIndex = firstBlock.Node.Index + indexIncrement_PhraseLot;
                        bool setStripsPanelToInitialPosition = false;

                        //Console.WriteLine ( "strips panel space " + (mStripsPanel.Height + mStripsPanel.Location.Y) );
                        if (nextThresholdIndex >= currentlyActiveStrip.Node.PhraseChildCount)
                            {
                            nextThresholdIndex = currentlyActiveStrip.Node.PhraseChildCount - 1;

                            }
                        else if (nextThresholdIndex <= lastBlock.Node.Index
                            && (currentlyActiveStrip.IsContentViewFilledWithBlocks || lastBlock.Node.Index - firstBlock.Node.Index > indexIncrement_PhraseLot + 1)
                            && lastBlock.Node.Index < currentlyActiveStrip.Node.PhraseChildCount - 1)
                        //&& mStripsPanel.Height + mStripsPanel.Location.Y <= contentViewVisibleHeight + 1)
                            {

                            nextThresholdIndex = nextThresholdIndex + indexIncrement_PhraseLot;
                            if (nextThresholdIndex >= currentlyActiveStrip.Node.PhraseChildCount) nextThresholdIndex = currentlyActiveStrip.Node.PhraseChildCount - 1;
                            setStripsPanelToInitialPosition = true;
                            }
                        //Console.WriteLine ( "threshold index : " + nextThresholdIndex );
                        // create blocks for additional interval
                        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch ();
                        stopWatch.Start ();

                        if (!setStripsPanelToInitialPosition)
                            {
                            CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                            currentlyActiveStrip.Node.PhraseChild ( nextThresholdIndex ),
                            false,
                           ContentViewDepthForCreatingBlocks + interval );

                            // check if strips panel can be located up by full interval
                            int pixelsUp = interval;
                            Block lastBlockAfterScroll = currentlyActiveStrip.LastBlock;
                            if (lastBlockAfterScroll != null && lastBlockAfterScroll.Node.Index > nextThresholdIndex)
                                {
                                // find block with threshold index
                                Block thresholdIndexBlock = currentlyActiveStrip.FindBlock ( currentlyActiveStrip.Node.PhraseChild ( nextThresholdIndex ) );
                                if (thresholdIndexBlock != null)
                                    {
                                    int thresholdBlockBottom = LocationOfBlockInStripPanel ( thresholdIndexBlock ).Y + thresholdIndexBlock.Height;
                                    int newPixelsUp = (mStripsPanel.Location.Y + thresholdBlockBottom) - contentViewVisibleHeight;
                                    if (newPixelsUp < pixelsUp && newPixelsUp > 0) pixelsUp = newPixelsUp;
                                    //Console.WriteLine ( " showing till threshold block while scroll down " + pixelsUp + " interval " + interval + " thresholdd " + thresholdIndexBlock.Location + " " + lastBlockAfterScroll.Location );
                                    }
                                }
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                mStripsPanel.Location.Y - pixelsUp );

                            if (mStripsPanel.Height + mStripsPanel.Location.Y < this.Height)
                                CreatePhraseBlocksForFillingContentView ( currentlyActiveStrip );

                            if (Math.Abs ( mStripsPanel.Location.Y ) > mStripsPanel.Height - contentViewVisibleHeight)
                                {
                                int cordY = (mStripsPanel.Height - (contentViewVisibleHeight / 2)) * -1;
                                Block newLastBlock = currentlyActiveStrip.LastBlock;
                                if (newLastBlock != null
                                    && (lastBlock.Bottom + currentlyActiveStrip.BlocksLayoutTopPosition) < contentViewVisibleHeight)
                                    {
                                    cordY = currentlyActiveStrip.BlocksLayoutTopPosition * -1;
                                    }
                                mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
    cordY );
                                }
                            }
                        else
                            {
                            CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                        currentlyActiveStrip.Node.PhraseChild ( nextThresholdIndex ),
                        true,
                       0 );
                            }
                        stopWatch.Stop ();
                        //Console.WriteLine ( "time while croll down " + stopWatch.ElapsedMilliseconds );
                        //Console.WriteLine ( "Strips panel location after scroll " + mStripsPanel.Location );
                        }
                    else if (interval < 0) // move strips panel down
                        {
                        if (mStripsPanel.Location.Y > interval)
                            {//2

                            if (mStripsPanel.Location.Y >= currentlyActiveStrip.BlocksLayoutTopPosition * -1)
                                {//3
                                Console.WriteLine ( "Scroll while creating previous phrases " );
                                if (firstBlock.Node.Index > 0)
                                    {//4
                                    int prevThreshold = firstBlock.Node.Index - 1;
                                    System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch ();
                                    //stopWatch.Start ();

                                    int requiredPhraseCount = currentlyActiveStrip.GetPhraseCountForContentViewVisibleHeight ( mHScrollBar.Location.Y, verticalScrollToolStripContainer1.Location.X,
                                        currentlyActiveStrip.Node.PhraseChild ( prevThreshold ), true );
                                    stopWatch.Start ();
                                    //currentlyActiveStrip.RemoveAllBlocks ( false );
                                    //currentlyActiveStrip.AddsRangeOfBlocks ( currentlyActiveStrip.Node.PhraseChild ( prevThreshold - requiredPhraseCount ),
                                    //currentlyActiveStrip.Node.PhraseChild ( prevThreshold ) );
                                    //UpdateSize ();
                                    //currentlyActiveStrip.UpdateColors ();
                                    CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                            currentlyActiveStrip.Node.PhraseChild ( prevThreshold ),
                            false );


                                    Block lastBlockInNewLayout = currentlyActiveStrip.LastBlock;
                                    Block expectedLastBlock = null;
                                    if (lastBlockInNewLayout != null && lastBlockInNewLayout.Node.IsRooted && lastBlockInNewLayout.Node.Index > prevThreshold
                                        &&
                                        (expectedLastBlock = currentlyActiveStrip.FindBlock ( currentlyActiveStrip.Node.PhraseChild ( prevThreshold ) )) != null
                                        && expectedLastBlock.Bottom < lastBlockInNewLayout.Bottom)
                                        {
                                        mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                            (expectedLastBlock.Bottom + currentlyActiveStrip.BlocksLayoutTopPosition - contentViewVisibleHeight) * -1 );

                                        }
                                    else
                                        {
                                        mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                            (mStripsPanel.Height - contentViewVisibleHeight) * -1 );

                                        CreatePhraseBlocksForFillingContentView ( currentlyActiveStrip );
                                        }

                                    stopWatch.Stop ();
                                    //Console.WriteLine ( "stop watch " + stopWatch.Elapsed.TotalMilliseconds );
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
                                    currentlyActiveStrip.RemoveAllFollowingBlocks ( true, true );
                                    }
                                Console.WriteLine ( "adjusted upto label " );
                                }//-3
                            }//-2
                        else // just move strips panel down
                            {//2
                            int cordY = mStripsPanel.Location.Y - interval; //interval is negetive
                            if (currentlyActiveStrip.OffsetForFirstPhrase > 0 && cordY > (currentlyActiveStrip.BlocksLayoutTopPosition * -1)) cordY = (currentlyActiveStrip.BlocksLayoutTopPosition * -1);
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                                        cordY );//interval is negetive
                            Console.WriteLine ( "just moved strips panel down " );
                            }//-2
                        // adjust strip panel location if it is more than Y=0
                        if (mStripsPanel.Location.Y > 0)
                            {
                            Block newFirstBlock = null;
                            int cordY = (newFirstBlock == null || (newFirstBlock != null && newFirstBlock.Node.Index == 0)) ? 0 :
                                currentlyActiveStrip.BlocksLayoutTopPosition * -1;
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X, cordY );
                            //Console.WriteLine ( "Strips panel has to be adjusted as it moved ahead 0 position" );
                            }
                        //Console.WriteLine ( "Strips panel location while moving up " + mStripsPanel.Location.Y );
                        }
                    //ReturnFocusFromVerticalScrollPanel ();
                        if (Math.Abs(interval) >= 200) PlayShowBlocksCompletedSound();
                    IsScrollActive = false;
                    mProjectView.ObiForm.Cursor = Cursors.Default;
                    UpdateVerticalScrolPanelButtons();

                    //update selection if flag is true
                    if (updateBlockSelection && selectedItemDepthFromContentViewOrigin >= 0)
                        {
                        int depthOfBlockInsTrip = Math.Abs ( mStripsPanel.Location.Y ) + selectedItemDepthFromContentViewOrigin - currentlyActiveStrip.Location.Y;
                        Block blockToBeSelected = currentlyActiveStrip.FindBlockAtLocationInStrip ( depthOfBlockInsTrip );
                        if (blockToBeSelected != null && blockToBeSelected.Node != mProjectView.Selection.Node)
                        {
                            mProjectView.SelectedBlockNode = blockToBeSelected.Node;
                        }
                        else if (currentlyActiveStrip.FirstBlock != null )
                        {
                            //if scroll down is used and last phrase lot was  not yet created then only select first block.
                            if (interval > 0)
                            {
                                if ( mProjectView.Selection != null && mProjectView.Selection.EmptyNodeForSelection != null && firstBlock.Node.Index >= mProjectView.Selection.EmptyNodeForSelection.Index )  mProjectView.SelectedBlockNode = currentlyActiveStrip.FirstBlock.Node;
                            }
                            else
                            {
                                mProjectView.SelectedBlockNode = currentlyActiveStrip.FirstBlock.Node;
                            }
                        }
                        //if (blockToBeSelected != null) Console.WriteLine ( "selected block location " + (LocationOfBlockInStripPanel ( blockToBeSelected ).Y + mStripsPanel.Location.Y) );
                        }
                        if (previousSelection != null) ManageSelectionChangeWhileScroll(previousSelection, currentlyActiveStrip);
                    }
                verticalScrollToolStripContainer1.TrackBarValueInPercentage = EstimateScrollPercentage ( currentlyActiveStrip );
                }// check ends for currently active strip

            }

        //@singleSection
        public Point LocationOfBlockInStripPanel (Control block )
            {
            Point location = new Point ( block.Location.X, block.Location.Y );

            Control parent = block.Parent;
            while (parent != null && parent != mStripsPanel)
                {
                location.X += parent.Location.X;
                location.Y += parent.Location.Y;
                parent = parent.Parent;
                }
            return location;
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
                phraseLotSize = (phraseLotSize / 2);
                }


            if (currentlyActiveStrip.Node.PhraseChildCount <= (phraseLotSize * 6 / 5))
                {
                phraseLotSize = currentlyActiveStrip.Node.PhraseChildCount;
                }
            //Console.WriteLine ( "Calculated phrase lot size " + phraseLotSize );
            return phraseLotSize;
            }

        //@singleSection
        private int EstimateScrollPercentage ( Strip currentlyActiveStrip )
            {
            // if upper block top position is more than the location of strips panel then only calculation will give right result else use 0
            int startY =mStripsPanel.Location.Y > -(currentlyActiveStrip.BlocksLayoutTopPosition)?0:   Math.Abs ( mStripsPanel.Location.Y + currentlyActiveStrip.BlocksLayoutTopPosition);
            int endY = startY + mHScrollBar.Location.Y;

            List<int> boundaryPhraseIndexes = currentlyActiveStrip.GetBoundaryPhrasesIndexForVisiblePhrases ( startY, endY );
            if (boundaryPhraseIndexes == null || boundaryPhraseIndexes.Count == 0) return 0;

            int percentageValue = 0;
            if (boundaryPhraseIndexes[0] == 0)
                {
                percentageValue = 0;
                }
            else if (boundaryPhraseIndexes.Count == 2 && currentlyActiveStrip.Node.PhraseChildCount > 0
                && boundaryPhraseIndexes[1] == currentlyActiveStrip.Node.PhraseChildCount - 1)
                {
                percentageValue = 100;
                }
            else if (boundaryPhraseIndexes.Count == 2 && currentlyActiveStrip.Node.PhraseChildCount > 0)
                {
                int midIndexVisible = Convert.ToInt32 ( (boundaryPhraseIndexes[0] + boundaryPhraseIndexes[1]) / 2 );
                percentageValue = Convert.ToInt32 ( (midIndexVisible * 100) / currentlyActiveStrip.Node.PhraseChildCount );
                }
            //Console.WriteLine ( "estimated percentage of scroll " + percentageValue );
            return percentageValue;
            }

        //@singleSection
        private bool UpdateScrollTrackBarAccordingToSelectedNode()
        {
            if (mProjectView.Selection != null && (mProjectView.Selection is StripIndexSelection || mProjectView.Selection.Node is EmptyNode))
            {
                Strip currentlyActiveStrip = ActiveStrip;
                if (currentlyActiveStrip != null && !currentlyActiveStrip.IsBlocksVisible)
                {
                    EmptyNode currentlySelectedEmptyNode = mProjectView.Selection is StripIndexSelection ?
                        (((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection != null ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection : (currentlyActiveStrip.LastBlock != null ? currentlyActiveStrip.LastBlock.Node : null)) :
                        (EmptyNode)mProjectView.Selection.Node;

                    if (currentlySelectedEmptyNode != null && currentlyActiveStrip.Node == currentlySelectedEmptyNode.ParentAs<SectionNode>())
                    {
                        int percentageValue = Convert.ToInt32(  (currentlySelectedEmptyNode.Index * 100 )/ currentlyActiveStrip.Node.PhraseChildCount);
                        if (verticalScrollToolStripContainer1.TrackBarValueInPercentage != percentageValue) verticalScrollToolStripContainer1.TrackBarValueInPercentage = percentageValue;
                        return true;    
                    }
                }// check ends for currently active strip
            }
            return false;
        }



        //@singleSection : Scroll to top
        public bool ScrollStripsPanel_Top ()
        {
            Strip currentlyActiveStrip = ActiveStrip;

            if (currentlyActiveStrip != null && currentlyActiveStrip.Node.PhraseChildCount > 0)
                {
                    DeselectStripRenameTextSelection(currentlyActiveStrip);
                    if (ScrollRestrictedWhileRecording(currentlyActiveStrip))
                    {
                        if (mProjectView.TransportBar.RecordingPhrase != null)
                        {
                            Block b = currentlyActiveStrip.FindBlock(mProjectView.TransportBar.RecordingPhrase);
                            if (b != null) EnsureControlVisible(b);
                        }
                        return false;
                    }

                    NodeSelection previousSelection = mProjectView.Selection;
                mProjectView.ObiForm.Cursor = Cursors.WaitCursor;
                IsScrollActive = true;

                Block firstBlock = currentlyActiveStrip.FirstBlock;
                CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                                currentlyActiveStrip.Node.PhraseChild ( 0 ),
                                false );

                mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 0 );

                CreatePhraseBlocksForFillingContentView ( currentlyActiveStrip );
                verticalScrollToolStripContainer1.TrackBarValueInPercentage = 0;
                //ReturnFocusFromVerticalScrollPanel ();
                if(firstBlock != null && currentlyActiveStrip.FirstBlock != null &&  firstBlock != currentlyActiveStrip.FirstBlock)  PlayShowBlocksCompletedSound();
                IsScrollActive = false;
                mProjectView.ObiForm.Cursor = Cursors.Default;
                if (previousSelection != null) ManageSelectionChangeWhileScroll(previousSelection, currentlyActiveStrip);
                return true;
                }
            return false;
            }

        //@singleSection : Scroll to bottom
        public bool ScrollStripsPanel_Bottom ()
            {
                Strip currentlyActiveStrip = ActiveStrip;
            
            if (currentlyActiveStrip != null && currentlyActiveStrip.Node.PhraseChildCount > 0)
                {
                    DeselectStripRenameTextSelection(currentlyActiveStrip);
                    if (ScrollRestrictedWhileRecording(currentlyActiveStrip))
                    {
                        if (mProjectView.TransportBar.RecordingPhrase != null)
                        {
                            Block b = currentlyActiveStrip.FindBlock(mProjectView.TransportBar.RecordingPhrase);
                            if (b != null) EnsureControlVisible(b);
                        }
                        return false;
                    }

                    NodeSelection previousSelection = mProjectView.Selection;
                mProjectView.ObiForm.Cursor = Cursors.WaitCursor;
                IsScrollActive = true;
                CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                    currentlyActiveStrip.Node.PhraseChild ( currentlyActiveStrip.Node.PhraseChildCount - 1 ),
                    false );
                mStripsPanel.Location = new Point ( mStripsPanel.Location.X,
                    (mStripsPanel.Height - (mHScrollBar.Location.Y - 10)) * -1 );
                verticalScrollToolStripContainer1.TrackBarValueInPercentage = 100;
                //ReturnFocusFromVerticalScrollPanel ();
                PlayShowBlocksCompletedSound();
                IsScrollActive = false;
                mProjectView.ObiForm.Cursor = Cursors.Default;
                if (previousSelection != null) ManageSelectionChangeWhileScroll(previousSelection, currentlyActiveStrip);
                return true;

                }
            return false;
            }

        public bool  DeselectStripRenameTextSelection ( Strip currentlyActiveStrip)
        {
            if (mProjectView.Selection != null && mProjectView.Selection is TextSelection && mProjectView.Selection.Node == currentlyActiveStrip.Node)
            {
                mProjectView.Selection = new NodeSelection(mProjectView.Selection.Node, mProjectView.Selection.Control);
                return true;
            }
            return false;
        }


        //@singleSection
        private void ManageSelectionChangeWhileScroll(NodeSelection previousSelection, Strip stripControl)
        {
                        if ( previousSelection != null && mProjectView.Selection != null
                //&& previousSelection != mProjectView.Selection 
                && (previousSelection.Node is EmptyNode || previousSelection is StripIndexSelection )
                //&& (mProjectView.Selection.Node is SectionNode && (mProjectView.Selection is StripIndexSelection)) 
                && m_PreviousSelectionForScroll != mProjectView.Selection)
                            {
                            EmptyNode previouslySelectedNode = previousSelection is StripIndexSelection? ((StripIndexSelection)previousSelection).EmptyNodeForSelection: (EmptyNode)previousSelection.Node ;

                            if (previousSelection != null && stripControl.FindBlock(previouslySelectedNode) == null)
                            {
                                m_PreviousSelectionForScroll = previousSelection;
                                //MessageBox.Show(m_PreviousSelectionForScroll.ToString());
                            }
            }
            SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(stripControl, false) ;
        }

        //@singleSection: flag to prevent screen refresh in case
        // when selection has to be restored on phrase in next phrase lot
        private bool m_DisablePhraseCreationWhileSelectionRestore = false;

            //@singleSection
            private bool SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(Strip currentlyActiveStrip, bool createRequiredNode)
            {
                
                if (m_PreviousSelectionForScroll != null && m_PreviousSelectionForScroll.Node.IsRooted && mProjectView.Selection != null
                    && (m_PreviousSelectionForScroll.Node is EmptyNode || m_PreviousSelectionForScroll is StripIndexSelection))
                    //&& (mProjectView.Selection.Node is SectionNode && !(mProjectView.Selection is StripIndexSelection)))
                {
                    if (currentlyActiveStrip == null ) currentlyActiveStrip =  FindStrip(m_PreviousSelectionForScroll is StripIndexSelection?(SectionNode)m_PreviousSelectionForScroll.Node :  m_PreviousSelectionForScroll.Node.ParentAs<SectionNode> () );
                                        if (currentlyActiveStrip != null)
                    {
                        EmptyNode previouslySelectedEmptyNode = m_PreviousSelectionForScroll is StripIndexSelection ? ((StripIndexSelection)m_PreviousSelectionForScroll).EmptyNodeForSelection : (EmptyNode)m_PreviousSelectionForScroll.Node;
                        
                        if (previouslySelectedEmptyNode != null)
                        {
                            if (createRequiredNode) CreateBlocksTillNodeInStrip(currentlyActiveStrip, previouslySelectedEmptyNode, false);
                            
                            Block blockForPreviouslySelectedNode = currentlyActiveStrip.FindBlock(previouslySelectedEmptyNode);
                            if (blockForPreviouslySelectedNode != null)
                            {
                                //MessageBox.Show("selected " + m_PreviousSelectionForScroll.Node.ToString());
                                bool selectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                                DisableScrolling();
                                //mProjectView.SelectedBlockNode = previouslySelectedEmptyNode;
                                //mProjectView.Selection = m_PreviousSelectionForScroll;

                                if (!createRequiredNode) m_DisablePhraseCreationWhileSelectionRestore = true;
                                if (m_PreviousSelectionForScroll is AudioSelection)
                                {
                                    mProjectView.Selection = null;
                                    mProjectView.Selection = m_PreviousSelectionForScroll;
                                }
                                else
                                {
                                    mProjectView.Selection = new NodeSelection(m_PreviousSelectionForScroll.Node, m_PreviousSelectionForScroll.Control);
                                }
                                m_DisablePhraseCreationWhileSelectionRestore = false;
                                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = selectionChangedPlaybackEnabledStatus;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }


        private void ReturnFocusFromVerticalScrollPanel ()
            {
            if (mSelectedItem != null && mSelectedItem is Control
                && !((Control)mSelectedItem).Focused)
                {
                DisableScrolling ();
                ((Control)mSelectedItem).Focus ();
                }
            }

        //@singleSection
        private bool ScrollRestrictedWhileRecording(Strip currentlyActiveStrip)
        {
            if (mProjectView.TransportBar.IsRecorderActive && this.ContainsFocus)
            {
                if (mProjectView.TransportBar.RecordingPhrase != null && mProjectView.TransportBar.RecordingPhrase.ParentAs<SectionNode>() == currentlyActiveStrip.Node)
                {
                    SelectPhraseBlockOrStrip(mProjectView.TransportBar.RecordingPhrase);
                }
                return true;
            }
            return false;
        }

        //@singleSection
        private bool m_IsScrollActive = false;
        public bool IsScrollActive
            {
            get
                {
                return m_IsScrollActive;
                }
            set
                {
                m_IsScrollActive = value;
                ChangeVisibilityProcessState ( m_IsScrollActive );
                if (!mProjectView.TransportBar.Enabled) mProjectView.TransportBar.Enabled = m_IsScrollActive;
                //if (!m_IsScrollActive) PlayShowBlocksCompletedSound();
                //mProjectView.ObiForm.ShowHideInvisibleDialog ( m_IsScrollActive );
                }
            }


        //@singleSection
        public void mStripsPanel_LocationChanged ( object sender, EventArgs e )
            {
                UpdateVerticalScrolPanelButtons();
        }

        private void UpdateVerticalScrolPanelButtons () 
    {
            // if strip label is visible partially or fully
            if (mStripsPanel.Location.Y >= (-43 * ZoomFactor ) )
                {
                Strip currentlyActiveStrip = ActiveStrip;

                if (currentlyActiveStrip != null)
                    {
                    if ((currentlyActiveStrip.FirstBlock == null && currentlyActiveStrip.Node.PhraseChildCount == 0)
                        || (currentlyActiveStrip.FirstBlock != null && currentlyActiveStrip.FirstBlock.Node.IsRooted && currentlyActiveStrip.FirstBlock.Node.Index == 0))
                        {
                        if ( mStripsPanel.Location.Y < 0 && mStripsPanel.Location.Y > ( currentlyActiveStrip.BlocksLayoutTopPosition / 2 ) * -1  )
                        {
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X , 0 ) ;
                        }

                            if (mStripsPanel.Location.Y > -10) verticalScrollToolStripContainer1.CanScrollUp = false;
                            else verticalScrollToolStripContainer1.CanScrollUp = true;
                        }
                    else if ( currentlyActiveStrip.FirstBlock != null && currentlyActiveStrip.FirstBlock.Node.IsRooted && currentlyActiveStrip.FirstBlock.Node.Index > 0)
                                                 {
                            //if (Math.Abs ( mStripsPanel.Location.Y ) > currentlyActiveStrip.BlocksLayoutTopPosition)
                        // set position of strip panel to hide label -- for precaution
                        mStripsPanel.Location = new Point ( mStripsPanel.Location.X, currentlyActiveStrip.BlocksLayoutTopPosition * -1 );
                        Console.WriteLine ( "precautionary setting of strips label for threshold index " );
                        verticalScrollToolStripContainer1.CanScrollUp = true;
                        }
                    }
                }
            else if (mStripsPanel.Location.Y < 0)
                {
                verticalScrollToolStripContainer1.CanScrollUp = true;
                }

            if (mStripsPanel.Location.Y + mStripsPanel.Height < mHScrollBar.Location.Y)
                {
                Strip currentlyActiveStrip = ActiveStrip;
                if (currentlyActiveStrip != null)
                    {
                        if ((currentlyActiveStrip.LastBlock == null && currentlyActiveStrip.Node.PhraseChildCount == 0)
                            || (currentlyActiveStrip.LastBlock != null && currentlyActiveStrip.LastBlock.Node.IsRooted && currentlyActiveStrip.LastBlock.Node.Index == currentlyActiveStrip.Node.PhraseChildCount - 1))
                        {
                            int previousTrackBarValue = verticalScrollToolStripContainer1.TrackBarValueInPercentage;
                            verticalScrollToolStripContainer1.CanScrollDown = false;
                            if (mStripsPanel.Location.Y >= -(currentlyActiveStrip.BlocksLayoutTopPosition) && previousTrackBarValue < 20  &&  verticalScrollToolStripContainer1.TrackBarValueInPercentage == 100 && currentlyActiveStrip.OffsetForFirstPhrase == 0) verticalScrollToolStripContainer1.TrackBarValueInPercentage = 0;
                        }
                        else
                        {
                            verticalScrollToolStripContainer1.CanScrollDown = true;
                        }
                    }
                }
            else
                {
                verticalScrollToolStripContainer1.CanScrollDown = true;
                }
            }

        private void Recorder_StateChanged(object sender, EventArgs e)
        {
            if (mProjectView.ObiForm.Settings.Audio_ShowLiveWaveformWhileRecording && mProjectView.TransportBar.IsRecording)
            {
                waveform_recording_control.Visible = true;    //@Onthefly
                waveform_recording_control.BringToFront();
                waveform_recording_control.RecordingSession = mProjectView.TransportBar.RecordingSession;   //@Onthefly
            }
            else
            {
                waveform_recording_control.Visible = false;   //@Onthefly
            }
        }

        private int m_StripPanelPreviousWidth = 0;
        private void mStripsPanel_Resize ( object sender, EventArgs e )
        {
        if (mProjectView == null) return;
        if (m_StripPanelPreviousWidth != mStripsPanel.Width && Math.Abs ( m_StripPanelPreviousWidth - mStripsPanel.Width ) > 50
            && mProjectView.Selection != null
            && (mProjectView.Selection is StripIndexSelection || mProjectView.Selection.Node is EmptyNode))
            {
                Strip currentlyActiveStrip = ActiveStrip;
            EmptyNode currentlySelectedEmptyNode = mProjectView.Selection is StripIndexSelection && ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection != null ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection :
                mProjectView.Selection.Node is EmptyNode ? (EmptyNode)mProjectView.Selection.Node : null;

            if (currentlySelectedEmptyNode != null && currentlyActiveStrip != null)
                {
                Block selectedBlock = currentlyActiveStrip.FindBlock( currentlySelectedEmptyNode );
                if (selectedBlock != null) EnsureControlVisible ( selectedBlock );
                }
                
            }
            
        m_StripPanelPreviousWidth = mStripsPanel.Width;
            if ( verticalScrollToolStripContainer1 != null && !verticalScrollToolStripContainer1.CanScrollDown && mStripsPanel.Location.Y + mStripsPanel.Height > mHScrollBar.Location.Y )
                 UpdateVerticalScrolPanelButtons () ;
            }

        public void RecreateContentsWhileInitializingRecording(EmptyNode recordingResumePhrase)
        {
            if (recordingResumePhrase != null
                || (mProjectView.Selection != null && mProjectView.Selection.Node is SectionNode && !(mProjectView.Selection is StripIndexSelection)))
                {
                SectionNode section = recordingResumePhrase != null ? recordingResumePhrase.ParentAs<SectionNode> () :
                    (SectionNode)mProjectView.Selection.Node;

                Strip stripControl = FindStrip ( section );

                if (stripControl == null)
                {
                    if (MessageBox.Show(string.Format (Localizer.Message( "Recording_CreateSectionContentsInformation"), section.Label),
                        Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK
                        &&    (Selection == null || !(Selection.Control is ContentView) ))
                    {
                        stripControl= CreateStripForSelectedSection(section, true);
                        EmptyNode lastNodeToCreate = recordingResumePhrase != null && recordingResumePhrase.IsRooted ? recordingResumePhrase : 
                            section.PhraseChildCount > 0? section.PhraseChild(section.PhraseChildCount - 1): null;
                        if (lastNodeToCreate != null)
                        {
                            CreateBlocksTillNodeInStrip(stripControl, lastNodeToCreate, false);
                            Block preRecordingBlock  = stripControl.FindBlock (lastNodeToCreate ) ;
                            if (preRecordingBlock != null)
                            {
                                EnsureControlVisible(preRecordingBlock);
                                int trackBarPercent = (lastNodeToCreate.Index * 100) / stripControl.Node.PhraseChildCount;
                                verticalScrollToolStripContainer1.TrackBarValueInPercentage = trackBarPercent;
                            }
                            if (mStripsPanel.Location.Y < 0) verticalScrollToolStripContainer1.CanScrollUp = true;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                if (recordingResumePhrase != null && stripControl.FindBlock ( recordingResumePhrase ) != null) return;

                Block firstBlock = stripControl.FirstBlock;
                Block lastBlock = stripControl.LastBlock;

                if (firstBlock != null && lastBlock != null
                    && (lastBlock.Node.Index < stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 ).Index
                    || (recordingResumePhrase != null && lastBlock.Node.Index < recordingResumePhrase.Index)))
                    {
                    EmptyNode lastVisiblePhraseIntended = recordingResumePhrase != null ? recordingResumePhrase :
                        stripControl.Node.PhraseChild ( stripControl.Node.PhraseChildCount - 1 );

                    if (lastVisiblePhraseIntended.Index < lastBlock.Node.Index + 10)
                        {
                        CreateBlocksTillNodeInStrip ( stripControl, lastVisiblePhraseIntended, false );
                        if (recordingResumePhrase != null) CreatePhraseBlocksForFillingContentView ( stripControl );
                        int trackBarPercent = (lastVisiblePhraseIntended.Index * 100) / stripControl.Node.PhraseChildCount;
                        verticalScrollToolStripContainer1.TrackBarValueInPercentage = trackBarPercent;
                        if (mStripsPanel.Location.Y < 0) verticalScrollToolStripContainer1.CanScrollUp = true;
                        return;
                        }
                    //System.Media.SystemSounds.Asterisk.Play ();
                    //stripControl.RemoveAllBlocks ( false );
                    stripControl.CreateNewLayout ( false );
                    UpdateSize ();
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
                        verticalScrollToolStripContainer1.TrackBarValueInPercentage = 100;
                        if (mStripsPanel.Location.Y <= stripControl.BlocksLayoutTopPosition*-1 ) verticalScrollToolStripContainer1.CanScrollUp = true;
                    }
                }
            }

        //@singleSection
        public void PostRecording_RecreateInvisibleRecordingPhrases(SectionNode section, int initialIndex, int count)
        {
            if (initialIndex + count == section.PhraseChildCount )
            {
                                Strip currentlyActiveStrip = FindStrip(section);
                if (currentlyActiveStrip != null)
                {
                    bool shouldRecreateBlocks = false;
                    for (int i =  section.PhraseChildCount-1; i >= initialIndex && i >= 0 ; --i)
                    {
                        EmptyNode node = section.PhraseChild(i);
                        Block b = currentlyActiveStrip.FindBlock(node);
                        if (b != null && 
                            ((LocationOfBlockInStripPanel(b).Y + b.Height) > mStripsPanel.Height) || ((LocationOfBlockInStripPanel(b).Y + b.Height) > currentlyActiveStrip.Height))
                        {
                            currentlyActiveStrip.RemoveBlock(node , true);
                            shouldRecreateBlocks = true;
                            //Console.WriteLine("Removing post recording phrase block " + node);
                        }
                    }
                    if ( shouldRecreateBlocks)  CreateBlocksTillNodeInStrip(currentlyActiveStrip,(EmptyNode) section.PhraseChild(section.PhraseChildCount - 1), false);
                }
                
            }
        }

        // @phraseLimit
        /// <summary>
        /// Make phrase blocks visible for strip  passed as parameter
        /// </summary>
        /// <param name="stripControl"></param>
        /// <returns></returns>
        private bool CreateBlocksInStrip(Strip stripControl)
            {
            return CreateLimitedBlocksInStrip ( stripControl, null );
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


        //@phraseLimit: required in @singleSection also
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
                        mProjectView.WriteToLogFile(ex.ToString());
                        MessageBox.Show ( ex.ToString () );
                        return 0;
                        }

                    }
                }
            return 0;
            }

        // @phraseLimit: required in @singleSection
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
                //if (!stripControl.IsBlocksVisible) m_VisibleStripsList.Remove ( stripControl );
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
                //if (!stripControl.IsBlocksVisible) m_VisibleStripsList.Remove ( stripControl );
                return countRequired;
                }
            return 0;
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

                if (mStripsPanel.Controls.Count > 0)
                    {
                    foreach (Control c in mStripsPanel.Controls)
                        {
                        if (c is Strip)
                            {
                            Strip s = (Strip)c;
                            if (s.Node == node)
                                {
                                if (s.IsBlocksVisible) return true;
                                else return false;
                                }
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
                string FilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ShowBlocksCompleted.wav");
                if (System.IO.File.Exists(FilePath) && mProjectView.ObiForm.Settings.Audio_AudioClues)
                {
                    System.Media.SoundPlayer showBlocksPlayer = new System.Media.SoundPlayer(FilePath);
                    showBlocksPlayer.Play();
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
        private urakawa.command.Command DeleteStripCommand ( SectionNode section )
            {
            Commands.Node.Delete delete = new Commands.Node.Delete ( mProjectView, section, Localizer.Message ( "delete_section_shallow" ) );
            if (section.SectionChildCount > 0)
                {
                CompositeCommand command = mProjectView.Presentation.CommandFactory.CreateCompositeCommand ();
                command.ShortDescription = delete.ShortDescription;
                for (int i = 0; i < section.SectionChildCount; ++i)
                    {
                    command.ChildCommands.Insert (command.ChildCommands.Count, new Commands.TOC.MoveSectionOut ( mProjectView, section.SectionChild ( i ) ) );
                    }
                    command.ChildCommands.Insert(command.ChildCommands.Count, delete);
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

        public void UpdateBlockForFindNavigation(EmptyNode node, bool IsFineNavigation)
        {
            Block block = FindBlock(node);
            if (block != null)
            {
                block.IsFineNavigationMode = IsFineNavigation;
                block.UpdateColors();
            }
        }

        // Find the selectable item for this selection object (block, strip or strip cursor.)
        private ISelectableInContentView FindSelectable ( NodeSelection selection )
            {
            return
                selection == null ? null :
                selection is StripIndexSelection ? (ISelectableInContentView)FindStripCursor ( (StripIndexSelection)selection ) :
                selection.Node is SectionNode ? (ISelectableInContentView)FindStrip ( (SectionNode)selection.Node ) :
                selection.Node is EmptyNode ? m_ZoomWaveformPanel == null ? (ISelectableInContentView)FindBlock((EmptyNode)selection.Node) :
                (ISelectableInContentView)m_ZoomWaveformPanel : null;  //@zoomwaveform
            }

        private bool IsAudioRangeSelected { get { return mSelection != null && mSelection is AudioSelection && ((AudioSelection)mSelection).AudioRange != null && !((AudioSelection)mSelection).AudioRange.HasCursor; } }
        private bool IsBlockSelected { get { return mSelectedItem != null && mSelectedItem is Block && mSelection.GetType () == typeof ( NodeSelection ) && ((Block)mSelectedItem ).Node.IsRooted; } }
        private bool IsBlockOrWaveformSelected { get { return mSelectedItem != null && mSelectedItem is Block && ((Block)mSelectedItem ).Node.IsRooted; } }
        private bool IsInView ( ObiNode node ) { return node is SectionNode && FindStrip ( (SectionNode)node ) != null; }
        private bool IsStripCursorSelected { get { return mSelection != null && mSelection is StripIndexSelection; } }
        private bool IsStripSelected { get { return mSelectedItem != null && mSelectedItem is Strip && mSelection.GetType () == typeof ( NodeSelection ); } }

        // Listen to changes in the presentation (new nodes being added or removed)
        private void Presentation_changed ( object sender, urakawa.events.DataModelChangedEventArgs e )
            {
                if (e is ObjectRemovedEventArgs<urakawa.core.TreeNode> && mProjectView.ObiForm.Settings.PlayOnNavigate
                && ((ObjectRemovedEventArgs<urakawa.core.TreeNode>)e).m_RemovedObject is EmptyNode)
                {
                    mProjectView.TransportBar.SkipPlayOnNavigateForSection = true;
                }
            if (e is ObjectAddedEventArgs<urakawa.core.TreeNode>)
                {
                TreeNodeAdded ( (ObjectAddedEventArgs<urakawa.core.TreeNode>)e );
                }
            else if (e is ObjectRemovedEventArgs<urakawa.core.TreeNode>)
                {
                TreeNodeRemoved ( (ObjectRemovedEventArgs<urakawa.core.TreeNode>)e );
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

            if (clipboard == null ||
                (clipboard != null && strip != null && clipboard.Node != strip.Node)) // @phraseLimit
                {
                if (strip != null) strip.DestroyStripHandle ();
                strip = null;
                }
            else if (strip != null)
                {
                strip.RemoveAllBlocks ( false );
                }

            }

        // Remove the strip or block for the removed tree node
        private void TreeNodeRemoved ( ObjectRemovedEventArgs<urakawa.core.TreeNode> e )
            {
            if (e.m_RemovedObject is SectionNode)
                {


                    RemoveStripsForSection_Safe((SectionNode)e.m_RemovedObject);

                }
                else if (e.m_RemovedObject is EmptyNode)
                {
                // TODO in the future, the parent of a removed empty node
                // will not always be a section node!
                Strip strip = FindStrip ( (SectionNode)e.SourceObject );
                if (strip != null) strip.RemoveBlock((EmptyNode)e.m_RemovedObject);

                EmptyNode eNode = (EmptyNode)e.m_RemovedObject;
                    if (eNode.Role_ == EmptyNode.Role.Anchor ) mProjectView.Presentation.ListOfAnchorNodes_Remove(eNode) ;
                }
            }

        // Add a new strip for a newly added section node or a new block for a newly added empty node.
        private void TreeNodeAdded ( ObjectAddedEventArgs<urakawa.core.TreeNode> e )
            {
                if (e.m_AddedObject is SectionNode) mProjectView.Presentation.ListOfSectionsToBeIteratedForAnchors_Add((SectionNode)e.m_AddedObject);
            //@singleSection : AddStripForSection_Safe replaced by CreateStripForAddedSectionNode
            // this will remove existing strips before creating new strip in content view
            Control c = e.m_AddedObject is SectionNode ? (Control)CreateStripForAddedSectionNode ( (SectionNode)e.m_AddedObject, true ) :
                // TODO: in the future, the source node will not always be a section node!
                e.m_AddedObject is EmptyNode ? AddBlockForNodeConsideringPhraseLimit ( (Strip)FindStrip ( (SectionNode)e.SourceObject ), ((EmptyNode)e.m_AddedObject) ) : // @phraseLimit
                null;
            UpdateNewControl ( c );
            }

        // @phraseLimit
        private Block AddBlockForNodeConsideringPhraseLimit ( Strip stripControl, EmptyNode node )
            {
                if (node != null &&  node.Role_ == EmptyNode.Role.Anchor ) mProjectView.Presentation.ListOfAnchorNodes_Add(node);
            // if the node is above max phrase limit per section, do not add block and return
            if (node.Index > mProjectView.MaxVisibleBlocksCount
                || stripControl == null)//@singleSection: this null check shuld surely be replaced by strip creation code
                {
                //@singleSection : if no strip is visible in content view, make the parent strip of empty node visible 
                if (ActiveStrip == null)
                    {
                    stripControl = CreateStripForAddedSectionNode ( node.ParentAs<SectionNode> (), true );
                    }
                else
                    {
                    return null;
                    }
                }
            Block lastBlock = stripControl.LastBlock;
            if (lastBlock != null)
                {
                int phraseLotSize = PhraseCountInLot ( stripControl, true );
                int nextThreshold = (Convert.ToInt32 ( lastBlock.Node.Index / phraseLotSize ) + 1) * phraseLotSize;

                if (node.Index > nextThreshold
                    || (stripControl.IsContentViewFilledWithBlocks && node.Index > lastBlock.Node.Index))
                    {
                    //here no need for applying check for nodes before threshold as it is handled in add blok for node function in strip
                    //Console.WriteLine ( "exiting before making block " );
                    return null;
                    }

                // if the block to be created is at offset from the last phrase block then 
                // should return but it is good to fill the content view if it is not filled with blocks
                    if (node.Index > lastBlock.Node.Index + 1)
                    {
                        if (!stripControl.IsContentViewFilledWithBlocks )  CreatePhraseBlocksForFillingContentView(stripControl);
                        return null;
                    }
                }
            // else add block
            Block b = stripControl.AddBlockForNode ( node );
            if (b == null && !stripControl.IsStripsControlsWithinSafeCount)
            {
                if (lastBlock != null && lastBlock.Node.IsRooted && lastBlock.Node.Index >  node.Index 
                    && stripControl.FirstBlock != null && stripControl.FirstBlock.Node.IsRooted && stripControl.FirstBlock.Node.Index < node.Index)
                {
                    NodeSelection prevSelection = mProjectView.Selection;
                    stripControl.RemoveAllFollowingBlocks(stripControl.Node.PhraseChild(lastBlock.Node.Index - 1), false, false);
                    if (prevSelection != null && prevSelection.Node == lastBlock.Node) ManageSelectionChangeWhileScroll(mProjectView.Selection, stripControl);
                    b = stripControl.AddBlockForNode(node);
                }
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
            waveform_recording_control.invertColor = SystemInformation.HighContrast;   //@Onthefly
            waveform_recording_control.projectView = mProjectView;    //@Onthefly
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
                    if (e is ObjectAddedEventArgs<urakawa.core.TreeNode> &&
                        ((ObjectAddedEventArgs<urakawa.core.TreeNode>)e).m_AddedObject == node)
                        {
                        f ();
                        mProjectView.Presentation.Changed -= h;
                        }
                };
                mProjectView.Presentation.Changed += h;
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

        public void InitializeShortcutKeys ()
            {
            mShortcutKeys = new Dictionary<Keys, ProjectView.HandledShortcutKey> ();
            KeyboardShortcuts_Settings keyboardShortcuts = mProjectView.ObiForm.KeyboardShortcuts;

            mShortcutKeys[keyboardShortcuts.ContentView_SelectCompleteWaveform.Value] = delegate() { return mProjectView.TransportBar.MarkSelectionWholePhrase(); };
            //mShortcutKeys[Keys.A] = delegate () { return mProjectView.TransportBar.MarkSelectionWholePhrase (); };
            mShortcutKeys[keyboardShortcuts.ContentView_PlaySelectedWaveform.Value] = delegate() { return mProjectView.TransportBar.PreviewAudioSelection(); };
            //ShortcutKeys[Keys.C] = delegate() { return mProjectView.TransportBar.PreviewAudioSelection(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarNextSection.Value] = delegate() { return mProjectView.TransportBar.NextSection(); };
            //mShortcutKeys[Keys.H] = delegate() { return mProjectView.TransportBar.NextSection(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarPreviousSection.Value] = delegate() { return mProjectView.TransportBar.PrevSection(); };
            //mShortcutKeys[Keys.Shift | Keys.H] = delegate() { return mProjectView.TransportBar.PrevSection(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarPreviousPhrase.Value] = delegate() { return mProjectView.TransportBar.PrevPhrase(); };
            //mShortcutKeys[Keys.J] = delegate() { return mProjectView.TransportBar.PrevPhrase(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarNextPhrase.Value] = delegate() { return mProjectView.TransportBar.NextPhrase(); };
            //mShortcutKeys[Keys.K] = delegate() { return mProjectView.TransportBar.NextPhrase(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarNudgeForward.Value] = delegate() { return mProjectView.TransportBar.Nudge(TransportBar.Forward); };
            //mShortcutKeys[Keys.N] = delegate() { return mProjectView.TransportBar.Nudge(TransportBar.Forward); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarNudgeBackward.Value] = delegate() { return mProjectView.TransportBar.Nudge(TransportBar.Backward); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarFineNavigationOn.Value] = delegate() { return FineNavigationModeOn (); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarFineNavigationOff.Value] = delegate() { return FineNavigationModeOff(); };
            //mShortcutKeys[Keys.Shift | Keys.N] = delegate() { return mProjectView.TransportBar.Nudge(TransportBar.Backward); };
            mShortcutKeys[keyboardShortcuts.ContentView_MarkSelectionBeginTime.Value] = delegate() { return mProjectView.TransportBar.MarkSelectionBeginTime(); };
            //mShortcutKeys[Keys.OemOpenBrackets] = delegate() { return mProjectView.TransportBar.MarkSelectionBeginTime(); };
            mShortcutKeys[keyboardShortcuts.ContentView_MarkSelectionEndTime.Value] = delegate() { return mProjectView.TransportBar.MarkSelectionEndTime(); };
            mShortcutKeys[keyboardShortcuts.ContentView_ExpandAudioSelectionAtLeft.Value] = delegate() { return ExpandAudioSelectionAtLeft (); };
            mShortcutKeys[keyboardShortcuts.ContentView_ContractAudioSelectionAtLeft.Value] = delegate() { return ContractAudioSelectionAtLeft(); };
            //mShortcutKeys[Keys.OemCloseBrackets] = delegate() { return mProjectView.TransportBar.MarkSelectionEndTime(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarNextPage.Value] = delegate() { return mProjectView.TransportBar.NextPage(); };
           // mShortcutKeys[Keys.P] = delegate() { return mProjectView.TransportBar.NextPage(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarPreviousPage.Value] = delegate() { return mProjectView.TransportBar.PrevPage(); };
            //mShortcutKeys[Keys.Shift | Keys.P] = delegate() { return mProjectView.TransportBar.PrevPage(); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarPreviewFromAudioCursor.Value] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.From, TransportBar.UseAudioCursor); };
            //mShortcutKeys[Keys.V] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.From, TransportBar.UseAudioCursor); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarPreviewFromSelection.Value] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.From, TransportBar.UseSelection); };
            //mShortcutKeys[Keys.Shift | Keys.V] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.From, TransportBar.UseSelection); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarPreviewUptoAudioCursor.Value] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.Upto, TransportBar.UseAudioCursor); };
            //mShortcutKeys[Keys.X] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.Upto, TransportBar.UseAudioCursor); };
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarPreviewUptoSelection.Value] = delegate() { return mProjectView.TransportBar.Preview(TransportBar.Upto, TransportBar.UseSelection); };
            mShortcutKeys[keyboardShortcuts.ContentView_ZoomWaveformPanel.Value] = delegate() { return ShowZoomWaveformPanel(); };
            

            // playback shortcuts.

            mShortcutKeys[keyboardShortcuts.ContentView_FastPlayStepDown.Value] = FastPlayRateStepDown;
            mShortcutKeys[keyboardShortcuts.ContentView_FastPlayStepUp.Value] = FastPlayRateStepUp;
            mShortcutKeys[keyboardShortcuts.ContentView_FastPlayRateNormilize.Value] = FastPlayRateNormalise;
            mShortcutKeys[keyboardShortcuts.ContentView_FastPlayNormalizeWithElapseBack.Value] = FastPlayNormaliseWithLapseBack;
            mShortcutKeys[keyboardShortcuts.ContentView_FastPlayWithElapseForward.Value] = delegate() { return mProjectView.TransportBar.StepForward(); };
            mShortcutKeys[keyboardShortcuts.ContentView_MarkSelectionFromBeginningToTheCursor.Value] = MarkSelectionFromBeginningToTheCursor;
            mShortcutKeys[keyboardShortcuts.ContentView_MarkSelectionFromCursorToTheEnd.Value] = MarkSelectionFromCursorToTheEnd;
            /*mShortcutKeys[Keys.S] = FastPlayRateStepDown;
            mShortcutKeys[Keys.F] = FastPlayRateStepUp;
            mShortcutKeys[Keys.D] = FastPlayRateNormalise;
            mShortcutKeys[Keys.E] = FastPlayNormaliseWithLapseBack;
            mShortcutKeys[Keys.Shift | Keys.OemOpenBrackets] = MarkSelectionFromCursor;
            mShortcutKeys[Keys.Shift | Keys.OemCloseBrackets] = MarkSelectionToCursor;*/

            // Strips navigation
            mShortcutKeys[keyboardShortcuts.ContentView_SelectPrecedingPhrase.Value] = SelectPrecedingBlock;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectFollowingPhrase.Value] = SelectFollowingBlock;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectLastPhraseInStrip.Value] = SelectLastBlockInStrip;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectFirstPhraseInStrip.Value] = SelectFirstBlockInStrip;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectNextPagePhrase.Value] = SelectNextPageNode;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectPrecedingPagePhrase.Value] = SelectPrecedingPageNode;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectNextSpecialRolePhrase.Value] = SelectNextSpecialRoleNode;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectPrecedingSpecialRolePhrase.Value] = SelectPreviousSpecialRoleNode;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectNextEmptyPhrase.Value] = SelectNextEmptyNode;
            /*
            mShortcutKeys[Keys.Left] = SelectPrecedingBlock;
            mShortcutKeys[Keys.Right] = SelectFollowingBlock;
            mShortcutKeys[Keys.End] = SelectLastBlockInStrip;
            mShortcutKeys[Keys.Home] = SelectFirstBlockInStrip;
            mShortcutKeys[Keys.Control | Keys.PageDown] = SelectNextPageNode;
            mShortcutKeys[Keys.Control | Keys.PageUp] = SelectPrecedingPageNode;
            mShortcutKeys[Keys.F4] = SelectNextSpecialRoleNode;
            mShortcutKeys[Keys.Shift | Keys.F4] = SelectPreviousSpecialRoleNode;
            mShortcutKeys[Keys.Control | Keys.Alt | Keys.F4] = SelectNextEmptyNode;
             */

            mShortcutKeys[keyboardShortcuts.ContentView_SelectPrecedingStrip.Value] = SelectPreviousStrip;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectFollowingStrip.Value] = SelectNextStrip;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectFirstStrip.Value] = SelectFirstStrip;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectLastStrip.Value] = SelectLastStrip;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectFirstSkippableNode.Value] = SelectFirstSkippableNode;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectLastSkippableNode.Value] = SelectLastSkippableNode;
            /*
            mShortcutKeys[Keys.Control | Keys.Up] = SelectPreviousStrip;
            mShortcutKeys[Keys.Control | Keys.Down] = SelectNextStrip;
            mShortcutKeys[Keys.Control | Keys.Shift | Keys.Up] = SelectPreviousStrip;
            mShortcutKeys[Keys.Control | Keys.Shift | Keys.Down] = SelectNextStrip;
            mShortcutKeys[Keys.Control | Keys.Home] = SelectFirstStrip;
            mShortcutKeys[Keys.Control | Keys.End] = SelectLastStrip;
             */
            mShortcutKeys[keyboardShortcuts.ContentView_SelectUp.Value] = SelectUp;
           //mShortcutKeys[Keys.Escape] = SelectUp;

            // Control + arrows moves the strip cursor
            mShortcutKeys[keyboardShortcuts.ContentView_SelectPrecedingStripCursor.Value] = SelectPrecedingStripCursor;
            mShortcutKeys[keyboardShortcuts.ContentView_SelectFollowingStripCursor.Value] = SelectFollowingStripCursor;
            /*
            mShortcutKeys[Keys.Control | Keys.Left] = SelectPrecedingStripCursor;
            mShortcutKeys[Keys.Control | Keys.Right] = SelectFollowingStripCursor;
             */
            mShortcutKeys[keyboardShortcuts.ContentView_ScrollDown_LargeIncrementWithSelection.Value] = ScrollDown_LargeIncrementWithSelection;
            mShortcutKeys[keyboardShortcuts.ContentView_ScrollUp_LargeIncrementWithSelection.Value] = ScrollUp_LargeIncrementWithSelection;
            mShortcutKeys[keyboardShortcuts.ContentView_ScrollDown_SmallIncrementWithSelection.Value] = ArrowKey_Down;
            mShortcutKeys[keyboardShortcuts.ContentView_ScrollUp_SmallIncrementWithSelection.Value] = ArrowKey_Up;

            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarRecordSingleKey.Value] = mProjectView.TransportBar.Record_Button;
            mShortcutKeys[keyboardShortcuts.ContentView_TransportBarStopSingleKey.Value] = mProjectView.TransportBar.Stop ;
        }

        private bool CanUseKeys { get { return (mSelection == null || !(mSelection is TextSelection)) && !m_IsBlocksVisibilityProcessActive; } }
        
        protected override bool ProcessCmdKey ( ref Message msg, Keys key )
            {
                if (mProjectView.ObiForm.Settings.Project_OptimizeMemory &&  ShouldSkipKeyDueToMemoryOverload(key)) return true;
            if (CanUseKeys &&
                ((msg.Msg == ProjectView.WM_KEYDOWN) || (msg.Msg == ProjectView.WM_SYSKEYDOWN)) &&
                mShortcutKeys.ContainsKey ( key ) && mShortcutKeys[key] ()) return true;
            if (ProcessTabKeyInContentsView ( key )) return true;
            return base.ProcessCmdKey ( ref msg, key );
            }

            int m_KeysMillisecond = 0;
            Keys m_PrevKey;
            int m_KeyRepeatCount = 0;
        private bool ShouldSkipKeyDueToMemoryOverload(Keys key)
        {
            if (Settings == null || !Settings.Project_OptimizeMemory) return false;
            bool isArrowKeyPressed = (key == Keys.Left || key == Keys.Right || key == (Keys.Control|Keys.Left) || key == (Keys.Control|Keys.Right)) ;

            if (isArrowKeyPressed && m_PrevKey.ToString() == key.ToString()
                && mProjectView.Selection != null && mProjectView.Selection.Node is EmptyNode && !(mProjectView.Selection is AudioSelection) ) // key bypass should not happen for audio selection
            {//1
                m_KeyRepeatCount++;

                if (m_KeyRepeatCount > 70 )
                    //|| (m_KeyRepeatCount >40 && (key == Keys.Up || key == Keys.Down)))
                {//2
                    System.Diagnostics.PerformanceCounter ramPerformanceCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");

                    //Console.WriteLine("count " + m_KeyRepeatCount + " : " + m_KeysMillisecond);
                    if (ramPerformanceCounter.NextValue() < 100
                        && Math.Abs(DateTime.Now.Second - m_KeysMillisecond) < 2)
                    {//3
                        
                        //Console.WriteLine("count " + m_KeyRepeatCount + " : " + m_KeysMillisecond);
                            System.GC.GetTotalMemory(true);
                            System.GC.WaitForFullGCComplete(500);
                        }//-3
                        System.Media.SystemSounds.Beep.Play();
                        ramPerformanceCounter.Close();
                        m_KeysMillisecond = DateTime.Now.Second;
                        m_PrevKey = key;
                        return true;
                    
                        ramPerformanceCounter.Close();
                    
                    m_KeyRepeatCount = 0;
                }//-2

            }//-1
            else if (m_KeyRepeatCount > 71 || m_PrevKey != key)
            {
                m_KeyRepeatCount = 0;
            }
            m_KeysMillisecond = DateTime.Now.Second;
            m_PrevKey = key;
            return false;
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
            if (strip != null && strip.FirstBlock != null)
                {
                int index = f ( strip, mSelectedItem );
                
                if (index >= 0)
                    {
                    //mProjectView.Selection = new StripIndexSelection ( strip.Node, this, index ); //@singleSection: original
                    mProjectView.Selection = new StripIndexSelection ( strip.Node, this, index + strip.OffsetForFirstPhrase );//@singleSection: new
                    return true;
                    }
                }
            return false;
            }

        private bool SelectPrecedingBlock ()
            {
                if (mProjectView.TransportBar.FineNavigationModeForPhrase)
                {
                    return NudgeInFineNavigation(false);
                }
                if (!mProjectView.TransportBar.IsPlayerActive && !mProjectView.TransportBar.IsRecorderActive && m_PreviousSelectionForScroll != null) SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(null, true);

                                    ISelectableInContentView item = mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock : mSelectedItem  ;
                        if (item == null) return false;
               
            Strip strip = StripFor ( item);
            if (strip == null) return false;
            Block blockToSelect = strip.BlockBefore ( item );

            if (blockToSelect != null && strip.LastBlock != null)//blockToSelect will be null if blockToSelect index is negative(blcock in previous lot)
                {
                if ( !(item is Strip ))
                    {
                                mProjectView.Selection = new NodeSelection ( blockToSelect.Node, this );
                return true;
                    }
                }
            else if (strip.FirstBlock != null && strip.FirstBlock.Node.Index > 0)   //if last block is block to be selected that means the block before first block should be selected.
                {
                    if (mProjectView.TransportBar.IsRecorderActive) return true;
                                EmptyNode nodeToSelect = (EmptyNode)strip.FirstBlock.Node.PrecedingNode;
                CreateBlocksInPreviousThresholdsSlot ();//@singleSection
                mProjectView.Selection = new NodeSelection ( nodeToSelect, this );
                return true;
                }
                
            return blockToSelect == null;
            //return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.BlockBefore ( mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock : item ); } );
            }

        private bool SelectPrecedingStripCursor ()
            {
                if (!mProjectView.TransportBar.IsPlayerActive && !mProjectView.TransportBar.IsRecorderActive && m_PreviousSelectionForScroll != null) SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(null, true);

            bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
            Block PlaybackBlock = null;
            if (mProjectView.TransportBar.CanUsePlaybackSelection)
                {
                PlaybackBlock = mPlaybackBlock;
                mProjectView.TransportBar.Stop ();
                }

                ISelectableInContentView item = PlaybackBlock != null ? mPlaybackBlock : mSelectedItem;
                if (item == null)
                {
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
                    return false;
                }
                Strip strip = StripFor(item);
                if (strip == null)
                {
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
                    return false;
                }

                int nodeIndexOfStripToSelect = -1;
                if (mProjectView.Selection != null && mProjectView.Selection is StripIndexSelection && strip.OffsetForFirstPhrase > 0
                    && ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection!= null  && ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection.Index == strip.OffsetForFirstPhrase && !mProjectView.TransportBar.IsRecorderActive)
                {
                    nodeIndexOfStripToSelect = strip.OffsetForFirstPhrase - 1;
                    CreateBlocksInPreviousThresholdsSlot();//@singleSection
                    nodeIndexOfStripToSelect = nodeIndexOfStripToSelect - strip.OffsetForFirstPhrase;
                    mProjectView.Selection = new StripIndexSelection(strip.Node, this, nodeIndexOfStripToSelect + strip.OffsetForFirstPhrase);
                }
                else
                {
                    nodeIndexOfStripToSelect = strip.StripIndexBefore(item);
                }
            //bool ReturnVal = SelectStripCursorFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.StripIndexBefore ( PlaybackBlock != null ? PlaybackBlock : item ); } );
                bool ReturnVal = nodeIndexOfStripToSelect != -1 ? SelectStripCursorFor(delegate(Strip strip1, ISelectableInContentView item1) { return nodeIndexOfStripToSelect; }):
                    false;
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;

            return ReturnVal;
            }

        //@singleSection
        private void CreateBlocksInPreviousThresholdsSlot ()
            {
            EmptyNode currentlySelectedNode = mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock.Node :
                mProjectView.Selection != null && mProjectView.Selection.Node is EmptyNode ? (EmptyNode)mProjectView.Selection.Node :
                mProjectView.Selection != null && mProjectView.Selection is StripIndexSelection ? ((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection : null;

            if (currentlySelectedNode != null)
                {
                //Console.WriteLine ( "currently selected node in blocks : " + currentlySelectedNode );
                Strip s = FindStrip ( currentlySelectedNode.ParentAs<SectionNode> () );
                if (currentlySelectedNode.Index > 0 && s != null && s.OffsetForFirstPhrase == currentlySelectedNode.Index)
                    {
                    if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Pause ();

                    CreateBlocksTillNodeInStrip ( s, (EmptyNode)currentlySelectedNode.PrecedingNode, false );
                    //Console.WriteLine ( "creating node till : " + currentlySelectedNode.PrecedingNode.Index );

                    }
                }
            }

        private bool SelectFollowingBlock ()
            {
                if (mProjectView.TransportBar.FineNavigationModeForPhrase)
                {
                    return NudgeInFineNavigation(true);
                }
                if (!mProjectView.TransportBar.IsPlayerActive && !mProjectView.TransportBar.IsRecorderActive && m_PreviousSelectionForScroll != null) SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(null, true);
                if (mProjectView.TransportBar.IsRecorderActive && mSelectedItem == null && mProjectView.Selection != null)
                {
                    Strip currentlyActiveStrip = ActiveStrip;
                    if (currentlyActiveStrip != null && currentlyActiveStrip.FirstBlock != null) mProjectView.SelectedBlockNode = currentlyActiveStrip.FirstBlock.Node;
                    return true;
                }

                ObiNode currentlySelectedNode = mProjectView.TransportBar.IsPlayerActive ? mProjectView.TransportBar.PlaybackPhrase : 
                    mProjectView.Selection != null ? mProjectView.Selection.Node : null;
                
                if (mSelectedItem != null &&  mSelectedItem is Strip 
                    && currentlySelectedNode != null &&  currentlySelectedNode is SectionNode && ((SectionNode)currentlySelectedNode).PhraseChildCount > 0)
                {
                    return SelectFirstBlockInStrip();
                }
                else
                {
                    if (mProjectView.TransportBar.IsPlayerActive  && currentlySelectedNode != null && currentlySelectedNode is EmptyNode
                        &&    (mPlaybackBlock == null || currentlySelectedNode != mPlaybackBlock.Node || (mSelectedItem is Strip && mProjectView.GetSelectedPhraseSection.PhraseChildCount == 1)))
                    {
                        
                        SelectPhraseBlockOrStrip((EmptyNode) currentlySelectedNode);
                    }
                    bool returnVal = SelectBlockFor(delegate(Strip strip, ISelectableInContentView item) { return strip.BlockAfter(mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock != null ? mPlaybackBlock : item); });
                    if (!returnVal && mSelectedItem != null && mSelectedItem is Block || mSelectedItem is StripCursor) returnVal = true;
                    return returnVal;
                }
            }

        public bool NudgeInFineNavigation (bool forward)
        {
            if (forward)
            {
                if (mProjectView.Selection is AudioSelection && !((AudioSelection)mProjectView.Selection).AudioRange.HasCursor)
                    return mProjectView.TransportBar.NudgeSelectedAudio(TransportBar.NudgeSelection.ExpandAtRight);
                else
                    return mProjectView.TransportBar.Nudge(TransportBar.Forward);
            }
            else
            {
                if (mProjectView.Selection is AudioSelection && !((AudioSelection)mProjectView.Selection).AudioRange.HasCursor)
                    return mProjectView.TransportBar.NudgeSelectedAudio(TransportBar.NudgeSelection.ContractAtRight);
                else
                    return mProjectView.TransportBar.Nudge(TransportBar.Backward); 
            }
        }

        private bool SelectFollowingStripCursor ()
            {
                if (!mProjectView.TransportBar.IsPlayerActive && !mProjectView.TransportBar.IsRecorderActive && m_PreviousSelectionForScroll != null) SelectPreviouslySelectedEmptyNodeForScrollSelectionChange(null, true);
            // if recorder is active and selection is at last strip cursor, return
                if (mProjectView.TransportBar.IsRecorderActive && mProjectView.Selection != null && mProjectView.Selection is StripIndexSelection && (((StripIndexSelection)mProjectView.Selection).EmptyNodeForSelection == null || FindBlock(mProjectView.Selection.EmptyNodeForSelection) == null)) return true;
            bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
            Block PlaybackBlock = null;
            if (mProjectView.TransportBar.CanUsePlaybackSelection)
                {
                PlaybackBlock = mPlaybackBlock;
                mProjectView.TransportBar.Stop ();
                }
            // if playback not active, first strip index should be selected.
                if (PlaybackBlock == null && mSelectedItem != null && mSelectedItem is Strip )
                {
                    ScrollStripsPanel_Top();
                }
            bool ReturnVal = SelectStripCursorFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.StripIndexAfter ( PlaybackBlock != null ? PlaybackBlock : item ); } );
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
            return ReturnVal;
            }

        private bool SelectLastBlockInStrip ()
            {
            if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
            //if (mProjectView.Selection != null) CreateBlocksTillEndInStrip ( mStrips[mProjectView.GetSelectedPhraseSection] );//@singleSection//@singleSection:original

            //return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return strip.LastBlock; } );//@singleSection:commented

            if (mProjectView.Selection != null && mProjectView.GetSelectedPhraseSection.PhraseChildCount > 0) //@singleSection
                {
                SectionNode section = mProjectView.GetSelectedPhraseSection;
                SelectPhraseBlockOrStrip ( section.PhraseChild ( section.PhraseChildCount - 1 ) );
                //verticalScrollToolStripContainer1.TrackBarValueInPercentage = 100;
                return true;
                }
            return false;
            }

        private bool SelectFirstBlockInStrip ()
            {
            if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
            //if (mProjectView.Selection != null && mProjectView.GetSelectedPhraseSection.PhraseChildCount >0) CreateBlocksTillNodeInStrip( mStrips[mProjectView.GetSelectedPhraseSection], mProjectView.GetSelectedPhraseSection.PhraseChild(0),true );//@singleSection
            if (mProjectView.Selection != null && mProjectView.GetSelectedPhraseSection.PhraseChildCount > 0) //@singleSection
                {
                SelectPhraseBlockOrStrip ( mProjectView.GetSelectedPhraseSection.PhraseChild ( 0 ) );
                if ( !mProjectView.TransportBar.IsRecorderActive)  mStripsPanel.Location = new Point ( mStripsPanel.Location.X, 0 );
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
                || (mProjectView.Selection != null && !(mProjectView.Selection.Node is SectionNode))))
                {
                mProjectView.TransportBar.MoveSelectionToPlaybackPhrase ();
                }

            if (mProjectView.GetSelectedPhraseSection == null) return false;
            SectionNode previousSection = mProjectView.GetSelectedPhraseSection.PrecedingSection; //@singleSection
            if (RestrictDynamicLoadingForRecording ( mProjectView.GetSelectedPhraseSection )) return true;
            if (previousSection != null && mProjectView.Selection.Node is SectionNode) CreateStripForSelectedSection ( previousSection, true ); //@singleSection

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

            if (currentlySelectedSection == null) return false;

            SectionNode nextSection = currentlySelectedSection.FollowingSection;
            if (RestrictDynamicLoadingForRecording ( currentlySelectedSection )) return true;
            if (mProjectView.TransportBar.IsPlayerActive && nextSection != null) mProjectView.TransportBar.Stop ();

            if (nextSection != null &&
                (mProjectView.Selection == null ||
                (mProjectView.Selection != null && (mProjectView.Selection.Node is EmptyNode || mProjectView.Selection is StripIndexSelection))))
                {
                    bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;

                mProjectView.Selection = new NodeSelection ( currentlySelectedSection, this );
                if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Stop ();
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip)
                        {
                        if (((Strip)c).Node == currentlySelectedSection) ((Strip)c).FocusStripLabel ();
                        }
                    }
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
                }
            if (nextSection != null) CreateStripForSelectedSection ( nextSection, true ); //@singleSection: ends

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
            Strip currentlyActiveStrip = ActiveStrip;
            if (currentlyActiveStrip != null && RestrictDynamicLoadingForRecording(currentlyActiveStrip.Node)) return true;
            if (section != null) CreateStripForSelectedSection ( section, true ); //@singleSection

            return SelectStripFor ( delegate ( Strip strip )
{
    return mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[0] : null;
} );
            }

        private bool SelectLastStrip ()
            {

            ObiNode n = null;
            for (n = ((ObiRootNode)mProjectView.Presentation.RootNode).LastLeaf;
                !(n is SectionNode);
                n = n.PrecedingNode)
            { }

            SectionNode section = (SectionNode)n;
            Strip currentlyActiveStrip = ActiveStrip;
            if (currentlyActiveStrip != null && RestrictDynamicLoadingForRecording(currentlyActiveStrip.Node)) return true;
            if (mProjectView.TransportBar.IsPlayerActive && section != null) mProjectView.TransportBar.Stop ();
            if (mProjectView.Selection != null && mProjectView.Selection.Node is PhraseNode && section != null)
                {
                    bool SelectionChangedPlaybackEnabledStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                mProjectView.Selection = new NodeSelection ( mProjectView.GetSelectedPhraseSection, this );
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip)
                        {
                        if (((Strip)c).Node == mProjectView.GetSelectedPhraseSection) ((Strip)c).FocusStripLabel ();
                        }
                    }
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabledStatus;
                }
            if (section != null) CreateStripForSelectedSection ( section, true ); //@singleSection: ends

            return SelectStripFor ( delegate ( Strip strip )
{
    return mStripsPanel.Controls.Count > 0 ? (Strip)mStripsPanel.Controls[mStripsPanel.Controls.Count - 1] : null;
} );
            }

        // Select the item above the currently selected item.
        // E.g. from an audio selection a phrase, from a phrase a strip, from a strip nothing.
        private bool SelectUp ()
            {
                if (mProjectView.TransportBar.FineNavigationModeForPhrase)
                {
                    mProjectView.TransportBar.FineNavigationModeForPhrase = false;
                    return true;
                }
            else if (mSelection is AudioSelection)
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
                //mProjectView.Selection = null; //@singleSection: esc key should not end up in deselecting everything
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

        private bool FineNavigationModeOn()
        {
            if (mProjectView.TransportBar.CanEnterFineNavigationMode )
            {
                mProjectView.TransportBar.FineNavigationModeForPhrase = true;
                return true;
            }
            return false;
        }

        private bool FineNavigationModeOff()
        {
            if (mProjectView.TransportBar.FineNavigationModeForPhrase)
            {
                mProjectView.TransportBar.FineNavigationModeForPhrase = false;
                return true;
            }
            return false;
        }

        private bool ExpandAudioSelectionAtLeft()
        {
            if (mProjectView.TransportBar.FineNavigationModeForPhrase)
            {
                return mProjectView.TransportBar.NudgeSelectedAudio(TransportBar.NudgeSelection.ExpandAtLeft);
            }
            return false;
        }

        private bool ContractAudioSelectionAtLeft()
        {
            if (mProjectView.TransportBar.FineNavigationModeForPhrase)
            {
                return mProjectView.TransportBar.NudgeSelectedAudio(TransportBar.NudgeSelection.ContractAtLeft);
            }
            return false;
        }

        /// <summary>
        /// returns active current node from transport bar if player is active else return selected node from project view
        /// </summary>
        private ObiNode SelectedNodeInTransportbarOrProjectview
            {
            get
                {
                if (mProjectView.TransportBar.IsPlayerActive)
                    return mPlaybackBlock != null ? mPlaybackBlock.Node : null;
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
                            if (((EmptyNode)n).Role_ == EmptyNode.Role.Custom && EmptyNode.SkippableNamesList.Contains(((EmptyNode)n).CustomRole))
                            {
                                SelectPhraseBlockOrStrip((EmptyNode)n); // @phraseLimit
                                SelectFirstSkippableNode();
                            }
                            else
                            {
                                SelectPhraseBlockOrStrip((EmptyNode)n); // @phraseLimit
                            }
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
                        EmptyNode preceedingNode = (n.PrecedingNode!= null && n.PrecedingNode is EmptyNode)? (EmptyNode)n.PrecedingNode: null ;
                        if ( preceedingNode != null && preceedingNode.Role_ == EmptyNode.Role.Custom && EmptyNode.SkippableNamesList.Contains( preceedingNode.CustomRole) 
                            &&  ((EmptyNode)n).CustomRole == preceedingNode.CustomRole ) 
                        {
                            continue ;
                        }
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
                for (ObiNode n = ((ObiRootNode)mProjectView.Presentation.RootNode).FirstLeaf; n != null; n = n.FollowingNode)
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
                for (ObiNode n = ((ObiRootNode)mProjectView.Presentation.RootNode).LastLeaf; n != null; n = n.PrecedingNode)
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
                            if ((n is EmptyNode && !(n is PhraseNode))
                                || (n is PhraseNode && ((PhraseNode)n).Duration == 0))
                                {
                                //mProjectView.Selection = new NodeSelection ( n, this );
                                SelectPhraseBlockOrStrip ( (EmptyNode)n ); // @phraseLimit
                                return true;
                                }
                            }
                        }
                    for (ObiNode n = ((ObiRootNode)mProjectView.Presentation.RootNode).FirstLeaf; n != null; n = n.FollowingNode)
                        {
                        if (n is EmptyNode && !(n is PhraseNode)
                            || (n is PhraseNode && ((PhraseNode)n).Duration == 0))
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

        private bool SelectFirstSkippableNode () { return mProjectView.GotoSkippableNoteEnds(true ) ; }
        private bool SelectLastSkippableNode () { return mProjectView.GotoSkippableNoteEnds(false ) ; }


        // @phraseLimit
        public void SelectPhraseBlockOrStrip ( EmptyNode node )
            {
            if (node != null)
                {
                //@singleSection: all of the part in this if block refactored, old part is commented below
                // no need to change anything in functions like next / previous page, todo, special node etc. changing this function did the behaviour of single section
                SectionNode parentSection = node.ParentAs<SectionNode> ();
                bool isParentSectionVisible = false;
                Strip strip = null;

                //check if strip layout contains this section strip
                foreach (Control c in mStripsPanel.Controls)
                    {
                    if (c is Strip)
                        {
                        strip = ((Strip)c);
                        if (strip.Node == parentSection) isParentSectionVisible = true;
                        }
                    }
                //restrict dynamic loading for recording check
                if (strip != null &&  RestrictDynamicLoadingForRecording ( strip.Node ))
                    {
                    if (mProjectView.Selection == null
                        || (mProjectView.Selection != null && mProjectView.TransportBar.RecordingPhrase != null && mProjectView.Selection.Node != mProjectView.TransportBar.RecordingPhrase))
                        {
                        mProjectView.Selection = new NodeSelection ( mProjectView.TransportBar.RecordingPhrase, this );
                        }
                    return;
                    }

                if (!isParentSectionVisible)
                    {
                    if (MessageBox.Show ( Localizer.Message ("SelectPhraseOrSection_ShowSection"), "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes)
                        {
                        CreateStripForSelectedSection ( parentSection, true );
                        }
                    else
                        {
                        return;
                        }
                    }

                if (node != null && strip != null && strip.FindBlock ( node ) == null)
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

                    if (strip != null)
                        {
                        this.Cursor = Cursors.WaitCursor;
                        bool playOnNavigateStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                        mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                        IsScrollActive = true;
                        try
                            {
                            CreateBlocksTillNodeInStrip ( strip, (EmptyNode)node, false );
                            }
                        catch (System.Exception ex)
                            {
                            MessageBox.Show ( ex.ToString () );
                            }
                        

                        this.Cursor = Cursors.Default;
                        IsScrollActive = false;
                        mProjectView.TransportBar.SelectionChangedPlaybackEnabled = playOnNavigateStatus;
                        }
                    }

                if (node != null) mProjectView.Selection = new NodeSelection ( node, this );
                
                //if playback is active, update playback block
                if (mProjectView.TransportBar.IsPlayerActive && mPlaybackBlock == null)
                    {
                    //SetPlaybackPhraseAndTime ( mProjectView.TransportBar.PlaybackPhrase, mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                    mProjectView.SetPlaybackBlockIfRequired ();
                    }

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
                        return ((Strip)c);
                        }
                    }
                return null;
                }
            }

        //@singleSection
        public bool RestrictDynamicLoadingForRecording ( SectionNode sectionShown )
            {
            if (mProjectView.TransportBar.IsRecorderActive
                && mProjectView.TransportBar.RecordingSection != null
                && sectionShown != null
                && mProjectView.TransportBar.RecordingSection == sectionShown)
                {
                return true;
                }
            return false;
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

        private bool MarkSelectionFromBeginningToTheCursor ()
            {
            return mProjectView.TransportBar.MarkSelectionFromBeginningToTheCursor ();
            }

        private bool MarkSelectionFromCursorToTheEnd ()
            {
            return mProjectView.TransportBar.MarkSelectionFromCursorToTheEnd ();
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
                        //SelectFirstStrip ();
                        s.FocusStripLabel (); //@singleSection
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
                        /* //@singleSection
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
                         */
                        if (s.LastBlock != null)//@singleSection
                            {
                            System.Media.SystemSounds.Beep.Play ();
                            return SelectBlockFor ( delegate ( Strip strip, ISelectableInContentView item ) { return s.LastBlock; } );
                            }
                        else
                            {
                            System.Media.SystemSounds.Beep.Play ();
                            s.FocusStripLabel ();
                            return true;
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
            Context_AddSectionMenuItem.Enabled = mProjectView.CanAddSection && !IsZoomWaveformActive && !mProjectView.TransportBar.IsListening;
            Context_InsertSectionMenuItem.Enabled = mProjectView.CanInsertSection;
            Context_SplitSectionMenuItem.Enabled = CanSplitStrip && !mProjectView.TransportBar.IsRecorderActive && !IsZoomWaveformActive;
           // Context_MergeSectionMenuItem.Enabled = mProjectView.CanMergeStripWithNext && !IsZoomWaveformActive;
            Context_MergeWithNextSectionMenuItem.Enabled = mProjectView.CanMergeStripWithNext && !IsZoomWaveformActive;
            Context_MultiSectionOperations.Enabled = mProjectView.EnableMultiSectionOperation;
            Context_AddBlankPhraseMenuItem.Enabled = mProjectView.CanAddEmptyBlock && !mProjectView.TransportBar.IsRecorderActive;
            Context_AddEmptyPagesMenuItem.Enabled = mProjectView.CanAddEmptyBlock && !mProjectView.TransportBar.IsRecorderActive;
            Context_ImportAudioFilesMenuItem.Enabled = mProjectView.CanImportPhrases;
            Context_FineNavigationMenuItem.Enabled = (mProjectView.TransportBar.FineNavigationModeForPhrase || mProjectView.TransportBar.CanEnterFineNavigationMode) && !IsZoomWaveformActive;
            Context_FineNavigationMenuItem.Checked= mProjectView.TransportBar.FineNavigationModeForPhrase;
            Context_SplitPhraseMenuItem.Enabled = mProjectView.CanSplitPhrase && !mProjectView.TransportBar.IsRecorderActive;
            Context_MergePhraseWithNextMenuItem.Enabled = CanMergeBlockWithNext;
            Context_SplitMergeWithNextMenuItem.Enabled = mProjectView.CanSplitPhrase ;
            Context_SplitMergeWithPreviousMenuItem.Enabled = mProjectView.CanSplitPhrase ;
            Context_MergeMenuItem.Enabled = mProjectView.Presentation != null && (IsBlockOrWaveformSelected || IsZoomWaveformActive) && mProjectView.GetSelectedPhraseSection != null && mProjectView.GetSelectedPhraseSection.PhraseChildCount > 1 && !mProjectView.TransportBar.IsRecorderActive;
            Context_CropAudioMenuItem.Enabled = mProjectView.CanCropPhrase;
            //Context_PhraseIsTODOMenuItem.Enabled = mProjectView.CanSetTODOStatus && !mProjectView.TransportBar.IsActive;
            Context_PhraseIsTODOMenuItem.Enabled = mProjectView.CanSetTODOStatus; // made consistent with drop down menu. if not suitable the commented lines around can be restored.
            Context_PhraseIsTODOMenuItem.Checked = mProjectView.IsCurrentBlockTODO;
            Context_PhraseIsUsedMenuItem.Enabled = CanSetSelectedPhraseUsedStatus;
            Context_PhraseIsUsedMenuItem.Checked = mProjectView.IsBlockUsed;
            Context_AssignRoleMenuItem.Enabled = mProjectView.CanAssignARole;
            Context_AssignRole_PlainMenuItem.Enabled = mProjectView.CanAssignPlainRole;
            Context_AssignRole_HeadingMenuItem.Enabled = mProjectView.CanAssignHeadingRole;
            Context_AssignRole_PageMenuItem.Enabled = mProjectView.CanAssignARole;
            Context_AssignRole_SilenceMenuItem.Enabled = mProjectView.CanAssignSilenceRole;
            Context_AssignRole_NewCustomRoleMenuItem.Enabled = mProjectView.CanAssignARole;
            Context_AssignRole_AnchorMenuItem.Enabled = mProjectView.CanAssignAnchorRole && !mProjectView.TransportBar.IsRecorderActive;
            Context_ClearRoleMenuItem.Enabled = mProjectView.CanAssignPlainRole;
            Context_ApplyPhraseDetectionMenuItem.Enabled = mProjectView.CanApplyPhraseDetection;
            Context_PhraseDetection_ApplyPhraseDetectionInProjectMenuItem.Enabled = mProjectView.CanApplyPhraseDetectionInWholeProject;
            Context_CutMenuItem.Enabled = (CanRemoveAudio || CanRemoveBlock || CanRemoveStrip) && !mProjectView.TransportBar.IsRecorderActive;
            Context_CopyMenuItem.Enabled = (CanCopyAudio || CanCopyBlock || CanCopyStrip) && !mProjectView.TransportBar.IsRecorderActive;
            Context_PasteMenuItem.Enabled = mProjectView.CanPaste;
            Context_PasteBeforeMenuItem.Enabled = mProjectView.CanPasteBefore;
            Context_PasteInsideMenuItem.Enabled = mProjectView.CanPasteInside;
          Context_Delete_deleteSelectionMenuItem.Enabled = (CanRemoveAudio || CanRemoveBlock || CanRemoveStrip) && !mProjectView.TransportBar.IsRecorderActive;
            Context_AudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
            Context_AudioSelection_BeginMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
            Context_AudioSelection_EndMenuItem.Enabled = mProjectView.CanMarkSelectionEnd;
            Context_PropertiesMenuItem.Enabled = mProjectView.CanShowSectionPropertiesDialog ||
                mProjectView.CanShowPhrasePropertiesDialog || mProjectView.CanShowProjectPropertiesDialog;
            Context_PhraseDetection_ApplyPhraseDetectionInProjectMenuItem.Enabled = mProjectView.CanApplyPhraseDetectionInWholeProject;
            Context_Merge_MergeWithFollowingPhrasesMenuItem.Enabled = mProjectView.CanMergePhraseWithFollowingPhrasesInSection;
            Context_Merge_MergeWithPrecedingPhrasesMenuItem.Enabled = mProjectView.CanMergeWithPhrasesBeforeInSection;
            Context_Delete_deleteFollowingPhrasesMenuItem.Enabled = mProjectView.CanDeleteFollowingPhrasesInSection;
            Context_ExportReplaceAudioMenuItem.Enabled = mProjectView.CanExportSelectedNodeAudio;
            Context_SkippablesMenuItem.Enabled = !IsZoomWaveformActive;
            Context_Skippable_BeginSpecialNodeMarkToolStripMenuItem.Enabled = mProjectView.CanBeginSpecialNote; //@AssociateNode
         // Context_Skippable_EndSpecialNodeMarkToolStripMenuItem.Enabled = mProjectView.Presentation != null && !mProjectView.TransportBar.IsRecorderActive && mProjectView.Selection != null && m_BeginNote != null && mProjectView.Selection.Node is EmptyNode && m_BeginNote != mProjectView.Selection.Node && mProjectView.Selection.Node.ParentAs<SectionNode>() == m_BeginNote.ParentAs<SectionNode>(); //@AssociateNode
            Context_Skippable_EndSpecialNodeMarkToolStripMenuItem.Enabled = mProjectView.CanEndSpecialNote;
            Context_Skippable_GotoAssociatedNodeToolStripMenuItem.Enabled = mProjectView.CanGotoSkippableNote; //@AssociateNode
            Context_Skippable_MoveToEndNoteToolStripMenuItem.Enabled = mProjectView.CanMoveToEndNote;   //@AssociateNode
            Context_Skippable_MoveToStartNoteToolStripMenuItem.Enabled = mProjectView.CanMoveToStartNote;  //@AssociateNode
            Context_Skippable_RemoveAssociatedNodeToolStripMenuItem.Enabled = mProjectView.CanRemoveSkippableNode; //@AssociateNode
            //Context_Skippable_AssociateSpecialNodeMarkToolStripMenuItem.Enabled = mProjectView != null && !mProjectView.TransportBar.IsRecorderActive && (mProjectView.Selection == null || (mProjectView.Selection != null && mProjectView.Selection.Node is EmptyNode && ((EmptyNode)mProjectView.Selection.Node).Role_ != EmptyNode.Role.Custom && ((EmptyNode)mProjectView.Selection.Node).Role_ != EmptyNode.Role.Plain));
            Context_Skippable_AssociateSpecialNodeMarkToolStripMenuItem.Enabled = mProjectView.CanAssociateNode;   //@AssociateNode
            Context_Skippable_ClearRoleFromNoteToolStripMenuItem.Enabled = mProjectView.CanClearSkippableRole;
            Context_GenerateSpeechForPageMenuItem.Enabled = mProjectView.CanGenerateSpeechForPage;
            Context_SettingsFromsilencePhraseToolStripMenuItem.Enabled = mProjectView.CanUpdatePhraseDetectionSettingsFromSilencePhrase;
            Context_ReplaceAudioMenuItem.Enabled = mProjectView.CanExportSelectedNodeAudio;
            Context_AudioProcessing.Enabled = mProjectView.CanExportSelectedNodeAudio;
            }

        private bool CanSetSelectedPhraseUsedStatus
            {
            get
                {
                //return IsBlockSelected && SelectedEmptyNode.AncestorAs<SectionNode> ().Used;
                    return IsBlockOrWaveformSelected &&   mProjectView.Selection.Node is EmptyNode  && mProjectView.Selection.Node.AncestorAs<SectionNode>().Used;
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

        // Merge phrase context menu item
        private void Context_MergePhraseWithNextMenuItem_Click ( object sender, EventArgs e ) { mProjectView.MergeBlockWithNext (); }

        private void Context_Merge_MergeWithFollowingPhrasesMenuItem_Click ( object sender, EventArgs e )
            {
            mProjectView.MergePhraseWithFollowingPhrasesInSection ();
            }

        private void Context_Merge_MergeWithPrecedingPhrasesMenuItem_Click ( object sender, EventArgs e )
            {
            mProjectView.MergeWithPhrasesBeforeInSection ();
            }

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

        private void Context_AssignRole_AnchorMenuItem_Click(object sender, EventArgs e)   //@AssociateNode
        {
            // mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Anchor, "Anchor");     // @Anchor
            //((EmptyNode)mProjectView.Selection.Node).Role_ = EmptyNode.Role.Anchor;
            mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Anchor, null);
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

        private void Context_PhraseDetection_ApplyPhraseDetectionInProjectMenuItem_Click ( object sender, EventArgs e )
            {
            mProjectView.ApplyPhraseDetectionInWholeProject ();
            }

        // Delete following phrases context menu item
        private void Context_DeleteFollowingPhrasesMenuItem_Click ( object sender, EventArgs e )
            {  }

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
        private void Context_DeleteMenuItem_Click ( object sender, EventArgs e ) {  }

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
            Strip currentlyActiveStrip = ActiveStrip;
            if (currentlyActiveStrip != null)
                {
                int indexOfPhraseToBeShown = Convert.ToInt32 ( (scrollValue * currentlyActiveStrip.Node.PhraseChildCount) / currentlyActiveStrip.PredictedStripHeight );
                //Console.WriteLine ( "Index of phrase to be shown for verticle scroll " + indexOfPhraseToBeShown );
                CreateBlocksTillNodeInStrip ( currentlyActiveStrip,
                    currentlyActiveStrip.Node.PhraseChild ( indexOfPhraseToBeShown ),
                    false );
                // adjust location of strips panel such that the phrase blocks at end are shown.
                if (mStripsPanel.Height > (this.Height - mHScrollBar.Location.Y))
                    {
                    int stripsPanelYLocation = this.Height - mStripsPanel.Height;
                    if (indexOfPhraseToBeShown == 0)
                        {
                        stripsPanelYLocation = 0;

                        }
                    mStripsPanel.Location = new Point ( mStripsPanel.Location.X, stripsPanelYLocation );
                    //Console.WriteLine ( "Y location of strip panel after scroll is " + stripsPanelYLocation + " " + mStripsPanel.Size );
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
            m_ScrolBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler ( m_ScrolBackgroundWorker_RunWorkerCompleted );
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

            CreateBlocksTillNodeInStrip ( s, null, true );
            }

        //@singleSection
        private void mVScrollBar_Scroll ( object sender, ScrollEventArgs e )
            {
            mScroll = true;
            this.mVScrollBar.Maximum = PredictedMaxStripsLayoutHeight;
            int height;
            timer1.Start ();
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll
                && e.OldValue < e.NewValue)
                {// StartCreatingBlockForScroll (); 
                }

            height = mStripsPanel.Location.Y - PredictedMaxStripsLayoutHeight;
            if (PredictedMaxStripsLayoutHeight < this.Size.Height)
                {
                this.mStripsPanel.Location = new Point ( mStripsPanel.Location.X, this.Size.Height + height );
                }
            }

        //@singleSection
        private void ContentView_Resize ( object sender, EventArgs e )
            {
            if (ActiveStrip != null)
                {
                //CreateLimitedBlocksInStrip ( ActiveStrip, null );
                }
            //this.contentViewLabel1.Size = new Size(this.Size.Width + this.mVScrollBar.Width, 22);
            this.verticalScrollToolStripContainer1.Location = new Point(this.Width - verticalScrollToolStripContainer1.Width, 0);
            this.verticalScrollToolStripContainer1.Size = new Size(verticalScrollToolStripContainer1.Width, mHScrollBar.Location.Y);
            mCornerPanel.Location = new Point(this.verticalScrollToolStripContainer1.Location.X, mHScrollBar.Location.Y);
            this.waveform_recording_control.Size = new Size(this.Size.Width - verticalScrollToolStripContainer1.Width, Convert.ToInt32(104 * ZoomFactor));   //@Onthefly
            mCornerPanel.BringToFront();
            this.contentViewLabel1.BringToFront();
           }

        public void ResizeContentViewFromStripResize()
        {
            UpdateSize();
            if (mSelectedItem != null
                && (mSelectedItem is Block || mSelectedItem is StripCursor))
            {
                EnsureControlVisible((Control)mSelectedItem);

                Control c = ((Control)mSelectedItem);
                //int selectedControlY = LocationOfBlockInStripPanel(c).Y;

                //if (mStripsPanel.Location.Y + selectedControlY < 0) mStripsPanel.Location = new Point(mStripsPanel.Location.X, selectedControlY * -1);
                //else if (mStripsPanel.Location.Y + selectedControlY >= (mHScrollBar.Location.Y - c.Height)) mStripsPanel.Location = new Point(mStripsPanel.Location.X, (selectedControlY+c.Height - mHScrollBar.Location.Y) * -1);
            }
            
            if (ActiveStrip != null)
            {
                if ( mProjectView.Selection != null
                &&    ( mProjectView.Selection.Node is SectionNode && !(mProjectView.Selection is StripIndexSelection)))
            {
Block lastBlock = ActiveStrip.LastBlock ;
                    if ( lastBlock != null )
                    {
                        int selectedControlY = LocationOfBlockInStripPanel(lastBlock).Y;
                        if ( mStripsPanel.Location.Y + selectedControlY  < 0 )
                        {
                            mStripsPanel.Location = new Point ( mStripsPanel.Location.X ,(mStripsPanel.Height - mHScrollBar.Location.Y )*-1 ) ;
                        }
                    }
                    else
                    {
                        if (mSelectedItem is Strip)//if last block is null, strip should be considered
                        {
                            EnsureControlVisible((Control) mSelectedItem ) ;
                        }
                    }
            }
                /*
                // strip panel can also expand or contract so first try to pull down panel till content view label is not visible
                // create additional phrases after that

                int spaceBelowStripsPanel = mHScrollBar.Location.Y - (mStripsPanel.Location.Y + mStripsPanel.Height);
                //Console.WriteLine("resizing " + spaceBelowStripsPanel + " location " + Math.Abs(mStripsPanel.Location.Y));
                if (Math.Abs(mStripsPanel.Location.Y) - ActiveStrip.BlocksLayoutTopPosition > spaceBelowStripsPanel)
                {
                    mStripsPanel.Location = new Point(mStripsPanel.Location.X,
                        (mStripsPanel.Height - mHScrollBar.Location.Y) * -1);
                    //Console.WriteLine("changed location " + mStripsPanel.Location + " " + mHScrollBar.Location);
                    //System.Media.SystemSounds.Asterisk.Play();
                }
                else if (Math.Abs(mStripsPanel.Location.Y) > ActiveStrip.BlocksLayoutTopPosition)
                {
                    mStripsPanel.Location = new Point(mStripsPanel.Location.X, ActiveStrip.BlocksLayoutTopPosition * -1);
                }
                */
            if (CreateLimitedBlocksInStrip(ActiveStrip, null))
            {
                UpdateVerticalScrolPanelButtons();
            }
            }
            
            
        }

        //@singleSection
        public void EnsureVisibilityOfSelectedItem ()
    {
        if (mSelectedItem != null && mSelectedItem is Control)
        {
            EnsureControlVisible((Control)mSelectedItem);
        }
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

            if (currentlyActiveStrip == null)
                {
                if (mProjectView.Selection != null && mProjectView.Selection.Node is SectionNode && mProjectView.Selection.Control is TOCView)
                    {
                    CreateSelectedStripAndPhraseBlocks ( mProjectView.Selection );
                    }
                return;
                }

            if (mProjectView.GetSelectedPhraseSection == currentlyActiveStrip.Node)
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

        private void ContentView_MouseWheel ( object sender, MouseEventArgs e )
            {
            
            int interval;
            int increment = Convert.ToInt32 ( mHScrollBar.Location.Y * 0.4 );
            if (e.Delta < 0)
                interval = -e.Delta / 120;
            else
                interval = e.Delta / 120;
            if (e.Delta > 0)
                increment = increment * (-1);

            if (IsScrollActive)
            { }
            else
            {
                if (interval > 10)
                    interval = 10;
                ScrollMStripsPanel(increment * interval, false);
            }
            //Console.WriteLine ( "mouse wheel scrolling " + increment + " " + interval);
             
            //Console.WriteLine ( "mouse wheel " + e.Delta );
            }

        private void timer1_Tick ( object sender, EventArgs e )
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
            timer1.Stop ();
            if (mScroll)
                {
                mEnableScrolling = false;
                CreatePhrasesAccordingToVScrollBarValue ( mVScrollBar.Value );
                }
            }

        private void exportAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {  }

        private void Context_MergeSectionWithNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MergeStrips();
        }

        private void Context_MergeMultipleSectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MergeMultipleSections();
        }

        public Settings Settings { get { return mProjectView.ObiForm.Settings; } }

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
        public void VerifyFlowBreakMarks()
        {
            if (ActiveStrip != null)
            {
                ActiveStrip.VerifyFlowBreak();
            }
        }

        private void Context_Skippable_BeginSpecialNodeMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MarkBeginNote();
        }

        private void Context_Skippable_EndSpecialNodeMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MarkEndNote();
            mProjectView.AssignRoleToMarkedContinuousNodes(); //@AssociateNode
        }

        private void Context_Skippable_AssociateSpecialNodeMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.AssociateNodeToSpecialNode(); //@AssociateNode
        }

        private void Context_Skippable_GotoAssociatedNodeToolStripMenuItem_Click(object sender, EventArgs e)   //@AssociateNode
        {
            if (((EmptyNode)mProjectView.Selection.Node).AssociatedNode == null)
                MessageBox.Show(Localizer.Message( "Associate_node_with_anchor_node"));
            if (mProjectView.Selection.Node is EmptyNode && ((EmptyNode)mProjectView.Selection.Node).AssociatedNode != null)
                mProjectView.SelectedBlockNode = ((EmptyNode)mProjectView.Selection.Node).AssociatedNode;
        }

        private void Context_Skippable_RemoveAssociatedNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.DeassociateSpecialNode(); //@AssociateNode
        }

        private void mSkippableMoveToStartNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.GotoSkippableNoteEnds(true);  //@AssociateNode
        }

        private void mSkippableMoveToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.GotoSkippableNoteEnds(false);   //@AssociateNode
        }

        private void waveform_recording_control_Load(object sender, EventArgs e)
        {
            waveform_recording_control.Width = this.Size.Width - verticalScrollToolStripContainer1.Width;   //@Onthefly
        }  

        private void Context_FineNavigationMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.FineNavigationModeForPhrase =
                !mProjectView.TransportBar.FineNavigationModeForPhrase;
        }

        private void Context_Skippable_ClearRoleFromNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ClearSkippableChunk();
        }

        private void Context_GenerateSpeechForPageMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.GenerateSpeechForPage( false);
        }

        private void settingsFromsilencePhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.UpdatePhraseDetectionSettingsFromSilencePhrase();
        }

        private void Context_ReplaceAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {          
        }

        ////@zoomwaveform
        //private void ShowZoomWaveformPanel ()
        //{
        //    if (mProjectView.TransportBar.IsPlayerActive)
        //    {
        //        mProjectView.TransportBar.Pause();
        //    }
            
        //    if (mProjectView != null && mProjectView.Selection != null)
        //    {
        //        if (ActiveStrip != null && mProjectView.Selection.EmptyNodeForSelection != null)
        //        {
        //            m_ZoomWaveformPanel = new ZoomWaveform(this, ActiveStrip, mProjectView.Selection.EmptyNodeForSelection, mProjectView);
        //            this.Controls.Add(m_ZoomWaveformPanel);
        //            m_ZoomWaveformPanel.Location = new Point(0, 0);
        //            m_ZoomWaveformPanel.Show();
        //            m_ZoomWaveformPanel.BringToFront();
        //            m_ZoomWaveformPanel.Focus();
        //        }
        //    }
        //}

        //@zoomwaveform
        public void RemovePanel()
        {
            
            if (m_ZoomWaveformPanel != null)
            {
                this.Controls.Remove(m_ZoomWaveformPanel);
                m_ZoomWaveformPanel.Dispose();
                m_ZoomWaveformPanel = null;
                if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Pause();
                NodeSelection previousSelection = mProjectView.Selection is NodeSelection ? mProjectView.Selection : null;
                if (previousSelection != null)
                {
                    bool playOnNavigateStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                    mProjectView.Selection = new NodeSelection(previousSelection.Node, this);
                    if (previousSelection is AudioSelection)
                    {
                        mProjectView.Selection = new AudioSelection((PhraseNode)previousSelection.Node, this, ((AudioSelection)previousSelection).AudioRange);
                    }
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = playOnNavigateStatus;
                    if (mProjectView.TransportBar.IsPlayerActive) UpdateCursorPosition(mProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset);
                    EnsureVisibilityOfSelectedItem();
                }
                m_ZoomWaveformPanel = null;
            }
        }

        public void ZoomPanelZoomIn() { if (IsZoomWaveformActive) m_ZoomWaveformPanel.ZoomIn (); } //@zoomwaveform
        public void ZoomPanelZoomOut() { if (IsZoomWaveformActive) m_ZoomWaveformPanel.ZoomOut () ; } //@zoomWaveform
        public void ZoomPanelReset() { if (IsZoomWaveformActive) m_ZoomWaveformPanel.Reset () ; } //@zoomWaveform
        public void ZoomPanelToolTipInit() { if (IsZoomWaveformActive) m_ZoomWaveformPanel.ZoomPanelToolTipInit(); } //@zoomWaveform    
        public void ZoomPanelClose() { if (IsZoomWaveformActive) m_ZoomWaveformPanel.IsNewProjectOpened(); }

        //@zoomwaveform
        public void ShowEditPanel()
        {
            if (mProjectView == null || mProjectView.Selection == null)
            {
                m_Edit = new Toolbar_EditAudio();
            }
            else
            {
                m_Edit = new Toolbar_EditAudio(this, ActiveStrip, mProjectView.Selection.EmptyNodeForSelection, mProjectView);
            }
            this.Controls.Add(m_Edit);
            m_Edit.Show();
            //  m_Edit.Location = new Point(0, 0);
            m_Edit.BringToFront();
            m_Edit.Focus();

        }

        //@zoomwaveform
        public void RemoveEditPanel()
        {
            if (m_Edit != null)
            {
                this.Controls.Remove(m_Edit);
                m_Edit = null;
            }
        }

        private void Context_ZoomPhrase_Click(object sender, EventArgs e)
        {     //@zoomwaveform
            ShowZoomWaveformPanel();
        }

        private bool ShowZoomWaveformPanel()
        {
            if (mProjectView != null && mProjectView.Selection != null && !mProjectView.TransportBar.IsRecorderActive)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    mProjectView.TransportBar.Pause();
                }
                if (ActiveStrip != null && mProjectView.Selection.EmptyNodeForSelection != null)
                {
                    m_ZoomWaveformPanel = new ZoomWaveform(this, ActiveStrip, mProjectView.Selection.EmptyNodeForSelection, mProjectView);
                    this.Controls.Add(m_ZoomWaveformPanel);
                    m_ZoomWaveformPanel.Location = new Point(0, 0);
                    m_ZoomWaveformPanel.Show();
                    m_ZoomWaveformPanel.BringToFront();
                 //   mProjectView.TransportBar.Hide();
                    m_ZoomWaveformPanel.Focus();
                    mProjectView.TransportBar.UpdateButtons();
                    return true;
                }
            }
            return false;
        }
        public bool IsZoomWaveformActive
        {
            get
            {
                if (m_ZoomWaveformPanel == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //@zoomwaveform
        public void SelectCursor(PhraseNode node, AudioRange audioRange)
        {
            mProjectView.Selection = new AudioSelection(node, mProjectView.Selection.Control, audioRange);
            
        }

        public void SplitAndMerge(bool mergeWithNext)
        {
            mProjectView.SplitAndMerge(mergeWithNext);
        }
        public bool StripIsSelected
        {
            get
            {

                if (mProjectView != null && mProjectView.Selection != null && mProjectView.Selection is StripIndexSelection)
                    return true;
                else
                    return false;
            }
        }

        private void Context_SplitMergeWithNextMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.SplitAndMerge(true);
        }

        private void Context_SplitMergeWithPreviousMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.SplitAndMerge(false);
        }

        private void mContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (m_ZoomWaveformPanel == null && mProjectView != null && mProjectView.Selection != null && (mProjectView.Selection.Node is PhraseNode || mProjectView.Selection.Node is EmptyNode) && !mProjectView.TransportBar.IsRecorderActive)
            {
                Context_ZoomPhrase.Enabled = true;
            }
            else
            {
                Context_ZoomPhrase.Enabled = false;
            }
        }

        private void Context_Delete_deleteSelectionMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.Delete();
        }

        private void Context_Delete_deleteFollowingPhrasesMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.DeleteFollowingPhrasesInSection();
        }

        private void Context_ExportAudioMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ExportAudioOfSelectedNode();
        }

        private void Context_ReplaceAudioMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ReplaceAudioOfSelectedNode();
        }

        private void Context_AudioProcessing_Click(object sender, EventArgs e)
        {
            if (mProjectView.TransportBar.IsPlayerActive)
            {
                if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                mProjectView.TransportBar.Stop();
            }
            if (mProjectView.CanExportSelectedNodeAudio)
            {
                
                    mProjectView.ProcessAudio();
                
            }
        }

        private void Context_MergeWithNextSectionMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MergeStrips();
        }

        private void Context_MultiSectionOperations_Click(object sender, EventArgs e)
        {
            mProjectView.MergeMultipleSections();
        }

     

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
