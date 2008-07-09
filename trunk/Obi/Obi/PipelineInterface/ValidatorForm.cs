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
            AssociateParameterObjectToControls();
        }

        private void AssociateParameterObjectToControls()
        {
            foreach (ScriptParameter p in M_ValidatorScriptParser.ParameterList)
            {
                if (p.ParameterDiscriptiveName == "Input OPF")
                    m_txtDTBFilePath.Tag = p;
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

        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            if (!File.Exists(m_txtDTBFilePath.Text))
            {
                MessageBox.Show("Not able to find source DTB.", "Error!");
                return;
            }

            // execute script with input values
                        try
                        {
                            ((ScriptParameter) m_txtDTBFilePath.Tag).ParameterValue = m_txtDTBFilePath.Text;
                            M_ValidatorScriptParser.ExecuteScript();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
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