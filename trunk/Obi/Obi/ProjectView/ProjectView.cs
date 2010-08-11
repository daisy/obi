using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.command;

namespace Obi.ProjectView
    {
    public partial class ProjectView : UserControl
        {
        private Presentation mPresentation;  // presentation
        private NodeSelection mSelection;    // currently selected node
        private Clipboard mClipboard;        // the clipboard
        private bool mSynchronizeViews;      // synchronize views flag
        private ObiForm mForm;               // parent form
        private bool mTOCViewVisible;        // keep track of the TOC view visibility (don't reopen it accidentally)
        private bool mMetadataViewVisible;   // keep track of the Metadata view visibility
        private Timer mTabbingTimer;         // ???
        //private bool mShowOnlySelected; // is set to show only one section in contents view. @show single section
        public readonly int MaxVisibleBlocksCount; // @phraseLimit
        public readonly int MaxOverLimitForPhraseVisibility; // @phraseLimit


        public event EventHandler SelectionChanged;             // triggered when the selection changes
        public event EventHandler FindInTextVisibilityChanged;  // triggered when the search bar is shown or hidden
        public event EventHandler BlocksVisibilityChanged; // triggered when phrase blocks are bbecoming  visible or invisible // @phraseLimit


        /// <summary>
        /// Create a new project view with no project yet.
        /// </summary>
        public ProjectView ()
            {
            InitializeComponent ();
            InitializeShortcutKeys ();
            mTOCView.ProjectView = this;
            mContentView.ProjectView = this;
            mMetadataView.ProjectView = this;
            mFindInText.ProjectView = this;
            mTransportBar.ProjectView = this;
            mTransportBar.Enabled = false;
            mTOCViewVisible = !mTOCSplitter.Panel1Collapsed && !mMetadataSplitter.Panel1Collapsed;
            mMetadataViewVisible = !mTOCSplitter.Panel1Collapsed && !mMetadataSplitter.Panel2Collapsed;
            mPresentation = null;
            mSelection = null;
            mForm = null;
            mClipboard = null;
            mTabbingTimer = null;
            //mShowOnlySelected = false;
            MaxVisibleBlocksCount = 1250; // @phraseLimit
            MaxOverLimitForPhraseVisibility = 300; // @phraseLimit
            }


        /// <summary>
        /// Add a new empty block.
        /// </summary>
        public void AddEmptyBlock ()
            {
            if (CanAddEmptyBlock)
                {
                EmptyNode node = new EmptyNode ( mPresentation );
                ObiNode parent = mContentView.Selection.ParentForNewNode ( node );
                AddUnusedAndExecute ( new Commands.Node.AddEmptyNode ( this, node, parent, mContentView.Selection.IndexForNewNode ( node ) ),
                    node, parent );
                }
            }

        /// <summary>
        /// Add new empty pages.
        /// </summary>
        public void AddEmptyPages ()
            {
            if (CanAddEmptyBlock)
                {
                if (TransportBar.IsActive) TransportBar.Stop ();

                Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber ( NextPageNumber, false, true );
                if (dialog.ShowDialog () == DialogResult.OK) AddPageRange ( dialog.Number, dialog.NumberOfPages, dialog.Renumber );
                }
            }

        /// <summary>
        /// Get the list of names of currently addable metadata entries.
        /// </summary>
        public List<string> AddableMetadataNames
            {
            get
                {
                List<string> addable = new List<string> ();
                foreach (MetadataEntryDescription d in MetadataEntryDescription.GetDAISYEntries ().Values)
                    {
                    if (mMetadataView.CanAdd ( d )) addable.Add ( d.Name );
                    }
                return addable;
                }
            }

        /// <summary>
        /// Add a new metadata entry to the project
        /// </summary>
        public void AddMetadataEntry ()
            {
            if (CanAddMetadataEntry ()) mPresentation.Do ( new Commands.Metadata.AddEntry ( this ) );
            }

        /// <summary>
        /// Add a new metadata entry to the project
        /// </summary>
        public urakawa.metadata.Metadata AddMetadataEntry ( string name )
            {
            Commands.Metadata.AddEntry cmd = new Commands.Metadata.AddEntry ( this, name );
            mPresentation.Do ( cmd );
            return cmd.Entry;
            }

        /// <summary>
        /// Add a new section or strip.
        /// </summary>
        public void AddSection ()
            {
            // if metadata is selected, for section creating rules, treat as nothing is selected.
            if (Selection != null && Selection is MetadataSelection)
                Selection = null;

            if (mContentView.CanAddStrip)
                {
                AddStrip ();
                }
            else if (CanAddSection)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();
                Commands.Node.AddSectionNode add = new Commands.Node.AddSectionNode ( this, mTOCView );
                AddUnusedAndExecute ( add, add.NewSection, add.NewSectionParent );
                }
            }

        // Add a new strip to the project.
        public void AddStrip ()
            {
            // quick fix to avoid a crash.
            if (Presentation != null && Presentation.FirstSection == null && Selection != null)
                Selection = null;

            if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();

            // select parent section node if a child phrase node is selected
            if (Selection != null && Selection.Node is EmptyNode)
                {
                Selection = new NodeSelection ( Selection.Node.ParentAs<SectionNode> (), Selection.Control );
                }
            Commands.Node.AddSectionNode add = new Commands.Node.AddSectionNode ( this, mContentView );

            if (mContentView.Selection != null)
                {
                CompositeCommand command = mPresentation.CreateCompositeCommand ( add.getShortDescription () );
                SectionNode selected = mContentView.Selection.Node is SectionNode ?
                    (SectionNode)mContentView.Selection.Node : mContentView.Selection.Node.ParentAs<SectionNode> ();
                command.append ( add );
                for (int i = selected.SectionChildCount - 1; i >= 0; --i)
                    {
                    SectionNode child = selected.SectionChild ( i );
                    Commands.Node.Delete delete = new Commands.Node.Delete ( this, child );
                    delete.UpdateSelection = false;
                    command.append ( delete );
                    Commands.Node.AddNode readd = new Commands.Node.AddNode ( this, child, add.NewSection, 0 );
                    readd.UpdateSelection = false;
                    command.append ( readd );
                    }
                if (!add.NewSectionParent.Used) AppendMakeUnused ( command, add.NewSection );
                mPresentation.Do ( command );
                }
            else
                {
                mPresentation.Do ( add );
                }
            }


        /// <summary>
        /// stores path of DAISY export directory in metadata
        /// Stores relative path if export directory lies inside project directory else stores absolute path
        /// </summary>
        /// <param name="format"></param>
        /// <param name="exportPath"></param>
        /// <param name="projectDirectory"></param>
        public void SetExportPathMetadata ( Obi.Export.ExportFormat format, string exportPath, string projectDirectory )
            {
            string exportMetadataName = null;
            if (format == Obi.Export.ExportFormat.DAISY3_0)
                {
                exportMetadataName = Metadata.OBI_DAISY3ExportPath;
                }
            else if (format == Obi.Export.ExportFormat.DAISY2_02)
                {
                exportMetadataName = Metadata.OBI_DAISY2ExportPath;
                }

            // if export directory is inside project directory, save relative path
            if (exportPath.StartsWith ( projectDirectory ))
                {
                if (!projectDirectory.EndsWith ( "\\" )) projectDirectory = projectDirectory + System.IO.Path.DirectorySeparatorChar;

                exportPath = exportPath.Replace ( projectDirectory, "" );
                }
            urakawa.metadata.Metadata m = mPresentation.GetFirstMetadataItem ( exportMetadataName );
            if (m == null)
                {
                m = AddMetadataEntry ( exportMetadataName );
                }

            Commands.Metadata.ModifyContent cmd = new Commands.Metadata.ModifyContent ( this,
                m,
                exportPath );
            mPresentation.Do ( cmd );
            }

        /// <summary>
        /// returns absolute export path for specified format of DAISY fileset passed in parameter.
        /// if that particular DAISY export do not exists, returns null
        /// </summary>
        /// <param name="format"></param>
        /// <param name="projectDirectory"></param>
        /// <returns></returns>
        public string GetDAISYExportPath ( Obi.Export.ExportFormat format, string projectDirectory )
            {
            if (mPresentation == null)
                return null;

            urakawa.metadata.Metadata m = null;
            if (format == Obi.Export.ExportFormat.DAISY3_0)
                {
                m = mPresentation.GetFirstMetadataItem ( Metadata.OBI_DAISY3ExportPath );
                }
            else if (format == Obi.Export.ExportFormat.DAISY2_02)
                {
                m = mPresentation.GetFirstMetadataItem ( Metadata.OBI_DAISY2ExportPath );
                }

            string exportPath = m != null ?
                        m.getContent () : null;

            // create absolute path if export path is relative
            if (!string.IsNullOrEmpty ( exportPath )
                && !System.IO.Path.IsPathRooted ( exportPath ))
                {
                exportPath = System.IO.Path.Combine ( projectDirectory, exportPath );
                }
            return exportPath;
            }


        /// <summary>
        /// Insert a new subsection in the book as the last child of the selected section node in the TOC view.
        /// </summary>
        public void AddSubSection ()
            {
            if (CanAddSubsection)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();
                Commands.Node.AddSubSection add = new Commands.Node.AddSubSection ( this );
                AddUnusedAndExecute ( add, add.NewSection, add.NewSectionParent );
                }
            }

        /// <summary>
        /// Append commands to make a node and its descendants unused to a composite command.
        /// This is used when adding new nodes under unused nodes.
        /// </summary>
        public void AppendMakeUnused ( CompositeCommand command, ObiNode node )
            {
            node.acceptDepthFirst (
                delegate ( urakawa.core.TreeNode n )
                    {
                    if (n is ObiNode && ((ObiNode)n).Used)
                        {
                        Commands.Node.ToggleNodeUsed unused = new Commands.Node.ToggleNodeUsed ( this, (ObiNode)n );
                        unused.UpdateSelection = false;
                        command.append ( unused );
                        }
                    return true;
                    }, delegate ( urakawa.core.TreeNode n ) { }
            );
            }

        public bool CanAddEmptyBlock { get { return mContentView.Selection != null && IsPhraseCountWithinLimit; } } // @phraseLimit
        public bool CanAddMetadataEntry () { return mPresentation != null; }
        public bool CanAddMetadataEntry ( MetadataEntryDescription d ) { return mMetadataView.CanAdd ( d ); }
        public bool CanAddSection { get { return mPresentation != null && (mTOCView.CanAddSection || mContentView.CanAddStrip); } }
        public bool CanAddSubsection { get { return mTOCView.CanAddSubsection; } }

        public bool CanApplyPhraseDetection
            {
            get
                {
                return mPresentation != null &&
                    Selection != null &&
                    Selection.Node is PhraseNode &&
                    ((PhraseNode)Selection.Node).Role_ != EmptyNode.Role.Silence &&
                    !TransportBar.IsRecorderActive &&
                    IsPhraseCountWithinLimit; // @phraseLimit
                }
            }

        /// <summary>
        /// Can assign the plain role if there is an empty node to assign to,
        /// and the current role is different from plain.
        /// </summary>
        public bool CanAssignPlainRole
            {
            get { return IsBlockSelected && SelectedNodeAs<EmptyNode> ().Role_ != EmptyNode.Role.Plain; }
            }

        /// <summary>
        /// Can assign the silence role if there is a phrase node to assign to,
        /// and the current role is different from silence.
        /// </summary>
        public bool CanAssignSilenceRole
            {
            get
                {
                PhraseNode node = SelectedNodeAs<PhraseNode> ();
                return node != null && node.Role_ != EmptyNode.Role.Silence;
                }
            }

        /// <summary>
        /// Can assign this custom role if there is an empty node to assign to,
        /// and the role is different from the one of this node.
        /// </summary>
        public bool CanAssignCustomRole ( string customRole )
            {
            EmptyNode node = SelectedNodeAs<EmptyNode> ();
            return node != null && (node.Role_ != EmptyNode.Role.Custom || node.CustomRole != customRole);
            }

        /// <summary>
        /// Can assign the heading role if there is a phrase node (must have audio)
        /// and there is no phrase role in the parent section. It must also be used.
        /// </summary>
        public bool CanAssignHeadingRole
            {
            get
                {
                PhraseNode node = SelectedNodeAs<PhraseNode> ();
                return node != null &&
                    node.Used &&
                    node.Role_ != EmptyNode.Role.Heading &&
                    node.AncestorAs<SectionNode> () != null &&
                    node.AncestorAs<SectionNode> ().Heading == null;
                }
            }

        /// <summary>
        /// Can assign at least one role (for instance custom or page); as long as there is a block to assign it to.
        /// </summary>
        public bool CanAssignARole { get { return IsBlockSelected; } }

        public bool CanCopy { get { return mPresentation != null && (CanCopySection || CanCopyStrip || CanCopyBlock || CanCopyAudio) && !TransportBar.IsRecorderActive; } }
        public bool CanCopyAudio { get { return mContentView.CanCopyAudio && !TransportBar.IsRecorderActive; } }
        public bool CanCopySection { get { return mTOCView.CanCopySection && !TransportBar.IsRecorderActive; } }
        public bool CanCopyStrip { get { return mContentView.CanCopyStrip && !TransportBar.IsRecorderActive; } }
        public bool CanCopyBlock { get { return mContentView.CanCopyBlock && !TransportBar.IsRecorderActive; } }
        public bool CanCut { get { return CanDelete; } }
        public bool CanDecreaseLevel { get { return mTOCView.CanDecreaseLevel && !(Selection is TextSelection); } }
        public bool CanDelete { get { return mPresentation != null && Selection != null && (CanRemoveSection || CanRemoveStrip || CanRemoveBlock || CanRemoveAudio || CanRemoveMetadata) && !TransportBar.IsRecorderActive && !(Selection is TextSelection); } }
        public bool CanFastForward { get { return mTransportBar.CanFastForward; } }
        public bool CanFocusOnContentView { get { return mPresentation != null && !mContentView.Focused; } }
        public bool CanFocusOnTOCView { get { return mPresentation != null && !mTOCView.Focused; } }
        public bool CanIncreaseLevel { get { return mTOCView.CanIncreaseLevel && !(Selection is TextSelection); } }
        public bool CanInsertSection { get { return CanInsertStrip || mTOCView.CanInsertSection && !TransportBar.IsRecorderActive && Presentation != null && Presentation.FirstSection != null; } }
        public bool CanInsertStrip { get { return mContentView.Selection != null && !TransportBar.IsRecorderActive; } }
        public bool CanMergeStripWithNext { get { return mContentView.CanMergeStripWithNext && !TransportBar.IsRecorderActive; } }
        public bool CanNavigateNextPage { get { return mTransportBar.CanNavigateNextPage; } }
        public bool CanNavigateNextPhrase { get { return mTransportBar.CanNavigateNextPhrase; } }
        public bool CanNavigateNextSection { get { return mTransportBar.CanNavigateNextSection; } }
        public bool CanNavigatePrevPage { get { return mTransportBar.CanNavigatePrevPage; } }
        public bool CanNavigatePrevPhrase { get { return mTransportBar.CanNavigatePrevPhrase; } }
        public bool CanNavigatePrevSection { get { return mTransportBar.CanNavigatePrevSection; } }
        public bool CanPaste { get { return mPresentation != null && mSelection != null && mSelection.CanPaste ( mClipboard ) && !TransportBar.IsRecorderActive; } }
        public bool CanPasteBefore { get { return mPresentation != null && mTOCView.CanPasteBefore ( mClipboard ) && !TransportBar.IsRecorderActive; } }
        public bool CanPasteInside
            {
            get
            { return mPresentation != null && mTOCView.CanPasteInside ( mClipboard ) && !TransportBar.IsRecorderActive; }
            }
        public bool CanPause { get { return mTransportBar.CanPause; } }
        public bool CanPlay { get { return mTransportBar.CanPlay; } }
        public bool CanPlaySelection { get { return mTransportBar.CanPlay && mSelection != null; } }
        public bool CanPreview { get { return mTransportBar.CanPreview; } }
        public bool CanPreviewAudioSelection { get { return mTransportBar.CanPreviewAudioSelection; } }
        public bool CanRemoveAudio { get { return mContentView.CanRemoveAudio; } }
        public bool CanRemoveBlock { get { return mContentView.CanRemoveBlock; } }
        public bool CanRemoveMetadata { get { return mMetadataView.CanRemoveMetadata; } }
        public bool CanRemoveSection { get { return mTOCView.CanRemoveSection; } }
        public bool CanRemoveStrip { get { return mContentView.CanRemoveStrip; } }
        public bool CanResume { get { return mTransportBar.CanResumePlayback; } }
        public bool CanRenameSection { get { return Selection != null && (CanRenameStrip || mTOCView.CanRenameSection); } }
        public bool CanRenameStrip { get { return Selection != null && (mContentView.CanRenameStrip); } }
        public bool CanRewind { get { return mTransportBar.CanRewind; } }
        public bool CanSetBlockUsedStatus { get { return mContentView.CanSetBlockUsedStatus; } }
        public bool CanSetSectionUsedStatus { get { return mTOCView.CanSetSectionUsedStatus; } }
        public bool CanSetSelectedNodeUsedStatus { get { return CanSetSectionUsedStatus || CanSetBlockUsedStatus; } }
        public bool CanSetTODOStatus { get { return IsBlockSelected || mTransportBar.IsActive; } }
        public bool CanShowOnlySelectedSection { get { return SelectedNodeAs<ObiNode> () != null; } }
        public bool CanSplitStrip { get { return mContentView.CanSplitStrip && !TransportBar.IsRecorderActive; } }
        public bool CanStop { get { return mTransportBar.CanStop; } }
        public bool CanCropPhrase
            {
            get
                {
                return Selection != null && Selection is AudioSelection && ((AudioSelection)Selection).AudioRange != null && !((AudioSelection)Selection).AudioRange.HasCursor && IsPhraseCountWithinLimit; // @phraseLimit
                }
            }

        // @phraseLimit
        /// <summary>
        ///  returns true if phrases in selected section are within  max visible blocks limit
        /// </summary>
        public bool IsPhraseCountWithinLimit { get { return GetSelectedPhraseSection != null && GetSelectedPhraseSection.PhraseChildCount <= MaxVisibleBlocksCount; } }

        // @phraseLimit
        public bool CanShowSectionContents { get { return GetSelectedPhraseSection != null && !mContentView.IsSectionPhrasesVisible ( GetSelectedPhraseSection ); } }

        public bool CanMarkPhrase
            {
            get
                {
                /*EmptyNode node = mTransportBar.HasAudioCursor ?
                    mTransportBar.CurrentPlaylist.CurrentPhrase :
                    SelectedNodeAs<EmptyNode>();
                return mTransportBar.CurrentState == TransportBar.State.Recording ||
                    mTransportBar.CurrentState == TransportBar.State.Paused ||
                    (node != null && (node.NodeKind != mMarkRole || node.CustomClass != mMarkCustomRole));*/
                return false;
                }
            }

        /// <summary> can mark selection begin from keyboard /// </summary>
        public bool CanMarkSelectionBegin { get { return TransportBar.IsPlayerActive; } }

        /// <summary> can mark selection end from keyboard /// </summary>
        public bool CanMarkSelectionEnd { get { return TransportBar.IsPlayerActive && Selection != null && Selection is AudioSelection; } }

        // @phraseLimit
        /// <summary>
        /// returns selection node or parent section node of selected phrase
        /// </summary>
        public SectionNode GetSelectedPhraseSection
            {
            get
                {
                if (Selection != null)
                    {
                    if (Selection.Node is SectionNode) return SelectedNodeAs<SectionNode> ();
                    else if (Selection.Node is EmptyNode) return Selection.Node.ParentAs<SectionNode> ();
                    }
                return null;
                }
            }

        // @phraseLimit
        /// <summary>
        /// returns string to indicate that selected section  has its contents invisible
        /// if section is visible or selection is not section selection then return null string
        /// </summary>
        public string InvisibleSelectedStripString { get { return mContentView.InvisibleStripString ( Selection != null ? GetSelectedPhraseSection : null ); } }

        /// <summary>
        /// Contents of the clipboard
        /// </summary>
        public Clipboard Clipboard
            {
            get { return mClipboard; }
            set { mClipboard = value; }
            }

        /// <summary>
        /// Color settings obtained from the form's settings.
        /// </summary>
        public ColorSettings ColorSettings
            {
            get { return mForm == null ? ColorSettings.DefaultColorSettings () : mForm.ColorSettings; }
            set
                {
                if (value != null)
                    {
                    BackColor = value.ProjectViewBackColor;
                    mTOCView.ColorSettings =
                    mContentView.ColorSettings =
                    mTransportBar.ColorSettings = value;
                    }
                }
            }

        // Yes please don't
        private bool ShouldSerializeColorSettings () { return false; }

        /// <summary>
        /// Copy the current selection into the clipboard. Noop if there is no selection.
        /// </summary>
        public void Copy ()
            {
            if (Selection != null && Selection is TextSelection) return;
            if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();
            if (CanCopySection)
                {
                mPresentation.Do ( new Commands.Node.Copy ( this, true, Localizer.Message ( "copy_section" ) ) );
                }
            else if (CanCopyStrip)
                {
                mPresentation.Do ( new Commands.Node.Copy ( this, false, Localizer.Message ( "copy_section_shallow" ) ) );
                }
            else if (CanCopyBlock)
                {
                mPresentation.Do ( new Commands.Node.Copy ( this, true, Localizer.Message ( "copy_phrase" ) ) );
                }
            else if (CanCopyAudio)
                {
                mPresentation.Do ( new Commands.Audio.Copy ( this ) );
                }
            }

        /// <summary>
        /// Cut (delete) the selection and store it in the clipboard.
        /// </summary>
        public void Cut ()
            {
            if (Selection != null && Selection is TextSelection) return;
            if (CanDelete && mTransportBar.IsPlayerActive) mTransportBar.Stop ();
            if (CanRemoveSection || CanRemoveStrip)
                {
                bool isSection = mSelection.Control is TOCView;
                CompositeCommand command = Presentation.CreateCompositeCommand (
                    Localizer.Message ( isSection ? "cut_section" : "cut_section_shallow" ) );
                command.append ( new Commands.Node.Copy ( this, isSection ) );
                if (CanRemoveStrip)
                    command.append ( mContentView.DeleteStripCommand () );
                else
                    command.append ( new Commands.Node.Delete ( this, mSelection.Node ) );
                mPresentation.Do ( command );
                // quick fix for null selection problem while cutting single section ins single section project
                if (mPresentation.FirstSection == null) Selection = null;
                }
            else if (CanRemoveBlock)
                {
                CompositeCommand command = mPresentation.getCommandFactory ().createCompositeCommand ();
                command.setShortDescription ( Localizer.Message ( "cut_phrase" ) );
                command.append ( new Commands.Node.Copy ( this, true ) );
                command.append ( new Commands.Node.Delete ( this, mSelection.Node ) );
                mPresentation.Do ( command );
                }
            else if (CanRemoveAudio)
                {
                CompositeCommand command = mPresentation.getCommandFactory ().createCompositeCommand ();
                command.setShortDescription ( Localizer.Message ( "cut_audio" ) );
                ICommand delete = Commands.Audio.Delete.GetCommand ( this );
                PhraseNode deleted = delete is Commands.Audio.Delete ?
                    ((Commands.Audio.Delete)delete).Deleted : (PhraseNode)Selection.Node;
                command.append ( new Commands.Audio.Copy ( this, deleted,
                    new AudioRange ( 0.0, deleted.Audio.getDuration ().getTimeDeltaAsMillisecondFloat () ) ) );
                command.append ( delete );
                mPresentation.Do ( command );
                }
            }

        /// <summary>
        /// Delete the current selection. Noop if there is no selection.
        /// </summary>
        public void Delete ()
            {
            if (Selection != null && Selection is TextSelection) return;
            if (CanDelete && mTransportBar.IsPlayerActive) mTransportBar.Stop ();
            if (CanRemoveSection)
                {
                mPresentation.Do ( new Commands.Node.Delete ( this, mTOCView.Selection.Section,
                    Localizer.Message ( "delete_section" ) ) );
                }
            else if (CanRemoveStrip)
                {
                // first create landing strip
                if (Selection != null && Selection.Node is SectionNode) //@singleSection: begin
                    {
                    SectionNode section = (SectionNode)Selection.Node;
                    SectionNode landingSectionNode = section.FollowingSection;
                    if (landingSectionNode == null) landingSectionNode  = section.PrecedingSection;

                    if (landingSectionNode  != null)
                        {
                        mContentView.CreateStripForSelectedSection ( landingSectionNode, false );
                        }
                    
                    }//@singleSection: end

                mPresentation.Do ( mContentView.DeleteStripCommand () );
                }
            else if (CanRemoveBlock)
                {
                mPresentation.Do ( new Commands.Node.Delete ( this, SelectedNodeAs<EmptyNode> (),
                    Localizer.Message ( "delete_phrase" ) ) );
                }
            else if (CanRemoveAudio)
                {
                mPresentation.Do ( Commands.Audio.Delete.GetCommand ( this ) );
                }
            else if (CanRemoveMetadata)
                {
                mPresentation.Do ( new Commands.Metadata.DeleteEntry ( this ) );
                }
            }

        public bool CanDeleteMetadataEntry ( urakawa.metadata.Metadata m )
            {
            return mPresentation.getListOfMetadata ().Contains ( m );
            }

        /// <summary>
        /// Delete all unused nodes.
        /// Ask before deleting silence phrases (unless they are in unused sections...)
        /// </summary>
        public void DeleteUnused ()
            {
            // handle selection to avoid exception if selected node is deleted
            if (GetSelectedPhraseSection != null && (!GetSelectedPhraseSection.Used))
                Selection = null;

            CompositeCommand command = mPresentation.CreateCompositeCommand ( Localizer.Message ( "delete_unused" ) );
            // Collect silence node deletion commands separately in case the user wants to keep them.
            List<ICommand> silence = new List<ICommand> ();
            mPresentation.RootNode.acceptDepthFirst (
                delegate ( urakawa.core.TreeNode node )
                    {
                    if (node is ObiNode && !((ObiNode)node).Used)
                        {
                        Commands.Node.Delete delete = new Commands.Node.Delete ( this, (ObiNode)node, false );
                        if (Selection == null) delete.UpdateSelection = true; // temp fix, if selection is null, delete updates afterDeleteSelection to null to avoid selecting something through some event.
                        if (node is PhraseNode && ((PhraseNode)node).Role_ == EmptyNode.Role.Silence)
                            {
                            silence.Add ( delete );
                            }
                        else
                            {
                            command.insert ( delete, 0 );
                            }
                        return false;
                        }
                    return true;
                    }, delegate ( urakawa.core.TreeNode node ) { }
            );
            if (silence.Count > 0)
                {
                if (MessageBox.Show ( Localizer.Message ( "delete_silence_phrases" ),
                    Localizer.Message ( "delete_silence_phrases_caption" ), MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question ) == DialogResult.Yes)
                    {
                    foreach (ICommand c in silence) command.append ( c );
                    }
                }

            //if (Selection != null && mTOCView.ContainsFocus) Selection = null;
            if (command.getCount () > 0) mPresentation.Do ( command );
            }

        /// <summary>
        /// Cancel any audio selection.
        /// </summary>
        public void DeselectAudio ()
            {
            if (Selection != null && Selection is AudioSelection)
                {
                ((AudioSelection)Selection).AudioRange.SelectionBeginTime = 0;
                ((AudioSelection)Selection).AudioRange.SelectionEndTime = 0;
                }
            }


        /// <summary>
        /// Show (select) the strip for the current selection
        /// </summary>
        public void FocusOnContentView ()
            {
            if (CanFocusOnContentView)
                {
                if (mSelection != null && mSelection.Control is TOCView)
                    {
                    // show the selected section in content view
                    if (GetSelectedPhraseSection != null) mContentView.CreateStripForSelectedSection ( GetSelectedPhraseSection , true); //@singleSection

                    if (TransportBar.IsPlayerActive)
                        {
                        // if block to be selected is invisible, select parent strip
                        //@singleSection : following 3 lines commented 
                        //if (mContentView.IsBlockInvisibleButStripVisible ( mTransportBar.CurrentPlaylist.CurrentPhrase ))
                            //Selection = new NodeSelection ( mTransportBar.CurrentPlaylist.CurrentPhrase.ParentAs<SectionNode> (), mContentView );
                        //else
                            //Selection = new NodeSelection ( mContentView.PlaybackPhrase, mContentView );//@singleSection: original
                        Selection = new NodeSelection ( TransportBar.CurrentPlaylist.CurrentPhrase, mContentView );//@singleSection: new
                        }// playback active check ends
                    else
                        Selection = new NodeSelection ( mSelection.Node, mContentView );
                    }
                //mContentView.Focus();
                }
            }

        /// <summary>
        /// Show the selection (if any) in the TOC view, and focus on this view.
        /// </summary>
        public void FocusOnTOCView ()
            {
            if (CanFocusOnTOCView)
                {
                // check for first section node added for a quick fix.
                if (Presentation.FirstSection == null) Selection = null;

                SectionNode node = mSelection == null ? null :
                    mSelection.Node is SectionNode ? (SectionNode)mSelection.Node :
                    mSelection.Node.AncestorAs<SectionNode> ();
                if (node != null) Selection = new NodeSelection ( node, mTOCView );
                mTOCView.Focus ();
                }
            }

        /// <summary>
        /// Insert a new section before the selected one at the same level.
        /// </summary>
        public void InsertSection ()
            {
            if (CanInsertStrip)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();
                // select section if empty node is selected
                if (Selection.Node is EmptyNode) Selection = new NodeSelection ( Selection.Node.ParentAs<SectionNode> (), Selection.Control );

                Commands.Node.InsertSectionNode insert = new Commands.Node.InsertSectionNode ( this );
                AddUnusedAndExecute ( insert, insert.NewSection, insert.NewSectionParent );
                }
            else if (CanInsertSection)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();
                Commands.Node.InsertSectionNode insert = new Commands.Node.InsertSectionNode ( this );
                AddUnusedAndExecute ( insert, insert.NewSection, insert.NewSectionParent );
                }
            }

        // These methods are used to know what is currently selected:
        // block, strip or section; strict (i.e. not the label or the waveform) or not.
        public bool IsBlockSelected { get { return SelectedNodeAs<EmptyNode> () != null; } }
        public bool IsBlockSelectedStrict { get { return IsBlockSelected && mSelection.GetType () == typeof ( NodeSelection ); } }

        /// <summary>
        /// Merge the selection strip with the next one, i.e. either its first sibling or its first child.
        /// </summary>
        public void MergeStrips ()
            {
            if (CanMergeStripWithNext)
                {
                // if total phrase count after merge is more than max phrases per section, return
                SectionNode section = mContentView.SelectedSection;
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild ( 0 );
                if ((section.PhraseChildCount + next.PhraseChildCount) > MaxVisibleBlocksCount) // @phraseLimit
                    {
                    MessageBox.Show ( Localizer.Message ( "Operation_Cancelled" ) + "\n" + string.Format ( Localizer.Message ( "ContentsHidden_PhrasesExceedMaxLimitPerSection" ), MaxVisibleBlocksCount ) );
                    return;
                    }

                // if next strip has no. of large phrases, first make all phrase blocks invisible to improve speed 
                if (next.PhraseChildCount > 30) mContentView.RemoveBlocksInStrip ( next );
                try
                    {
                    mPresentation.Do ( mContentView.MergeSelectedStripWithNextCommand () );
                    if (mSelection != null && mSelection.Node is SectionNode) UpdateBlocksLabelInStrip ( (SectionNode)mSelection.Node );
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                // hide newly made phrases visible if the strip has its contents hidden
                //HideNewPhrasesInInvisibleSection ( section );//@singleSection: original
                mContentView.CreateBlocksInStrip (); //@singleSection: new statement
                }
            }

        /// <summary>
        /// Show or hide the Metadata view.
        /// </summary>
        public bool MetadataViewVisible
            {
            get { return mMetadataViewVisible; }
            set
                {
                mMetadataViewVisible = value;
                if (value)
                    {
                    mTOCSplitter.Panel1Collapsed = false;
                    mMetadataSplitter.Panel1Collapsed = !TOCViewVisible;
                    }
                else if (!value && !TOCViewVisible) mTOCSplitter.Panel1Collapsed = true;
                mMetadataSplitter.Panel2Collapsed = !value;
                if (!mMetadataView.Visible)
                    {
                    FocusOnTOCView ();
                    if (!mTOCView.Focused)
                        FocusOnContentView ();
                    }
                }
            }

        /// <summary>
        /// Increase the level of the selected section (was "move in.")
        /// TODO: rewrite command as a composite command.
        /// </summary>
        public void IncreaseSelectedSectionLevel ()
            {
            if (CanIncreaseLevel)
                {
                if (mTransportBar.IsActive) mTransportBar.Stop ();

                mContentView.EventsAreEnabled = false;
                mPresentation.Do ( new Commands.TOC.MoveSectionIn ( this, mTOCView.Selection.Section ) );
                mContentView.EventsAreEnabled = true;
                }
            }

        /// <summary>
        /// Decrease the level of the selected section (was "move out.")
        /// TODO: rewrite command as a composite command.
        /// </summary>
        public void DecreaseSelectedSectionLevel ()
            {
            if (CanDecreaseLevel)
                {
                if (mTransportBar.IsActive) mTransportBar.Stop ();

                mContentView.EventsAreEnabled = false;
                mPresentation.Do ( new Commands.TOC.MoveSectionOut ( this, mTOCView.Selection.Section ) );
                mContentView.EventsAreEnabled = true;
                }
            }

        public PageNumber CurrentOrNextPageNumber
            {
            get
                {
                return mSelection.Node is EmptyNode && ((EmptyNode)mSelection.Node).Role_ == EmptyNode.Role.Page ?
                    ((EmptyNode)mSelection.Node).PageNumber : NextPageNumber;
                }
            }

        /// <summary>
        /// Get the next page number for the selected block.
        /// </summary>
        public PageNumber NextPageNumber
            {
            get
                {
                ObiNode node = mSelection.Node;
                if (node is SectionNode)
                    {
                    if (mSelection is StripIndexSelection)
                        {
                        // Page number follows the page number for the block at this 
                        int index = ((StripIndexSelection)mSelection).Index;
                        if (index > 0)
                            {
                            if (index < mSelection.Section.PhraseChildCount)
                                {
                                node = node.PhraseChild ( index - 1 );
                                }
                            else if (node.LastUsedPhrase != null)
                                {
                                node = node.LastUsedPhrase;
                                }
                            }
                        }
                    else if (node.LastUsedPhrase != null)
                        {
                        node = node.LastUsedPhrase;
                        }
                    }
                return mPresentation.PageNumberFollowing ( node );
                }
            }

        /// <summary>
        /// The parent form as an Obi form.
        /// </summary>
        public ObiForm ObiForm
            {
            get { return mForm; }
            set { mForm = value; }
            }

        /// <summary>
        /// Paste the contents of the clipboard in the current context. Noop if the clipboard is empty.
        /// </summary>
        public void Paste ()
            {
            if (CanPaste)
                {
                // if clipboard has phrase and the phrase count per section is above the max limit, return
                if (mClipboard != null && mClipboard.Node is EmptyNode && GetSelectedPhraseSection != null && GetSelectedPhraseSection.PhraseChildCount >= MaxVisibleBlocksCount) // @phraseLimit
                    return;

                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();
                bool PlaySelectionFlagStatus = TransportBar.SelectionChangedPlaybackEnabled;
                mTransportBar.SelectionChangedPlaybackEnabled = false;

                mPresentation.Do ( mSelection.PasteCommand ( this ) );

                mTransportBar.SelectionChangedPlaybackEnabled = PlaySelectionFlagStatus;
                }
            }

        /// <summary>
        /// Paste the contents of the clipboard before the selected section.
        /// </summary>
        public void PasteBefore ()
            {
            if (CanPasteBefore)
                {
                Commands.Node.Paste paste = new Commands.Node.PasteBefore ( this );
                AddUnusedAndExecute ( paste, paste.Copy, paste.CopyParent );
                }
            }

        /// <summary>
        /// Paste the contents of the clipboard inside (as the last child of) the selected section.
        /// </summary>
        public void PasteInside ()
            {
            if (CanPasteInside)
                {
                Commands.Node.PasteInside paste = new Commands.Node.PasteInside ( this );
                AddUnusedAndExecute ( paste, paste.Copy, paste.CopyParent );
                }
            }

        public void SetPlaybackPhraseAndTime ( PhraseNode node, double time )
            {
            if (mContentView != null)
                {
                mContentView.SetPlaybackPhraseAndTime ( node, time );
                if (node != null) MakePhraseNodeVisible ( node );
                }
            }

        /// <summary>
        /// Set the presentation that the project view displays.
        /// </summary>
        public Presentation Presentation
            {
            get { return mPresentation; }
            set
                {
                ProjectVisible = value != null;
                if (mPresentation != value)
                    {
                    mPresentation = value;
                    mTransportBar.Enabled = mPresentation != null;
                    if (mPresentation != null)
                        {
                        mTOCView.SetNewPresentation ();
                        mContentView.NewPresentation ();
                        mTransportBar.NewPresentation ();
                        mMetadataView.NewPresentation ();
                        }
                    }
                }
            }

        /// <summary>
        /// Rename a section node after the name was changed directly by the user (not through a menu.)
        /// </summary>
        public void RenameSectionNode ( SectionNode section, string label )
            {
            mPresentation.Do ( new Commands.Node.RenameSection ( this, section, label ) );
            }

        /// <summary>
        /// Select a section or strip and start renaming it.
        /// </summary>
        public void SelectAndRenameSelection ( NodeSelection selection )
            {
            if (selection.Control is IControlWithRenamableSelection)
                {
                ((IControlWithRenamableSelection)selection.Control).SelectAndRename ( selection.Node );
                }
            }

        // Quick way to set the selection

        public EmptyNode SelectedBlockNode { set { Selection = new NodeSelection ( value, mContentView ); } }
        public SectionNode SelectedSectionNode { set { Selection = new NodeSelection ( value, mTOCView ); } }
        public SectionNode SelectedStripNode { set { Selection = new NodeSelection ( value, mContentView ); } }

        /// <summary>
        /// Currently selected node of the given type (e.g. SectionNode, EmptyNode or PhraseNode).
        /// Null if there is no selection, or selection of a different kind.
        /// </summary>
        public T SelectedNodeAs<T> () where T : ObiNode
            {
            return mSelection == null ? null : mSelection.Node as T;
            }

        /// <summary>
        /// The current selection in the view.
        /// </summary>
        public NodeSelection Selection
            {
            get { return mSelection; }
            set
                {
                // Selection is disabled when the transport bar is active.
                if (mSelection != value)
                    {
                    // deselect if there was a selection in a different control
                    if (mSelection != null && (value == null || mSelection.Control != value.Control))
                        {
                        mSelection.Control.Selection = null;
                        }
                    // select in the control
                    mSelection = value;
                    //UpdateShowOnlySelected ( mSelection == null ? false : mShowOnlySelected ); //@ShowSingleSection
                    if (mSelection != null)
                        {
                        if (mSelection.Control == mTOCView) TOCViewVisible = true;
                        else if (mSelection.Control == mMetadataView) MetadataViewVisible = true;
                        mSelection.Control.Selection = value;
                        }
                    if (SelectionChanged != null) SelectionChanged ( this, new EventArgs () );
                    }
                }
            }

        /// <summary>
        /// Set a page number on the selected phrase.
        /// </summary>
        public void SetPageNumberOnSelection ()
            {
            if (CanSetPageNumber)
                {
                if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause ();

                Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber ( CurrentOrNextPageNumber, false, false );
                if (dialog.ShowDialog () == DialogResult.OK) SetPageNumberOnSelectedBock ( dialog.Number, dialog.Renumber );
                }
            }

        /// <summary>
        /// Set the used status of the selected node, and of all its descendants.
        /// </summary>
        public void SetSelectedNodeUsedStatus ( bool used )
            {
            if (CanSetSelectedNodeUsedStatus && mSelection.Node.Used != used)
                {
                CompositeCommand command = Presentation.CreateCompositeCommand ( String.Format (
                    Localizer.Message ( mSelection.Node is SectionNode ? "mark_section_used" : "mark_phrase_used" ),
                    Localizer.Message ( mSelection.Node.Used ? "unused" : "used" ) ) );
                mSelection.Node.acceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {
                        if (n is ObiNode && ((ObiNode)n).Used != used)
                            {
                            if (n is PhraseNode && ((PhraseNode)n).Role_ == EmptyNode.Role.Heading)
                                {
                                command.append ( new Commands.Node.AssignRole ( this, (PhraseNode)n, EmptyNode.Role.Plain ) );
                                //command.append(new Commands.Node.UnsetNodeAsHeadingPhrase(this, (PhraseNode)n));
                                }
                            command.append ( new Commands.Node.ToggleNodeUsed ( this, (ObiNode)n ) );
                            }
                        return true;
                        }, delegate ( urakawa.core.TreeNode n ) { }
                );
                Presentation.Do ( command );
                }
            }

        /// <summary>
        /// Get the strip that the selection is in, or null if there is no applicable selection.
        /// </summary>
        public Strip StripForSelection { get { return mContentView == null ? null : mContentView.StripForSelection; } }




        /// <summary>
        /// Set content wrapping in the content view.
        /// </summary>
        public bool WrapStripContents { set { mContentView.WrapStripContents = value; } }

        /// <summary>
        /// Select the name field of the selected section and start editing it.
        /// </summary>
        public void StartRenamingSelectedSection ()
            {
            if (CanRenameStrip)
                {
                mContentView.SelectAndRename ( mContentView.Selection.Section );
                }
            else if (CanRenameSection)
                {
                mTOCView.SelectAndRename ( SelectedNodeAs<SectionNode> () );
                }
            }

        /// <summary>
        /// Split a strip at the selected position.
        /// </summary>
        public void SplitStrip ()
            {
            if (CanSplitStrip)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Pause ();

                bool PlayOnSelectionStatus = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;
                SectionNode OriginalSectionNode = null;
                if (mSelection != null && mSelection.Node is EmptyNode) OriginalSectionNode = mSelection.Node.ParentAs<SectionNode> ();

                mPresentation.Do ( mContentView.SplitStripCommand () );
                if (OriginalSectionNode != null) UpdateBlocksLabelInStrip ( OriginalSectionNode );

                TransportBar.SelectionChangedPlaybackEnabled = PlayOnSelectionStatus;
                }
            }

        /// <summary>
        /// Set the synchronize views flag for this view and resynchronize the views if necessary.
        /// </summary>
        public bool SynchronizeViews
            {
            set
                {
                //mSynchronizeViews = value;//@singleSection: original 
                mSynchronizeViews =  false;//@singleSection: new
                if (!CanShowOnlySelectedSection) //@ShowSingleSection
                    {
                    if (mSynchronizeViews)
                        {
                        mTOCView.ResyncViews ();
                        if (mSelection != null && mSelection.Control == mTOCView)
                            {
                            mContentView.MakeStripVisibleForSection ( SelectedNodeAs<SectionNode> () );
                            }
                        }
                    else
                        {
                        mContentView.UnsyncViews ();
                        }
                    }
                }
            }

        /// <summary>
        /// Show or hide the TOC view.
        /// </summary>
        public bool TOCViewVisible
            {
            get { return mTOCViewVisible; }
            set
                {
                mTOCViewVisible = value;
                if (value)
                    {
                    mTOCSplitter.Panel1Collapsed = false;
                    mMetadataSplitter.Panel2Collapsed = !MetadataViewVisible;
                    }
                else
                    {
                    if (mSelection != null && mSelection.Control == mTOCView) Selection = null;
                    if (!MetadataViewVisible) mTOCSplitter.Panel1Collapsed = true;
                    }
                mMetadataSplitter.Panel1Collapsed = !value;
                }
            }

        /// <summary>
        /// Get the transport bar for this project view.
        /// </summary>
        public TransportBar TransportBar { get { return mTransportBar; } }



        // Execute a command, but first add some extra stuff to maintain the unusedness of the new node
        // depending on the unusedness of its parent.
        private void AddUnusedAndExecute ( ICommand command, ObiNode node, ObiNode parent )
            {
            if (parent.Used)
                {
                mPresentation.Do ( command );
                }
            else
                {
                CompositeCommand c;
                if (command is CompositeCommand)
                    {
                    c = (CompositeCommand)command;
                    }
                else
                    {
                    c = mPresentation.CreateCompositeCommand ( command.getShortDescription () );
                    c.append ( command );
                    }
                AppendMakeUnused ( c, node );
                mPresentation.Do ( c );
                }
            }

        /// <summary>
        /// Show or hide the project display.
        /// </summary>
        private bool ProjectVisible
            {
            set
                {
                mTransportBarSplitter.Visible = value;
                mNoProjectLabel.Visible = !value;
                }
            }







        /// <summary>
        /// Show or hide the search bar.
        /// </summary>
        public bool FindInTextVisible
            {
            get { return !mFindInTextSplitter.Panel2Collapsed; }
            set
                {
                bool isVisible = !mFindInTextSplitter.Panel2Collapsed;
                if (isVisible != value) mFindInTextSplitter.Panel2Collapsed = !value;
                if (FindInTextVisibilityChanged != null) FindInTextVisibilityChanged ( this, null );
                }
            }

        /// <summary>
        /// Show or hide the transport bar.
        /// </summary>
        public bool TransportBarVisible
            {
            get { return !mTransportBarSplitter.Panel2Collapsed; }
            set
                {
                bool isVisible = !mTransportBarSplitter.Panel2Collapsed;
                if (isVisible != value) mTransportBarSplitter.Panel2Collapsed = !value;
                }
            }

        public bool IsSectionSelected { get { return SelectedNodeAs<SectionNode> () != null && mSelection.Control == mTOCView; } }
        public bool IsSectionSelectedStrict { get { return IsSectionSelected && mSelection.GetType () == typeof ( NodeSelection ); } }
        public bool IsStripSelected { get { return SelectedNodeAs<SectionNode> () != null && mSelection.Control == mContentView; } }
        public bool IsStripSelectedStrict { get { return IsStripSelected && mSelection.GetType () == typeof ( NodeSelection ); } }

        public bool CanDeselect { get { return mSelection != null; } }

        public bool CanShowInStripsView { get { return IsSectionSelected; } }

        public bool CanShowPhrasePropertiesDialog { get { return Selection != null && Selection.Node is EmptyNode; } }
        public bool CanShowProjectPropertiesDialog { get { return mPresentation != null; } }
        public bool CanShowSectionPropertiesDialog { get { return Selection != null && Selection.Node is SectionNode && Presentation != null && Presentation.FirstSection != null; } } // quick fix of inserting first section check to avoid a crash

        public bool CanMarkSectionUnused { get { return mTOCView.CanSetSectionUsedStatus && mSelection.Node.Used; } }
        public bool CanMergeBlockWithNext { get { return mContentView.CanMergeBlockWithNext; } }
        public bool CanSplitPhrase { get { return mTransportBar.CanSplitPhrase; } }

        public bool IsBlockUsed { get { return mContentView.IsBlockUsed; } }
        public bool IsStripUsed { get { return mContentView.IsStripUsed; } }

        /// <summary>
        /// True when there is a block that is selected that is TODO, or a block playing back that is TODO.
        /// </summary>
        public bool IsCurrentBlockTODO
            {
            get
                {
                return mTransportBar.PlaybackPhrase != null ?
                    mTransportBar.PlaybackPhrase.TODO :
                    mContentView.SelectedEmptyNode != null ?
                    mContentView.SelectedEmptyNode.TODO : false;
                }
            }

        /// <summary>
        /// Show the strip for the given section
        /// </summary>
        public void MakeStripVisibleForSection ( SectionNode section )
            {
            if (mSynchronizeViews) mContentView.MakeStripVisibleForSection ( section );
            }

        /// <summary>
        /// Show the tree node in the TOC view for the given section
        /// </summary>
        public void MakeTreeNodeVisibleForSection ( SectionNode section )
            {
            if (mSynchronizeViews) mTOCView.MakeTreeNodeVisibleForSection ( section );
            }

        /// <summary>
        /// Show/hide strips for nodes that were collapsed/expanded when the views are synchronized
        /// </summary>
        public void SetStripsVisibilityForSection ( SectionNode section, bool visible )
            {
            if (mSynchronizeViews) mContentView.SetStripsVisibilityForSection ( section, visible );
            }

        public void SetStripVisibilityForSection ( SectionNode section, bool visible )
            {
            if (mSynchronizeViews) mContentView.SetStripVisibilityForSection ( section, visible );
            }

        /// <summary>
        /// Make sure that a phrase node is visible in the contents view while it is playing,
        /// and the corresponding section is also visible in the TOC view (if it is shown.)
        /// </summary>
        public void MakePhraseNodeVisible ( PhraseNode phrase )
            {
            SectionNode section = phrase.AncestorAs<SectionNode> ();
            mTOCView.MakeTreeNodeVisibleForSection ( section );
            }

        #region Find in Text

        public bool CanFindFirstTime { get { return mPresentation != null && mPresentation.RootNode.SectionChildCount > 0; } }
        public void FindInText ()
            {
            //show the form if it's not already shown
            if (mFindInTextSplitter.Panel2Collapsed == true) mFindInTextSplitter.Panel2Collapsed = false;
            FindInTextVisible = true;
            //iterating over the layout panel seems to be the way to search the sections 
            if (mContentView.ContainsFocus)
                {
                mFindInText.StartNewSearch ( mTOCView, mContentView,  FindViews.ContentView);
                }
            else
                {
                mFindInText.StartNewSearch ( mTOCView, mContentView, FindViews.TocView);
                }
            }
        public void FindNextInText ()
            {
            if (FindInTextVisible) mFindInText.FindNext ();
            }

        public void FindPreviousInText ()
            {
            if (FindInTextVisible) mFindInText.FindPrevious ();
            }

        public bool CanFindNextPreviousText
            {
            get { return mFindInText.CanFindNextPreviousText; }
            }
        #endregion


        public void ListenToSelection () { }
        public bool CanListenToSection { get { return mTransportBar.Enabled && mTOCView.Selection != null; } }
        public bool CanListenToStrip { get { return mTransportBar.Enabled && mContentView.SelectedSection != null; } }
        public bool CanListenToBlock { get { return mTransportBar.Enabled && mContentView.SelectedPhraseNode != null; } }

        // Blocks



        /// <summary>
        /// Import new phrases in the strip, one block per file.
        /// </summary>
        public void ImportPhrases ()
            {
            if (CanImportPhrases)
                {
                if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Stop ();

                string[] paths = SelectFilesToImport ();
                if (paths != null)
                    {
                    Dialogs.ImportFileSplitSize dialog =
                        new Dialogs.ImportFileSplitSize ( ObiForm.Settings.SplitPhrasesOnImport,
                            ObiForm.Settings.MaxPhraseDurationMinutes );
                    if (dialog.ShowDialog () == DialogResult.OK)
                        {
                        ObiForm.Settings.MaxPhraseDurationMinutes = dialog.MaxPhraseDurationMinutes;
                        ObiForm.Settings.SplitPhrasesOnImport = dialog.SplitPhrases;
                        bool createSectionForEachPhrase = dialog.createSectionForEachPhrase;
                         // convert from minutes to milliseconds
                        double durationMs = dialog.SplitPhrases ? dialog.MaxPhraseDurationMinutes * 60000.0 : 0.0;
                        List<PhraseNode> phraseNodes = new List<PhraseNode> ( paths.Length );
                        Dictionary<PhraseNode, string> phrase_SectionNameMap = new Dictionary<PhraseNode, string> (); // used for importing sections
                        Dialogs.ProgressDialog progress =
                            new Dialogs.ProgressDialog ( Localizer.Message ( "import_audio_progress_dialog_title" ),
                                delegate ()
                                    {
                                    foreach (string path in paths)
                                        {
                                        List<PhraseNode> phrases = null;
                                        try
                                            {
                                            phrases = mPresentation.CreatePhraseNodeList ( path, durationMs );
                                            }
                                        catch (System.Exception ex)
                                            {
                                            MessageBox.Show ( String.Format ( Localizer.Message ( "import_phrase_error_text" ), path ) + "\n\n" + ex.ToString () );
                                            continue;
                                            }
                                        if (createSectionForEachPhrase && phrases != null && phrases.Count > 0
                                            && !phrase_SectionNameMap.ContainsKey ( phrases[0] ))
                                            {
                                            phrase_SectionNameMap.Add ( phrases[0], System.IO.Path.GetFileNameWithoutExtension ( path ) );
                                            }
                                        foreach (PhraseNode p in phrases)
                                            {
                                            try
                                                {
                                                phraseNodes.Add ( p );
                                                
                                                }
                                            catch (Exception)
                                                {
                                                MessageBox.Show (
                                                    String.Format ( Localizer.Message ( "import_phrase_error_text" ), path ),
                                                    Localizer.Message ( "import_phrase_error_caption" ),
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error );
                                                }
                                            }
                                        }
                                    } );
                        progress.ShowDialog ();
                        if (phraseNodes.Count > 0)
                            {
                            if (GetSelectedPhraseSection != null && (GetSelectedPhraseSection.PhraseChildCount + phraseNodes.Count <= MaxVisibleBlocksCount)) // @phraseLimit
                                {
                                this.ObiForm.Cursor = Cursors.WaitCursor;
                                if (createSectionForEachPhrase)
                                    {
                                    mPresentation.Do ( GetImportSectionsFromAudioCommands( phraseNodes, phrase_SectionNameMap ) );
                                    }
                                else
                                    {
                                    mPresentation.Do ( GetImportPhraseCommands ( phraseNodes ) );
                                    }
                                // hide new phrases if section's contents are hidden
                                //HideNewPhrasesInInvisibleSection ( GetSelectedPhraseSection );//@singleSection: original
                                mContentView.CreateBlocksInStrip (); //@singleSection: new
                                }
                            else
                                MessageBox.Show ( Localizer.Message ( "Operation_Cancelled" ) + "\n" + string.Format ( Localizer.Message ( "ContentsHidden_PhrasesExceedMaxLimitPerSection" ), MaxVisibleBlocksCount ) );
                            }

                        }
                    }
                this.ObiForm.Cursor = Cursors.Default;
                }
            }

        // Create a command to import phrases
        private CompositeCommand GetImportPhraseCommands ( List<PhraseNode> phraseNodes )
            {
            CompositeCommand command = Presentation.CreateCompositeCommand ( Localizer.Message ( "import_phrases" ) );
            ObiNode parent;
            int index;
            if (mContentView.Selection.Node is SectionNode)
                {
                // Import into a section, at 0 or at the selected index
                parent = mContentView.Selection.Node;
                index = mContentView.Selection is StripIndexSelection ?
                    ((StripIndexSelection)mContentView.Selection).Index : 0;
                }
            else
                {
                // Import after a phrase; if this is an empty phrase, we'll handle that later.
                parent = mContentView.Selection.Node.ParentAs<ObiNode> ();
                index = mContentView.Selection.Node.Index + 1;
                }
            // Add all nodes in order
            for (int i = 0; i < phraseNodes.Count; ++i)
                {
                command.append ( new Commands.Node.AddNode ( this, phraseNodes[i], parent, index + i ) );
                }
            if (mContentView.Selection.Node.GetType () == typeof ( EmptyNode ))
                {
                // If the selection was an empty node, then remove (so that the first phrase is "imported into it".)
                // Remember to keep its attributes so that if we import audio into a page, it keeps its page number.
                Commands.Node.MergeAudio.AppendCopyNodeAttributes ( command, this, (EmptyNode)mContentView.Selection.Node,
                    phraseNodes[0] );
                Commands.Node.Delete delete = new Commands.Node.Delete ( this, mContentView.Selection.Node );
                delete.UpdateSelection = false;
                command.append ( delete );
                }
            return command;
            }

        private CompositeCommand GetImportSectionsFromAudioCommands ( List<PhraseNode> phraseNodes,Dictionary<PhraseNode, string> phrase_SectionNameMap )
            {
            CompositeCommand command = Presentation.CreateCompositeCommand ( Localizer.Message ( "import_phrases" ) );
            SectionNode newSectionNode = null;
            int phraseInsertIndex = 0;

            for (int i = 0; i < phraseNodes.Count; i++)
                {
                if (phrase_SectionNameMap.ContainsKey ( phraseNodes[i] ))
                    {
                    Commands.Command addSectionCmd = new Commands.Node.AddSectionNode ( this, mTOCView, phrase_SectionNameMap[phraseNodes[i]] );
                    addSectionCmd.UpdateSelection = true;
                    command.append ( addSectionCmd );
                    newSectionNode = ((Commands.Node.AddSectionNode)addSectionCmd).NewSection;
                    phraseInsertIndex = 0;
                    }

                command.append ( new Commands.Node.AddNode ( this, phraseNodes[i], newSectionNode,phraseInsertIndex  ) );
                ++phraseInsertIndex;
                }

            return command;
            }

        public bool CanImportPhrases { get { return mContentView.Selection != null && !TransportBar.IsRecorderActive; } }

        /// <summary>
        /// Bring up the file chooser to select audio files to import and return new phrase nodes for the selected files,
        /// or null if nothing was selected.
        /// </summary>
        private string[] SelectFilesToImport ()
            {
            OpenFileDialog dialog = new OpenFileDialog ();
            dialog.Multiselect = true;
            dialog.Filter = Localizer.Message ( "audio_file_filter" );
            return dialog.ShowDialog () == DialogResult.OK ? Audio.AudioFormatConverter.ConvertFiles ( dialog.FileNames, mPresentation ) : null;
            }

        public void SelectNothing () { Selection = null; }

        public void SetRoleForSelectedBlock ( EmptyNode.Role kind, string custom )
            {
            mPresentation.Do ( new Commands.Node.AssignRole ( this, SelectedNodeAs<EmptyNode> (), kind, custom ) );
            }

        public void SetSilenceRoleForSelectedPhrase ()
            {
            PhraseNode node = SelectedNodeAs<PhraseNode> ();
            if (node != null)
                {
                CompositeCommand command = Presentation.getCommandFactory ().createCompositeCommand ();
                Commands.Node.AssignRole silence = new Commands.Node.AssignRole ( this, node, EmptyNode.Role.Silence );
                command.append ( silence );
                command.setShortDescription ( silence.getShortDescription () );
                if (node.Used) command.append ( new Commands.Node.ToggleNodeUsed ( this, node ) );
                Presentation.Do ( command );
                }
            }

        public void ClearRoleOfSelectedPhrase ()
            {
            PhraseNode node = SelectedNodeAs<PhraseNode> ();
            if (node != null)
                {
                if (node.Role_ != EmptyNode.Role.Silence)
                    {
                    SetRoleForSelectedBlock ( EmptyNode.Role.Plain, null );
                    }
                else
                    {
                    CompositeCommand command = Presentation.getCommandFactory ().createCompositeCommand ();
                    Commands.Node.AssignRole ClearRoleCmd = new Commands.Node.AssignRole ( this, node, EmptyNode.Role.Plain );
                    command.append ( ClearRoleCmd );
                    command.setShortDescription ( ClearRoleCmd.getShortDescription () );
                    if (!node.Used) command.append ( new Commands.Node.ToggleNodeUsed ( this, node ) );
                    Presentation.Do ( command );
                    }

                }
            }

        /// <summary>
        /// Split a phrase at the playback cursor time (when playback is going on),
        /// the cursor position, or at both ends of the audio selection.
        /// </summary>
        public void SplitPhrase ()
            {
            bool wasPlaying = TransportBar.CurrentState == TransportBar.State.Playing;
            if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause ();

            if (Selection != null && !(Selection.Node is PhraseNode)
                && GetSelectedPhraseSection != null && !mContentView.IsSectionPhrasesVisible ( GetSelectedPhraseSection ))
                {
                MessageBox.Show ( Localizer.Message ( "ContentsHidden_CannotExecuteCommand" ) );
                return;
                }

            if (CanSplitPhrase)
                {
                bool playbackOnSelectionChangeStatus = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;
                try
                    {
                    CompositeCommand command = null;
                    command = Commands.Node.SplitAudio.GetSplitCommand ( this );
                    if (command != null) mPresentation.Do ( command );
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                if (wasPlaying || ObiForm.Settings.PlayOnNavigate) TransportBar.PlayOrResume ( mSelection.Node );
                TransportBar.SelectionChangedPlaybackEnabled = playbackOnSelectionChangeStatus;
                }
            }

        /// <summary>
        /// Merge the selected block with the following one.
        /// </summary>
        public void MergeBlockWithNext ()
            {
            if (CanMergeBlockWithNext)
                {
                if (TransportBar.IsPlayerActive) TransportBar.Pause ();
                try
                    {
                    bool playbackOnSelectionChangeStatus = TransportBar.SelectionChangedPlaybackEnabled;
                    TransportBar.SelectionChangedPlaybackEnabled = false;

                    // if selection is null or selection node is not phrase node, but at the same time, playback block is active
                    // then select phrase node from playback node
                    if ( ( Selection == null || ( Selection != null && !(Selection.Node is PhraseNode) ))
                        && mContentView.PlaybackBlock != null)
                        {
                        Selection = new NodeSelection ( mContentView.PlaybackBlock.Node, mContentView );
                        }
                    mPresentation.getUndoRedoManager ().execute ( Commands.Node.MergeAudio.GetMergeCommand ( this ) );
                    TransportBar.SelectionChangedPlaybackEnabled = playbackOnSelectionChangeStatus;
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                }
            }

        public void ToggleEmptyNodeTo_DoMark ()
            {
            if (TransportBar.Enabled)
                {
                TransportBar.MarkTodo ();
                }
            else if (IsBlockSelected)
                {
                EmptyNode node = (EmptyNode)Selection.Node;
                Commands.Node.ToggleNodeTODO command = new Commands.Node.ToggleNodeTODO ( this, node );
                Presentation.Do ( command );
                }
            }

        public void UpdateCursorPosition ( double time ) { mContentView.UpdateCursorPosition ( time ); }
        public void SelectAtCurrentTime () { mContentView.SelectAtCurrentTime (); }


        /// <summary>
        /// Used for adding custom types on the fly: add it to the presentation and also set it on the block
        /// </summary>
        /// <param name="customName"></param>
        /// <param name="kind"></param>
        public void AddCustomTypeAndSetOnBlock ( EmptyNode.Role nodeKind, string customClass )
            {
            if (IsBlockSelected)
                {
                EmptyNode node = SelectedNodeAs<EmptyNode> ();
                if (node.Role_ != nodeKind || node.CustomRole != customClass)
                    {
                    mPresentation.Do ( new Obi.Commands.Node.AssignRole ( this, node, customClass ) );
                    }
                }
            }

        public bool CanSetPageNumber { get { return IsBlockSelected; } }

        /// <summary>
        /// Set the page number on the selected block and optionally renumber subsequent blocks.
        /// </summary>
        /// <param name="number">The new page number.</param>
        /// <param name="renumber">If true, renumber subsequent blocks.</param>
        public void SetPageNumberOnSelectedBock ( PageNumber number, bool renumber )
            {
            if (CanSetPageNumber)
                {
                ICommand cmd = new Commands.Node.SetPageNumber ( this, SelectedNodeAs<EmptyNode> (), number );
                if (renumber)
                    {
                    CompositeCommand k = Presentation.CreateCompositeCommand ( cmd.getShortDescription () );
                    for (ObiNode n = SelectedNodeAs<EmptyNode> ().FollowingNode; n != null; n = n.FollowingNode)
                        {
                        if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                            ((EmptyNode)n).PageNumber.Kind == number.Kind)
                            {
                            number = number.NextPageNumber ();
                            k.append ( new Commands.Node.SetPageNumber ( this, (EmptyNode)n, number ) );
                            }
                        }
                    k.append ( cmd );
                    cmd = k;
                    }
                mPresentation.Do ( cmd );
                }
            }

        /// <summary>
        /// Add a range of pages at the selection position. The page number is increased by one for every subsequent page.
        /// </summary>
        /// <param name="number">The starting number for the range of pages.</param>
        /// <param name="count">The number of pages to add.</param>
        /// <param name="renumber">Renumber subsequent pages if true.</param>
        public void AddPageRange ( PageNumber number, int count, bool renumber )
            {
            if (CanAddEmptyBlock)
                {
                CompositeCommand cmd = Presentation.CreateCompositeCommand ( Localizer.Message ( "add_blank_pages" ) );
                int index = -1;
                ObiNode parent = null;
                // For every page, add a new empty block and give it a number.
                for (int i = 0; i < count; ++i)
                    {
                    EmptyNode node = new EmptyNode ( Presentation );
                    if (parent == null)
                        {
                        parent = mSelection.ParentForNewNode ( node );
                        index = mSelection.IndexForNewNode ( node );
                        }
                    cmd.append ( new Commands.Node.AddEmptyNode ( this, node, parent, index + i ) );
                    cmd.append ( new Commands.Node.SetPageNumber ( this, node, number ) );
                    number = number.NextPageNumber ();
                    }
                // Add commands to renumber the following pages; be careful that the previous blocks have not
                // been added yet!
                // Also be careful to only renumber pages of the same kind.
                if (renumber)
                    {
                    for (ObiNode n = parent.FollowingNodeAfter ( index - 1 ); n != null; n = n.FollowingNodeAfter ( -1 ))
                        {
                        if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                            ((EmptyNode)n).PageNumber.Kind == number.Kind)
                            {
                            Commands.Node.SetPageNumber c = new Commands.Node.SetPageNumber ( this, (EmptyNode)n, number );
                            c.UpdateSelection = false;
                            cmd.append ( c );
                            number = number.NextPageNumber ();
                            }
                        }
                    }
                mPresentation.Do ( cmd );
                }
            }

        /// <summary>
        /// Apply phrase detection on selected audio block by computing silence threshold from a silence block
        ///  nearest  preceding silence block is  used
        /// </summary>
        public void ApplyPhraseDetection ()
            {
            if (CanApplyPhraseDetection)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();
                ObiNode node;
                for (node = SelectedNodeAs<EmptyNode> ();
                    node != null && !(node is PhraseNode && ((PhraseNode)node).Role_ == EmptyNode.Role.Silence);
                    node = node.PrecedingNode) { }
                Dialogs.SentenceDetection dialog = new Obi.Dialogs.SentenceDetection ( node as PhraseNode );
                if (dialog.ShowDialog () == DialogResult.OK)
                    {
                    bool playbackOnSelectionChangedStatus = TransportBar.SelectionChangedPlaybackEnabled;
                    TransportBar.SelectionChangedPlaybackEnabled = false;
                    CompositeCommand command = null;
                    Dialogs.ProgressDialog progress = new Dialogs.ProgressDialog ( Localizer.Message ( "phrase_detection_progress" ),
                        delegate ()
                            {
                            command = Commands.Node.SplitAudio.GetPhraseDetectionCommand ( this, SelectedNodeAs<PhraseNode> (),
                                dialog.Threshold, dialog.Gap, dialog.LeadingSilence );
                            } );
                    progress.ShowDialog ();
                    mPresentation.Do ( command );

                    SectionNode SNode = GetSelectedPhraseSection;
                    if (SNode != null && SNode.PhraseChildCount > MaxVisibleBlocksCount)
                        MessageBox.Show ( string.Format ( Localizer.Message ( "ContentHidden_SectionHasOverlimitPhrases" ), SNode.Label, MaxVisibleBlocksCount ), Localizer.Message ( "Caption_Warning" ), MessageBoxButtons.OK, MessageBoxIcon.Warning );

                    // hide newly added phrases if contents of section are hidden
                    //HideNewPhrasesInInvisibleSection ( SNode ); //@singleSection: original
                    mContentView.CreateBlocksInStrip (); //@singleSection: new
                    TransportBar.SelectionChangedPlaybackEnabled = playbackOnSelectionChangedStatus;
                    }
                }
            }

        /// <summary>
        /// Select the phrase after the currently selected phrase in the same strip in the content view.
        /// If no phrase is selected, select the first phrase of the currently selected strip.
        /// If no strip is selected, select the first phrase of the first strip.
        /// </summary>
        public void SelectNextPhrase ()
            {
            mContentView.SelectNextPhrase ( SelectedNodeAs<ObiNode> () );
            }

        public void UpdateBlocksLabelInStrip ( SectionNode node )
            {
            // if (node != null) mContentView.UpdateBlocksLabelInStrip(node); //commented to improve efficiency
            }


        #region Keyboard shortcuts

        public delegate bool HandledShortcutKey ();
        public static readonly int WM_KEYDOWN = 0x100;
        public static readonly int WM_SYSKEYDOWN = 0x104;

        private Dictionary<Keys, HandledShortcutKey> mShortcutKeys;

        // Initialize the list of shortcut keys
        private void InitializeShortcutKeys ()
            {
            mShortcutKeys = new Dictionary<Keys, HandledShortcutKey> ();
            mShortcutKeys[Keys.Control | Keys.Tab] = delegate () { return SelectViewsInCycle ( true ); };
            mShortcutKeys[Keys.Control | Keys.Shift | Keys.Tab] = delegate () { return SelectViewsInCycle ( false ); };
            mShortcutKeys[Keys.F6] = delegate () { return ToggleFocusBTWTOCViewAndContentsView (); };
            mShortcutKeys[Keys.Shift | Keys.Space] = delegate () { return TogglePlayPause ( UseSelection ); };
            mShortcutKeys[Keys.Space] = delegate () { return TogglePlayPause ( UseAudioCursor ); };
            mShortcutKeys[Keys.Alt | Keys.Enter] = delegate () { return ShowNodePropertiesDialog (); };
            mShortcutKeys[Keys.F8] = delegate () { return mTransportBar.FocusOnTimeDisplay (); };
            }

        // Process key press: if this is a key down event, lookup the shortcut tables;
        // if the key was not handled then, proceed with the default process.
        protected override bool ProcessCmdKey ( ref Message msg, Keys key )
            {
            if (!CanUseKey ( key )) return false;

            return (((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) &&
                CanUseKey ( key ) && mShortcutKeys.ContainsKey ( key ) && mShortcutKeys[key] ()) ||
                base.ProcessCmdKey ( ref msg, key );
            }

        // Trap the delete key to prevent deleting a node during text editing
        private bool CanUseKey ( Keys key ) { return !((Selection is TextSelection || mFindInText.ContainsFocus) && key == Keys.Delete); }

        #endregion

        private bool SelectViewsInCycle ( bool clockwise )
            {
            List<Control> ViewList = new List<Control> ();
            ViewList.Add ( mTOCView );
            ViewList.Add ( mMetadataView );
            ViewList.Add ( mPanelInfoLabelButton );
            ViewList.Add ( mContentView );
            ViewList.Add ( mTransportBar );

            if (mTOCSplitter.Focused)
                mPanelInfoLabelButton.Focus ();

            Control FocussedView = null;
            int FocussedViewIndex = -1;

            for (int i = 0; i < ViewList.Count; i++)
                {
                if (ViewList[i].ContainsFocus)
                    {
                    FocussedView = ViewList[i];
                    FocussedViewIndex = i;
                    break;
                    }
                }

            int NewFocussedIndex = -1;
            if (FocussedViewIndex != -1)
                {
                if (clockwise)
                    {
                    NewFocussedIndex = FocusNextView ( ViewList, FocussedViewIndex );
                    return true;
                    //if (NewFocussedIndex == 2) ObiForm.SelectNextControl(ObiForm.ActiveControl, true, true, true, true);
                    }
                else
                    {
                    NewFocussedIndex = FocusPreviousView ( ViewList, FocussedViewIndex );
                    return true;
                    }
                }
            else
                {
                if (mTOCView.Focus ())
                    return true;
                else
                    return mPanelInfoLabelButton.Focus ();
                }
            }

        private int FocusNextView ( List<Control> ViewList, int FocussedViewIndex )
            {
            int Index = FocussedViewIndex;
            for (int i = 1; i <= ViewList.Count; i++)
                {
                Index = FocussedViewIndex + i;
                if (Index >= ViewList.Count)
                    Index = Index - ViewList.Count;

                if (ViewList[Index].CanFocus)
                    {
                    ViewList[Index].Focus ();
                    return Index;
                    }
                }
            return -1;
            }


        private int FocusPreviousView ( List<Control> ViewList, int FocussedViewIndex )
            {
            int Index = FocussedViewIndex;

            for (int i = 1; i <= ViewList.Count; i++)
                {
                Index = FocussedViewIndex - i;

                if (Index < 0)
                    Index = ViewList.Count + Index;

                if (ViewList[Index].CanFocus)
                    {
                    ViewList[Index].Focus ();
                    return Index;
                    }
                }
            return -1;
            }


        private static readonly bool UseSelection = true;
        private static readonly bool UseAudioCursor = false;

        // Toggle play/pause in the transport bar.
        // If the useSelection flag is set, resume from the selection
        // rather than from the audio cursor.
        public bool TogglePlayPause ( bool useSelection )
            {
            if (!(mSelection is TextSelection) &&
                (mContentView.ContainsFocus
                || mTOCView.ContainsFocus
                || mTransportBar.ContainsFocus))
                {
                if ((TransportBar.CanPausePlayback || TransportBar.CanResumePlayback) && useSelection)
                    {
                    // Resume from selection, not from audio cursor
                    TransportBar.Stop ();
                    TransportBar.PlayOrResume ();
                    return true;
                    }
                else if (TransportBar.CanPause)
                    {
                    // Pause playback or recording
                    TransportBar.Pause ();
                    return true;
                    }
                else if (TransportBar.CanPlay || TransportBar.CanResumePlayback)
                    {
                    // Start playback or resume from audio cursor
                    TransportBar.PlayOrResume ();
                    return true;
                    }
                else if (TransportBar.IsListening)
                    {
                    TransportBar.Stop ();
                    return true;
                    }
                }
            return false;
            }

        /// <summary>
        /// Toggle the TODO status of a phrase.
        /// </summary>
        public void ToggleTODOForPhrase ()
            {
            if (TransportBar.IsActive)
                {
                TransportBar.MarkTodo ();
                }
            else
                {
                ToggleEmptyNodeTo_DoMark ();
                }
            }

        // Initialize timer for tabbing
        private void InitialiseTabbingTimer ()
            {
            if (mTabbingTimer == null)
                {
                mTabbingTimer = new System.Windows.Forms.Timer ();
                mTabbingTimer.Tick += new System.EventHandler ( TabbingTimer_Tick );
                mTabbingTimer.Interval = 500;
                }
            }

        private void mPanelInfoLabelButton_Enter ( object sender, EventArgs e )
            {
            mPanelInfoLabelButton.Size = new Size ( 150, 20 );
            mPanelInfoLabelButton.BackColor = System.Drawing.SystemColors.ControlLight;
            mPanelInfoLabelButton.Text = mPanelInfoLabelButton.AccessibleName;
            InitialiseTabbingTimer ();
            mTabbingTimer.Start ();
            }

        private void TabbingTimer_Tick ( object sender, EventArgs e )
            {
            if (mPanelInfoLabelButton.ContainsFocus) SendKeys.Send ( "{tab}" );
            mTabbingTimer.Stop ();
            }

        private void mPanelInfoLabelButton_Leave ( object sender, EventArgs e )
            {
            mPanelInfoLabelButton.BackColor = System.Drawing.Color.Transparent;
            mPanelInfoLabelButton.Size = new Size ( 1, 1 );
            mPanelInfoLabelButton.Text = "";
            }

        public bool CanToggleFocusToContentsView
            {
            get
                {
                if (mTOCView.ContainsFocus)
                    return true;
                else if (mContentView.ContainsFocus)
                    return false;
                else if (mTOCView.Visible) // if neither of views has focus then check if toc is visible, if visible  and focus on it
                    return false;
                else // if neither of view has focus and TOC is not visible, focus on contents view.
                    return true;
                }
            }


        public bool ToggleFocusBTWTOCViewAndContentsView ()
            {
            if (mTOCView.ContainsFocus)
                FocusOnContentView ();
            else if (mContentView.ContainsFocus)
                FocusOnTOCView ();
            else if (mTOCView.Visible) // if neither of views has focus then check if toc is visible, if visible  and focus on it
                FocusOnTOCView ();
            else // if neither of view has focus and TOC is not visible, focus on contents view.
                FocusOnContentView ();


            return true;
            }

        public void SelectNextTODOPhrase ()
            {
            mContentView.SelectNextTODONode ();
            }

        public void SelectPreviousTODOPhrase ()
            {
            mContentView.SelectPrecedingTODONode ();
            }

        /// <summary>
        /// Ensure that the selection is in the content view.
        /// </summary>
        public void SelectInContentView ()
            {
            if (mSelection != null && mSelection.Control != mContentView)
                {
                Selection = new NodeSelection ( mSelection.Node, mContentView );
                }
            }

        public void SelectPhraseInContentView ( PhraseNode node )
            {
            if (node != null) Selection = new NodeSelection ( node, mContentView );
            }


        /// <summary>
        /// Show the project properties dialog. Handle potential renaming of the project.
        /// </summary>
        public bool ShowProjectPropertiesDialog ()
            {
            Dialogs.ProjectProperties dialog = new Dialogs.ProjectProperties ( this );
            if (dialog.ShowDialog () == DialogResult.OK && dialog.ProjectTitle != mPresentation.Title &&
                dialog.ProjectTitle != null && dialog.ProjectTitle != "")
                {
                mPresentation.Do ( new Commands.Metadata.ModifyContent ( this,
                    mPresentation.GetFirstMetadataItem ( Metadata.DC_TITLE ), dialog.ProjectTitle ) );
                }
            return true; // return true for keyboard handling
            }


        /// <summary>
        /// Show properties dialog for the selected node (phrase or section) or the project if nothing is selected.
        /// </summary>
        public bool ShowNodePropertiesDialog ()
            {
            return CanShowPhrasePropertiesDialog ? ShowPhrasePropertiesDialog ( false ) :
                CanShowSectionPropertiesDialog ? ShowSectionPropertiesDialog () :
                true;
            }

        /// <summary>
        /// Show the section properties dialog. When the user closes the dialog, look for changes to commit.
        /// Emit a single command consolidating all changes (title, level and used flag).
        /// </summary>
        public bool ShowSectionPropertiesDialog ()
            {
            Dialogs.SectionProperties dialog = new Dialogs.SectionProperties ( this );
            if (dialog.ShowDialog () == DialogResult.OK)
                {
                CompositeCommand command =
                    mPresentation.CreateCompositeCommand ( Localizer.Message ( "update_section" ) );
                if (dialog.Label != dialog.Node.Label && dialog.Label != null && dialog.Label != "")
                    {
                    command.append ( new Commands.Node.RenameSection ( this, dialog.Node, dialog.Label ) );
                    }
                for (int i = dialog.Node.Level; i < dialog.Level; ++i)
                    {
                    command.append ( new Commands.TOC.MoveSectionIn ( this, dialog.Node ) );
                    }
                for (int i = dialog.Level; i < dialog.Node.Level; ++i)
                    {
                    command.append ( new Commands.TOC.MoveSectionOut ( this, dialog.Node ) );
                    }
                if (dialog.Used != dialog.Node.Used)
                    {
                    command.append ( new Commands.Node.ToggleNodeUsed ( this, dialog.Node ) );
                    }
                if (command.getCount () == 1) command.setShortDescription ( command.getListOfCommands ()[0].getShortDescription () );
                if (command.getCount () > 0) mPresentation.Do ( command );
                }
            return true;
            }

        public bool ShowPhrasePropertiesDialog ( bool SetCustomClassName )
            {
            // if custom class is to be set then playback should be paused else allowed to continue.
            if (SetCustomClassName && TransportBar.IsPlayerActive)
                TransportBar.Pause ();

            Dialogs.PhraseProperties dialog = new Dialogs.PhraseProperties ( this, SetCustomClassName );
            if (dialog.ShowDialog () == DialogResult.OK)
                {
                CompositeCommand command =
                    mPresentation.CreateCompositeCommand ( Localizer.Message ( "update_phrase" ) );

                if (dialog.Role == EmptyNode.Role.Page && dialog.PageChange)
                    {
                    Dialogs.SetPageNumber PageDialog = new Dialogs.SetPageNumber ( this.CurrentOrNextPageNumber, false, false );
                    if (PageDialog.ShowDialog () == DialogResult.OK && CanSetPageNumber)
                        {
                        ICommand PageCmd = new Commands.Node.SetPageNumber ( this, SelectedNodeAs<EmptyNode> (), PageDialog.Number );
                        command.append ( PageCmd );
                        PageNumber number = PageDialog.Number;
                        if (PageDialog.Renumber)
                            {
                            for (ObiNode n = SelectedNodeAs<EmptyNode> ().FollowingNode; n != null; n = n.FollowingNode)
                                {
                                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                                    ((EmptyNode)n).PageNumber.Kind == number.Kind)
                                    {
                                    number = number.NextPageNumber ();
                                    command.append ( new Commands.Node.SetPageNumber ( this, (EmptyNode)n, number ) );
                                    }
                                }
                            }
                        }
                    } // page related braces ends

                if (dialog.Role != dialog.Node.Role_ ||
                    (dialog.Role == EmptyNode.Role.Custom && dialog.Node.Role_ == EmptyNode.Role.Custom &&
                    dialog.CustomClass != dialog.Node.CustomRole))
                    {
                    command.append ( new Commands.Node.AssignRole ( this, dialog.Node, dialog.Role, dialog.CustomClass ) );
                    }
                if (dialog.Used != dialog.Node.Used)
                    {
                    command.append ( new Commands.Node.ToggleNodeUsed ( this, dialog.Node ) );
                    }
                if (dialog.TODO != dialog.Node.TODO)
                    {
                    command.append ( new Commands.Node.ToggleNodeTODO ( this, dialog.Node ) );
                    }
                if (command.getCount () == 1) command.setShortDescription ( command.getListOfCommands ()[0].getShortDescription () );
                if (command.getCount () > 0) mPresentation.Do ( command );
                }
            return true;
            }

        public void CropPhrase ()
            {
            if (CanCropPhrase)
                {
                if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause ();
                bool playbackOnSelectionStatus = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;

                ICommand crop = Commands.Node.SplitAudio.GetCropCommand ( this );
                mPresentation.Do ( crop );

                TransportBar.SelectionChangedPlaybackEnabled = playbackOnSelectionStatus;
                }
            }

        /// <summary>
        /// Get the phrase node to split depending on the selection or the playback node.
        /// </summary>
        public PhraseNode GetNodeForSplit ()
            {
            PhraseNode playing = mTransportBar.PlaybackPhrase;
            return playing == null ? SelectedNodeAs<PhraseNode> () : playing;
            }

        public void SelectFromTransportBar ( ObiNode node, IControlWithSelection selectionControl )
            {
            if (node != null)
                {
                //@singleSection: added this if block and code inside it, 
                if (mContentView.ContainsFocus && node is EmptyNode)
                    {
                    mContentView.SelectPhraseBlockOrStrip ((EmptyNode) node );
                    return;
                    }
                // if block to be selected is invisible, select parent section
                if (mContentView.IsBlockInvisibleButStripVisible ( (EmptyNode)node )
                    || (selectionControl != null && selectionControl is TOCView))
                    {
                    node = node.ParentAs<SectionNode> ();
                    }

                if (selectionControl == null)
                    Selection = new NodeSelection ( node, mContentView );
                else
                    Selection = new NodeSelection ( node, selectionControl );

                }
            }

        [DefaultValue ( 0.01f )]
        public float AudioScale
            {
            get { return ObiForm == null ? 0.01f : ObiForm.AudioScale; }
            set { if (value > 0.0f) mContentView.AudioScale = value; }
            }

        [DefaultValue ( 1.0f )]
        public float ZoomFactor
            {
            get { return ObiForm == null ? 1.0f : ObiForm.ZoomFactor; }
            set
                {
                if (value > 0.0f)
                    {
                    mTOCView.ZoomFactor =
                    mContentView.ZoomFactor =
                    mMetadataView.ZoomFactor =
                    mFindInText.ZoomFactor = value;
                    }
                }
            }

        /// <summary>
        /// Update the context menus of the view.
        /// </summary>
        public void UpdateContextMenus ()
            {
            mTOCView.UpdateContextMenu ();
            mContentView.UpdateContextMenu ();
            }

        /// <summary>
        /// Add a custom class to the content view context menu.
        /// </summary>
        public void AddCustomRoleToContextMenu ( string name, ObiForm form )
            {
            mContentView.AddCustomRoleToContextMenu ( name, form );
            }


        /// <summary>
        ///  Remove a custom class from contents view context menu
        /// </summary>
        public void RemoveCustomRoleFromContextMenu ( string name, ObiForm form )
            {
            mContentView.RemoveCustomRoleFromContextMenu ( name, form );
            }

        public void SuspendLayout_All ()
            {
            mContentView.SuspendLayout_All ();
            }

        public void ResumeLayout_All ()
            {
            mContentView.ResumeLayout_All ();
            }

        public CompositeCommand GetRenumberPageKindCommand ( EmptyNode node, PageNumber number )
            {
            CompositeCommand k = Presentation.CreateCompositeCommand ( "RenumberAll" );
            for (ObiNode n = node; n != null; n = n.FollowingNode)
                {
                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                    ((EmptyNode)n).PageNumber.Kind == number.Kind)
                    {
                    k.append ( new Commands.Node.SetPageNumber ( this, (EmptyNode)n, number ) );
                    number = number.NextPageNumber ();

                    }
                }
            return k;
            }

        // @phraseLimit
        /// <summary>
        /// shows contents of selected strip
        /// </summary>
        public void ShowSelectedSectionContents ()
            {
            //
            if (Selection != null && Selection.Node is SectionNode)//@singleSection
                {
                mContentView.CreateStripForSelectedSection ( (SectionNode)Selection.Node, true );
                }
//@singleSection: commented following two lines as this is not required with single section
            //if (CanShowSectionContents)
                //mContentView.CreateBlocksInStrip ();
            }

        /*@singleSection
        // @phraseLimit
        /// <summary>
        /// makes contents of strips invisible if blocks visibility limit is exceeded. The operation is started from selected section
        /// </summary>
        /// <param name="removeFromSelected"></param>
        public void MakeOldStripsBlocksInvisible ( bool removeFromSelected ) { mContentView.MakeOldStripsBlocksInvisible ( removeFromSelected ); }
        */

        // @phraseLimit
        /// <summary>
        /// if parameter section has its contents hidden, makes all its new phrases hidden
        /// </summary>
        /// <param name="section"></param>
        public void HideNewPhrasesInInvisibleSection ( SectionNode section )
            {
            if (section != null && !mContentView.IsSectionPhrasesVisible ( section ))
                {
                MessageBox.Show ( Localizer.Message ( "ContentsHidden_HideNewPhrases" ) );
                mContentView.RemoveBlocksInStrip ( section );
                }
            }

        //@phraseLimit
        public void ChangeVisibilityProcessState ( bool active )
            {
            if (active)
                {
                this.Cursor = Cursors.WaitCursor;
                }
            else
                {
                this.Cursor = Cursors.Default;
                }
            if (BlocksVisibilityChanged != null) BlocksVisibilityChanged ( this, new EventArgs () );
            }

        /// <summary>
        /// searches page number or phrase index entered in dialog box and highlight it in content view
        /// </summary>
        public void GoToPageOrPhrase ()
            {
            Dialogs.GoToPageOrPhrase GoToDialog = new Obi.Dialogs.GoToPageOrPhrase ();
            if (GoToDialog.ShowDialog () == DialogResult.OK)
                {
                if (GoToDialog.Number != null)
                    {
                    int pageNumber = GoToDialog.Number.Number;
                    PageKind kind = GoToDialog.Number.Kind;
                    EmptyNode node = null;
                    EmptyNode firstSpecialPageMatch = null; // holds first match of special page

                    //flag to indicate if iterations has passed through selected node.   is true if iteration is moved ahead of selected node
                    bool isAfterSelection =( Selection == null || ( Selection != null && Presentation.FirstSection == Selection.Node)) ? true : false; 

                    for (ObiNode n = Presentation.RootNode.FirstLeaf; n != null; n = n.FollowingNode)
                        {
                        if (n is EmptyNode)
                            {
                            EmptyNode testNode = (EmptyNode)n;
                            if (testNode.Role_ == EmptyNode.Role.Page
                                && testNode.PageNumber.Kind == kind)
                                {
                                // test special pages and other pages separately
                                if (testNode.PageNumber.Kind == PageKind.Special)
                                    {
                                    if (testNode.PageNumber.ArabicNumberOrLabel == GoToDialog.Number.ArabicNumberOrLabel)
                                        {
                                        if ( firstSpecialPageMatch == null )  firstSpecialPageMatch = testNode;

                                        if (Selection != null 
                                            && isAfterSelection )
                                            {
                                            node = testNode;
                                            break;
                                            }
                                        }
                                    }
                                else if (testNode.PageNumber.Number == pageNumber) // if not special compare int number
                                    {
                                    node = testNode;
                                    break;
                                    }

                                }
                            }
                        // check if iterations has passed selected node
                        if (Selection != null &&
                            Selection.Node == n)
                            {
                            isAfterSelection = true;
                            }
                        }

                    // check if special page is null
                    if (kind == PageKind.Special
                        && node == null && firstSpecialPageMatch != null)
                        {
                        node = firstSpecialPageMatch;
                        }

                    if (node != null)
                        {
                        //@singleSection
                        //Selection = new NodeSelection ( node, mContentView );
                        if (TransportBar.IsPlayerActive) TransportBar.Pause ();
                        mContentView.SelectPhraseBlockOrStrip ( node );
                        }
                    else
                        {
                        MessageBox.Show ( "Page does not exist" );
                        }
                    }
                else if (GoToDialog.PhraseIndex != null)
                    {
                    int phraseIndex = (int)GoToDialog.PhraseIndex - 1;
                    SectionNode section = GetSelectedPhraseSection != null ? GetSelectedPhraseSection : 
                        mContentView.ActiveStrip != null ? mContentView.ActiveStrip.Node ://@singleSection
                        mPresentation.FirstSection;
                    
                    if (section != null && section.PhraseChildCount > 0)
                        {
                        if (phraseIndex >= section.PhraseChildCount)
                            {
                            // for message box display, phrase index should start from 1 so it should be incremented for display.
                            if (MessageBox.Show ( string.Format ( Localizer.Message ( "GoToPageOrPhrase_MoreThanPhraseCount" ), (phraseIndex + 1).ToString (), (section.PhraseChildCount).ToString () ),
    "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 ) == DialogResult.Yes)
                                {
                                phraseIndex = section.PhraseChildCount - 1;
                                }
                            else
                                {
                                return;
                                }
                            }
                        //@singleSection
                        //Selection = new NodeSelection ( section.PhraseChild ( phraseIndex ), mContentView );
                        if (TransportBar.IsPlayerActive) TransportBar.Pause ();
                        mContentView.SelectPhraseBlockOrStrip ( section.PhraseChild ( phraseIndex ) );
                        } // section null check ends
                    }


                } // dialog OK check ends
            }


        //@singleSection
        public void RecreateContentsWhileInitializingRecording ( EmptyNode recordingResumePhrase ) { mContentView.RecreateContentsWhileInitializingRecording ( recordingResumePhrase ); }

        //@singleSection
        public bool IsContentViewScrollActive { get { return mContentView.IsScrollActive; } }

        /// <summary>
        /// Work around specificallly for disabling scrolling during some conditions of playback @AudioScrolling
        /// </summary>
        public void DisableScrollingInContentsView ()
            {
            mContentView.DisableScrolling ();
            }


        //@ShowSingleSection
        /*
        // taken from project view line 786
public bool ShowOnlySelectedSection
        {
            set
            {
                mShowOnlySelected = value;
                                UpdateShowOnlySelected(value);
            }
        }

        
        private void UpdateShowOnlySelected(bool showOnly)
        {
            ObiNode node = SelectedNodeAs<ObiNode>();
            node = node is SectionNode ? (SectionNode)node :
                node == null || node is RootNode ? null :
                node.AncestorAs<SectionNode>();
            if (showOnly)
            {
                            mContentView.ShowOnlySelectedSection(node);
            }
            else
            {
                SynchronizeViews = mSynchronizeViews;
            }
            mContentView.MakeStripVisibleForSection((SectionNode)node);
        }
        */

        }

    public class ImportingFileEventArgs
        {
        public string Path;  // path of the file being imported
        public ImportingFileEventArgs ( string path ) { Path = path; }
        }

    public delegate void ImportingFileEventHandler ( object sender, ImportingFileEventArgs e );
    }
