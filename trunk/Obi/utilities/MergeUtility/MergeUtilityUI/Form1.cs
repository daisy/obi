using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;

namespace MergeUtilityUI
    {
    
    public partial class m_formDaisy3Merger : Form
        {
         private ProgressDialogDTB progress = null;
        
        public m_formDaisy3Merger ()
            {
            InitializeComponent ();
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
           
            if (m_lbOPFfiles.Items.Count >= 2) //&& (m_txtDirectoryPath.Text != null))
            {
                m_BtnMerge.Enabled = true;
            }
            m_StatusLabel.Text = "All The Opf Files have been successfully added in the ListBox";     
        }

        private void m_BtnOutputDirectory_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = "Select the  output Directory where you want to put the merged OPF files";
            FolderBrowserDialog saveDir = new FolderBrowserDialog();
            saveDir.ShowNewFolderButton = true;
            saveDir.SelectedPath = m_txtDirectoryPath.Text;

            if (saveDir.ShowDialog(this) == DialogResult.OK)
            {
                m_txtDirectoryPath.Text = saveDir.SelectedPath;                
            }
        }

        private void m_BtnDelete_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = " Deleting the selected file from the Listbox ";
            m_lbOPFfiles.Items.Remove(m_lbOPFfiles.SelectedItem);           
        }

        private void m_BtnReset_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = " Clearing all the Text ";
            m_txtDirectoryPath.Clear();
            m_lbOPFfiles.Items.Clear();
            m_txtDTBookInfo.Clear();
            m_BtnMerge.Enabled = false;
        }

        private void m_btnUP_Click(object sender, EventArgs e)
        {
            m_StatusLabel.Text = " You have selected " + m_lbOPFfiles.SelectedItem.ToString() + " to move up."; 
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
        }

        private void m_BtnMerge_Click(object sender, EventArgs e)
        {
            //m_StatusLabel.Text = "You have selected " + m_lbOPFfiles.SelectedItems.ToString() + " from the listbox for merging. ";
            
            m_bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_bgWorker_DoWork);
            m_bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(m_bgWorker_RunWorkerCompleted);

            progress = new ProgressDialogDTB();
            m_bgWorker.RunWorkerAsync();
            progress.ShowDialog();
            progress = null;

            while (m_bgWorker.IsBusy)
            {
                Application.DoEvents();
            }                        
        }
        
        private delegate void  updateProgress();
        private void m_bgWorker_DoWork(object sender, EventArgs e)
        {
            string[] listOfOpfFiles = new string[m_lbOPFfiles.Items.Count];
            for (int i = 0; i < m_lbOPFfiles.Items.Count; i++)
            {
                listOfOpfFiles[i] = m_lbOPFfiles.Items[i].ToString();
            }
            
            DTBMerger.DTBMerger obj = new DTBMerger.DTBMerger(listOfOpfFiles, m_txtDirectoryPath.Text);
            obj.MergeDTDs();
            int Num = obj.ProgressInfo;          
       
        }

        private void m_bgWorker_RunWorkerCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (progress != null)
            {
                progress.Hide();
                progress = null;
            }
            if (e.Cancelled)
            {
                m_StatusLabel.Text = " Progress was cancelled ";
            }
            if (e.Error == null)
            {
                m_StatusLabel.Text = "The Files has been merged and put in Selected output directory.";
            }
            if (e.Error != null)
            {
                m_StatusLabel.Text = "Failed in merging the files";
            }
        }

        private void m_BtnExit_Click(object sender, EventArgs e)
        {           
            Close();
        }

        private void m_lbOPFfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //m_StatusLabel.Text = " You have selected " + m_lbOPFfiles.SelectedItem.ToString() + " from the ListBox.";
            if (m_lbOPFfiles.SelectedIndex >= 0)
            {
                m_txtDTBookInfo.Clear();
                DTBMerger.DTBFilesInfo fileInfo = new DTBMerger.DTBFilesInfo(m_lbOPFfiles.SelectedItem.ToString());
                m_txtDTBookInfo.Multiline = true;
                m_txtDTBookInfo.Text = fileInfo.title + Environment.NewLine + fileInfo.ID + Environment.NewLine + fileInfo.time;
            }
            
        }      

       }//class
    }//namespace