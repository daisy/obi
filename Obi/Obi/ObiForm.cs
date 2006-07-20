using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Commands;

namespace Obi
{
    /// <summary>
    /// The main for of the application.
    /// The form consists mostly of a menu bar and a project panel.
    /// We also keep an undo stack (the command manager) and settings.
    /// </summary>
    public partial class ObiForm : Form
    {
        private Project mProject;                  // the project currently being authored
        private Settings mSettings;                // application settings
        private Commands.CommandManager mCmdMngr;  // the undo stack for this project (should it belong to the project?)
        
        private static readonly string XUKFilter = "Obi project file (*.xuk)|*.xuk";  // filter for opening/saving XUK files

        /// <summary>
        /// Initialize a new form. No project is opened at creation time.
        /// </summary>
        public ObiForm()
        {
            InitializeComponent();
            mProject = null;
            mSettings = null;
            mCmdMngr = new Commands.CommandManager();
            InitializeSettings();
            FormUpdateClosedProject();  // no project opened, same as if we closed a project.
        }

        #region GUI event handlers

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(mSettings.DefaultPath);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (ClosedProject())
                {
                    mProject = new Project();
                    mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
                    mProject.Create(dialog.Path, dialog.Title, mSettings.IdTemplate, mSettings.UserProfile);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Open a project from a XUK file by prompting the user for a file location.
        /// Try to close a possibly open project first.
        /// </summary>
        private void mOpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = XUKFilter;
                dialog.InitialDirectory = mSettings.DefaultPath;
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    OpenProject(dialog.FileName);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Clear the list of recently opened files (prompt the user first.)
        /// </summary>
        private void mClearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(Localizer.Message("clear_recent_text"),
                Localizer.Message("clear_recent_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) ClearRecentList();
            Ready();
        }

        /// <summary>
        /// Save the current project under its current name, or ask for one if none is defined yet.
        /// </summary>
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                mProject.SaveAs(mProject.XUKPath);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Save the project under a (presumably) different name.
        /// </summary>
        private void mSaveProjectasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = XUKFilter;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                mProject.SaveAs(dialog.FileName);
                AddRecentProject(dialog.FileName);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Revert the project to its last saved state.
        /// </summary>
        private void mDiscardChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                DialogResult discard = MessageBox.Show(Localizer.Message("discard_changes_text"),
                    Localizer.Message("discard_changes_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (discard == DialogResult.Yes)
                {
                    ForceOpenProject(mProject.XUKPath);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Close the current project.
        /// </summary>
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                mProject.Close();
                mProject = null;
            }
        }

        /// <summary>
        /// Exit if and only if the currently open project was saved correctly.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                Application.Exit();
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Edit the metadata for the project.
        /// </summary>
        private void metadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject.Metadata);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mProject.Touch();
                }
                Ready();
            }
        }

