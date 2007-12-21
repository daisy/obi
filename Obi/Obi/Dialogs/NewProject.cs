using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
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
        private string mBasepath;       // the base path for the project
        private bool mCanClose;         // can close the form or not (cannot close if file existed)
        private string mExtension;      // file extension for the file chooser
        private string mFilename;       // the actual filename 
        private bool mUserSetLocation;  // the location was changed manually by the user

        /// <summary>
        /// Create a new dialog with default information (dummy name and default path.)
        /// </summary>
        /// <param name="basepath">The initial directory where to create the project.</param>
        /// <param name="filename">The actual file name for the project.</param>
        /// <param name="extension">The file extension (for a file dialog.)</param>
        /// <param name="title">The project title.</param>
        public NewProject(string basepath, string filename, string extension, string title)
        {
            InitializeComponent();
            mTitleBox.Text = title;
            mBasepath = basepath;
            mExtension = extension;
            mFilename = filename;
            mCanClose = true;
            mUserSetLocation = false;
            GenerateFileName();
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
        /// Make the "create title section" checkbox invisible (use only for project import.)
        /// </summary>
        public void DisableAutoTitleCheckbox() { mAutoTitleCheckBox.Enabled = false; }

        /// <summary>
        /// The chosen path for the XUK project file; derived from the title or chosen
        /// by the user.
        /// </summary>
        public string Path
        {
            get { return mFileBox.Text; }
        }

        /// <summary>
        /// The chosen title for the project.
        /// </summary>
        public string Title
        {
            get { return mTitleBox.Text; }
        }


        /// <summary>
        /// Generate the file name for the project.
        /// </summary>
        private void GenerateFileName()
        {
            if (!mUserSetLocation)
            {
                mFileBox.Text = String.Format("{0}{1}{2}{1}{3}",
                    mBasepath,
                    System.IO.Path.DirectorySeparatorChar,
                    SafeName(mTitleBox.Text),
                    mFilename);
            }
        }

        /// <summary>
        /// Before closing, make sure that the directory chosen works.
        /// </summary>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            string directory = System.IO.Path.GetDirectoryName(mFileBox.Text);
            mCanClose = ObiForm.CanUseDirectory(directory, true);
        }

        /// <summary>
        /// Update the path text box with the selected path from the file chooser.
        /// </summary>
        private void mSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(mFileBox.Text);
            dialog.Filter = mExtension;
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mFileBox.Text = dialog.FileName;
                mUserSetLocation = true;
            }
        }

        /// <summary>
        /// Generate a new XUK file name when a new title is chosen.
        /// </summary>
        private void mTitleBox_TextChanged(object sender, EventArgs e) { GenerateFileName(); }

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

        /// <summary>
        /// Get a safename for the project directory from the title.
        /// </summary>
        /// <param name="title">The project title.</param>
        /// <returns>A safe name for the file system.</returns>
        private static string SafeName(string title)
        {
            string invalid = "[";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) invalid += String.Format("\\x{0:x2}", (int)c);
            invalid += "]+";
            string safe = Regex.Replace(title, invalid, "_");
            safe = Regex.Replace(safe, "^_", "");
            safe = Regex.Replace(safe, "_$", "");
            return safe;
        }
    }
}