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
        public ReportDialog(string reportDialogTitle, string labelInfo, List<string> problemStrings)
            : this()
        {
            m_ProblemStringList = problemStrings;
           // m_lblReportDialog.Text = labelInfo;
            m_txtBoxPath.Text = labelInfo;
            this.Text = reportDialogTitle;
            if (problemStrings != null && problemStrings.Count != 0)
               m_btnDetails.Enabled = true;
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void m_btnDetails_Click(object sender, EventArgs e)
        {
            if (m_btnDetails.Text == "Show details")
            {
                this.Height = 304;
                m_btnDetails.Text = "Hide details";
                m_lbDetailsOfImportedFiles.Visible = true;
            }
            else
            {
                this.Height = 154;
                m_btnDetails.Text = "Show details";
                m_lbDetailsOfImportedFiles.Visible = false;
            }
            if (m_ProblemStringList != null && m_lbDetailsOfImportedFiles.Items.Count < 1)
            {                
                for (int i = 0; i < m_ProblemStringList.Count; i++)
                    m_lbDetailsOfImportedFiles.Items.Add(m_ProblemStringList[i]);
            }            
        }
    }
} 