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
        
      //  string a = m_txtDirectoryPath.Text;
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
        }

        private void m_BtnOutputDirectory_Click(object sender, EventArgs e)
        {
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
            m_lbOPFfiles.Items.Remove(m_lbOPFfiles.SelectedItem);           
        }

        private void m_BtnReset_Click(object sender, EventArgs e)
        {
            m_txtDirectoryPath.Clear();
            m_lbOPFfiles.Items.Clear();
        }

        private void m_btnUP_Click(object sender, EventArgs e)
        {            
            if (m_lbOPFfiles.SelectedIndex != 0 && m_lbOPFfiles.SelectedIndex != -1)
            {
                object item = m_lbOPFfiles.SelectedItem;
                int index = m_lbOPFfiles.SelectedIndex;
                m_lbOPFfiles.Items.RemoveAt(index);

                m_lbOPFfiles.Items.Insert(index - 1, item);
            }         
        }

        private void m_BtnDown_Click(object sender, EventArgs e)
        {
            try
            {
                int lCount = m_lbOPFfiles.Items.Count;
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
            string[] listOfOpfFiles =  new string[m_lbOPFfiles.Items.Count];
            for (int i = 0; i < m_lbOPFfiles.Items.Count;i++ )
            {
                listOfOpfFiles[i] = m_lbOPFfiles.Items[i].ToString();
            }
          
            DTBMerger.DTBMerger obj = new DTBMerger.DTBMerger(listOfOpfFiles, m_txtDirectoryPath.Text);
            obj.MergeDTDs();
        }

        private void m_BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        
      }//class
    }//namespace

//class DTBinfo.book title,author we can call
//aligning the buttons in form
//Exit->Cancel button