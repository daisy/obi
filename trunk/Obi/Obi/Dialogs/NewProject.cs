using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class NewProject : Form
    {
        private string mTitle;
        private string mPath;

        public string Title { get { return mTitle; } }
        public string Path { get { return mPath; } }

        public NewProject(string path)
        {
            InitializeComponent();
            mPath = path;
            mTitle = Localizer.Message("new_project");
            titleBox.Text = mTitle;
            fileBox.Text = mPath;
            GenerateFileName();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mTitle = titleBox.Text;
            mPath = fileBox.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(fileBox.Text);
            dialog.Filter = "XUK project file (*.xuk)|*.xuk";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileBox.Text = dialog.FileName;
            }
        }

        private void titleBox_Leave(object sender, EventArgs e)
        {
            GenerateFileName();
        }

        private void GenerateFileName()
        {
            fileBox.Text = String.Format(@"{0}\{1}.xuk", System.IO.Path.GetDirectoryName(fileBox.Text),
                Project.ShortName(titleBox.Text));
        }
    }
}