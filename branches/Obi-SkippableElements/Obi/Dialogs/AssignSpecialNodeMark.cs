using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AssignSpecialNodeMark : Form
    {
        private string m_SelectedSpecialNode = "";
        public AssignSpecialNodeMark()
        {
            InitializeComponent();
            m_cmbBoxSpecialNode.SelectedIndex = 0;
        }

        public string SelectedSpecialNode
        {
            get { return m_SelectedSpecialNode; }
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            m_SelectedSpecialNode = m_cmbBoxSpecialNode.SelectedItem.ToString();            
        }

    }
}