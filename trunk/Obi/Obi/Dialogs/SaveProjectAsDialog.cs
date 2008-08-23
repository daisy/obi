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
    public partial class SaveProjectAsDialog : Form
    {
        private string m_OriginalProjectPath;
        private string m_NewProjectDirectoryPath;

        public SaveProjectAsDialog()
        {
            InitializeComponent();
                    }

        public SaveProjectAsDialog( string ProjectDirectoryPath):this ()
        {
            DirectoryInfo DirInfo = new DirectoryInfo(ProjectDirectoryPath);
                        m_txtProjectDirectoryName.Text = DirInfo.Name;
            m_txtParentDirectory.Text =DirInfo.Parent.FullName ;
            m_OriginalProjectPath = ProjectDirectoryPath;
        }
        

        public string NewProjectDirectoryPath
        {
            get { return m_NewProjectDirectoryPath; }
                    }

        public bool SavePrimaryDirectoriesOnly
        {
            get { return m_chkSavePrimaryDirectories.Checked; }
        }

        public bool ActivateNewProject
        {
            get { return m_chkActivateNewProject.Checked; }
        }
        private void m_btnBrowseParentDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog () == DialogResult.OK)
            {
                m_txtParentDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            string DirPath = Path.Combine(m_txtParentDirectory.Text, m_txtProjectDirectoryName.Text);

            if (new DirectoryInfo(m_OriginalProjectPath).FullName == new DirectoryInfo(DirPath).FullName)
            {
                MessageBox.Show( Localizer.Message("CannotSaveInSameProjectDirectory"));
                                return;
            }

            try
            {
                Directory.CreateDirectory(DirPath);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                DialogResult = DialogResult.None;
                return;
            }
            m_NewProjectDirectoryPath = DirPath;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}