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
                // Reset the handlers from the previous project
                if (mProject != null)
                {
                    //md test 20061207
                    mStripManagerPanel.Selected -= new Events.SelectedHandler(mProject.test_StripManagerSelect);
                    mTOCPanel.Selected -= new Events.SelectedHandler(mProject.test_TOCSelect);

                    mTOCPanel.AddSiblingSectionRequested -= new Events.SectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);
                    mStripManagerPanel.AddSiblingSectionRequested -=
                        new Events.SectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);

                    mProject.AddedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
                    mProject.AddedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

                    mTOCPanel.AddChildSectionNodeRequested -= new Events.SectionNodeHandler(mProject.CreateChildSectionNodeRequested);

                    mProject.MovedSectionNode -= new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    mProject.MovedSectionNode -= new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
                    mProject.UndidMoveSectionNode -= new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    mProject.UndidMoveSectionNode -= new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
                
                    mTOCPanel.IncreaseSectionNodeLevelRequested -= new Events.SectionNodeHandler(mProject.IncreaseSectionNodeLevelRequested);
                    //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

                    mTOCPanel.DecreaseSectionNodeLevelRequested -= new Events.SectionNodeHandler(mProject.DecreaseSectionNodeLevelRequested);
                    mProject.DecreasedSectionNodeLevel -= new Events.SectionNodeHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);
                    mProject.DecreasedSectionNodeLevel -= new Events.SectionNodeHandler(mStripManagerPanel.SyncDecreaseSectionNodeLevel);

                    mTOCPanel.RenameSectionNodeRequested -= new Events.RenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
                    mStripManagerPanel.RenameSectionRequested -= new Events.RenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
                    mProject.RenamedSectionNode -= new Events.RenameSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
                    mProject.RenamedSectionNode -= new Events.RenameSectionNodeHandler(mStripManagerPanel.SyncRenamedNode);

                    mTOCPanel.DeleteSectionNodeRequested -= new Events.SectionNodeHandler(mProject.RemoveSectionNodeRequested);
                    mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
                    mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedSectionNode);

                    mStripManagerPanel.ImportAudioAssetRequested -= new Events.RequestToImportAssetHandler(mProject.ImportAssetRequested);
                    //mProject.ImportedAsset -= new Events.Node.ImportedAssetHandler(mStripManagerPanel.SyncCreateNewAudioBlock);
                    mProject.AddedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);

                    mStripManagerPanel.SetMediaRequested -= new Events.SetMediaHandler(mProject.SetMediaRequested);
                    mProject.MediaSet -= new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);

                    mStripManagerPanel.SplitAudioBlockRequested -= new Events.SplitPhraseNodeHandler(mProject.SplitAudioBlockRequested);
                    mStripManagerPanel.RequestToApplyPhraseDetection -=
                        new Events.RequestToApplyPhraseDetectionHandler(mProject.ApplyPhraseDetection);

                    mStripManagerPanel.MergeNodes -= new Events.MergePhraseNodesHandler(mProject.MergeNodesRequested);

                    mStripManagerPanel.DeleteBlockRequested -=
                        new Events.PhraseNodeHandler(mProject.DeletePhraseNodeRequested);
                    mProject.DeletedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);

                    mTOCPanel.CutSectionNodeRequested -= new Events.SectionNodeHandler(mProject.CutSectionNodeRequested);
                    mProject.CutSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncCutSectionNode);
                    mProject.CutSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);

                    mTOCPanel.CopySectionNodeRequested -= new Events.SectionNodeHandler(mProject.CopySectionNodeRequested);
                    mProject.CopiedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
                    mProject.CopiedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
                    mProject.UndidCopySectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
                    mProject.UndidCopySectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.PasteSectionNodeRequested -= new Events.SectionNodeHandler(mProject.PasteSectionNodeRequested);
                    mProject.PastedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
                    mProject.PastedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
                    mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
                    mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);

                    mProject.TouchedNode -= new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
                    mProject.UpdateTime -= new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

                    //md 20060812
                    mStripManagerPanel.RequestToShallowDeleteSectionNode -= new Events.SectionNodeHandler(mProject.ShallowDeleteSectionNodeRequested);

                    mStripManagerPanel.RequestToCutSectionNode -=
                        new Events.SectionNodeHandler(mProject.CutSectionNodeRequested);
                    mStripManagerPanel.RequestToCutPhraseNode -=
                        new Events.PhraseNodeHandler(mProject.CutPhraseNode);
                    mStripManagerPanel.RequestToCopyPhraseNode -=
                        new Events.PhraseNodeHandler(mProject.CopyPhraseNode);
                    mStripManagerPanel.RequestToPastePhraseNode -=
                        new Events.NodeEventHandler(mProject.PastePhraseNode);
                    mStripManagerPanel.RequestToSetPageNumber -= new Events.RequestToSetPageNumberHandler(mProject.SetPageRequested);
                    mStripManagerPanel.RequestToRemovePageNumber -=
                        new Events.PhraseNodeHandler(mProject.RemovePageRequested);
                    mProject.RemovedPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
                    mProject.SetPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);
                }
                // Set up the handlers for the new project
                if (value != null)
                {
                    //md test 20061207
                    mStripManagerPanel.Selected += new Events.SelectedHandler(value.test_StripManagerSelect);
                    mTOCPanel.Selected += new Events.SelectedHandler(value.test_TOCSelect);

                    mTOCPanel.AddSiblingSectionRequested += new Events.SectionNodeHandler(value.CreateSiblingSectionNodeRequested);
                    mStripManagerPanel.AddSiblingSectionRequested += new Events.SectionNodeHandler(value.CreateSiblingSectionNodeRequested);
                    value.AddedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
                    value.AddedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

                    mTOCPanel.AddChildSectionNodeRequested += new Events.SectionNodeHandler(value.CreateChildSectionNodeRequested);
  
                    value.MovedSectionNode += new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    value.MovedSectionNode += new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
                    
                    value.UndidMoveSectionNode += new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    value.UndidMoveSectionNode += new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
                    
                    mTOCPanel.IncreaseSectionNodeLevelRequested += new Events.SectionNodeHandler(value.IncreaseSectionNodeLevelRequested);
                    //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

                    mTOCPanel.DecreaseSectionNodeLevelRequested += new Events.SectionNodeHandler(value.DecreaseSectionNodeLevelRequested);
                    value.DecreasedSectionNodeLevel += new Events.SectionNodeHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);
                    value.DecreasedSectionNodeLevel += new Events.SectionNodeHandler(mStripManagerPanel.SyncDecreaseSectionNodeLevel);

                    mTOCPanel.RenameSectionNodeRequested += new Events.RenameSectionNodeHandler(value.RenameSectionNodeRequested);
                    mStripManagerPanel.RenameSectionRequested += new Events.RenameSectionNodeHandler(value.RenameSectionNodeRequested);
                    value.RenamedSectionNode += new Events.RenameSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
                    value.RenamedSectionNode += new Events.RenameSectionNodeHandler(mStripManagerPanel.SyncRenamedNode);

                    mTOCPanel.DeleteSectionNodeRequested += new Events.SectionNodeHandler(value.RemoveSectionNodeRequested);
                    value.DeletedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
                    value.DeletedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedSectionNode);

                    // Block events

                    mStripManagerPanel.RequestToCutSectionNode += new Events.SectionNodeHandler(value.CutSectionNodeRequested);
                    mStripManagerPanel.RequestToCutPhraseNode += new Events.PhraseNodeHandler(value.CutPhraseNode);
                    mStripManagerPanel.RequestToCopyPhraseNode += new Events.PhraseNodeHandler(value.CopyPhraseNode);
                    mStripManagerPanel.RequestToPastePhraseNode += new Events.NodeEventHandler(value.PastePhraseNode);

                    mStripManagerPanel.ImportAudioAssetRequested += new Events.RequestToImportAssetHandler(value.ImportAssetRequested);
                    mStripManagerPanel.DeleteBlockRequested += new Events.PhraseNodeHandler(value.DeletePhraseNodeRequested);
                    mStripManagerPanel.MoveAudioBlockForwardRequested += new Events.PhraseNodeHandler(value.MovePhraseNodeForwardRequested);
                    mStripManagerPanel.MoveAudioBlockBackwardRequested += new Events.PhraseNodeHandler(value.MovePhraseNodeBackwardRequested);
                    mStripManagerPanel.SetMediaRequested += new Events.SetMediaHandler(value.SetMediaRequested);
                    mStripManagerPanel.SplitAudioBlockRequested += new Events.SplitPhraseNodeHandler(value.SplitAudioBlockRequested);
                    mStripManagerPanel.RequestToApplyPhraseDetection += new Events.RequestToApplyPhraseDetectionHandler(value.ApplyPhraseDetection);

                    value.AddedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
                    value.DeletedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
                    value.MediaSet += new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);
                    value.TouchedNode += new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
                    value.UpdateTime += new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

                    mStripManagerPanel.MergeNodes += new Events.MergePhraseNodesHandler(value.MergeNodesRequested);

                    //md: clipboard in the TOC
                    mTOCPanel.CutSectionNodeRequested += new Events.SectionNodeHandler(value.CutSectionNodeRequested);
                    value.CutSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncCutSectionNode);
                    value.CutSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);

                    mTOCPanel.CopySectionNodeRequested += new Events.SectionNodeHandler(value.CopySectionNodeRequested);
                    value.CopiedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
                    value.CopiedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
                    value.UndidCopySectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
                    value.UndidCopySectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.PasteSectionNodeRequested += new Events.SectionNodeHandler(value.PasteSectionNodeRequested);
                    value.PastedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
                    value.PastedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
                    value.UndidPasteSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
                    value.UndidPasteSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);


                    //md 20060812
                    mStripManagerPanel.RequestToShallowDeleteSectionNode += new Events.SectionNodeHandler(value.ShallowDeleteSectionNodeRequested);

                    mStripManagerPanel.RequestToSetPageNumber += new Events.RequestToSetPageNumberHandler(value.SetPageRequested);
                    mStripManagerPanel.RequestToRemovePageNumber += new Events.PhraseNodeHandler(value.RemovePageRequested);
                    value.RemovedPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
                    value.SetPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);
                }
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                mSplitContainer.Panel1Collapsed = false;
                mNoProjectLabel.Text = mProject == null ? Localizer.Message("no_project") : "";
                mTransportBar.Visible = mProject != null;
            }
        }

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
        /// Return the node that is selected in either view, if any.
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
    }
}
