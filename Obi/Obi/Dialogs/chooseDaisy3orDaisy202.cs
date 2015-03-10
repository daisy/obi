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
        public bool ExportDaisy2
        {
            get
            {
                return m_cbDaisy202.Checked;
            }
        }
        public bool ExportDaisy3
        {
            get
            {
                return  m_cbDaisy3.Checked;
            }
        }
        public bool ExportEpub3
        {
            get
            {
                return m_cbEpub3.Checked;
            }
        }


        public Obi.ImportExport.ExportFormat chooseOption 
        { 
            get 
            {
                m_ExportFormat = m_cbDaisy3.Checked ? ImportExport.ExportFormat.DAISY3_0 :
                    m_cbDaisy202.Checked ? ImportExport.ExportFormat.DAISY2_02 :
                    ImportExport.ExportFormat.EPUB3;
                return m_ExportFormat; 
            } 
        }       

        private void m_OKBtn_Click(object sender, EventArgs e)
        {
             this.DialogResult = DialogResult.OK;
        }

        private bool m_RestrictToSingleDAISYChoice =false;
        public bool RestrictToSingleDAISYChoice
        { get { return m_RestrictToSingleDAISYChoice ; }
            set
            {
                m_RestrictToSingleDAISYChoice = value;
                m_cbEpub3.Enabled = !value;
            }
            }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_cbDaisy3_CheckedChanged(object sender, EventArgs e )
        {
            if(m_RestrictToSingleDAISYChoice && m_cbDaisy3.Checked)
            {
                m_cbDaisy202.Checked = false;
            }
        }

        private void m_cbDaisy202_CheckedChanged(object sender, EventArgs e)
        {
            if (m_RestrictToSingleDAISYChoice && m_cbDaisy202.Checked)
            {
                m_cbDaisy3.Checked = false;
            }
        }

    }
}