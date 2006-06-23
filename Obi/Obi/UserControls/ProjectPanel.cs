using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

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
            }
        }

        public Boolean NCXPanelVisible { get { return !mSplitContainer.Panel1Collapsed; } }

        public StripManagerPanel StripManager { get { return stripManagerPanel1; } }

        /// <summary>
        /// Create a new project panel with currently no project.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            Project = null;
        }

        public void HideNCXPanel()
        {
            mSplitContainer.Panel1Collapsed = true;
        }

        public void ShowNCXPanel()
        {
            mSplitContainer.Panel1Collapsed = false;
        }
    }
}
