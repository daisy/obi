using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Zaboom
{
    public partial class SourceView : Form
    {
        public SourceView()
        {
            InitializeComponent();
        }

        public SourceView(string path, string title)
        {
            InitializeComponent();
            Text = Text + " - " + title;
            sourceBox.Text = "";
            try
            {
                System.IO.StreamReader reader = System.IO.File.OpenText(path);
                sourceBox.AppendText(reader.ReadToEnd());
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Could not read file \"{0}\": {1}",
                    path, e.Message));
            }
            sourceBox.Select(0, 0);
            sourceBox.ScrollToCaret();
        }
    }
}