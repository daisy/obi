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
        private ObiNode mClipboard;              // node in the clipboard
        private bool mSynchronizeViews;          // synchronize views flag
        private ObiForm mForm;                   // parent form

        public event EventHandler SelectionChanged;             // triggered when the selection changes
        public event EventHandler FindInTextVisibilityChanged;  // triggered when the search bar is shown or hidden
        public event ImportingFileEventHandler ImportingFile;   // triggered when a file is being imported
        public event EventHandler FinishedImportingFiles;       // triggered when all files were imported


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
        }


        /// <summary>
        /// Contents of the clipboard (at the moment a single node.)
        /// </summary>
        public ObiNode Clipboard
        {
            get { return mClipboard; }
            set { mClipboard = value; }
        }

        /// <summary>
        /// Enable/disable tooltips in the view.
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

        /// <summary>
        /// The parent form as an Obi form.
        /// </summary>
        public ObiForm ObiForm
        {
            get { return mForm; }
            set { mForm = value; }
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
                    if (mPresentation != null)
                    {
                        mTOCView.NewPresentation();
                        mStripsView.NewPresentation();
                    }
                }
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
        /// If the current selection is a section, return it, otherwise return null.
        /// </summary>
        public SectionNode SelectedSection
        {
            get
            {
                return mSelection == null ? // || mSelection.Control != mTOCPanel ?
                    null : mSelection.Node as SectionNode;
            }
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
                    }
                    if (mSelection != null) mSelection.Control.Selection = value;
                    if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Get a label for the node currently selected, i.e. "" if nothing is seleced,
        /// "audio block" for an audio block, "strip" for a strip and "section" for a
        /// section.
        /// </summary>
        public string SelectedName
        {
            get
            {
                return
                    mSelection == null ? "" :
                    mSelection.Node is PhraseNode ? Localizer.Message("audio_block") :
                    // mSelection.Control == mStripManagerPanel ? Localizer.Message("strip") :
                                                               Localizer.Message("section");
            }
        }

        /// <summary>
        /// Currently selected node, regardless of its type or where it is selected.
        /// Null if nothing is selected.
        /// </summary>
        public ObiNode SelectionNode
        {
            get { return mSelection == null ? null : mSelection.Node; }
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
                    // TODO depending on where the focus is, resync TOC-wise or strips-wise
                    // (now it is TOC-wise...)
                    mTOCView.ResyncViews();
                    if (mSelection != null && mSelection.Control == mTOCView)
                    {
                        mStripsView.MakeStripVisibleForSection(SelectedSection);
                    }
                }
                else
                {
                    mStripsView.UnsyncViews();
                }
            }
        }

        /// <summary>
        /// The transport bar for the view.
        /// </summary>
        public UserControls.TransportBar TransportBar { get { return mTransportBar; } }


        #region Strips

        /// <summary>
        /// True if the current selection/focus allows the insertion of an audio block.
        /// </summary>
        public bool CanInsertAudioBlock
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// True if the current selection allows merging of blocks.
        /// </summary>
        public bool CanMergeBlocks
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// True if the current selection allows the removal of a page number.
        /// </summary>
        public bool CanRemovePageNumber
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// True if the current selection allows the setting of a page number.
        /// </summary>
        public bool CanSetPageNumber
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// True if the current selection allows the the toggling of and audio block's used flag.
        /// </summary>
        public bool CanToggleAudioBlockUsedFlag
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// The phrase node for the block selected in the strips view.
        /// Null if no strip is selected.
        /// TODO: we need a compound node kind for container blocks.
        /// </summary>
        public PhraseNode SelectedBlockNode
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// The section node for the strip selected in the strips view.
        /// Null if no strip is selected.
        /// </summary>
        public SectionNode SelectedStripNode
        {
            get { return null; }
            set { Selection = new NodeSelection(value, mStripsView, false); }
        }

        /// <summary>
        /// The label for the toggling of an audio block's used flag.
        /// </summary>
        public string ToggleAudioBlockUsedFlagLabel
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }




        internal void SplitBlock()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void QuickSplitBlock()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void ApplyPhraseDetection()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void MergeBlocks()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void MoveBlock(PhraseNode.Direction direction)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void MarkSelectedAudioBlockAsHeading()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void EditAnnotationForSelectedAudioBlock()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void RemoveAnnotationForAudioBlock()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void SetPageNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void RemovePageNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void FocusOnAnnotation()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void GoToPage()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void ToggleSelectedStripUsed()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        /// <summary>
        /// Add a new section node to the project.
        /// </summary>
        public void AddNewSection()
        {
            if (CanAddSection)
            {
                // TODO when there is a dummy node this becomes unnecessary
                mPresentation.UndoRedoManager.execute(new Commands.TOC.AddNewSection(this, mTOCView.Selection == null ?
                    new NodeSelection(mPresentation.RootNode, mTOCView, true) : mTOCView.Selection));
            }
        }

        /// <summary>
        /// Insert a new subsection in the book as the last child of the selected section node in the TOC view.
        /// </summary>
        public void AddNewSubSection()
        {
            if (CanAddSubSection)
            {
                // hack to simulate the dummy node
                SectionNode section = (SectionNode)mTOCView.Selection.Node;
                mPresentation.UndoRedoManager.execute(new Commands.TOC.AddNewSection(this,
                    section.SectionChildCount > 0 ?
                    new NodeSelection(section.SectionChild(section.SectionChildCount - 1), mTOCView, false) :
                    new NodeSelection(mTOCView.Selection.Node, mTOCView, true)));
            }
        }

        /// <summary>
        /// Add a new strip after, and at the same level as, the selected strip
        /// </summary>
        public void AddNewStrip()
        {
            if (CanAddStrip) { mPresentation.UndoRedoManager.execute(new Commands.Strips.AddNewStrip(this)); }
        }

        /// <summary>
        /// Select the name field of the selected section and start editing it.
        /// </summary>
        public void StartRenamingSelectedSection()
        {
            if (CanRenameSection) mTOCView.SelectAndRename(mTOCView.Selection.Section);
        }

        /// <summary>
        /// Select the label of the strip and start editing it.
        /// </summary>
        public void StartRenamingSelectedStrip()
        {
            if (CanRenameStrip) mStripsView.SelectAndRename(mStripsView.Selection.Section);
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
        /// Change the used status of the selected section, and of all its subsections.
        /// </summary>
        public void ToggleSectionUsed()
        {
            if (CanToggleSectionUsed)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.ToggleSectionUsed(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Cut (delete) the selection and store it in the clipboard.
        /// </summary>
        public void Cut()
        {
            if (CanRemoveSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.Cut(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Copy the current selection into the clipboard. Noop if there is no selection.
        /// </summary>
        public void Copy()
        {
            if (CanCopySection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.Copy(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard in the current context. Noop if the clipboard is empty.
        /// </summary>
        public void Paste()
        {
            if (CanPasteSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.Paste(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Delete the current selection. Noop if there is no selection.
        /// </summary>
        public void Delete()
        {
            if (CanRemoveSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.Delete(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Select a section node in the TOC view.
        /// </summary>
        public void SelectInTOCView(SectionNode section)
        {
            mTOCView.SelectNode(section);
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

        public void RenameSectionNode(SectionNode section, string label)
        {
            mPresentation.UndoRedoManager.execute(new Commands.Node.RenameSection(this, section, label));
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


        public bool CanCut { get { return Selection != null; } }
        public bool CanCopy { get { return Selection != null; } }
        public bool CanDelete { get { return Selection != null; } }
        public bool CanPaste { get { return Selection != null && mClipboard != null; } }

        public bool CanShowInStripsView { get { return SelectedSection != null && mSelection.Control == mTOCView; } }
        public bool CanShowInTOCView { get { return SelectedSection != null && mSelection.Control == mStripsView; } }

        // hacky but will do for now
        public bool CanAddSection { get { return mTOCView.CanAddSection && !mStripsView.CanAddStrip; } }
        public bool CanAddStrip { get { return mStripsView.CanAddStrip; } }
        public bool CanAddSubSection { get { return mTOCView.Selection != null; } }
        public bool CanCopySection { get { return mTOCView.Selection != null && !mTOCView.Selection.IsDummy; } }
        public bool CanMarkSectionUnused { get { return mTOCView.CanToggleSectionUsed && mTOCView.Selection.Node.Used; } }
        public bool CanMarkSectionUsed { get { return mTOCView.CanToggleSectionUsed && !mTOCView.Selection.Node.Used; } }
        public bool CanMoveSectionIn { get { return mTOCView.CanMoveSectionIn; } }
        public bool CanMoveSectionOut { get { return mTOCView.CanMoveSectionOut; } }
        public bool CanPasteSection
        {
            get
            {
                return mTOCView.Selection != null && mTOCView.Selection.Node is SectionNode
                    && mClipboard is SectionNode;
            }
        }
        public bool CanRemoveSection { get { return mTOCView.CanRemoveSection; } }
        public bool CanRenameSection { get { return mTOCView.CanRenameSection; } }
        public bool CanRenameStrip { get { return mStripsView.CanRenameStrip; } }
        public bool CanToggleSectionUsed { get { return mTOCView.CanToggleSectionUsed; } }

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

        /// <summary>
        /// Show (select) the section node for the current selection
        /// </summary>
        public void ShowSelectedSectionInTOCView()
        {
            if (CanShowInTOCView) Selection = new NodeSelection(mSelection.Node, mTOCView, false);
        }

        /// <summary>
        /// Show (select) the strip for the current selection
        /// </summary>
        public void ShowSelectedSectionInStripsView()
        {
            if (CanShowInStripsView) Selection = new NodeSelection(mSelection.Node, mStripsView, false);
        }

        #region Find in Text

        public void FindInText()
        {
            //show the form if it's not already shown
            if (mFindInTextSplitter.Panel2Collapsed == true) mFindInTextSplitter.Panel2Collapsed = false;
            FindInTextVisible = true;
            //iterating over the layout panel seems to be the way to search the sections 
            mFindInText.ShowFindInText(mStripsView.LayoutPanel);
        }
        
        #endregion


        /// <summary>
        /// Split a strip at the selected position. (TODO!)
        /// </summary>
        public void SplitStrip() { if (CanSplitStrip) { } }
        public bool CanSplitStrip { get { return false; } }

        /// <summary>
        /// Merge a strip with the next one. (TODO!)
        /// </summary>
        public void MergeStrips() { if (CanMergeStrips) {} }
        public bool CanMergeStrips { get { return false; } }

        /// <summary>
        /// Debug information about the selected strip
        /// </summary>
        public void AboutStrip() { if (CanTellAboutStrip) { mStripsView.AboutSelectedStrip(); } }
        public bool CanTellAboutStrip { get { return mStripsView.Selection != null; } }

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

        private void mStripsView_Load(object sender, EventArgs e)
        {

        }
    }

    public class ImportingFileEventArgs
    {
        public string Path;  // path of the file being imported
        public ImportingFileEventArgs(string path) { Path = path; }
    }

    public delegate void ImportingFileEventHandler(object sender, ImportingFileEventArgs e);

}