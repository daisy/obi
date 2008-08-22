using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class ProjectView : UserControl
    {
        private Presentation mPresentation;  // presentation
        private NodeSelection mSelection;    // currently selected node
        private Clipboard mClipboard;        // the clipboard
        private bool mShowOnlySelected;      // show only selected section
        private bool mSynchronizeViews;      // synchronize views flag
        private ObiForm mForm;               // parent form
        private bool mTOCViewVisible;        // keep track of the TOC view visibility (don't reopen it accidentally)
        private bool mMetadataViewVisible;   // keep track of the Metadata view visibility
        private Timer mTabbingTimer;         // ???

        public event EventHandler SelectionChanged;             // triggered when the selection changes
        public event EventHandler FindInTextVisibilityChanged;  // triggered when the search bar is shown or hidden
        public event ImportingFileEventHandler ImportingFile;   // triggered when a file is being imported
        public event EventHandler FinishedImportingFiles;       // triggered when all files were imported


        /// <summary>
        /// Create a new project view with no project yet.
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();
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
        }


        /// <summary>
        /// Add a new empty block.
        /// </summary>
        public void AddEmptyBlock()
        {
            if (CanAddEmptyBlock)
            {
                EmptyNode node = new EmptyNode(mPresentation);
                ObiNode parent = mContentView.Selection.ParentForNewNode(node);
                AddUnusedAndExecute(new Commands.Node.AddEmptyNode(this, node, parent, mContentView.Selection.IndexForNewNode(node)),
                    node, parent);
            }
        }

        /// <summary>
        /// Add new empty pages.
        /// </summary>
        public void AddEmptyPages()
        {
            if (CanAddEmptyBlock)
            {
                Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber(NextPageNumber, false, true);
                if (dialog.ShowDialog() == DialogResult.OK) AddPageRange(dialog.Number, dialog.NumberOfPages, dialog.Renumber);
            }
        }

        /// <summary>
        /// Get the list of names of currently addable metadata entries.
        /// </summary>
        public List<string> AddableMetadataNames
        {
            get
            {
                List<string> addable = new List<string>();
                foreach (MetadataEntryDescription d in MetadataEntryDescription.GetDAISYEntries().Values)
                {
                    if (mMetadataView.CanAdd(d)) addable.Add(d.Name);
                }
                return addable;
            }
        }

        /// <summary>
        /// Add a new metadata entry to the project
        /// </summary>
        public void AddMetadataEntry()
        {
            if (CanAddMetadataEntry()) mPresentation.getUndoRedoManager().execute(new Commands.Metadata.AddEntry(this));
        }

        /// <summary>
        /// Add a new metadata entry to the project
        /// </summary>
        public urakawa.metadata.Metadata AddMetadataEntry(string name)
        {
            Commands.Metadata.AddEntry cmd = new Commands.Metadata.AddEntry(this, name);
            mPresentation.getUndoRedoManager().execute(cmd);
            return cmd.Entry;
        }

        /// <summary>
        /// Add a new section or strip.
        /// </summary>
        public void AddSection()
        {
            if (mContentView.CanAddStrip)
            {
                                                AddStrip();
            }
            else if (CanAddSection)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                Commands.Node.AddSectionNode add = new Commands.Node.AddSectionNode(this, mTOCView);
                AddUnusedAndExecute(add, add.NewSection, add.NewSectionParent);
            }
        }

        /// <summary>
        /// Add a new strip to the project.
        /// </summary>
        private void AddStrip()
        {
            if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                        Commands.Node.AddSectionNode add = new Commands.Node.AddSectionNode(this, mContentView);
            urakawa.undo.CompositeCommand command = mPresentation.CreateCompositeCommand(add.getShortDescription());
            SectionNode selected = null;
            if (mContentView.Selection.Node is SectionNode)
                selected = (SectionNode)mContentView.Selection.Node;
            else
                selected = (SectionNode)mContentView.Selection.Node.ParentAs<SectionNode>();
            
            command.append(add);
            for (int i = selected.SectionChildCount - 1; i >= 0; --i)
            {
                SectionNode child = selected.SectionChild(i);
                
                command.append(new Commands.Node.Delete(this, child));
                command.append(new Commands.Node.AddNode(this, child, add.NewSection, 0));
            }
            //command.append(add);
            if (!add.NewSectionParent.Used) AppendMakeUnused(command, add.NewSection);
                        mPresentation.getUndoRedoManager().execute(command);
                    }

        /// <summary>
        /// Insert a new subsection in the book as the last child of the selected section node in the TOC view.
        /// </summary>
        public void AddSubSection()
        {
            if (CanAddSubSection)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                                Commands.Node.AddSubSection add = new Commands.Node.AddSubSection(this);
                AddUnusedAndExecute(add, add.NewSection, add.NewSectionParent);
            }
        }

        /// <summary>
        /// Append commands to make a node and its descendants unused to a composite command.
        /// This is used when adding new nodes under unused nodes.
        /// </summary>
        public void AppendMakeUnused(urakawa.undo.CompositeCommand command, ObiNode node)
        {
            node.acceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is ObiNode && ((ObiNode)n).Used) command.append(new Commands.Node.ToggleNodeUsed(this, (ObiNode)n));
                    return true;
                }, delegate(urakawa.core.TreeNode n) { }
            );
        }

        public bool CanAddEmptyBlock { get { return mContentView.Selection != null; } }
        public bool CanAddMetadataEntry() { return mPresentation != null; }
        public bool CanAddMetadataEntry(MetadataEntryDescription d) { return mMetadataView.CanAdd(d); }
        public bool CanAddSection { get { return mPresentation != null && (mTOCView.CanAddSection || mContentView.CanAddStrip); } }
        public bool CanAddSubSection { get { return (mTOCView.CanAddSection && mTOCView.Selection != null) ; } }
        public bool CanAssignRole { get { return IsBlockSelected; } }
        public bool CanClearRole { get { return IsBlockSelected && ((EmptyNode)mSelection.Node).NodeKind != EmptyNode.Kind.Plain; } }
        public bool CanCopy { get { return mPresentation != null && (CanCopySection || CanCopyStrip || CanCopyBlock || CanCopyAudio) && !TransportBar.IsRecorderActive; } }
        public bool CanCopyAudio { get { return mContentView.CanCopyAudio && !TransportBar.IsRecorderActive; } }
        public bool CanCopySection { get { return mTOCView.CanCopySection && !TransportBar.IsRecorderActive; } }
        public bool CanCopyStrip { get { return mContentView.CanCopyStrip && !TransportBar.IsRecorderActive; } }
        public bool CanCopyBlock { get { return mContentView.CanCopyBlock && !TransportBar.IsRecorderActive; } }
        public bool CanCut { get { return CanDelete; } }
        public bool CanDecreaseLevel { get { return mTOCView.CanDecreaseLevel; } }
        public bool CanDelete { get { return mPresentation != null && Selection != null && (CanRemoveSection || CanRemoveStrip || CanRemoveBlock || CanRemoveAudio || CanRemoveMetadata) && !TransportBar.IsRecorderActive; } }
        public bool CanFastForward { get { return mTransportBar.CanFastForward; } }
        public bool CanFocusOnContentView { get { return mPresentation != null && !mContentView.Focused; } }
        public bool CanFocusOnTOCView { get { return mPresentation != null && !mTOCView.Focused; } }
        public bool CanIncreaseLevel { get { return mTOCView.CanIncreaseLevel; } }
        public bool CanInsertSection { get { return CanInsertStrip || mTOCView.Selection != null && !TransportBar.IsRecorderActive; } }
        public bool CanInsertStrip { get { return mContentView.Selection != null && !TransportBar.IsRecorderActive; } }
        public bool CanMergeStripWithNext { get { return mContentView.CanMergeStripWithNext && !TransportBar.IsRecorderActive; } }
        public bool CanNavigateNextPage { get { return mTransportBar.CanNavigateNextPage; } }
        public bool CanNavigateNextPhrase { get { return mTransportBar.CanNavigateNextPhrase; } }
        public bool CanNavigateNextSection { get { return mTransportBar.CanNavigateNextSection; } }
        public bool CanNavigatePrevPage { get { return mTransportBar.CanNavigatePrevPage; } }
        public bool CanNavigatePrevPhrase { get { return mTransportBar.CanNavigatePrevPhrase; } }
        public bool CanNavigatePrevSection { get { return mTransportBar.CanNavigatePrevSection; } }
        public bool CanPaste { get { return mPresentation != null && mSelection != null && mSelection.CanPaste(mClipboard) && !TransportBar.IsRecorderActive; } }
        public bool CanPasteBefore { get { return mPresentation != null && mTOCView.CanPasteBefore(mClipboard) && !TransportBar.IsRecorderActive; } }
        public bool CanPasteInside { get
        { return mPresentation != null && mTOCView.CanPasteInside(mClipboard) && !TransportBar.IsRecorderActive; } }
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
        public bool CanSetStripUsedStatus { get { return mContentView.CanSetStripUsedStatus; } }
        public bool CanSetSelectedNodeUsedStatus { get { return CanSetSectionUsedStatus || CanSetBlockUsedStatus || CanSetStripUsedStatus; } }
        public bool CanShowOnlySelectedSection { get { return SelectedNodeAs<ObiNode>() != null; } }
        public bool CanSplitStrip { get { return mContentView.CanSplitStrip && !TransportBar.IsRecorderActive; } }
        public bool CanStop { get { return mTransportBar.CanStop; } }
        public bool CanApplyPhraseDetection { get { return mPresentation != null && Selection != null && Selection.Node is PhraseNode && !TransportBar.IsRecorderActive; } }
        public bool CanCropPhrase
        {
            get
            {
                return Selection != null && Selection is AudioSelection && !((AudioSelection)Selection).AudioRange.HasCursor;
            }
        }


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
            get { return mForm == null ? ColorSettings.DefaultColorSettings() : mForm.ColorSettings; }
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
        private bool ShouldSerializeColorSettings() { return false; }

        /// <summary>
        /// Copy the current selection into the clipboard. Noop if there is no selection.
        /// </summary>
        public void Copy()
        {
            if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
            if (CanCopySection)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Node.Copy(this, true, Localizer.Message("copy_section")));
            }
            else if (CanCopyStrip)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Node.Copy(this, false, Localizer.Message("copy_section_shallow")));
            }
            else if (CanCopyBlock)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Node.Copy(this, true, Localizer.Message("copy_phrase")));
            }
            else if (CanCopyAudio)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Audio.Copy(this));
            }
        }

        /// <summary>
        /// Cut (delete) the selection and store it in the clipboard.
        /// </summary>
        public void Cut()
        {
            if (CanDelete && mTransportBar.IsPlayerActive) mTransportBar.Stop();
            if (CanRemoveSection || CanRemoveStrip)
            {
                bool isSection = mSelection.Control is InheritedTOCView;
                urakawa.undo.CompositeCommand command = Presentation.CreateCompositeCommand(
                    Localizer.Message(isSection ? "cut_section" : "cut_section_shallow"));
                command.append(new Commands.Node.Copy(this, isSection));
                command.append(new Commands.Node.Delete(this, mSelection.Node));
                mPresentation.getUndoRedoManager().execute(command);
            }
            else if (CanRemoveBlock)
            {
                urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("cut_phrase"));
                command.append(new Commands.Node.Copy(this, true));
                command.append(new Commands.Node.Delete(this, mSelection.Node));
                mPresentation.getUndoRedoManager().execute(command);
            }
            else if (CanRemoveAudio)
            {
                urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("cut_audio"));
                Commands.Audio.Delete delete = new Commands.Audio.Delete(this);
                command.append(new Commands.Audio.Copy(this, delete.Deleted,
                    new AudioRange(0.0, delete.Deleted.Audio.getDuration().getTimeDeltaAsMillisecondFloat())));
                command.append(delete);
                mPresentation.getUndoRedoManager().execute(command);
            }
        }

        /// <summary>
        /// Delete the current selection. Noop if there is no selection.
        /// </summary>
        public void Delete()
        {
            if (CanDelete && mTransportBar.IsPlayerActive) mTransportBar.Stop();
            if (CanRemoveSection)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Node.Delete(this, mTOCView.Selection.Section,
                    Localizer.Message("delete_section")));
            }
            else if (CanRemoveStrip)
            {
                mPresentation.getUndoRedoManager().execute(mContentView.DeleteStripCommand());
            }
            else if (CanRemoveBlock)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Node.Delete(this, SelectedNodeAs<EmptyNode>(),
                    Localizer.Message("delete_phrase")));
            }
            else if (CanRemoveAudio)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Audio.Delete(this));
            }
            else if (CanRemoveMetadata)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Metadata.DeleteEntry(this));
            }
        }

        /// <summary>
        /// Delete all unused nodes.
        /// </summary>
        public void DeleteUnused()
        {
            urakawa.undo.CompositeCommand command = mPresentation.CreateCompositeCommand(Localizer.Message("delete_unused"));
            mPresentation.RootNode.acceptDepthFirst(
                delegate(urakawa.core.TreeNode node)
                {
                    if (node is ObiNode && !((ObiNode)node).Used)
                    {
                        command.append(new Commands.Node.Delete(this, (ObiNode)node));
                        return false;
                    }
                    return true;
                }, delegate(urakawa.core.TreeNode node) { }
            );
            if (command.getCount() > 0) mPresentation.getUndoRedoManager().execute(command);
        }


        /// <summary>
        /// Show (select) the strip for the current selection
        /// </summary>
        public void FocusOnContentView()
        {
                        if (CanFocusOnContentView)
            {
                                                if (mSelection != null && mSelection.Control is  InheritedTOCView)
                {
                                        if (TransportBar.IsPlayerActive)
                        Selection = new NodeSelection(mContentView.PlaybackPhrase, mContentView);
                    else
                        Selection = new NodeSelection(mSelection.Node, mContentView);
                }
                mContentView.Focus();
            }
        }

        /// <summary>
        /// Show the selection (if any) in the TOC view, and focus on this view.
        /// </summary>
        public void FocusOnTOCView()
        {
            if (CanFocusOnTOCView)
            {
                SectionNode node = mSelection == null ? null :
                    mSelection.Node is SectionNode ? (SectionNode)mSelection.Node :
                    mSelection.Node.AncestorAs<SectionNode>();
                if (node != null) Selection = new NodeSelection(node, mTOCView);
                mTOCView.Focus();
            }
        }

        /// <summary>
        /// Insert a new section before the selected one at the same level.
        /// </summary>
        public void InsertSection()
        {
            if (CanInsertStrip)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                Commands.Node.InsertSectionNode insert = new Commands.Node.InsertSectionNode(this);
                AddUnusedAndExecute(insert, insert.NewSection, insert.NewSectionParent);
            }
            else if (CanInsertSection)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                Commands.Node.InsertSectionNode insert = new Commands.Node.InsertSectionNode(this);
                AddUnusedAndExecute(insert, insert.NewSection, insert.NewSectionParent);
            }
        }

        // These methods are used to know what is currently selected:
        // block, strip or section; strict (i.e. not the label or the waveform) or not.
        public bool IsBlockSelected { get { return SelectedNodeAs<EmptyNode>() != null; } }
        public bool IsBlockSelectedStrict { get { return IsBlockSelected && mSelection.GetType() == typeof(NodeSelection); } }

        /// <summary>
        /// Merge the selection strip with the next one, i.e. either its first sibling or its first child.
        /// </summary>
        public void MergeStrips()
        {
            if (CanMergeStripWithNext)
            {
                mPresentation.getUndoRedoManager().execute(mContentView.MergeSelectedStripWithNextCommand());
                if (mSelection != null && mSelection.Node is SectionNode) UpdateBlocksLabelInStrip((SectionNode)mSelection.Node);
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
                    FocusOnTOCView();
                    if (!mTOCView.Focused)
                        FocusOnContentView();
                }
            }
        }

        /// <summary>
        /// Increase the level of the selected section (was "move in.")
        /// </summary>
        public void IncreaseSelectedSectionNodeLevel()
        {
            if (CanIncreaseLevel)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.TOC.MoveSectionIn(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Decrease the level of the selected section (was "move out.")
        /// </summary>
        public void DecreaseSelectedSectionLevel()
        {
            if (CanDecreaseLevel)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.TOC.MoveSectionOut(this, mTOCView.Selection.Section));
            }
        }

        public PageNumber CurrentOrNextPageNumber
        {
            get
            {
                return mSelection.Node is EmptyNode && ((EmptyNode)mSelection.Node).NodeKind == EmptyNode.Kind.Page ?
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
                                node = node.PhraseChild(index - 1);
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
                return mPresentation.PageNumberFollowing(node);
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
        public void Paste()
        {
            if (CanPaste)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                mTransportBar.SelectionChangedPlaybackEnabled = false;
                mPresentation.getUndoRedoManager().execute(mSelection.PasteCommand(this));
                mTransportBar.SelectionChangedPlaybackEnabled = true;
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard before the selected section.
        /// </summary>
        public void PasteBefore()
        {
            if (CanPasteBefore)
            {
                Commands.Node.Paste paste = new Commands.Node.PasteBefore(this);
                AddUnusedAndExecute(paste, paste.Copy, paste.CopyParent);
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard inside (as the last child of) the selected section.
        /// </summary>
        public void PasteInside()
        {
            if (CanPasteInside)
            {
                Commands.Node.PasteInside paste = new Commands.Node.PasteInside(this);
                AddUnusedAndExecute(paste, paste.Copy, paste.CopyParent);
            }
        }

        /// <summary>
        /// Set the block currently playing back as a "light" selection.
        /// </summary>
        public PhraseNode PlaybackPhrase
        {
            set
            {
                if (mContentView != null)
                {
                    mContentView.PlaybackPhrase = value;
                    if (value != null) MakePhraseNodeVisible(value);
                }
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
                        mTOCView.SetNewPresentation();
                        mContentView.NewPresentation();
                        mTransportBar.NewPresentation();
                        mMetadataView.NewPresentation();
                    }
                }
            }
        }

        /// <summary>
        /// Rename a section node after the name was changed directly by the user (not through a menu.)
        /// </summary>
        public void RenameSectionNode(SectionNode section, string label)
        {
            mPresentation.getUndoRedoManager().execute(new Commands.Node.RenameSection(this, section, label));
        }

        /// <summary>
        /// Select a section or strip and start renaming it.
        /// </summary>
        public void SelectAndRenameSelection(NodeSelection selection)
        {
            if (selection.Control is IControlWithRenamableSelection)
            {
                ((IControlWithRenamableSelection)selection.Control).SelectAndRename(selection.Node);
            }
        }

        // Quick way to set the selection

        public EmptyNode SelectedBlockNode { set { Selection = new NodeSelection(value, mContentView); } }
        public SectionNode SelectedSectionNode { set { Selection = new NodeSelection(value, mTOCView); } }
        public SectionNode SelectedStripNode { set { Selection = new NodeSelection(value, mContentView); } }

        /// <summary>
        /// Currently selected node of the given type (e.g. SectionNode, EmptyNode or PhraseNode).
        /// Null if there is no selection, or selection of a different kind.
        /// </summary>
        public T SelectedNodeAs<T>() where T : ObiNode
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
                System.Diagnostics.Debug.Print("Selection: `{0}' >>> `{1}'", mSelection, value);
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
                    UpdateShowOnlySelected(mSelection == null ? false : mShowOnlySelected);
                    if (mSelection != null)
                    {
                        if (mSelection.Control == mTOCView) TOCViewVisible = true;
                        else if (mSelection.Control == mMetadataView) MetadataViewVisible = true;
                        mSelection.Control.Selection = value;
                    }
                    if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Set the used status of the selected node, and of all its descendants.
        /// </summary>
        public void SetSelectedNodeUsedStatus(bool used)
        {
            if (CanSetSelectedNodeUsedStatus && mSelection.Node.Used != used)
            {
                urakawa.undo.CompositeCommand command = Presentation.CreateCompositeCommand(String.Format(
                    Localizer.Message(mSelection.Node is SectionNode ? "mark_section_used" : "mark_phrase_used"),
                    Localizer.Message(mSelection.Node.Used ? "unused" : "used")));
                mSelection.Node.acceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n is ObiNode && ((ObiNode)n).Used != used) command.append(new Commands.Node.ToggleNodeUsed(this, ((ObiNode)n)));
                        return true;
                    }, delegate(urakawa.core.TreeNode n) { }
                );
                Presentation.getUndoRedoManager().execute(command);
            }
        }

        public bool ShowOnlySelectedSection
        {
            set
            {
                mShowOnlySelected = value;
                UpdateShowOnlySelected(value);
            }
        }

        /// <summary>
        /// Get the strip that the selection is in, or null if there is no applicable selection.
        /// </summary>
        public Strip StripForSelection { get { return mContentView == null ? null : mContentView.StripForSelection; } }



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

        public bool WrapStrips { set { mContentView.WrapStrips = value; } }

        /// <summary>
        /// Select the name field of the selected section and start editing it.
        /// </summary>
        public void StartRenamingSelectedSection()
        {
            if (CanRenameStrip)
            {
                mContentView.SelectAndRename(mContentView.Selection.Section);
            }
            else if (CanRenameSection)
            {
                mTOCView.SelectAndRename(SelectedNodeAs<SectionNode>());
            }
        }

        /// <summary>
        /// Split a strip at the selected position.
        /// </summary>
        public void SplitStrip()
        {
            if (CanSplitStrip)
            {
                if (TransportBar.CurrentState == TransportBar.State.Playing) TransportBar.Pause();

                SectionNode OriginalSectionNode = null;
                if (mSelection != null && mSelection.Node is EmptyNode) OriginalSectionNode = mSelection.Node.ParentAs<SectionNode>();
                TransportBar.SelectionChangedPlaybackEnabled = false;
                mPresentation.getUndoRedoManager().execute(mContentView.SplitStripCommand());
                
                if (OriginalSectionNode != null) UpdateBlocksLabelInStrip(OriginalSectionNode);
                TransportBar.SelectionChangedPlaybackEnabled = true;
            }
        }

        /// <summary>
        /// Set the synchronize views flag for this view and resynchronize the views if necessary.
        /// </summary>
        public bool SynchronizeViews
        {
            set
            {
                mSynchronizeViews = value;
                if (!mShowOnlySelected || !CanShowOnlySelectedSection)
                {
                    if (mSynchronizeViews)
                    {
                        mTOCView.ResyncViews();
                        if (mSelection != null && mSelection.Control == mTOCView)
                        {
                            mContentView.MakeStripVisibleForSection(SelectedNodeAs<SectionNode>());
                        }
                    }
                    else
                    {
                        mContentView.UnsyncViews();
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
        private void AddUnusedAndExecute(urakawa.undo.ICommand command, ObiNode node, ObiNode parent)
        {
            if (parent.Used)
            {
                mPresentation.getUndoRedoManager().execute(command);
            }
            else
            {
                urakawa.undo.CompositeCommand c;
                if (command is urakawa.undo.CompositeCommand)
                {
                    c = (urakawa.undo.CompositeCommand)command;
                }
                else
                {
                    c = mPresentation.CreateCompositeCommand(command.getShortDescription());
                    c.append(command);
                }
                AppendMakeUnused(c, node);
                mPresentation.getUndoRedoManager().execute(c);
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
                if (FindInTextVisibilityChanged != null) FindInTextVisibilityChanged(this, null);
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

        public bool IsSectionSelected { get { return SelectedNodeAs<SectionNode>() != null && mSelection.Control == mTOCView; } }
        public bool IsSectionSelectedStrict { get { return IsSectionSelected && mSelection.GetType() == typeof(NodeSelection); } }
        public bool IsStripSelected { get { return SelectedNodeAs<SectionNode>() != null && mSelection.Control == mContentView; } }
        public bool IsStripSelectedStrict { get { return IsStripSelected && mSelection.GetType() == typeof(NodeSelection); } }

        public bool CanDeselect { get { return mSelection != null; } }

        public bool CanShowInStripsView { get { return IsSectionSelected; } }

        public bool CanMarkSectionUnused { get { return mTOCView.CanSetSectionUsedStatus && mSelection.Node.Used; } }
        public bool CanMarkStripUnused { get { return !mContentView.CanSetStripUsedStatus || mSelection.Node.Used; } }
        public bool CanMergeBlockWithNext { get { return mContentView.CanMergeBlockWithNext; } }
        public bool CanSplitPhrase { get { return mTransportBar.CanSplitPhrase; } }

        public bool IsBlockUsed { get { return mContentView.IsBlockUsed; } }
        public bool IsStripUsed { get { return mContentView.IsStripUsed; } }

        /// <summary>
        /// Show the strip for the given section
        /// </summary>
        public void MakeStripVisibleForSection(SectionNode section)
        {
            if (mSynchronizeViews) mContentView.MakeStripVisibleForSection(section);
        }

        /// <summary>
        /// Show the tree node in the TOC view for the given section
        /// </summary>
        public void MakeTreeNodeVisibleForSection(SectionNode section)
        {
            if (mSynchronizeViews) mTOCView.MakeTreeNodeVisibleForSection(section);
        }

        /// <summary>
        /// Show/hide strips for nodes that were collapsed/expanded when the views are synchronized
        /// </summary>
        public void SetStripsVisibilityForSection(SectionNode section, bool visible)
        {
            if (mSynchronizeViews) mContentView.SetStripsVisibilityForSection(section, visible);
        }

        public void SetStripVisibilityForSection(SectionNode section, bool visible)
        {
            if (mSynchronizeViews) mContentView.SetStripVisibilityForSection(section, visible);
        }

        /// <summary>
        /// Make sure that a phrase node is visible in the contents view while it is playing,
        /// and the corresponding section is also visible in the TOC view (if it is shown.)
        /// </summary>
        public void MakePhraseNodeVisible(PhraseNode phrase)
        {
            SectionNode section = phrase.AncestorAs<SectionNode>();
            mTOCView.MakeTreeNodeVisibleForSection(section);
        }

        #region Find in Text

        public bool CanFindFirstTime { get { return mPresentation != null && mPresentation.RootNode.SectionChildCount> 0 ; } }
        public void FindInText()
        {
            //show the form if it's not already shown
            if (mFindInTextSplitter.Panel2Collapsed == true) mFindInTextSplitter.Panel2Collapsed = false;
            FindInTextVisible = true;
            //iterating over the layout panel seems to be the way to search the sections 
            mFindInText.StartNewSearch(mContentView);
        }

        public void FindNextInText()
        {
            if (FindInTextVisible) mFindInText.FindNext();
        }

        public void FindPreviousInText()
        {
            if (FindInTextVisible) mFindInText.FindPrevious();
        }

        public bool CanFindNextPreviousText
        {
            get { return mFindInText.CanFindNextPreviousText; }
        }
        #endregion


        public void ListenToSelection() { }
        public bool CanListenToSection { get { return mTransportBar.Enabled && mTOCView.Selection != null; } }
        public bool CanListenToStrip { get { return mTransportBar.Enabled && mContentView.SelectedSection != null; } }
        public bool CanListenToBlock { get { return mTransportBar.Enabled && mContentView.SelectedPhraseNode != null; } }

        // Blocks



        /// <summary>
        /// Import new phrases in the strip, one block per file.
        /// </summary>
        public void ImportPhrases()
        {
            if (CanImportPhrases)
            {
                string[] paths = SelectFilesToImport();
                if (paths != null)
                {
                    List<PhraseNode> phrases = new List<PhraseNode>(paths.Length);
                    Dialogs.ImportFileSplitSize dialog = new Dialogs.ImportFileSplitSize(ObiForm.Settings.MaxPhraseDurationMinutes);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        ObiForm.Settings.MaxPhraseDurationMinutes = dialog.MaxPhraseDurationMinutes;
                        double duration = dialog.MaxPhraseDurationMinutes * 60000.0;  // convert from minutes to milliseconds
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                        {
                            CreatePhrasesForFiles(phrases, paths, duration);
                        });
                        worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                            delegate(object sender, RunWorkerCompletedEventArgs e)
                            {
                                if (phrases.Count > 0) mPresentation.getUndoRedoManager().execute(new Commands.Strips.ImportPhrases(this, phrases));
                            });
                        worker.RunWorkerAsync();
                    }
                }
            }
        }

        public bool CanImportPhrases { get { return mContentView.Selection != null; } }

        /// <summary>
        /// Bring up the file chooser to select audio files to import and return new phrase nodes for the selected files,
        /// or null if nothing was selected.
        /// </summary>
        private string[] SelectFilesToImport()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = Localizer.Message("audio_file_filter");
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames : null;
        }

        private void CreatePhrasesForFiles(List<PhraseNode> phrases, string[] paths, double duration)
        {
            foreach (string path in paths)
            {
                List<PhraseNode> PhraseList = mPresentation.CreatePhraseNodeList(path, duration);
                foreach (PhraseNode p in PhraseList)
                {
                    if (ImportingFile != null) ImportingFile(this, new ImportingFileEventArgs(path));
                    try
                    {
                        phrases.Add(p);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("import_phrase_error_text"), path),
                            Localizer.Message("import_phrase_error_caption"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            if (FinishedImportingFiles != null) FinishedImportingFiles(this, null);
        }

        public void SelectNothing() { Selection = null; }

        public void SetCustomTypeForSelectedBlock(EmptyNode.Kind kind, string custom)
        {
            if (IsBlockSelected)
            {
                mPresentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(this, SelectedNodeAs<EmptyNode>(), kind, custom));
            }
        }

        /// <summary>
        /// Split a phrase at the playback cursor time (when playback is going on),
        /// the cursor position, or at both ends of the audio selection.
        /// </summary>
        public void SplitPhrase()
        {
            urakawa.undo.CompositeCommand command = CanSplitPhrase ? Commands.Node.SplitAudio.GetSplitCommand(this) : null;
            if (command != null)
            {
                TransportBar.SelectionChangedPlaybackEnabled = false;
                mPresentation.getUndoRedoManager().execute(command);
                // Replace this with the on-the-fly splitting behavior (see ticket #95.)
                if (ObiForm.Settings.PlayOnNavigate) TransportBar.PlayOrResume(mSelection.Node);
                TransportBar.SelectionChangedPlaybackEnabled = true;
            }
        }

        /// <summary>
        /// Merge the selected block with the following one.
        /// </summary>
        public void MergeBlockWithNext()
        {
            if (CanMergeBlockWithNext) mPresentation.getUndoRedoManager().execute(Commands.Node.MergeAudio.GetMergeCommand(this));
        }

        public void MakeSelectedBlockIntoSilencePhrase()
        {
            EmptyNode node = SelectedNodeAs<EmptyNode>();
            if (node != null)
            {
                urakawa.undo.CompositeCommand command = Presentation.getCommandFactory().createCompositeCommand();
                Commands.Node.ChangeCustomType silence = new Commands.Node.ChangeCustomType(this, node, EmptyNode.Kind.Silence);
                command.append(silence);
                command.setShortDescription(silence.getShortDescription());
                if (node.Used) command.append(new Commands.Node.ToggleNodeUsed(this, node));
                Presentation.getUndoRedoManager().execute(command);
            }
        }

        public void MakeSelectedBlockIntoHeadingPhrase()
        {
            if (IsBlockSelected)
            {
                urakawa.undo.CompositeCommand command = Presentation.getCommandFactory().createCompositeCommand();
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                SectionNode parent = node.AncestorAs<SectionNode>();
                if (parent.Heading != null) command.append(new Commands.Node.ChangeCustomType(this, parent.Heading, EmptyNode.Kind.Plain));
                Commands.Node.ChangeCustomType custom = new Commands.Node.ChangeCustomType(this, node, EmptyNode.Kind.Heading);
                command.append(custom);
                command.setShortDescription(custom.getShortDescription());
                mPresentation.getUndoRedoManager().execute(command);
            }
        }

        public void ToggleEmptyNodeTo_DoMark()
        {
            if (Selection != null && Selection.Node is EmptyNode)
            {
                EmptyNode node = (EmptyNode)Selection.Node;
                Commands.Node.ToggleNodeTo_Do command = new Commands.Node.ToggleNodeTo_Do(this, node);
                Presentation.getUndoRedoManager().execute(command);
            }
        }

        public void UpdateCursorPosition(double time) { mContentView.UpdateCursorPosition(time); }
        public void SelectAtCurrentTime() { mContentView.SelectAtCurrentTime(); }


        /// <summary>
        /// Used for adding custom types on the fly: add it to the presentation and also set it on the block
        /// </summary>
        /// <param name="customName"></param>
        /// <param name="kind"></param>
        public void AddCustomTypeAndSetOnBlock(EmptyNode.Kind nodeKind, string customClass)
        {
            if (IsBlockSelected)
            {
                EmptyNode node = SelectedNodeAs<EmptyNode>();
                if (node.NodeKind != nodeKind || node.CustomClass != customClass)
                {
                    mPresentation.getUndoRedoManager().execute(new Obi.Commands.Node.ChangeCustomType(this, node, customClass));
                }
            }
        }

        public bool CanSetPageNumber { get { return IsBlockSelected; } }

        /// <summary>
        /// Set the page number on the selected block and optionally renumber subsequent blocks.
        /// </summary>
        /// <param name="number">The new page number.</param>
        /// <param name="renumber">If true, renumber subsequent blocks.</param>
        public void SetPageNumberOnSelectedBock(PageNumber number, bool renumber)
        {
            if (CanSetPageNumber)
            {
                urakawa.undo.ICommand cmd = new Commands.Node.SetPageNumber(this, SelectedNodeAs<EmptyNode>(), number);
                if (renumber)
                {
                    urakawa.undo.CompositeCommand k = Presentation.CreateCompositeCommand(cmd.getShortDescription());
                    for (ObiNode n = SelectedNodeAs<EmptyNode>().FollowingNode; n != null; n = n.FollowingNode)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page &&
                            ((EmptyNode)n).PageNumber.Kind == number.Kind)
                        {
                            number = number.NextPageNumber();
                            k.append(new Commands.Node.SetPageNumber(this, (EmptyNode)n, number));
                        }
                    }
                    k.append(cmd);
                    cmd = k;
                }
                mPresentation.getUndoRedoManager().execute(cmd);
            }
        }

        /// <summary>
        /// Add a range of pages at the selection position. The page number is increased by one for every subsequent page.
        /// </summary>
        /// <param name="number">The starting number for the range of pages.</param>
        /// <param name="count">The number of pages to add.</param>
        /// <param name="renumber">Renumber subsequent pages if true.</param>
        public void AddPageRange(PageNumber number, int count, bool renumber)
        {
            if (CanAddEmptyBlock)
            {
                urakawa.undo.CompositeCommand cmd =
                    Presentation.CreateCompositeCommand(Localizer.Message("add_blank_pages"));
                int index = -1;
                ObiNode parent = null;
                // For every page, add a new empty block and give it a number.
                for (int i = 0; i < count; ++i)
                {
                    EmptyNode node = new EmptyNode(Presentation);
                    if (parent == null)
                    {
                        parent = mSelection.ParentForNewNode(node);
                        index = mSelection.IndexForNewNode(node);
                    }
                    cmd.append(new Commands.Node.AddEmptyNode(this, node, parent, index + i));
                    cmd.append(new Commands.Node.SetPageNumber(this, node, number));
                    number = number.NextPageNumber();
                }
                // Add commands to renumber the following pages; be careful that the previous blocks have not
                // been added yet!
                // Also be careful to only renumber pages of the same kind.
                if (renumber)
                {
                    ObiNode from = index < parent.getChildCount() ? (ObiNode)parent.getChild(index) : parent;
                    for (ObiNode n = from; n != null; n = n.FollowingNode)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page &&
                            ((EmptyNode)n).PageNumber.Kind == number.Kind)
                        {
                            cmd.append(new Commands.Node.SetPageNumber(this, (EmptyNode)n, number));
                            number = number.NextPageNumber();
                        }
                    }
                }
                mPresentation.getUndoRedoManager().execute(cmd);
            }
        }

        /// <summary>
        /// Apply phrase detection on selected audio block by computing silence threshold from a silence block
        ///  nearest  preceding silence block is  used
        /// </summary>
        public void ApplyPhraseDetection()
        {
            // first check if selected node is phrase node.
            if (CanApplyPhraseDetection)
            {
                if (mTransportBar.IsPlayerActive) mTransportBar.Stop();
                PhraseNode SilenceNode = null;

                //ObiNode  IterationNode = (EmptyNode)mPresentation.FirstSection.PhraseChild (0)  ;
                ObiNode IterationNode = SelectedNodeAs<EmptyNode>();

                while (IterationNode != null)
                {
                    if (IterationNode is EmptyNode && ((EmptyNode)IterationNode).NodeKind == EmptyNode.Kind.Silence)
                    {
                        SilenceNode = (PhraseNode)IterationNode;
                        break;
                    }
                    IterationNode = IterationNode.PrecedingNode;
                }


                Dialogs.SentenceDetection PhraseDetectionDialog = new Obi.Dialogs.SentenceDetection(SilenceNode);
                PhraseDetectionDialog.ShowDialog();
                if (PhraseDetectionDialog.DialogResult == DialogResult.OK)
                {
                    TransportBar.SelectionChangedPlaybackEnabled = false;
                    mPresentation.getUndoRedoManager().execute(new Commands.Node.PhraseDetection(this, PhraseDetectionDialog.Threshold, PhraseDetectionDialog.Gap, PhraseDetectionDialog.LeadingSilence));
                    TransportBar.SelectionChangedPlaybackEnabled = true;
                }
            }// check for phrase node ends
            else
                System.Media.SystemSounds.Beep.Play();
        }


        /// <summary>
        /// Select the phrase after the currently selected phrase in the same strip in the content view.
        /// If no phrase is selected, select the first phrase of the currently selected strip.
        /// If no strip is selected, select the first phrase of the first strip.
        /// </summary>
        public void SelectNextPhrase()
        {
            mContentView.SelectNextPhrase(SelectedNodeAs<ObiNode>());
        }

        public void UpdateBlocksLabelInStrip(SectionNode node)
        {
            if (node != null) mContentView.UpdateBlocksLabelInStrip(node);
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            // trap delete key for preventing deleting of node during rename
            if (Selection is TextSelection && key == Keys.Delete) return false;

            return (key == (Keys)(Keys.Control | Keys.Tab) && SelectViewsInCycle(true)) ||
                    (key == (Keys)(Keys.Control | Keys.Shift | Keys.Tab) && SelectViewsInCycle(false)) ||
                    (key == (Keys)(Keys.F6) && ToggleFocusBTWTOCViewAndContentsView()) ||
                    (key == (Keys)(Keys.Shift | Keys.Space) && TogglePlayPause(UseSelection)) ||
                    (key == Keys.Space && TogglePlayPause(UseAudioCursor)) ||
                    (key == (Keys)(Keys.Alt | Keys.Enter) && ShowNodePropertiesDialog()) ||
                    (key == Keys.F8 && mTransportBar.FocusOnTimeDisplay()) ||
                    base.ProcessCmdKey(ref msg, key);
        }

        private bool SelectViewsInCycle(bool clockwise)
        {
            List<Control> ViewList = new List<Control>();
            ViewList.Add(mTOCView);
            ViewList.Add(mMetadataView);
            ViewList.Add(mPanelInfoLabelButton);
            ViewList.Add(mContentView);
            ViewList.Add(mTransportBar);

            if (mTOCSplitter.Focused)
                mPanelInfoLabelButton.Focus();

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
                    NewFocussedIndex = FocusNextView(ViewList, FocussedViewIndex);
                    return true;
                    //if (NewFocussedIndex == 2) ObiForm.SelectNextControl(ObiForm.ActiveControl, true, true, true, true);
                }
                else
                {
                    NewFocussedIndex = FocusPreviousView(ViewList, FocussedViewIndex);
                    return true;
                }
            }
            else
            {
                if (mTOCView.Focus())
                    return true;
                else
                    return mPanelInfoLabelButton.Focus();
            }
        }

        private int FocusNextView(List<Control> ViewList, int FocussedViewIndex)
        {
            int Index = FocussedViewIndex;
            for (int i = 1; i <= ViewList.Count; i++)
            {
                Index = FocussedViewIndex + i;
                if (Index >= ViewList.Count)
                    Index = Index - ViewList.Count;

                if (ViewList[Index].CanFocus)
                {
                    ViewList[Index].Focus();
                    return Index;
                }
            }
            return -1;
        }


        private int FocusPreviousView(List<Control> ViewList, int FocussedViewIndex)
        {
            int Index = FocussedViewIndex;

            for (int i = 1; i <= ViewList.Count; i++)
            {
                Index = FocussedViewIndex - i;

                if (Index < 0)
                    Index = ViewList.Count + Index;

                if (ViewList[Index].CanFocus)
                {
                    ViewList[Index].Focus();
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
        public bool TogglePlayPause(bool useSelection)
        {
            if (!(mSelection is TextSelection) &&
                (mContentView.ContainsFocus
                || mTOCView.ContainsFocus
                || mTransportBar.ContainsFocus))
            {
                if ((TransportBar.CanPausePlayback || TransportBar.CanResumePlayback) && useSelection)
                {
                    // Resume from selection, not from audio cursor
                    TransportBar.Stop();
                    TransportBar.PlayOrResume();
                    return true;
                }
                else if (TransportBar.CanPause)
                {
                    // Pause playback or recording
                    TransportBar.Pause();
                    return true;
                }
                else if (TransportBar.CanPlay || TransportBar.CanResumePlayback)
                {
                    // Start playback or resume from audio cursor
                    TransportBar.PlayOrResume();
                    return true;
                }
            }
            return false;
        }

        // Initialize timer for tabbing
        private void InitialiseTabbingTimer()
        {
            if (mTabbingTimer == null)
            {
                mTabbingTimer = new System.Windows.Forms.Timer();
                mTabbingTimer.Tick += new System.EventHandler(TabbingTimer_Tick);
                mTabbingTimer.Interval = 500;
            }
        }

        private void mPanelInfoLabelButton_Enter(object sender, EventArgs e)
        {
            mPanelInfoLabelButton.Size = new Size(150, 20);
            mPanelInfoLabelButton.BackColor = System.Drawing.SystemColors.ControlLight;
            mPanelInfoLabelButton.Text = mPanelInfoLabelButton.AccessibleName;
            InitialiseTabbingTimer();
            mTabbingTimer.Start();
        }

        private void TabbingTimer_Tick(object sender, EventArgs e)
        {
            if (mPanelInfoLabelButton.ContainsFocus) SendKeys.Send("{tab}");
            mTabbingTimer.Stop();
        }

        private void mPanelInfoLabelButton_Leave(object sender, EventArgs e)
        {
            mPanelInfoLabelButton.BackColor = System.Drawing.Color.Transparent;
            mPanelInfoLabelButton.Size = new Size(1, 1);
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


        public bool ToggleFocusBTWTOCViewAndContentsView()
        {
            if (mTOCView.ContainsFocus)
                FocusOnContentView();
            else if (mContentView.ContainsFocus)
                FocusOnTOCView();
            else if (mTOCView.Visible) // if neither of views has focus then check if toc is visible, if visible  and focus on it
                FocusOnTOCView();
            else // if neither of view has focus and TOC is not visible, focus on contents view.
                FocusOnContentView();


            return true;
        }

        public void SelectNextTODOPhrase()
        {
            mContentView.SelectNextTODONode();
        }

        public void SelectPreviousTODOPhrase()
        {
            mContentView.SelectPrecedingTODONode();
        }

        /// <summary>
        /// Ensure that the selection is in the content view.
        /// </summary>
        public void SelectInContentView()
        {
            if (mSelection != null && mSelection.Control != mContentView)
            {
                Selection = new NodeSelection(mSelection.Node, mContentView);
            }
        }

        public void SelectPhraseInContentView(PhraseNode node)
        {
            if (node != null) Selection = new NodeSelection(node, mContentView);
        }


        public bool ShowNodePropertiesDialog()
        {
            if (Selection != null)
            {
                if (Selection.Node is SectionNode)
                {
                    ShowSectionPropertiesDialog();
                }
                else if (Selection.Node is EmptyNode)
                {
                    ShowPhrasePropertiesDialog();
                }
            }
            return true;
        }

        /// <summary>
        /// Show the section properties dialog. When the user closes the dialog, look for changes to commit.
        /// Emit a single command consolidating all changes (title, level and used flag).
        /// </summary>
        public void ShowSectionPropertiesDialog()
        {
            if (Selection != null && Selection.Node is SectionNode)
            {
                Dialogs.SectionProperties dialog = new Dialogs.SectionProperties(this);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    urakawa.undo.CompositeCommand command =
                        mPresentation.CreateCompositeCommand(Localizer.Message("update_section"));
                    if (dialog.Label != dialog.Node.Label && dialog.Label != null && dialog.Label != "")
                    {
                        command.append(new Commands.Node.RenameSection(this, dialog.Node, dialog.Label));
                    }
                    for (int i = dialog.Node.Level; i < dialog.Level; ++i)
                    {
                        command.append(new Commands.TOC.MoveSectionIn(this, dialog.Node));
                    }
                    for (int i = dialog.Level; i < dialog.Node.Level; ++i)
                    {
                        command.append(new Commands.TOC.MoveSectionOut(this, dialog.Node));
                    }
                    if (dialog.Used != dialog.Node.Used)
                    {
                        command.append(new Commands.Node.ToggleNodeUsed(this, dialog.Node));
                    }
                    if (command.getCount() == 1) command.setShortDescription(command.getListOfCommands()[0].getShortDescription());
                    if (command.getCount() > 0) mPresentation.getUndoRedoManager().execute(command);
                }
            }
        }

        public void ShowPhrasePropertiesDialog()
        {
            if (Selection != null && Selection.Node is EmptyNode)
            {
                Dialogs.PhraseProperties dialog = new Dialogs.PhraseProperties(this);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    urakawa.undo.CompositeCommand command =
                        mPresentation.CreateCompositeCommand(Localizer.Message("update_phrase"));
                    if (dialog.Role != dialog.Node.NodeKind ||
                        (dialog.Role == EmptyNode.Kind.Custom && dialog.Node.NodeKind == EmptyNode.Kind.Custom &&
                        dialog.CustomClass != dialog.Node.CustomClass))
                    {
                        command.append(new Commands.Node.ChangeCustomType(this, dialog.Node, dialog.Role, dialog.CustomClass));
                    }
                    if (dialog.Used != dialog.Node.Used)
                    {
                        command.append(new Commands.Node.ToggleNodeUsed(this, dialog.Node));
                    }
                    if (dialog.TODO != dialog.Node.TODO)
                    {
                        command.append(new Commands.Node.ToggleNodeTo_Do(this, dialog.Node));
                    }
                    if (command.getCount() == 1) command.setShortDescription(command.getListOfCommands()[0].getShortDescription());
                    if (command.getCount() > 0) mPresentation.getUndoRedoManager().execute(command);
                }
            }
        }

        public void CropPhrase()
        {
            if (CanCropPhrase)
            {
                PhraseNode phrase = Selection.Phrase;
                bool splitBefore = ((AudioSelection)Selection).AudioRange.SelectionBeginTime > 0;
                bool splitAfter = ((AudioSelection)Selection).AudioRange.SelectionEndTime < phrase.Duration;
                urakawa.undo.CompositeCommand command = mPresentation.CreateCompositeCommand(Localizer.Message("crop_audio"));
                command.append(new Commands.Node.SplitAudioSelection(this));
                if (splitAfter) command.append(new Commands.Node.DeleteWithOffset(this, phrase, 1 + (splitBefore ? 1 : 0)));
                if (splitBefore) command.append(new Commands.Node.Delete(this, phrase));
                mPresentation.getUndoRedoManager().execute(command);
            }

        }

        /// <summary>
        /// Get the phrase node to split depending on the selection or the playback node.
        /// </summary>
        public PhraseNode GetNodeForSplit()
        {
            PhraseNode playing = mTransportBar.PlaybackPhrase;
            return playing == null ? SelectedNodeAs<PhraseNode>() : playing;
        }

        private void ProjectView_Layout(object sender, LayoutEventArgs e)
        {
            System.Diagnostics.Debug.Print("LAYOUT form ProjectView: control={0}, component={1}, property={2}",
                e.AffectedControl, e.AffectedComponent, e.AffectedProperty);
        }

        [DefaultValue(0.01f)]
        public float AudioScale
        {
            get { return ObiForm == null ? 0.01f : ObiForm.AudioScale; }
            set { if (value > 0.0f) mContentView.AudioScale = value; }
        }

        [DefaultValue(1.0f)]
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
                    mTransportBar.ZoomFactor = value;
                }
            }
        }
    }

    public class ImportingFileEventArgs
    {
        public string Path;  // path of the file being imported
        public ImportingFileEventArgs(string path) { Path = path; }
    }

    public delegate void ImportingFileEventHandler(object sender, ImportingFileEventArgs e);
}
