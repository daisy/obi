using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace MergeUtilityUI
{

    public partial class Daisy3MergerForm: Form
    {
        private ProgressDialogDTB progress = null;

        public Daisy3MergerForm  ()
        {
            InitializeComponent();
        }

        private void m_btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog select_opfFile = new OpenFileDialog();
            select_opfFile.Filter = "OPF Files (*.opf)|*.opf";
            select_opfFile.FilterIndex = 1;
            select_opfFile.RestoreDirectory = true;
            if (select_opfFile.ShowDialog(this) == DialogResult.OK)
            {
                m_lbOPFfiles.Items.Add(select_opfFile.FileName);
            }
             m_BtnMerge.Enabled = m_lbOPFfiles.Items.Count >= 2;             
             m_BtnMerge.Enabled  = true;
             m_BtnReset.Enabled = true;

            if (m_lbOPFfiles.Items.Count == 1)
            {
                m_StatusLabel.Text = "  Please select at least two OPF Files for merging ";
            }
            m_StatusLabel.Text = " The OPF Files selected has successfully been added in the Listbox ";
         }//m_btnAdd_Click

        private void m_BtnOutputDirectory_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = "Select the  output Directory where you want to put the merged OPF files";
            FolderBrowserDialog saveDir = new FolderBrowserDialog();
            saveDir.ShowNewFolderButton = true;
            saveDir.SelectedPath = m_txtDirectoryPath.Text;

            if (saveDir.ShowDialog(this) == DialogResult.OK)
            {
                m_txtDirectoryPath.Text = saveDir.SelectedPath;
                checkOutDirExists(m_txtDirectoryPath.Text);
            }
            if (m_txtDirectoryPath.Text.Length > 0)
            {
                m_StatusLabel.Text = " You have selected the path " + saveDir.SelectedPath + " to save the merged OPF files ";
            }
        }//m_BtnOutputDirectory_Click

        private void checkOutDirExists(string outPath)
        {
            if (Directory.Exists(m_txtDirectoryPath.Text))
            {
                string[] fileEntries = Directory.GetFiles(m_txtDirectoryPath.Text);
                string[] subdirectoryEntries = Directory.GetDirectories(m_txtDirectoryPath.Text);
                if (fileEntries.Length != 0 || subdirectoryEntries.Length != 0)
                {
                    if (MessageBox.Show("Directory" + " " + m_txtDirectoryPath.Text + " " + "is not empty. If you want to empty it anyways press Yes if not then press No and then choose again", "Choose Directory", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        Directory.Delete(m_txtDirectoryPath.Text, true);
                    else
                    {
                        m_txtDirectoryPath.Clear();
                        FolderBrowserDialog saveDire = new FolderBrowserDialog();
                        saveDire.ShowNewFolderButton = true;
                        saveDire.SelectedPath = m_txtDirectoryPath.Text;

                        if (saveDire.ShowDialog(this) == DialogResult.OK)
                        {
                            m_txtDirectoryPath.Text = saveDire.SelectedPath;
                            checkOutDirExists(m_txtDirectoryPath.Text);

                        }
                    }
                }
            }
        }         

        private void m_BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                m_StatusLabel.Text = " Deleting the selected file from the Listbox ";
                m_lbOPFfiles.Items.Remove(m_lbOPFfiles.SelectedItem);
                m_txtDTBookInfo.Clear();
                m_StatusLabel.Text = string.Empty;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }//m_BtnDelete_Click

        private void m_BtnReset_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = " Clearing all the Text ";
            m_txtDirectoryPath.Clear();
            m_lbOPFfiles.Items.Clear();
            m_txtDTBookInfo.Clear();
            m_BtnMerge.Enabled = false;
            m_BtnValidateInput.Enabled = false;
            m_BtnValidateOutput.Enabled = false;
            m_BtnValidateInput.Enabled = false;
            m_btnUP.Enabled = false;
            m_BtnDown.Enabled = false;
        }//m_BtnReset_Click

        private void m_btnUP_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = " You have selected " + m_lbOPFfiles.SelectedItem.ToString() + " to move up.";
            try
            {
                if (m_lbOPFfiles.Items.Count != 0)
                {
                    if (m_lbOPFfiles.SelectedIndex != 0 && m_lbOPFfiles.SelectedIndex != -1)
                    {
                        object item = m_lbOPFfiles.SelectedItem;
                        int index = m_lbOPFfiles.SelectedIndex;
                        m_lbOPFfiles.Items.RemoveAt(index);
                        m_lbOPFfiles.Items.Insert(index - 1, item);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }//m_btnUP_Click
        
        private void m_BtnDown_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = " You have selected " + m_lbOPFfiles.SelectedItem.ToString() + " to move down.";
            try
            {
                int index = m_lbOPFfiles.SelectedIndex;

                if (m_lbOPFfiles.Items.Count != 0)
                {
                    if (m_lbOPFfiles.SelectedIndex != m_lbOPFfiles.Items.Count - 1 && m_lbOPFfiles.SelectedIndex != -1)
                    {
                        object item = m_lbOPFfiles.SelectedItem;
                        m_lbOPFfiles.Items.RemoveAt(index);
                        m_lbOPFfiles.Items.Insert(index + 1, item);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }//m_BtnDown_Click

        private void m_BtnMerge_Click(object sender, EventArgs e)
        {
            try
            {
                m_StatusLabel.Text = "You have selected all the files from the listbox for merging. ";
                if (m_txtDirectoryPath.Text == "")
                {
                    MessageBox.Show("Output Directory Path cannot be empty, Please select the output Directory Path",
                                    "Select Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_txtDirectoryPath.Focus();
                }
                if (m_txtDirectoryPath.Text.Length > 0)
                {
                    m_bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_bgWorker_DoWork);
                    m_bgWorker.RunWorkerCompleted +=
                        new System.ComponentModel.RunWorkerCompletedEventHandler(m_bgWorker_RunWorkerCompleted);


                    m_bgWorker.RunWorkerAsync();

                    if (m_bgWorker.IsBusy)
                    {
                        progress = new ProgressDialogDTB();
                        progress.ShowDialog();
                    }

                    while (m_bgWorker.IsBusy)
                    {
                        Application.DoEvents();
                    }
                }
            }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.ToString());
             }
        }//m_BtnMerge_Click
                
        private void m_bgWorker_DoWork(object sender, EventArgs e)
        {
            string[] listOfOpfFiles = new string[m_lbOPFfiles.Items.Count];
            for (int i = 0; i < m_lbOPFfiles.Items.Count; i++)
            {
                listOfOpfFiles[i] = m_lbOPFfiles.Items[i].ToString();
            }

            DTBMerger.DTBMerger obj = new DTBMerger.DTBMerger(listOfOpfFiles, m_txtDirectoryPath.Text, DTBMerger.PageMergeOptions.KeepExisting);
            obj.MergeDTDs();
        }//m_bgWorker_DoWork

        private void m_bgWorker_RunWorkerCompleted(object sender, AsyncCompletedEventArgs e)
        {            
            if (progress != null)
            {
                progress.Close();
                progress = null;
            }
            if (e.Cancelled)
            {
                m_StatusLabel.Text = " Progress was cancelled ";
            }
            if (e.Error == null)
            {
                m_StatusLabel.Text = "The Files has been merged and put in Selected output directory.";
                m_BtnMerge.Enabled = false;
                m_BtnValidateOutput.Enabled = true;
            }
            if (e.Error != null)
            {
                m_StatusLabel.Text = "Failed in merging the files";
            }            
        }//m_bgWorker_RunWorkerCompleted

        private void m_BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_lbOPFfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
           m_BtnValidateInput.Enabled = m_lbOPFfiles.Items.Count > 0 && m_lbOPFfiles.SelectedIndex >= 0;
           m_BtnDelete.Enabled = m_lbOPFfiles.Items.Count > 0 && m_lbOPFfiles.SelectedIndex >= 0;
            
            if (m_lbOPFfiles.Items.Count == 1)
            {
                m_StatusLabel.Text = "  Please select at least two OPF Files for merging ";
            }
            this.m_btnUP.Enabled = this.m_lbOPFfiles.SelectedIndex > 0;
            this.m_BtnDown.Enabled = this.m_lbOPFfiles.SelectedIndex > -1 && m_lbOPFfiles.SelectedIndex < m_lbOPFfiles.Items.Count - 1;

            if (m_lbOPFfiles.SelectedIndex >= 0)
            {
                m_txtDTBookInfo.Clear();
                DTBMerger.DTBFilesInfo fileInfo = new DTBMerger.DTBFilesInfo(m_lbOPFfiles.SelectedItem.ToString());
                m_txtDTBookInfo.Multiline = true;
                m_txtDTBookInfo.ReadOnly = true;
                m_txtDTBookInfo.Text = 
                    "Title: " + fileInfo.title + Environment.NewLine + 
                    "Book ID: " +  fileInfo.ID + Environment.NewLine + 
                    "Total duration: " +  fileInfo.time;
            }
        }//m_lbOPFfiles_SelectedIndexChanged

        private void m_BtnValidateInput_Click(object sender, EventArgs e)
        {
            if (m_lbOPFfiles.SelectedItems.Count != 0)
            {
                m_StatusLabel.Text = " Validating The Input OPF File..";
                DTBMerger.PipelineInterface.ScriptsFunctions obj = new DTBMerger.PipelineInterface.ScriptsFunctions();
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pipeline-lite");
                string completeScriptPath = Path.Combine(scriptPath, "scripts");
                obj.Validate(Path.Combine(completeScriptPath, "Z3986DTBValidator.taskScript"),
                             m_lbOPFfiles.SelectedItem.ToString(), "", 30);
                m_StatusLabel.Text = "";
            }
        }//m_BtnValidateInput_Click

        private void m_BtnValidateOutput_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = " Validating the Output Directory merged OPF files..... ";
            DTBMerger.PipelineInterface.ScriptsFunctions obj = new DTBMerger.PipelineInterface.ScriptsFunctions();
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pipeline-lite");
            string completeScriptPath = Path.Combine(scriptPath, "scripts");
            DirectoryInfo dir = new DirectoryInfo(m_txtDirectoryPath.Text);
            FileInfo[] opfFiles = dir.GetFiles("*.opf ", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo in opfFiles)
            {
                obj.Validate(Path.Combine(completeScriptPath, "Z3986DTBValidator.taskScript"), fileInfo.FullName, "", 30);
            }
            m_StatusLabel.Text = string.Empty;
        }//m_BtnValidateOutput_Click

    }//class
}//namespace