using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.PipelineInterface.ParameterControls
{
    public partial class PathBrowserControl : BaseUserControl
    {
        private string m_SelectedPath;
        private string m_TextLabel;
        private ScriptParameter m_Parameter;
        private DataTypes.PathDataType m_PathData;
                private string m_ProjectDirectory;

        public PathBrowserControl()
        {
                        InitializeComponent();
        }

        public PathBrowserControl( ScriptParameter p , string SelectedPath ,string ProjectDirectory):this  ()
        {
                        m_SelectedPath = SelectedPath;
                        m_ProjectDirectory = ProjectDirectory;

                        m_Parameter = p;
                        m_PathData = (DataTypes.PathDataType)p.ParameterDataType;
                        label1.Text = p.NiceName;
                        textBox1.AccessibleName = p.NiceName;
                        base.Value = p.Description;

                        base.Size = this.Size;
        }

        public override string Text
        {
            get { return m_TextLabel; }
            set
            {
                if (value != null)
                {
                    label1.Text = value;
                    textBox1.AccessibleName = value;
                }
            }
        }



        public string SelectedPath
        {
            get
            {
                return m_SelectedPath;
            }
            set
            {
                if (value != null)
                {
                    textBox1.Text = value;

                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (m_PathData.m_FileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.File)
            {
                if (m_PathData.m_InputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.input)
                    UpdatePathTextboxFromOpenFileDialog();
                else if (m_PathData.m_InputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.output)
                    UpdatePathTextboxFromSaveFileDialog();
            }
            else if (m_PathData.m_FileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory)
            {
                UpdatePathTextboxFromFolderBrowserDialog();
            }
        }

        private void UpdatePathTextboxFromOpenFileDialog()
        {
            if (openFileDialog1.ShowDialog () == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void UpdatePathTextboxFromSaveFileDialog()
        {
            if (saveFileDialog1.ShowDialog () == DialogResult.OK)
            {
                textBox1.Text = saveFileDialog1.FileName;
            }
        }

        private void UpdatePathTextboxFromFolderBrowserDialog()
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            if (folderBrowserDialog1.ShowDialog () == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        public override void UpdateScriptParameterValue()
            	{
                    m_SelectedPath = textBox1.Text;
		 if (m_PathData.m_InputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.output )
                      {
                          try
                          {
                              if (m_PathData.m_FileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory) CheckForOutputDirectory();
                              else if (m_PathData.m_FileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.File) File.CreateText(textBox1.Text);
                          }
                          catch (System.Exception ex)
                          {
                              MessageBox.Show(ex.ToString());
                              return;
                          }
         }
            // update    parameter
            try
                {
                    m_PathData.Value = textBox1.Text;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
        
            	}
        

        
        /*
            m_SelectedPath = textBox1.Text;
                        // check for file   
            if (m_PathData.m_FileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.File)
            { //1
                                                if (m_PathData.m_InputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.input)
                { //2
                    if (!File.Exists(textBox1.Text))
                    { //3
                        MessageBox.Show("Not able to find source file.", "Error!");
                        return;
                    } //-3
                    MessageBox.Show("Input Exists!!!");
                } //-2
                else // file is output file
                { //2
                    if (!File.Exists(textBox1.Text))
                    { //3
                        File.Create(textBox1.Text);
                    } //-3
                    MessageBox.Show("Bypassed  create file!!!");
                } //-2
                //
                            } //-1
            // it is directory path
            else 
            { //1
                if (m_PathData.m_InputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.input)
                { //2
                    if (!Directory.Exists(textBox1.Text))
                    { //3
                        MessageBox.Show("Source Directory do not exist");
                        return;
                    } //-3
                    MessageBox.Show("source dir!!!");
                } //-2
                else // if it is output directory
                { //2
                    MessageBox.Show("Inside out directory!!!");
                    if( !CheckForOutputDirectory()) return ;
                } //-2
            }//-1
                MessageBox.Show(textBox1.Text);
                // if this point is reached after passing all tests, assign value to script parameter.
                try
                {
                    m_PathData.Value = textBox1.Text;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
        }
        */
         
        private bool  CheckForOutputDirectory()
        {
                MessageBox.Show(textBox1.Text + ":" + m_ProjectDirectory + ":" + m_SelectedPath);
                        if (Directory.Exists(textBox1.Text) &&
                            (textBox1.Text == m_ProjectDirectory || textBox1.Text == Directory.GetParent(m_SelectedPath ).FullName))
            {
                MessageBox.Show("Choose some other directory", "ERROR!");
                return false;
            }
            MessageBox.Show("2");
            if (!Directory.Exists(textBox1.Text))
            {
                DialogResult result = MessageBox.Show("Not able to find output directory. \n Do you want to create it?", "Error!", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (textBox1.Text == ""
                        || !Directory.CreateDirectory(textBox1.Text).Exists)
                    {
                        MessageBox.Show("Not able to create directory", "Error!");
                        return false;
                    }
                } // if no button is pressed
                else
                    return false;
            }

            if (Directory.GetFiles(textBox1.Text, "*.*", SearchOption.AllDirectories).Length > 0)
            {
                DialogResult result = MessageBox.Show("Output directory is not empty! Do you want to empty it?", "Warning", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Directory.Delete(textBox1.Text, true);
                    Directory.CreateDirectory(textBox1.Text);
                }
                else
                    return false;
            }
                        return  true;
        } // end of dir check function



    }
}
