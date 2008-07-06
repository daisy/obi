using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using urakawa.media.timing;

namespace Obi.Dialogs
{
    public partial class ImportFileSplitSize : Form
    {

        private double m_MaxPhraseDuration;
        public ImportFileSplitSize()
        {
            InitializeComponent();
                                    m_MaxPhraseDuration = 0 ;
            m_txtPhraseSize.Text = "10";
        }

        public double MaxPhraseDuration
        {
            get { return m_MaxPhraseDuration; }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            if (m_txtPhraseSize.Text != "" && Convert.ToUInt32(m_txtPhraseSize.Text) != 0)
            {
                double timeLength;
                try
                {
                    timeLength = Convert.ToUInt32(m_txtPhraseSize.Text) * 60 * 1000;
                    m_MaxPhraseDuration = timeLength;
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Invalid input");
                }
            }
                        Close();
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close () ;
        }

    }
}