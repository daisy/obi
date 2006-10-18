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
                // Reset the handlers from the previous project
                if (mProject != null)
                {
                    mProject.AddedSectionNode -=
                        new Events.Node.Section.AddedSectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);
                    mProject.AddedSectionNode -=
                        new Events.Node.Section.AddedSectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
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



                    mStripManagerPanel.ImportAudioAssetRequested -= new Events.Strip.RequestToImportAssetHandler(mProject.ImportAssetRequested);
                    //mProject.ImportedAsset -= new Events.Node.ImportedAssetHandler(mStripManagerPanel.SyncCreateNewAudioBlock);
                    mProject.AddedPhraseNode -= new Events.Node.AddedPhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);

                    mStripManagerPanel.SetMediaRequested -= new Events.Node.SetMediaHandler(mProject.SetMediaRequested);
                    mProject.MediaSet -= new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);

                    mStripManagerPanel.SplitAudioBlockRequested -= new Events.Node.SplitNodeHandler(mProject.SplitAudioBlockRequested);
                    mStripManagerPanel.MergeNodes -= new Events.Node.MergeNodesHandler(mProject.MergeNodesRequested);

                    mStripManagerPanel.DeleteBlockRequested -=
                        new Events.Node.RequestToDeleteBlockHandler(mProject.DeletePhraseNodeRequested);
                    mProject.DeletedPhraseNode -= new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);


                    mTOCPanel.RequestToCopySectionNode -= new Events.Node.RequestToCopySectionNodeHandler(mProject.CopySectionNodeRequested);
                    mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
                    mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
                    mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
                    mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.RequestToPasteSectionNode -= new Events.Node.RequestToPasteSectionNodeHandler(mProject.PasteSectionNodeRequested);
                    
                    mProject.TouchedNode -= new Events.Node.TouchedNodeHandler(mStripManagerPanel.SyncTouchedNode);
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
                // Set up the handlers for the new project
                if (value != null)
                {
                    value.AddedSectionNode +=
                        new Events.Node.Section.AddedSectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
                    value.AddedSectionNode +=
                        new Events.Node.Section.AddedSectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);
                    value.CutSectionNode +=
                        new Events.Node.Section.CutSectionNodeHandler(mTOCPanel.SyncCutSectionNode);
                    value.CutSectionNode +=
                        new Events.Node.Section.CutSectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);
                    value.DeletedSectionNode +=
                        new Events.Node.Section.DeletedSectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
                    value.DeletedSectionNode +=
                        new Events.Node.Section.DeletedSectionNodeHandler(mStripManagerPanel.SyncDeletedNode);
                    value.PastedSectionNode +=
                        new Events.Node.Section.PastedSectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
                    value.PastedSectionNode +=
                        new Events.Node.Section.PastedSectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
                    mTOCPanel.RequestToAddChildSectionNode +=
                        new Events.Node.Section.RequestToAddChildSectionNodeHandler(value.CreateChildSectionNodeRequested);
                    mTOCPanel.RequestToAddSiblingSection +=
                        new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(value.CreateSiblingSectionNodeRequested);
                    mStripManagerPanel.AddSiblingSection +=
                        new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(value.CreateSiblingSectionNodeRequested);
                    mTOCPanel.RequestToCutSectionNode +=
                        new Events.Node.Section.RequestToCutSectionNodeHandler(value.CutSectionNodeRequested);
                    mTOCPanel.RequestToDeleteSectionNode +=
                        new Events.Node.Section.RequestToDeleteSectionNodeHandler(value.RemoveNodeRequested);
                    value.UndidPasteSectionNode +=
                        new Events.Node.Section.UndidPasteSectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
                    value.UndidPasteSectionNode +=
                        new Events.Node.Section.UndidPasteSectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);





                    //these all relate to moving nodes up and down
                    mTOCPanel.RequestToMoveSectionNodeUp += new Events.Node.RequestToMoveSectionNodeUpHandler(value.MoveSectionNodeUpRequested);
                    mTOCPanel.RequestToMoveSectionNodeDown += new Events.Node.RequestToMoveSectionNodeDownHandler(value.MoveSectionNodeDownRequested);
                    value.MovedSectionNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    value.MovedSectionNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
                    value.UndidMoveNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    value.UndidMoveNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
                    mStripManagerPanel.RequestToMoveSectionNodeDownLinear += new Events.Node.RequestToMoveSectionNodeDownLinearHandler(value.MoveSectionNodeDownLinearRequested);
                    mStripManagerPanel.RequestToMoveSectionNodeUpLinear += new Events.Node.RequestToMoveSectionNodeUpLinearHandler(value.MoveSectionNodeUpLinearRequested);
                    value.ShallowSwappedSectionNodes += new Events.Node.ShallowSwappedSectionNodesHandler(mTOCPanel.SyncShallowSwapNodes);
                    value.ShallowSwappedSectionNodes += new Events.Node.ShallowSwappedSectionNodesHandler(mStripManagerPanel.SyncShallowSwapNodes);

                    mTOCPanel.RequestToIncreaseSectionNodeLevel +=
                        new Events.Node.RequestToIncreaseSectionNodeLevelHandler(value.IncreaseSectionNodeLevelRequested);
                    //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

                    mTOCPanel.RequestToDecreaseSectionNodeLevel +=
                        new Events.Node.RequestToDecreaseSectionNodeLevelHandler(value.DecreaseSectionNodeLevelRequested);
                    value.DecreasedSectionNodeLevel += new Events.Node.DecreasedSectionNodeLevelHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);

                    mTOCPanel.RequestToRenameSectionNode +=
                        new Events.Node.Section.RequestToRenameSectionNodeHandler(value.RenameSectionNodeRequested);
                    mStripManagerPanel.RenameSection +=
                        new Events.Node.Section.RequestToRenameSectionNodeHandler(value.RenameSectionNodeRequested);
                    value.RenamedSectionNode +=
                        new Events.Node.Section.RenamedSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
                    value.RenamedSectionNode +=
                        new Events.Node.Section.RenamedSectionNodeHandler(mStripManagerPanel.SyncRenamedSectionNode);


                    // Block events

                    mStripManagerPanel.RequestToCutPhraseNode +=
                        new Events.Node.RequestToCutPhraseNodeHandler(value.CutPhraseNode);
                    mStripManagerPanel.RequestToCopyPhraseNode +=
                        new Events.Node.RequestToCopyPhraseNodeHandler(value.CopyPhraseNode);
                    mStripManagerPanel.RequestToPastePhraseNode +=
                        new Events.Node.RequestToPastePhraseNodeHandler(value.PastePhraseNode);

                    mStripManagerPanel.ImportAudioAssetRequested +=
                        new Events.Strip.RequestToImportAssetHandler(value.ImportAssetRequested);
                    mStripManagerPanel.DeleteBlockRequested +=
                        new Events.Node.RequestToDeleteBlockHandler(value.DeletePhraseNodeRequested);
                    mStripManagerPanel.MoveAudioBlockForwardRequested +=
                        new Events.Node.RequestToMoveBlockHandler(value.MovePhraseNodeForwardRequested);
                    mStripManagerPanel.MoveAudioBlockBackwardRequested +=
                        new Events.Node.RequestToMoveBlockHandler(value.MovePhraseNodeBackwardRequested);
                    mStripManagerPanel.SetMediaRequested += new Events.Node.SetMediaHandler(value.SetMediaRequested);
                    mStripManagerPanel.SplitAudioBlockRequested +=
                        new Events.Node.SplitNodeHandler(value.SplitAudioBlockRequested);

                    value.AddedPhraseNode +=
                        new Events.Node.AddedPhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
                    value.DeletedPhraseNode +=
                        new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
                    value.MediaSet += new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);
                    value.TouchedNode += new Events.Node.TouchedNodeHandler(mStripManagerPanel.SyncTouchedNode);
                    value.UpdateTime += new Events.Strip.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);



                    mStripManagerPanel.MergeNodes += new Events.Node.MergeNodesHandler(value.MergeNodesRequested);


                    //md: clipboard in the TOC
                    
                    mTOCPanel.RequestToCopySectionNode += new Events.Node.RequestToCopySectionNodeHandler(value.CopySectionNodeRequested);
                    value.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
                    value.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
                    value.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
                    value.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.RequestToPasteSectionNode += new Events.Node.RequestToPasteSectionNodeHandler(value.PasteSectionNodeRequested);

                
                    //md 20060812
                    mStripManagerPanel.RequestToShallowDeleteSectionNode +=
                        new Events.Node.RequestToShallowDeleteSectionNodeHandler(value.ShallowDeleteSectionNodeRequested);

                    mStripManagerPanel.RequestToSetPageNumber += new Events.Node.RequestToSetPageNumberHandler(value.SetPageRequested);
                    mStripManagerPanel.RequestToRemovePageNumber +=
                        new Events.Node.RequestToRemovePageNumberHandler(value.RemovePageRequested);
                    value.RemovedPageNumber += new Events.Node.RemovedPageNumberHandler(mStripManagerPanel.SyncRemovedPageNumber);
                    value.SetPageNumber += new Events.Node.SetPageNumberHandler(mStripManagerPanel.SyncSetPageNumber);
                } 
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                mSplitContainer.Panel1Collapsed = false;
            }
        }

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
