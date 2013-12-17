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
        public EmptySection()
        {
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");          
        }
        public EmptySection(string title) : this() { mMessageLabel.Text = string.Format(mMessageLabel.Text, title); }

        public bool KeepWarning
        {
            set { mKeepWarningCheckbox.Checked = value; }
            get { return mKeepWarningCheckbox.Checked; }
        }
    }
}