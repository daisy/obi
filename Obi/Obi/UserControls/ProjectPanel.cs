using System;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// Top level panel that displays the current project, using a splitter (TOC on the left, strips on the right.)
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private Project mProject;       // the project to display
        private ObiNode mSelectedNode;  // the selected node in one of the views (null if none)

        #region properties

        /// <summary>
        /// Return the parent form, which is supposed to be an ObiForm.
        /// </summary>
        public ObiForm ParentObiForm
        {
            get { return (ObiForm)ParentForm; }
        }

        /// <summary>
        /// True if there is a node currently selected and it can be cut/copied/deleted.
        /// </summary>
        public bool CanCutCopyDeleteNode
        {
            get { return mSelectedNode != null; }
        }

        /// <summary>
        /// An audio block's status can be changed only if its parent section is used.
        /// </summary>
        public bool CanToggleAudioBlock
        {
            get
            {
                PhraseNode selected = mStripManagerPanel.SelectedPhraseNode;
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
                SectionNode selected = mTOCPanel.SelectedSection;
                if (selected == null) return false;
                if (selected.Used) return true;
                SectionNode parent = selected.ParentSection;
                return parent == null || parent.Used;
            }
        }

        /// <summary>
        /// Set the project for the panel, as well as all the correct handlers.
        /// </summary>
        public Project Project
        {
            get { return mProject; }
            set
            {
                if (mProject != null) UnsetEventHandlers();
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
            //md test 20061207
            mStripManagerPanel.SelectionChanged += new Events.SelectedHandler(this.test_WidgetSelect);
            mTOCPanel.SelectionChanged += new Events.SelectedHandler(this.test_WidgetSelect);

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

            // Block events

            mStripManagerPanel.SetMediaRequested += new Events.SetMediaHandler(project.SetMediaRequested);

            project.AddedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            project.DeletedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
            project.MediaSet += new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);
            project.TouchedNode += new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
            project.UpdateTime += new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

            project.PastedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            project.PastedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            project.UndidPasteSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            project.UndidPasteSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);


            //md 20060812
            mStripManagerPanel.ShallowDeleteSectionNodeRequested += new Events.SectionNodeHandler(project.ShallowDeleteSectionNodeRequested);

            project.RemovedPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            project.SetPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);

            project.ToggledNodeUsedState += new Obi.Events.ObiNodeHandler(mStripManagerPanel.ToggledNodeUsedState);
            project.ToggledNodeUsedState += new Obi.Events.ObiNodeHandler(mTOCPanel.ToggledNodeUsedState);
        }

        /// <summary>
        /// Unset event handlers from the old project (still set.)
        /// </summary>
        private void UnsetEventHandlers()
        {
            //md test 20061207
            mStripManagerPanel.SelectionChanged -= new Events.SelectedHandler(this.test_WidgetSelect);
            mTOCPanel.SelectionChanged -= new Events.SelectedHandler(this.test_WidgetSelect);

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

            mStripManagerPanel.SetMediaRequested -= new Events.SetMediaHandler(mProject.SetMediaRequested);
            mProject.MediaSet -= new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);

            mProject.DeletedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
          
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);

            mProject.TouchedNode -= new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
            mProject.UpdateTime -= new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

            //md 20060812
            mStripManagerPanel.ShallowDeleteSectionNodeRequested -= new Events.SectionNodeHandler(mProject.ShallowDeleteSectionNodeRequested);

            mProject.RemovedPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            mProject.SetPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);

            mProject.ToggledNodeUsedState -= new Obi.Events.ObiNodeHandler(mStripManagerPanel.ToggledNodeUsedState);
            mProject.ToggledNodeUsedState -= new Obi.Events.ObiNodeHandler(mTOCPanel.ToggledNodeUsedState);
        }

        #endregion

        /// <summary>
        /// Get a label for the node currently selected, i.e. "" if nothing is seleced,
        /// "audio block" for an audio block, "strip" for a strip and "section" for a
        /// section.
        /// </summary>
        public string SelectedLabel
        {
            get
            {
                return mStripManagerPanel.SelectedPhraseNode != null ? Localizer.Message("audio_block") :
                    mStripManagerPanel.SelectedSectionNode != null ? Localizer.Message("strip") :
                    mTOCPanel.SelectedSection != null ? Localizer.Message("section") : "";
            }
        }

        /// <summary>
        /// Selected node in the panel.
        /// To set the selection, use the StripView or the TOCPanel facilities.
        /// </summary>
        public ObiNode SelectedNode
        {
            get { return mSelectedNode; }
            set { mSelectedNode = value; }
        }

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
                    Localizer.Message(CanToggleAudioBlock && mStripManagerPanel.SelectedPhraseNode.Used ?
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
                    Localizer.Message(CanToggleSection && mTOCPanel.SelectedSection.Used ?
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
        /// Create a new project panel with currently no project.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            mTOCPanel.ProjectPanel = this;
            mStripManagerPanel.ProjectPanel = this;
            mTransportBar.ProjectPanel = this;
            mTransportBar.Enabled = false;
            Project = null;
        }

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
        /// Deselect everything in controls other than the sender.
        /// </summary>
        public void DeselectEverythingElse(object from)
        {
            if (from is StripManagerPanel)
            {
                mTOCPanel.SelectedSection = null;
            }
            else if (from is TOCPanel)
            {
                mStripManagerPanel.SelectedNode = null;
            }
            mSelectedNode = null;
        }

        /// <summary>
        /// This test method shows what gets selected when.
        /// </summary>
        public void test_WidgetSelect(object sender, Obi.Events.Node.SelectedEventArgs e)
        {
            CoreNode target = null;
            if (e.Widget is Obi.UserControls.SectionStrip)
            {
                System.Diagnostics.Debug.Write("SectionStrip - ");
                Obi.UserControls.SectionStrip strip = (Obi.UserControls.SectionStrip)e.Widget;
                target = strip.Node;
            }
            else if (e.Widget is Obi.UserControls.AudioBlock)
            {
                System.Diagnostics.Debug.Write("AudioBlock - ");
                Obi.UserControls.AudioBlock block = (Obi.UserControls.AudioBlock)e.Widget;
                target = block.Node;
            }
            else if (e.Widget is System.Windows.Forms.TreeNode)
            {
                System.Diagnostics.Debug.Write("TOC.TreeNode - ");
                System.Windows.Forms.TreeNode treenode = (System.Windows.Forms.TreeNode)e.Widget;
                target = this.mTOCPanel.SelectedSection;
            }

            if (target != null) System.Diagnostics.Debug.Write(target.GetType().ToString() + ": ");
            else System.Diagnostics.Debug.Write("!target node is null");

            string text = "";
            if (target is SectionNode) text = Project.GetTextMedia((CoreNode)target).getText();
            if (target is PhraseNode) text = ((urakawa.media.TextMedia)Project.GetMediaForChannel((CoreNode)target, Project.AnnotationChannelName)).getText();
            System.Diagnostics.Debug.Write("\"" + text + "\"");
            if (e.Selected) System.Diagnostics.Debug.Write(" is selected\n");
            else System.Diagnostics.Debug.Write(" is deselected\n");
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
            else if (mSelectedNode == null)
            {
                // no selection: can only paste a section
                return mProject != null && node is SectionNode;
            }
            else if (node is SectionNode)
            {
                // a section can only be pasted under a section
                return mSelectedNode is SectionNode;
            }
            else
            {
                // a phrase can be pasted anywhere as long as a node is selected in the strip view
                // but not in the toc view
                return mTOCPanel.SelectedSection == null;
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
                    mTOCPanel.SelectedSection != null ? "section" :         // pasting in TOC panel
                    ((SectionNode)node).SectionChildCount > 0 ? "strips" :  // pasting several strips
                    "strip");                                               // pasting only one strip
        }
    }
}
