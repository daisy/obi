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
        public EmptySection(string title, Settings settings)
            : this()
        {
            mMessageLabel.Text = string.Format(mMessageLabel.Text, title);
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
        }

        public bool KeepWarning
        {
            set { mKeepWarningCheckbox.Checked = value; }
            get { return mKeepWarningCheckbox.Checked; }
        }
    }
}