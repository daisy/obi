using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MergeUtilityUI
    {
    public partial class m_formDaisy3Merger : Form
        {
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
                m_lbOPFfiles.Items.Add(select_opfFile.SafeFileName);                     
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

        }
    }