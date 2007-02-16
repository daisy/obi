using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// Top level panel that displays the current project, using a splitter (TOC on the left, strips on the right.)
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private Project mProject;  // the project to display

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
                mNoProjectLabel.Text = mProject == null ? Localizer.Message("no_project") : "";
                mTransportBar.Visible = mProject != null;
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

            mStripManagerPanel.SetPageNumberRequested += new Events.RequestToSetPageNumberHandler(project.SetPageRequested);
            mStripManagerPanel.RemovePageNumberRequested += new Events.PhraseNodeHandler(project.RemovePageRequested);
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

            mStripManagerPanel.SetPageNumberRequested -= new Events.RequestToSetPageNumberHandler(mProject.SetPageRequested);
            mStripManagerPanel.RemovePageNumberRequested -=
                new Events.PhraseNodeHandler(mProject.RemovePageRequested);
            mProject.RemovedPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            mProject.SetPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);

            mProject.ToggledNodeUsedState -= new Obi.Events.ObiNodeHandler(mStripManagerPanel.ToggledNodeUsedState);
            mProject.ToggledNodeUsedState -= new Obi.Events.ObiNodeHandler(mTOCPanel.ToggledNodeUsedState);
        }

        #endregion

        /// <summary>
        /// TOC panel can be visible (true) or hidden (false).
        /// </summary>
        public Boolean TOCPanelVisible
        {
            get { return mProject != null && !mSplitContainer.Panel1Collapsed; }
        }

        /// <summary>
        /// The strip manager for this project.
        /// </summary>
        public StripManagerPanel StripManager
        {
            get { return mStripManagerPanel; }
        }

        /// <summary>
        /// The TOC panel for this project.
        /// </summary>
        public TOCPanel TOCPanel
        {
            get { return mTOCPanel; }
        }

        /// <summary>
        /// The transport bar for the project.
        /// </summary>
        public TransportBar TransportBar
        {
            get { return mTransportBar; }
        }

        /// <summary>
        /// Return the node that is selected in either view, or null if no node is selected.
        /// </summary>
        public ObiNode SelectedNode
        {
            get
            {
                return mStripManagerPanel.SelectedNode != null ?
                        mStripManagerPanel.SelectedNode :
                    mTOCPanel.IsNodeSelected ?
                        mTOCPanel.SelectedSection : null;
            }
        }

        /// <summary>
        /// Return the section node currently selected, either in the strip manager or the TOC view.
        /// If no section node is selected, return null.
        /// </summary>
        public SectionNode SelectedSection
        {
            get
            {
                return mStripManagerPanel.SelectedSectionNode != null ?
                    mStripManagerPanel.SelectedSectionNode :
                    mTOCPanel.SelectedSection;
            }
        }

        /// <summary>
        /// True if there is a node currently selected and it can be cut/copied/deleted.
        /// The selected node must be used in order to do that.
        /// </summary>
        public bool CanCutCopyDeleteNode
        {
            get
            {
                ObiNode selected = SelectedNode;
                return selected != null && selected.Used;
            }
        }

        /// <summary>
        /// True if there is currently a selected section and its use status
        /// can be toggled (i.e. its parent is used)
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
        /// True if there is currently a selected strip and its use status
        /// can be toggled (i.e. its parent is used)
        /// Can check for subsections as well? [optional]
        /// </summary>
        public bool CanToggleStrip
        {
            get
            {
                SectionNode selected = mStripManagerPanel.SelectedSectionNode;
                if (selected == null) return false;
                if (selected.SectionChildCount > 0) return false;  // optional
                if (selected.Used) return true;
                SectionNode parent = selected.ParentSection;
                return parent == null || parent.Used;
            }
        }

        /// <summary>
        /// String to show in the menu: "mark audio block as used/unused"
        /// </summary>
        public string ToggleStripString
        {
            get
            {
                return String.Format(Localizer.Message("mark_x_as_y"), Localizer.Message("strip"),
                    Localizer.Message(CanToggleStrip && !mStripManagerPanel.SelectedSectionNode.Used ?
                    "used" : "unused"));
            }
        }

        /// <summary>
        /// True if there is currently a selected audio block and its use
        /// status can be toggled (its parent section must be used.)
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
        /// String to show in the menu: "mark audio block as used/unused"
        /// </summary>
        public string ToggleAudioBlockString
        {
            get
            {
                return String.Format(Localizer.Message("mark_x_as_y"), Localizer.Message("audio_block"),
                    Localizer.Message(CanToggleAudioBlock && !mStripManagerPanel.SelectedPhraseNode.Used ?
                    "used" : "unused"));
            }
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
                return mStripManagerPanel.SelectedPhraseNode != null ? Localizer.Message("audio_block") :
                    mStripManagerPanel.SelectedSectionNode != null ? Localizer.Message("strip") :
                    mTOCPanel.SelectedSection != null ? Localizer.Message("section") : "";
            }
        }

        /// <summary>
        /// Create a new project panel with currently no project.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            mTOCPanel.ProjectPanel = this;
            mStripManagerPanel.ProjectPanel = this;
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
            mTransportBar.Playlist = new Playlist(mProject, Audio.AudioPlayer.Instance);
        }

        //things to deselect:
        /*StripManagerPanel.mPhraseNodeMap[mSelectedPhrase] (audio blocks)
	    StripManagerPanel.mSectionNodeMap[mSelectedSection] (section strip)
	    annotation block (future)
	    page block (future)
	    TOC panel node	*/
        //IMPORTANT! don't raise anything like "SelectionChanged" events
        internal void DeselectEverything()
        {
            mTOCPanel.SelectedSection = null;
            mStripManagerPanel.SelectedPhraseNode = null;// SelectedBlock = null;
            mStripManagerPanel.SelectedSectionNode = null;// SelectedSectionStrip = null;
        }
        
        internal void test_WidgetSelect(object sender, Obi.Events.Node.SelectedEventArgs e)
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
        /// True if the given node can be pasted. Some rules for pasting:
        ///   (i) there must be something to paste,
        ///   (ii) the project must be open,
        ///   (iii) if no node is selected, the context is understood to be the context panel
        ///     and the root node; as such, only sections can be pasted,
        ///   (iv) a phrase node cannot be pasted in the TOC panel,
        ///   (v) if the selected node is a phrase, only a phrase can be pasted,
        ///   (vi) if the selected node is an unused section, then a section can be pasted only
        ///     if the parent is the root or is used.
        ///   (vii) if the selected node is an unused phrase, then a phrse can be pasted only
        ///     if the parent section is used and the selected phrase is followed by a used
        ///     phrase or is the last of its section.
        ///   (viii) there is no viii.
        /// </summary>
        /// <param name="node">The node to paste (coming from the clipboard.) Null if no node is in the clipboard.</param>
        public bool CanPaste(ObiNode node)
        {
            if (node == null)
            {
                return false;
            }
            else
            {
                ObiNode selected = SelectedNode;
                if (selected == null)
                {
                    // the context is the root so only a section node can be pasted.
                    return mProject != null && node is SectionNode;
                }
                else
                {
                    if (node is SectionNode)
                    {
                        // a section node can be pasted only in the context of a section node,
                        // which parent or self must be used
                        return (selected is SectionNode) &&
                            (selected.Used ||
                            ((SectionNode)selected).ParentSection != null && ((SectionNode)selected).ParentSection.Used);
                    }
                    else
                    {
                        if (selected is PhraseNode && !selected.Used)
                        {
                            // pasting after an unused phrase node works if its last or
                            // followed by a used node in a used section
                            SectionNode parent = ((PhraseNode)selected).ParentSection;
                            return selected.Index == parent.PhraseChildCount - 1 ||
                                parent.PhraseChild(selected.Index + 1).Used;
                        }
                        else
                        {
                            // must be in the strip manager (i.e. not in the TOC panel)
                            return mTOCPanel.SelectedSection == null;
                        }
                    }
                }
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

        internal void HandleShortcutKeys(Keys key)
        {
            bool handled = mTransportBar.HandleShortcutKeys(key);
        }
    }
}
