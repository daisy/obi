using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MergeUtilityUI
{
    public partial class DaisyMergerOptionForm : Form
    {
        Daisy3MergerForm mergerFrm;
        
        public DaisyMergerOptionForm()
        {
            InitializeComponent();
        }

        private void m_btnOK_Click(object sender, EventArgs e)
        {                     
            if (m_rdbDaisy3.Checked || m_rdbDaisy202.Checked)
            {
                mergerFrm = new Daisy3MergerForm(m_rdbDaisy3.Checked , m_rdbDaisy202.Checked);
                mergerFrm.ShowDialog();
            }
            else 
            {
                MessageBox.Show(" Please select one of the Export on which merging can take place");
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DaisyMergerOptionForm_Load(object sender, EventArgs e)
        {
            m_rdbDaisy3.Checked = true;
        }       
    }
}