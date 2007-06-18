using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Zaboom
{
    public partial class NewProjectDialog : Form
    {
        private string basepath;
        private string filename;
        private string extension;
        private bool userSetLocation;

        public NewProjectDialog(string basepath, string filename, string extension, string title)
        {
            InitializeComponent();
            this.title.Text = title;
            this.basepath = basepath;
            this.extension = extension;
            this.filename = filename;
            userSetLocation = false;
            GenerateFileName();
        }

        public string Title { get { return title.Text; } }
        public string ProjectLocation { get { return location.Text; } }

        private void GenerateFileName()
        {
            if (!userSetLocation)
            {
                location.Text = String.Format("{0}{1}{2}{1}{3}",
                    basepath,
                    Path.DirectorySeparatorChar,
                    SafeName(title.Text),
                    filename);
            }
        }

        public static string SafeName(string title)
        {
            string invalid = "[";
            foreach (char c in Path.GetInvalidFileNameChars()) invalid += String.Format("\\x{0:x2}", (int)c);
            invalid += "]+";
            string safe = Regex.Replace(title, invalid, "_");
            safe = Regex.Replace(safe, "^_", "");
            safe = Regex.Replace(safe, "_$", "");
            return safe;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = Path.GetDirectoryName(location.Text);
            dialog.Filter = extension;
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                location.Text = dialog.FileName;
                userSetLocation = true;
            }
        }

        private void title_TextChanged(object sender, EventArgs e)
        {
            GenerateFileName();
        }
     }
}