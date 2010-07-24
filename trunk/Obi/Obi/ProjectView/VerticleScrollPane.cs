using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class VerticleScrollPane : UserControl
    {
        public VerticleScrollPane()
        {
            InitializeComponent();
        }

        private void m_BtnGoToBegining_Click(object sender, EventArgs e)
        {

        }

        private void m_BtnOneLineUp_Click(object sender, EventArgs e)
        {

        }

        private void m_BtnLargeIncrementUp_Click(object sender, EventArgs e)
        {

        }

        private void m_BtnLargeIncrementDown_Click(object sender, EventArgs e)
        {

        }

        private void m_BtnOneLineDown_Click(object sender, EventArgs e)
        {

        }

        private void m_BtnGoToEnd_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackBar1.Capture = false;            
        }
    }
}
