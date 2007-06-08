using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ShowSource : Form
    {
        public ShowSource()
        {
            InitializeComponent();
        }

        public ShowSource(Project project)
        {
            InitializeComponent();
            mSourceTextBox.Text = "";
            try
            {
                System.IO.StreamReader reader = System.IO.File.OpenText(project.XUKPath);
                mSourceTextBox.AppendText(reader.ReadToEnd());
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Could not read XUK file \"{0}\": {1}",
                    project.XUKPath, e.Message));
            }
            mSourceTextBox.Select(0, 0);
            mSourceTextBox.ScrollToCaret();
        }
    }
}