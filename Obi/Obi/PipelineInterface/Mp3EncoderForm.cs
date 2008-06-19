using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PipelineInterface
{
    public partial class Mp3EncoderForm : Form
    {
        private Mp3Encoder m_Encoder;

        public Mp3EncoderForm()
        {
            InitializeComponent();
                        m_Encoder = new Mp3Encoder();
        }

        public Mp3EncoderForm(string InputPath )
        {
            InitializeComponent();
            m_Encoder = new Mp3Encoder();
            InputPath = InputPath + "\\obi_dtb.opf";
            if ( InputPath != null  && File.Exists (InputPath) )
            m_txtInputFile.Text = InputPath;
        }

        private void Mp3EncoderForm_Load(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            //folderBrowserDialog1.SelectedPath = @"C:\Avi\Project\CurrentProjects\DaisyUra\TrialModules\PipelineCmd\Trial\Output";
            openFileDialog1.Filter = "DTB 3.0 Source files|*.opf|DTB2.02 Source Files|ncc.html";

                                    m_txtOutputDirectory.Text = folderBrowserDialog1.SelectedPath;
                        //m_txtInputFile.Text = m_Encoder.InputFilePath;
            //m_txtOutputDirectory.Text = m_Encoder.OutputDirectory;

            m_comboBitRate.Items.Add(32);
            m_comboBitRate.Items.Add(48);
            m_comboBitRate.Items.Add(64);
            m_comboBitRate.Items.Add(128);
            m_comboBitRate.SelectedIndex = 0;
        }

        private void m_btnBrowseInputFile_Click(object sender, EventArgs e)
        {
            DialogResult result =  openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_txtInputFile.Text = openFileDialog1.FileName;
            }
        }

        private void m_btnBrowseOutputDirectory_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_txtOutputDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void m_btnOK_Click(object sender, EventArgs e)
        {
                        if (!File.Exists(m_txtInputFile.Text ) )
                        {
                            MessageBox.Show ("Not able to find source DTB." , "Error!" ) ;
                            return;
                        }
                        if (!Directory.Exists(m_txtOutputDirectory.Text))
                        {
                            MessageBox.Show("Not able to find output directory", "Error!");
                            return;
                        }

                        if (Directory.GetFiles(m_txtOutputDirectory.Text, "*.*",    SearchOption.AllDirectories).Length > 0)
                        {
                            DialogResult result = MessageBox.Show("Output directory is not empty! Do you want to empty it?", "Warning" ,MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                                                                                                                Directory.Delete(m_txtOutputDirectory.Text, true);
                                Directory.CreateDirectory(m_txtOutputDirectory.Text);
                            }
                            else
                                return;
                        }

// write script file

                        try
                        {
                            m_Encoder.InputFilePath = m_txtInputFile.Text;
                            m_Encoder.OutputDirectory = m_txtOutputDirectory.Text;
                            m_Encoder.bitrate = m_comboBitRate.Items[m_comboBitRate.SelectedIndex].ToString();
                            
                            m_Encoder.ExecuteEncoder();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show ( ex.ToString ()) ;
                            return;
                        }
            Close();
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}