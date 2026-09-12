using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Obi.Models;

namespace Obi.Dialogs
{
    public partial class PhraseDetectionMethodDialog : Form
    {
        public PhraseDetectionMethod SelectedMethod
        {
            get
            {
                if (m_RadioAI.Checked)
                    return PhraseDetectionMethod.AI;

                return PhraseDetectionMethod.Traditional;
            }
        }

        public PhraseDetectionMethodDialog()
        {
            InitializeComponent();

            m_RadioTraditional.Checked = true;
        }

        private void m_BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}