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
    public partial class ValidatorForm : Form
    {

        private ScriptParser M_ValidatorScriptParser; //instance of ScriptParser class
        private string m_ScriptFilePath; // Path to validator script file

        public ValidatorForm()
        {
            InitializeComponent();
                    }

        public ValidatorForm(string InputPath, string ProjectDirectory)
            : this()
        {
            string relativeScriptPath = "\\PipelineLight\\scripts\\Z3986DTBValidator.taskScript";
            m_ScriptFilePath = AppDomain.CurrentDomain.BaseDirectory + relativeScriptPath;
            if (!File.Exists(m_ScriptFilePath))
            {
                MessageBox.Show("Pipeline script file not found");
                return;
            }

            M_ValidatorScriptParser = new ScriptParser(m_ScriptFilePath);

            if (InputPath != "" && File.Exists(InputPath))
            {
                m_txtDTBFilePath.Text = InputPath;
                openFileDialog1.InitialDirectory = Directory.GetParent(InputPath).FullName;
            }
            else
            {
                openFileDialog1.InitialDirectory = ProjectDirectory;
                saveFileDialog1.InitialDirectory = ProjectDirectory;
            }

            openFileDialog1.Filter = "DTB 3.0 Source files|*.opf|DTB2.02 Source Files|ncc.html";
            saveFileDialog1.Filter = "xml file|*.xml ";
            saveFileDialog1.DefaultExt = ".xml";
            saveFileDialog1.AddExtension = true;
            m_txtErrorFilePath.Text = ProjectDirectory + "\\Validator error report.xml";

            AssociateParameterObjectToControls();
        }

        private void AssociateParameterObjectToControls()
        {
            foreach (ScriptParameter p in M_ValidatorScriptParser.ParameterList)
            {
                if (p.ParameterDiscriptiveName == "Input OPF")
                    m_txtDTBFilePath.Tag = p;
                else if (p.ParameterDiscriptiveName == "Validation Report")
                    m_txtErrorFilePath.Tag = p;
                            }
        }


        private void m_btnBrowseDTBFilePath_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_txtDTBFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void m_chkReportToFile_CheckedChanged(object sender, EventArgs e)
        {
            if (m_chkReportToFile.Checked)
            {
                m_lblErrorReportFilePath.Enabled = true;
                m_txtErrorFilePath.Enabled = true;
                m_btnBrowseErrorFile.Enabled = true;
                            }
            else
            {
                m_lblErrorReportFilePath.Enabled = false;
                m_txtErrorFilePath.Enabled = false;
                m_btnBrowseErrorFile.Enabled = false;
            }

        }

        private void m_btnBrowseErrorFile_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_txtErrorFilePath.Text = saveFileDialog1.FileName;
            }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            if (!File.Exists(m_txtDTBFilePath.Text))
            {
                MessageBox.Show("Not able to find source DTB.", "Error!");
                return;
            }

            if (m_chkReportToFile.Checked)
            {
                if (!File.Exists(m_txtErrorFilePath.Text))
                {
                    if (!CreateErrorReportFile(m_txtErrorFilePath.Text))
                    {
                        MessageBox.Show("Unable to create validator error report file.", "Error!");
                        return ;
                    }
                }
            }
            

            // execute script with input values
                        try
                        {
                            ((ScriptParameter) m_txtDTBFilePath.Tag).ParameterValue = m_txtDTBFilePath.Text;
                            ((ScriptParameter)m_txtErrorFilePath.Tag).ParameterValue = m_txtErrorFilePath.Text;

                            M_ValidatorScriptParser.ExecuteScript();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            return;
                        }

            Close();
        }

        private bool CreateErrorReportFile(string path)
        {
            try
            {
                File.CreateText(path);
            }
            catch ( System.Exception ex )
            {
                MessageBox.Show ( ex.ToString () ) ;
                return false ;
            }
            return true;
            
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}