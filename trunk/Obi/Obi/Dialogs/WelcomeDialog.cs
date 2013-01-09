using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class WelcomeDialog : Form
    {
        /// <summary>
        /// Possible actions offered by this dialog.
        /// </summary>
        public enum Option { NewProject, NewProjectFromImport, OpenProject, OpenLastProject, OpenEmpty, ViewHelp };

        private Option mResult;

        public WelcomeDialog(bool canOpenLastProject)
        {
            InitializeComponent();
            mOpenLastProjectButton.Enabled = canOpenLastProject;
            mResult = Option.OpenEmpty;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Introducing Obi/Getting Started/Starting Obi.htm");
        }

        /// <summary>
        /// Result of the user choice.
        /// </summary>
        public Option Result { get { return mResult; } }

        // Create a new project.
        private void mNewProjectButton_Click(object sender, EventArgs e)
        {
            mResult = Option.NewProject;
            Close();
        }

        // Create a new project from import
        private void mImportButton_Click(object sender, EventArgs e)
        {
            mResult = Option.NewProjectFromImport;
            Close();
        }

        // Open an existing project
        private void mOpenProjectButton_Click(object sender, EventArgs e)
        {
            mResult = Option.OpenProject;
            Close();
        }

        // Open last project (if possible)
        private void mOpenLastProjectButton_Click(object sender, EventArgs e)
        {
            mResult = Option.OpenLastProject;
            Close();
        }

        // Open Obi with no project
        private void mOpenEmptyButton_Click(object sender, EventArgs e)
        {
            mResult = Option.OpenEmpty;
            Close();
        }

        // Open Obi with no project and view the manual
        private void mViewManualButton_Click(object sender, EventArgs e)
        {
            mResult = Option.ViewHelp;
            Close();
        }
    }
}