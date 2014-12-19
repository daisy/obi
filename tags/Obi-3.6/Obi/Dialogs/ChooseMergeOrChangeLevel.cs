using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ChooseMergeOrChangeLevel : Form
    {
        public ChooseMergeOrChangeLevel()
        {
            InitializeComponent();           
        }
        public ChooseMergeOrChangeLevel(ProjectView.ProjectView View)
            : this()
        {
            if (!View.CanMergeStripWithNext)
            {
                m_rbMerge.Enabled = false;
            }
        }

        public bool MergeSection
        {
            get
            {
                return m_rbMerge.Checked;
            }
        }
        public bool ChangeSectionLevel
        {
            get
            {
                return m_rbChangeLevel.Checked;
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}