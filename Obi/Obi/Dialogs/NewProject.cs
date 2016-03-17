using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private string mBasepath;       // the base path for the project
        private bool mCanClose;         // can close the form or not (cannot close if file existed)
        private string mExtension;      // file extension for the file chooser
        private string mFilename;       // the actual filename
        private bool mUserSetLocation;  // the location was changed manually by the user
        private AudioSettings m_AudioSettingsDialog = null;

        /// <summary>
        /// Create a new dialog with default information (dummy name and default path.)
        /// </summary>
        /// <param name="basepath">The initial directory where to create the project.</param>
        /// <param name="filename">The actual file name for the project.</param>
        /// <param name="extension">The file extension (for a file dialog.)</param>
        /// <param name="title">The project title.</param>
        public NewProject(string basepath, string filename, string extension, string title, Size size, int defaultAudioChannels, int defaultAudioSampleRate, Settings settings)
        {
            InitializeComponent();
            mTitleBox.Text = title;
            mTitleBox.SelectionStart = 0;
            mTitleBox.SelectionLength = title.Length;
            mIDBox.Text = Guid.NewGuid().ToString();
            mBasepath = basepath;
            mExtension = extension;
            mFilename = filename;
            mCanClose = true;
            mUserSetLocation = false;
            if (size.Width >= MinimumSize.Width && size.Height >= MinimumSize.Height) Size = size;
            GenerateFileName();
            m_AudioSettingsDialog = new AudioSettings(defaultAudioChannels, defaultAudioSampleRate, settings);//@fontconfig
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
         //   m_AudioSettingsDialog = new AudioSettings(defaultAudioChannels, defaultAudioSampleRate);
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Creating and Working with Projects/Creating a new project.htm");
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
        /// Generated or user-chosen ID for the project.
        /// </summary>
        public string ID 
        { 
            get { return mIDBox.Text; }
            set { mIDBox.Text = value; }
        }

        /// <summary>
        /// The chosen path for the XUK project file; derived from the title or chosen
        /// by the user.
        /// </summary>
        public string Path { get { return mFileBox.Text; }}

        /// <summary>
        /// The chosen title for the project.
        /// </summary>
        public string Title { get { return mTitleBox.Text; } }

        public int AudioChannels { get { return m_AudioSettingsDialog != null ? m_AudioSettingsDialog.AudioChannels : 1; } }
        public int AudioSampleRate { get { return m_AudioSettingsDialog != null ? m_AudioSettingsDialog.AudioSampleRate : 44100; } }

        // Update the file box to generate a filename for the project
        private void GenerateFileName()
        {
            if (!mUserSetLocation) mFileBox.Text = GetFileName(mBasepath, mFilename);
        }

        // Get a filename for a given path/title
        private string GetFileName(string path, string filename)
        {
            return String.Format("{0}{1}{2}{1}{3}",
                path,
                System.IO.Path.DirectorySeparatorChar,
                Program.SafeName(mTitleBox.Text),
                filename);
        }

        /// <summary>
        /// Before closing, make sure that the directory chosen works.
        /// </summary>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            mCanClose = false;
            try
            {

                if (mTitleBox.Text == "")
                {
                    MessageBox.Show(Localizer.Message("NewProject_EmptyTitle"));
                    return;
                }
                ObiForm.CheckProjectPath(mFileBox.Text, true);

                m_AudioSettingsDialog.ShowDialog();

                mCanClose = true;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, Localizer.Message("location_error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Update the path text box with the selected path from the file chooser.
        /// </summary>
        private void mSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            try
            {
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(mFileBox.Text);
            }
            catch (Exception) { }
            dialog.FileName = mFilename;
            dialog.Filter = mExtension;
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                string dir = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (System.IO.Directory.Exists(dir) &&
                    (System.IO.Directory.GetFiles(dir).Length > 0 ||
                    System.IO.Directory.GetDirectories(dir).Length > 0))
                {
                    // If trying to create a project in a non-empty directory,
                    // propose a new path to the user in this directory.
                    string p_path = GetFileName(dir, System.IO.Path.GetFileName(path));
                    p_path = System.IO.Path.GetFullPath (p_path);
                    DialogResult result = MessageBox.Show(
                        String.Format(Localizer.Message("propose_directory_text"), p_path),
                        Localizer.Message("propose_directory_caption"),
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        path = p_path;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }   
                mFileBox.Text = path;
                mUserSetLocation = true;
            }
        }

        /// <summary>
        /// Generate a new XUK file name when a new title is chosen.
        /// </summary>
        private void mTitleBox_TextChanged(object sender, EventArgs e)     { GenerateFileName(); 

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

        // Generate a new GUID
        private void mGenerateIDButton_Click(object sender, EventArgs e) { mIDBox.Text = Guid.NewGuid().ToString(); }
    }
}