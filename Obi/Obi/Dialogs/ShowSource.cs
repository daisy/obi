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

        public ShowSource() { InitializeComponent(); }

        /// <summary>
        /// Create a new source view for the current project view.
        /// </summary>
        public ShowSource(ProjectView.ProjectView view): this()
        {
            mView = view;
            Text = String.Format("{0} - {1}", Text, mView.Presentation.Title);
            UpdateView();
            mView.Presentation.CommandExecuted += new Obi.Commands.UndoRedoEventHandler(Presentation_CommandExecuted);
            mView.Presentation.CommandUnexecuted += new Obi.Commands.UndoRedoEventHandler(Presentation_CommandUnexecuted);
        }

        private void Presentation_CommandUnexecuted(object sender, Obi.Commands.UndoRedoEventArgs e) { UpdateView(); }
        private void Presentation_CommandExecuted(object sender, Obi.Commands.UndoRedoEventArgs e) { UpdateView(); }

        // Unregister the listener when the window closes.
        private void SourceView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mView.Presentation != null)
            {
                mView.Presentation.CommandExecuted -= new Obi.Commands.UndoRedoEventHandler(Presentation_CommandExecuted);
                mView.Presentation.CommandUnexecuted -= new Obi.Commands.UndoRedoEventHandler(Presentation_CommandExecuted);
            }
        }

        // Update the source view.
        private void UpdateView()
        {
            sourceBox.Text = mView.Presentation.XukString;
            sourceBox.Select(0, 0);
            sourceBox.ScrollToCaret();
        }
    }
}