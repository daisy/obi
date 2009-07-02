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
         private Obi.Export.ExportFormat obj = new Obi.Export.ExportFormat();
        public chooseDaisy3orDaisy202()
        {
            InitializeComponent();
        }
        public Obi.Export.ExportFormat chooseOption { get { return obj; } }       
        private void m_OKBtn_Click(object sender, EventArgs e)
        {
             this.DialogResult = DialogResult.OK;
            if (m_radBtnDaisy3.Checked)
            {
                obj = Obi.Export.ExportFormat.DAISY3_0;
                Close();
            }
            else if (m_radBtnDaisy202.Checked)
            {
                obj = Obi.Export.ExportFormat.DAISY2_02;
                Close();
            }
            else
            {
                MessageBox.Show(" Please select one of the Export on which merging can take place");
            }

        }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}