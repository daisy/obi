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

        public ShowSource() 
        {
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Source View.htm");  
        }

        /// <summary>
        /// Create a new source view for the current project view.
        /// </summary>
        public ShowSource(ProjectView.ProjectView view): this()
        {
            mView = view;
            Text = String.Format("{0} - {1}", Text, mView.Presentation.Title);
            UpdateView();
            mView.Presentation.UndoRedoManager.CommandDone += new EventHandler<urakawa.events.undo.DoneEventArgs>(ShowSource_commandDone);
            mView.Presentation.UndoRedoManager.CommandReDone += new EventHandler<urakawa.events.undo.ReDoneEventArgs>(ShowSource_commandReDone);
            mView.Presentation.UndoRedoManager.CommandUnDone += new EventHandler<urakawa.events.undo.UnDoneEventArgs>(ShowSource_commandUnDone);
        }

        private void ShowSource_commandDone(object sender, urakawa.events.undo.DoneEventArgs e) { UpdateView(); }
        private void ShowSource_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e) { UpdateView(); }
        private void ShowSource_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e) { UpdateView(); }

        // Unregister the listener when the window closes.
        private void SourceView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mView.Presentation != null)
            {
                mView.Presentation.UndoRedoManager.CommandDone -= new EventHandler<urakawa.events.undo.DoneEventArgs>(ShowSource_commandDone);
                mView.Presentation.UndoRedoManager.CommandReDone -= new EventHandler<urakawa.events.undo.ReDoneEventArgs>(ShowSource_commandReDone);
                mView.Presentation.UndoRedoManager.CommandUnDone -= new EventHandler<urakawa.events.undo.UnDoneEventArgs>(ShowSource_commandUnDone);
            }
        }

        // Update the source view.
        private void UpdateView()
        {
            sourceBox.Text = mView.Presentation.GenerateXukString;
            sourceBox.Select(0, 0);
            sourceBox.ScrollToCaret();
        }
    }
}