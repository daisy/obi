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
    public partial class ExportDirectory : Form
    {
        private string mXukPath;  // path to XUK project (for relative paths)
        private bool mCanClose;   // can prevent from closing on problem
   
        public ExportDirectory(string path, string xukPath)
        {
            InitializeComponent();
            mPathTextBox.Text = path;
            mXukPath = xukPath;
            mCanClose = true; 

            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message("EachLevel"));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level1") );
            m_ComboSelectLevelForAudioFiles.Items.Add (Localizer.Message ("Level2" )) ;
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level3" ) );
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level4" ) );
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level5" ) );
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level6" ) );

            
            m_ComboSelectLevelForAudioFiles.SelectedIndex = 0 ;

        }

        /// <summary>
        /// Get the full path (if not rooted, use the xuk path as context.)
        /// </summary>
        public string DirectoryPath
        {
            get
            {
                try
                {
                    return Path.IsPathRooted(mPathTextBox.Text) ?
                        mPathTextBox.Text :
                        Path.Combine(Path.GetDirectoryName(mXukPath), mPathTextBox.Text);
                }
                catch (Exception)
                {
                    return mPathTextBox.Text;
                }
            }
        }


        /// <summary>
        /// Level upto which one audio file per section has to be made, all section deeper than this level are combined in one audio file
        /// in case one audio file per section has to be followed throughout , a large number, 100 is returned
                /// </summary>
        public int LevelSelection
            {
            get
                {
                return m_ComboSelectLevelForAudioFiles.SelectedIndex == 0? 100 : m_ComboSelectLevelForAudioFiles.SelectedIndex;
                }
            }


        private void mSelectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = mPathTextBox.Text;
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == DialogResult.OK && ObiForm.CheckProjectDirectory_Safe(dialog.SelectedPath, true))
            {
                mPathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void mOKButton_Click(object sender, EventArgs e)
        {
            mCanClose = ObiForm.CheckProjectDirectory_Safe(DirectoryPath, true);
        }

        private void SelectDirectoryPath_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mCanClose)
            {
                e.Cancel = true;
                mCanClose = true;
            }
        }              
    }
}