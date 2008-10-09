using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.Dialogs
{
    public partial class SelectDirectoryPath : Form
    {
        private string m_DirectoryPath;

        public SelectDirectoryPath(string path)
        {
            InitializeComponent();
                                    m_txtDirectoryPath.Text = path;
            folderBrowserDialog1.SelectedPath = path;
            folderBrowserDialog1.ShowNewFolderButton = true    ;
        }

        public string DirectoryPath
        {
            get { return m_DirectoryPath; }
        }

        private void m_btnDirectoryBrowse_Click(object sender, EventArgs e)
        {
                       DialogResult result =  folderBrowserDialog1.ShowDialog();
                       if (result == DialogResult.OK)
                       {
                           m_txtDirectoryPath.Text = folderBrowserDialog1.SelectedPath;
                       }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            if ( m_txtDirectoryPath.Text.Trim () != ""    &&   !Directory.Exists(m_txtDirectoryPath.Text))
            {
                DialogResult MsgBoxResult =  MessageBox.Show("Directory do not exist. \n Do you want to create it?", "Error!", MessageBoxButtons.YesNo);
                if (MsgBoxResult == DialogResult.Yes)
                {
                    if (!CreateDirectory(m_txtDirectoryPath.Text)) return;
                }
                else
                {
                    return;
                }
            }

        try
            {
            m_DirectoryPath = System.IO.Path.GetFullPath ( m_txtDirectoryPath.Text );
            }
        catch (System.Exception ex)
            {
            MessageBox.Show ( ex.ToString () );
            return;
            }
            

            this.DialogResult = DialogResult.OK;
            Close();
        }

        public bool  CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}