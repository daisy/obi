using System;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// Top level panel that displays the current project.
    /// Contains a TOC (tree) view, a strip view, and a transport bar.
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private Project mProject;                 // the project to display
        private NodeSelection mCurrentSelection;  // node currently selected, and where
        private bool mEnableTooltips;             // enable or disable tooltips

        /// <summary>
        /// Create an empty project panel.
        /// Empty views and transport bar are created as well.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            mTOCPanel.ProjectPanel = this;
            mStripManagerPanel.ProjectPanel = this;
            mTransportBar.ProjectPanel = this;
            // TODO: find out why the transport bar cannot disable itself when created.
            mTransportBar.Enabled = false;
            Project = null;
        }


        #region selection

        /// <summary>
        /// Current selection in the project panel.
        /// </summary>
        public NodeSelection CurrentSelection
        {
            get { return mCurrentSelection; }
            set
            {
                if (value == null)
                {
                    Deselect();
                    mCurrentSelection = null;
                }
                else if (mCurrentSelection == null ||
                    value.Node != mCurrentSelection.Node ||
                    value.Control != mCurrentSelection.Control)
                {
                    if (mTransportBar.IsSelectionRelevant(value.Node))
                    {
                        if (mTransportBar.CanSelectPhrase(value.Node))
                        {
                            Select(value);
                            mTransportBar.CurrentSelectedNode = value.Node;
                        }
                    }
                    else
                    {
                        Select(value);
                    }
                }
            }
        }

        private void Select(NodeSelection value)
        {
            Deselect();
            mCurrentSelection = value;
            value.Control.CurrentSelectedNode = value.Node;
            System.Diagnostics.Debug.Print("+++ SELECTED {0} +++", value);
        }

        /// <summary>
        /// Deselect the currently selection node (if any) in its control.
        /// </summary>
        private void Deselect()
        {
            if (mCurrentSelection != null)
            {
                mCurrentSelection.Control.CurrentSelectedNode = null;
                System.Diagnostics.Debug.Print("--- DESELECTED ---");
            }
        }

        /// <summary>
        /// Currently selected node, regardless of its type or where it is selected.
        /// Null if nothing is selected.
        /// </summary>
        public ObiNode CurrentSelectionNode
        {
            get { return mCurrentSelection == null ? null : mCurrentSelection.Node; }
        }

        /// <summary>
        /// Section node for the currently selected section in the TOC panel.
        /// Null if there is no such selection.
        /// </summary>
        public SectionNode CurrentSelectedSection
        {
            get
            {
                return mCurrentSelection == null || mCurrentSelection.Control != mTOCPanel ?
                    null : mCurrentSelection.Node as SectionNode;
            }   
        }

        /// <summary>
        /// Section node for the currently selected strip in the strip manager panel.
        /// Null if there is no such selection.
        /// </summary>
        public SectionNode CurrentSelectedStrip
        {
            get
            {
                return mCurrentSelection == null || mCurrentSelection.Control != mStripManagerPanel ?
                    null : mCurrentSelection.Node as SectionNode;
            }
        }

        /// <summary>
        /// Phrase node for the currently selected audio block (if any, null otherwise.)
        /// </summary>
        public PhraseNode CurrentSelectedAudioBlock
        {
            get { return mCurrentSelection == null ? null : mCurrentSelection.Node as PhraseNode; }
        }

        /// <summary>
        /// Get a label for the node currently selected, i.e. "" if nothing is seleced,
        /// "audio block" for an audio block, "strip" for a strip and "section" for a
        /// section.
        /// </summary>
        public string SelectedLabel
        {
            get
            {
                return mCurrentSelection == null ? "" :
                    mCurrentSelection.Node is PhraseNode ? Localizer.Message("audio_block") :
                    mCurrentSelection.Control == mStripManagerPanel ? Localizer.Message("strip") :
                    Localizer.Message("section");
            }
        }

        #endregion


        #region properties

        /// <summary>
        /// An audio block can be toggled if and only if it is in a used section.
        /// </summary>
        public bool CanToggleAudioBlock
        {
            get
            {
                PhraseNode selected = CurrentSelectedAudioBlock;
                return selected != null && selected.ParentSection.Used;
            }
        }

        /// <summary>
        /// A section's used state can be changed only if it or its parent is used
        /// (cannot have a used section under an unused section), or it is the root.
        /// </summary>
        public bool CanToggleSection
        {
            get
            {
                SectionNode selected = CurrentSelectedSection;
                if (selected == null) return false;
                if (selected.Used) return true;
                SectionNode parent = selected.ParentSection;
                return parent == null || parent.Used;
            }
        }

        public bool EnableTooltips
        {
            get { return mEnableTooltips; }
            set
            {
                mEnableTooltips = value;
                mTOCPanel.EnableTooltips = value;
                mTransportBar.EnableTooltips = value;
                mStripManagerPanel.EnableTooltips = value;
            }
        }

        /// <summary>
        /// Return the parent form, which is supposed to be an ObiForm.
        /// </summary>
        public ObiForm ParentObiForm
        {
            get { return (ObiForm)ParentForm; }
        }

        /// <summary>
        /// Set the project for the panel, as well as all the correct handlers.
        /// </summary>
        public Project Project
        {
            get { return mProject; }
            set
            {
                if (mProject != null)
                {
                    UnsetEventHandlers();
                    if (value == null) Deselect();
                }
                if (value != null) SetEventHandlers(value);
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                mSplitContainer.Panel1Collapsed = false;
                mNoProjectLabel.Visible = mProject == null;
                mTransportBar.UpdatedProject();
            }
        }

        #region ugly event stuff

        /// <summary>
        /// Set event handlers for the new project.
        /// </summary>
        /// <param name="project">The new project.</param>
        private void SetEventHandlers(Project project)
        {
            project.AddedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
            project.AddedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);
            project.MovedSectionNode += new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.MovedSectionNode += new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
            project.UndidMoveSectionNode += new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.UndidMoveSectionNode += new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
            project.DecreasedSectionNodeLevel += new Events.SectionNodeHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);
            project.DecreasedSectionNodeLevel += new Events.SectionNodeHandler(mStripManagerPanel.SyncDecreaseSectionNodeLevel);
            project.RenamedSectionNode += new Events.RenameSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            project.RenamedSectionNode += new Events.RenameSectionNodeHandler(mStripManagerPanel.SyncRenamedNode);
            project.DeletedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            project.DeletedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedSectionNode);
            project.PastedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            project.PastedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            project.UndidPasteSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            project.UndidPasteSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);

            project.AddedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            project.DeletedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
            project.MediaSet += new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);
            project.TouchedNode += new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
            project.UpdateTime += new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);
            project.RemovedPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            project.SetPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);

            project.ToggledNodeUsedState += new Obi.Events.ObiNodeHandler(mStripManagerPanel.ToggledNodeUsedState);
            project.ToggledNodeUsedState += new Obi.Events.ObiNodeHandler(mTOCPanel.ToggledNodeUsedState);

            project.HeadingChanged +=new Obi.Events.SectionNodeHeadingHandler(mStripManagerPanel.SyncHeadingChanged);
        }

        /// <summary>
        /// Unset event handlers from the old project (still set.)
        /// </summary>
        private void UnsetEventHandlers()
        {
            mProject.AddedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
            mProject.AddedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

            mProject.MovedSectionNode -= new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.MovedSectionNode -= new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
            mProject.UndidMoveSectionNode -= new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.UndidMoveSectionNode -= new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);

            mProject.DecreasedSectionNodeLevel -= new Events.SectionNodeHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);
            mProject.DecreasedSectionNodeLevel -= new Events.SectionNodeHandler(mStripManagerPanel.SyncDecreaseSectionNodeLevel);

            mProject.RenamedSectionNode -= new Events.RenameSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            mProject.RenamedSectionNode -= new Events.RenameSectionNodeHandler(mStripManagerPanel.SyncRenamedNode);

            mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedSectionNode);

            mProject.AddedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);

            mProject.MediaSet -= new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);

            mProject.DeletedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
          
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);

            mProject.TouchedNode -= new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
            mProject.UpdateTime -= new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

            mProject.RemovedPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            mProject.SetPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);

            mProject.ToggledNodeUsedState -= new Obi.Events.ObiNodeHandler(mStripManagerPanel.ToggledNodeUsedState);
            mProject.ToggledNodeUsedState -= new Obi.Events.ObiNodeHandler(mTOCPanel.ToggledNodeUsedState);

            mProject.HeadingChanged -= new Obi.Events.SectionNodeHeadingHandler(mStripManagerPanel.SyncHeadingChanged);
        }

        #endregion

        /// <summary>
        /// The strip manager for this project.
        /// </summary>
        public StripManagerPanel StripManager
        {
            get { return mStripManagerPanel; }
        }

        /// <summary>
        /// TOC panel can be visible (true) or hidden (false).
        /// </summary>
        public Boolean TOCPanelVisible
        {
            get { return mProject != null && !mSplitContainer.Panel1Collapsed; }
        }

        /// <summary>
        /// The TOC panel for this project.
        /// </summary>
        public TOCPanel TOCPanel
        {
            get { return mTOCPanel; }
        }

        /// <summary>
        /// String to show in the menu: "mark audio block as used/unused"
        /// </summary>
        public string ToggleAudioBlockString
        {
            get
            {
                return String.Format(Localizer.Message("mark_x_as_y"), Localizer.Message("audio_block"),
                    Localizer.Message(CanToggleAudioBlock && CurrentSelectedAudioBlock.Used ?
                    "unused" : "used"));
            }
        }

        /// <summary>
        /// String to show in the menu: "mark section as used/unused"
        /// </summary>
        public string ToggleSectionString
        {
            get
            {
                return String.Format(Localizer.Message("mark_x_as_y"), Localizer.Message("section"),
                    Localizer.Message(CanToggleSection && CurrentSelectedSection.Used ?
                    "unused" : "used"));
            }
        }

        /// <summary>
        /// The transport bar for the project.
        /// </summary>
        public TransportBar TransportBar
        {
            get { return mTransportBar; }
        }

        #endregion


        /// <summary>
        /// Hide the TOC panel.
        /// </summary>
        public void HideTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = true;
        }

        /// <summary>
        /// Show the TOC panel.
        /// </summary>
        public void ShowTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = false;
        }


        /// <summary>
        /// Synchronize the project views with the core tree and initialize the playlist for the transport bar.
        /// Used when opening a XUK file or touching the project.
        /// </summary>
        public void SynchronizeWithCoreTree()
        {
            mTOCPanel.SynchronizeWithCoreTree(mProject.RootNode);
            mStripManagerPanel.SynchronizeWithCoreTree(mProject.RootNode);
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
            else if (mCurrentSelection == null)
            {
                // no selection: can only paste a section
                return mProject != null && node is SectionNode;
            }
            else if (node is SectionNode)
            {
                // a section can only be pasted under a section
                // same for strips
                return mCurrentSelection.Node is SectionNode;
            }
            else
            {
                // a phrase can ony be pasted in the strip view
                return mCurrentSelection.Control == mStripManagerPanel;
            }
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
                    mCurrentSelection != null &&
                    mCurrentSelection.Control == mTOCPanel ? "section" :    // pasting in TOC panel
                    ((SectionNode)node).SectionChildCount > 0 ? "strips" :  // pasting several strips
                    "strip");                                               // pasting only one strip
        }

        #region edit

        /// <summary>
        /// Cut the selected node (and deselect it!)
        /// </summary>
        public void Cut()
        {
            TransportBar.Enabled = false;
            if (CurrentSelectedAudioBlock != null)
            {
                mProject.CutPhraseNode(CurrentSelectedAudioBlock);
                CurrentSelection = null;
            }
            else if (CurrentSelectedStrip != null)
            {
                mProject.ShallowCutSectionNode(CurrentSelectedStrip);
                CurrentSelection = null;
            }
            else if (CurrentSelectedSection != null)
            {
                mProject.CutSectionNode(CurrentSelectedSection);
                CurrentSelection = null;
            }
            TransportBar.Enabled = true;
        }

        public void Copy()
        {
            TransportBar.Enabled = false;
            if (CurrentSelectedAudioBlock != null)
            {
                mProject.CopyPhraseNode(CurrentSelectedAudioBlock);
            }
            else if (CurrentSelectedStrip != null)
            {
                mProject.ShallowCopySectionNode(CurrentSelectedStrip, true);
            }
            else if (CurrentSelectedSection != null)
            {
                mProject.CopySectionNode(CurrentSelectedSection);
            }
            TransportBar.Enabled = true;
        }

        /// <summary>
        /// Delete the selected node (and deselect it!)
        /// </summary>
        public void Delete()
        {
            TransportBar.Enabled = false;
            if (CurrentSelectedAudioBlock != null)
            {
                // Avn: Following check added to prevent deleting when shortcuts are disabled like during renaming.
                // These checks are not added at lower layer like inside stripmanager so as to avoid problems in internally issued delete commands
                if (mStripManagerPanel.AllowShortcuts)
                {
                    mProject.DeletePhraseNode(CurrentSelectedAudioBlock);
                    CurrentSelection = null;
                }
            }
            else if (CurrentSelectedStrip != null)
            {
                // to review!
                                                    mProject.ShallowDeleteSectionNode(this, CurrentSelectedStrip);
                    CurrentSelection = null;
                            }
            else if (CurrentSelectedSection != null)
            {
                // Avn: Following check added to prevent deleting  during during renaming etc.
                if (mTOCPanel.AllowDelete)
                {
                    mProject.DeleteSectionNode(CurrentSelectedSection);
                    CurrentSelection = null;
                }
                            }
            TransportBar.Enabled = true;
        }

        /// <summary>
        /// Paste the node in the clipboard in the selection context.
        /// </summary>
        public void Paste()
        {
            TransportBar.Enabled = false;
            if (CurrentSelection != null)
            {
                if (mProject.Clipboard.Section != null)
                {
                    IControlWithSelection control = CurrentSelection.Control;
                    SectionNode pasted = mProject.PasteSectionNode(CurrentSelectionNode);
                    CurrentSelection = new NodeSelection(pasted, control);
                }
                else if (mProject.Clipboard.Phrase != null)
                {
                    PhraseNode pasted = mProject.PastePhraseNode(mProject.Clipboard.Phrase, CurrentSelectionNode);
                    CurrentSelection = new NodeSelection(pasted, mStripManagerPanel);
                }
            }
            else
            { 
                //TODO: figure out how to paste as append to the root
            }
            TransportBar.Enabled = true;
        }

        #endregion
    }
}
