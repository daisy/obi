using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Obi.Dialogs
{
    public partial class ShowSource : Form
    {
        private Project mProject;

        public ShowSource()
        {
            InitializeComponent();
        }

        public ShowSource(Project project)
        {
            InitializeComponent();
            this.mProject = project;
            Text = Text + " - " + project.Title;
            UpdateView();
            project.StateChanged += new Events.Project.StateChangedHandler(project_StateChanged);
        }

        private string GetXUKSource()
        {
            StringBuilder srcstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            XmlWriter writer = XmlWriter.Create(srcstr, settings);
            mProject.saveXUK(writer);
            writer.Close();
            return srcstr.ToString();
        }

        private void project_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            if (e.Change == Obi.Events.Project.StateChange.Modified) UpdateView();
        }

        private void SourceView_FormClosed(object sender, FormClosedEventArgs e)
        {
            mProject.StateChanged -= new Events.Project.StateChangedHandler(project_StateChanged);
        }

        private void UpdateView()
        {
            sourceBox.Text = GetXUKSource();
            sourceBox.Select(0, 0);
            sourceBox.ScrollToCaret();
        }
    }
}