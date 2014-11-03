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
        private string mDir;
        private string m_FullPath;
        private bool m_PathChecked = false;
        /// <summary>
        /// Used by the designer.
        /// </summary>
        public SaveProjectAsDialog()
        {
            InitializeComponent();
            mCanClose = true;
            mUserSetLocation = false;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Creating and Working with Projects/Saving Project in a different Location.htm");
        }

        /// <summary>
        /// Create a new dialog for the original project path
        /// </summary>
        public SaveProjectAsDialog(string path,Settings settings)
            : this()
        {
            mFilename = Path.GetFileName(path);
            mDir = Path.GetDirectoryName(path);
          
            mOriginalProjectPath = path;
        //    mNewDirectoryTextBox.Text = string.Format(Localizer.Message("save_as_new_directory_name"), mDir);
        //    mNewDirectoryTextBox.SelectionStart = 0;
        //    mNewDirectoryTextBox.SelectionLength = mNewDirectoryTextBox.Text.Length;
            m_ProjectNameTextBox.Text = mFilename;
            GenerateFileName();
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
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
            if (!mUserSetLocation) //mLocationTextBox.Text = Path.Combine(mNewDirectoryTextBox.Text, mFilename);
                mLocationTextBox.Text = string.Format(Localizer.Message("save_as_new_directory_name"), mDir);
        }


        // Browse to a new location.
        private void mSelectButton_Click(object sender, EventArgs e)
        {
            //  SaveFileDialog dialog = new SaveFileDialog();
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string[] logicalDrives = System.IO.Directory.GetLogicalDrives();
        //    dialog.AddExtension = true;
            try
            {
          //      dialog.FileName = Path.GetFullPath(mLocationTextBox.Text);
                dialog.SelectedPath = Path.GetFullPath(mLocationTextBox.Text);        
            }
            catch (Exception) {}
        //    dialog.DefaultExt = Localizer.Message("obi_filter");
            
            if (dialog.ShowDialog () == DialogResult.OK )
            {
                if (Path.GetFullPath(Path.GetDirectoryName(Path.Combine(dialog.SelectedPath, m_ProjectNameTextBox.Text))) ==
                    Path.GetFullPath(Path.GetDirectoryName(mOriginalProjectPath)))
                {
                    MessageBox.Show(Localizer.Message("save_as_error_same_directory"));
                    return;
                }
                foreach (string drive in logicalDrives)
                {
                    if ((dialog.SelectedPath == drive) || (dialog.SelectedPath == Environment.GetFolderPath(Environment.SpecialFolder.Desktop)) || (dialog.SelectedPath == Environment.GetFolderPath(Environment.SpecialFolder.MyComputer) || (dialog.SelectedPath == Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))))
                    {
                        MessageBox.Show(string.Format(Localizer.Message("SaveAs_cannot_save_in_root"), dialog.SelectedPath));
                        return;
                    }
                }
                if (ObiForm.CheckProjectPath_Safe(Path.Combine(dialog.SelectedPath, m_ProjectNameTextBox.Text), true))
                {
                    m_PathChecked = true; 
                    mLocationTextBox.Text = dialog.SelectedPath;
                    mUserSetLocation = true;

                    // mLocationTextBox.Text = Path.GetDirectoryName(dialog.FileName);
                    // mLocationTextBox.Text = dialog.FileName;        
                    // mNewDirectoryTextBox.TextChanged -= new EventHandler ( mNewDirectoryTextBox_TextChanged );
                    // mNewDirectoryTextBox.Text = Directory.GetParent ( mLocationTextBox.Text ).FullName;
                    // mNewDirectoryTextBox.TextChanged += new EventHandler ( mNewDirectoryTextBox_TextChanged );               
                    // mNewDirectoryTextBox.ReadOnly = true;
                    // mLocationTextBox.Text = Path.GetFullPath(string.Format(Localizer.Message("save_as_new_directory_name"), mDir));
                    //  m_ProjectNameTextBox.Text = "project.obi";
                }                 
            }
        }

        // Validate the chosen location before closing
        private void mOKButton_Click(object sender, EventArgs e)
        {
            string newPath = Path.Combine(mLocationTextBox.Text, m_ProjectNameTextBox.Text);
            try
            {
                string[] logicalDrives = System.IO.Directory.GetLogicalDrives();
                foreach (string drive in logicalDrives)
                {
                    if ((mLocationTextBox.Text == drive) || (mLocationTextBox.Text == Environment.GetFolderPath(Environment.SpecialFolder.Desktop)) || (mLocationTextBox.Text == Environment.GetFolderPath(Environment.SpecialFolder.MyComputer) || (mLocationTextBox.Text == Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))))
                    {
                        MessageBox.Show(string.Format(Localizer.Message("SaveAs_cannot_save_in_root"), mLocationTextBox.Text));
                        mCanClose = false;
                        return;
                    }
                    // Must not save in same directory                    
                }
                if (Path.GetFullPath(Path.GetDirectoryName(newPath)) ==
                        Path.GetFullPath(Path.GetDirectoryName(mOriginalProjectPath)))
                    {
                        MessageBox.Show(Localizer.Message("save_as_error_same_directory"));
                        mCanClose = false;
                    }

                    // The selected location must be suitable
                    else if (!m_PathChecked && !ObiForm.CheckProjectPath(newPath, true))
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
                      //  GenerateFileName(); 
            }
    }
}