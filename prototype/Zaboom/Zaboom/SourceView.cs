using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Zaboom
{
    public partial class SourceView : Form
    {
        private Project project;

        public SourceView()
        {
            InitializeComponent();
        }

        public SourceView(Project project)
        {
            InitializeComponent();
            this.project = project;
            Text = Text + " - " + project.Title;
            UpdateView();
            project.StateChanged += new StateChangedHandler(project_StateChanged);
        }

        private string GetXUKSource()
        {
            StringBuilder srcstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "  ";
            XmlWriter writer = XmlWriter.Create(srcstr, settings);
            project.saveXUK(writer);
            writer.Close();
            return srcstr.ToString();
        }

        void project_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.Change == StateChange.Modified) UpdateView();
        }

        private void SourceView_FormClosed(object sender, FormClosedEventArgs e)
        {
            project.StateChanged -= new StateChangedHandler(project_StateChanged);
        }

        private void UpdateView()
        {
            sourceBox.Text = GetXUKSource();
            sourceBox.Select(0, 0);
            sourceBox.ScrollToCaret();
        }
    }
}