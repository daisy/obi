using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.PipelineInterface
{
    public partial class Mp3EncoderForm : Form
    {
        
        private ScriptParser M_ScriptParser; //instance of ScriptParser class
        private string m_ScriptFilePath; // Path of DTBToAudioEncoder script file


        public Mp3EncoderForm()
        {
            InitializeComponent();
                                }

        public Mp3EncoderForm(string InputPath, string ProjectDirectory)
            : this()
        {
                        //string relativeScriptPath = "\\PipelineCmd\\scripts\\manipulation\\simple\\DTBAudioEncoder.taskScript";
            string relativeScriptPath = "\\PipelineLight\\scripts\\DTBAudioEncoder.taskScript";
            m_ScriptFilePath = AppDomain.CurrentDomain.BaseDirectory + relativeScriptPath;
            if (!File.Exists(m_ScriptFilePath))
            {
                MessageBox.Show("Pipeline script file not found");
                return ;
            }

            M_ScriptParser = new ScriptParser(m_ScriptFilePath);

            InputPath = InputPath + "\\obi_dtb.opf";
            if (InputPath != null && File.Exists(InputPath))
            {
                m_txtInputFile.Text = InputPath;
                openFileDialog1.InitialDirectory = Directory.GetParent(InputPath).FullName;
                folderBrowserDialog1.SelectedPath = Directory.GetParent(InputPath).FullName; 
            }
            else
            {
                openFileDialog1.InitialDirectory = ProjectDirectory;
                folderBrowserDialog1.SelectedPath = ProjectDirectory;
            }
        

// associate script parameter objects
        AssociateParameterObjectToControls();
        }

        /// <summary>
        /// <summary></summary>
        ///  Associate script parameter objects to their respective controls
        /// </summary>
        private void AssociateParameterObjectToControls()
        {
            foreach (ScriptParameter p in M_ScriptParser.ParameterList)
            {
                if (p.ParameterDiscriptiveName == "Input OPF")
                    m_txtInputFile.Tag = p;
                else if (p.ParameterDiscriptiveName == "Output directory")
                    m_txtOutputDirectory.Tag = p;
                else if (p.ParameterDiscriptiveName == "Bitrate")
                    m_comboBitRate.Tag = p;
            }
        }

        private void Mp3EncoderForm_Load(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            //folderBrowserDialog1.SelectedPath = @"C:\Avi\Project\CurrentProjects\DaisyUra\TrialModules\PipelineCmd\Trial\Output";
            openFileDialog1.Filter = "DTB 3.0 Source files|*.opf|DTB2.02 Source Files|ncc.html";

                                    //m_txtOutputDirectory.Text = folderBrowserDialog1.SelectedPath;
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
                            DialogResult result =  MessageBox.Show("Not able to find output directory. \n Do you want to create it?", "Error!",MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                if (m_txtOutputDirectory.Text == "" 
                                    || !Directory.CreateDirectory (m_txtOutputDirectory.Text).Exists)
                                {
                                    MessageBox.Show("Not able to create directory", "Error!");
                                    return;
                                }
                            } // if no button is pressed
                            else
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

// execute script with input values
                        try
                        {
                            ((ScriptParameter) m_txtInputFile.Tag).ParameterValue = m_txtInputFile.Text;
                            ((ScriptParameter)m_txtOutputDirectory.Tag).ParameterValue  = m_txtOutputDirectory.Text;
                            ((ScriptParameter)m_comboBitRate.Tag).ParameterValue = m_comboBitRate.Items[m_comboBitRate.SelectedIndex].ToString();

                          M_ScriptParser.ExecuteScript();
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