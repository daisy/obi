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
        public bool IsBothOptionEnable;
        public chooseDaisy3orDaisy202()
        {
            InitializeComponent();
            m_ExportFormat = Obi.ImportExport.ExportFormat.DAISY3_0;            
        }

        public bool bothOptionEnabled
        {
            get { return IsBothOptionEnable; }
            set
            {
                IsBothOptionEnable = value;
                if (IsBothOptionEnable)
                    m_radBtnDaisy3_202.Enabled = true;
                else
                    m_radBtnDaisy3_202.Enabled = false;
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
        }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}