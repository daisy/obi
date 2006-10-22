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

        public Project Project
        {
            get { return mProject; }
            set
            {
                if (mProject != null) ResetEventHandlers();
                if (value != null) SetEventHandlers(value);
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                ShowTOCPanel();
            }
        }

        #region Setting/resetting handlers

        /// <summary>
        /// Set all the event handlers for the panel
        /// </summary>
        /// <param name="value">The project for which we are setting handlers.</param>
        private void SetEventHandlers(Project project)
        {
            project.AddedPhraseNode +=
                new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            project.AddedSectionNode +=
                new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedEmptySectionNode);
            project.AddedSectionNode +=
                new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);

            mStripManagerPanel.DeleteBlockRequested +=
                new Events.PhraseNodeHandler(project.DeletePhraseNodeRequested);
            mStripManagerPanel.ImportAudioAssetRequested +=
                new Events.Strip.RequestToImportAssetHandler(project.ImportAssetRequested);
            mStripManagerPanel.MergePhraseNodes +=
                new Events.MergePhraseNodesHandler(project.MergePhraseNodesRequested);
            mStripManagerPanel.MoveAudioBlockBackwardRequested +=
                new Events.PhraseNodeHandler(project.MovePhraseNodeBackwardRequested);
            mStripManagerPanel.MoveAudioBlockForwardRequested +=
                new Events.PhraseNodeHandler(project.MovePhraseNodeForwardRequested);
            mStripManagerPanel.SetMediaRequested +=
                new Events.PhraseNodeSetMediaHandler(project.SetMediaRequested);
            mStripManagerPanel.SplitAudioBlockRequested +=
                new Events.SplitPhraseNodeHandler(project.SplitAudioBlockRequested);
            

            
            project.CutSectionNode +=
                new Events.Node.Section.CutSectionNodeHandler(mTOCPanel.SyncCutSectionNode);
            project.CutSectionNode +=
                new Events.Node.Section.CutSectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);
            project.DeletedSectionNode +=
                new Events.Node.Section.DeletedSectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            project.DeletedSectionNode +=
                new Events.Node.Section.DeletedSectionNodeHandler(mStripManagerPanel.SyncDeletedNode);
            project.PastedSectionNode +=
                new Events.Node.Section.PastedSectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            project.PastedSectionNode +=
                new Events.Node.Section.PastedSectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            mTOCPanel.RequestToAddChildSectionNode +=
                new Events.Node.Section.RequestToAddChildSectionNodeHandler(project.CreateChildSectionNodeRequested);
            mTOCPanel.RequestToAddSiblingSection +=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(project.CreateSiblingSectionNodeRequested);
            mStripManagerPanel.AddSiblingSection +=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(project.CreateSiblingSectionNodeRequested);
            mTOCPanel.RequestToCutSectionNode +=
                new Events.Node.Section.RequestToCutSectionNodeHandler(project.CutSectionNodeRequested);
            mTOCPanel.RequestToDeleteSectionNode +=
                new Events.Node.Section.RequestToDeleteSectionNodeHandler(project.RemoveNodeRequested);
            project.UndidPasteSectionNode +=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            project.UndidPasteSectionNode +=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);





            //these all relate to moving nodes up and down
            mTOCPanel.RequestToMoveSectionNodeUp += new Events.Node.RequestToMoveSectionNodeUpHandler(project.MoveSectionNodeUpRequested);
            mTOCPanel.RequestToMoveSectionNodeDown += new Events.Node.RequestToMoveSectionNodeDownHandler(project.MoveSectionNodeDownRequested);
            project.MovedSectionNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.MovedSectionNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            project.UndidMoveNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.UndidMoveNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            mStripManagerPanel.RequestToMoveSectionNodeDownLinear += new Events.Node.RequestToMoveSectionNodeDownLinearHandler(project.MoveSectionNodeDownLinearRequested);
            mStripManagerPanel.RequestToMoveSectionNodeUpLinear += new Events.Node.RequestToMoveSectionNodeUpLinearHandler(project.MoveSectionNodeUpLinearRequested);
            project.ShallowSwappedSectionNodes += new Events.Node.ShallowSwappedSectionNodesHandler(mTOCPanel.SyncShallowSwapNodes);
            project.ShallowSwappedSectionNodes += new Events.Node.ShallowSwappedSectionNodesHandler(mStripManagerPanel.SyncShallowSwapNodes);

            mTOCPanel.RequestToIncreaseSectionNodeLevel +=
                new Events.Node.RequestToIncreaseSectionNodeLevelHandler(project.IncreaseSectionNodeLevelRequested);
            //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

            mTOCPanel.RequestToDecreaseSectionNodeLevel +=
                new Events.Node.RequestToDecreaseSectionNodeLevelHandler(project.DecreaseSectionNodeLevelRequested);
            project.DecreasedSectionNodeLevel += new Events.Node.DecreasedSectionNodeLevelHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);

            mTOCPanel.RequestToRenameSectionNode +=
                new Events.Node.Section.RequestToRenameSectionNodeHandler(project.RenameSectionNodeRequested);
            mStripManagerPanel.RenameSection +=
                new Events.Node.Section.RequestToRenameSectionNodeHandler(project.RenameSectionNodeRequested);
            project.RenamedSectionNode +=
                new Events.Node.Section.RenamedSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            project.RenamedSectionNode +=
                new Events.Node.Section.RenamedSectionNodeHandler(mStripManagerPanel.SyncRenamedSectionNode);


            // Block events

            mStripManagerPanel.RequestToCutPhraseNode +=
                new Events.Node.RequestToCutPhraseNodeHandler(project.CutPhraseNode);
            mStripManagerPanel.RequestToCopyPhraseNode +=
                new Events.Node.RequestToCopyPhraseNodeHandler(project.CopyPhraseNode);
            mStripManagerPanel.RequestToPastePhraseNode +=
                new Events.Node.RequestToPastePhraseNodeHandler(project.PastePhraseNode);


            project.DeletedPhraseNode +=
                new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
            project.MediaSet += new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);
            project.TouchedPhraseNode += new Events.Node.TouchedNodeHandler(mStripManagerPanel.SyncTouchedNode);
            project.UpdateTime += new Events.Strip.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);





            //md: clipboard in the TOC

            mTOCPanel.RequestToCopySectionNode += new Events.Node.RequestToCopySectionNodeHandler(project.CopySectionNodeRequested);
            project.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
            project.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
            project.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
            project.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

            mTOCPanel.RequestToPasteSectionNode += new Events.Node.RequestToPasteSectionNodeHandler(project.PasteSectionNodeRequested);


            //md 20060812
            mStripManagerPanel.RequestToShallowDeleteSectionNode +=
                new Events.Node.RequestToShallowDeleteSectionNodeHandler(project.ShallowDeleteSectionNodeRequested);

            mStripManagerPanel.RequestToSetPageNumber += new Events.Node.RequestToSetPageNumberHandler(project.SetPageRequested);
            mStripManagerPanel.RequestToRemovePageNumber +=
                new Events.Node.RequestToRemovePageNumberHandler(project.RemovePageRequested);
            project.RemovedPageNumber += new Events.Node.RemovedPageNumberHandler(mStripManagerPanel.SyncRemovedPageNumber);
            project.SetPageNumber += new Events.Node.SetPageNumberHandler(mStripManagerPanel.SyncSetPageNumber);
        }

        private void ResetEventHandlers()
        {
            mProject.AddedPhraseNode -=
                new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            mProject.AddedSectionNode -=
                new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedEmptySectionNode);
            mProject.AddedSectionNode -=
                new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);

            mStripManagerPanel.DeleteBlockRequested -=
                new Events.PhraseNodeHandler(mProject.DeletePhraseNodeRequested);
            mStripManagerPanel.ImportAudioAssetRequested -=
                new Events.Strip.RequestToImportAssetHandler(mProject.ImportAssetRequested);
            mStripManagerPanel.MergePhraseNodes -=
                new Events.MergePhraseNodesHandler(mProject.MergePhraseNodesRequested);
            mStripManagerPanel.SetMediaRequested -=
                new Events.PhraseNodeSetMediaHandler(mProject.SetMediaRequested);
            mStripManagerPanel.SplitAudioBlockRequested -=
                new Events.SplitPhraseNodeHandler(mProject.SplitAudioBlockRequested);
            


            mProject.AddedPhraseNode -=
                new Events.Node.Phrase.AddedPhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            mProject.CutSectionNode -=
                new Events.Node.Section.CutSectionNodeHandler(mTOCPanel.SyncCutSectionNode);
            mProject.CutSectionNode -=
                new Events.Node.Section.CutSectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);
            mProject.DeletedSectionNode -=
                new Events.Node.Section.DeletedSectionNodeHandler(mStripManagerPanel.SyncDeletedNode);
            mProject.DeletedSectionNode -=
                new Events.Node.Section.DeletedSectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            mProject.PastedSectionNode -=
                new Events.Node.Section.PastedSectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            mProject.PastedSectionNode -=
                new Events.Node.Section.PastedSectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            mTOCPanel.RequestToAddChildSectionNode -=
                new Events.Node.Section.RequestToAddChildSectionNodeHandler(mProject.CreateChildSectionNodeRequested);
            mTOCPanel.RequestToAddSiblingSection -=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);
            mStripManagerPanel.AddSiblingSection -=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);
            mTOCPanel.RequestToCutSectionNode -=
                new Events.Node.Section.RequestToCutSectionNodeHandler(mProject.CutSectionNodeRequested);
            mTOCPanel.RequestToDeleteSectionNode -=
                new Events.Node.Section.RequestToDeleteSectionNodeHandler(mProject.RemoveNodeRequested);
            mProject.UndidPasteSectionNode -=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);
            mProject.UndidPasteSectionNode -=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);




            //these are all events related to moving nodes up and down
            mTOCPanel.RequestToMoveSectionNodeUp -= new Events.Node.RequestToMoveSectionNodeUpHandler(mProject.MoveSectionNodeUpRequested);
            mTOCPanel.RequestToMoveSectionNodeDown -= new Events.Node.RequestToMoveSectionNodeDownHandler(mProject.MoveSectionNodeDownRequested);
            mProject.MovedSectionNode -= new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.MovedSectionNode -= new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            mProject.UndidMoveNode -= new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.UndidMoveNode -= new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            mStripManagerPanel.RequestToMoveSectionNodeDownLinear -= new Events.Node.RequestToMoveSectionNodeDownLinearHandler(mProject.MoveSectionNodeDownLinearRequested);
            mStripManagerPanel.RequestToMoveSectionNodeUpLinear -= new Events.Node.RequestToMoveSectionNodeUpLinearHandler(mProject.MoveSectionNodeUpLinearRequested);
            mProject.ShallowSwappedSectionNodes -= new Events.Node.ShallowSwappedSectionNodesHandler(mTOCPanel.SyncShallowSwapNodes);
            mProject.ShallowSwappedSectionNodes -= new Events.Node.ShallowSwappedSectionNodesHandler(mStripManagerPanel.SyncShallowSwapNodes);

            mTOCPanel.RequestToIncreaseSectionNodeLevel -= new Events.Node.RequestToIncreaseSectionNodeLevelHandler(mProject.IncreaseSectionNodeLevelRequested);
            //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

            mTOCPanel.RequestToDecreaseSectionNodeLevel -= new Events.Node.RequestToDecreaseSectionNodeLevelHandler(mProject.DecreaseSectionNodeLevelRequested);
            mProject.DecreasedSectionNodeLevel -= new Events.Node.DecreasedSectionNodeLevelHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);

            mTOCPanel.RequestToRenameSectionNode -= new Events.Node.Section.RequestToRenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
            mStripManagerPanel.RenameSection -= new Events.Node.Section.RequestToRenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
            mProject.RenamedSectionNode -= new Events.Node.Section.RenamedSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            mProject.RenamedSectionNode -= new Events.Node.Section.RenamedSectionNodeHandler(mStripManagerPanel.SyncRenamedSectionNode);



            //mProject.ImportedAsset -= new Events.Node.ImportedAssetHandler(mStripManagerPanel.SyncCreateNewAudioBlock);

            mStripManagerPanel.SetMediaRequested -= new Events.Node.SetMediaHandler(mProject.SetMediaRequested);
            mProject.MediaSet -= new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);

            mProject.DeletedPhraseNode -= new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);


            mTOCPanel.RequestToCopySectionNode -= new Events.Node.RequestToCopySectionNodeHandler(mProject.CopySectionNodeRequested);
            mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
            mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
            mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
            mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

            mTOCPanel.RequestToPasteSectionNode -= new Events.Node.RequestToPasteSectionNodeHandler(mProject.PasteSectionNodeRequested);

            mProject.TouchedPhraseNode -= new Events.Node.TouchedNodeHandler(mStripManagerPanel.SyncTouchedNode);
            mProject.UpdateTime -= new Events.Strip.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

            //md 20060812
            mStripManagerPanel.RequestToShallowDeleteSectionNode -= new Events.Node.RequestToShallowDeleteSectionNodeHandler(mProject.ShallowDeleteSectionNodeRequested);

            mStripManagerPanel.RequestToCutPhraseNode -=
                new Events.Node.RequestToCutPhraseNodeHandler(mProject.CutPhraseNode);
            mStripManagerPanel.RequestToCopyPhraseNode -=
                new Events.Node.RequestToCopyPhraseNodeHandler(mProject.CopyPhraseNode);
            mStripManagerPanel.RequestToPastePhraseNode -=
                new Events.Node.RequestToPastePhraseNodeHandler(mProject.PastePhraseNode);
            mStripManagerPanel.RequestToSetPageNumber -= new Events.Node.RequestToSetPageNumberHandler(mProject.SetPageRequested);
            mStripManagerPanel.RequestToRemovePageNumber -=
                new Events.Node.RequestToRemovePageNumberHandler(mProject.RemovePageRequested);
            mProject.RemovedPageNumber -= new Events.Node.RemovedPageNumberHandler(mStripManagerPanel.SyncRemovedPageNumber);
            mProject.SetPageNumber -= new Events.Node.SetPageNumberHandler(mStripManagerPanel.SyncSetPageNumber);
        }

        #endregion

        public Boolean TOCPanelVisible
        {
            get { return mProject != null && !mSplitContainer.Panel1Collapsed; }
        }

        public StripManagerPanel StripManager
        {
            get { return mStripManagerPanel; }
        }

        public TOCPanel TOCPanel
        {
            get { return mTOCPanel; }
        }

        /// <summary>
        /// Create a new project panel with currently no project.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            //mg 20060804: set two convenince attrs on the toc and tree views
            mTOCPanel.ProjectPanel = this;
            mStripManagerPanel.ProjectPanel = this;
            //end mg
            Project = null;
        }

        public void HideTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = true;
        }

        public void ShowTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = false;
        }

        /// <summary>
        /// Synchronize the project views with the core tree. Used when opening a XUK file.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
        {
            mTOCPanel.SynchronizeWithCoreTree(root);
            mStripManagerPanel.SynchronizeWithCoreTree(root);
        }
    }
}
