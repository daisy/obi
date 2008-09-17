using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
    {
    public partial class AssignNewCustomRole : Form
        {
        private string m_strCustomRoleName;

        public AssignNewCustomRole ()
            {
            InitializeComponent ();
            }

        public string CustomClassName
            {
            get { return m_strCustomRoleName; }
            }

        private void m_btnOk_Click ( object sender, EventArgs e )
            {
            m_strCustomRoleName = m_txtCustomRoleName.Text;
            Close ();
            }

        private void m_btnCancel_Click ( object sender, EventArgs e )
            {
            Close ();
            }
        }
    }