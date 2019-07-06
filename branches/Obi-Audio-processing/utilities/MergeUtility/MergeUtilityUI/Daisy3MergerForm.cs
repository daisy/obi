using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace MergeUtilityUI
    {

    public partial class Daisy3MergerForm : Form
        {
        private ProgressDialogDTB progress = null;
        private bool daisy3Option = false;
        private bool daisy202option = false;
        private DTBMerger.Merger m_Merger = null;
        private string m_PipelineLiteDir;
        private int m_ValidatorTimeTolerance = 50;
        private bool m_CanRemoveDuplicatePagesInDAISY3;


        public Daisy3MergerForm ()
            {
            InitializeComponent ();
            m_bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler ( m_bgWorker_DoWork );
            m_bgWorker.RunWorkerCompleted +=
                new System.ComponentModel.RunWorkerCompletedEventHandler ( m_bgWorker_RunWorkerCompleted );
            m_bgWorker.WorkerSupportsCancellation = true;
            
            m_Merger = null;

            try
                {
                LoadSettings ();
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void LoadSettings ()
            {
            string settingsFilePath = Path.Combine (
                System.AppDomain.CurrentDomain.BaseDirectory,
                "DTBMerger_settings.xml" );
            XmlDocument settingsDocument = DTBMerger.CommonFunctions.CreateXmlDocument ( settingsFilePath );
            string pipelinePath = settingsDocument.GetElementsByTagName ( "pipelineLitePath" )[0].InnerText;
            if (!pipelinePath.EndsWith ( "\\" )) pipelinePath = pipelinePath + "\\";

            if (Path.IsPathRooted ( pipelinePath ))
                {
                m_PipelineLiteDir = pipelinePath;
                }
            else
                {
                m_PipelineLiteDir = Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, pipelinePath );
                }
            if (!Directory.Exists ( m_PipelineLiteDir ))
                {
                MessageBox.Show ( "Pipeline-lite not found at: " + m_PipelineLiteDir, "WARNING" );
                }

            string strTimeTolerance = settingsDocument.GetElementsByTagName ( "validatorTimeTolerance" )[0].InnerText;
            int timeTolerance = 0;
            int.TryParse ( strTimeTolerance, out timeTolerance );
            if (timeTolerance > 0) m_ValidatorTimeTolerance = timeTolerance;
            
            // remove duplicate page in DAISY 3 flag
            string strRemoveDuplicatePages = settingsDocument.GetElementsByTagName ( "removeDuplicatePagesD3" )[0].InnerText ;
            m_CanRemoveDuplicatePagesInDAISY3 = strRemoveDuplicatePages == "false" ? false : true;
            }

        private void m_btnAdd_Click ( object sender, EventArgs e )
            {

            OpenFileDialog select_File = new OpenFileDialog ();

            if (daisy3Option == true)
                {
                select_File.Filter = "OPF Files (*.opf)|*.opf";
                }
            else if (daisy202option == true)
                {
                select_File.Filter = "HTML Files (*.html)|*.html| HTM Files (*.htm)|*.htm";
                }

            select_File.RestoreDirectory = true;
            if (select_File.ShowDialog ( this ) == DialogResult.OK)
                {
                m_lbDTBfiles.Items.Add ( select_File.FileName );
                if (daisy3Option == true)
                    m_StatusLabel.Text = " The OPF Files selected has successfully been added in the Listbox ";
                if (daisy202option == true)
                    m_StatusLabel.Text = " The NCC Files selected has successfully been added in the Listbox ";
                }
            m_BtnMerge.Enabled = m_lbDTBfiles.Items.Count >= 2;
            m_BtnReset.Enabled = m_lbDTBfiles.Items.Count >= 1;

            if (m_lbDTBfiles.Items.Count == 1)
                {
                if (daisy3Option == true)
                    m_StatusLabel.Text = "  Please select at least two OPF Files for merging ";
                if (daisy202option == true)
                    m_StatusLabel.Text = "  Please select at least two NCC Files for merging ";
                }
           }//m_btnAdd_Click

        private void m_BtnOutputDirectory_Click ( object sender, EventArgs e )
            {
            m_StatusLabel.Text = "Select the  output Directory where you want to place the merged files";
            m_FolderBrowserDialog.ShowNewFolderButton = true;
            m_FolderBrowserDialog.SelectedPath = m_txtDirectoryPath.Text;
            if (m_FolderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    m_txtDirectoryPath.Text = m_FolderBrowserDialog.SelectedPath;
                    if (checkIfDriveSelected()) { return; }
                    if (!checkOutDirExists(m_txtDirectoryPath.Text)) { return; }
                }
            if (m_txtDirectoryPath.Text.Length > 0)
                {
                m_BtnReset.Enabled = true;
                m_StatusLabel.Text = " You have selected the path " + m_FolderBrowserDialog.SelectedPath + " to save the merged files ";
                }
            }//m_BtnOutputDirectory_Click

            private bool checkOutDirExists(string outPath)
            {
                bool flag = false;
                try
                {
                    if (Directory.Exists(m_txtDirectoryPath.Text))
                    {
                        flag = true;
                        string[] fileEntries = Directory.GetFiles(m_txtDirectoryPath.Text);
                        string[] subdirectoryEntries = Directory.GetDirectories(m_txtDirectoryPath.Text);
                        if (fileEntries.Length != 0 || subdirectoryEntries.Length != 0)
                        {
                            if (MessageBox.Show("Directory" + " " + m_txtDirectoryPath.Text + " " + "is not empty. If you want to empty it anyways press Yes if not then press No and then choose again", "Choose Directory", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Directory.Delete(m_txtDirectoryPath.Text, true);
                            }
                            else
                                m_txtDirectoryPath.Clear();
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return flag;
            }//checkOutDirExists 

        private bool checkIfDriveSelected()
        {
            bool flag = false;
            string[] fixedDrives = Environment.GetLogicalDrives();
            foreach (string drive in fixedDrives)
            {
                if (m_txtDirectoryPath.Text.Equals(drive, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(" Its a root directory , you cannot save here. Please select some other Directory. ", "Root Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_txtDirectoryPath.Clear();
                    flag = true;
                }
            }
            return flag;
        }


        private void m_BtnMerge_Click ( object sender, EventArgs e )
            {
            try
                {
                if (m_lbDTBfiles.Items.Count >= 2)
                    {
                    m_StatusLabel.Text = "Merging all DTBs in listbox. ";
                    if (m_txtDirectoryPath.Text == "")
                        {
                        MessageBox.Show ( "Output Directory Path cannot be empty, Please select the output Directory Path",
                                        "Select Directory", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        m_BtnOutputDirectory.Focus ();
                        m_StatusLabel.Text = "Click Browse button to select the Directory to save the merged files..";
                        return;
                        }
                    if (m_txtDirectoryPath.Text.Length > 0)
                        {
                        if (!m_bgWorker.IsBusy)
                            {
                            m_bgWorker.RunWorkerAsync ();
                            }
                        else
                            {
                            MessageBox.Show ( " Please be patient, earlier task is in progress " );
                            return;
                            }
                        if (m_bgWorker.IsBusy)
                            {
                            progress = new ProgressDialogDTB ();
                            progress.FormClosing += new FormClosingEventHandler ( ProgressDialog_FormClosing );
                            progress.ShowDialog ( this );
                            }

                        while (m_bgWorker.IsBusy)
                            {
                            Application.DoEvents ();
                            }
                        }
                    }
                else
                    {
                    MessageBox.Show ( "Either there are no files or only one file in the Listbox to merge. Minimum 2 files are needed for merging. Please add some files in Listbox.Click Add button.", "Listbox Empty", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                    m_btnAdd.Focus ();
                    }

                }
            catch (Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }//m_BtnMerge_Click

        private void m_bgWorker_DoWork ( object sender, DoWorkEventArgs e )
            {
            try
                {
                StartMerging ();
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                m_Merger.RequestCancel ();
                }
            }//m_bgWorker_DoWork

        private void StartMerging ()
            {
            m_Merger = null;
            string[] listOfDTBFiles = new string[m_lbDTBfiles.Items.Count];
            for (int i = 0; i < m_lbDTBfiles.Items.Count; i++)
                {
                listOfDTBFiles[i] = m_lbDTBfiles.Items[i].ToString ();
                }

            // create temp directory
            string outputDirTemp = Path.Combine ( m_txtDirectoryPath.Text, "temp" );
            if (Directory.Exists ( outputDirTemp ))
                {
                Directory.Delete ( outputDirTemp, true );
                }
            Directory.CreateDirectory ( outputDirTemp );

            if (m_rdbExistingNumberOfPages.Checked)
                {
                m_Merger = new DTBMerger.Merger ( listOfDTBFiles, outputDirTemp, DTBMerger.PageMergeOptions.KeepExisting );
                }
            else if (m_rdbRenumberPages.Checked)
                {
                m_Merger = new DTBMerger.Merger ( listOfDTBFiles, outputDirTemp, DTBMerger.PageMergeOptions.Renumber );
                }
            
            if (daisy3Option == true)
                {
                // assign can remove duplicate flag, it is added for precaution due to last minute changes
                m_Merger.CanRemoveDuplicatePagesInDAISY3 = m_CanRemoveDuplicatePagesInDAISY3;
                m_Merger.MergeDTDs ();
                }
            else if (daisy202option == true)
                {
                m_Merger.MergeDAISY2DTDs ();
                }

            // apply pretty printer script and remove temp directory
            string prettyPrinterInputFileName = Path.GetFileName ( listOfDTBFiles[0] );
            string dtbPath = Path.Combine ( outputDirTemp, prettyPrinterInputFileName );
            string prettyPrinterPath = Path.Combine ( m_PipelineLiteDir, "scripts\\PrettyPrinter.taskScript-hidden" );
            if (File.Exists ( prettyPrinterPath ))
                {
                DTBMerger.PipelineInterface.ScriptsFunctions.PrettyPrinter ( prettyPrinterPath,
                    dtbPath,
                    m_txtDirectoryPath.Text );

                // check if pretty printer has worked well by checking if ncc.html or .opf files are at output
                string[] filesArray = Directory.GetFiles ( m_txtDirectoryPath.Text,
                    prettyPrinterInputFileName,
                    SearchOption.TopDirectoryOnly );

                if (filesArray != null && filesArray.Length > 0)
                    {
                    Directory.Delete ( outputDirTemp, true );
                    }
                }

            }//StartMerging()

        private void ProgressDialog_FormClosing ( object sender, EventArgs e )
            {
            if (progress != null && m_bgWorker.IsBusy && !m_bgWorker.CancellationPending)
                {
                try
                    {
                    m_Merger.RequestCancel ();
                    //m_bgWorker.CancelAsync();
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                }
            progress.FormClosing -= new FormClosingEventHandler ( ProgressDialog_FormClosing );
            }//ProgressDialog_FormClosing

        private void m_bgWorker_RunWorkerCompleted ( object sender, AsyncCompletedEventArgs e )
            {
            if (progress != null)
                {
                progress.Close ();
                progress = null;
                }

            if (e.Error == null)
                {
                if (m_Merger != null && !m_Merger.IsCancelled)
                    {
                    m_StatusLabel.Text = "Files have been merged and placed in the given directory path. ";
                    m_BtnValidateOutput.Enabled = true;
                    MessageBox.Show ( "Files have been merged and put in the respective directory " + m_txtDirectoryPath.Text + " .", "Files Merged in Directory", MessageBoxButtons.OK, MessageBoxIcon.Information );
                    }
                }
            if (e.Error != null)
                {
                MessageBox.Show ( e.Error.Message );
                m_StatusLabel.Text = "Failed in merging the DTBs";
                }
            }//m_bgWorker_RunWorkerCompleted

        private void m_btnUP_Click ( object sender, EventArgs e )
            {
            m_StatusLabel.Text = " You have selected " + m_lbDTBfiles.SelectedItem.ToString () + " to move up.";
            try
                {
                if (m_lbDTBfiles.Items.Count != 0)
                    {
                    if (m_lbDTBfiles.SelectedIndex != 0 && m_lbDTBfiles.SelectedIndex != -1)
                        {
                        object item = m_lbDTBfiles.SelectedItem;
                        int index = m_lbDTBfiles.SelectedIndex;
                        m_lbDTBfiles.Items.RemoveAt ( index );
                        m_lbDTBfiles.Items.Insert ( index - 1, item );
                        }
                    }
                }
            catch (Exception exp)
                {
                MessageBox.Show ( exp.Message );
                }
            }//m_btnUP_Click

        private void m_BtnDown_Click ( object sender, EventArgs e )
            {
            m_StatusLabel.Text = " You have selected " + m_lbDTBfiles.SelectedItem.ToString () + " to move down.";
            try
                {
                int index = m_lbDTBfiles.SelectedIndex;

                if (m_lbDTBfiles.Items.Count != 0)
                    {
                    if (m_lbDTBfiles.SelectedIndex != m_lbDTBfiles.Items.Count - 1 && m_lbDTBfiles.SelectedIndex != -1)
                        {
                        object item = m_lbDTBfiles.SelectedItem;
                        m_lbDTBfiles.Items.RemoveAt ( index );
                        m_lbDTBfiles.Items.Insert ( index + 1, item );
                        }
                    }
                }
            catch (Exception exp)
                {
                MessageBox.Show ( exp.Message );
                }
            }//m_BtnDown_Click

        private void m_lbDTBfiles_SelectedIndexChanged ( object sender, EventArgs e )
            {
            m_BtnValidateInput.Enabled = m_lbDTBfiles.Items.Count > 0 && m_lbDTBfiles.SelectedIndex >= 0;
            m_BtnDelete.Enabled = m_lbDTBfiles.Items.Count > 0 && m_lbDTBfiles.SelectedIndex >= 0;
            m_BtnValidateOutput.Enabled = false;
            m_BtnReset.Enabled = false;
            m_BtnReset.Enabled = m_lbDTBfiles.Items.Count > 0 || m_txtDirectoryPath.Text.Length > 0;

            if (m_lbDTBfiles.Items.Count == 1)
                {
                if (daisy3Option == true)
                    m_StatusLabel.Text = "  Please select at least two OPF Files for merging ";
                if (daisy202option == true)
                    m_StatusLabel.Text = "  Please select at least two NCC Files for merging ";
                }
            this.m_btnUP.Enabled = this.m_lbDTBfiles.SelectedIndex > 0;
            this.m_BtnDown.Enabled = this.m_lbDTBfiles.SelectedIndex > -1 && m_lbDTBfiles.SelectedIndex < m_lbDTBfiles.Items.Count - 1;

            if (m_lbDTBfiles.SelectedIndex >= 0)
                {
                m_txtDTBookInfo.Clear ();
                DTBMerger.DTBFilesInfo fileInfo = new DTBMerger.DTBFilesInfo ( m_lbDTBfiles.SelectedItem.ToString () );
                m_txtDTBookInfo.Multiline = true;
                m_txtDTBookInfo.ReadOnly = true;
                m_txtDTBookInfo.Text =
                    "Title: " + fileInfo.title + Environment.NewLine +
                    "Book ID: " + fileInfo.ID + Environment.NewLine +
                    "Total duration: " + fileInfo.time;
                }
            }//m_lbOPFfiles_SelectedIndexChanged

        private void m_BtnValidateInput_Click ( object sender, EventArgs e )
            {
            if (m_lbDTBfiles.SelectedItems.Count != 0)
                {
                if (daisy3Option == true)
                    m_StatusLabel.Text = " Validating The Input OPF File..  Please Wait ";
                if (daisy202option == true)
                    m_StatusLabel.Text = " Validating The Input NCC File..  Please Wait ";


                string completeScriptPath = Path.Combine ( m_PipelineLiteDir, "scripts" );

                try
                    {
                    if (daisy3Option == true)
                        {
                        DTBMerger.PipelineInterface.ScriptsFunctions.Validate ( Path.Combine ( completeScriptPath, "Daisy3DTBValidator.taskScript" ), m_lbDTBfiles.SelectedItem.ToString (), "", m_ValidatorTimeTolerance );
                        }
                    if (daisy202option == true)
                        {
                        DTBMerger.PipelineInterface.ScriptsFunctions.Validate ( Path.Combine ( completeScriptPath, "Daisy202DTBValidator.taskScript" ), m_lbDTBfiles.SelectedItem.ToString (), "", m_ValidatorTimeTolerance );
                        }
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }

                m_StatusLabel.Text = "";
                }
            }//m_BtnValidateInput_Click

        private void m_BtnValidateOutput_Click ( object sender, EventArgs e )
            {
            if (daisy3Option == true)
                m_StatusLabel.Text = " Validating The Output OPF File.. Please wait";
            if (daisy202option == true)
                m_StatusLabel.Text = " Validating The Output NCC File.. Please wait";


            string completeScriptPath = Path.Combine ( m_PipelineLiteDir, "scripts" );
            DirectoryInfo dir = new DirectoryInfo ( m_txtDirectoryPath.Text );

            FileInfo[] opfFiles = dir.GetFiles ( "*.opf ", SearchOption.TopDirectoryOnly );
            FileInfo[] htmlFiles = dir.GetFiles ( "*.html ", SearchOption.TopDirectoryOnly );
            try
                {
                if (daisy3Option == true)
                    {
                    foreach (FileInfo fileInfo in opfFiles)
                        {
                        DTBMerger.PipelineInterface.ScriptsFunctions.Validate ( Path.Combine ( completeScriptPath, "Daisy3DTBValidator.taskScript" ), fileInfo.FullName, "", m_ValidatorTimeTolerance );
                        }
                    }
                if (daisy202option == true)
                    {
                    foreach (FileInfo fileInfo in htmlFiles)
                        {
                        DTBMerger.PipelineInterface.ScriptsFunctions.Validate ( Path.Combine ( completeScriptPath, "Daisy202DTBValidator.taskScript" ), fileInfo.FullName, "", m_ValidatorTimeTolerance );
                        }
                    }

                m_BtnValidateOutput.Enabled = false;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            m_StatusLabel.Text = string.Empty;

            }//m_BtnValidateOutput_Click                     

        private void m_BtnDelete_Click ( object sender, EventArgs e )
            {
            try
                {
                m_StatusLabel.Text = " Deleted the selected file from the Listbox ";
                m_lbDTBfiles.Items.Remove ( m_lbDTBfiles.SelectedItem );
                m_txtDTBookInfo.Clear ();
                }
            catch (Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }//m_BtnDelete_Click

        private void m_BtnReset_Click ( object sender, EventArgs e )
            {
            m_StatusLabel.Text = " Cleared all the Text ";
            m_txtDirectoryPath.Clear ();
            m_lbDTBfiles.Items.Clear ();
            m_txtDTBookInfo.Clear ();
            m_BtnMerge.Enabled = false;
            m_BtnValidateInput.Enabled = false;
            m_BtnValidateOutput.Enabled = false;
            m_btnUP.Enabled = false;
            m_BtnDown.Enabled = false;
            m_BtnDelete.Enabled = false;
            m_BtnReset.Enabled = false;
            }//m_BtnReset_Click

        private void m_BtnExit_Click ( object sender, EventArgs e )
            {
            Close ();
            }//m_BtnExit_Click

        private void Daisy3MergerForm_Load ( object sender, EventArgs e )
            {
            ShowOptionWindow ();
            }

        private void ShowOptionWindow ()
            {
            DaisyMergerOptionForm opfrm = new DaisyMergerOptionForm ();
            if (opfrm.ShowDialog () == DialogResult.OK)
                {
                switch (opfrm.chooseOption)
                    {
                case DaisyMergerOptionForm.Option.d3:
                daisy3Option = true;
                break;
                case DaisyMergerOptionForm.Option.d202:
                daisy202option = true;
                break;
                    }
                }
            else
                {
                this.Close ();
                }
            }

        private void ShowHelpFile ()
            {
            try
                {
                System.Diagnostics.Process.Start ( (new Uri ( Path.Combine ( Path.GetDirectoryName ( GetType ().Assembly.Location ),
                     "DTB Merger-Help.htm" ) )).ToString () );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                return;
                }
            }

        private void m_BtnHelp_Click(object sender, EventArgs e)
        {
            ShowHelpFile();
        }

        private void Daisy3MergerForm_HelpRequested ( object sender, HelpEventArgs hlpevent )
            {
            ShowHelpFile ();
            }
        
        }//class
    }//namespace