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
        public Epub3Validator()
        {
            InitializeComponent();
        }

        private void m_btnBrowseInputOPF_Click(object sender, EventArgs e)
        {
            DialogResult result = m_openFileDialogBrowse.ShowDialog();
            //string exportEpubPath = mProjectView.GetDAISYExportPath(Obi.ImportExport.ExportFormat.EPUB3,
            //                                                               Path.GetDirectoryName(mSession.Path));

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

       
    }
}