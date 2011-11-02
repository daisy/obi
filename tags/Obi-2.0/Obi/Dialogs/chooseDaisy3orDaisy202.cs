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
        }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}