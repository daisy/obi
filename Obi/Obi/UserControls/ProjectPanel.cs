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
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                mSplitContainer.Panel1Collapsed = false;
                if (mProject != null)
                {
                    mTOCPanel.AddSiblingSection += new Events.Node.AddSiblingSectionHandler(mProject.CreateSiblingSectionRequested);
                    mProject.AddedSiblingNode += new Events.Sync.AddedSiblingNodeHandler(mTOCPanel.SyncAddedSiblingNode);
                    mProject.AddedSiblingNode += new Events.Sync.AddedSiblingNodeHandler(mStripManagerPanel.SyncAddedSiblingNode);
                    mTOCPanel.AddChildSection += new Events.Node.AddChildSectionHandler(mProject.CreateChildSectionRequested);
                    mProject.AddedChildNode += new Events.Sync.AddedChildNodeHandler(mTOCPanel.SyncAddedChildNode);
                    mProject.AddedChildNode += new Events.Sync.AddedChildNodeHandler(mStripManagerPanel.SyncAddedChildNode);

                    mTOCPanel.MoveSectionUp += new Events.Node.MoveSectionUpHandler(mProject.MoveNodeUpRequested);
                    mTOCPanel.DeleteSection += new Events.Node.DeleteSectionHandler(mProject.RemoveNodeRequested);

                    mTOCPanel.RenameSection += new Events.Node.RenameSectionHandler(mProject.RenameNodeRequested);
                    mProject.RenamedNode += new Events.Sync.RenamedNodeHandler(mTOCPanel.SyncRenamedNode);
                    mProject.RenamedNode += new Events.Sync.RenamedNodeHandler(mStripManagerPanel.SyncRenamedNode);
                }
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
