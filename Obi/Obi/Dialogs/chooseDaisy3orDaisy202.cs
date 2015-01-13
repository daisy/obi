using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class chooseDaisy3orDaisy202 : Form
    {
         private Obi.ImportExport.ExportFormat m_ExportFormat = new Obi.ImportExport.ExportFormat();
        
        public chooseDaisy3orDaisy202()
        {
            InitializeComponent();
            m_ExportFormat = Obi.ImportExport.ExportFormat.DAISY3_0;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");                  
        }


        public Obi.ImportExport.ExportFormat chooseOption { get { return m_ExportFormat; } }       

        private void m_OKBtn_Click(object sender, EventArgs e)
        {
             this.DialogResult = DialogResult.OK;
             if (m_cbDaisy3.Checked && !m_cbDaisy202.Checked && !m_cbEpub3.Checked)
             {
                 m_ExportFormat = Obi.ImportExport.ExportFormat.DAISY3_0;
                 Close();
             }
             else if (m_cbDaisy202.Checked && !m_cbDaisy3.Checked && !m_cbEpub3.Checked)
             {
                 m_ExportFormat = Obi.ImportExport.ExportFormat.DAISY2_02;
                 Close();
             }
             else if (m_cbEpub3.Checked && !m_cbDaisy3.Checked && !m_cbDaisy202.Checked)
             {
                 m_ExportFormat = Obi.ImportExport.ExportFormat.EPUB3;
                 Close();
             }
             else if (m_cbDaisy3.Checked && m_cbDaisy202.Checked && !m_cbEpub3.Checked)
             {
                 m_ExportFormat = Obi.ImportExport.ExportFormat.Both_DAISY3_DAISY202;
                 Close();
             }
             else if (m_cbDaisy3.Checked && m_cbEpub3.Checked && !m_cbDaisy202.Checked)
             {
                 m_ExportFormat = Obi.ImportExport.ExportFormat.Both_DAISY3_EPUB3;
                 Close();
             }
             else if (m_cbDaisy202.Checked && m_cbEpub3.Checked && !m_cbDaisy3.Checked)
             {
                 m_ExportFormat = Obi.ImportExport.ExportFormat.Both_DAISY202_EPUB3;
                 Close();
             }
             else if (m_cbDaisy3.Checked && m_cbDaisy202.Checked && m_cbEpub3.Checked)
             {
                 m_ExportFormat = Obi.ImportExport.ExportFormat.All;
                 Close();
             }
        }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}