using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class EmptySection : Form
    {
        public EmptySection() { InitializeComponent(); }
        public EmptySection(string title) : this() { mMessageLabel.Text = string.Format(mMessageLabel.Text, title); }

        public bool KeepWarning
        {
            set { mKeepWarningCheckbox.Checked = value; }
            get { return mKeepWarningCheckbox.Checked; }
        }
    }
}