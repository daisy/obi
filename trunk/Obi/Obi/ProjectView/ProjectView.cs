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
        private Project mProject;                // project model
        private NodeSelection mSelection;        // currently selected node
        private Commands.UndoRedoManager mUndo;  // the undo manager for the project view

        public Commands.UndoRedoEventHandler CommandExecuted;
        public Commands.UndoRedoEventHandler CommandUnexecuted;

        public ProjectView()
        {
            InitializeComponent();
            mProject = null;
            mSelection = null;
            mTransportBar.ProjectView = this;
            // Create the undo/redo manager for the view and pass along its events
            mUndo = new Commands.UndoRedoManager();
            mUndo.CommandExecuted += new Commands.UndoRedoEventHandler(delegate(object sender, Commands.UndoRedoEventArgs e)
            {
                if (CommandExecuted != null) CommandExecuted(sender, e);
            });
            mUndo.CommandUnexecuted += new Commands.UndoRedoEventHandler(delegate(object sender, Commands.UndoRedoEventArgs e)
            {
                if (CommandUnexecuted != null) CommandUnexecuted(sender, e);
            });
        }


        /// <summary>
        /// A node (from the clipboard) can be pasted if the selection context is right.
        /// Note that pasting a used section or used phrase under an unused section should
        /// turn the pasted node to unused.
        /// </summary>
        /// <param name="node">The node to paste (coming from the clipboard.) Null if no node is in the clipboard.</param>
        public bool CanPaste(ObiNode node)
        {
            if (node == null)
            {
                // nothing to paste
                return false;
            }
            else if (mSelection == null)
            {
                // no selection: can only paste a section
                return mProject != null && node is SectionNode;
            }
            else if (node is SectionNode)
            {
                // a section can only be pasted under a section
                // same for strips
                return mSelection.Node is SectionNode;
            }
            else
            {
                // a phrase can ony be pasted in the strip view
                return false; // mCurrentSelection.Control == mStripManagerPanel;
            }
        }

        /// <summary>
        /// Copy the current selection into the clipboard. Noop if there is no selection.
        /// </summary>
        public void Copy()
        {
        }

        /// <summary>
        /// Copy the current selection into the clipboard then delete it from the panel. Noop if there is no selection.
        /// </summary>
        public void Cut()
        {
        }

        /// <summary>
        /// Delete the current selection. Noop if there is no selection.
        /// </summary>
        public void Delete()
        {
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
                mTransportBar.EnableTooltips = value;
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard in the current context. Noop if the clipboard is empty.
        /// </summary>
        public void Paste()
        {
        }

        /// <summary>
        /// Get a label for the node currently in the clipboard, with regard to the
        /// context in which it is to be pasted (i.e. show "strip" if the context is
        /// the strip manager, which it is by default, or "section" if the context is
        /// the TOC panel.)
        /// </summary>
        /// <param name="node">The node to paste (coming from the clipboard.) Null if
        /// no node is in the clipboard.</param>
        public string PasteLabel(ObiNode node)
        {
            return node == null ? "" :                                      // nothing to paste
                Localizer.Message(node is PhraseNode ? "audio_block" :      // audio block
                    mSelection != null &&
                    //mSelection.Control == mTOCPanel ? "section" :    // pasting in TOC panel
                    ((SectionNode)node).SectionChildCount > 0 ? "strips" :  // pasting several strips
                    "strip");                                               // pasting only one strip
        }

        /// <summary>
        /// The project model that is shown by this view.
        /// </summary>
        public Project Project
        {
            get { return mProject; }
            set
            {
                ProjectVisible = value != null;
                if (mProject != value)
                {
                    /* cleanup old project */
                    mProject = value;
                    /* initialize stuff */
                    if (mProject != null)
                    {
                        mTOCView.NewProject();
                        mStripsView.Project = mProject;
                        // mMetadataView.Project = mProject;
                        SynchronizeWithCoreTree();
                    }
                }
            }
        }

        private bool ProjectVisible
        {
            set
            {
                mHSplitter.Visible = value;
                mVSplitter.Visible = value;
                mTransportBar.Visible = value;
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
                if (mSelection != value)
                {
                    if (mSelection != null) mSelection.Control.Selection = null;
                    mSelection = value;
                    if (value != null) value.Control.Selection = value.Node;
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
                    mSelection == null ?                       "" :
                    mSelection.Node is PhraseNode ?            Localizer.Message("audio_block") :
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
        /// Redraw everything to keep the view in sync with the model.
        /// </summary>
        public void SynchronizeWithCoreTree()
        {
        }

        /// <summary>
        /// The transport bar for the view.
        /// </summary>
        public UserControls.TransportBar TransportBar { get { return mTransportBar; } }

        /// <summary>
        /// True if the TOC panel is visible. Set it to change the panel visibility.
        /// </summary>
        public bool TOCPanelVisible
        {
            get { return true; }
            set { /* change the TOC panel visibility. */ }
        }


        #region TOC Panel

        /// <summary>
        /// Add a new sibling section after the currently selected section in the TOC view.
        /// If no section is selected, then append a new section at the top level.
        /// </summary>
        public SectionNode AddSection()
        {
            return mProject.CreateSiblingSectionNode(mTOCView.SelectedSection);
        }

        /// <summary>
        /// Show the selected section in the TOC view in the strip view.
        /// </summary>
        public void ShowSelectedSectionInStripView()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

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
            set {  }
        }

        /// <summary>
        /// The label for the toggling of an audio block's used flag.
        /// </summary>
        public string ToggleAudioBlockUsedFlagLabel
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }



        internal void InsertStrip()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void StartRenamingSelectedStrip()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal void ImportPhrases()
        {
            throw new Exception("The method or operation is not implemented.");
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

        internal void ToggleSelectedAudioBlockUsed()
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

        internal void ShowSelectedStripInTOCView()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        /// <summary>
        /// Add a new section node to the project.
        /// </summary>
        public void AddNewSection()
        {
            if (mProject != null) mUndo.execute(new Commands.TOC.AddNewSection(this, mTOCView.SelectedSection));
        }

        /// <summary>
        /// Insert a new subsection in the book as the last child of the selected section node in the TOC view.
        /// </summary>
        public void AddNewSubSection()
        {
            if (mProject != null && mTOCView.SelectedSection != null)
            {
                mUndo.execute(new Commands.TOC.AddNewSection(this, mTOCView.SelectedSection, true));
            }
        }

        /// <summary>
        /// Select the name field of the selected section and start editing it.
        /// </summary>
        public void StartRenamingSelectedSection()
        {
            if (mSelection != null && mTOCView.SelectedSection != null)
            {
                mTOCView.SelectAndRenameNode(mTOCView.SelectedSection);
            }
        }

        /// <summary>
        /// Undo the last command if there is any. Don't do anything otherwise.
        /// </summary>
        public void Undo()
        {
            if (mUndo.canUndo()) mUndo.undo();
        }

        /// <summary>
        /// Redo the last command if there is ant. Don't do anything otherwise.
        /// </summary>
        public void Redo()
        {
            if (mUndo.canRedo()) mUndo.redo();
        }

        /// <summary>
        /// Select a section node in the TOC view.
        /// </summary>
        public void SelectInTOCView(SectionNode section)
        {
            mTOCView.SelectNode(section);
        }

        /// <summary>
        /// Select a section node in the TOC view and start renaming it.
        /// </summary>
        /// <param name="section"></param>
        public void SelectAndRenameNodeInTOCView(SectionNode section)
        {
            mTOCView.SelectAndRenameNode(section);
        }

        public void RenameSectionNode(SectionNode section, string label)
        {
            mUndo.execute(new Commands.Node.RenameSection(this, section, label));
        }
    }
}