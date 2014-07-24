using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioProcessingDialog : Form
    {
        public AudioProcessingDialog()
        {
            InitializeComponent();
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {

        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}