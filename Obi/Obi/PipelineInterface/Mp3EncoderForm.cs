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
                ScriptParser mParser;

        public Mp3EncoderForm()
        {
            InitializeComponent();
        }
        private string m_InputPath;
        private String m_ProjectDirectory;

        public Mp3EncoderForm(string scriptPath, string inputPath, string ProjectDirectory)
            : this()
        {
                                    if (!File.Exists(scriptPath)) throw new Exception(string.Format(Localizer.Message("no_script"), scriptPath));
            mParser = new ScriptParser(scriptPath);

            m_ProjectDirectory = ProjectDirectory;
            if (inputPath != null)
            {
                inputPath = Path.Combine(inputPath, "obi_dtb.opf");  // !!!
                                m_InputPath = inputPath;
            }
            
            
            if ( inputPath != null &&  File.Exists(inputPath))
            {
                m_txtInputFile.Text = inputPath;
                openFileDialog1.InitialDirectory = Directory.GetParent(inputPath).FullName;
                folderBrowserDialog1.SelectedPath = Directory.GetParent(inputPath).FullName;
            }
            else
            {
                openFileDialog1.InitialDirectory = ProjectDirectory;
                folderBrowserDialog1.SelectedPath = ProjectDirectory;
            }
            AssociateParameterObjectToControls(mParser);
        }

        /// <summary>
        /// <summary></summary>
        ///  Associate script parameter objects to their respective controls
        /// </summary>
        private void AssociateParameterObjectToControls(ScriptParser script)
        {
            foreach (ScriptParameter p in script.ParameterList)
            {
                if (p.NiceName== "Input OPF")
                    m_txtInputFile.Tag = p;
                else if (p.NiceName== "Output directory")
                    m_txtOutputDirectory.Tag = p;
                else if (p.NiceName== "Bitrate")
                    m_comboBitRate.Tag = p;
            }
        }

        private void Mp3EncoderForm_Load(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            openFileDialog1.Filter = Localizer.Message("dtb_filter");
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
            if (!File.Exists(m_txtInputFile.Text))
            {
                MessageBox.Show("Not able to find source DTB.", "Error!");
                return;
            }
            else
                m_InputPath = m_txtInputFile.Text;
                        if (Directory.Exists(m_txtOutputDirectory.Text) &&
                            (m_txtOutputDirectory.Text == m_ProjectDirectory || m_txtOutputDirectory.Text == Directory.GetParent(m_InputPath).FullName))
                        {
                            MessageBox.Show("Choose some other directory", "ERROR!");
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

                          mParser.ExecuteScript();
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