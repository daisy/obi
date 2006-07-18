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
    /// Top level panel that displays the current project, using a splitter (NCX on the left, strips on the right.)
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
                    tocPanel.AddChildSection += new Events.Node.AddChildSectionHandler(mProject.CreateChildSection);
                    tocPanel.AddSiblingSection += new Events.Node.AddSiblingSectionHandler(mProject.CreateSiblingSection);
                    tocPanel.DeleteSection += new Events.Node.DeleteSectionHandler(mProject.RemoveNode);
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
                return stripManagerPanel1;
            }
        }

        public TOCPanel TOCPanel
        {
            get
            {
                return tocPanel;
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


        public void Clear()
        {
            tocPanel.Clear();
        }

        /// <summary>
        /// Synchronize the project views with the core tree. Used when opening a XUK file.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
        {
            tocPanel.SynchronizeWithCoreTree(root);
            //stripManagerPanel1.SynchronizeWithCoreTree(root);
        }
    }
}
