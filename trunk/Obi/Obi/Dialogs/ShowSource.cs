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
        private Project mProject;  // the project for which the source is shown

        public ShowSource()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a source view window for a project.
        /// </summary>
        public ShowSource(Project project)
        {
            InitializeComponent();
            this.mProject = project;
            Text = Text + " - " + project.Title;
            UpdateView();
            project.StateChanged += new Events.Project.StateChangedHandler(project_StateChanged);
        }

        /// <summary>
        /// Get the source text to display for the project.
        /// </summary>
        /// <returns>The source as a string.</returns>
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

        /// <summary>
        /// Handler for state changes in the project. Refresh the view when the project is modified.
        /// </summary>
        private void project_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            if (e.Change == Obi.Events.Project.StateChange.Modified) UpdateView();
        }

        /// <summary>
        /// Unregister the listener when the window closes.
        /// </summary>
        private void SourceView_FormClosed(object sender, FormClosedEventArgs e)
        {
            mProject.StateChanged -= new Events.Project.StateChangedHandler(project_StateChanged);
        }

        /// <summary>
        /// Update the source view.
        /// </summary>
        private void UpdateView()
        {
            sourceBox.Text = GetXUKSource();
            sourceBox.Select(0, 0);
            sourceBox.ScrollToCaret();
        }
    }
}