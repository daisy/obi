using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AudioLib;
using System.Xml;

namespace AudioFormatConverterUI
    {

    public partial class m_audioFormatConverterForm : Form
        {
        private XmlDocument m_SettingsDocument;

        public m_audioFormatConverterForm ()
            {
            InitializeComponent ();
            m_cb_channel.SelectedIndex = 0;
            m_cb_sampleRate.SelectedIndex = 0;

            try
                {
                LoadSettings ();
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void m_btn_Add_Click ( object sender, EventArgs e )
            {
            OpenFileDialog addFile = new OpenFileDialog ();
            addFile.RestoreDirectory = true;
            addFile.Multiselect = true;
            //addFile.SafeFileNames = true;
            addFile.Filter = " MP3 Files(*.mp3)|*.mp3| Wave Files (*.wav)|*.wav| All Files (*.*)|*.*";

            if (addFile.ShowDialog ( this ) == DialogResult.OK)
                {
                if (addFile.FileNames.Length > 0)
                    {
                    foreach (string audioFileName in addFile.FileNames)
                        {
                        string filename = Path.GetFullPath ( audioFileName );
                        addFile.CheckFileExists = true;
                        addFile.CheckPathExists = true;
                        m_lb_addFiles.Items.Add ( filename );
                        }
                    }

                }
            m_btn_Start.Enabled = m_lb_addFiles.Items.Count >= 1;
            m_btnReset.Enabled = m_lb_addFiles.Items.Count >= 1;
            }

        private void m_btn_Browse_Click ( object sender, EventArgs e )
            {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog ();
            browserDialog.ShowNewFolderButton = true;
            browserDialog.SelectedPath = m_txt_Browse.Text;

            if (browserDialog.ShowDialog ( this ) == DialogResult.OK)
                {
                m_txt_Browse.Text = browserDialog.SelectedPath;
                if (CheckIfDriveSelected ()) { return; }
                }
            }

        private bool CheckIfDriveSelected ()
            {
            bool flag = false;
            string[] fixedDrives = Environment.GetLogicalDrives ();
            foreach (string drive in fixedDrives)
                {
                if (m_txt_Browse.Text.Equals ( drive, StringComparison.OrdinalIgnoreCase ))
                    {
                    MessageBox.Show ( " Its a root directory , you cannot save here. Please select some other Directory. ", "Root Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                    m_txt_Browse.Clear ();
                    flag = true;
                    }
                }
            return flag;
            }

        private void m_btn_cancel_Click ( object sender, EventArgs e )
            {
            Close ();
            }
        private void m_btnDelete_Click ( object sender, EventArgs e )
            {
            try
                {
                m_lb_addFiles.Items.Remove ( m_lb_addFiles.SelectedItem );
                m_btn_Start.Enabled = m_lb_addFiles.Items.Count >= 1;
                }
            catch (Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }
        private void m_btnReset_Click ( object sender, EventArgs e )
            {
            m_lb_addFiles.Items.Clear ();
            m_btn_Start.Enabled = false;
            m_btnDelete.Enabled = false;
            m_txt_Browse.Clear ();
            }

        private void m_btn_Start_Click ( object sender, EventArgs e )
            {
            if (m_txt_Browse.Text == "")
                {
                MessageBox.Show ( "Output Directory Path cannot be empty. Please select the output Directory Path",
                                "Select Directory", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
                }
            StartConversion ();
            }

        private void StartConversion ()
            {
                m_btn_Add.Enabled = false;
                m_btn_Browse.Enabled = false;
                m_btn_Start.Enabled = false;
                m_btnReset.Enabled = false;
                m_btnDelete.Enabled = false;
                m_cb_channel.Enabled = false;
                m_cb_sampleRate.Enabled = false;
            
            IWavFormatConverter audioConverter = new WavFormatConverter ( true );
            int samplingRate = int.Parse ( m_cb_sampleRate.SelectedItem.ToString () );
            int channels = m_cb_channel.SelectedIndex + 1;
            int bitDepth = 16;
            string outputDirectory = m_txt_Browse.Text;
            string convertedFilePath = null;
            bool flag = false;

            if (!Directory.Exists ( outputDirectory )) return;

            int listPositionIndex = 0;


            while (m_lb_addFiles.Items.Count > listPositionIndex && listPositionIndex < 50)
                {
                string filePath = (string)m_lb_addFiles.Items[listPositionIndex];
                //MessageBox.Show ( filePath );
                try
                    {
                    if (Path.GetExtension ( filePath ) == ".wav")
                        {
                        convertedFilePath = audioConverter.ConvertSampleRate ( filePath, outputDirectory, channels, samplingRate, bitDepth );
                        }
                    else if (Path.GetExtension ( filePath ) == ".mp3")
                        {
                        convertedFilePath = audioConverter.UnCompressMp3File ( filePath, outputDirectory, channels, samplingRate, bitDepth );
                        }
                    // rename converted file to appropriate name
                    string newFilePath = Path.Combine ( outputDirectory,
                        Path.GetFileNameWithoutExtension ( filePath ) ) + ".wav";
                    //MessageBox.Show ( newFilePath );
                    if (File.Exists ( newFilePath ))
                        {
                        if (MessageBox.Show ( "File: " + Path.GetFileName ( newFilePath ) + "  already exists. Do you want to overwrite it?", "Warning", MessageBoxButtons.YesNo ) == DialogResult.Yes)
                            {
                            File.Delete ( newFilePath );
                            File.Move ( convertedFilePath, newFilePath );
                            }
                        }
                    else
                        {
                        File.Move ( convertedFilePath, newFilePath );
                        }

                    m_lb_addFiles.Items.RemoveAt ( 0 );
                    }
                catch (System.Exception ex)
                    {
                    flag = true;
                    MessageBox.Show ( ex.ToString () );
                    listPositionIndex++;
                    }
                }

                if (flag == false)
                {
                    MessageBox.Show("Files have been converted");
                }
                else
                    MessageBox.Show("Some files have not been converted properly");

            
            m_btn_Add.Enabled = true;
            m_btn_Start.Enabled = false;
            m_btnDelete.Enabled = false;
            m_btn_Browse.Enabled = true;
            m_btnReset.Enabled = true;
            m_cb_channel.Enabled = true;
            m_cb_sampleRate.Enabled = true;
            }

       
        private void LoadSettings ()
            {
            string settingsFilePath = Path.Combine (
                System.AppDomain.CurrentDomain.BaseDirectory,
                "settings.xml" );
            m_SettingsDocument = CommonFunctions.CreateXmlDocument ( settingsFilePath );
            string strSamplingRate = m_SettingsDocument.GetElementsByTagName ( "samplingrate" )[0].InnerText;

            int samplingRate = int.Parse ( strSamplingRate );

            for (int i = 0; i < m_cb_sampleRate.Items.Count; i++)
                {

                if (int.Parse ( m_cb_sampleRate.Items[i].ToString () ) == samplingRate)
                    {
                    m_cb_sampleRate.SelectedIndex = i;
                    }
                }

            string strChannels = m_SettingsDocument.GetElementsByTagName ( "channels" )[0].InnerText;
            int channels = int.Parse ( strChannels );

            m_cb_channel.SelectedIndex = channels - 1;

            }

        private void SaveSettings ()
            {
            string settingsFilePath = Path.Combine (
                System.AppDomain.CurrentDomain.BaseDirectory,
                "settings.xml" );

            XmlNode samplingRateNode = m_SettingsDocument.GetElementsByTagName ( "samplingrate" )[0];
            samplingRateNode.RemoveAll ();
            samplingRateNode.AppendChild (
                m_SettingsDocument.CreateTextNode ( m_cb_sampleRate.Items[m_cb_sampleRate.SelectedIndex].ToString () ) );

            XmlNode channelsNode = m_SettingsDocument.GetElementsByTagName ( "channels" )[0];
            channelsNode.RemoveAll ();
            channelsNode.AppendChild (
                m_SettingsDocument.CreateTextNode ( (m_cb_channel.SelectedIndex + 1).ToString () ) );

            CommonFunctions.WriteXmlDocumentToFile ( m_SettingsDocument, settingsFilePath );
            }

        private void m_audioFormatConverterForm_FormClosing ( object sender, FormClosingEventArgs e )
            {
            try
                {
                SaveSettings ();
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void ShowHelpFile ()
            {
            try
                {
                System.Diagnostics.Process.Start ( (new Uri ( Path.Combine ( Path.GetDirectoryName ( GetType ().Assembly.Location ),
                     "Audio Format Converter-Help.htm" ) )).ToString () );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                return;
                }
            }

        private void m_btn_Help_Click ( object sender, EventArgs e )
            {
            ShowHelpFile (); 
            }

        
        private void m_audioFormatConverterForm_HelpRequested ( object sender, HelpEventArgs hlpevent )
            {
            ShowHelpFile ();
            }

        private void m_lb_addFiles_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            m_btnDelete.Enabled = m_lb_addFiles.Items.Count >= 1 && m_lb_addFiles.SelectedIndex >= 0;
        }
        
        }//m_audioFormatConverterForm Class
    }//AudioFormatConverterUI Namespace