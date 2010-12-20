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
        private ScriptParameter m_Parameter;
        private DataTypes.PathDataType mPathData;
                private string m_ProjectDirectory;

        public PathBrowserControl() { InitializeComponent(); }

        public PathBrowserControl(ScriptParameter p, string SelectedPath, string ProjectDirectory)
            : this()
        {
            m_SelectedPath = SelectedPath;
            m_ProjectDirectory = ProjectDirectory;
            m_Parameter = p;
            mPathData = (DataTypes.PathDataType)p.ParameterDataType;

            int wdiff = mNiceNameLabel.Width;
            mNiceNameLabel.Text = p.NiceName;
            wdiff -= mNiceNameLabel.Width;
            if (wdiff < 0)
            {
                Point location = mNiceNameLabel.Location;
                Width -= wdiff;
                mNiceNameLabel.Location = location;
            }
            else
            {
                mNiceNameLabel.Location = new Point(mNiceNameLabel.Location.X - wdiff, mNiceNameLabel.Location.Y);
            }
            mTextBox.AccessibleName = p.Description;
            base.DescriptionLabel = p.Description;
            if (mLabel.Width + mLabel.Margin.Horizontal > Width) Width = mLabel.Width + mLabel.Margin.Horizontal;
            if (mPathData.isInputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.input)
            {
                mTextBox.Text = SelectedPath;
            }
            else if (mPathData.IsFileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory)
            {
                mTextBox.Text = Path.IsPathRooted(p.ParameterValue) ? p.ParameterValue :
                    Path.GetFullPath(Path.Combine(ProjectDirectory, p.ParameterValue));
            }
            else
            {
                mTextBox.Text = p.ParameterValue;
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
                    mTextBox.Text = value;

                }
            }
        }

        private void mBrowseButton_Click(object sender, EventArgs e)
        {
            if (mPathData.IsFileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.File)
            {
                if (mPathData.isInputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.input)
                {
                    UpdatePathTextboxFromOpenFileDialog();
                }
                else if (mPathData.isInputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.output)
                {
                    UpdatePathTextboxFromSaveFileDialog();
                }
            }
            else if (mPathData.IsFileOrDirectory == Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory)
            {
                UpdatePathTextboxFromFolderBrowserDialog();
            }
        }

        // Bring up the file browser to choose an input file
        private void UpdatePathTextboxFromOpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = mTextBox.Text;
            dialog.CheckPathExists = true;
            dialog.CheckFileExists = false;
            if (dialog.ShowDialog() == DialogResult.OK) mTextBox.Text = dialog.FileName;
        }

        // Bring up the save file dialog to choose an output file
        private void UpdatePathTextboxFromSaveFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = mTextBox.Text;
            dialog.AddExtension = true; dialog.DefaultExt = "xml";
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == DialogResult.OK) mTextBox.Text = dialog.FileName;
        }

        private void UpdatePathTextboxFromFolderBrowserDialog()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            //dialog.SelectedPath = mTextBox.Text;
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == DialogResult.OK) mTextBox.Text = dialog.SelectedPath;
        }

        public override void UpdateScriptParameterValue()
            	{
                    if ((mTextBox.Text != null && mTextBox.Text != ""))
                        //||   m_Parameter.IsParameterRequired)
                    {
                        m_SelectedPath = mTextBox.Text;
                        if (mPathData.isInputOrOutput == Obi.PipelineInterface.DataTypes.PathDataType.InputOrOutput.output)
                        {
                            try
                            {
                                if (mPathData.IsFileOrDirectory ==
                                    Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory)
                                {
                                    ObiForm.CheckProjectDirectory(mTextBox.Text, true);
                                }
                                else if (mPathData.IsFileOrDirectory ==
                                    Obi.PipelineInterface.DataTypes.PathDataType.FileOrDirectory.File)
                                {
                                    File.CreateText(mTextBox.Text).Close();
                                }
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
                            mPathData.Value = mTextBox.Text;
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    } // null value check ends
            	}
        
 
        private bool  CheckForOutputDirectory()
        {
                                        if ( Directory.Exists(mTextBox.Text) &&
                            (mTextBox.Text == m_ProjectDirectory || mTextBox.Text == Directory.GetParent(m_SelectedPath ).FullName))
            {
                MessageBox.Show(Localizer.Message("Choose_OtherDirectory"), Localizer.Message("Caption_Error"));
                return false;
            }
                        if (!Directory.Exists(mTextBox.Text) )
            {
                DialogResult result = MessageBox.Show(Localizer.Message("OutputDirectoryNotFound_Permission_Create"), Localizer.Message("Caption_Error"), MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (mTextBox.Text == ""
                        || !Directory.CreateDirectory(mTextBox.Text).Exists)
                    {
                                                MessageBox.Show(string.Format(Localizer.Message("NotAbleToCreateDirectory"), mTextBox.Text), Localizer.Message("Caption_Error"));
                        return false;
                    }
                } // if no button is pressed
                else
                    return false;
            }

            if (Directory.GetFiles(mTextBox.Text, "*.*", SearchOption.AllDirectories).Length > 0)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("Permission_EmptyOutputDirectory") , Localizer.Message("Caption_Warning") , MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Directory.Delete(mTextBox.Text, true);
                    Directory.CreateDirectory(mTextBox.Text);
                }
                else
                    return false;
            }
                        return  true;
        } // end of dir check function



    }
}
