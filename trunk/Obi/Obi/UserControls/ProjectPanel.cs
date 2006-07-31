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
            get
            {
                return mProject;
            }
            set
            {
                // Reset the handlers from the previous project
                if (mProject != null)
                {
                    mTOCPanel.RequestToAddSiblingSection -= new Events.Node.RequestToAddSiblingNodeHandler(mProject.CreateSiblingSectionRequested);
                    mStripManagerPanel.AddSiblingSection -=
                        new Events.Node.RequestToAddSiblingNodeHandler(mProject.CreateSiblingSectionRequested);
                    
                    mProject.AddedSectionNode -= new Events.Node.AddedSectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
                    mProject.AddedSectionNode -= new Events.Node.AddedSectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

                    mTOCPanel.RequestToAddChildSection -= new Events.Node.RequestToAddChildNodeHandler(mProject.CreateChildSectionRequested);
                    
                    mTOCPanel.RequestToMoveSectionUp -= new Events.Node.RequestToMoveNodeUpHandler(mProject.MoveNodeUpRequested);
                    mProject.MovedNodeUp -= new Events.Node.MovedNodeUpHandler(mTOCPanel.SyncMovedNodeUp);

                    mTOCPanel.RequestToMoveSectionDown -= new Events.Node.RequestToMoveNodeDownHandler(mProject.MoveNodeDownRequested);
                    mProject.MovedNodeDown -= new Events.Node.MovedNodeDownHandler(mTOCPanel.SyncMovedNodeDown);

                    mTOCPanel.RequestToIncreaseSectionLevel -= new Events.Node.RequestToIncreaseNodeLevelHandler(mProject.IncreaseNodeLevelRequested);
                    mProject.IncreasedNodeLevel -= new Events.Node.IncreasedNodeLevelHandler(mTOCPanel.SyncIncreasedNodeLevel);

                    mTOCPanel.RequestToDecreaseSectionLevel -= new Events.Node.RequestToDecreaseNodeLevelHandler(mProject.DecreaseNodeLevelRequested);
                    mProject.DecreasedNodeLevel -= new Events.Node.DecreasedNodeLevelHandler(mTOCPanel.SyncDecreasedNodeLevel);

                    mTOCPanel.RequestToRenameSection -= new Events.Node.RequestToRenameNodeHandler(mProject.RenameNodeRequested);
                    mStripManagerPanel.RenameSection -= new Events.Node.RequestToRenameNodeHandler(mProject.RenameNodeRequested);
                    mProject.RenamedNode -= new Events.Node.RenamedNodeHandler(mTOCPanel.SyncRenamedNode);
                    mProject.RenamedNode -= new Events.Node.RenamedNodeHandler(mStripManagerPanel.SyncRenamedNode);

                    mTOCPanel.RequestToDeleteSection -= new Events.Node.RequestToDeleteNodeHandler(mProject.RemoveNodeRequested);
                    mProject.DeletedNode -= new Events.Node.DeletedNodeHandler(mTOCPanel.SyncDeletedNode);
                    mProject.DeletedNode -= new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeletedNode);

                    mStripManagerPanel.ImportPhrase -= new Events.Strip.RequestToImportAssetHandler(mProject.ImportPhraseRequested);
                    mProject.ImportedAsset -= new Events.Node.ImportedAssetHandler(mStripManagerPanel.SyncImportedAsset);

                    //marisa: TESTING only
                    mTOCPanel.RequestToUndoMoveNode -= new Events.Node.RequestToUndoMoveNodeHandler(mProject.TestUndoMoveNode);
                }
                // Set up the handlers for the new project
                if (value != null)
                {
                    mTOCPanel.RequestToAddSiblingSection += new Events.Node.RequestToAddSiblingNodeHandler(value.CreateSiblingSectionRequested);
                    mStripManagerPanel.AddSiblingSection +=
                        new Events.Node.RequestToAddSiblingNodeHandler(value.CreateSiblingSectionRequested);
                    value.AddedSectionNode += new Events.Node.AddedSectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
                    value.AddedSectionNode += new Events.Node.AddedSectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

                    mTOCPanel.RequestToAddChildSection += new Events.Node.RequestToAddChildNodeHandler(value.CreateChildSectionRequested);
                    
                    mTOCPanel.RequestToMoveSectionUp += new Events.Node.RequestToMoveNodeUpHandler(value.MoveNodeUpRequested);
                    value.MovedNodeUp += new Events.Node.MovedNodeUpHandler(mTOCPanel.SyncMovedNodeUp);

                    mTOCPanel.RequestToMoveSectionDown += new Events.Node.RequestToMoveNodeDownHandler(value.MoveNodeDownRequested);
                    value.MovedNodeDown += new Events.Node.MovedNodeDownHandler(mTOCPanel.SyncMovedNodeDown);

                    mTOCPanel.RequestToIncreaseSectionLevel += new Events.Node.RequestToIncreaseNodeLevelHandler(value.IncreaseNodeLevelRequested);
                    value.IncreasedNodeLevel += new Events.Node.IncreasedNodeLevelHandler(mTOCPanel.SyncIncreasedNodeLevel);

                    mTOCPanel.RequestToDecreaseSectionLevel += new Events.Node.RequestToDecreaseNodeLevelHandler(value.DecreaseNodeLevelRequested);
                    value.DecreasedNodeLevel += new Events.Node.DecreasedNodeLevelHandler(mTOCPanel.SyncDecreasedNodeLevel);

                    mTOCPanel.RequestToRenameSection += new Events.Node.RequestToRenameNodeHandler(value.RenameNodeRequested);
                    mStripManagerPanel.RenameSection += new Events.Node.RequestToRenameNodeHandler(value.RenameNodeRequested);
                    value.RenamedNode += new Events.Node.RenamedNodeHandler(mTOCPanel.SyncRenamedNode);
                    value.RenamedNode += new Events.Node.RenamedNodeHandler(mStripManagerPanel.SyncRenamedNode);

                    mTOCPanel.RequestToDeleteSection += new Events.Node.RequestToDeleteNodeHandler(value.RemoveNodeRequested);
                    value.DeletedNode += new Events.Node.DeletedNodeHandler(mTOCPanel.SyncDeletedNode);
                    value.DeletedNode += new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeletedNode);

                    mStripManagerPanel.ImportPhrase += new Events.Strip.RequestToImportAssetHandler(value.ImportPhraseRequested);
                    value.ImportedAsset += new Events.Node.ImportedAssetHandler(mStripManagerPanel.SyncImportedAsset);

                    //marisa: TESTING only
                    mTOCPanel.RequestToUndoMoveNode += new Events.Node.RequestToUndoMoveNodeHandler(value.TestUndoMoveNode);
                }
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                mSplitContainer.Panel1Collapsed = false;
            }
        }

        public Boolean TOCPanelVisible
        {
            get
            {
                return mProject != null && !mSplitContainer.Panel1Collapsed;
            }
        }

        public StripManagerPanel StripManager
        {
            get
            {
                return mStripManagerPanel;
            }
        }

        public TOCPanel TOCPanel
        {
            get
            {
                return mTOCPanel;
            }
        }

        /// <summary>
        /// Create a new project panel with currently no project.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
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


        /*public void Clear()
        {
            mTOCPanel.Clear();
        }*/

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
