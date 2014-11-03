using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class Epub3Validator : Form
    {

        private string m_ProjectDirectoryPath = null ;

        private Epub3Validator()
        {
            InitializeComponent();
        }

        public Epub3Validator(string projectdirectoryPath,Settings settings)
            : this()
        {
            m_ProjectDirectoryPath = projectdirectoryPath;
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
        }

        private void m_btnBrowseInputOPF_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            m_openFileDialogBrowse.Filter = "(*.epub;*.opf)|*.epub;*.opf";
            m_openFileDialogBrowse.InitialDirectory= m_ProjectDirectoryPath;
            if (m_openFileDialogBrowse.ShowDialog() == DialogResult.OK)
            {

                m_txtInputEPUB.Text = m_openFileDialogBrowse.FileName;

            }
            
        }

        public string InputEpubPath
        {
            get
            {
                return m_txtInputEPUB.Text;
            }
            set
            {
                m_txtInputEPUB.Text = value;
            }
        }
        //public string OutputValidatorReportFilePath
        //{
        //    set
        //    {
        //        m_txtValidationReport.Text = value;
        //    }
        //}
        public string CompletionStatus
        {
            set
            {
                m_lblEpubCompletionStatus.Text = value;
            }
        }
        public string EpubCheckOutputText
        {
            set
            {
                m_epubCheckRichTextBox.Text = value;
            }
        }
        public bool ShowEpubValidatorDialog
        {
            set
            {
                m_lblSelectInputFile.Visible = value;
                m_lblInputEPUB.Visible = value;
                m_txtInputEPUB.Visible = value;
                m_btnBrowseInputOPF.Visible = value;
                //m_lblSelectValidationFile.Visible = value;
                //m_lblValidationReport.Visible = value;
                //m_txtValidationReport.Visible = value;
                //m_btnBrowseReport.Visible = value;
             
            }
        }
        public bool ShowResultDialog
        {
            set
            {
                m_lblEpubCompletionStatus.Visible = value;
                m_epubCheckRichTextBox.Visible = value;
            }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

       
    }
}