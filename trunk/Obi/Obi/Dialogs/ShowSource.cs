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
        private ProjectView.ProjectView mView;  // the project view with the attached project

        /// <summary>
        /// Used by the designer.
        /// </summary>
        public ShowSource()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a new source view for the current project view.
        /// </summary>
        public ShowSource(ProjectView.ProjectView view): this()
        {
            mView = view;
            Text = String.Format("{0} - {1}", Text, mView.Project.Title);
            UpdateView();
            mView.Project.StateChanged += new Obi.Events.Project.StateChangedHandler(project_StateChanged);
            mView.CommandExecuted += new Obi.Commands.UndoRedoEventHandler(view_CommandExecuted);
            mView.CommandUnexecuted += new Obi.Commands.UndoRedoEventHandler(view_CommandExecuted);
        }

        // Return the XUK source as a string.
        private string GetXUKSource()
        {
            StringBuilder srcstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            XmlWriter writer = XmlWriter.Create(srcstr, settings);
            mView.Project.saveXUK(writer);
            writer.Close();
            return srcstr.ToString();
        }

        // Update the view on project modification
        private void project_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            if (e.Change == Obi.Events.Project.StateChange.Modified) UpdateView();
        }

        // Update the view on undo/redo
        private void view_CommandExecuted(object sender, Obi.Commands.UndoRedoEventArgs e) { UpdateView(); }

        // Unregister the listener when the window closes.
        private void SourceView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mView != null)
            {
                if (mView.Project != null)
                {
                    mView.Project.StateChanged -= new Obi.Events.Project.StateChangedHandler(project_StateChanged);
                }
                mView.CommandExecuted -= new Obi.Commands.UndoRedoEventHandler(view_CommandExecuted);
                mView.CommandUnexecuted -= new Obi.Commands.UndoRedoEventHandler(view_CommandExecuted);
            }
        }

        // Update the source view.
        private void UpdateView()
        {
            sourceBox.Text = GetXUKSource();
            sourceBox.Select(0, 0);
            sourceBox.ScrollToCaret();
        }
    }
}