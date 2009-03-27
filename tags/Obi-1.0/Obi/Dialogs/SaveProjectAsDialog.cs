using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.Dialogs
{
    public partial class SaveProjectAsDialog : Form
    {
        private bool mCanClose;               // prevent closing if unset
        private string mFilename;             // filename for the new project
        private string mNewProjectPath;       // new project path as selected by the user
        private string mOriginalProjectPath;  // original project path
        private bool mUserSetLocation;        // the location was changed manually by the user


        /// <summary>
        /// Used by the designer.
        /// </summary>
        public SaveProjectAsDialog()
        {
            InitializeComponent();
            mCanClose = true;
            mUserSetLocation = false;
        }

        /// <summary>
        /// Create a new dialog for the original project path
        /// </summary>
        public SaveProjectAsDialog(string path)
            : this()
        {
            mFilename = Path.GetFileName(path);
            string dir = Path.GetDirectoryName(path);
            mOriginalProjectPath = path;
            mNewDirectoryTextBox.Text = string.Format(Localizer.Message("save_as_new_directory_name"), dir);
            mNewDirectoryTextBox.SelectionStart = 0;
            mNewDirectoryTextBox.SelectionLength = mNewDirectoryTextBox.Text.Length;
            GenerateFileName();
        }


        /// <summary>
        /// Path to save the new project.
        /// </summary>
        public string NewProjectPath { get { return mNewProjectPath; } }

        /// <summary>
        /// If true, edit new project instead of the old one after saving.
        /// </summary>
        public bool SwitchToNewProject { get { return mSwitchToNewCheckBox.Checked; } }


        // Update the file box to generate a filename for the project
        private void GenerateFileName()
        {
            if (!mUserSetLocation) mLocationTextBox.Text = Path.Combine(mNewDirectoryTextBox.Text, mFilename);
        }


        // Browse to a new location.
        private void mSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            try
            {
                dialog.FileName = Path.GetFullPath(mLocationTextBox.Text);
            }
            catch (Exception) {}
            dialog.DefaultExt = Localizer.Message("obi_filter");
            if (dialog.ShowDialog () == DialogResult.OK && ObiForm.CheckProjectPath_Safe(dialog.FileName, true))
            {
                mLocationTextBox.Text = dialog.FileName;
                mNewDirectoryTextBox.TextChanged -= new EventHandler ( mNewDirectoryTextBox_TextChanged );
                mNewDirectoryTextBox.Text = Directory.GetParent ( mLocationTextBox.Text ).FullName;
                mNewDirectoryTextBox.TextChanged += new EventHandler ( mNewDirectoryTextBox_TextChanged );
                mUserSetLocation = true;
                mNewDirectoryTextBox.ReadOnly = true;
            }
        }

        // Validate the chosen location before closing
        private void mOKButton_Click(object sender, EventArgs e)
        {
            string newPath = mLocationTextBox.Text;
            try
            {
                // Must not save in same directory
                if (Path.GetFullPath(Path.GetDirectoryName(newPath)) ==
                    Path.GetFullPath(Path.GetDirectoryName(mOriginalProjectPath)))
                {
                    MessageBox.Show(Localizer.Message("save_as_error_same_directory"));
                    mCanClose = false;
                }
                // The selected location must be suitable
                else if (!ObiForm.CheckProjectPath(newPath, true))
                {
                    mCanClose = false;
                }
                else
                {
                    mNewProjectPath = newPath;
                    mCanClose = true;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(string.Format(Localizer.Message("cannot_use_project_path"), newPath, x.Message),
                    Localizer.Message("cannot_use_project_path_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                mCanClose = false;
            }
        }

        // Only close if we really can.
        private void SaveProjectAsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mCanClose)
            {
                e.Cancel = true;
                mCanClose = true;
            }
        }

        // Update location on the fly when the target directory changes
        private void mNewDirectoryTextBox_TextChanged(object sender, EventArgs e) 
            {
                        GenerateFileName(); 
            }
    }
}