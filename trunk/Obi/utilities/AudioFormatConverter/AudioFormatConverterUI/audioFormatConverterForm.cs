using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AudioLib;

namespace AudioFormatConverterUI
    {

    public partial class m_audioFormatConverterForm : Form
        {
        
        public m_audioFormatConverterForm ()
         {
            InitializeComponent ();
            m_cb_channel.SelectedIndex = 0;
            m_cb_sampleRate.SelectedIndex = 0; 
         }

        private void m_btn_Add_Click(object sender, EventArgs e)
        {
            OpenFileDialog addFile = new OpenFileDialog();
            addFile.RestoreDirectory = true;
            addFile.Multiselect = true;
            //addFile.SafeFileNames = true;
            addFile.Filter = " MP3 Files(*.mp3)|*.mp3| Wave Files (*.wav)|*.wav| All Files (*.*)|*.*";
            
            if(addFile.ShowDialog(this) == DialogResult.OK)
            {
                if(addFile.FileNames.Length > 0)
                {
                    foreach (string audioFileName in addFile.FileNames)
                    {
                        string filename = Path.GetFullPath(audioFileName);
                        addFile.CheckFileExists = true;
                        addFile.CheckPathExists = true;
                        m_lb_addFiles.Items.Add(filename);  
                    }
                }
                
            }
        }

        private void m_btn_Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog  browserDialog = new FolderBrowserDialog();
            browserDialog.ShowNewFolderButton = true;
            browserDialog.SelectedPath = m_txt_Browse.Text;

            if(browserDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_txt_Browse.Text = browserDialog.SelectedPath;
                if (CheckIfDriveSelected()) { return; }
                if (!CheckOutDirExists()) { return; }
            }
        }
        private bool CheckOutDirExists()
        {
            bool flag = false;
            try
            {
                if (Directory.Exists(m_txt_Browse.Text))
                {
                    flag = true;
                    string[] fileEntries = Directory.GetFiles(m_txt_Browse.Text);
                    string[] subdirectoryEntries = Directory.GetDirectories(m_txt_Browse.Text);
                    if (fileEntries.Length != 0 || subdirectoryEntries.Length != 0)
                    {
                        if (MessageBox.Show("Directory" + " " + m_txt_Browse.Text + " " + "is not empty. If you want to empty it anyways press Yes if not then press No and then choose again", "Choose Directory", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Directory.Delete(m_txt_Browse.Text, true);
                        }
                        else
                            m_txt_Browse.Clear();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return flag;
        }//CheckOutDirExists 

        private bool CheckIfDriveSelected()
        {
            bool flag = false;
            string[] fixedDrives = Environment.GetLogicalDrives();
            foreach (string drive in fixedDrives)
            {
                if (m_txt_Browse.Text.Equals(drive, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(" Its a root directory , you cannot save here. Please select some other Directory. ", "Root Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_txt_Browse.Clear();
                    flag = true;
                }
            }
            return flag;
        }

        private void m_btn_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void m_btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                m_lb_addFiles.Items.Remove(m_lb_addFiles.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void m_btnReset_Click(object sender, EventArgs e)
        {
            m_lb_addFiles.Items.Clear();
            m_txt_Browse.Clear();
        }

        private void m_btn_Start_Click ( object sender, EventArgs e )
            {
            IWavFormatConverter audioConverter = new WavFormatConverter ( true);
            int samplingRate = int.Parse( m_cb_sampleRate.SelectedItem.ToString() );
            int channels = m_cb_channel.SelectedIndex + 1;
            int bitDepth = 16 ;
                        string outputDirectory = m_txt_Browse.Text;
                        string convertedFilePath= null;

                        if (!Directory.Exists ( outputDirectory )) return;

            int listPositionIndex = 0;

            while (m_lb_addFiles.Items.Count > listPositionIndex    &&    listPositionIndex < 50)
                {
                string filePath = (string) m_lb_addFiles.Items[listPositionIndex];
                //MessageBox.Show ( filePath );
                try
                    {
                    if (Path.GetExtension ( filePath ) == ".wav")
                        {
                        convertedFilePath = audioConverter.ConvertSampleRate ( filePath, outputDirectory, channels, samplingRate, bitDepth );
                        }
                    else if (Path.GetExtension ( filePath ) == ".mp3")
                        {
                        convertedFilePath=  audioConverter.UnCompressMp3File ( filePath, outputDirectory, channels,samplingRate, bitDepth) ;
                        }
                    // rename converted file to appropriate name
                    string newFilePath = Path.Combine ( outputDirectory,
                        Path.GetFileNameWithoutExtension( filePath ) ) + ".wav";
                    //MessageBox.Show ( newFilePath );
                    if (File.Exists ( newFilePath))
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

                    m_lb_addFiles.Items.RemoveAt (0) ;
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    listPositionIndex++;
                    }
                }
            }

    }//m_audioFormatConverterForm Class
    }//AudioFormatConverterUI Namespace