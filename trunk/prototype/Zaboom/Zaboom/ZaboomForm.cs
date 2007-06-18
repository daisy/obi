using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Zaboom
{
    public partial class ZaboomForm : Form
    {
        public ZaboomForm()
        {
            InitializeComponent();
            Text = "Zaboom";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectDialog dialog = new NewProjectDialog(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "project.zab",
                "Zaboom project files|*.zab",
                "Untitled Zaboom Project");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                projectPanel.Project = new Project(dialog.Title, new Uri(dialog.ProjectLocation));
                Text = "Zaboom - " + projectPanel.Project.TitleSaved;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectPanel.Project != null)
            {
                projectPanel.Project.Save();
                Text = "Zaboom - " + projectPanel.Project.TitleSaved;
            }
        }

        private void importFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectPanel.Project != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ("WAV files|*.wav");
                dialog.Multiselect = false;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        projectPanel.Project.ImportAudioFile(dialog.FileName);
                    }
                    catch (Exception e_)
                    {
                        MessageBox.Show(
                            String.Format("Could not open WAV file {0}: {1}", dialog.FileName, e_.Message),
                            "Error opening WAV file",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectPanel.Project != null)
            {
                SourceView dialog = new SourceView(projectPanel.Project.getPresentation().getBaseUri().LocalPath,
                    projectPanel.Project.Title);
                dialog.ShowDialog();
            }
        }
    }
}