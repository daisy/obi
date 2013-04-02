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
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");        
        }
        public ReportDialog(string reportDialogTitle, string labelInfo, List<string> problemStrings)
            : this()
        {
            m_ProblemStringList = problemStrings;
          //  m_lblReportDialog.Text = labelInfo;
            m_txtBoxPath.Text = labelInfo;
            this.Text = reportDialogTitle;
            m_btnDetails.Text = Localizer.Message("Show_details");
            m_btnDetails.AccessibleName = m_btnDetails.Text;
            if (problemStrings != null && problemStrings.Count > 0)
            {
                m_btnDetails.Enabled = true;
            }
            if (!m_btnDetails.Enabled || m_btnDetails.Text == Localizer.Message("Show_details") )
            {
                m_lbDetailsOfErrors.Visible = false;
                m_grpBox_lb_ErrorsList.Visible = false;
            }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void m_btnDetails_Click(object sender, EventArgs e)
        {
            if (m_btnDetails.Text ==Localizer.Message("Show_details"))
            {
                this.MaximumSize = new Size(453, 304);
                this.Height = 304;
                m_btnDetails.Text = Localizer.Message("Hide_details");
                m_btnDetails.AccessibleName = m_btnDetails.Text;
                m_lbDetailsOfErrors.Visible = true;
                m_grpBox_lb_ErrorsList.Visible = true;
            }
            else
            {
                this.MaximumSize = new Size(453, 150);
                this.Height = 150;
                m_btnDetails.Text = Localizer.Message("Show_details");
                m_btnDetails.AccessibleName = m_btnDetails.Text;
                m_lbDetailsOfErrors.Visible = false;
                m_grpBox_lb_ErrorsList.Visible = false;
            }
            if (m_ProblemStringList != null && m_lbDetailsOfErrors.Items.Count < 1)
            {
                for (int i = 0; i < m_ProblemStringList.Count; i++)
                {
                    m_lbDetailsOfErrors.Items.Add(m_ProblemStringList[i]);
                }
                m_lbDetailsOfErrors.Focus();
            }            
        }

    }
} 