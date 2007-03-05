using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// Dialog for creating a new project.
    /// The user can choos a title for the project and a file to save it to.
    /// Since we save the project as soon as we create it, make sure that it
    /// can be saved and that it does not overwrite another project by accident.
    /// </summary>
    public partial class NewProject : Form
    {
        private bool mCheckExisting;  // flag for checking for an existing file before closing.
        private bool mCanClose;       // as a result, this flag tells whether to close the form or not.
 
        /// <summary>
        /// The chosen title for the project.
        /// </summary>
        public string Title
        {
            get { return mTitleBox.Text; }
        }

        /// <summary>
        /// Flag telling whether to automatically create a new section for the title.
        /// The value initial will be the one chosen when a new project was created last.
        /// </summary>
        public bool CreateTitleSection
        {
            get { return mAutoTitleCheckBox.Checked; }
            set { mAutoTitleCheckBox.Checked = value; }
        }

        /// <summary>
        /// The chosen path for the XUK project file; derived from the title or chosen
        /// by the user.
        /// </summary>
        public string Path
        {
            get { return mFileBox.Text; }
        }

        /// <summary>
        /// Create a new dialog with default information (dummy name and default path.)
        /// </summary>
        /// <param name="path">The initial directory where to create the project.</param>
        public NewProject(string path)
        {
            InitializeComponent();
            mCheckExisting = true;
            mCanClose = true;
            mTitleBox.Text = Localizer.Message("new_project");
            // Add a trailing separator so that the last directory doesn't look like a file name (lame.)
            mFileBox.Text = path + (path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ?
                "" : System.IO.Path.DirectorySeparatorChar.ToString());
            GenerateFileName();
        }

        /// <summary>
        /// Update the path text box with the selected path from the file chooser.
        /// </summary>
        private void selectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(mFileBox.Text);
            dialog.Filter = Localizer.Message("xuk_filter");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mFileBox.Text = dialog.FileName;
                mCheckExisting = false;  // the user was asked by the file chooser
            }
        }

        /// <summary>
        /// Generate a new XUK file name when a new title is chosen.
        /// </summary>
        private void mTitleBox_TextChanged(object sender, EventArgs e)
        {
            GenerateFileName();
        }

        /// <summary>
        /// Generate a full path from the initial directory and the title of the project.
        /// </summary>
        private void GenerateFileName()
        {
            try
            {
                mFileBox.Text = String.Format(@"{0}{1}{2}.xuk", System.IO.Path.GetDirectoryName(mFileBox.Text),
                    System.IO.Path.DirectorySeparatorChar, Project.SafeName(mTitleBox.Text));
                mCheckExisting = true;  // since the filename changed, we should check
            }
            catch (Exception)
            {
                // catch a possible ill-formed path exception when the text field is initialized
            }
        }

        /// <summary>
        /// Before closing, select
        /// </summary>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            mCanClose = IOUtils.ValidateAndCreateDir(System.IO.Path.GetDirectoryName(mFileBox.Text));
            if (mCheckExisting && File.Exists(mFileBox.Text))
            {
                DialogResult result = MessageBox.Show(String.Format(Localizer.Message("overwrite_file_text"), mFileBox.Text),
                    Localizer.Message("overwrite_file_caption"),
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.No:
                        mCanClose = false;
                        break;
                    case DialogResult.Cancel:
                        this.DialogResult = result;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// If we decided not to overwrite a file, then the form should not be closed.
        /// Otherwise (no overwrite problem, user allowed overwrite, cancel...) close as usual.
        /// </summary>
        private void NewProject_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mCanClose)
            {
                e.Cancel = true;
                mCanClose = true;
            }
        }
    }
}