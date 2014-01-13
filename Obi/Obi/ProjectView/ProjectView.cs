using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using AudioLib;
using urakawa.command;
using urakawa.daisy.export.visitor;
using urakawa.property.channel;

//using urakawa.publish;

namespace Obi.ProjectView
    {
    public partial class ProjectView : UserControl
        {
        private ObiPresentation mPresentation;  // presentation
        private NodeSelection mSelection;    // currently selected node
        private Clipboard mClipboard;        // the clipboard
        private bool mSynchronizeViews;      // synchronize views flag
        private ObiForm mForm;               // parent form
        private bool mTOCViewVisible;        // keep track of the TOC view visibility (don't reopen it accidentally)
        private bool mMetadataViewVisible;   // keep track of the Metadata view visibility
        private Timer mTabbingTimer;         // ??
        private bool m_DisableSectionSelection;//@singleSection
        HelpProvider helpProvider1;
        
        //private bool mShowOnlySelected; // is set to show only one section in contents view. @show single section
        public readonly int MaxVisibleBlocksCount; // @phraseLimit
        public readonly int MaxOverLimitForPhraseVisibility; // @phraseLimit
        public  static string m_LogFilePath;
        public static string m_LogFilePathPrev;

        public event EventHandler SelectionChanged;             // triggered when the selection changes
        public event EventHandler FindInTextVisibilityChanged;  // triggered when the search bar is shown or hidden
        public event EventHandler BlocksVisibilityChanged; // triggered when phrase blocks are bbecoming  visible or invisible // @phraseLimit
        public event ProgressChangedEventHandler ProgressChanged; //Updates the toolstrip progress bar on obi form
        
    
        /// <summary>
        /// Create a new project view with no project yet.
        /// </summary>
        public ProjectView ()
            {
            InitializeComponent ();
            //InitializeShortcutKeys ();
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
            MaxVisibleBlocksCount = 10000; // @phraseLimit
            MaxOverLimitForPhraseVisibility = 300; // @phraseLimit
            m_DisableSectionSelection = false;
            m_LogFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ObiSession.log");
            m_LogFilePathPrev= System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ObiSessionPrev.log");
            VerifyLogFileExistenceWhileStartup();
            helpProvider1 = new HelpProvider();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            }


        /// <summary>
        /// Add a new empty block.
        /// </summary>
        public void AddEmptyBlock ()
            {
            if (CanAddEmptyBlock)
                {
                    EmptyNode node = Presentation.TreeNodeFactory.Create<EmptyNode>();
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

                Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber ( NextPageNumber, true, true );
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

        public EmptyNode BeginNote    //@AssociateNode
        { 
            get { return mContentView.BeginSpecialNode; }
            set { mContentView.BeginSpecialNode = value; }
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

        public void SaveDefaultMetadatas()
        {
            if (mPresentation != null)
            {
                Dictionary<string, string> metadataDictionary = new Dictionary<string, string>();
                foreach (urakawa.metadata.Metadata m in mPresentation.Metadatas.ContentsAs_ListCopy)
                {
                    if (m.NameContentAttribute.Name != Metadata.DC_IDENTIFIER)
                    {
                        if (!metadataDictionary.ContainsKey(m.NameContentAttribute.Name)) metadataDictionary.Add(m.NameContentAttribute.Name, m.NameContentAttribute.Value);
                    }
                }
                ObiForm.Settings_Permanent.UpdateDefaultMetadata(metadataDictionary);
                ObiForm.Settings_Permanent.SaveSettings();
            }
        }

        public void LoadDefaultMetadata(bool overwriteExisting)
        {
            if (mPresentation == null) return;
            Dictionary<string, string> defaultMetadatas = ObiForm.Settings_Permanent.GetDefaultMetadata();
            if (defaultMetadatas != null && defaultMetadatas.Count > 0)
            {
                CompositeCommand command = mPresentation.CreateCompositeCommand(Localizer.Message("Metadata_SetDefaults"));
                foreach (urakawa.metadata.Metadata m in mPresentation.Metadatas.ContentsAs_ListCopy)
                {
                    if (defaultMetadatas.ContainsKey(m.NameContentAttribute.Name))
                    {
                        if (m.NameContentAttribute.Name == Metadata.DC_IDENTIFIER) continue;
                        //Console.WriteLine(m.NameContentAttribute.Name);
                        if (overwriteExisting)
                        {
                            string content = m.NameContentAttribute.Name == Metadata.DC_DATE? DateTime.Today.ToString("yyyy-MM-dd") : defaultMetadatas[m.NameContentAttribute.Name];
                            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Metadata.ModifyContent(this, m, content));
                        }
                        defaultMetadatas.Remove(m.NameContentAttribute.Name);
                    }
                }
                    foreach( string name in defaultMetadatas.Keys)
                    {
                        if (name == Metadata.DC_IDENTIFIER) continue;
                        Commands.Metadata.AddEntry addEntryCmd = new Commands.Metadata.AddEntry(this, name);
                        command.ChildCommands.Insert(command.ChildCommands.Count, addEntryCmd);

                        string content = name == Metadata.DC_DATE ? DateTime.Today.ToString("yyyy-MM-dd") : defaultMetadatas[name];
                            
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Metadata.ModifyContent(this, addEntryCmd.Entry, content));
                    }
                    try
                    {
                        if (command.ChildCommands.Count > 0) mPresentation.Do(command);
                    }
                    catch (System.Exception ex)
                    {
                        WriteToLogFile(ex.ToString());
                        MessageBox.Show(ex.ToString());
                    }

            }
        }

        /// <summary>
        /// Add a new section or strip.
        /// </summary>
        public void AddSection ()
            {
            // if metadata is selected, for section creating rules, treat as nothing is selected.
            if (Selection != null && Selection is MetadataSelection)
                Selection = null;

            // handle recording time section creation
            if ( TransportBar.CurrentState == TransportBar.State.Monitoring ) return ;
            if (Selection != null && Selection.Control is ContentView && TransportBar.CurrentState == TransportBar.State.Recording)
                {
                TransportBar.NextSection ();
                return;
                }

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

            // handle recording time section creation
            if (TransportBar.CurrentState == TransportBar.State.Monitoring) return;
            if (Selection != null && Selection.Control is ContentView && TransportBar.CurrentState == TransportBar.State.Recording)
            {
                TransportBar.NextSection();
                return;
            }
            if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();

            // select parent section node if a child phrase node is selected
            if (Selection != null && Selection.Node is EmptyNode)
                {
                Selection = new NodeSelection ( Selection.Node.ParentAs<SectionNode> (), Selection.Control );
                }
            Commands.Node.AddSectionNode add = new Commands.Node.AddSectionNode ( this, mContentView );

            if (mContentView.Selection != null)
                {
                CompositeCommand command = mPresentation.CreateCompositeCommand ( add.ShortDescription );
                SectionNode selected = mContentView.Selection.Node is SectionNode ?
                    (SectionNode)mContentView.Selection.Node : mContentView.Selection.Node.ParentAs<SectionNode> ();
                command.ChildCommands.Insert(command.ChildCommands.Count, add );
                for (int i = selected.SectionChildCount - 1; i >= 0; --i)
                    {
                    SectionNode child = selected.SectionChild ( i );
                    Commands.Node.Delete delete = new Commands.Node.Delete ( this, child );
                    delete.UpdateSelection = false;
                    command.ChildCommands.Insert(command.ChildCommands.Count, delete );
                    Commands.Node.AddNode readd = new Commands.Node.AddNode ( this, child, add.NewSection, 0 );
                    readd.UpdateSelection = false;
                    command.ChildCommands.Insert(command.ChildCommands.Count, readd );
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
        public void SetExportPathMetadata ( Obi.ImportExport.ExportFormat format, string exportPath, string projectDirectory )
            {
            string exportMetadataName = null;
            if (format == Obi.ImportExport.ExportFormat.DAISY3_0)
                {
                exportMetadataName = Metadata.OBI_DAISY3ExportPath;
                }
            else if (format == Obi.ImportExport.ExportFormat.DAISY2_02)
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
        public string GetDAISYExportPath ( Obi.ImportExport.ExportFormat format, string projectDirectory )
            {
            if (mPresentation == null)
                return null;

            urakawa.metadata.Metadata m = null;
            if (format == Obi.ImportExport.ExportFormat.DAISY3_0)
                {
                m = mPresentation.GetFirstMetadataItem ( Metadata.OBI_DAISY3ExportPath );
                }
            else if (format == Obi.ImportExport.ExportFormat.DAISY2_02)
                {
                m = mPresentation.GetFirstMetadataItem ( Metadata.OBI_DAISY2ExportPath );
                }

            string exportPath = m != null ?
                        m.NameContentAttribute.Value : null;

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
            node.AcceptDepthFirst (
                delegate ( urakawa.core.TreeNode n )
                    {
                    if (n is ObiNode && ((ObiNode)n).Used)
                        {
                        Commands.Node.ToggleNodeUsed unused = new Commands.Node.ToggleNodeUsed ( this, (ObiNode)n );
                        unused.UpdateSelection = false;
                        command.ChildCommands.Insert(command.ChildCommands.Count, unused );
                        }
                    return true;
                    }, delegate ( urakawa.core.TreeNode n ) { }
            );
            }

        public bool CanAddEmptyBlock { get { return mContentView.Selection != null && IsPhraseCountWithinLimit; } } // @phraseLimit
        public bool CanAddMetadataEntry () { return mPresentation != null; }
        public bool CanAddMetadataEntry ( MetadataEntryDescription d ) { return mMetadataView.CanAdd ( d ); }
        public bool CanAddSection { get { return mPresentation != null && (mTOCView.CanAddSection || mContentView.CanAddStrip) && !(Selection is TextSelection) && !IsZoomWaveformActive ; } }
        public bool CanAddSubsection { get { return mTOCView.CanAddSubsection; } }

        public bool CanUpdatePhraseDetectionSettingsFromSilencePhrase { get { return mPresentation != null && Selection != null && Selection.Node is PhraseNode && Selection.EmptyNodeForSelection.Role_ == EmptyNode.Role.Silence; } }
        public bool CanApplyPhraseDetectionInWholeProject { get { return mPresentation != null && mPresentation.RootNode.Children.Count > 0 && !TransportBar.IsRecorderActive && !IsZoomWaveformActive; } }
        public bool CanApplyPhraseDetection
            {
            get
                {
                return mPresentation != null &&
                    Selection != null &&
                    ((Selection.Node is PhraseNode && ((PhraseNode)Selection.Node).Role_ != EmptyNode.Role.Silence) || (Selection.Node is SectionNode && ((SectionNode)Selection.Node).PhraseChildCount > 0)) &&
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

        public bool CanAssignAnchorRole   //@AssociateNode
        {
            get
            {
                PhraseNode node = SelectedNodeAs<PhraseNode>();
                return node != null && node.Role_ != EmptyNode.Role.Anchor;
            }
        }

        public bool CanAssociateNode    //@AssociateNode
        {
            get
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                return mPresentation != null && node != null 
                    && !TransportBar.IsRecorderActive && 
                    (Selection == null || (Selection != null && node is EmptyNode && node.Role_ != EmptyNode.Role.Custom) 
                    && (node.Role_ == EmptyNode.Role.Anchor || CanAssignAnchorRole));
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

        public bool CanClearSkippableRole 
        { 
            get 
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                return CanAssignPlainRole && node.Role_ == EmptyNode.Role.Custom && !string.IsNullOrEmpty(node.CustomRole) && EmptyNode.SkippableNamesList.Contains(node.CustomRole);
            }
        }

        public bool CanMoveToStartNote
        {
            get
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                return Selection != null && node != null && node.IsRooted && node is EmptyNode && node.Role_ == EmptyNode.Role.Custom && node.Index > 0;  //@AssociateNode()
            }
        }

        public bool CanMoveToEndNote
        {
            get
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                return Selection != null && node != null && node.IsRooted && node is EmptyNode &&node.Index < node.ParentAs<SectionNode>().PhraseChildCount - 1 && node.Role_ == EmptyNode.Role.Custom;                 
            }
        }

        public bool CanRemoveSkippableNode
        {
            get
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                return Selection != null && node != null && !TransportBar.IsRecorderActive && node is EmptyNode && node.Role_ == EmptyNode.Role.Anchor && node.AssociatedNode != null;
            }
        }

        public bool CanGotoSkippableNote
        {
            get
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                return Selection != null && node != null && node.IsRooted && node is EmptyNode && node.Role_ == EmptyNode.Role.Anchor && node.AssociatedNode != null; //@AssociateNode           
            }
        }

        public bool CanBeginSpecialNote
        {
            get
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                return Selection != null && node != null && !TransportBar.IsRecorderActive && node is EmptyNode && node.Role_ != EmptyNode.Role.Anchor && !IsZoomWaveformActive;
            }
        }

        public bool CanEndSpecialNote
        {
            get
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                //@enable begin node = end node
                //return mPresentation != null && node != null && !TransportBar.IsRecorderActive && Selection != null && BeginNote != null && node is EmptyNode && (BeginNote != mContentView.EndSpecialNode || BeginNote != node) && node.ParentAs<SectionNode>() == BeginNote.ParentAs<SectionNode>();
                if (mPresentation == null || node == null || BeginNote == null || !node.IsRooted || !BeginNote.IsRooted) return false;
                return Selection != null &&  !TransportBar.IsRecorderActive && node is EmptyNode && BeginNote.Index <= node.Index && node.ParentAs<SectionNode>() == BeginNote.ParentAs<SectionNode>() && !IsZoomWaveformActive;
            }
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
        public bool CanDecreaseLevel { get { return mTOCView.CanDecreaseLevel && !TransportBar.IsRecorderActive; } }
        public bool CanDelete { get { return mPresentation != null && Selection != null && (CanRemoveSection || CanRemoveStrip || CanRemoveBlock || CanRemoveAudio || CanRemoveMetadata) && !TransportBar.IsRecorderActive && !(Selection is TextSelection); } }
        public bool CanFastForward { get { return mTransportBar.CanFastForward; } }
        public bool CanFocusOnContentView { get { return mPresentation != null && !mContentView.Focused; } }
        public bool CanFocusOnTOCView { get { return mPresentation != null && !mTOCView.Focused; } }
        public bool CanIncreaseLevel { get { return mTOCView.CanIncreaseLevel && !TransportBar.IsRecorderActive; } }
        public bool CanInsertSection { get { return (CanInsertStrip || mTOCView.CanInsertSection) && !TransportBar.IsRecorderActive && Presentation != null && Presentation.FirstSection != null && !(Selection is TextSelection) && !IsZoomWaveformActive ; } }
        public bool CanInsertStrip { get { return mContentView.Selection != null && !TransportBar.IsRecorderActive; } }
        //public bool CanMergeStripWithNext { get { return mContentView.CanMergeStripWithNext && !TransportBar.IsRecorderActive; } }
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

        public bool CanMergeStripWithNext
            {
            get
                {
                //return mContentView.CanMergeStripWithNext && !TransportBar.IsRecorderActive;  //@singleSection: commented
                return this.Selection != null && this.Selection.Node is SectionNode &&
                    mSelection.Node.IsRooted && //@singleSection
                    !TransportBar.IsRecorderActive &&
                     (mSelection.Node.Index < mSelection.Node.ParentAs<ObiNode> ().SectionChildCount - 1 ||
                        ((SectionNode)mSelection.Node).SectionChildCount > 0);
                }
            }

        public bool CanDeleteFollowingPhrasesInSection
            {
            get
                {
                return Selection != null
                && ((Selection.Node is EmptyNode && Selection.Node.IsRooted && ((EmptyNode)Selection.Node).Index < GetSelectedPhraseSection.PhraseChildCount - 1) || (Selection is StripIndexSelection && ((StripIndexSelection)Selection).EmptyNodeForSelection != null) || (Selection.Node is SectionNode && ((SectionNode)Selection.Node).PhraseChildCount > 0 && !(Selection is StripIndexSelection)))
                && !TransportBar.IsRecorderActive;
                }
            }

        public bool CanGenerateSpeechForPage { get { return mPresentation != null && Selection != null && Selection.Node is EmptyNode && ((EmptyNode)Selection.Node).Role_ == EmptyNode.Role.Page && ((EmptyNode)Selection.Node).Duration == 0 && !TransportBar.IsRecorderActive;  } }
        public bool CanGenerateSpeechForAllEmptyPages { get { return mPresentation != null && !TransportBar.IsRecorderActive; } }

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
        public bool CanRenameSection { get { return Selection != null && !TransportBar.IsRecorderActive && (CanRenameStrip || mTOCView.CanRenameSection); } }
        public bool CanRenameStrip { get { return Selection != null && (mContentView.CanRenameStrip); } }
        public bool CanRewind { get { return mTransportBar.CanRewind; } }
        public bool CanSetBlockUsedStatus { get { return mContentView.CanSetBlockUsedStatus; } }
        public bool CanSetSectionUsedStatus { get { return mTOCView.CanSetSectionUsedStatus; } }
        public bool CanSetSelectedNodeUsedStatus { get { return CanSetSectionUsedStatus || CanSetBlockUsedStatus; } }
        public bool CanSetTODOStatus { get { return IsBlockSelected || mTransportBar.IsActive; } }
        public bool CanShowOnlySelectedSection { get { return SelectedNodeAs<ObiNode> () != null; } }
        public bool CanSplitStrip { get { return mContentView.CanSplitStrip && !TransportBar.IsRecorderActive && !IsZoomWaveformActive; } }
        public bool CanStop { get { return mTransportBar.CanStop; } }
        public bool CanCropPhrase
            {
            get
                {
                return Selection != null && Selection is AudioSelection && ((AudioSelection)Selection).AudioRange != null && !((AudioSelection)Selection).AudioRange.HasCursor && IsPhraseCountWithinLimit; // @phraseLimit
                }
            }
        public bool CanMergeProject { get { return mPresentation != null; } }

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

            try
                {
            if (CanRemoveSection || CanRemoveStrip)
                {
                bool isSection = mSelection.Control is TOCView;
                CompositeCommand command = Presentation.CreateCompositeCommand (
                    Localizer.Message ( isSection ? "cut_section" : "cut_section_shallow" ) );
                
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Copy(this, isSection));
                    if (CanRemoveStrip)
                        command.ChildCommands.Insert(command.ChildCommands.Count, mContentView.DeleteStripCommand());
                    else
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(this, mSelection.Node));
                    mPresentation.Do(command);
                
                // quick fix for null selection problem while cutting single section ins single section project
                if (mPresentation.FirstSection == null) Selection = null;
                }
            else if (CanRemoveBlock)
                {
                    EmptyNode anchor = null;
                    if (CanDeleteSpecialNode(out anchor)) //@AssociateNode
                    {
                        CompositeCommand command = mPresentation.CommandFactory.CreateCompositeCommand();
                        command.ShortDescription = Localizer.Message("cut_phrase");
                        
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Copy(this, true));
                        if (anchor != null) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AssociateAnchorNode(this, anchor, (EmptyNode)Selection.Node.FollowingNode));
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(this, mSelection.Node));
                        mPresentation.Do(command);                       
                    }
                }
            else if (CanRemoveAudio)
                {
                CompositeCommand command = mPresentation.CommandFactory.CreateCompositeCommand ();
                command.ShortDescription = Localizer.Message ( "cut_audio" ) ;
                urakawa.command.Command delete = Commands.Audio.Delete.GetCommand ( this );
                PhraseNode deleted = delete is Commands.Audio.Delete ?
                    ((Commands.Audio.Delete)delete).Deleted : (PhraseNode)Selection.Node;
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Audio.Copy ( this, deleted,
                    new AudioRange ( 0.0, deleted.Audio.Duration.AsMilliseconds ) ) );
                command.ChildCommands.Insert(command.ChildCommands.Count, delete );
                mPresentation.Do ( command );
                }
            }
            catch (System.Exception ex)
            {
                this.WriteToLogFile(ex.ToString());
                MessageBox.Show(Localizer.Message("ProjectViewFormMsg_CutOperationFail") + "\n\n" + ex.ToString());   //@Messagecorrected
            }
            }

     
        /// <summary>
        /// Delete the current selection. Noop if there is no selection.
        /// </summary>
        public void Delete ()
            {
            if (Selection != null && Selection is TextSelection) return;
            if (CanDelete && mTransportBar.IsPlayerActive) mTransportBar.Stop ();
            
            try
            {
                if (CanRemoveSection)
                {
                    mPresentation.Do(new Commands.Node.Delete(this, mTOCView.Selection.Section,
                        Localizer.Message("delete_section")));
                }
                else if (CanRemoveStrip)
                {
                    // first create landing strip
                    if (Selection != null && Selection.Node is SectionNode) //@singleSection: begin
                    {
                        SectionNode section = (SectionNode)Selection.Node;
                        SectionNode landingSectionNode = section.FollowingSection;
                        if (landingSectionNode == null) landingSectionNode = section.PrecedingSection;

                        if (landingSectionNode != null)
                        {
                            mContentView.CreateStripForSelectedSection(landingSectionNode, false);
                        }
                    }//@singleSection: end

                    mPresentation.Do(mContentView.DeleteStripCommand());
                }
                else if (CanRemoveBlock)
                {
                    EmptyNode anchor = null;
                    if (CanDeleteSpecialNode(out anchor))              //@AssociateNode
                    {
                        urakawa.command.CompositeCommand deleteCmd = mPresentation.CreateCompositeCommand("Delete command");

                        if (anchor != null) deleteCmd.ChildCommands.Insert(deleteCmd.ChildCommands.Count, new Commands.Node.AssociateAnchorNode(this, anchor, (EmptyNode) Selection.Node.FollowingNode));
                        deleteCmd.ChildCommands.Insert(deleteCmd.ChildCommands.Count, new Commands.Node.Delete(this, SelectedNodeAs<EmptyNode>(),
                           Localizer.Message("delete_phrase")));
                        mPresentation.Do(deleteCmd);
                    }
                }
                else if (CanRemoveAudio)
                {
                    mPresentation.Do(Commands.Audio.Delete.GetCommand(this));
                }
                else if (CanRemoveMetadata)
                {
                    mPresentation.Do(new Commands.Metadata.DeleteEntry(this));
                }
            }
            catch (System.Exception ex)
            {
                this.WriteToLogFile(ex.ToString());
                MessageBox.Show(Localizer.Message("ProjectViewFormMsg_DeleteOperationFail") + "\n\n" + ex.ToString());  //@Messagecorrected
            }
            }

        public bool CanDeleteSpecialNode(out EmptyNode anchor)   //@AssociateNode
        {
            anchor = null;
            bool canDeleteSpecialNode = false;             
            if (((EmptyNode)Selection.Node).Role_ == EmptyNode.Role.Custom)
            {
              //  MessageBox.Show(((EmptyNode)Selection.Node).Index.ToString());
                if (((EmptyNode)Selection.Node).Index == ((EmptyNode)Selection.Node).ParentAs<SectionNode>().PhraseChildCount - 1)
                    return true;
                if (((EmptyNode)Selection.Node).Index == 0 && (((EmptyNode)Selection.Node).Index == 0 && ((EmptyNode)Selection.Node).CustomRole != ((EmptyNode)((EmptyNode)Selection.Node).FollowingNode).CustomRole))
                    return true;
                if (((EmptyNode)Selection.Node).CustomRole != ((EmptyNode)((EmptyNode)Selection.Node).FollowingNode).CustomRole)
                    canDeleteSpecialNode = true;
                else if ((((EmptyNode)Selection.Node).Index == 0 && ((EmptyNode)Selection.Node).CustomRole == ((EmptyNode)((EmptyNode)Selection.Node).FollowingNode).CustomRole) 
                    || (((EmptyNode)Selection.Node).CustomRole != ((EmptyNode)((EmptyNode)Selection.Node).PrecedingNode).CustomRole))
                {
                    if (mPresentation.GetAnchorForReferencedNode((EmptyNode)Selection.Node) != null)
                    {
                        if (MessageBox.Show(Localizer.Message("Associate_next_phrase"), "Delete", MessageBoxButtons.YesNo,
                              MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            anchor = mPresentation.GetAnchorForReferencedNode((EmptyNode)Selection.Node);
                            canDeleteSpecialNode = true;
                        }
                        else
                            return false;
                    }
                    else
                        canDeleteSpecialNode = true;
                }
                else
                {
                    if (MessageBox.Show(Localizer.Message("Node_between_chunk"), "Delete", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) == DialogResult.Yes)
                        canDeleteSpecialNode = true;
                    
                    else
                        return false;
                }
            }
            else
                canDeleteSpecialNode = true;
            return canDeleteSpecialNode;
        }

        public bool CanDeleteMetadataEntry ( urakawa.metadata.Metadata m )
            {
            return mPresentation.Metadatas.ContentsAs_ListCopy.Contains ( m );
            }

        /// <summary>
        /// Delete all unused nodes.
        /// Ask before deleting silence phrases (unless they are in unused sections...)
        /// </summary>
        public void DeleteUnused ()
            {
                if (mPresentation.RootNode.Children.Count > 0)
                {
                    // handle selection to avoid exception if selected node is deleted
                    int sectionPosition = -1;
                    
                    if (GetSelectedPhraseSection != null && (!GetSelectedPhraseSection.Used))
                    {
                        sectionPosition = GetSelectedPhraseSection.Position;
                        Selection = null;
                    }

                    // update selection if unused phrase is selected as it will be deleted
                    if (Selection != null && Selection.Node is EmptyNode && !((EmptyNode)Selection.Node).Used && GetSelectedPhraseSection.Used) 
                    {
                        EmptyNode currentlySelectedEmptyNode = (EmptyNode)Selection.Node ;
                        SectionNode currentlySelectedSection = currentlySelectedEmptyNode.ParentAs<SectionNode> ();
                        
                        ObiNode newSelectionNode = null ;
                        for (int i = currentlySelectedEmptyNode.Index; i < currentlySelectedSection.PhraseChildCount ; i++)
                        {
                            if (currentlySelectedSection.PhraseChild(i).Used)
                            {
                                newSelectionNode = currentlySelectedSection.PhraseChild(i);
                                break;
                            }
                        }
                        if (currentlySelectedEmptyNode == null)
                        {
                            for (int i = currentlySelectedEmptyNode.Index; i >= 0; i--)
                            {
                                if (currentlySelectedSection.PhraseChild(i).Used)
                                {
                                    newSelectionNode = currentlySelectedSection.PhraseChild(i);
                                    break;
                                }
                            }
                        }
                        if (newSelectionNode == null) newSelectionNode = currentlySelectedSection;
                        if (newSelectionNode != null) Selection = new NodeSelection(newSelectionNode, mContentView);
                    }
                    CompositeCommand command = mPresentation.CreateCompositeCommand(Localizer.Message("delete_unused"));
                    // Collect silence node deletion commands separately in case the user wants to keep them.
                    //List<urakawa.command.Command> silence = new List<urakawa.command.Command>();
                    bool? deleteSilence = null;
                    bool phrasesDeleted = false;
                    mPresentation.RootNode.AcceptDepthFirst(
                        delegate(urakawa.core.TreeNode node)
                        {
                            if (node is ObiNode && !((ObiNode)node).Used)
                            {
                                
                                if (node is PhraseNode && ((PhraseNode)node).Role_ == EmptyNode.Role.Silence )
                                {
                                    if (deleteSilence == null)
                                    {
                                        if (MessageBox.Show(Localizer.Message("delete_silence_phrases"),
                            Localizer.Message("delete_silence_phrases_caption"), MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            deleteSilence = true;
                                        }
                                        else
                                        {
                                            deleteSilence = false;
                                        }
                                    } // one time deleteSilence flag assignment done
                                    if (deleteSilence == false) return true;
                                } // silence stuff ends

                                phrasesDeleted = true;
                                Commands.Node.Delete delete = new Commands.Node.Delete(this, (ObiNode)node, false);
                                if (Selection == null || Selection.Node == node) delete.UpdateSelection = true; // temp fix, if selection is null, delete updates afterDeleteSelection to null to avoid selecting something through some event.
                                    command.ChildCommands.Insert(0, delete);
                                    return false;
                                } //unused obi node check ends
                                
                            return true;
                        }, delegate(urakawa.core.TreeNode node) { }
                    );
                    //if (silence.Count > 0)
                    //{
                        //if (MessageBox.Show(Localizer.Message("delete_silence_phrases"),
                            //Localizer.Message("delete_silence_phrases_caption"), MessageBoxButtons.YesNo,
                            //MessageBoxIcon.Question) == DialogResult.Yes)
                        //{
                            //foreach (urakawa.command.Command c in silence) command.ChildCommands.Insert(command.ChildCommands.Count, c);
                        //}
                    //}

                    //if (Selection != null && mTOCView.ContainsFocus) Selection = null;
                    if (command.ChildCommands.Count > 0) mPresentation.Do(command);
                    if (sectionPosition > -1)//@singleSection
                    {
                        SectionNode section = null;
                        int totalSections = ((ObiRootNode)mPresentation.RootNode).SectionChildCount;
                        if (sectionPosition < totalSections)
                        {
                            section = ((ObiRootNode)mPresentation.RootNode).SectionChild(sectionPosition);
                        }
                        else if (totalSections > 0)
                        {
                            section = ((ObiRootNode)mPresentation.RootNode).SectionChild(totalSections - 1);
                        }
                        if (section != null)
                        {
                            mContentView.CreateStripForSelectedSection(section, true);
                            Selection = new NodeSelection(section, mContentView);
                        }
                    }
                    MessageBox.Show(phrasesDeleted? Localizer.Message("UnusedPhrasesDeletedFromProject"): Localizer.Message("DeleteUnused_NoUnusedPhraseFoundToDelete"), 
                        Localizer.Message("UnusedPhrasesDeletedCaption"), MessageBoxButtons.OK);
                }
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
                    /*
                    Strip currentlyActiveStrip = mContentView.ActiveStrip;

                    if (GetSelectedPhraseSection != null )//@singleSection
                        {
                        if ( currentlyActiveStrip != null   
                        &&   GetSelectedPhraseSection != currentlyActiveStrip.Node && mContentView.RestrictDynamicLoadingForRecording ( currentlyActiveStrip.Node ))
                            {
                            MessageBox.Show ( Localizer.Message ("RecordingRestriction_CannotCreateStrip") ,Localizer.Message("Caption_Information") , MessageBoxButtons.OK  );
                            currentlyActiveStrip.Focus ();
                            }
                        else if (GetSelectedPhraseSection != null && (!TransportBar.IsRecorderActive || TransportBar.RecordingSection == GetSelectedPhraseSection))
                            {
                            mContentView.CreateStripForSelectedSection ( GetSelectedPhraseSection, true );
                            }
                        else if ( TransportBar.IsRecorderActive ) 
                            {
                            MessageBox.Show ( Localizer.Message ( "RecordingRestriction_CannotCreateStrip" ), Localizer.Message ( "Caption_Information" ), MessageBoxButtons.OK );
                            return;
                            }
                        }
                    */
                    //following function called for replacement of above commented code
                        if (!ShowSelectedSectionContents()) return;

                    if (TransportBar.IsPlayerActive)
                        {
                        
                        //Selection = new NodeSelection ( TransportBar.CurrentPlaylist.CurrentPhrase, mContentView );//@singleSection: new
                        mContentView.SelectPhraseBlockOrStrip ( TransportBar.PlaybackPhrase );
                        SetPlaybackBlockIfRequired ();
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

        public bool SetPlaybackBlockIfRequired ()
            {
            if (TransportBar.IsPlayerActive
                        && (mContentView.PlaybackBlock == null || mContentView.PlaybackBlock.Node != TransportBar.PlaybackPhrase))
                {
                this.SetPlaybackPhraseAndTime ( TransportBar.PlaybackPhrase, TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                this.UpdateCursorPosition ( TransportBar.CurrentPlaylist.CurrentTimeInAsset );
                return true;
                }
            return false;
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
                SectionNode section = (SectionNode)Selection.Node;
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild ( 0 );
                if ((section.PhraseChildCount + next.PhraseChildCount) > MaxVisibleBlocksCount) // @phraseLimit
                    {
                    MessageBox.Show ( Localizer.Message ( "Operation_Cancelled" ) + "\n" + string.Format ( Localizer.Message ( "ContentsHidden_PhrasesExceedMaxLimitPerSection" ), MaxVisibleBlocksCount ) );
                    return;
                    }
                    if (Selection != null && Selection is TextSelection) Selection = new NodeSelection(Selection.Node, Selection.Control);
                try
                    {
                    //mPresentation.Do ( mContentView.MergeSelectedStripWithNextCommand () );
                    mPresentation.Do ( MergeSelectedStripWithNextCommand () );
                    if (mSelection != null && mSelection.Node is SectionNode) UpdateBlocksLabelInStrip ( (SectionNode)mSelection.Node );
                    }
                catch (System.Exception ex)
                    {
                    this.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ProjectViewFormMsg_MergeOperationFail") + "\n\n" + ex.ToString());  //@Messagecorrected
                    }

                // hide newly made phrases visible if the strip has its contents hidden
                //HideNewPhrasesInInvisibleSection ( section );//@singleSection: original
                //mContentView.CreateBlocksInStrip (); //@singleSection: new statement
                }
            }

        //@singleSection: moved from contentview to enable merge enable in TOC view
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
                command = Presentation.CommandFactory .CreateCompositeCommand ();
                command.ShortDescription = Localizer.Message ( "merge_sections" );
                SectionNode section = (SectionNode)Selection.Node;
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection ( this, new NodeSelection ( section, Selection.Control ) ) );
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild ( 0 );

                int progressInterval = next.PhraseChildCount> 90? (next.PhraseChildCount * 2 / 90) : 1 ; // interval multiplied by 2 as we want to report with increment of 2
                for (int i = 0; i < next.PhraseChildCount; ++i)
                    {
                    EmptyNode newPhraseNode = (EmptyNode)next.PhraseChild ( i ).Copy ( false, true );
                    EmptyNode existingNode = ((EmptyNode)next.PhraseChild(i));
                    if(existingNode.Role_ == EmptyNode.Role.Anchor)  newPhraseNode.AssociatedNode = existingNode.AssociatedNode;
                    
                    if (newPhraseNode.Role_ == EmptyNode.Role.Heading) newPhraseNode.Role_ = EmptyNode.Role.Plain;
                    if (!section.Used && newPhraseNode.Used) newPhraseNode.Used = section.Used;
                    
                    Commands.Command add = new Commands.Node.AddNode ( this, newPhraseNode, section, section.PhraseChildCount + i, false );
                    
                    if (i % progressInterval == 0) add.ProgressPercentage = (i * 90 / next.PhraseChildCount); 
                    command.ChildCommands.Insert(command.ChildCommands.Count,add );
                    
                    if (existingNode.Role_ == EmptyNode.Role.Custom && EmptyNode.SkippableNamesList.Contains(existingNode.CustomRole)
                        && existingNode.PrecedingNode != null && existingNode.PrecedingNode is EmptyNode && ((EmptyNode)existingNode.PrecedingNode).CustomRole != existingNode.CustomRole)
                    {
                        EmptyNode anchorNode = mPresentation.GetAnchorForReferencedNode(existingNode);
                        if (anchorNode != null)
                        {
                            Commands.Node.DeAssociateAnchorNode disassociateCmd = new Obi.Commands.Node.DeAssociateAnchorNode(this, anchorNode);
                            Commands.Node.AssociateAnchorNode associateCmd = new Obi.Commands.Node.AssociateAnchorNode(this, anchorNode, newPhraseNode);
                            command.ChildCommands.Insert(command.ChildCommands.Count, disassociateCmd);
                            command.ChildCommands.Insert(command.ChildCommands.Count, associateCmd);
                        }
                    }


                    }
                Console.WriteLine ( "add in merge complete" );
                //command.ChildCommands.Insert(command.ChildCommands.Count, mContentView.DeleteStripCommand ( next ) );
                // add shallow delete command
                Commands.Node.Delete delete = new Commands.Node.Delete ( this, next, Localizer.Message ( "delete_section_shallow" ) );
                if (next.SectionChildCount > 0)
                    {
                    CompositeCommand deleteCommand = this.Presentation.CommandFactory.CreateCompositeCommand ();
                    command.ShortDescription =  delete.ShortDescription;
                    for (int i = next.SectionChildCount-1 ; i >= 0 ; --i)
                    {
                        //deleteCommand.ChildCommands.Insert(deleteCommand.ChildCommands.Count, new Commands.TOC.MoveSectionOut ( this, next.SectionChild ( i ) ) );
                        // above line replaced by following 5 lines.
                        Commands.Node.Delete deleteChildSection = new Obi.Commands.Node.Delete(this, next.SectionChild(i), false);
                        deleteCommand.ChildCommands.Insert(deleteCommand.ChildCommands.Count, deleteChildSection);
                        int insertIndex = next.Index < section.SectionChildCount? next.Index : section.SectionChildCount ;
                        Commands.Node.AddNode insert = new Obi.Commands.Node.AddNode(this, next.SectionChild(i), section, insertIndex);
                        deleteCommand.ChildCommands.Insert(deleteCommand.ChildCommands.Count, insert);
                    }
                    delete.ProgressPercentage = 100;
                    deleteCommand.ChildCommands.Insert(deleteCommand.ChildCommands.Count, delete );
                    command.ChildCommands.Insert(command.ChildCommands.Count, deleteCommand );//
                    }
                else
                    {
                    delete.ProgressPercentage = 100;
                    command.ChildCommands.Insert(command.ChildCommands.Count, delete );
                    }
                }
            return command;
            }

        public void MergeMultipleSections()
        {
            if (GetSelectedPhraseSection != null)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                List<SectionNode> listOfSections = ((Obi.ObiRootNode)mPresentation.RootNode).GetListOfAllSections(); //use this list in merge section dialog
                //MessageBox.Show(listOfSections.Count.ToString()); 
                int selectedSectionIndex = listOfSections.IndexOf(GetSelectedPhraseSection);
                if (selectedSectionIndex > 0) listOfSections.RemoveRange(0, selectedSectionIndex);
                //foreach (SectionNode s in listOfSections) MessageBox.Show(s.Label + " " + s.Level.ToString ());
                Obi.Dialogs.SelectMergeSectionRange selectionDialog = new Obi.Dialogs.SelectMergeSectionRange(listOfSections, selectedSectionIndex);
                if (selectionDialog.ShowDialog () == DialogResult.OK
                    && selectionDialog.SelectedSections != null && selectionDialog.SelectedSections.Count > 1)
                {
                    List<SectionNode> selectedSections = selectionDialog.SelectedSections;
                    if (selectedSections.Count <= 1) return;
                    urakawa.command.CompositeCommand mergeSectionCommand = mPresentation.CreateCompositeCommand("MergeMultipleSections");

                    SectionNode firstSection = selectedSections[0];
                    selectedSections.Remove(firstSection);
                    //first arrange the children whose parents will be deleted
                    int lastSelectedSectionIndex = listOfSections.IndexOf(selectedSections[selectedSections.Count - 1]);

                    if (lastSelectedSectionIndex < listOfSections.Count - 1)
                    {
                        for (int i = lastSelectedSectionIndex + 1; i < listOfSections.Count; i++)
                        {
                            if (selectedSections.Contains(listOfSections[i].ParentAs<SectionNode>()))
                            {
                                mergeSectionCommand.ChildCommands.Insert(mergeSectionCommand.ChildCommands.Count, new Commands.Node.Delete(this, listOfSections[i]));
                                mergeSectionCommand.ChildCommands.Insert(mergeSectionCommand.ChildCommands.Count, new Commands.Node.AddNode(this, listOfSections[i], firstSection, firstSection.SectionChildCount, false));
                            }
                        }
                    }

                    List<EmptyNode> phraseList = new List<EmptyNode>();

                    //for (int i = 0; i < selectedSections.Count; i++)
                    for (int i = selectedSections.Count-1 ; i >= 0; i--)
                    {
                        for (int j = selectedSections[i].PhraseChildCount-1 ; j >= 0 ; j--)
                        {
                            phraseList.Insert (0,selectedSections[i].PhraseChild(j));
                            mergeSectionCommand.ChildCommands.Insert(mergeSectionCommand.ChildCommands.Count, new Commands.Node.Delete(this, selectedSections[i].PhraseChild(j)));
                        }
                    }
                    for (int i = selectedSections.Count - 1; i >= 0; i--)
                    {
                        if (!selectedSections.Contains(selectedSections[i].ParentAs<SectionNode>()))
                            mergeSectionCommand.ChildCommands.Insert(mergeSectionCommand.ChildCommands.Count, new Commands.Node.Delete(this, selectedSections[i]));
                    }

                    for (int i = 0; i < phraseList.Count; i++)
                    {
                        Commands.Command add = new Commands.Node.AddNode(this, phraseList[i], firstSection, firstSection.PhraseChildCount + i, false);
                        mergeSectionCommand.ChildCommands.Insert(mergeSectionCommand.ChildCommands.Count, add);
                    }

                    if (mergeSectionCommand.ChildCommands.Count > 0) mPresentation.Do(mergeSectionCommand);
                }

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

                if (mClipboard is AudioClipboard )
                {
                    if(TransportBar.CurrentState == TransportBar.State.Playing ) TransportBar.Pause();
                }
                else if (mTransportBar.IsPlayerActive)
                {
                    mTransportBar.Stop();
                }
                bool PlaySelectionFlagStatus = TransportBar.SelectionChangedPlaybackEnabled;
                mTransportBar.SelectionChangedPlaybackEnabled = false;
                if (CanPasteSpecialNode())    //@AssociateNode
                {
                    try
                    {
                        mPresentation.Do(mSelection.PasteCommand(this, true));
                    }
                    catch (System.Exception ex)
                    {
                        this.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ProjectViewFormMsg_PasteOperationFail") + "\n\n" + ex.ToString());  //@Messagecorrected
                    }
                }
                else
                {
                    try
                    {
                        mPresentation.Do(mSelection.PasteCommand(this, false));
                    }
                    catch (System.Exception ex)
                    {
                        this.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ProjectViewFormMsg_PasteOperationFail") + "\n\n" + ex.ToString());   //@Messagecorrected
                    }
                }
                mTransportBar.SelectionChangedPlaybackEnabled = PlaySelectionFlagStatus;
                }
            }

        public bool CanPasteSpecialNode()   //@AssociateNode
        {
            EmptyNode stripIndexNode = (EmptyNode)Selection.EmptyNodeForSelection;
            bool m_CanPasteSpecialNode = false;
            if ((Selection is StripIndexSelection && stripIndexNode != null && ((EmptyNode)stripIndexNode.FollowingNode) != null && ((EmptyNode)stripIndexNode.FollowingNode)  is EmptyNode && stripIndexNode.CustomRole == ((EmptyNode)stripIndexNode.FollowingNode).CustomRole && ((EmptyNode)stripIndexNode.FollowingNode).CustomRole != ((EmptyNode)mClipboard.Node).CustomRole && stripIndexNode.CustomRole != ((EmptyNode)mClipboard.Node).CustomRole)
                || (Selection.Node is PhraseNode && ((EmptyNode)this.Selection.Node.FollowingNode) != null && ((EmptyNode)this.Selection.Node.FollowingNode).CustomRole == ((EmptyNode)this.Selection.Node).CustomRole && ((EmptyNode)this.Selection.Node.FollowingNode).CustomRole != ((EmptyNode)mClipboard.Node).CustomRole && ((EmptyNode)this.Selection.Node).CustomRole != ((EmptyNode)mClipboard.Node).CustomRole))
            {
                if (((EmptyNode)mClipboard.Node).Role_ == EmptyNode.Role.Plain)
                    m_CanPasteSpecialNode = true;
                else
                    m_CanPasteSpecialNode = false;
            }            
            return m_CanPasteSpecialNode;
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
        public ObiPresentation Presentation
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
                        mTOCView.SetNewPresentation();
                        mContentView.NewPresentation();
                        mTransportBar.NewPresentation();
                        mMetadataView.NewPresentation();
                        if (mContentView.ActiveStrip != null && this.Selection == null) mTOCView.HighlightNodeWithoutSelection = mContentView.ActiveStrip.Node;

                        mPresentation.AddCustomClass(EmptyNode.Footnote, null);   //@AssociateNode
                        mPresentation.AddCustomClass(EmptyNode.Sidebar, null);    //@AssociateNode
                        mPresentation.AddCustomClass(EmptyNode.ProducerNote, null);      //@AssociateNode
                        mPresentation.AddCustomClass(EmptyNode.Note, null);           //@AssociateNode 
                        mPresentation.AddCustomClass(EmptyNode.Annotation, null);         //@AssociateNode   
                        mPresentation.AddCustomClass(EmptyNode.EndNote, null);           //@AssociateNode
                    }
                    else
                    {
                        mContentView.BeginSpecialNode = null;            //@AssociateNode
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

        
        public void UpdateBlockForFindNavigation(EmptyNode node, bool IsFineNavigation)
        {
           mContentView.UpdateBlockForFindNavigation(node, IsFineNavigation);
        }

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
            get 
            {
                
                return mSelection; 
            }
            set
                {
                    if (m_DisableSectionSelection && value != null )
                    {
                        m_DisableSectionSelection = false;
                        //bypass selecting only if selection is in same section, allow it select section if different section is selected
                        if ( value.Node is SectionNode && GetSelectedPhraseSection != null && GetSelectedPhraseSection == value.Node )  return;
                    }
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

                        if (mSelection.Control == mContentView && !(mSelection is TextSelection)  && GetSelectedPhraseSection != null) mTOCView.HighlightNodeWithoutSelection = GetSelectedPhraseSection;
                        mSelection.Control.Selection = value;
                        }
                    if (SelectionChanged != null) SelectionChanged ( this, new EventArgs () );
                    }
                }
            }

        public void DisableSectionSelection()
        {
            m_DisableSectionSelection = true;
        }
        public void EnableSectionSelection()
        {
            m_DisableSectionSelection = false;
        }

        /// <summary>
        /// Set a page number on the selected phrase.
        /// </summary>
        public void SetPageNumberOnSelection ()
            {
            if (CanSetPageNumber)
                {
                if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause ();
                if (mTransportBar.IsRecorderActive) mTransportBar.Stop();
                Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber ( CurrentOrNextPageNumber, true, false );
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
                mSelection.Node.AcceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {
                        if (n is ObiNode && ((ObiNode)n).Used != used)
                            {
                            if (n is PhraseNode && ((PhraseNode)n).Role_ == EmptyNode.Role.Heading)
                                {
                                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AssignRole ( this, (PhraseNode)n, EmptyNode.Role.Plain ) );
                                //command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.UnsetNodeAsHeadingPhrase(this, (PhraseNode)n));
                                }
                                if (n is EmptyNode && !((EmptyNode)n).Used && ((EmptyNode)n).Role_ == EmptyNode.Role.Silence) return true;
                            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed ( this, (ObiNode)n ) );
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
        //@singleSection
        private bool PauseAndCreatePlayingSection()
        {
            if (mTransportBar.IsPlayerActive)
            {
                mTransportBar.Pause();
                Strip currentlyActiveStrip = mContentView.ActiveStrip;
                if (currentlyActiveStrip != null && TransportBar.PlaybackPhrase != null
                    && currentlyActiveStrip.Node == TransportBar.PlaybackPhrase.ParentAs<SectionNode>())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Split a strip at the selected position.
        /// </summary>
        public void SplitStrip ()
            {
            if (CanSplitStrip)
                {
                    if (mTransportBar.IsPlayerActive && !PauseAndCreatePlayingSection()) return;
                    
                bool PlayOnSelectionStatus = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;
                SectionNode OriginalSectionNode = null;
                if (mSelection != null && mSelection.Node is EmptyNode) OriginalSectionNode = mSelection.Node.ParentAs<SectionNode> ();
                try
                    {
                    mPresentation.Do ( mContentView.SplitStripCommand () );
                    }
                catch (System.Exception ex)
                    {
                    this.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ProjectViewFormMsg_SplitOperationFail") + "\n\n" + ex.ToString());   //@Messagecorrected
                    }
                
                if (OriginalSectionNode != null) UpdateBlocksLabelInStrip ( OriginalSectionNode );

                TransportBar.SelectionChangedPlaybackEnabled = PlayOnSelectionStatus;
                }
            }

        //@singleSection
        public void DeleteFollowingPhrasesInSection ()
            {
            if (CanDeleteFollowingPhrasesInSection)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();

                if (Selection != null && Selection.Node is SectionNode && !(Selection is StripIndexSelection)
                    && MessageBox.Show(Localizer.Message("DeleteFollowing_SectionSelection"), Localizer.Message("Caption_Warning"), MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                bool PlayOnSelectionStatus = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;
                
                SectionNode section = Selection.Node is SectionNode ? (SectionNode)Selection.Node : ((EmptyNode)Selection.Node).ParentAs<SectionNode> ();
                int startNodeIndex = Selection is StripIndexSelection && ((StripIndexSelection)Selection).EmptyNodeForSelection != null ? ((StripIndexSelection)Selection).EmptyNodeForSelection.Index :
                    Selection.Node is SectionNode && section.PhraseChildCount > 0 && !(Selection is StripIndexSelection)? 0:
                    Selection.Node is EmptyNode ? Selection.Node.Index + 1:
                    -1;

                EmptyNode startNode = null;
                if (startNodeIndex >= 0 && startNodeIndex < section.PhraseChildCount)
                    {
                    startNode = section.PhraseChild ( startNodeIndex );
                    }
                else
                    {
                    return;
                    }

                try
                    {
                    mPresentation.Do ( GetDeleteRangeOfPhrasesInSectionCommand ( section, startNode, section.PhraseChild ( section.PhraseChildCount - 1 ) ) );
                    
                    }
                catch (System.Exception ex)
                    {
                    this.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ProjectViewFormMsg_DeleteOperationFail") + "\n\n" + ex.ToString()); //@Messagecorrected
                    }
                TransportBar.SelectionChangedPlaybackEnabled = PlayOnSelectionStatus;
                }
            }

        //@singleSection
        public urakawa.command.Command GetDeleteRangeOfPhrasesInSectionCommand ( SectionNode section, EmptyNode startNode, EmptyNode endNode )
            {
            CompositeCommand command = mPresentation.CreateCompositeCommand ( Localizer.Message ( "Delete_RangeOfPhrases" ) );
            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection ( this,Selection ) );

            int startIndex = startNode.Index;
            int endIndex = endNode.Index < section.PhraseChildCount ? endNode.Index : section.PhraseChildCount - 1;

            ObiNode updateSelectionNode = null;
            if (endIndex < section.PhraseChildCount - 1)
                {
                updateSelectionNode = section.PhraseChild ( endIndex + 1 );
                }
            else if (startIndex > 0)
                {
                updateSelectionNode = section.PhraseChild ( startIndex - 1 );
                }
            else
                {
                updateSelectionNode = section;
                }
            if ( updateSelectionNode != null ) 
            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection (this, new NodeSelection (updateSelectionNode, Selection.Control )) );

            int progressInterval = (endIndex - startIndex) > 100 ? (endIndex - startIndex) * 2 / 100 : 1*2; // multiplied by 2 to increment progress by 2
            int progressPercent = 0;
            for (int i = endIndex; i >= startIndex; i--)
                {
                Commands.Node.Delete deleteCommand = new Obi.Commands.Node.Delete ( this, section.PhraseChild ( i ), false );
                if (i == startIndex || progressPercent > 98) progressPercent = 98;
                if ((i - startIndex) % progressInterval == 0) deleteCommand.ProgressPercentage = progressPercent += 2;
                command.ChildCommands.Insert(command.ChildCommands.Count, deleteCommand );
                }
             
            
            return command;
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


        //public void UpdateTOCBackColorForEmptySection(SectionNode n)
        //{
        //    if (n != null && ObiForm.Settings != null)
        //    {
        //        mTOCView.UpdateTOCBackColorForEmptySection((SectionNode)n);

        //    }


        //}
        /// <summary>
        /// Get the transport bar for this project view.
        /// </summary>
        public TransportBar TransportBar { get { return mTransportBar; } }



        // Execute a command, but first add some extra stuff to maintain the unusedness of the new node
        // depending on the unusedness of its parent.
        private void AddUnusedAndExecute ( urakawa.command.Command command, ObiNode node, ObiNode parent )
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
                    c = mPresentation.CreateCompositeCommand ( command.ShortDescription);
                    c.ChildCommands.Insert(c.ChildCommands.Count, command );
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

        public bool CanShowPhrasePropertiesDialog { get { return Selection != null && Selection.Node is EmptyNode && !TransportBar.IsRecorderActive; } }
        public bool CanShowProjectPropertiesDialog { get { return mPresentation != null && !mTransportBar.IsRecorderActive; } }
        public bool CanShowSectionPropertiesDialog { get { return Selection != null && Selection.Node is SectionNode && Presentation != null && Presentation.FirstSection != null && !TransportBar.IsRecorderActive; } } // quick fix of inserting first section check to avoid a crash

        public bool CanMarkSectionUnused { get { return mTOCView.CanSetSectionUsedStatus && mSelection.Node.Used; } }
        public bool CanMarkStripUnused { get { return mContentView.CanSetStripUsedStatus && mSelection.Node.Used; } }
        public bool CanMergeBlockWithNext { get { return mContentView.CanMergeBlockWithNext; } }
        public bool CanMergePhraseWithFollowingPhrasesInSection { get { return CanMergeBlockWithNext && !IsZoomWaveformActive; } }
        public bool CanSplitPhrase { get { return mTransportBar.CanSplitPhrase; } }

        public bool IsBlockUsed { get { return mContentView.IsBlockUsed; } }
        public bool IsStripUsed { get { return mContentView.IsStripUsed; } }

        public bool CanMergeWithPhrasesBeforeInSection 
            { 
            get 
                { 
                EmptyNode node   =  mContentView.PlaybackBlock != null? mContentView.PlaybackBlock.Node : 
                    Selection != null && Selection.Node is EmptyNode ? (EmptyNode) Selection.Node : null ;
                return node != null && node.IsRooted && node.Index > 0 && !TransportBar.IsRecorderActive && !IsZoomWaveformActive;
                }
            }

        /// <summary>
        /// True when there is a block that is selected that is TODO, or a block playing back that is TODO.
        /// </summary>
        public bool IsCurrentBlockTODO
            {
            get
                {
                return mTransportBar.PlaybackPhrase != null ?
                    mTransportBar.PlaybackPhrase.TODO :
                    Selection != null && Selection.Node is EmptyNode?
                   ((EmptyNode) Selection.Node).TODO : false;
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

        public bool CanFindFirstTime { get
        {
            return mPresentation != null && ((ObiRootNode)mPresentation.RootNode).SectionChildCount > 0;
        } }
        public void FindInText ()
            {
            //show the form if it's not already shown
            if (mFindInTextSplitter.Panel2Collapsed == true) mFindInTextSplitter.Panel2Collapsed = false;
            FindInTextVisible = true;
            //iterating over the layout panel seems to be the way to search the sections 
            if (mContentView.ContainsFocus)
                {
                    if (IsZoomWaveformActive)
                    {
                        mContentView.ZoomPanelClose();
                    }
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

                if (this.ObiForm.CheckDiskSpace() <= 100)
                {
                    DialogResult result = MessageBox.Show(string.Format(Localizer.Message("LimitedDiskSpaceWarning"), 100), Localizer.Message("Memory_Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                NodeSelection initialSelection = this.Selection;
                string [] filesPathArray = SelectFilesToImport ();
                if (this.Selection != initialSelection) this.Selection = initialSelection;//workaround to prevent ocasional case of shifting selection from strip cursor to section
                

                if (filesPathArray != null)
                    {
                        long threshold = (long) ObiForm.Settings.DefaultThreshold;
                      double gap = (double) ObiForm.Settings.DefaultGap;
                        double leadingSilence = (double) ObiForm.Settings.DefaultLeadingSilence;

                    Dialogs.ImportFileSplitSize dialog =
                        new Dialogs.ImportFileSplitSize ( ObiForm.Settings , filesPathArray);
                    
                    if (dialog.ShowDialog () == DialogResult.OK)
                        {
                            
                            if (dialog.ApplyPhraseDetection)
                            {
                                Dialogs.SentenceDetection sentenceDetection = new Obi.Dialogs.SentenceDetection(threshold,gap,leadingSilence);
                                if (sentenceDetection.ShowDialog() == DialogResult.OK)
                                {
                                    threshold = sentenceDetection.Threshold;
                                    gap = sentenceDetection.Gap;
                                    leadingSilence = sentenceDetection.LeadingSilence;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        // if focus is in toc view, shift it to content view
                            if (Selection.Control == null || Selection.Control is TOCView) Selection = new NodeSelection(Selection.Node, mContentView);  

                            filesPathArray = dialog.FilesPaths;
                            Dialogs.ProgressDialog progress_AudioConverter = new Obi.Dialogs.ProgressDialog(Localizer.Message("AudioFileImport_ProcessingFiles"),
                        delegate(Dialogs.ProgressDialog progress1)
                        {
                            filesPathArray = Audio.AudioFormatConverter.ConvertFiles(filesPathArray, mPresentation);
                        });
                            progress_AudioConverter.OperationCancelled += new Obi.Dialogs.OperationCancelledHandler(delegate(object sender, EventArgs e) { Audio.AudioFormatConverter.IsRequestCancellation = true; });
                            progress_AudioConverter.ShowDialog();
                            if (progress_AudioConverter.Exception != null) throw progress_AudioConverter.Exception;

                            List<string> paths = new List<string>();
                            for (int i = 0; i < filesPathArray.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(filesPathArray[i]) && System.IO.File.Exists(filesPathArray[i])) paths.Add(filesPathArray[i]);
                            }

                        ObiForm.Settings.MaxPhraseDurationMinutes = dialog.MaxPhraseDurationMinutes;
                        ObiForm.Settings.SplitPhrasesOnImport = dialog.SplitPhrases;
                        bool createSectionForEachPhrase = dialog.createSectionForEachPhrase;
                         // convert from minutes to milliseconds
                        double durationMs = dialog.SplitPhrases ? dialog.MaxPhraseDurationMinutes * 60000.0 : (ObiForm.Settings.MaxAllowedPhraseDurationInMinutes * 60000.0);
                       // double durationMs = dialog.SplitPhrases ? dialog.MaxPhraseDurationMinutes * 60000.0 : 0.0;

                        // to do: add chedk box in dialog and use a flag for using the following sort
                        //if ( dialog.SortFileNamesAscending )  paths.Sort();
                        List<PhraseNode> phraseNodes = new List<PhraseNode> ( paths.Count );
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
                                            if (!dialog.SplitPhrases  && phrases.Count > 1)
                                                MessageBox.Show(String.Format(Localizer.Message("Import_Phrase_SizeLimit"), ObiForm.Settings.MaxAllowedPhraseDurationInMinutes));                                                                                                                 
                                            }
                                        catch (System.Exception ex)
                                            {
                                            this.WriteToLogFile(ex.ToString());
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
                                            catch (Exception ex)
                                                {
                                                this.WriteToLogFile(ex.ToString());
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
                                bool selectionChangePlaybackEnabled = TransportBar.SelectionChangedPlaybackEnabled;
                                TransportBar.SelectionChangedPlaybackEnabled = false;
                                try
                                {
                                    if (createSectionForEachPhrase)
                                    {
                                        CompositeCommand createSectionsCommand = GetImportSectionsFromAudioCommands(phraseNodes, phrase_SectionNameMap, dialog.CharacterCountToTruncateFromStart, dialog.CharactersToBeReplacedWithSpaces, dialog.PageIdentificationString);

                                        mPresentation.Do(createSectionsCommand);
                                    }
                                    else
                                    {
                                        mPresentation.Do(GetImportPhraseCommands(phraseNodes));
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
//phrase detection
                                    
                                    if (dialog.ApplyPhraseDetection )
                                    {
                                        ApplyPhraseDetectionOnPhraseList(phraseNodes, threshold, gap, leadingSilence);
                                    }
                                    TransportBar.SelectionChangedPlaybackEnabled = selectionChangePlaybackEnabled;
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

        private void ApplyPhraseDetectionOnPhraseList(List<PhraseNode> phraseNodes, long threshold, double gap, double before)
        {

            urakawa.command.CompositeCommand phraseDetectionCommand = null;
            try
            {
                if (IsZoomWaveformActive) mContentView.RemovePanel();
            Dialogs.ProgressDialog progress = new Dialogs.ProgressDialog ( Localizer.Message ( "phrase_detection_progress" ),
                        delegate(Dialogs.ProgressDialog progress1)
                            {
                                phraseDetectionCommand = Commands.Node.SplitAudio.GetPhraseDetectionCommand(this, phraseNodes, threshold, gap, before, ObiForm.Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection);
        });
            progress.OperationCancelled += new Obi.Dialogs.OperationCancelledHandler(delegate(object sender, EventArgs e) { Audio.PhraseDetection.CancelOperation = true; });
            this.ProgressChanged += new ProgressChangedEventHandler(progress.UpdateProgressBar);

            progress.ShowDialog();
            if (progress.Exception != null) throw (progress.Exception);

            
                if (phraseDetectionCommand != null )  mPresentation.Do(phraseDetectionCommand);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        // Create a command to import phrases
        private CompositeCommand GetImportPhraseCommands(List<PhraseNode> phraseNodes)
            {
            CompositeCommand command = Presentation.CreateCompositeCommand ( Localizer.Message ( "import_phrases" ) );
            ObiNode parent;
            int index;
            if (mContentView.Selection.Node is SectionNode)
                {
                // Import into a section, at 0 or at the selected index
                parent = mContentView.Selection.Node;
                index = mContentView.Selection is StripIndexSelection ?
                    ((StripIndexSelection)mContentView.Selection).Index : parent.PhraseChildCount; //changed on 26 March 2013 for Obi 2.6-beta: when section is selected phrases are appended at the end instead of 0 position, just as recording
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
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode ( this, phraseNodes[i], parent, index + i ) );
                }
            if (mContentView.Selection.Node.GetType () == typeof ( EmptyNode ))
                {
                // If the selection was an empty node, then remove (so that the first phrase is "imported into it".)
                // Remember to keep its attributes so that if we import audio into a page, it keeps its page number.
                Commands.Node.MergeAudio.AppendCopyNodeAttributes ( command, this, (EmptyNode)mContentView.Selection.Node,
                    phraseNodes[0] );
                Commands.Node.Delete delete = new Commands.Node.Delete ( this, mContentView.Selection.Node );
                delete.UpdateSelection = false;
                command.ChildCommands.Insert(command.ChildCommands.Count, delete );
                }
            return command;
            }

        private CompositeCommand GetImportSectionsFromAudioCommands(List<PhraseNode> phraseNodesList, Dictionary<PhraseNode, string> phrase_SectionNameMap, int CharacterCountToTruncateFromStart ,string  CharactersToBeReplacedWithSpaces, string PageIdentificationString)
            {
                List<PhraseNode> phraseNodes = new List<PhraseNode>();
                phraseNodes.AddRange(phraseNodesList);
            CompositeCommand command = Presentation.CreateCompositeCommand ( Localizer.Message ( "import_phrases" ) );
            CharacterCountToTruncateFromStart++; //fix for loss of one character count
            if (Selection != null) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(this, new NodeSelection(Selection.Node, Selection.Control)));
            SectionNode newSectionNode = null;
            int phraseInsertIndex = 0;
            SectionNode firstSection = null;

            m_DisableSectionSelection = false;
            if (GetSelectedPhraseSection != null && (Selection.Node is EmptyNode || Selection is StripIndexSelection)) Selection = new NodeSelection(GetSelectedPhraseSection, mContentView);

            if (Selection != null && Selection.Node is SectionNode
                && ((SectionNode)Selection.Node).PhraseChildCount == 0 && phraseNodes.Count > 0)
            {
                newSectionNode = (SectionNode)Selection.Node;
                firstSection = newSectionNode;
                phraseInsertIndex = 0;
                string sectionName = phrase_SectionNameMap[phraseNodes[0]];
                if ( CharacterCountToTruncateFromStart > 0  && CharacterCountToTruncateFromStart  < sectionName.Length) sectionName = sectionName.Substring (CharacterCountToTruncateFromStart - 1, sectionName.Length - CharacterCountToTruncateFromStart + 1) ;
                if (!string.IsNullOrEmpty(CharactersToBeReplacedWithSpaces))
                {
                    sectionName = sectionName.Replace(CharactersToBeReplacedWithSpaces, " ");
                    sectionName = sectionName.TrimStart();
                }
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.RenameSection(this, newSectionNode, sectionName));
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(this, phraseNodes[0], newSectionNode, phraseInsertIndex));
                phraseNodes.RemoveAt(0);

                
for (int j = 0;
                        j < phraseNodes.Count && !phrase_SectionNameMap.ContainsKey(phraseNodes[j]);
                        j++)
                {
                    phraseInsertIndex++;
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(this, phraseNodes[j], newSectionNode, phraseInsertIndex));
                }
            }
            List<EmptyNode> pagePhrases = new List<EmptyNode>();
            for (int i = phraseNodes.Count-1; i >= 0; i--)
                {
                    
                if (phrase_SectionNameMap.ContainsKey ( phraseNodes[i] ))
                {


                    string newSectionName = phrase_SectionNameMap[phraseNodes[i]];
                    //MessageBox.Show(newSectionName);
                    if (CharacterCountToTruncateFromStart > 0 && CharacterCountToTruncateFromStart < newSectionName.Length) newSectionName = newSectionName.Substring(CharacterCountToTruncateFromStart - 1, newSectionName.Length - CharacterCountToTruncateFromStart + 1);
                    if (!string.IsNullOrEmpty(CharactersToBeReplacedWithSpaces))
                    {
                        newSectionName = newSectionName.Replace(CharactersToBeReplacedWithSpaces, " ");
                        newSectionName = newSectionName.TrimStart();
                    }
                    
                    if (PageIdentificationString == null ||  !newSectionName.StartsWith(PageIdentificationString))
                    {
                        // create a new section and add phrase to it
                        Commands.Command addSectionCmd = new Commands.Node.AddSectionNode(this, mTOCView, null);
                        addSectionCmd.UpdateSelection = true;
                        command.ChildCommands.Insert(command.ChildCommands.Count, addSectionCmd);
                        newSectionNode = ((Commands.Node.AddSectionNode)addSectionCmd).NewSection;

                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.RenameSection(this, newSectionNode, newSectionName));
                    }
                    else
                    {
                        string pageNumberString = newSectionName.Substring(PageIdentificationString.Length, newSectionName.Length - PageIdentificationString.Length);
                        pageNumberString = pageNumberString.Replace("_", " ");
                        pageNumberString = pageNumberString.Replace("-", " ");
                        System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"[a-zA-Z]");
                        if (rg.IsMatch(pageNumberString))
                        {
                            string[] splitString = rg.Split(pageNumberString);
                            if (splitString.Length > 0) pageNumberString = splitString[0];
                            Console.WriteLine("page number string: " + pageNumberString);
                        }
                        int pageNum = 0;
                        int.TryParse(pageNumberString, out pageNum);
                        if (pageNum > 0)
                        {
                            
                            PhraseNode node = CreatePagePhraseWithNegligibleAudio( new PageNumber(pageNum, PageKind.Normal));
                            pagePhrases.Insert(0, node);
                        }
                        pagePhrases.Insert(0, phraseNodes[i]) ;
                        int j = 0;
                        for (j = i + 1;
                        j < phraseNodes.Count && !phrase_SectionNameMap.ContainsKey(phraseNodes[j]);
                        j++)
                        {
                            pagePhrases.Insert(0,phraseNodes[j]);
                        }
                        if (pagePhrases.Count > 0
                            && i == 0)
                        {
                            phraseInsertIndex = firstSection.PhraseChildCount;
                            for (int pageCount = 0; pageCount < pagePhrases.Count; pageCount++)
                            {//2
                                phraseInsertIndex++;
                                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(this, pagePhrases[pageCount], firstSection, phraseInsertIndex));
                            }//-2
                            pagePhrases.Clear();
                            
                        }
                        continue;
                    }
                    phraseInsertIndex = 0;
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(this, phraseNodes[i], newSectionNode, phraseInsertIndex));

                    for (int j = i + 1;
                        j < phraseNodes.Count && !phrase_SectionNameMap.ContainsKey(phraseNodes[j]);
                        j++)
                    {
                        phraseInsertIndex++;
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(this, phraseNodes[j], newSectionNode, phraseInsertIndex));
                    }
                    if (PageIdentificationString != null &&  newSectionName.Contains(PageIdentificationString) && !newSectionName.StartsWith(PageIdentificationString))
                    {
                        int pageNumIndex = newSectionName.IndexOf(PageIdentificationString);
                        string pageNumberString = newSectionName.Substring(pageNumIndex + PageIdentificationString.Length, newSectionName.Length - pageNumIndex - PageIdentificationString.Length);
                        //MessageBox.Show(pageNumberString);
                        pageNumberString = pageNumberString.Replace("_", " ");
                        pageNumberString = pageNumberString.Replace("-", " ");
                        System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"[a-zA-Z]");
                        if (rg.IsMatch(pageNumberString))
                        {
                            string[] splitString = rg.Split(pageNumberString);
                            if (splitString.Length > 0) pageNumberString = splitString[0];
                            Console.WriteLine("page number string: " + pageNumberString);
                        }
                        int pageNum = 0;
                        int.TryParse(pageNumberString, out pageNum);
                        if (pageNum > 0)
                        {
                            PhraseNode node = CreatePagePhraseWithNegligibleAudio(new PageNumber(pageNum, PageKind.Normal));
                            
                            phraseInsertIndex++;
                            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(this, node, newSectionNode, phraseInsertIndex));
                        }
                    }

                    if (pagePhrases.Count > 0)
                    {//1
                        for (int pageCount = 0; pageCount < pagePhrases.Count; pageCount++)
                        {//2
                            phraseInsertIndex++;
                            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(this, pagePhrases[pageCount], newSectionNode, phraseInsertIndex));
                        }//-2
                        pagePhrases.Clear();
                    }//-1
                    
                    
                    //MessageBox.Show(phrase_SectionNameMap[phraseNodes[i]]);
                }
}
                if (newSectionNode != null) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(this, new NodeSelection(newSectionNode, mContentView)));
            return command;
            }

        private PhraseNode CreatePagePhraseWithNegligibleAudio(PageNumber pgNumber)
        {
            PhraseNode node = mPresentation.CreatePhraseNode();
            node.Audio = mPresentation.MediaFactory.CreateManagedAudioMedia();
            node.Audio.AudioMediaData = mPresentation.MediaDataFactory.Create<urakawa.media.data.audio.codec.WavAudioMediaData>();
            //byte [] zeroAudio = new byte[4096] ;\
            int audioLength = Convert.ToInt32(mPresentation.MediaDataManager.DefaultPCMFormat.Data.ByteRate * 0.75);
            byte[] zeroAudio = new byte[audioLength];
            node.Audio.AudioMediaData.AppendPcmData(new System.IO.MemoryStream(zeroAudio), new urakawa.media.timing.Time(mPresentation.MediaDataManager.DefaultPCMFormat.Data.ConvertBytesToTime(zeroAudio.Length)));
            node.Role_ = EmptyNode.Role.Page;
            node.PageNumber = pgNumber;
            return node;
        }

        public bool CanImportPhrases { get { return GetSelectedPhraseSection!= null  && !TransportBar.IsRecorderActive; } }
        public bool CanExportSelectedNodeAudio { get { return Selection != null && (Selection.Node is PhraseNode || (Selection.Node is SectionNode && !(Selection is StripIndexSelection))) && !(Selection is AudioSelection)  && !TransportBar.IsRecorderActive; } }
        public bool CanReplacePhrasesWithimproperAudioWithEmptyNodes { get { return mPresentation != null && !mTransportBar.IsRecorderActive; } }

        /// <summary>
        /// Bring up the file chooser to select audio files to import and return new phrase nodes for the selected files,
        /// or null if nothing was selected.
        /// </summary>
        private string[] SelectFilesToImport()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = Localizer.Message("audio_file_filter");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileNames;
            }
            else
            {
                return null;
            }
        }

        public void GenerateSpeechForPage(bool forAllEmptyPages)
        {
            if (CanGenerateSpeechForPage || CanGenerateSpeechForAllEmptyPages)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                List<EmptyNode> listOfEmptyPages = null ;
                if (forAllEmptyPages)
                {
                    listOfEmptyPages = GetListOfEmptyPagesInProject();
                }
                else
                {
                    listOfEmptyPages = new List<EmptyNode> () ;
                    listOfEmptyPages.Add((EmptyNode) Selection.Node);
                }
                if (listOfEmptyPages == null || listOfEmptyPages.Count == 0)
                {
                    MessageBox.Show(Localizer.Message("ProjectView_NoEmptyPageFoundForSpeechGeneration"));
                    return;
                }
                bool isException = false ;
                EmptyNode nodeToBeSelected = null;
                
                try
                    {
                    
                        CompositeCommand cmd = mPresentation.CreateCompositeCommand("Generate speech from page text");
                        if (Selection != null && forAllEmptyPages)
                        {
                            cmd.ChildCommands.Insert( cmd.ChildCommands.Count, new Commands.UpdateSelection (this, new NodeSelection(Selection.Node,Selection.Control)) );
                        }
                    Dialogs.ProgressDialog progress = new Dialogs.ProgressDialog ( Localizer.Message ( "SpeechGenerationProgress" ),
                        delegate(Dialogs.ProgressDialog progress1)
                            {
                for (int i = 0; i < listOfEmptyPages.Count; i++)
                {
                    PageNumber number = listOfEmptyPages[i].PageNumber;
                    string text = "Page" + (number.Kind == PageKind.Front ? " front, " + number.Number.ToString() : number.Kind == PageKind.Normal ? ", " + number.Number.ToString() : ", " + number.Unquoted);
                    string filePath = System.IO.Path.Combine(mPresentation.DataProviderManager.DataFileDirectoryFullPath, mPresentation.DataProviderManager.GetNewDataFileRelPath(".wav"));
                    Audio.AudioFormatConverter.InitializeTTS(ObiForm.Settings, mPresentation.MediaDataManager.DefaultPCMFormat.Data);
                    Audio.AudioFormatConverter.Speak(text, filePath, ObiForm.Settings, mPresentation.MediaDataManager.DefaultPCMFormat.Data);
                    //if (ProgressChanged != null)
                        //ProgressChanged(this, new ProgressChangedEventArgs( Convert.ToInt32((i*100)/100) , "" ));

                if (System.IO.File.Exists(filePath))
                {
                    
                        PhraseNode pagePhrase = mPresentation.CreatePhraseNode(filePath);
                        pagePhrase.CopyAttributes(listOfEmptyPages[i]);
                        
                        cmd.ChildCommands.Insert(cmd.ChildCommands.Count, new Commands.Node.Delete(this, listOfEmptyPages[i]));
                        Commands.Node.AddNode add = new Obi.Commands.Node.AddNode(this, pagePhrase, listOfEmptyPages[i].ParentAs<SectionNode>(), listOfEmptyPages[i].Index);
                        add.UpdateSelection = !forAllEmptyPages;
                        if (Selection != null && Selection.Node == listOfEmptyPages[i])
                        {
                            nodeToBeSelected = pagePhrase;
                        }
                        cmd.ChildCommands.Insert(cmd.ChildCommands.Count, add);
                        

                        
                }
                }
            });
                    progress.OperationCancelled += new Obi.Dialogs.OperationCancelledHandler(delegate(object sender, EventArgs e) { Audio.PhraseDetection.CancelOperation = true; });
                    //this.ProgressChanged += new ProgressChangedEventHandler(progress.UpdateProgressBar);
                    progress.ShowDialog();
                    if (progress.Exception != null) throw (progress.Exception);

                    if (Selection != null && forAllEmptyPages)
                    {
                        if (nodeToBeSelected != null)
                        {
                            NodeSelection sel = new NodeSelection(nodeToBeSelected, mContentView);
                            cmd.ChildCommands.Insert(cmd.ChildCommands.Count, new Commands.UpdateSelection(this, sel));
                            
                        }
                    }
                    mPresentation.Do(cmd);
                }
                    catch (System.Exception ex)
                    {
                        isException = true;
                        WriteToLogFile(ex.ToString());
                        MessageBox.Show(ex.ToString());
                        
                    }

                    if (forAllEmptyPages)
                    {
                        if (!isException &&  listOfEmptyPages!=null && listOfEmptyPages.Count > 0)
                        {
                            MessageBox.Show(Localizer.Message("ProjectView_GenarteSpeechForAllPages"));
                            //if (Selection != null && !Selection.Node.IsRooted && selectedPhraseIndex > -1)
                            //{
                                //if (selectedSection != null && selectedPhraseIndex < selectedSection.PhraseChildCount) Selection = new NodeSelection(selectedSection.PhraseChild(selectedPhraseIndex), mContentView);
                            //}
                        }
                    }
            }

        }

        private List<EmptyNode> GetListOfEmptyPagesInProject()
        {
            List <EmptyNode> listOfEmptyPages = new List<EmptyNode> ();
            mPresentation.RootNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                        {
                            if (!(n is PhraseNode)) listOfEmptyPages.Add((EmptyNode)n) ;
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });

            return listOfEmptyPages;
        }

        public void SelectNothing () 
        {
            if (Selection != null && Selection is TextSelection && Selection.Node is SectionNode && Selection.Control is ContentView) Selection = new NodeSelection(Selection.Node, mContentView);
            Selection = null; 
        }

        public void SetRoleForSelectedBlock ( EmptyNode.Role kind, string custom )
            {
            mPresentation.Do ( new Commands.Node.AssignRole ( this, SelectedNodeAs<EmptyNode> (), kind, custom ) );
            }

        public void SetCustomTypeOnEmptyNode(EmptyNode node, EmptyNode.Role nodeKind, string customClass)   //@AssociateNode
        {
            if (node != null)
            {
                if (node.Role_ != nodeKind || node.CustomRole != customClass)
                {
                    mPresentation.Do(new Obi.Commands.Node.AssignRole(this, node, customClass));
                }
            }
        }

        public void SetSilenceRoleForSelectedPhrase ()
            {
            PhraseNode node = SelectedNodeAs<PhraseNode> ();
            if (node != null)
                {
                CompositeCommand command = Presentation.CommandFactory.CreateCompositeCommand ();
                Commands.Node.AssignRole silence = new Commands.Node.AssignRole ( this, node, EmptyNode.Role.Silence );
                command.ChildCommands.Insert(command.ChildCommands.Count, silence );
                command.ShortDescription = silence.ShortDescription;
                if (node.Used) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed ( this, node ) );
                Presentation.Do ( command );
                }
            }

        public void ClearRoleOfSelectedPhrase ()
            {
            EmptyNode node = SelectedNodeAs<EmptyNode> ();
            if (node != null)
                {
                    CompositeCommand command = Presentation.CommandFactory.CreateCompositeCommand();
                if (node.Role_ != EmptyNode.Role.Silence)
                    {
                    //SetRoleForSelectedBlock ( EmptyNode.Role.Plain, null );
                        Commands.Node.AssignRole ClearRoleCmd = new Commands.Node.AssignRole(this, node, EmptyNode.Role.Plain);
                        command.ChildCommands.Insert(command.ChildCommands.Count, ClearRoleCmd);
                        command.ShortDescription = ClearRoleCmd.ShortDescription;

                    // if selected node is associated to anchor then it should be disassociated and if possible associated to next skippable phrase in the chunk
                    if ( node.Role_ == EmptyNode.Role.Custom && !string.IsNullOrEmpty(node.CustomRole) && EmptyNode.SkippableNamesList.Contains (node.CustomRole) )
                    {
                        EmptyNode anchorNode =  mPresentation.GetAnchorForReferencedNode(node);
                        if (anchorNode != null )
                        {
                            Commands.Node.DeAssociateAnchorNode disassociateCmd = new Obi.Commands.Node.DeAssociateAnchorNode(this,anchorNode ) ;
                            disassociateCmd.UpdateSelection = false;
                            command.ChildCommands.Insert(command.ChildCommands.Count, disassociateCmd);
                             ObiNode next = node.FollowingNode;
                             EmptyNode followingNode = next != null ? (EmptyNode)next : null;
                            if (followingNode != null && followingNode.Role_ == EmptyNode.Role.Custom && followingNode.CustomRole == node.CustomRole && mPresentation.GetAnchorForReferencedNode(followingNode) == null)
                            {
                                Commands.Node.AssociateAnchorNode associateCmd = new Obi.Commands.Node.AssociateAnchorNode(this,anchorNode,followingNode ) ;
                                associateCmd.UpdateSelection = false;
                                command.ChildCommands.Insert(command.ChildCommands.Count, associateCmd);
                            }
                        } // check for anchor != null
                    }// check for skippable

                    }
                else
                    {
                    
                    Commands.Node.AssignRole ClearRoleCmd = new Commands.Node.AssignRole ( this, node, EmptyNode.Role.Plain );
                    command.ChildCommands.Insert(command.ChildCommands.Count, ClearRoleCmd );
                    command.ShortDescription = ClearRoleCmd.ShortDescription ;
                    if (!node.Used) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed ( this, node ) );
                    
                    }
                    Presentation.Do(command);
                }
            }

        /// <summary>
        /// Split a phrase at the playback cursor time (when playback is going on),
        /// the cursor position, or at both ends of the audio selection.
        /// </summary>
        public void SplitPhrase ()
            {
            
            bool wasPlaying = TransportBar.CurrentState == TransportBar.State.Playing;
            if (TransportBar.CurrentState == TransportBar.State.Playing && !PauseAndCreatePlayingSection()) return;
            if (TransportBar.PlaybackPhrase != null && TransportBar.CurrentPlaylist.PlaybackRate != 0)
            {
                if (TransportBar.PlaybackPhrase.Duration < TransportBar.SplitBeginTime)
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

                    // reselect the selected node: work around for disable scrolling problem. 
                    //earlier selection was assigned twice before command, it do not happen now so this happens post command execution
                    Selection = new NodeSelection ( Selection.Node, mContentView );
                    EmptyNode currentNode = (EmptyNode)Selection.Node;
                    if (currentNode.Role_ == EmptyNode.Role.Custom)     //@AssociateNode
                        SetRoleForSelectedBlock(EmptyNode.Role.Custom, currentNode.CustomRole);
                }
                catch (System.Exception ex)
                    {
                    this.WriteToLogFile(ex.ToString());
                    MessageBox.Show (Localizer.Message("ProjectViewFormMsg_SplitOperationFail") + "\n\n" + ex.ToString () );   //@Messagecorrected
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
                double duration;
               
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
                        duration = Selection.Node.Duration + Selection.Node.FollowingNode.Duration;
                        if (duration > ObiForm.Settings.MaxAllowedPhraseDurationInMinutes * 60 * 1000)
                        {
                            MessageBox.Show(string.Format(Localizer.Message("MergePhrases_SizeLimitMessage"), ObiForm.Settings.MaxAllowedPhraseDurationInMinutes));
                            
                            TransportBar.SelectionChangedPlaybackEnabled = playbackOnSelectionChangeStatus;
                            return;
                        }
                    mPresentation.UndoRedoManager.Execute ( Commands.Node.MergeAudio.GetMergeCommand ( this ) );
                    TransportBar.SelectionChangedPlaybackEnabled = playbackOnSelectionChangeStatus;
                    }
                catch (System.Exception ex)
                    {
                     this.WriteToLogFile(ex.ToString());   
                     MessageBox.Show(Localizer.Message("ProjectViewFormMsg_MergeOperationFail") + "\n\n" + ex.ToString()); //@Messagecorrected
                    }
                }
            }

        public void MergePhraseWithFollowingPhrasesInSection () { MergeRangeOfPhrasesInSection ( true ); }
        public void MergeWithPhrasesBeforeInSection  () { MergeRangeOfPhrasesInSection ( false); }

        private void MergeRangeOfPhrasesInSection ( bool mergeWithFollowing)
            {
        if ( ( mergeWithFollowing && CanMergePhraseWithFollowingPhrasesInSection )
            || (!mergeWithFollowing && CanMergeWithPhrasesBeforeInSection ))
                {
                if (mTransportBar.IsPlayerActive)
                    {
                    TransportBar.MoveSelectionToPlaybackPhrase ();
                    mTransportBar.Stop ();
                    }

                SectionNode section = ((EmptyNode)Selection.Node).ParentAs<SectionNode> ();

                EmptyNode startNode = mergeWithFollowing ? (EmptyNode)Selection.Node :
                    section.PhraseChild ( 0 );
                EmptyNode endNode = mergeWithFollowing ? section.PhraseChild ( section.PhraseChildCount - 1 ) :
                    (EmptyNode)Selection.Node;
            // check if the merged phrase is not more than 10 mins

                double durationSum = 0;
                //if ((endNode.Index - startNode.Index) > 300)
                //{
                    //MessageBox.Show(string.Format(Localizer.Message("MergePhrases_CountLimitMessage"), 300 ));
                    //MessageBox.Show("The number of phrases merged is more than 300");
                    //return;
                //}
                for (int i = startNode.Index; i <= endNode.Index; i++)
                    {
                    durationSum += section.PhraseChild (i).Duration;
                    }
                    if (durationSum > ObiForm.Settings.MaxAllowedPhraseDurationInMinutes * 60 * 1000)
                    {
                        MessageBox.Show(string.Format(Localizer.Message("MergePhrases_SizeLimitMessage"), ObiForm.Settings.MaxAllowedPhraseDurationInMinutes));
                    return;
                    }

                bool PlayOnSelectionStatus = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;

                
                try
                    {
                    mPresentation.Do ( GetMergeRangeOfPhrasesInSectionCommand ( section, startNode, endNode));
                    
                    }
                catch (System.Exception ex)
                    {
                    this.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ProjectViewFormMsg_MergeOperationFail") + "\n\n" + ex.ToString());   //@Messagecorrected
                    }
                TransportBar.SelectionChangedPlaybackEnabled = PlayOnSelectionStatus;
                }
        }


        public urakawa.command.Command GetMergeRangeOfPhrasesInSectionCommand ( SectionNode section, EmptyNode startNode, EmptyNode endNode )
            {
            CompositeCommand command = mPresentation.CreateCompositeCommand ( Localizer.Message ( "Merge_RangeOfPhrases" ) );
            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection ( this, Selection ) );

            int startIndex = startNode.Index;
            int endIndex = endNode.Index;

            EmptyNode nodeToSelect = null;
            for (int i = startIndex; i <= endIndex; i++)
                {
                if (section.PhraseChild ( i ) is PhraseNode)
                    {
                    nodeToSelect = section.PhraseChild ( i );
                    break;
                    }
                }
            //MessageBox.Show ( nodeToSelect.ToString () );

            Commands.UpdateSelection initialSelection = new Commands.UpdateSelection ( this, new NodeSelection ( nodeToSelect != null ? nodeToSelect : section.PhraseChild ( startIndex ), Selection.Control ) ) ;
            initialSelection.RefreshSelectionForUnexecute = true ;
            command.ChildCommands.Insert(command.ChildCommands.Count,initialSelection );
            
            EmptyNode phraseRole = null;
            
            int progressInterval = (endIndex - startIndex) > 100 ? (endIndex - startIndex) * 2 / 100 : 1*2 ; // multiplied by 2 to increment progress by 2
            int progressPercent = 0;
            EmptyNode firstNode = null;
            PhraseNode secondNode = null;
            List<PhraseNode> listOfNodesToMerge = new List<PhraseNode>();
            for (int i = endIndex; i >= startIndex; i--)
                {
                firstNode = section.PhraseChild ( i );
                if (firstNode.Role_ == EmptyNode.Role.Heading)
                    {
                    phraseRole = firstNode;
                    }
                if (firstNode is PhraseNode)
                    {
                    if (secondNode != null)
                        {
                            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(this, secondNode, false));
                            if(!listOfNodesToMerge.Contains(secondNode) ) listOfNodesToMerge.Insert(0, secondNode);
                            
                        if (i == startIndex || progressPercent > 98) progressPercent = 98;
                        
                        }
                    secondNode = (PhraseNode)firstNode;
                    }
                else if (!(firstNode is PhraseNode))
                    {
                    if (firstNode.Index ==startIndex  && secondNode == null)
                    {/* do nothing */ }
                    else
                        {
                        // before deleting first node, delete second node so that delete is in order else it will create problem in undo
                            if (secondNode != null )
                            {


                                if (nodeToSelect != secondNode)
                                {
                                    if (!listOfNodesToMerge.Contains(secondNode)) listOfNodesToMerge.Insert(0, secondNode);
                                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(this, secondNode, false));
                                    secondNode = null;
                                }
                                
                            }
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete ( this, firstNode, false ) );
                        //Console.WriteLine ( "deleting in merge " + firstNode );
                        }
                    }
    
                }
                PhraseNode tempNode = mPresentation.CreatePhraseNode();
                if (listOfNodesToMerge.Count > 0)
                    tempNode.Audio = listOfNodesToMerge[0].Audio.Copy();

                if (listOfNodesToMerge.Count > 1)
                {
                    for (int i = 1; i < listOfNodesToMerge.Count; i++)
                    {
                        tempNode.MergeAudioWith(listOfNodesToMerge[i].Audio.Copy());
                    }
                }
                if (!(startNode is PhraseNode)) startNode = nodeToSelect;
                if (tempNode != null && tempNode.Audio != null)
                {
                    Commands.Command mergeCmd = new Commands.Node.MergeAudio(this, (PhraseNode)startNode, tempNode);
                    command.ChildCommands.Insert(command.ChildCommands.Count, mergeCmd);
                }
            if (phraseRole != null )
                {
                command.ChildCommands.Insert (0, new Commands.Node.AssignRole ( this, nodeToSelect != null ? nodeToSelect : startNode, EmptyNode.Role.Heading ));
                command.ChildCommands.Insert(0, new Commands.Node.UnsetNodeAsHeadingPhrase(this, (PhraseNode)phraseRole));
                                }
            return command;
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
                urakawa.command.Command cmd = new Commands.Node.SetPageNumber ( this, SelectedNodeAs<EmptyNode> (), number );
                if (renumber)
                    {
                        CompositeCommand k = GetPageRenumberCommand (SelectedNodeAs<EmptyNode> (), number, cmd.ShortDescription ,true) ;
                    //CompositeCommand k = Presentation.CreateCompositeCommand ( cmd.ShortDescription );
                    //for (ObiNode n = SelectedNodeAs<EmptyNode> ().FollowingNode; n != null; n = n.FollowingNode)
                        //{
                        //if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                            //((EmptyNode)n).PageNumber.Kind == number.Kind)
                            //{
                            //number = number.NextPageNumber ();
                            //k.ChildCommands.Insert(k.ChildCommands.Count, new Commands.Node.SetPageNumber ( this, (EmptyNode)n, number ) );
                            //}
                        //}
                    k.ChildCommands.Insert(k.ChildCommands.Count, cmd );
                    cmd = k;
                    }
                mPresentation.Do ( cmd );
                }
            }

        /// <summary>
        /// Creates composite command for renumbering pages 
        /// </summary>
        /// <param name="pageNode"></param>
        /// <param name="number"></param>
        /// <param name="shortDescription"></param>
        /// <returns></returns>
        public CompositeCommand GetPageRenumberCommand(EmptyNode pageNode, PageNumber number, string shortDescription, bool renumberFromNextNode)
        {
            // when bool renumberFromNextNode is true, code bahaves as earlier
            CompositeCommand k = Presentation.CreateCompositeCommand(shortDescription);
            for (ObiNode n = renumberFromNextNode? pageNode.FollowingNode: pageNode; 
                n != null; 
                n = n.FollowingNode)
            {
                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                    ((EmptyNode)n).PageNumber.Kind == number.Kind)
                {
                    number = number.NextPageNumber();
                    Commands.Node.SetPageNumber c =   new Commands.Node.SetPageNumber(this, (EmptyNode)n, number) ;
                    c.UpdateSelection = false;
                    k.ChildCommands.Insert(k.ChildCommands.Count,c );
                }
            }
            return k;
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
                        EmptyNode node = Presentation.TreeNodeFactory.Create<EmptyNode>();
                    if (parent == null)
                        {
                        parent = mSelection.ParentForNewNode ( node );
                        index = mSelection.IndexForNewNode ( node );
                        }
                    cmd.ChildCommands.Insert(cmd.ChildCommands.Count, new Commands.Node.AddEmptyNode ( this, node, parent, index + i ) );
                    cmd.ChildCommands.Insert(cmd.ChildCommands.Count, new Commands.Node.SetPageNumber ( this, node, number ) );
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
                            cmd.ChildCommands.Insert(cmd.ChildCommands.Count, c );
                            number = number.NextPageNumber ();
                            }
                        }
                    }
                mPresentation.Do ( cmd );
                }
            }

        public void UpdatePhraseDetectionSettingsFromSilencePhrase()
        {
            if (CanUpdatePhraseDetectionSettingsFromSilencePhrase)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();

                PhraseNode node = SelectedNodeAs<PhraseNode>();
                Dialogs.SentenceDetection dialog = new Obi.Dialogs.SentenceDetection ( node as PhraseNode );
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ObiForm.Settings.DefaultGap=(decimal) dialog.Gap;
                    ObiForm.Settings.DefaultLeadingSilence = (decimal)dialog.LeadingSilence;
                    ObiForm.Settings.DefaultThreshold = dialog.Threshold;
                    
                }
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
                ObiNode node = null;
                if (Selection.Node is SectionNode && ((SectionNode)Selection.Node).PhraseChildCount> 0 )
                    {
                    node = ((SectionNode)Selection.Node).PhraseChild(0) ;
                    }

                for (node =node != null?node:  SelectedNodeAs<EmptyNode> ();
                    node != null && !(node is PhraseNode && ((PhraseNode)node).Role_ == EmptyNode.Role.Silence);
                    node = node.PrecedingNode) { }
                Dialogs.SentenceDetection dialog = new Obi.Dialogs.SentenceDetection ( node as PhraseNode );
                if (dialog.ShowDialog () == DialogResult.OK)
                    {
                        Audio.PhraseDetection.RetainSilenceInBeginningOfPhrase = ObiForm.Settings.RetainInitialSilenceInPhraseDetection;
                    bool playbackOnSelectionChangedStatus = TransportBar.SelectionChangedPlaybackEnabled;
                    TransportBar.SelectionChangedPlaybackEnabled = false;
                    ObiNode phraseDetectionNode = Selection.Node ;
                    CompositeCommand command = null;
                    ObiForm.CanAutoSave = false;//explicitly do this as getting of command takes a lot of time
                    Dialogs.ProgressDialog progress = new Dialogs.ProgressDialog ( Localizer.Message ( "phrase_detection_progress" ),
                        delegate(Dialogs.ProgressDialog progress1)
                            {
                            command = Commands.Node.SplitAudio.GetPhraseDetectionCommand ( this, phraseDetectionNode,
                                dialog.Threshold, dialog.Gap, dialog.LeadingSilence,ObiForm.Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection);
                            } );
                    progress.OperationCancelled += new Obi.Dialogs.OperationCancelledHandler(delegate(object sender, EventArgs e) { Audio.PhraseDetection.CancelOperation = true; });
                    this.ProgressChanged += new ProgressChangedEventHandler(progress.UpdateProgressBar);
                        
                    progress.ShowDialog ();
                    if (progress.Exception != null) throw (progress.Exception);
                    ObiForm.CanAutoSave = true;//explicitly do this as getting of command takes a lot of time
                    mPresentation.Do ( command );

                    SectionNode SNode = phraseDetectionNode is EmptyNode? phraseDetectionNode.ParentAs<SectionNode>() : (SectionNode) phraseDetectionNode;
        
                    Strip currentlyActiveStrip = mContentView.ActiveStrip ;
                    if (currentlyActiveStrip != null && currentlyActiveStrip.Node == SNode)//@singleSection
                        {
                        mContentView.CreateBlocksInStrip (); 

                        if (SNode != null && SNode.PhraseChildCount > MaxVisibleBlocksCount)
                            MessageBox.Show ( string.Format ( Localizer.Message ( "ContentHidden_SectionHasOverlimitPhrases" ), SNode.Label, MaxVisibleBlocksCount ), Localizer.Message ( "Caption_Warning" ), MessageBoxButtons.OK, MessageBoxIcon.Warning );
                        }
                    TransportBar.SelectionChangedPlaybackEnabled = playbackOnSelectionChangedStatus;
                    }
                }
            }
    

        public void ApplyPhraseDetectionInWholeProject ()
            {
            if (CanApplyPhraseDetectionInWholeProject)
                {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop ();

                List<SectionNode> listOfAllSections = new List<SectionNode> ();
                List<PhraseNode> listOfAllSilencePhrases = new List<PhraseNode> ();
                mPresentation.RootNode.AcceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {
                        if (n is SectionNode) listOfAllSections.Add ( (SectionNode)n );
                        if (n is PhraseNode && ((PhraseNode)n).Role_ == EmptyNode.Role.Silence) listOfAllSilencePhrases.Add ( (PhraseNode)n );
                        return true;
                        },
                    delegate ( urakawa.core.TreeNode n ) { } );

                Dialogs.SelectPhraseDetectionSections sectionsSelectionDialog = new Obi.Dialogs.SelectPhraseDetectionSections ( listOfAllSections, listOfAllSilencePhrases,this.GetSelectedPhraseSection);
                sectionsSelectionDialog.ShowDialog () ;
                if (sectionsSelectionDialog.DialogResult == DialogResult.Cancel) return;

                List<SectionNode> sectionsList = sectionsSelectionDialog.SelectedSections;
                if (sectionsList.Count == 0) return;

                PhraseNode silencePhrase = sectionsSelectionDialog.SelectedSilencePhrase ;

/*                // first find silence phrase from first section
                SectionNode firstSection = mPresentation.FirstSection;
                PhraseNode silencePhrase = null ;
                for ( int i = 0 ; i < firstSection.PhraseChildCount ; i++ )
                    {
                    if ( firstSection.PhraseChild(i) is PhraseNode && firstSection.PhraseChild(i).Role_ == EmptyNode.Role.Silence )
                        {
                        silencePhrase = (PhraseNode) firstSection.PhraseChild(i) ;
                        break;
                        }
                    }
                */
                
                Dialogs.SentenceDetection dialog = new Obi.Dialogs.SentenceDetection ( silencePhrase );
                if (dialog.ShowDialog () == DialogResult.OK)
                    {
                    bool playbackOnSelectionChangedStatus = TransportBar.SelectionChangedPlaybackEnabled;
                    TransportBar.SelectionChangedPlaybackEnabled = false;
                    //CompositeCommand command = mPresentation.CreateCompositeCommand ( Localizer.Message ( "PhraseDetection_WholeProject" ) );
                    List<CompositeCommand> listOfCommands = new List<CompositeCommand> ();

                    int i = 0; //iterator for for loop
                    ObiForm.CanAutoSave = false;//explicitly do this as getting of command takes a lot of time
                    Dialogs.ProgressDialog progress = new Dialogs.ProgressDialog ( Localizer.Message ( "phrase_detection_progress" ),
                        delegate ( Dialogs.ProgressDialog progress1)
                            {
                                Audio.PhraseDetection.RetainSilenceInBeginningOfPhrase = ObiForm.Settings.RetainInitialSilenceInPhraseDetection;
                            for (i = 0; i < sectionsList.Count; i++)
                                {
                                    if (progress1.CancelOperation)
                                    {
                                        Audio.PhraseDetection.CancelOperation = true;
                                        break;
                                    }
                                if (sectionsList[i].PhraseChildCount > 0)
                                    {
                                    listOfCommands.Add( Commands.Node.SplitAudio.GetPhraseDetectionCommand ( this, sectionsList[i],
                                        dialog.Threshold, dialog.Gap, dialog.LeadingSilence, ObiForm.Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection ) );
                                    }
                                }
                            } );
                    progress.OperationCancelled += new Obi.Dialogs.OperationCancelledHandler(delegate ( object sender   , EventArgs e) { Audio.PhraseDetection.CancelOperation = true; });
                    int progressValue = 0 ;
                    int baseProgressValue = 0;
                    int previous_I = 0;
                    this.ProgressChanged+= new ProgressChangedEventHandler(delegate(object sender, ProgressChangedEventArgs e)
                    {
                        if (i != previous_I)
                        {
                            baseProgressValue += progressValue;
                            previous_I = i;
                        }
                        //progressValue = (e.ProgressPercentage * i )/ sectionsList.Count ;
                        progressValue =sectionsList.Count > 0? (100 * i) / sectionsList.Count: e.ProgressPercentage ;
                        //if (progressValue >= 100) progressValue = 95;
                        progress.UpdateProgressBar ( this, new ProgressChangedEventArgs ( progressValue, "") );
                    });
                    

                    progress.ShowDialog ();
                    ObiForm.CanAutoSave = true;//explicitly do this as getting of command takes a lot of time
                    //MessageBox.Show ( "Scanning of all files complete " );
                    
                    //Dialogs.ProgressDialog  progress2 = new Dialogs.ProgressDialog ( Localizer.Message ( "phrase_detection_progress" ),
                        //delegate ( Dialogs.ProgressDialog progress1 )
                            //{
                    try
                        {
                        for (i = 0; i < listOfCommands.Count; ++i)
                            {
                            //if (progress1.CancelOperation) break;
                            mPresentation.Do ( listOfCommands[i] );
                            //MessageBox.Show ( "command " + i.ToString () );
                            }
                        }
                    catch (System.Exception ex)
                        {
                        this.WriteToLogFile(ex.ToString());
                        MessageBox.Show (Localizer.Message("ProjectViewFormMsg_PhraseDetectionFail") + "\n\n" + ex.ToString () ); //@Messagecorrected
                        }
                    //} );
                    //progress2.ShowDialog ();

                    SectionNode SNode = GetSelectedPhraseSection;
                    if (SNode != null && SNode.PhraseChildCount > MaxVisibleBlocksCount)
                        MessageBox.Show ( string.Format ( Localizer.Message ( "ContentHidden_SectionHasOverlimitPhrases" ), SNode.Label, MaxVisibleBlocksCount ), Localizer.Message ( "Caption_Warning" ), MessageBoxButtons.OK, MessageBoxIcon.Warning );

                    
                    
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
        public void InitializeShortcutKeys ()
            {
                mContentView.InitializeShortcutKeys();

                KeyboardShortcuts_Settings keyboardShortcuts = ObiForm.KeyboardShortcuts;
            mShortcutKeys = new Dictionary<Keys, HandledShortcutKey> ();
            mShortcutKeys[keyboardShortcuts.ProjectView_MoveToNextViewClockwise.Value] = delegate() { return SelectViewsInCycle(true); };
            mShortcutKeys[keyboardShortcuts.ProjectView_FocusOnTransportBarTimeDisplay.Value] = delegate() { return mTransportBar.FocusOnTimeDisplay(); };
            mShortcutKeys[keyboardShortcuts.ProjectView_MoveToPreviousViewAnticlockwise.Value] = delegate() { return SelectViewsInCycle(false); };
            mShortcutKeys[keyboardShortcuts.ProjectView_PlayPauseUsingAudioCursor_Default.Value] = delegate() { return TogglePlayPause(UseAudioCursor); };
            mShortcutKeys[keyboardShortcuts.ProjectView_PlayPauseUsingSelection.Value] = delegate() { return TogglePlayPause(UseSelection); };
            mShortcutKeys[keyboardShortcuts.ProjectView_ShowPropertiesOfSelectedNode.Value] = delegate() { return ShowNodePropertiesDialog(); };
            mShortcutKeys[keyboardShortcuts.ProjectView_ToggleBetweenContentViewAndTOCView.Value] = delegate() { return ToggleFocusBTWTOCViewAndContentsView(); };
            mShortcutKeys[keyboardShortcuts.ProjectView_HardResetAllSettings.Value] = delegate() { return SettingsFileHardReset(); };
            mTransportBar.InitializeTooltipsForTransportpar();

          /*  mShortcutKeys[Keys.Control | Keys.Tab] = delegate () { return SelectViewsInCycle ( true ); };
            mShortcutKeys[Keys.Control | Keys.Shift | Keys.Tab] = delegate () { return SelectViewsInCycle ( false ); };
            mShortcutKeys[Keys.F6] = delegate () { return ToggleFocusBTWTOCViewAndContentsView (); };
            mShortcutKeys[Keys.Shift | Keys.Space] = delegate () { return TogglePlayPause ( UseSelection ); };
            mShortcutKeys[Keys.Space] = delegate () { return TogglePlayPause ( UseAudioCursor ); };
            mShortcutKeys[Keys.Alt | Keys.Enter] = delegate () { return ShowNodePropertiesDialog (); };
            mShortcutKeys[Keys.F8] = delegate () { return mTransportBar.FocusOnTimeDisplay (); };*/

            }

        // Process key press: if this is a key down event, lookup the shortcut tables;
        // if the key was not handled then, proceed with the default process.
        protected override bool ProcessCmdKey ( ref Message msg, Keys key )
            {
            if (!CanUseKey ( key )) return false;
            SetF1Help(key);
            return (((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) &&
                CanUseKey ( key ) && mShortcutKeys.ContainsKey ( key ) && mShortcutKeys[key] ()) ||
                base.ProcessCmdKey ( ref msg, key );
            }

        // Trap the delete key to prevent deleting a node during text editing
        private bool CanUseKey ( Keys key ) { return !((Selection is TextSelection || mFindInText.ContainsFocus) && key == Keys.Delete); }

        private void SetF1Help(Keys key)
        {
           if (key == Keys.F1)
            {
                if (Presentation == null)
                {
                    helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                    helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm"); 
                }
               else if (mFindInText.ContainsFocus )
               {
                   helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                   helpProvider1.SetHelpKeyword(this, "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Search by Text.htm");                  
               }
               else if ( mMetadataView.ContainsFocus )
               {
                   helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                   helpProvider1.SetHelpKeyword(this, "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Metadata View.htm");      
               }
               else if (mTOCView.ContainsFocus)
               {
                   helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                   helpProvider1.SetHelpKeyword(this, "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\TOC View.htm");                        
               }
               else if (mTransportBar.ContainsFocus)
               {
                   helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                 //helpProvider1.SetHelpKeyword(this, "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Transport Bar.htm");                        
                   helpProvider1.SetHelpKeyword(this, mTransportBar.GetHelpTopicPath());
               }
               else if (Selection != null && GetSelectedPhraseSection != null)
               {
                   if (Selection.Node is SectionNode)
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Sections\\Working with Sections.htm");
                   }
                   else if (Selection.EmptyNodeForSelection.Role_ == EmptyNode.Role.Silence)
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Phrases\\Silence.htm");
                   }
                   else if (Selection.EmptyNodeForSelection.Role_ == EmptyNode.Role.Page)
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Phrases\\Pages.htm");
                   }
                   else if (Selection.EmptyNodeForSelection.Role_ == EmptyNode.Role.Heading)
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Phrases\\Heading.htm");
                   }
                   else if (Selection.EmptyNodeForSelection.Role_ == EmptyNode.Role.Custom && (EmptyNode.SkippableNamesList.Contains(((EmptyNode)Selection.Node).CustomRole)))
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Phrases\\Skippable notes.htm");
                   }
                   else if (Selection.EmptyNodeForSelection.Role_ == EmptyNode.Role.Anchor)
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Phrases\\Associating Skippable Note with Anchor.htm");
                   }
                   else if (Selection.EmptyNodeForSelection.TODO)//For Todo phrases
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Phrases\\Changing the Todo or Used Status.htm");
                   }
                   else if (Selection.EmptyNodeForSelection.Role_ == EmptyNode.Role.Plain)
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Phrases\\Working with Phrases.htm");
                   }
                   else
                   {
                       helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                       helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");
                   }
               }//else if (Selection != null && GetSelectedPhraseSection != null)
               else
               {
                   helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                   helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");
               }
            } 
        }
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

        private bool SettingsFileHardReset()
        {
            if (MessageBox.Show(Localizer.Message("HardResetSettings_Question"), Localizer.Message("HardReset_Caption"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (MessageBox.Show( Localizer.Message("HardResetSettings_ConfirmAgain"), Localizer.Message("HardReset_Caption"), 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    try
                    {
                        ObiForm.Settings.ResetSettingsFile();
                        ObiForm.LoadDefaultKeyboardShortcuts();
                        ObiForm.KeyboardShortcuts.SaveSettings();

                        if (MessageBox.Show(Localizer.Message("HardResetPermanentSettings_Question"), Localizer.Message("HardReset_Caption"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            if (ObiForm.Settings_Permanent != null) ObiForm.Settings_Permanent.ResetSettingsFile();
                        }
                        MessageBox.Show(Localizer.Message("Confirm_Setting"));
                        return true;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            return false;
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
                || (mTransportBar.ContainsFocus )))
                //|| (mTransportBar.ContainsFocus && !mTransportBar.InterceptSpaceBar)))
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
                        if (ObiForm.Settings.Audio_UseRecordingPauseShortcutForStopping && mTransportBar.CurrentState == TransportBar.State.Recording)
                        {
                            TransportBar.Stop();
                        }
                        else
                        {
                            TransportBar.Pause();
                        }
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
            if (TransportBar.IsPlayerActive)
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
                bool selectionChangedPlaybackEnabled = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;
                if (TransportBar.IsPlayerActive || mTransportBar.IsRecorderActive) TransportBar.Stop();
            Dialogs.SectionProperties dialog = new Dialogs.SectionProperties ( this );
            if (dialog.ShowDialog () == DialogResult.OK)
                {
                CompositeCommand command =
                    mPresentation.CreateCompositeCommand ( Localizer.Message ( "update_section" ) );
                if (dialog.Label != dialog.Node.Label && dialog.Label != null && dialog.Label != "")
                    {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.RenameSection ( this, dialog.Node, dialog.Label ) );
                    }
                for (int i = dialog.Node.Level; i < dialog.Level; ++i)
                    {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.TOC.MoveSectionIn ( this, dialog.Node ) );
                    }
                for (int i = dialog.Level; i < dialog.Node.Level; ++i)
                    {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.TOC.MoveSectionOut ( this, dialog.Node ) );
                    }
                if (dialog.Used != dialog.Node.Used)
                    {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed ( this, dialog.Node ) );
                    for (int i = 0 ;i < dialog.Node.PhraseChildCount;i++ )
                    {
                        EmptyNode eNode = dialog.Node.PhraseChild(i);
                        if (dialog.Used != eNode.Used )   command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed(this, eNode));
                    }
                    }
                if (command.ChildCommands.Count == 1) command.ShortDescription = command.ChildCommands.Get(0).ShortDescription ;
                if (command.ChildCommands.Count > 0) mPresentation.Do(command);
                }
                TransportBar.SelectionChangedPlaybackEnabled = selectionChangedPlaybackEnabled;
            return true;
            }

        public bool ShowPhrasePropertiesDialog ( bool SetCustomClassName )
            {
            // if custom class is to be set then playback should be paused else allowed to continue.
            if (SetCustomClassName && TransportBar.IsPlayerActive)
                TransportBar.Pause ();
            bool selectionChangedPlaybackEnabled = TransportBar.SelectionChangedPlaybackEnabled;
            TransportBar.SelectionChangedPlaybackEnabled = false;
            if (TransportBar.IsPlayerActive) TransportBar.Pause();
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
                        urakawa.command.Command PageCmd = new Commands.Node.SetPageNumber ( this, SelectedNodeAs<EmptyNode> (), PageDialog.Number );
                        command.ChildCommands.Insert(command.ChildCommands.Count, PageCmd );
                        PageNumber number = PageDialog.Number;
                        if (PageDialog.Renumber)
                            {
                            for (ObiNode n = SelectedNodeAs<EmptyNode> ().FollowingNode; n != null; n = n.FollowingNode)
                                {
                                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                                    ((EmptyNode)n).PageNumber.Kind == number.Kind)
                                    {
                                    number = number.NextPageNumber ();
                                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.SetPageNumber ( this, (EmptyNode)n, number ) );
                                    }
                                }
                            }
                        }
                    } // page related braces ends

                if (dialog.Role != dialog.Node.Role_ ||
                    (dialog.Role == EmptyNode.Role.Custom && dialog.Node.Role_ == EmptyNode.Role.Custom &&
                    dialog.CustomClass != dialog.Node.CustomRole))
                    {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AssignRole ( this, dialog.Node, dialog.Role, dialog.CustomClass ) );
                    }
                if (dialog.Used != dialog.Node.Used)
                    {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed ( this, dialog.Node ) );
                    }
                if (dialog.TODO != dialog.Node.TODO)
                    {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeTODO ( this, dialog.Node ) );
                    }
                if (command.ChildCommands.Count == 1) command.ShortDescription = command.ChildCommands.Get(0).ShortDescription ;
                if (command.ChildCommands.Count > 0) mPresentation.Do(command);
                }
                TransportBar.SelectionChangedPlaybackEnabled = selectionChangedPlaybackEnabled;
            return true;
            }

        public void CropPhrase ()
            {
            if (CanCropPhrase)
                {
                if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause ();
                bool playbackOnSelectionStatus = TransportBar.SelectionChangedPlaybackEnabled;
                TransportBar.SelectionChangedPlaybackEnabled = false;

                urakawa.command.Command crop = Commands.Node.SplitAudio.GetCropCommand ( this );
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
                    if ((mContentView.ContainsFocus || selectionControl  == null || (mTransportBar.ContainsFocus && selectionControl is ContentView)) 
                        && node is EmptyNode)
                    {
                    mContentView.SelectPhraseBlockOrStrip ((EmptyNode) node );
                    if (TransportBar.IsPlayerActive 
                        && ( mContentView.PlaybackBlock == null ||  mContentView.PlaybackBlock.Node != TransportBar.PlaybackPhrase))
                        {
                        this.SetPlaybackPhraseAndTime ( TransportBar.PlaybackPhrase, TransportBar.CurrentPlaylist.CurrentTimeInAsset ) ;
                        }
                    return;
                    }
                // if block to be selected is invisible, select parent section
                if ((node is EmptyNode &&  mContentView.IsBlockInvisibleButStripVisible ( (EmptyNode)node ))
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
            /// To check whether zoom panel is active or not.
            /// </summary>
        public bool IsZoomWaveformActive
        {
            get
            {
                if (mContentView.IsZoomWaveformActive == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void ZoomPanelZoomIn() { if (IsZoomWaveformActive) mContentView.ZoomPanelZoomIn(); } //@zoomwaveform
        public void ZoomPanelZoomOut() { if (IsZoomWaveformActive) mContentView.ZoomPanelZoomOut(); } //@zoomWaveform
        public void ZoomPanelReset() { if (IsZoomWaveformActive) mContentView.ZoomPanelReset(); } //@zoomWaveform
        public void ZoomPanelToolTipInit() { if (IsZoomWaveformActive)  mContentView.ZoomPanelToolTipInit(); }
        public void ZoomPanelClose() { if (IsZoomWaveformActive) mContentView.ZoomPanelClose(); }

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
                    Commands.Node.SetPageNumber cmd = new Commands.Node.SetPageNumber ( this, (EmptyNode)n, number );
                    cmd.UpdateSelection = false ;
                    k.ChildCommands.Insert(k.ChildCommands.Count,cmd );
                    number = number.NextPageNumber ();

                    }
                }
            return k;
            }

        // @phraseLimit
        /// <summary>
        /// shows contents of selected strip
        /// </summary>
        public bool ShowSelectedSectionContents ()
            {
                        if (Selection != null && Selection.Node is SectionNode)//@singleSection
                {
                            //replacement code starts
                    Strip currentlyActiveStrip = mContentView.ActiveStrip;
                    Strip newStrip = null;
                    if (GetSelectedPhraseSection != null)//@singleSection
                    {
                        if (currentlyActiveStrip != null
                        && GetSelectedPhraseSection != currentlyActiveStrip.Node && mContentView.RestrictDynamicLoadingForRecording(currentlyActiveStrip.Node))
                        {
                            MessageBox.Show(Localizer.Message("RecordingRestriction_CannotCreateStrip"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK);
                            if (TransportBar.RecordingPhrase != null && currentlyActiveStrip.FindBlock(TransportBar.RecordingPhrase) != null) mContentView.SelectPhraseBlockOrStrip(TransportBar.RecordingPhrase);
                            //currentlyActiveStrip.Focus();
                        }
                        else if (GetSelectedPhraseSection != null && (!TransportBar.IsRecorderActive || TransportBar.RecordingSection == GetSelectedPhraseSection))
                        {
                            newStrip =  mContentView.CreateStripForSelectedSection(GetSelectedPhraseSection, true);
                        }
                        else if (TransportBar.IsRecorderActive)
                        {
                            MessageBox.Show(Localizer.Message("RecordingRestriction_CannotCreateStrip"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK);
                            return false;
                        }
                    }
                    return newStrip != null;
                            // commenting following one line as it is replaced by code above
                //return mContentView.CreateStripForSelectedSection ( (SectionNode)Selection.Node, true )!= null ;
                                                }
            return false ;

            }

        public void RecreateStrip()
        {
            try
            {
                SectionNode section = mContentView.ActiveStrip != null ? mContentView.ActiveStrip.Node : null;
                NodeSelection previousSelection = Selection != null && Selection is NodeSelection ? Selection : null;
                if (section != null)
                {
                    mContentView.RemoveBlocksInStrip(section);
                    
                    mContentView.CreateBlocksInStrip();
                    if (previousSelection != null && previousSelection.Control is ContentView)
                    {
                        if (previousSelection.Node is SectionNode && ((SectionNode)previousSelection.Node).PhraseChildCount > 0) Selection = new NodeSelection ( ((SectionNode) previousSelection.Node).PhraseChild(0) , previousSelection.Control);
                        Selection = new NodeSelection(previousSelection.Node, previousSelection.Control);
                    }
                    else if (previousSelection != null || previousSelection is NodeSelection)
                    {
                        Selection = new NodeSelection(section, mContentView);
                        Selection = previousSelection;
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToLogFile(ex.ToString());
                MessageBox.Show(ex.ToString());
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
        public void GoToPageOrPhrase()
        {
            if (mPresentation.FirstSection == null) return;
            if (mTransportBar.IsPlayerActive) mTransportBar.Pause();
            Dialogs.GoToPageOrPhrase GoToDialog = new Obi.Dialogs.GoToPageOrPhrase(GetSelectedPhraseSection != null ? GetSelectedPhraseSection.PhraseChildCount : mPresentation.FirstSection.PhraseChildCount);
            
            if (GoToDialog.ShowDialog() == DialogResult.OK)
            {
                if (GoToDialog.Number != null)
                {
                    int pageNumber = GoToDialog.Number.Number;
                    PageKind kind = GoToDialog.Number.Kind;
                    EmptyNode node = null;
                    EmptyNode firstSpecialPageMatch = null; // holds first match of special page

                    //flag to indicate if iterations has passed through selected node.   is true if iteration is moved ahead of selected node
                    bool isAfterSelection = (Selection == null || (Selection != null && Presentation.FirstSection == Selection.Node)) ? true : false;

                    for (ObiNode n = ((ObiRootNode)Presentation.RootNode).FirstLeaf; n != null; n = n.FollowingNode)
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
                                        if (firstSpecialPageMatch == null) firstSpecialPageMatch = testNode;

                                        if (Selection != null
                                            && isAfterSelection)
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
                        if (TransportBar.IsPlayerActive) TransportBar.Pause();
                        mContentView.SelectPhraseBlockOrStrip(node);
                    }
                    else
                    {
                        MessageBox.Show(Localizer.Message("GoToPageorPhrase_PageDonotExist"));
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
                            if (MessageBox.Show(string.Format(Localizer.Message("GoToPageOrPhrase_MoreThanPhraseCount"), (phraseIndex + 1).ToString(), (section.PhraseChildCount).ToString()),
    "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
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
                        if (TransportBar.IsPlayerActive) TransportBar.Pause();
                        mContentView.SelectPhraseBlockOrStrip(section.PhraseChild(phraseIndex));
                    } // section null check ends
                }
                else if (GoToDialog.TimeInSeconds != null)
                {
                    AudioSelection sel = null;
                    if (Selection != null)
                    {
                        ObiNode nodeSel = null;
                        PhraseNode phrNode = null;
                        double time = 0;

                        if (GoToDialog.SelectedIndex == 0)
                        {
                            if (this.Selection.Node is StripIndexSelection || this.Selection.Node is SectionNode)
                            {
                                MessageBox.Show(Localizer.Message("select_phrase"));
                                return;
                            }
                            else
                            {
                                if (this.Selection.Node.Duration > GoToDialog.TimeInSeconds)

                                    this.Selection =  new AudioSelection((PhraseNode) this.Selection.Node, mContentView,
                                                             new AudioRange(GoToDialog.TimeInSeconds));
                                else
                                {
                                    MessageBox.Show(Localizer.Message("time_exceeds_duration_phrase"));
                                    return;
                                }
                            }
                        }
                        else if (GoToDialog.SelectedIndex == 1)
                        {
                            if ((this.Selection.Node is SectionNode || this.Selection is StripIndexSelection) &&
                                Selection.Node.PhraseChildCount >= 1)
                                nodeSel = this.Selection.Node;
                            else if (this.Selection.Node is EmptyNode || this.Selection.Node is PhraseNode)
                                nodeSel = this.Selection.Node.ParentAs<SectionNode>();

                            double selectedNodeDuration = nodeSel != null ? nodeSel.Duration : 0.0;
                            if (selectedNodeDuration == 0 || selectedNodeDuration < GoToDialog.TimeInSeconds)
                            {
                                MessageBox.Show(Localizer.Message("time_exceeds_duration_section"));
                                return;
                            }

                            for (int i = 0; i < nodeSel.PhraseChildCount; i++)
                            {
                                if (time < GoToDialog.TimeInSeconds && nodeSel.PhraseChild(i) is PhraseNode)
                                {
                                    time = nodeSel.PhraseChild(i).Duration + time;
                                    phrNode = (PhraseNode) nodeSel.PhraseChild(i);
                                }
                                else
                                    break;
                            }
                            this.Selection = new AudioSelection((PhraseNode)phrNode, mContentView,
                                                   new AudioRange(GoToDialog.TimeInSeconds - (time - phrNode.Duration)));
                                           
                        }
                        else if (GoToDialog.SelectedIndex == 2)
                        {

                            double dTime = 0.0;
                            PhraseNode nodeToBeSelected = null;

                            this.Presentation.RootNode.AcceptDepthFirst(
                                delegate(urakawa.core.TreeNode n)
                                {
                                    if (nodeToBeSelected != null) return false;
                                    PhraseNode phraseNode = n as PhraseNode;
                                    if (phraseNode != null)
                                    {

                                        if (dTime + phraseNode.Duration < GoToDialog.TimeInSeconds)
                                        {
                                            dTime = (phraseNode).Duration + dTime;
                                        }
                                        else
                                        {
                                            nodeToBeSelected = phraseNode;

                                            return false;
                                        }
                                    }
                                    return true;
                                },
                                delegate(urakawa.core.TreeNode n) { });
                            if (nodeToBeSelected == null) MessageBox.Show("Time value exceeds the project");

                            mContentView.SelectPhraseBlockOrStrip(nodeToBeSelected);
                            Selection = new AudioSelection((PhraseNode)nodeToBeSelected, mContentView,
                                                     new AudioRange(GoToDialog.TimeInSeconds - dTime));
                         }
                    }
                      
                    else
                    {
                        if (Selection == null)
                            MessageBox.Show(Localizer.Message("select_phrase_or_section"));
                        else if (Selection.Node is SectionNode && Selection.Node.PhraseChildCount < 1)
                            MessageBox.Show(Localizer.Message("no_phrases_in_section"));

                    }
                } // dialog OK check ends
            }
        }


        //@singleSection
        public void RecreateContentsWhileInitializingRecording ( EmptyNode recordingResumePhrase ) 
        { 
            mContentView.RecreateContentsWhileInitializingRecording ( recordingResumePhrase );
            if (Selection != null && Selection.Node is SectionNode && Selection.Control is TOCView)
            {
                mTOCView.HighlightNodeWithoutSelection = (SectionNode)Selection.Node;
                if (!mTOCView.ContainsFocus) mTOCView.Focus();
            }
        }

        //@singleSection : workaround to make recording phrases visible when audio media is updated
        public void PostRecording_RecreateInvisibleRecordingPhrases(SectionNode section, int initialIndex, int count) { mContentView.PostRecording_RecreateInvisibleRecordingPhrases(section, initialIndex, count) ; }

        //@singleSection
        public bool IsContentViewScrollActive { get { return mContentView.IsScrollActive; } }

        //@singleSection
        public const string ProgressBar_Command = "command";
        public const string ProgressBar_Navigation = "navigation" ;
        public const string ProgressBar_Waveform = "waveform";

        private delegate void TriggerProgressChangedEventDelegate(string purpose, int progressInPercent);
        //@singleSection
        /// <summary>
        /// < Triggers progress changed event which updates progressbar on obi form.
        /// parameter purpose can be command, navigate , waveform.
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="progressInPercent"></param>
        public void TriggerProgressChangedEvent ( string purpose, int progressInPercent)
            {
                if (InvokeRequired)
                {
                    Invoke(new TriggerProgressChangedEventDelegate(TriggerProgressChangedEvent), purpose, progressInPercent);
                }
                else
                {
                    if (ProgressChanged != null)
                        ProgressChanged(this, new ProgressChangedEventArgs(progressInPercent, purpose));
                }
            }

        //@singleSection
        /// <summary>
        /// returns true if limited phrases in a strip are created and content view is not completely filled
        /// </summary>
        /// <returns></returns>
                public bool IsLimitedPhraseBlocksCreatedAfterCommand() { return mContentView.IsLimitedPhraseBlocksCreatedAfterCommand(); }

        /// <summary>
        /// Work around specificallly for disabling scrolling during some conditions of playback @AudioScrolling
        /// </summary>
        public void DisableScrollingInContentsView ()
            {
            mContentView.DisableScrolling ();
            }

        public void ClearContentView()
        {
            if (mContentView.ActiveStrip != null)
            {
                mContentView.RemoveBlocksInStrip(mContentView.ActiveStrip.Node);
            }
        }

        public void AssociateNodeToSpecialNode()  //@AssociateNode
        {
            if (TransportBar.IsRecorderActive) return;
            if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause();
            Dialogs.AssociateSpecialNode AssociateSpecialNode;
           // if (mSelection.Node is EmptyNode)
            {
                if(mSelection != null && mSelection.Node is EmptyNode)
                AssociateSpecialNode = new Obi.Dialogs.AssociateSpecialNode(((ObiRootNode)mPresentation.RootNode), ((EmptyNode)mSelection.Node));
                else
                AssociateSpecialNode = new Obi.Dialogs.AssociateSpecialNode(((ObiRootNode)mPresentation.RootNode), null);
                if (AssociateSpecialNode.ShowDialog() == DialogResult.OK)
                {
                    foreach (KeyValuePair<EmptyNode, EmptyNode> pair in AssociateSpecialNode.DictionaryToMapValues)
                    {
                          
                        if (pair.Key.AssociatedNode == null
                            || pair.Key.AssociatedNode != pair.Value)
                        {
                            
                            urakawa.command.CompositeCommand cmd = Presentation.CreateCompositeCommand("Associate anchor node");//todo:localize
                            if (pair.Key.AssociatedNode != null) cmd.ChildCommands.Insert(cmd.ChildCommands.Count, new Commands.Node.DeAssociateAnchorNode(this, pair.Key));

                            if (pair.Value != null)
                            {
                                cmd.ChildCommands.Insert(cmd.ChildCommands.Count, new Commands.Node.AssociateAnchorNode(this, pair.Key, pair.Value));
                            }
                            else if (pair.Value == null && pair.Key.AssociatedNode != null)
                            {
                                
                                cmd.ChildCommands.Insert(cmd.ChildCommands.Count, new Commands.Node.DeAssociateAnchorNode(this, pair.Key));
                            }
                            try
                            {
                                Presentation.Do(cmd);
                            }
                            catch (System.Exception ex)
                            {
                                this.WriteToLogFile(ex.ToString());
                                MessageBox.Show(Localizer.Message("ProjectViewFormMsg_AddingReferenceFail") + "\n\n" + ex.ToString());   //@Messagecorrected
                            }
                            //pair.Key.AssociatedNode = pair.Value;                         
                        }
                    }//foreach ends
                }//dialog ok ends

            }
        }

        public void MarkBeginNote()
        {
            mContentView.BeginSpecialNode = Selection.EmptyNodeForSelection; //@AssociateNode
            TransportBar.PlayAudioClue(TransportBar.AudioCluesSelection.SelectionBegin);
        }

        public void MarkEndNote()
        {
            if (mContentView.BeginSpecialNode == null) return;
            mContentView.EndSpecialNode = Selection.EmptyNodeForSelection; //@AssociateNode
            TransportBar.PlayAudioClue(TransportBar.AudioCluesSelection.SelectionEnd);
        }

        public void AssignRoleToMarkedContinuousNodes()  //@AssociateNode
        {
           // m_BeginNote = mContentView.BeginSpecialNode;
            EmptyNode startNode = mContentView.BeginSpecialNode;
           // EmptyNode endNode = Selection.EmptyNodeForSelection;
            EmptyNode endNode = mContentView.EndSpecialNode;
            if (startNode == null || endNode == null) return;
            bool IsSpecialNodeAdded = false;

            string customClass = "";
            List<EmptyNode> listOfEmptyNodesToMarkAsSpecialNodes = new List<EmptyNode>();
            ObiNode parentNode = startNode.ParentAs<SectionNode>();
            //@enabled begin node = end node: following if statement commented
            //if (startNode.Index == endNode.Index)
                //{
                    //MessageBox.Show(Localizer.Message("Start_node_different_from_end_node"));
                    //return;
                //}
            //else if (startNode.Index > endNode.Index)
            if (startNode.Index > endNode.Index)
                {
                    MessageBox.Show(Localizer.Message( "Start_node_index_greater_than_end"));
                    return;
                }
            Dialogs.AssignSpecialNodeMark AssignSpecialNodeDialog = new Obi.Dialogs.AssignSpecialNodeMark();
            AssignSpecialNodeDialog.ShowDialog();
            if (AssignSpecialNodeDialog.DialogResult == DialogResult.OK)
            {
                customClass = AssignSpecialNodeDialog.SelectedSpecialNode;

                if (AssignSpecialNodeDialog.IsRenumberChecked)
                    RenumberPage();
                else
                {
                    if (startNode.Index <= endNode.Index)
                    {
                        //for (int i = startNode.Index; i < endNode.Index; i++) //@enable begin node = end node
                        for (int i = startNode.Index; i <= endNode.Index; i++)
                        {
                            if (parentNode.PhraseChild(i).Role_ == EmptyNode.Role.Custom && parentNode.PhraseChild(i).CustomRole != customClass)
                            {
                                if (!IsSpecialNodeAdded)
                                {
                                    Dialogs.ExtendedMessageToAssociate assignSpecialNodeToChunk = new Obi.Dialogs.ExtendedMessageToAssociate();

                                    if (assignSpecialNodeToChunk.ShowDialog() == DialogResult.Yes)
                                        IsSpecialNodeAdded = assignSpecialNodeToChunk.Is_AssignRole;
                                    else if (assignSpecialNodeToChunk.Is_YesToAll)
                                    {
                                        endNode = (EmptyNode)this.Selection.Node;
                                        break;
                                    }
                                    else if (assignSpecialNodeToChunk.Is_Abort)
                                    {
                                        //endNode = parentNode.PhraseChild(i - 1);//@enabled begin node = end node
                                        return;
                                    }
                                }
                            }
                            else if (parentNode.PhraseChild(i).Role_ == EmptyNode.Role.Heading || parentNode.PhraseChild(i).Role_ == EmptyNode.Role.Silence || parentNode.PhraseChild(i).Role_ == EmptyNode.Role.Page || parentNode.PhraseChild(i).Role_ == EmptyNode.Role.Anchor)
                            {
                                // if (!IsSpecialNodeAdded)
                                {
                                    Dialogs.ExtendedMessageToAssociate assignSpecialNodeToChunk = new Obi.Dialogs.ExtendedMessageToAssociate();

                                    if (assignSpecialNodeToChunk.ShowDialog() == DialogResult.Yes)
                                        IsSpecialNodeAdded = assignSpecialNodeToChunk.Is_AssignRole;
                                    else if (assignSpecialNodeToChunk.Is_YesToAll)
                                    {
                                        endNode = (EmptyNode)this.Selection.Node;
                                        break;
                                    }
                                    else if (assignSpecialNodeToChunk.Is_Abort)
                                    {
                                        //endNode = parentNode.PhraseChild(i - 1);//@enabled begin node = end node
                                        return;
                                    }
                                }
                            }
                            if (parentNode.PhraseChild(i).Index < parentNode.PhraseChildCount - 1 && ((((EmptyNode)parentNode.PhraseChild(i)).Role_ != ((EmptyNode)parentNode.PhraseChild(i + 1)).Role_ && ((EmptyNode)parentNode.PhraseChild(i + 1)).Role_ == EmptyNode.Role.Custom) || ((EmptyNode)parentNode.PhraseChild(i + 1)).Role_ == EmptyNode.Role.Custom) && ((EmptyNode)parentNode.PhraseChild(i)).CustomRole != ((EmptyNode)parentNode.PhraseChild(i + 1)).CustomRole)
                                IsSpecialNodeAdded = false;
                        }
                    }

                    try
                    {
                        Presentation.Do(Commands.Node.AssignRole.GetCompositeCommandForAssigningRoleOnMultipleNodes(this, startNode, endNode, EmptyNode.Role.Custom, customClass));
                    }
                    catch (System.Exception ex)
                    {
                        this.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ProjectViewFormMsg_AssigningSkippableFail") + "\n\n" + ex.ToString());  //@Messagecorrected
                    }
                }
            }
            mContentView.BeginSpecialNode = null;
        }

        public void RenumberPage()
        {
            bool pageFound = false;
            if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause();
            PageNumber num = null;
            for (ObiNode n = BeginNote.PrecedingNode; n != null; n = n.PrecedingNode)
            {
                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                {
                    num = ((EmptyNode)n).PageNumber;
                    break;
                } 
            }
            for (ObiNode n = BeginNote; n != mContentView.EndSpecialNode.FollowingNode; n = n.FollowingNode)
            {
                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                {
                    pageFound = true;

                }
            }
            if (!pageFound)
            {
                MessageBox.Show(Localizer.Message("Page_not_found"));
                return;
            }
            Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber(num, false, false);
            dialog.IsRenumberChecked = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bool renumber = dialog.Renumber;
                PageNumber number = dialog.Number;              
                urakawa.command.Command cmd = new Commands.Node.SetPageNumber(this, SelectedNodeAs<EmptyNode>(), number);
                if (renumber)
                    {
                        CompositeCommand k = Presentation.CreateCompositeCommand(cmd.ShortDescription);
                       
                        for (ObiNode n = BeginNote; n != mContentView.EndSpecialNode.FollowingNode; n = n.FollowingNode )
                        {
                                if (n is EmptyNode &&( (EmptyNode)n).Role_ == EmptyNode.Role.Page &&
                                    ((EmptyNode)n).PageNumber.Kind == number.Kind)
                                {
                                    if (n == mContentView.EndSpecialNode)
                                    {
                                        cmd = new Commands.Node.SetPageNumber(this, SelectedNodeAs<EmptyNode>(), number);
                                       
                                    }
                                    k.ChildCommands.Insert(k.ChildCommands.Count, new Commands.Node.SetPageNumber(this, (EmptyNode)n, number));                                    
                                    number = number.NextPageNumber();
                                }                                
                        }
                        if (mContentView.EndSpecialNode.Role_ == EmptyNode.Role.Page)
                            k.ChildCommands.Insert(k.ChildCommands.Count, cmd);
                        cmd = k;
                    }
                    mPresentation.Do(cmd);        
            }
        }

        public void ClearSkippableChunk()
        {
            
            if (CanClearSkippableRole)
            {
                if (TransportBar.IsPlayerActive) TransportBar.Pause();
                try
                {
                    EmptyNode firstNode = GetSkippableNoteEndNode(true);
                    SectionNode parentSection = this.Selection.Node.ParentAs<SectionNode>();
                    CompositeCommand command = Presentation.CommandFactory.CreateCompositeCommand();
                    EmptyNode anchor = mPresentation.GetAnchorForReferencedNode(firstNode);

                    if (anchor != null && anchor.AssociatedNode != null && (MessageBox.Show(Localizer.Message("ProjectView_ClearAnchor"), Localizer.Message("Caption_Information"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                    {
                        Commands.Node.AssignRole ClearRoleCmd = new Commands.Node.AssignRole(this, anchor, EmptyNode.Role.Plain);
                        command.ChildCommands.Insert(command.ChildCommands.Count, ClearRoleCmd);
                    }
                    //anchor.AssociatedNode = null;
                    if (firstNode != null)
                    {                        
                        command.ShortDescription = "Remove skippable role from the chunk";
                        for (int i = firstNode.Index; i < parentSection.PhraseChildCount; i++)
                        {
                            EmptyNode node = parentSection.PhraseChild(i);
                            if (firstNode.CustomRole != node.CustomRole) break;

                            Commands.Node.AssignRole ClearRoleCmd = new Commands.Node.AssignRole(this, node, EmptyNode.Role.Plain);
                            command.ChildCommands.Insert(command.ChildCommands.Count, ClearRoleCmd);

                            if (!node.Used) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed(this, node));
                        }
                        if (command.ChildCommands.Count > 0 )  Presentation.Do(command);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void DeassociateSpecialNode()  //@AssociateNode
        {
            if(Selection.Node != null)
            {
                try
                {
                    Presentation.Do(new Commands.Node.DeAssociateAnchorNode(this, (EmptyNode)this.Selection.Node));
                }
                catch (System.Exception e)
                {
                    this.WriteToLogFile(e.ToString());
                    MessageBox.Show("ProjectViewMsg_DeassociatingSkippableFail" + "\n\n" + e.ToString()); //@Messagecorrected
                }
            }
        }

        public bool GotoSkippableNoteEnds(bool GotoBegin)  //@AssociateNode
        {
            EmptyNode endNode = GetSkippableNoteEndNode(GotoBegin) ;
            if ( endNode != null)
    {
        SelectedBlockNode = endNode  ;
        return true;
    }
    return false;
        }

        public EmptyNode GetSkippableNoteEndNode(bool GotoBegin)  //@AssociateNode
        {
            if (Selection == null || !(Selection.Node is EmptyNode)) return null;
            if (((EmptyNode)Selection.Node).Role_ != EmptyNode.Role.Custom || !EmptyNode.SkippableNamesList.Contains(((EmptyNode)Selection.Node).CustomRole)) return null;
            SectionNode parentSection = this.Selection.Node.ParentAs<SectionNode>();
            if (GotoBegin)
            { 
                for(int i = this.Selection.Node.Index; i>= 0; i--)
                {
                    if (i == 0)
                    {
                        return parentSection.PhraseChild(i);
                    }
                    else
                    {
                        if (parentSection.PhraseChild(i).Role_ != parentSection.PhraseChild(i - 1).Role_ || parentSection.PhraseChild(i).CustomRole != parentSection.PhraseChild(i - 1).CustomRole)
                        {
                            return parentSection.PhraseChild(i);

                        }
                    }
                }
            }
            else
            {
                for(int i = Selection.Node.Index; i< parentSection.PhraseChildCount ;i++)
                {
                    if (i == parentSection.PhraseChildCount - 1)
                     { 
                        return parentSection.PhraseChild(i); 
                    }
                    else
                    {
                        if (parentSection.PhraseChild(i).Role_ != parentSection.PhraseChild(i + 1).Role_ || parentSection.PhraseChild(i).CustomRole != parentSection.PhraseChild(i + 1).CustomRole)
                        {
                            return parentSection.PhraseChild(i);
                            
                        }
                    }
                }
            }
            return null;
        }

        public void ExportAudioOfSelectedNode()
        {
            if (!CanExportSelectedNodeAudio) return;
            string audioFileExportDirectory = ObiForm.ExportAudioDirectory;
            ObiNode nodeSelected = this.Selection.Node;
            double durationOfSelection = nodeSelected is PhraseNode ? nodeSelected.Duration : 0;
            if (nodeSelected is SectionNode)
            {
                for (int i = 0; i < ((SectionNode)nodeSelected).PhraseChildCount; i++) durationOfSelection += ((SectionNode)nodeSelected).PhraseChild(i).Duration;
            }
            if (durationOfSelection == 0)
            {
                MessageBox.Show(Localizer.Message("no_audio"));
                return;
            }
            if (!audioFileExportDirectory.EndsWith("\\")) audioFileExportDirectory = audioFileExportDirectory + "\\";
            try
            {
                
                if (!System.IO.Directory.Exists(audioFileExportDirectory)) System.IO.Directory.CreateDirectory(audioFileExportDirectory);
                TreeNodeTestDelegate nodeIsSection = delegate(urakawa.core.TreeNode node) { return node is SectionNode; };
                TreeNodeTestDelegate nodeIsOtherSection = delegate(urakawa.core.TreeNode node) { return (nodeSelected is SectionNode &&  node is SectionNode && node != nodeSelected ) ; };

                mPresentation.RemoveAllPublishChannels(); // remove any publish channel, in case they exist

                PublishFlattenedManagedAudioVisitor visitor = new PublishFlattenedManagedAudioVisitor(nodeIsSection, nodeIsOtherSection);
                
                
                //urakawa.property.channel.Channel publishChannel = mPresentation.AddChannel(ObiPresentation.PUBLISH_AUDIO_CHANNEL_NAME);

                Channel publishChannel = mPresentation.ChannelFactory.CreateAudioChannel();
                publishChannel.Name= ObiPresentation.PUBLISH_AUDIO_CHANNEL_NAME;
                
                visitor.DestinationChannel= publishChannel;
                visitor.SourceChannel = mPresentation.ChannelsManager.GetOrCreateAudioChannel();
                visitor.DestinationDirectory = new Uri(audioFileExportDirectory);

                visitor.EncodePublishedAudioFilesToMp3 = false;
                visitor.EncodePublishedAudioFilesStereo = mPresentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 2;
                uint sampleRate = mPresentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate;
                if (sampleRate == 44100) visitor.EncodePublishedAudioFilesSampleRate = SampleRate.Hz44100;
                else if (sampleRate == 22050) visitor.EncodePublishedAudioFilesSampleRate = SampleRate.Hz22050;
                else if (sampleRate == 11025) visitor.EncodePublishedAudioFilesSampleRate = SampleRate.Hz11025;
                visitor.DisableAcmCodecs = true;

                Obi.Dialogs.ProgressDialog progress = new Obi.Dialogs.ProgressDialog(Localizer.Message("AudioFileExport_progress_dialog_title"),
                            delegate(Dialogs.ProgressDialog progress1)
                            {
                nodeSelected.AcceptDepthFirst(visitor);
            });
                progress.OperationCancelled += new Obi.Dialogs.OperationCancelledHandler(delegate(object sender, EventArgs e) { visitor.RequestCancellation = true;});
                visitor.ProgressChangedEvent += new ProgressChangedEventHandler(progress.UpdateProgressBar);
                progress.ShowDialog();
                if (progress.Exception != null) throw progress.Exception;
                        
                //sdk2 TODO check that there is an audio file to write
                //visitor.WriteAndCloseCurrentAudioFile();

                mPresentation.ChannelsManager.RemoveManagedObject(publishChannel);

                //rename the audio file to relevant name
                string audioFilePath = System.IO.Path.Combine(audioFileExportDirectory, "aud001.wav");
                if (System.IO.File.Exists(audioFilePath))
                {
                    string newName = nodeSelected is SectionNode ? ((SectionNode)nodeSelected).ToString() + ".wav":
                        nodeSelected is PhraseNode ? ((EmptyNode)nodeSelected).ParentAs<SectionNode>().Label + ((EmptyNode)nodeSelected).ParentAs<SectionNode>().Position + "-" + ((EmptyNode)nodeSelected).ToString() + ".wav":
                        null;
                    newName = Obi.Program.SafeName(newName);
                        string newAudioFilePath = System.IO.Path.Combine(audioFileExportDirectory, newName);
                        if (System.IO.File.Exists(newAudioFilePath)) System.IO.File.Delete(newAudioFilePath);
                        System.IO.File.Move(audioFilePath, newAudioFilePath);
                        MessageBox.Show(Localizer.Message("ExportAudioOfSelectedNode_Completed") + newAudioFilePath, Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);                   
                }
            }
            catch (System.Exception ex)
            {
                this.WriteToLogFile(ex.ToString());
                MessageBox.Show(Localizer.Message("ProjectViewMsg_ExportingAudioFail") + "\n\n" + ex.ToString());   //@Messagecorrected
            }
        }

        
        public void ReplaceAudioOfSelectedNode()
        {
            if (CanExportSelectedNodeAudio)
            {
                try
                {
                    ObiNode node = Selection.Node;
                    List<EmptyNode> originalPhrases = new List<EmptyNode>();
                    List<double> originalTimings = new List<double>();

                    if (node is PhraseNode)
                    {
                        originalPhrases.Add((PhraseNode)node);
                        originalTimings.Add(((PhraseNode)node).Duration);
                    }
                    else if (node is SectionNode)
                    {
                        SectionNode section = (SectionNode)node;
                        double duration = 0;
                        for (int i = 0; i < section.PhraseChildCount; i++)
                        {
                            originalPhrases.Add(section.PhraseChild(i));
                            if (section.PhraseChild(i) is PhraseNode) originalTimings.Add(duration += ((PhraseNode)section.PhraseChild(i)).Duration);
                        }
                    }

                    if (originalPhrases.Count == 0) return;
                    //select audio file
                    OpenFileDialog openAudioFileDialog = new OpenFileDialog();
                    openAudioFileDialog.Filter = "*.wav|*.WAV";
                    openAudioFileDialog.Multiselect = false;
                    if (openAudioFileDialog.ShowDialog() == DialogResult.OK && System.IO.File.Exists(openAudioFileDialog.FileName))
                    {
                        urakawa.media.data.audio.AudioMediaData asset =
                            (urakawa.media.data.audio.AudioMediaData)mPresentation.MediaDataFactory.Create<urakawa.media.data.audio.codec.WavAudioMediaData>();
                        urakawa.media.data.audio.ManagedAudioMedia media = (urakawa.media.data.audio.ManagedAudioMedia)mPresentation.MediaFactory.CreateManagedAudioMedia();
                        media.MediaData = asset;

                        urakawa.data.FileDataProvider dataProv = (urakawa.data.FileDataProvider)media.Presentation.DataProviderFactory.Create(urakawa.data.DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                        dataProv.InitByCopyingExistingFile(openAudioFileDialog.FileName);
                        media.AudioMediaData.AppendPcmData(dataProv);

                        if (media.Duration.AsMilliseconds < originalTimings[originalTimings.Count - 1] - 100)
                        {
                            MessageBox.Show(string.Format(Localizer.Message("ProjectView_ErrorReplacingAudioDueToTimingMissmatch"), originalTimings[originalTimings.Count - 1], media.Duration.AsMilliseconds));
                            media = null;
                            return;
                        }

                        CompositeCommand updateAudioCommand = mPresentation.CreateCompositeCommand("Update audio for phrases");
                        if (originalPhrases.Count == 1)
                        {
                            PhraseNode phrase = (PhraseNode)originalPhrases[0];
                            //((PhraseNode)originalPhrases[0]).Audio = media;
                            updateAudioCommand.ChildCommands.Insert(updateAudioCommand.ChildCommands.Count,
                                        new Commands.Node.UpdateAudioMedia(this, phrase, media, false));
                        }
                        else
                        {
                            List<urakawa.media.data.audio.ManagedAudioMedia> mediaDataList = new List<urakawa.media.data.audio.ManagedAudioMedia>();

                            for (int i = originalTimings.Count - 2; i >= 0; --i)
                            {
                                if (originalTimings[i] < media.Duration.AsMilliseconds)
                                {
                                    urakawa.media.data.audio.ManagedAudioMedia split = media.Split(new urakawa.media.timing.Time(Convert.ToInt64(originalTimings[i] * urakawa.media.timing.Time.TIME_UNIT)));
                                    mediaDataList.Insert(0, split);
                                }
                            }// split loop ends
                            mediaDataList.Insert(0, media);
                            int mediaIndex = 0;

                            for (int i = 0; i < originalPhrases.Count; i++)
                            {
                                if (originalPhrases[i] is PhraseNode)
                                {
                                    PhraseNode phrase = (PhraseNode)originalPhrases[i];
                                    //if (phrase.Duration != mediaDataList[mediaIndex].Duration.AsMilliseconds) 
                                    //MessageBox.Show(i.ToString () + "Error in timings: " + phrase.Duration.ToString() + ":" + mediaDataList[mediaIndex].Duration.AsMilliseconds.ToString());
                                    //phrase.Audio = mediaDataList[mediaIndex];
                                    updateAudioCommand.ChildCommands.Insert(updateAudioCommand.ChildCommands.Count,
                                        new Commands.Node.UpdateAudioMedia(this, phrase, mediaDataList[mediaIndex], false));
                                    mediaIndex++;
                                }
                            }// update audio for loop
                        }
                            if(updateAudioCommand.ChildCommands.Count > 0 )  mPresentation.Do(updateAudioCommand);
                        
                    }
                }//try
                catch (System.Exception ex)
                {
                    WriteToLogFile(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }//CanReplace check
        }

        public bool IsWaveformRendering { get { return mContentView.IsWaveformRendering; } }
        public void WaveformRendering_PauseOrResume(bool pause) { mContentView.WaveformRendering_PauseOrResume (pause ) ;}

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


        public void ShowSpecialPhraseList()
        {
            if (mPresentation == null) return;
            if(mTransportBar.IsPlayerActive)  mTransportBar.Stop();
            Dialogs.SpecialPhraseList dialogs = new Dialogs.SpecialPhraseList(this);
            if (dialogs.ShowDialog() == DialogResult.OK)
            {
                SelectedBlockNode = dialogs.SpecialPhraseSelected;
            }
        }

        /// <summary>
        /// <Find phrases with negligible audio or with corrupt data providers and replace them with empty phrases
        /// </summary>
        public void ReplacePhrasesWithImproperAudioWithEmptyPhrases(ObiNode node, bool isThorough)
        {
            if (mPresentation == null) return;
            if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
            List<EmptyNode> phrasesToReplace = new List<EmptyNode>();
            int totalPhrasesCount = 0;
            node.AcceptDepthFirst(
                            delegate(urakawa.core.TreeNode n)
                            {
                                if (n is PhraseNode)
                                {
                                    totalPhrasesCount++;
                                    PhraseNode phrase = (PhraseNode)n;
                                    if (phrase.Audio == null || phrase.Audio.Duration.AsMilliseconds < 10)
                                    {
                                        phrasesToReplace.Add(phrase);
                                    }
                                    // apply this stream check only if requested explicitly because it is time consuming.
                                    else if (isThorough)
                                    {
                                        System.IO.Stream checkingStream = null;

                                        try
                                        {
                                            checkingStream = phrase.Audio.AudioMediaData.OpenPcmInputStream();

                                        }
                                        catch (System.Exception ex)
                                        {
                                            Console.WriteLine("exception thrown by " + phrase.ToString());
                                            Console.WriteLine(ex.ToString());
                                            phrasesToReplace.Add(phrase);
                                        }
                                        finally
                                        {
                                            if (checkingStream != null) checkingStream.Close();
                                            checkingStream = null;
                                        }
                                    }
                                }

                                return true;
                            },
                            delegate(urakawa.core.TreeNode n) { }
                        );
            float percentDefect = totalPhrasesCount > 0 ? (phrasesToReplace.Count * 100) / totalPhrasesCount: 0f;
            if (phrasesToReplace.Count > 0
                && MessageBox.Show(string.Format(Localizer.Message("ReplaceCorruptPhrasesWithEmptyNode_Question"), totalPhrasesCount, phrasesToReplace.Count),
                Localizer.Message("Caption_Warning"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, 
                (percentDefect < 10? MessageBoxDefaultButton.Button1: MessageBoxDefaultButton.Button2)) == DialogResult.Yes)
            {
                CompositeCommand replacePhrasesCommand = mPresentation.CreateCompositeCommand(Localizer.Message("ReplacePhrasesWithEmptyNode"));
                for (int i = 0; i < phrasesToReplace.Count; i++)
                {
                    EmptyNode empty = mPresentation.TreeNodeFactory.Create<EmptyNode>();
                    empty.CopyAttributes(phrasesToReplace[i]);
                    Commands.Node.Delete deleteCmd = new Obi.Commands.Node.Delete(this, phrasesToReplace[i], false);
                    replacePhrasesCommand.ChildCommands.Insert(replacePhrasesCommand.ChildCommands.Count, deleteCmd);
                    Commands.Node.AddNode add = new Obi.Commands.Node.AddNode(this, empty, phrasesToReplace[i].ParentAs<SectionNode>(), phrasesToReplace[i].Index, false);
                    replacePhrasesCommand.ChildCommands.Insert(replacePhrasesCommand.ChildCommands.Count, add);
                    Commands.Node.ToggleNodeTODO todoMarkCmd = new Obi.Commands.Node.ToggleNodeTODO(this, empty);
                    todoMarkCmd.UpdateSelection = false;
                    replacePhrasesCommand.ChildCommands.Insert(replacePhrasesCommand.ChildCommands.Count, todoMarkCmd);
                }

                //execute the command
                ExecuteReplacePhrasesCommandWithCallBack(replacePhrasesCommand);
            }
            else if (phrasesToReplace.Count == 0)
            {
                MessageBox.Show(Localizer.Message("ReplaceCorruptPhrasesWithEmptyNode_NothingFound"));
            }
        }

        private delegate void ReplacePhrasesCommandDelegate(CompositeCommand cmd);
        private void ExecuteReplacePhrasesCommandWithCallBack(CompositeCommand cmd)
        {
            if (InvokeRequired)
            {
                Invoke(new ReplacePhrasesCommandDelegate(ExecuteReplacePhrasesCommandWithCallBack), cmd);
            }
            else
            {
                try
                {
                    mPresentation.Do(cmd);
                }
                catch (System.Exception ex)
                {
                    this.WriteToLogFile(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }
            
        }

        public void SplitAndMerge(bool mergeWithNext)
        {
            if (CanSplitPhrase)
            {
                try
                {
                    mPresentation.Do(GetSplitAndMergeCommand(mergeWithNext));
                }
                catch (System.Exception ex)
                {
                    WriteToLogFile(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private CompositeCommand GetSplitAndMergeCommand(bool mergeWithNext)
        {
            PhraseNode selectedPhrase =mTransportBar.IsPlayerActive  && mTransportBar.PlaybackPhrase != null? mTransportBar.PlaybackPhrase:
                Selection != null && Selection is AudioSelection? (PhraseNode)Selection.Node:
                null;
            Console.WriteLine("Split Merge : " + selectedPhrase);
            if (selectedPhrase == null) return null;

            double time = -1 ;
            if (mTransportBar.IsPlayerActive && mTransportBar.PlaybackPhrase !=null)
            {
                time = mTransportBar.CurrentPlaylist.CurrentTimeInAsset ;
            }
            else if ( Selection != null && Selection is AudioSelection)
            {
                AudioSelection audioSel = (AudioSelection)Selection;
                if ( audioSel.AudioRange.HasCursor)
                {
                    time = audioSel.AudioRange.CursorTime ;
                }
                else
                {
                    time = mergeWithNext? audioSel.AudioRange.SelectionBeginTime: 
                        audioSel.AudioRange.SelectionEndTime ;
                }
            }
            Console.WriteLine("Split Merge : " + time);

            CompositeCommand splitMergeCmd = mPresentation.CreateCompositeCommand("Split & merge command");

            Commands.Node.SplitAudio splitCmd = Commands.Node.SplitAudio.AppendSplitCommandWithProperties(this, splitMergeCmd, selectedPhrase, time, false);
            PhraseNode newPhrase = splitCmd.NodeAfter;
            //MessageBox.Show(newPhrase.ToString());
                //splitMergeCmd.ChildCommands.Insert(splitMergeCmd.ChildCommands.Count, splitCmd);

                if (mergeWithNext)
                {
                    Console.WriteLine("Split Merge : bool:  " + mergeWithNext);
                    ObiNode nextNode = selectedPhrase.FollowingNode;
                    if ( nextNode != null && nextNode is EmptyNode && nextNode.ParentAs<SectionNode>() == selectedPhrase.ParentAs<SectionNode>())
                    {
                        //Console.WriteLine("Split Merge : " + newPhrase);
                        splitMergeCmd.ChildCommands.Insert(splitMergeCmd.ChildCommands.Count,
                            Commands.Node.MergeAudio.GetMergeCommand(this, newPhrase,(EmptyNode) nextNode) ) ;
                    }
                }
                else
                {
                    ObiNode prevNode = selectedPhrase.PrecedingNode;
                    if (prevNode != null && prevNode is EmptyNode && prevNode.ParentAs<SectionNode>() == selectedPhrase.ParentAs<SectionNode>())
                    {
                        splitMergeCmd.ChildCommands.Insert(splitMergeCmd.ChildCommands.Count,
                            Commands.Node.MergeAudio.GetMergeCommand(this, (EmptyNode)prevNode, selectedPhrase));
                    }
                }


            return splitMergeCmd;
        }
        public string SelectProjectFile()
        {
             OpenFileDialog dialog=new OpenFileDialog();
                dialog.Filter = Localizer.Message("Obi_ProjectMergeFilter");
                dialog.Title = Localizer.Message("Obi_ProjectMerge");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
                else
                {
                    return null;
                }
           
        }
        public void MergeProject (Session session)
        {
            if (MessageBox.Show(Localizer.Message("MergeProject_Information"),
                Localizer.Message("Caption_Information"),
                MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return;
            }
            string projectFile = SelectProjectFile();
            List<string> sourceProjectPaths=null;
            if (projectFile != null)
            {
                Dialogs.MergeProject dialog =
                           new Dialogs.MergeProject(projectFile);

                if (dialog.ShowDialog() == DialogResult.OK)
                {   
                     sourceProjectPaths = new List<string>(dialog.FilesPaths);

                }
            }
            if (sourceProjectPaths == null || sourceProjectPaths.Count == 0 ) return ;            
            SplitMergeProject splitMerge = null;
            List<List<SectionNode>> sectionsToMerge = new List<List<SectionNode>>();
            try
            {
                for (int i = 0; i < sourceProjectPaths.Count; i++)
                {
                    splitMerge = new SplitMergeProject(session, sourceProjectPaths[i]);
                    Dialogs.ProgressDialog progress =
                                    new Obi.Dialogs.ProgressDialog(string.Format( Localizer.Message("MergeProject_progress_dialog_title"), (i+1).ToString () , sourceProjectPaths.Count.ToString()),
                                                       delegate(Dialogs.ProgressDialog progress1)
                                                       {
                
                                                           splitMerge.DoWork();
                                                           if (splitMerge.SectionsToMerge != null && splitMerge.SectionsToMerge.Count > 0)
                                                           {
                                                               sectionsToMerge.Add(splitMerge.SectionsToMerge);
                                                           }

                                                       });

                    progress.OperationCancelled +=
                        new Obi.Dialogs.OperationCancelledHandler(
                            delegate(object sender, EventArgs e) { splitMerge.RequestCancellation = true; });
                    if (splitMerge != null) splitMerge.ProgressChangedEvent +=
                        new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                    progress.ShowDialog();
                    if (progress.Exception != null) throw progress.Exception;

                }
                if (sectionsToMerge == null || sectionsToMerge.Count == 0)
                {
                    MessageBox.Show(Localizer.Message("MergeProject_NoSectionToMerge")) ;
                    return;
                }

                CompositeCommand mergeProjectCommand = mPresentation.CreateCompositeCommand(Localizer.Message("MergeProjectCommand")) ;
                mergeProjectCommand.ChildCommands.Insert(mergeProjectCommand.ChildCommands.Count,
                   new Commands.UpdateSelection(this, Selection));
                ObiRootNode root =(ObiRootNode) mPresentation.RootNode;
                int sectionInsertionIndex = 0;
                for( int i=0 ; i < sectionsToMerge.Count; i++ )
                {
                    List<SectionNode> currentSectionsList = sectionsToMerge[i] ;
                    for (int j = 0; j < currentSectionsList.Count; j++)
                    {
                        SectionNode section = currentSectionsList[j];
                        mergeProjectCommand.ChildCommands.Insert(mergeProjectCommand.ChildCommands.Count,
                            new Commands.Node.AddNode(this, section, root, root.SectionChildCount + sectionInsertionIndex, false));
                        sectionInsertionIndex++;
                    }
            }
            if (mergeProjectCommand.ChildCommands.Count > 0) mPresentation.Do(mergeProjectCommand);

            for (int i = 0; i < sectionsToMerge.Count; i++)
            {
                List<SectionNode> currentSectionsList = sectionsToMerge[i] ;
                if(currentSectionsList.Count == 0 ) continue ;
                int firstOffset = currentSectionsList[0].Index ;
                
                foreach (SectionNode section in currentSectionsList)
                {
                    section.AcceptDepthFirst(delegate(urakawa.core.TreeNode n)
                {   
                    //for (int j = 0; j < section.PhraseChildCount; j++)
                        //section.PhraseChild(j).AssignAssociatedNodeByParsingLocationString(firstOffset);
                    if ( n is EmptyNode)
                    {
                        ((EmptyNode)n).AssignAssociatedNodeByParsingLocationString(firstOffset);
                    }
                    return true;
                }, delegate(urakawa.core.TreeNode n) { });


                }
            }
            }
                catch (System.Exception ex)
            {
                    WriteToLogFile(ex.ToString ()) ;
                    MessageBox.Show(ex.ToString ()) ;
                }
        }

        public void WriteToLogFile(string msg)
        {
            WriteToLogFile_Static(msg);
        }

        public static void WriteToLogFile_Static (string msg)
        {
            try
            {

                if (!System.IO.File.Exists(m_LogFilePath))
                {
                    System.IO.File.Create(m_LogFilePath).Close();
                }
                System.IO.FileStream fs = new System.IO.FileStream(m_LogFilePath, System.IO.FileMode.Append);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                sw.Write(sw.NewLine);
                sw.Write(msg);
                sw.Close();
                sw = null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Failed to write in log file " + "\n" + ex.ToString());
            }
        }

        public void RenameLogFileAfterSession()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_LogFilePath)
                    && System.IO.File.Exists(m_LogFilePath))
                {
                    // first delete previous log file
                    if (!string.IsNullOrEmpty(m_LogFilePathPrev) && System.IO.File.Exists(m_LogFilePathPrev))
                    {
                        System.IO.File.Delete(m_LogFilePathPrev);
                    }
                    System.IO.File.Move(m_LogFilePath, m_LogFilePathPrev);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Failed to reset log file " + "\n" + ex.ToString());
            }
        }

        public void VerifyLogFileExistenceWhileStartup()
        {
            if (System.IO.File.Exists(m_LogFilePath))
            {
                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string destinationPath = System.IO.Path.Combine(desktopPath, System.IO.Path.GetFileName(m_LogFilePath));
                    if (System.IO.File.Exists(destinationPath)) System.IO.File.Delete(destinationPath);
                    System.IO.File.Move(m_LogFilePath, destinationPath);
                    MessageBox.Show(Localizer.Message( "ProjectViewMsg_LogFileCreatedOnDesktop"),
                        Localizer.Message("Caption_Warning"));
                    //if (MessageBox.Show("The previous session of Obi was terminated abruptly. The log file ObiSession.log is created on the desktop. To report this error via emnail please pressYes else press No to continue",
                        //Localizer.Message("Caption_Warning"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //{
                        
                    //}
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }

        }

    public class ImportingFileEventArgs
        {
        public string Path;  // path of the file being imported
        public ImportingFileEventArgs ( string path ) { Path = path; }
        }

    public delegate void ImportingFileEventHandler ( object sender, ImportingFileEventArgs e );
    }