        /// <summary>
        /// Touch the project so that it seems that it was modified.
        /// </summary>
        private void touchProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.Touch();
        }

        /// <summary>
        /// Show or hide the NCX panel in the project panel.
        /// </summary>
        private void tableOfContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TOCPanelVisible)
            {
                mProjectPanel.HideTOCPanel();
                toolStripStatusLabel1.Text = Localizer.Message("status_toc_hidden");
                FormUpdateShowHideTOC();
            }
            else
            {
                mProjectPanel.ShowTOCPanel();
                toolStripStatusLabel1.Text = Localizer.Message("status_toc_shown");
                FormUpdateShowHideTOC();
            }
        }

        /// <summary>
        /// Edit the user profile through the user profile dialog.
        /// </summary>
        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.UserProfile dialog = new Dialogs.UserProfile(mSettings.UserProfile);
            dialog.ShowDialog();
            Ready();
        }

        /// <summary>
        /// Edit the preferences.
        /// </summary>
        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings.IdTemplate, mSettings.DefaultPath);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.IdTemplate.Contains("#")) mSettings.IdTemplate = dialog.IdTemplate;
                if (Directory.Exists(dialog.DefaultDir)) mSettings.DefaultPath = dialog.DefaultDir;
            }
            Ready();
        }

        /// <summary>
        /// Save the settings when closing.
        /// TODO: check for closing project?
        /// </summary>
        private void ObiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                mSettings.SaveSettings();
            }
            catch (Exception x)
            {
                MessageBox.Show(String.Format(Localizer.Message("save_settings_error_text"), x.Message),
                    Localizer.Message("save_settings_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mProject_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            switch (e.Change)
            {
                case Obi.Events.Project.StateChange.Closed:
                    FormUpdateClosedProject();
                    break;
                case Obi.Events.Project.StateChange.Modified:
                    FormUpdateModifiedProject();
                    break;
                case Obi.Events.Project.StateChange.Opened:
                    FormUpdateOpenedProject();
                    mProjectPanel.Project = mProject;
                    mProjectPanel.Clear();
                    mProjectPanel.SynchronizeWithCoreTree(mProject.getPresentation().getRootNode());
                    /*tableOfContentsToolStripMenuItem.Enabled = true;
                    addSubsectionToolStripMenuItem.Click +=
                        new System.EventHandler(mProjectPanel.TOCPanel.addSubSectionToolStripMenuItem_Click);
                    addSectionAtSameLevelToolStripMenuItem.Click +=
                        new System.EventHandler(mProjectPanel.TOCPanel.addSectionAtSameLevelToolStripMenuItem_Click);
                    moveSectionupToolStripMenuItem.Click +=
                        new System.EventHandler(mProjectPanel.TOCPanel.moveUpToolStripMenuItem_Click);
                    deleteSectionToolStripMenuItem.Click +=
                        new System.EventHandler(mProjectPanel.TOCPanel.deleteSectionToolStripMenuItem_Click);
                    FormUpdateShowHideTOC();*/
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    FormUpdateSavedProject();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        /// <param name="path">The path of the XUK file to open.</param>
        private void OpenProject(string path)
        {
            if (ClosedProject())
            {
                ForceOpenProject(path);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Open a project without asking anything (using for reverting, for instance.)
        /// </summary>
        /// <param name="path">The path of the project to open.</param>
        private void ForceOpenProject(string path)
        {
            try
            {
                mProject = new Project();
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
                mProject.Open(path);
                AddRecentProject(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Localizer.Message("open_project_error_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Initialize the application settings.
        /// </summary>
        private void InitializeSettings()
        {
            mSettings = Settings.GetSettings();
            ClearRecentList();
            for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
            {
                AddRecentProject((string)mSettings.RecentProjects[i]);
            }
        }

        /// <summary>
        /// Clear the list of recent projects.
        /// </summary>
        private void ClearRecentList()
        {
            for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
            {
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            mSettings.RecentProjects.Clear();
            mOpenRecentProjectToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Add a project to the list of recent projects.
        /// If the project was already in the list, promote it to the top of the list.
        /// Update the recent menu if necessary.
        /// </summary>
        /// <param name="path">The path of the project to add.</param>
        private void AddRecentProject(string path)
        {
            if (mSettings.RecentProjects.Count == 0)
            {
                mOpenRecentProjectToolStripMenuItem.Enabled = true;
            }
            else
            {
                if (mSettings.RecentProjects.Contains(path))
                {
                    int i = mSettings.RecentProjects.IndexOf(path);
                    mSettings.RecentProjects.RemoveAt(i);
                    mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
                }
            }
            mSettings.RecentProjects.Insert(0, path);
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = Path.GetFileName(path);
            item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { OpenProject(path); });
            mOpenRecentProjectToolStripMenuItem.DropDownItems.Insert(0, item);
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when no project is opened.
        /// </summary>
        private void FormUpdateClosedProject()
        {
            this.Text = Localizer.Message("obi");
            mSaveProjectToolStripMenuItem.Enabled = false;                        // cannot save
            mSaveProjectasToolStripMenuItem.Enabled = false;                      // cannot save as
            mDiscardChangesToolStripMenuItem.Enabled = false;                             // cannot discard changes
            closeProjectToolStripMenuItem.Enabled = false;                       // cannot close
            foreach (ToolStripItem item in editToolStripMenuItem.DropDownItems)  // cannot do any edit
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in tocToolStripMenuItem.DropDownItems)   // cannot modify the TOC
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in viewToolStripMenuItem.DropDownItems)  // cannot change view
            {
                item.Enabled = false;
            }
            mProjectPanel.Project = null;
            if (mProject == null)
            {
                Ready();
            }
            else
            {
                toolStripStatusLabel1.Text = String.Format(Localizer.Message("closed_project"), mProject.Metadata.Title);
            }
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when no project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            FormUpdateSavedProject();
            mSaveProjectasToolStripMenuItem.Enabled = true;
            closeProjectToolStripMenuItem.Enabled = true;
            FormUpdateShowHideTOC();
            foreach (ToolStripItem item in editToolStripMenuItem.DropDownItems)  // cannot do any edit
            {
                item.Enabled = true;
            }
            foreach (ToolStripItem item in viewToolStripMenuItem.DropDownItems)  // cannot change view
            {
                item.Enabled = true;
            }
            mProjectPanel.Project = null;
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when the project is saved.
        /// </summary>
        private void FormUpdateSavedProject()
        {
            this.Text = String.Format("{0} - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mSaveProjectToolStripMenuItem.Enabled = false;
            mDiscardChangesToolStripMenuItem.Enabled = false;
            toolStripStatusLabel1.Text = String.Format(Localizer.Message("saved_project"), mProject.LastPath);
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when the project is unsaved.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            this.Text = String.Format("{0}* - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mSaveProjectToolStripMenuItem.Enabled = true;
            mSaveProjectasToolStripMenuItem.Enabled = true;
            mDiscardChangesToolStripMenuItem.Enabled = true;
            closeProjectToolStripMenuItem.Enabled = true;
            Ready();
        }

        /// <summary>
        /// Change the label of th Show/Hide TOC menu item depending on the visibility of the NCX panel.
        /// </summary>
        private void FormUpdateShowHideTOC()
        {
            tableOfContentsToolStripMenuItem.Text =
                Localizer.Message(mProjectPanel.TOCPanelVisible ? "hide_toc_label" : "show_toc_label");
            foreach (ToolStripItem item in tocToolStripMenuItem.DropDownItems)
            {
                item.Enabled = mProjectPanel.TOCPanelVisible;
            }
        }

        /// <summary>
        /// Check whether a project is currently open and not saved; prompt the user about what to do.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool ClosedProject()
        {
            if (mProject != null && mProject.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        mProject.SaveAs(mProject.XUKPath);
                        mProject.Close();
                        return true;
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.No:
                        mProject.Close();
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                if (mProject != null) mProject.Close();
                return true;
            }
        }

        /// <summary>
        /// Update the status bar to say "Ready."
        /// </summary>
        private void Ready()
        {
            toolStripStatusLabel1.Text = Localizer.Message("ready");
        }
    }
}