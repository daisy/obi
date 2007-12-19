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
        private bool mEnableTooltips;            // tooltips flag
        private Presentation mPresentation;      // presentation
        private NodeSelection mSelection;        // currently selected node
        private Clipboard mClipboard;            // the clipboard
        private bool mSynchronizeViews;          // synchronize views flag
        private ObiForm mForm;                   // parent form

        public event EventHandler SelectionChanged;             // triggered when the selection changes
        public event EventHandler FindInTextVisibilityChanged;  // triggered when the search bar is shown or hidden
        public event ImportingFileEventHandler ImportingFile;   // triggered when a file is being imported
        public event EventHandler FinishedImportingFiles;       // triggered when all files were imported
        public event Obi.Events.SectionNodeHandler OnBeforePasteSection;    //triggered just before a section paste in the TOC view

        /// <summary>
        /// Create a new project view with no project yet.
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();
            mTOCView.ProjectView = this;
            mStripsView.ProjectView = this;
            mFindInText.ProjectView = this;
            mTransportBar.ProjectView = this;
            mTransportBar.Enabled = false;
            mTOCViewVisible = !mTOCSplitter.Panel1Collapsed && !mMetadataSplitter.Panel1Collapsed;
            mMetadataViewVisible = !mTOCSplitter.Panel1Collapsed && !mMetadataSplitter.Panel2Collapsed;
            mPresentation = null;
            mSelection = null;
            mForm = null;
            mClipboard = null;
        }


        /// <summary>
        /// Add a new empty block.
        /// </summary>
        public void AddEmptyBlock()
        {
            if (CanAddEmptyBlock)
            {
                EmptyNode node = new EmptyNode(mPresentation);
                mPresentation.UndoRedoManager.execute(new Commands.Node.AddEmptyNode(this, node,
                    mStripsView.Selection.ParentForNewNode(node), mStripsView.Selection.IndexForNewNode(node)));
            }
        }

        /// <summary>
        /// Add a new section node to the project.
        /// </summary>
        public void AddSection()
        {
            if (CanAddSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.AddSectionNode(this, mTOCView));
            }
        }

        /// <summary>
        /// Insert a new subsection in the book as the last child of the selected section node in the TOC view.
        /// </summary>
        public void AddSubSection()
        {
            if (CanAddSubSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.AddSubSection(this));
            }
        }

        public bool CanAddEmptyBlock { get { return mStripsView.Selection != null; } }
        public bool CanAddSection { get { return mTOCView.CanAddSection && !CanAddStrip; } }
        public bool CanAddStrip { get { return mStripsView.CanAddStrip; } }
        public bool CanAddSubSection { get { return mTOCView.CanAddSection && mTOCView.Selection != null; } }
        public bool CanAssignRole { get { return IsBlockSelected; } }
        public bool CanClearRole { get { return IsBlockSelected && ((EmptyNode)mSelection.Node).NodeKind != EmptyNode.Kind.Plain; } }
        public bool CanCopy { get { return CanCopySection || CanCopyStrip || CanCopyBlock || CanCopyAudio; } }
        public bool CanCopyAudio { get { return mStripsView.CanCopyAudio; } }
        public bool CanCopySection { get { return mTOCView.CanCopySection; } }
        public bool CanCopyStrip { get { return mStripsView.CanCopyStrip; } }
        public bool CanCopyBlock { get { return mStripsView.CanCopyBlock; } }
        public bool CanCut { get { return CanDelete; } }
        public bool CanDelete { get { return CanRemoveSection || CanRemoveStrip || CanRemoveBlock || CanRemoveAudio; } }
        public bool CanMergeStripWithNext { get { return mStripsView.CanMergeStripWithNext; } }
        public bool CanMoveSectionIn { get { return mTOCView.CanMoveSectionIn; } }
        public bool CanMoveSectionOut { get { return mTOCView.CanMoveSectionOut; } }
        public bool CanPaste { get { return mSelection != null && mSelection.CanPaste(mClipboard); } }
        public bool CanRemoveAudio { get { return mStripsView.CanRemoveAudio; } }
        public bool CanRemoveBlock { get { return mStripsView.CanRemoveBlock; } }
        public bool CanRemoveSection { get { return mTOCView.CanRemoveSection; } }
        public bool CanRemoveStrip { get { return mStripsView.CanRemoveStrip; } }
        public bool CanRenameSection { get { return mTOCView.CanRenameSection; } }
        public bool CanRenameStrip { get { return mStripsView.CanRenameStrip; } }
        public bool CanSetBlockUsedStatus { get { return mStripsView.CanSetBlockUsedStatus; } }
        public bool CanSetSectionUsedStatus { get { return mTOCView.CanSetSectionUsedStatus; } }
        public bool CanSetSelectedNodeUsedStatus { get { return CanSetSectionUsedStatus || CanSetBlockUsedStatus; } }
        public bool CanSplitStrip { get { return mStripsView.CanSplitStrip; } }
        
        /// <summary>
        /// Contents of the clipboard
        /// </summary>
        public Clipboard Clipboard
        {
            get { return mClipboard; }
            set { mClipboard = value; }
        }

        /// <summary>
        /// Copy the current selection into the clipboard. Noop if there is no selection.
        /// </summary>
        public void Copy()
        {
            if (CanCopySection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Copy(this, true, Localizer.Message("copy_section")));
            }
            else if (CanCopyStrip)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Copy(this, false, Localizer.Message("copy_strip")));
            }
            else if (CanCopyBlock)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Copy(this, true, Localizer.Message("copy_block")));
            }
            else if (CanCopyAudio)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Audio.Copy(this));
            }
        }

        /// <summary>
        /// Cut (delete) the selection and store it in the clipboard.
        /// </summary>
        public void Cut()
        {
            if (CanRemoveSection || CanRemoveStrip)
            {
                bool isSection = mSelection.Control is TOCView;
                urakawa.undo.CompositeCommand command = Presentation.CreateCompositeCommand(
                    Localizer.Message(isSection ? "cut_section" : "cut_strip"));
                command.append(new Commands.Node.Copy(this, isSection));
                command.append(new Commands.Node.Delete(this, mSelection.Node));
                mPresentation.UndoRedoManager.execute(command);
            }
            else if (CanRemoveBlock)
            {
                urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("cut_block"));
                command.append(new Commands.Node.Copy(this, true));
                command.append(new Commands.Node.Delete(this, mSelection.Node));
                mPresentation.UndoRedoManager.execute(command);
            }
            else if (CanRemoveAudio)
            {
                urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("cut_audio"));
                command.append(new Commands.Audio.Copy(this));
                command.append(new Commands.Audio.Delete(this));
                mPresentation.UndoRedoManager.execute(command);
            }
        }

        /// <summary>
        /// Delete the current selection. Noop if there is no selection.
        /// </summary>
        public void Delete()
        {
            if (CanRemoveSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Delete(this, mTOCView.Selection.Section,
                    Localizer.Message("delete_section")));
            }
            else if (CanRemoveStrip)
            {
                mPresentation.UndoRedoManager.execute(mStripsView.DeleteStripCommand());
            }
            else if (CanRemoveBlock)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Delete(this, SelectedNodeAs<EmptyNode>(),
                    Localizer.Message("delete_block")));
            }
            else if (CanRemoveAudio)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Audio.Delete(this));
            }
        }

        /// <summary>
        /// Enable/disable tooltips in the view (currently mostly disabled.)
        /// </summary>
        public bool EnableTooltips
        {
            get { return mEnableTooltips; }
            set
            {
                mEnableTooltips = value;
                // mStripManagerPanel.EnableTooltips = value;
                // mTOCPanel.EnableTooltips = value;
                // mTransportBar.EnableTooltips = value;
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
            if (CanMergeStripWithNext) mPresentation.UndoRedoManager.execute(mStripsView.MergeSelectedStripWithNextCommand());
        }

        /// <summary>
        /// Move the selected section node in.
        /// </summary>
        public void MoveSelectedSectionIn()
        {
            if (CanMoveSectionIn)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.MoveSectionIn(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Move the selected section node out.
        /// </summary>
        public void MoveSelectedSectionOut()
        {
            if (CanMoveSectionOut)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.MoveSectionOut(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Get the next page number for the selected block.
        /// </summary>
        public int NextPageNumber { get { return mPresentation.PageNumberFor(mSelection.Node); }  }

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
        public void Paste() { if (CanPaste) mPresentation.UndoRedoManager.execute(mSelection.PasteCommand(this)); }

            /*if (CanPaste)
            {
                Commands.Node.Paste paste = new Commands.Node.Paste(this);
                if (mSelection.Control is StripsView && mSelection.Node.SectionChildCount > 0)
                {
                    urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                    for (int i = 0; i < mSelection.Node.SectionChildCount; ++i)
                    {
                        command.append(new Commands.Node.ChangeParent(this, mSelection.Node.SectionChild(i), paste.Copy));
                    }
                    command.append(paste);
                    command.setShortDescription(paste.getShortDescription());
                    mPresentation.UndoRedoManager.execute(command);
                }
                else if (mSelection.Control is TOCView)
                {
                    //if no dummy nodes were required, proceed as normal
                    //else, assume that TOC View took care of the command
                    if (!mTOCView.AddDummyNodes()) mPresentation.UndoRedoManager.execute(paste);
                }
                else
                {
                    mPresentation.UndoRedoManager.execute(paste);
                }
            }*/

        /// <summary>
        /// Set the block currently playing back as a "light" selection.
        /// </summary>
        public PhraseNode PlaybackBlock { set { mStripsView.PlaybackBlock = value; } }

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
                        mStripsView.NewPresentation();
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
            mPresentation.UndoRedoManager.execute(new Commands.Node.RenameSection(this, section, label));
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

        public EmptyNode SelectedBlockNode { set { Selection = new NodeSelection(value, mStripsView); } }
        public SectionNode SelectedSectionNode { set { Selection = new NodeSelection(value, mTOCView); } }
        public SectionNode SelectedStripNode { set { Selection = new NodeSelection(value, mStripsView); } }

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
                if (mSelection != value)
                {
                    // deselect if there was a selection in a different control
                    if (mSelection != null && (value == null || mSelection.Control != value.Control))
                    {
                        mSelection.Control.Selection = null;
                    }
                    // select in the control
                    mSelection = value;
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
                    Localizer.Message(mSelection.Node is SectionNode ? "mark_section_used" : "mark_block_used"),
                    Localizer.Message(mSelection.Node.Used ? "unused" : "used")));
                mSelection.Node.acceptDepthFirst(delegate(urakawa.core.TreeNode node)
                    {
                        if (node is ObiNode && ((ObiNode)node).Used != used)
                        {
                            command.append(new Commands.Node.ToggleNodeUsed(this, (ObiNode)node));
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode node) { }
                );
                Presentation.UndoRedoManager.execute(command);
            }
        }

        /// <summary>
        /// Select the name field of the selected section and start editing it.
        /// </summary>
        public void StartRenamingSelectedSection()
        {
            if (CanRenameSection) mTOCView.SelectAndRename(SelectedNodeAs<SectionNode>());
        }

        /// <summary>
        /// Select the label of the strip and start editing it.
        /// </summary>
        public void StartRenamingSelectedStrip()
        {
            if (CanRenameStrip) mStripsView.SelectAndRename(mStripsView.Selection.Section);
        }

        /// <summary>
        /// Split a strip at the selected position.
        /// </summary>
        public void SplitStrip()
        {
            if (CanSplitStrip) mPresentation.UndoRedoManager.execute(mStripsView.SplitStripCommand());
        }

        /// <summary>
        /// Set the synchronize views flag for this view and resynchronize the views if necessary.
        /// </summary>
        public bool SynchronizeViews
        {
            set
            {
                mSynchronizeViews = value;
                if (mSynchronizeViews)
                {
                    mTOCView.ResyncViews();
                    if (mSelection != null && mSelection.Control == mTOCView)
                    {
                        mStripsView.MakeStripVisibleForSection(SelectedNodeAs<SectionNode>());
                    }
                }
                else
                {
                    mStripsView.UnsyncViews();
                }
            }
        }

        /// <summary>
        /// Get the transport bar for this project view.
        /// </summary>
        public TransportBar TransportBar { get { return mTransportBar; } }


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




















        private bool mTOCViewVisible;  // keep track of the TOC view visibility (don't reopen it accidentally)

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

        private bool mMetadataViewVisible;  // keep track of the Metadata view visibility

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
        public bool IsStripSelected { get { return SelectedNodeAs<SectionNode>() != null && mSelection.Control == mStripsView; } }
        public bool IsStripSelectedStrict { get { return IsStripSelected && mSelection.GetType() == typeof(NodeSelection); } }

        public bool CanDeselect { get { return mSelection != null; } }

        public bool CanShowInStripsView { get { return IsSectionSelected; } }
        public bool CanShowInTOCView { get { return IsStripSelected; } }

        public bool CanMarkSectionUnused { get { return mTOCView.CanSetSectionUsedStatus && mSelection.Node.Used; } }
        public bool CanMergeBlockWithNext { get { return mStripsView.CanMergeBlockWithNext; } }
        public bool CanSplitBlock { get { return mSelection is AudioSelection; } }
        
        public bool IsBlockUsed { get { return mStripsView.IsBlockUsed; } }

        /// <summary>
        /// Show the strip for the given section
        /// </summary>
        public void MakeStripVisibleForSection(SectionNode section)
        {
            if (mSynchronizeViews) mStripsView.MakeStripVisibleForSection(section);
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
            if (mSynchronizeViews) mStripsView.SetStripsVisibilityForSection(section, visible);
        }

        public void SetStripVisibilityForSection(SectionNode section, bool visible)
        {
            if (mSynchronizeViews) mStripsView.SetStripVisibilityForSection(section, visible);
        }

        /// <summary>
        /// Show (select) the section node for the current selection
        /// </summary>
        public void ShowSelectedSectionInTOCView()
        {
            if (CanShowInTOCView)
            {
                Selection = new NodeSelection(mSelection.Node, mTOCView);
            }
            else
            {
                TOCViewVisible = true;
                mTOCView.Focus();
            }
        }

        /// <summary>
        /// Show (select) the strip for the current selection
        /// </summary>
        public void ShowSelectedSectionInStripsView()
        {
            if (CanShowInStripsView)
            {
                Selection = new NodeSelection(mSelection.Node, mStripsView);
            }
            else
            {
                mStripsView.GetFocus();
            }
        }

        #region Find in Text

        public void FindInText()
        {
            //show the form if it's not already shown
            if (mFindInTextSplitter.Panel2Collapsed == true) mFindInTextSplitter.Panel2Collapsed = false;
            FindInTextVisible = true;
            //iterating over the layout panel seems to be the way to search the sections 
            mFindInText.StartNewSearch(mStripsView);
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
        public bool CanListenToStrip { get { return mTransportBar.Enabled && mStripsView.SelectedSection != null; } }
        public bool CanListenToBlock { get { return mTransportBar.Enabled && mStripsView.SelectedPhraseNode != null; } }

        // Blocks



        /// <summary>
        /// Import new phrases in the strip, one block per file.
        /// </summary>
        public void ImportPhrases()
        {
            if (CanImportPhrases)
            {
                string[] paths = SelectFilesToImport();
                List<PhraseNode> phrases = new List<PhraseNode>(paths.Length);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                {
                    CreatePhrasesForFiles(phrases, paths);
                });
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                    delegate(object sender, RunWorkerCompletedEventArgs e)
                    {
                        if (phrases.Count > 0) mPresentation.UndoRedoManager.execute(new Commands.Strips.ImportPhrases(this, phrases));
                    });
                worker.RunWorkerAsync();
            }
        }

        public bool CanImportPhrases { get { return mStripsView.Selection != null; } }

        /// <summary>
        /// Bring up the file chooser to select audio files to import and return new phrase nodes for the selected files,
        /// or null if nothing was selected.
        /// </summary>
        private string[] SelectFilesToImport()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = Localizer.Message("audio_file_filter");
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames : new string[0];
        }

        private void CreatePhrasesForFiles(List<PhraseNode> phrases, string[] paths)
        {
            foreach (string path in paths)
            {
                if (ImportingFile != null) ImportingFile(this, new ImportingFileEventArgs(path));
                try
                {
                    phrases.Add(mPresentation.CreatePhraseNode(path));
                }
                catch (Exception)
                {
                    MessageBox.Show(String.Format(Localizer.Message("import_phrase_error_text"), path),
                        Localizer.Message("import_phrase_error_caption"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            if (FinishedImportingFiles != null) FinishedImportingFiles(this, null);
        }

        public void SelectNothing() { Selection = null; }

        public void SetCustomTypeForSelectedBlock(EmptyNode.Kind kind, string custom)
        {
            if (IsBlockSelected)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.ChangeCustomType(this, SelectedNodeAs<EmptyNode>(), kind, custom));
            }
        }

        public void SplitBlock()
        {
            if (CanSplitBlock) mPresentation.UndoRedoManager.execute(new Commands.Node.SplitAudio(this));
        }

        public void MergeBlockWithNext()
        {
            if (CanMergeBlockWithNext) mPresentation.UndoRedoManager.execute(new Commands.Node.MergeAudio(this));
        }

        #region Containers (not yet...)

        /*public void MakeBlockIntoContainer()
        {
            //there is a problem with this because the treenode_added event gets thrown for each step
            //and Obi isn't used to dealing with empty nodes yet (which happens after cmd1 below gets executed)
            if (SelectedBlockNode != null)
            {
                urakawa.undo.CompositeCommand command = Presentation.getCommandFactory().createCompositeCommand();
                Commands.Node.AddContainer cmd1 = new Obi.Commands.Node.AddContainer(this, SelectedBlockNode.ParentAs<ObiNode>(), SelectedBlockNode.Index);
                Commands.Node.ChangeParent cmd2 = new Obi.Commands.Node.ChangeParent(this, SelectedBlockNode, cmd1.Container);
                command.append(cmd1);
                command.append(cmd2);
                mPresentation.UndoRedoManager.execute(command);
            }
        }*/

        //TODO: put this code into a command
        /*public void RemoveContainer()
        {
            if (SelectedBlockNode != null)
            {
                //here, selected block node is the container itself
                int index = SelectedBlockNode.Index;
                ObiNode parentNode = SelectedBlockNode.ParentAs<ObiNode>();
                for (int i = 0; i < SelectedBlockNode.PhraseChildCount; i++)
                {
                    PhraseNode node = (PhraseNode)SelectedBlockNode.PhraseChild(0).Detach();
                    parentNode.Insert(node, index + i);
                }
                parentNode.RemoveChild(SelectedBlockNode);
            }
        }*/

        #endregion

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
                Presentation.UndoRedoManager.execute(command);
            }
        }

        public void MakeSelectedBlockIntoHeadingPhrase()
        {
            if (IsBlockSelected)
            {
                urakawa.undo.CompositeCommand command = Presentation.getCommandFactory().createCompositeCommand();

                //1. clear existing custom type
                Commands.Node.ChangeCustomType cmd1 = new Obi.Commands.Node.ChangeCustomType(this, SelectedNodeAs<EmptyNode>(),
                    EmptyNode.Kind.Plain);
                //2. unset existing heading on section
                EmptyNode node = SelectedNodeAs<EmptyNode>().AncestorAs<SectionNode>().Heading;
                Commands.Node.UnsetNodeAsHeadingPhrase cmd2 = new Obi.Commands.Node.UnsetNodeAsHeadingPhrase(this, node);
                //3. set new heading
                Commands.Node.SetNodeAsHeadingPhrase cmd3 = new Obi.Commands.Node.SetNodeAsHeadingPhrase(this,
                    SelectedNodeAs<PhraseNode>());
                //4. assign new custom type as "heading"
                Commands.Node.ChangeCustomType cmd4 = new Obi.Commands.Node.ChangeCustomType(this, SelectedNodeAs<EmptyNode>(),
                    EmptyNode.Kind.Heading);
                command.setShortDescription(cmd4.getShortDescription());
                command.append(cmd1);
                command.append(cmd2);
                command.append(cmd3);
                command.append(cmd4);
                mPresentation.UndoRedoManager.execute(command);
            }
        }

        public void UpdateCursorPosition(double time) { mStripsView.UpdateCursorPosition(time); }
        public void SelectAtCurrentTime() { mStripsView.SelectAtCurrentTime(); }


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
                    mPresentation.UndoRedoManager.execute(new Obi.Commands.Node.ChangeCustomType(this, node, customClass));
                }
            }
        }

        public bool CanSetPageNumber { get { return IsBlockSelected; } } 

        /// <summary>
        /// Set the page number on the selected block and optionally renumber subsequent blocks.
        /// </summary>
        /// <param name="number">The new page number.</param>
        /// <param name="renumber">If true, renumber subsequent blocks.</param>
        public void SetPageNumberOnSelectedBock(int number, bool renumber)
        {
            if (CanSetPageNumber)
            {
                urakawa.undo.ICommand cmd = new Commands.Node.SetPageNumber(this, SelectedNodeAs<EmptyNode>(), number);
                if (renumber)
                {
                    urakawa.undo.CompositeCommand k = Presentation.CreateCompositeCommand(cmd.getShortDescription());
                    for (ObiNode n = SelectedNodeAs<EmptyNode>().FollowingNode; n != null; n = n.FollowingNode)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page)
                        {
                            k.append(new Commands.Node.SetPageNumber(this, (EmptyNode)n, ++number));
                        }
                    }
                    k.append(cmd);
                    cmd = k;
                }
                mPresentation.UndoRedoManager.execute(cmd);
            }
        }

        /// <summary>
        /// Add a range of pages at the selection position. The page number is increased by one for every subsequent page.
        /// </summary>
        /// <param name="number">The starting number for the range of pages.</param>
        /// <param name="count">The number of pages to add.</param>
        /// <param name="renumber">Renumber subsequent pages if true.</param>
        public void AddPageRange(int number, int count, bool renumber)
        {
            if (CanAddEmptyBlock)
            {
                urakawa.undo.CompositeCommand cmd =
                    Presentation.CreateCompositeCommand(Localizer.Message("add_empty_page_blocks"));
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
                    cmd.append(new Commands.Node.SetPageNumber(this, node, number++));
                }
                // Add commands to renumber the following pages; be careful that the previous blocks have not
                // been added yet!
                if (renumber)
                {
                    ObiNode from = index < parent.getChildCount() ? (ObiNode)parent.getChild(index) : parent;
                    for (ObiNode n = from; n != null; n = n.FollowingNode)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page)
                        {
                            cmd.append(new Commands.Node.SetPageNumber(this, (EmptyNode)n, number++));
                        }
                    }
                }
                mPresentation.UndoRedoManager.execute(cmd);
            }
        }

        /// <summary>
        /// Apply phrase detection on selected audio block by computing silence threshold from a silence block
        ///  nearest  preceding silence block is  used
                /// </summary>
        public void ApplyPhraseDetection()
        {
                        PhraseNode SilenceNode= null ;

            //ObiNode  IterationNode = (EmptyNode)mPresentation.FirstSection.PhraseChild (0)  ;
                        ObiNode IterationNode = SelectedNodeAs<EmptyNode> () ;

            while (IterationNode!= null)
            {
                if ( IterationNode is EmptyNode    &&     ((EmptyNode)  IterationNode).NodeKind == EmptyNode.Kind.Silence)
                {
                    SilenceNode =(PhraseNode)   IterationNode;
                    break;
                }
                IterationNode = IterationNode.PrecedingNode  ;
            }


            Dialogs.SentenceDetection PhraseDetectionDialog = new Obi.Dialogs.SentenceDetection(SilenceNode);
            PhraseDetectionDialog.ShowDialog();
            if (PhraseDetectionDialog.DialogResult == DialogResult.OK)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.PhraseDetection(this, PhraseDetectionDialog.Threshold, PhraseDetectionDialog.Gap, PhraseDetectionDialog.LeadingSilence));
            }

                }// end of function

    }

    public class ImportingFileEventArgs
    {
        public string Path;  // path of the file being imported
        public ImportingFileEventArgs(string path) { Path = path; }
    }

    public delegate void ImportingFileEventHandler(object sender, ImportingFileEventArgs e);

}