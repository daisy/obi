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
        
        public chooseDaisy3orDaisy202(Settings settings)
        {
            InitializeComponent();
            m_ExportFormat = Obi.ImportExport.ExportFormat.DAISY3_0;
            m_radBtnDaisy3_202.Enabled = false;
            m_radBtnEpub3.Enabled = false;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig  
            }
        }

        public bool bothOptionEnabled
        {
            get { return m_radBtnDaisy3_202.Enabled; }
            set
            {
                m_radBtnDaisy3_202.Enabled = value;
            }

        }

        public bool EnableEPUBOption
        {
            get
            {
                return m_radBtnEpub3.Enabled;
            }
            set
            {
                m_radBtnEpub3.Enabled = value;
            }
        }

        public Obi.ImportExport.ExportFormat chooseOption { get { return m_ExportFormat; } }       

        private void m_OKBtn_Click(object sender, EventArgs e)
        {
             this.DialogResult = DialogResult.OK;
            if (m_radBtnDaisy3.Checked)
            {
                m_ExportFormat = Obi.ImportExport.ExportFormat.DAISY3_0;
                Close();
            }
            else if (m_radBtnDaisy202.Checked)
            {
                m_ExportFormat = Obi.ImportExport.ExportFormat.DAISY2_02;
                Close();
            }
            else if (m_radBtnDaisy3_202.Checked)
            {
                m_ExportFormat = Obi.ImportExport.ExportFormat.Both_DAISY3_DAISY202;
                Close();
            }
            else if (m_radBtnEpub3.Checked)
            {
                m_ExportFormat = Obi.ImportExport.ExportFormat.EPUB3;
                Close();
            }
        }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}