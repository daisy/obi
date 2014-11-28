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
        private double m_BitRate;
        private bool m_IsEncoderCheck;
        private bool m_SectionNameToAudioFileNameCheck;
        private bool m_FilenameLengthLimit;
        private ExportAdvance m_ExportAdvance;
        private int m_encodingType = 0;
        private List<double> m_Mp3Mp4Bitrates;
        private List<double> m_AmrBitrates; 
        private List<double> m_3gpBitrates;
        private List<string> m_EncodingOptions;
      
        public ExportDirectory(string path, string xukPath, bool encodeToMP3, double bitRate, bool appendSectionNameToAudioFile,string encodingType)
        {
            InitializeComponent();
            mPathTextBox.Text = path;
            mXukPath = xukPath;
            mCanClose = true;
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message("EachLevel"));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level1"));
            m_ComboSelectLevelForAudioFiles.Items.Add(Localizer.Message("Level2"));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level3" ));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level4" ));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level5" ) );
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level6" ) );

            m_Mp3Mp4Bitrates = new List<double>(new double[] { 32, 40, 48, 56, 64, 128, 196, 256 });
            m_AmrBitrates = new List<double>(new double[] { 4.75, 5.15, 5.90, 6.70, 7.40, 7.95, 10.20, 12.20 });
            m_3gpBitrates = new List<double>(new double[] { 4.75, 5.15, 5.90, 6.70, 7.40, 7.95, 10.20, 12.20, 14.25, 15.85, 18.25, 19.85, 23.05, 23.85 });
            m_EncodingOptions = new List<string>(new string[] { "MP3", "MP4 (M4A)", "AMR", "3GP" });
            m_comboBoxEncodingType.Items.Clear();
            string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
            if (!File.Exists(ffmpegPath))
            {
                m_comboBoxEncodingType.Items.Add(m_EncodingOptions[0]);
                m_btnEncodingOptions.Visible = true;
            }
            else
            {
                foreach (string encode in m_EncodingOptions)
                {
                    m_comboBoxEncodingType.Items.Add(encode);
                }
                m_btnEncodingOptions.Visible = false;
            }

            m_encodingType = 0;//encodingType;
            if (encodingType ==AudioLib.AudioFileFormats.MP3.ToString())
            {
                m_encodingType = 0;
            }
            else if (encodingType == AudioLib.AudioFileFormats.MP4.ToString())
            {
                m_encodingType = 1;
            }
            else if (encodingType == AudioLib.AudioFileFormats.AMR.ToString())
            {
                m_encodingType = 2;
            }
            else if (encodingType == AudioLib.AudioFileFormats.GP3.ToString())
            {
                m_encodingType = 3;
            }
            m_ComboSelectLevelForAudioFiles.SelectedIndex = 0 ;
            m_ComboBoxBitrate.Items.Clear();
            if (m_encodingType == 0 || m_encodingType == 1)
            {
                foreach(double bitrateval in m_Mp3Mp4Bitrates)
                {
                    m_ComboBoxBitrate.Items.Add(bitrateval);
                }
               m_ComboBoxBitrate.SelectedIndex = 4;
                if (m_encodingType == 1)
                {
                    m_comboBoxEncodingType.SelectedIndex = 1;
                }
                else
                {
                    m_comboBoxEncodingType.SelectedIndex = 0;
                }
            }
            if (m_encodingType == 2)
            {
                foreach(double bitrateval in m_AmrBitrates)
                {
                    m_ComboBoxBitrate.Items.Add(bitrateval);
                }
              m_ComboBoxBitrate.SelectedIndex = 5;
                m_comboBoxEncodingType.SelectedIndex = 2;
            }
            if (m_encodingType == 3)
            {
                foreach (double bitrateval in m_3gpBitrates)
                {
                    m_ComboBoxBitrate.Items.Add(bitrateval);
                }
            m_ComboBoxBitrate.SelectedIndex = 8;
                m_comboBoxEncodingType.SelectedIndex = 3;
            }
           // Array Mp4Bitrate ={16,32,64,128,196,256,384};

            if (bitRate != 0)
            {
                //  m_ComboBoxBitrate.SelectedIndex = ;
                //m_ComboBoxBitrate.SelectedIndex = bitRate == 32 ? 0 : bitRate == 48 ? 1 : bitRate == 64 ? 2 : 3;
                for (int i = 0; i < m_ComboBoxBitrate.Items.Count; i++)
                {
                    if (double.Parse(m_ComboBoxBitrate.Items[i].ToString()) == bitRate)
                    {
                        m_ComboBoxBitrate.SelectedIndex = i;
                        break;
                    }
                }
            }            

            m_checkBoxEncoder.Checked = encodeToMP3;
            m_comboBoxEncodingType.Enabled = m_checkBoxEncoder.Checked;
            m_ComboBoxBitrate.Enabled = m_checkBoxEncoder.Checked;
            m_btnAdvance.Enabled = m_checkBoxEncoder.Checked;
            m_btnAdvance.Enabled = encodeToMP3;
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
        public bool EncodeAudioFiles
        {
            get { return m_IsEncoderCheck; }
        }

        public double BitRate
        {
            get { return m_BitRate; }
        }
        public AudioLib.AudioFileFormats EncodingFileFormat
        {
            get
            {
                return m_comboBoxEncodingType.SelectedIndex <= 0 ? AudioLib.AudioFileFormats.MP3 :
                m_comboBoxEncodingType.SelectedIndex == 1 ? AudioLib.AudioFileFormats.MP4 :
                m_comboBoxEncodingType.SelectedIndex == 2 ? AudioLib.AudioFileFormats.AMR:
                AudioLib.AudioFileFormats.GP3;
            }
        }

        public bool EnabledAdvancedParameters { get { return m_ExportAdvance != null && m_ExportAdvance.EnableAdvancedParameters; } }

        public bool Mp3ReSample { get { return EnabledAdvancedParameters && m_ExportAdvance.OptionalReSample ; } }
        public string Mp3RePlayGain { get { return EnabledAdvancedParameters? m_ExportAdvance.OptionalRePlayGain: null; } }
        public string Mp3ChannelMode { get { return EnabledAdvancedParameters? m_ExportAdvance.OptionalChannelMode: null; } }

        public bool AppendSectionNameToAudioFileName
        {
            get { return m_checkBoxAddSectionNameToAudioFileName.Checked; }
        }

        public bool LimitLengthOfAudioFileNames
        {
            get { return m_chkBoxFilenameLengthLimit.Checked; }
            set { m_chkBoxFilenameLengthLimit.Checked = value; }
        }
        public bool EpubLengthCheckboxEnabled
        {
            get { return m_chkBoxEpubFilenameLengthLimit.Checked; }
            set
            {
                m_chkBoxEpubFilenameLengthLimit.Enabled = value;
                m_EpubFileNamegroupBox.Enabled = value;
            }
            
        }
        public bool CreateDummyTextCheckboxEnabled
        {
            get { return m_chkBoxDummyTextHTMLfiles.Checked; }
            set
            {
                m_chkBoxDummyTextHTMLfiles.Enabled = value;               
            }
                        
        }

        public bool EPUB_CreateDummyTextInHtml 
        { 
            get { return m_chkBoxDummyTextHTMLfiles.Enabled && m_chkBoxDummyTextHTMLfiles.Checked; }
            set
            {
                m_chkBoxDummyTextHTMLfiles.Checked = value;
            }
        }

        public int EPUBFileLength
        {
            get
            {
                return Convert.ToInt32 (m_numericUpDownEpubFilenameLengthLimit.Value);
            }
            set
            {
                m_numericUpDownEpubFilenameLengthLimit.Value = value;
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
            m_IsEncoderCheck = m_checkBoxEncoder.Checked;
            m_comboBoxEncodingType.Enabled = m_checkBoxEncoder.Checked;
            m_ComboBoxBitrate.Enabled = m_checkBoxEncoder.Checked;
            m_btnAdvance.Enabled = m_checkBoxEncoder.Checked;
            if (m_comboBoxEncodingType.SelectedIndex == 0)
            {
                m_ComboBoxBitrate.Enabled = true;
                m_btnAdvance.Enabled = true;
            }

            // m_btnAdvance.Enabled = m_checkBoxEncoder.Checked;            
        }

        
        private void m_ComboBoxBitrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            //m_BitRate = int.Parse(m_ComboBoxBitrate.Items[m_ComboBoxBitrate.SelectedIndex].ToString());   
            m_BitRate = double.Parse(m_ComboBoxBitrate.Items[m_ComboBoxBitrate.SelectedIndex].ToString());     
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
            m_ExportAdvance = new ExportAdvance();
            // this.Controls.Add(mExportAdvance);    
            m_ExportAdvance.Show();
            

        }

        private void m_chkBoxEpubFilenameLengthLimit_CheckedChanged(object sender, EventArgs e)
        {
            if (m_chkBoxEpubFilenameLengthLimit.Checked)
            {
                m_numericUpDownEpubFilenameLengthLimit.Enabled = true;
            }
            else
            {
                m_numericUpDownEpubFilenameLengthLimit.Enabled = false;
            }
        }

        private void m_comboBoxEncodingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_encodingType = m_comboBoxEncodingType.SelectedIndex; ;          
            m_ComboBoxBitrate.Items.Clear();
            if (m_encodingType == 0 || m_encodingType == 1)
            {

                foreach (double bitrateval in m_Mp3Mp4Bitrates)
                {
                    m_ComboBoxBitrate.Items.Add(bitrateval);
                }
                if (m_encodingType == 0)
                {
                    m_ComboBoxBitrate.SelectedIndex = 4;
                    m_btnAdvance.Enabled = true;
                }
                else
                {
                    m_ComboBoxBitrate.SelectedIndex = 2;
                    m_btnAdvance.Enabled = false;
                }
               
            }
            if (m_encodingType == 2)
            {
                foreach (double bitrateval in m_AmrBitrates)
                {
                    m_ComboBoxBitrate.Items.Add(bitrateval);
                }
                m_ComboBoxBitrate.SelectedIndex = m_ComboBoxBitrate.Items.Count - 1;
                m_btnAdvance.Enabled = false;
            }
            if (m_encodingType == 3)
            {
                foreach (double bitrateval in m_3gpBitrates)
                {
                    m_ComboBoxBitrate.Items.Add(bitrateval);
                }
                m_ComboBoxBitrate.SelectedIndex = m_ComboBoxBitrate.Items.Count - 1;
                m_btnAdvance.Enabled = false;
            }
        }

        private void m_btnEncodingOptions_Click(object sender, EventArgs e)
        {          
                DownloadFile download = new DownloadFile();
                download.ShowDialog();
        }              
    }
}