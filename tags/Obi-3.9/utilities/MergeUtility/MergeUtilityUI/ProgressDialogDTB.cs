using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MergeUtilityUI
{
    public partial class ProgressDialogDTB : Form
    {
        private bool m_cancel = false;

        public ProgressDialogDTB()
        {
            InitializeComponent();
        }

        private bool Exit
        {
            get { return m_cancel; }
        }       

        private void ProgressDialogDTB_FormClosing( object sender, FormClosingEventArgs e )
        {
            m_cancel = true;
            e.Cancel = true;
        }
        private void progressDialogDTB_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            Application.Exit();
        }
    }
}