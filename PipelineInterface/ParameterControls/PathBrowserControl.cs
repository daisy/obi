using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PipelineInterface.ParameterControls
{
    public partial class PathBrowserControl : BaseUserControl
    {
        private string m_SelectedPath;
        private ScriptParameter m_Parameter;
        private DataTypes.PathDataType mPathData;
                private string m_ProjectDirectory;

        public PathBrowserControl() { InitializeComponent(); }

        public PathBrowserControl(ScriptParameter p, string SelectedPath, string ProjectDirectory,string ObiFont)
            : this()
        {
            m_SelectedPath = SelectedPath;
            m_ProjectDirectory = ProjectDirectory;
            m_Parameter = p;
            mPathData = (DataTypes.PathDataType)p.ParameterDataType;

            int wdiff = mNiceNameLabel.Width;
            mNiceNameLabel.Text = GetLocalizedString( p.NiceName);
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
            mTextBox.AccessibleName =GetLocalizedString ( p.Description);
            base.DescriptionLabel =GetLocalizedString( p.Description);
            if (mLabel.Width + mLabel.Margin.Horizontal > Width) Width = mLabel.Width + mLabel.Margin.Horizontal;
            if (mPathData.isInputOrOutput == PipelineInterface.DataTypes.PathDataType.InputOrOutput.input)
            {
                mTextBox.Text = SelectedPath;
            }
            else if (mPathData.IsFileOrDirectory == PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory)
            {
                mTextBox.Text = Path.IsPathRooted(p.ParameterValue) ? p.ParameterValue :
                    Path.GetFullPath(Path.Combine(ProjectDirectory, p.ParameterValue));
            }
            else
            {
                mTextBox.Text = p.ParameterValue;
            }
            if (ObiFont != this.Font.Name)
            {
                this.Font = new Font(ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
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
            if (mPathData.IsFileOrDirectory == PipelineInterface.DataTypes.PathDataType.FileOrDirectory.File)
            {
                if (mPathData.isInputOrOutput == PipelineInterface.DataTypes.PathDataType.InputOrOutput.input)
                {
                    UpdatePathTextboxFromOpenFileDialog();
                }
                else if (mPathData.isInputOrOutput == PipelineInterface.DataTypes.PathDataType.InputOrOutput.output)
                {
                    UpdatePathTextboxFromSaveFileDialog();
                }
            }
            else if (mPathData.IsFileOrDirectory == PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory)
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
                        if (mPathData.isInputOrOutput == PipelineInterface.DataTypes.PathDataType.InputOrOutput.output)
                        {
                            try
                            {
                                if (mPathData.IsFileOrDirectory ==
                                    PipelineInterface.DataTypes.PathDataType.FileOrDirectory.Directory)
                                {
                                     CheckProjectDirectory(mTextBox.Text, true);
                                }
                                else if (mPathData.IsFileOrDirectory ==
                                    PipelineInterface.DataTypes.PathDataType.FileOrDirectory.File)
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

        public static bool CheckProjectDirectory(string path, bool checkEmpty)
        {
            return Directory.Exists(path) ? CheckEmpty(path, checkEmpty) : DidCreateDirectory(path, true);
        }

        public static bool CheckEmpty(string path, bool checkEmpty)
        {
            if (checkEmpty &&
        (Directory.GetFiles(path).Length > 0 || Directory.GetDirectories(path).Length > 0))
            {
                DialogResult result = MessageBox.Show(
                    String.Format(Localizer.Message("really_use_directory_text"), path),
                    Localizer.Message("really_use_directory_caption"),
                    // MessageBoxButtons.YesNoCancel,
                   MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (Path.GetFullPath(path) != Path.GetPathRoot(path))
                        {
                            foreach (string f in Directory.GetFiles(path)) File.Delete(f);
                            foreach (string d in Directory.GetDirectories(path)) Directory.Delete(d, true);
                        }
                        else MessageBox.Show(Localizer.Message("CannotDeleteAtRoot"), Localizer.Message("Caption_Error"));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format(Localizer.Message("cannot_empty_directory"), path, e.Message),
                            Localizer.Message("cannot_empty_directory_caption"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                //  return result != DialogResult.Cancel;
                return true;
            }
            else
            {
                return true;  // the directory was empty or we didn't need to check
            }
        }

        private static bool DidCreateDirectory(string path, bool alwaysCreate)
        {
            if (alwaysCreate || MessageBox.Show(
                String.Format(Localizer.Message("create_directory_text"), path),
                Localizer.Message("create_directory_caption"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Directory.CreateDirectory(path);
                    return true;  // did create the directory
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        String.Format(Localizer.Message("cannot_create_directory_text"), path, e.Message),
                        Localizer.Message("cannot_create_directory_caption"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;  // couldn't create the directory
                }
            }
            else
            {
                return false;  // didn't want to create the directory
            }
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
