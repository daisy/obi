using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ReportDialog : Form
    {
        List<string> m_ProblemStringList = new List<string>();
    
        public ReportDialog()
        {
            InitializeComponent();
        }
        public ReportDialog(List<string> problemStrings, string labelInfo, string reportDialogName)
            : this()
        {
            m_ProblemStringList = problemStrings;
            m_lblReportDialog.Text = labelInfo;
            this.Text = reportDialogName;
            if (problemStrings.Count != null)
                m_btnDetails.Enabled = true;               
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void m_btnDetails_Click(object sender, EventArgs e)
        {
            this.Height = 240;
            if (m_ProblemStringList != null)
            {
                for (int i = 0; i < m_ProblemStringList.Count; i++)
                    m_lbDetailsOfImportedFiles.Items.Add(m_ProblemStringList[i]);
            }
        }
    }
}