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
        private int m_BitRate;
        private bool m_IsMP3Check;
        private bool m_SectionNameToAudioFileNameCheck;
        private bool m_FilenameLengthLimit;
        private ExportAdvance mExportAdvance;

        public ExportDirectory(string path, string xukPath, bool encodeToMP3, int bitRate, bool appendSectionNameToAudioFile)
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
            if (bitRate != 0)
            {
                m_ComboBoxBitrate.SelectedIndex = 0;
                //m_ComboBoxBitrate.SelectedIndex = bitRate == 32 ? 0 : bitRate == 48 ? 1 : bitRate == 64 ? 2 : 3;
                for (int i=0; i < m_ComboBoxBitrate.Items.Count;i++)  
                {
                    if( int.Parse( m_ComboBoxBitrate.Items[i].ToString ()) == bitRate )
                    {
                        m_ComboBoxBitrate.SelectedIndex = i;
                        break;
                    }
                }
            }
            m_checkBoxMP3Encoder.Checked = encodeToMP3;
            m_checkBoxAddSectionNameToAudioFileName.Checked = appendSectionNameToAudioFile;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Exporting and Validating DTB/Exporting as DAISY DTB.htm");
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
        public bool EncodeToMP3
        {
            get { return m_IsMP3Check; }
        }

        public int BitRate
        {
            get { return m_BitRate; }
        }

        public bool AppendSectionNameToAudioFileName
        {
            get { return m_checkBoxAddSectionNameToAudioFileName.Checked; }
        }

        public bool LimitLengthOfAudioFileNames
        {
            get { return m_chkBoxFilenameLengthLimit.Checked; }
            set { m_chkBoxFilenameLengthLimit.Checked = value; }
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
        public int AudioFileNameCharsLimit
        {
            get { return m_chkBoxFilenameLengthLimit.Checked ? Convert.ToInt32(m_numericUpDownFilenameLengthLimit.Value) : -1; }
            set { m_numericUpDownFilenameLengthLimit.Value = value;  }
        }

        public string AdditionalTextForTitle { set { if (!Text.Contains (value)) Text =Text +  "("+value+")" ;} }

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

        private void m_checkBoxMP3Encoder_CheckedChanged(object sender, EventArgs e)
        {
            m_IsMP3Check = m_checkBoxMP3Encoder.Checked;
            m_ComboBoxBitrate.Enabled = m_checkBoxMP3Encoder.Checked;            
        }

        
        private void m_ComboBoxBitrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_BitRate = int.Parse(m_ComboBoxBitrate.Items[m_ComboBoxBitrate.SelectedIndex].ToString());                   
        }

        private void m_chkBoxFilenameLengthLimit_CheckedChanged(object sender, EventArgs e)
        {
            m_FilenameLengthLimit = m_chkBoxFilenameLengthLimit.Checked;
            m_numericUpDownFilenameLengthLimit.Enabled = m_chkBoxFilenameLengthLimit.Checked;
        }

        private void m_checkBoxAddSectionNameToAudioFileName_CheckedChanged(object sender, EventArgs e)
        {
            m_SectionNameToAudioFileNameCheck = m_checkBoxAddSectionNameToAudioFileName.Checked;
            m_chkBoxFilenameLengthLimit.Enabled = m_checkBoxAddSectionNameToAudioFileName.Checked;

        }


        private void mbtnAdvance_Click(object sender, EventArgs e)
        {
            mExportAdvance = new ExportAdvance();
            // this.Controls.Add(mExportAdvance);    
            mExportAdvance.Show();
            

        }              
    }
}