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
        private Settings mSettings; //@fontconfig
        private bool m_IsMegaVoiceConnect = false;
        private string m_TemprorayPathOfMegavoiceExport = string.Empty;
        private string m_MegaVoiceExportFileName = string.Empty;
        private bool m_IsInsertCuePoints = false;
        private bool m_IsCueMarkingAllowed = false;

        public ExportDirectory(string path, string xukPath, bool encodeToMP3, double bitRate, bool appendSectionNameToAudioFile, string encodingType, Settings settings, bool isMegaVoiceConnect = false,string megaVoiceFileName = "")
        {
            InitializeComponent();
            if (!isMegaVoiceConnect)
            {
                mPathTextBox.Text = path;
            }
            else
            {
                mPathTextBox.Text = string.Empty;
                m_TemprorayPathOfMegavoiceExport = path;
                m_MegaVoiceExportFileName = megaVoiceFileName;
            }

            mXukPath = xukPath;
            mCanClose = true;
            mSettings = settings; //@fontconfig
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message("EachLevel"));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level1"));
            m_ComboSelectLevelForAudioFiles.Items.Add(Localizer.Message("Level2"));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level3" ));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level4" ));
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level5" ) );
            m_ComboSelectLevelForAudioFiles.Items.Add ( Localizer.Message ("Level6" ) );

            m_Mp3Mp4Bitrates = new List<double>(new double[] { 32, 40, 48, 56, 64, 96, 128, 160, 196,224,256,320 });
            m_AmrBitrates = new List<double>(new double[] { 4.75, 5.15, 5.90, 6.70, 7.40, 7.95, 10.20, 12.20 });
            m_3gpBitrates = new List<double>(new double[] { 4.75, 5.15, 5.90, 6.70, 7.40, 7.95, 10.20, 12.20, 14.25, 15.85, 18.25, 19.85, 23.05, 23.85 });
            if (!isMegaVoiceConnect)
            {
                m_EncodingOptions = new List<string>(new string[] { "MP3", "MP4 (M4A)", "AMR", "3GP" });
            }
            else
            {
                m_EncodingOptions = new List<string>(new string[] { "MP3", "MP4 (M4A)"});
            }
            m_comboBoxEncodingType.Items.Clear();
            string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
            if (!File.Exists(ffmpegPath))
            {
                m_comboBoxEncodingType.Items.Add(m_EncodingOptions[0]);
                m_btnEncodingOptions.Visible = true;
                m_comboBoxEncodingType.SelectedIndex = 0;
            //    m_ComboBoxBitrate.SelectedIndex = 4;
                encodingType = AudioLib.AudioFileFormats.MP3.ToString();
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
            if (encodingType == AudioLib.AudioFileFormats.MP3.ToString())
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
            if (m_encodingType == 0)
            {
                m_btnAdvance.Enabled = encodeToMP3;
            }
            m_IsMegaVoiceConnect = isMegaVoiceConnect;
            m_checkBoxAddSectionNameToAudioFileName.Checked = appendSectionNameToAudioFile;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Exporting and Validating DTB/Exporting as DAISY DTB.htm");
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
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
                set
                {
                    m_ComboSelectLevelForAudioFiles.SelectedIndex = value;
                }
            }

        public bool SelectLevelForAudioLevelsEnabled
        {
            set
            {
                m_ComboSelectLevelForAudioFiles.Enabled = value;
            }
        }

        public bool EncodeAudioFiles
        {
            get { return m_IsEncoderCheck; }
            set
            {
                m_checkBoxEncoder.Checked = value;
            }
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
            set
            {
                m_checkBoxAddSectionNameToAudioFileName.Checked = value;
            }
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
        public bool CreateMediaOverlaysForNavigationDocChecked
        {
            get { return m_chkBoxCreateMediaOverlays.Checked; }
            set
            {
                m_chkBoxCreateMediaOverlays.Enabled = value;
            }
        }
        public bool XhtmlElmentsEnabled
        {
            set
            {
                m_lblSelectLevelForAudioFiles.Enabled = value;
                m_ComboSelectLevelForAudioFiles.Enabled = value;
                m_btnEncodingOptions.Enabled = value;
                m_checkBoxEncoder.Enabled = value;
                m_comboBoxEncodingType.Enabled = value;
                m_ComboBoxBitrate.Enabled = value;
                m_btnAdvance.Enabled = value;
                m_checkBoxAddSectionNameToAudioFileName.Enabled = value;
                m_chkBoxFilenameLengthLimit.Enabled = value;
                m_numericUpDownFilenameLengthLimit.Enabled = value;
                m_chkBoxEpubFilenameLengthLimit.Enabled = value;
                m_numericUpDownEpubFilenameLengthLimit.Enabled = value;
                m_chkBoxDummyTextHTMLfiles.Enabled = value;
                m_chkBoxCreateMediaOverlays.Enabled = value;
                m_grpBoxSectionNameOperation.Enabled = value;
                m_grpBoxMP3Encoding.Enabled = false;
                m_EpubFileNamegroupBox.Enabled = false;
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

        public bool CreateCSVForCues
        {
            get
            {
                return m_chkBoxCreateCsvForCues.Checked;
            }
        }

        public bool CreateCSVForCuesEnabled
        {
            set
            {
                m_chkBoxCreateCsvForCues.Enabled = value;
            }
        }

        public bool AddCuePointsInAudioEnabled
        {
            set
            {
                if (m_checkBoxEncoder.Checked)
                {
                    m_chkBoxAddCuePointsInAudio.Enabled = false;
                }
                else
                {
                    m_chkBoxAddCuePointsInAudio.Enabled = value;
                }
                if (value)
                {
                    m_IsCueMarkingAllowed = true;
                }
            }
        }

        public bool AddCuePoints
        {
            get 
            {
                return m_chkBoxAddCuePointsInAudio.Checked;
            }
        }

        public bool IsInsertCuePoints
        {
            get
            {
                return m_IsInsertCuePoints;
            }
        }
        private void mSelectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = mPathTextBox.Text;
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == DialogResult.OK && (m_IsMegaVoiceConnect || ObiForm.CheckProjectDirectory_Safe(dialog.SelectedPath, true)))
            {
                if (m_IsMegaVoiceConnect)
                {
                    var drives = DriveInfo.GetDrives();
                    bool isRemovableDrive = false;
                    //List<string> temp = new List<string>();
                    foreach (var drive in drives)
                    {
                        if (drive.DriveType == DriveType.Removable)
                        {
                            Console.WriteLine(drive.Name);
                            //temp.Add(drive.Name);
                            if(dialog.SelectedPath.Contains(drive.Name))
                            {
                                if(dialog.SelectedPath == drive.Name)
                                isRemovableDrive = true;
                            }
                        }
                    }
                    if (!isRemovableDrive)
                    {
                        DialogResult tempResult = MessageBox.Show(Localizer.Message("USBDriveCheck"), Localizer.Message("Caption_Information"), MessageBoxButtons.YesNo,MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                      if (tempResult == DialogResult.No)
                          return;
                    }
                    
                }
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
            if (string.IsNullOrEmpty(mPathTextBox.Text))
            {
                MessageBox.Show(Localizer.Message("ExportDirectoryPathEmpty"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                mCanClose = false;
                return;
            }
            if (!m_IsMegaVoiceConnect)
            {
                mCanClose = ObiForm.CheckProjectDirectory_Safe(DirectoryPath, true);
            }
            else
            {
                if (!string.IsNullOrEmpty(mPathTextBox.Text) &&  !string.IsNullOrEmpty(m_MegaVoiceExportFileName))
                {
                    string MegaVoiceExportFinalPath = mPathTextBox.Text + "\\" + m_MegaVoiceExportFileName;
                    if (Directory.Exists(MegaVoiceExportFinalPath))
                    {
                        DialogResult tempResult = MessageBox.Show(Localizer.Message("ExportFolderExistsMegavoice"), Localizer.Message("Caption_Warning"),
                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (tempResult == DialogResult.No)
                        {
                            mCanClose = false;
                            return;
                        }
                    }
                    mCanClose = ObiForm.CheckProjectDirectory_Safe(m_TemprorayPathOfMegavoiceExport, true, false);
                }

            }
            if (mCanClose && m_chkBoxCreateMediaOverlays.Enabled)
            {
                mSettings.Export_EpubCreateMediaOverlays = m_chkBoxCreateMediaOverlays.Checked;
            }
            if (m_chkBoxAddCuePointsInAudio.Checked)
            {
                DialogResult result = MessageBox.Show(string.Format(Localizer.Message("ExportInsertCuePointOrCopy"),"\n"), Localizer.Message("Caption_Information"), MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    m_IsInsertCuePoints = true;
                }
            }
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
            if (m_IsCueMarkingAllowed)
            {
                if (m_checkBoxEncoder.Checked)
                {
                    m_chkBoxAddCuePointsInAudio.Enabled = false;
                    m_chkBoxAddCuePointsInAudio.Checked = false;
                }
                else
                {
                    m_chkBoxAddCuePointsInAudio.Enabled = true;
                }
            }
            m_IsEncoderCheck = m_checkBoxEncoder.Checked;
            m_comboBoxEncodingType.Enabled = m_checkBoxEncoder.Checked;
            m_ComboBoxBitrate.Enabled = m_checkBoxEncoder.Checked;
            if (m_comboBoxEncodingType.SelectedIndex == 0)
            {
                m_btnAdvance.Enabled = m_checkBoxEncoder.Checked;
            }
            else
            {
                m_btnAdvance.Enabled = false;
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
            m_ExportAdvance = new ExportAdvance(mSettings); //@fontconfig
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
                DownloadFile download = new DownloadFile(mSettings); //@fontconfig
                download.ShowDialog();
        }

        private void m_chkBoxCreateMediaOverlays_EnabledChanged(object sender, EventArgs e)
        {
            if (m_chkBoxCreateMediaOverlays.Enabled)
            {
                m_chkBoxCreateMediaOverlays.Checked = mSettings.Export_EpubCreateMediaOverlays;
            }
        }

        private void m_chkBoxCreateCsvForCues_CheckedChanged(object sender, EventArgs e)
        {
            if (m_chkBoxCreateCsvForCues.Checked)
            {
                m_ComboSelectLevelForAudioFiles.SelectedIndex = 0;
                m_ComboSelectLevelForAudioFiles.Enabled = false;
            }
            else
            {
                if (m_chkBoxAddCuePointsInAudio.Checked)
                {
                    m_ComboSelectLevelForAudioFiles.Enabled = false;
                }
                else
                {
                    m_ComboSelectLevelForAudioFiles.Enabled = true;
                }
            }
        }

        private void m_chkBoxAddCuePointsInAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (m_chkBoxAddCuePointsInAudio.Checked)
            {
                m_ComboSelectLevelForAudioFiles.SelectedIndex = 0;
                m_ComboSelectLevelForAudioFiles.Enabled = false;
                m_checkBoxEncoder.Enabled = false;
                m_checkBoxEncoder.Checked = false;
            }
            else
            {
                if (m_chkBoxCreateCsvForCues.Checked)
                {
                    m_ComboSelectLevelForAudioFiles.Enabled = false;
                }
                else
                {
                    m_ComboSelectLevelForAudioFiles.Enabled = true;
                }
                m_checkBoxEncoder.Enabled = true;
            }
        }              
    }
}