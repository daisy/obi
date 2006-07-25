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

        // filter for opening/saving XUK files
        public static readonly string XUKFilter = "Obi project file (*.xuk)|*.xuk|Any file|*.*";

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

            mOpenRecentProjectToolStripMenuItem.Enabled = mSettings.RecentProjects.Count > 0;
            FormUpdateClosedProject();  // no project opened, same as if we closed a project.
        }

        #region GUI event handlers

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(mSettings.DefaultPath);
            dialog.CreateTitleSection = mSettings.CreateTitleSection;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (ClosedProject())
                {
                    mProject = new Project();
                    mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
                    mProject.Create(dialog.Path, dialog.Title, mSettings.IdTemplate, mSettings.UserProfile,
                        dialog.CreateTitleSection);
                    mSettings.CreateTitleSection = dialog.CreateTitleSection;
                    AddRecentProject(mProject.XUKPath);
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
        private void mCloseProjectToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void mShowhideTableOfContentsToolStripMenuItem_Click(object sender, EventArgs e)
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
                    mProjectPanel.Project = mProject;
                    FormUpdateOpenedProject();
                    mProjectPanel.SynchronizeWithCoreTree(mProject.getPresentation().getRootNode());
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    FormUpdateSavedProject();
                    break;
            }
        }

        /// <summary>
        /// Setup the TOC and strip menus in the same way as the context menus for TOCPanel and StripManager.
        /// </summary>
        private void ObiForm_Load(object sender, EventArgs e)
        {
            mAddSectionAtSameLevelToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.addSectionAtSameLevelToolStripMenuItem_Click);
            mAddsubsectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.addSubSectionToolStripMenuItem_Click);
            mRenameSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.editLabelToolStripMenuItem_Click);
            mDeleteSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.deleteSectionToolStripMenuItem_Click);

            mProjectPanel.StripManager.SelectedStrip += new Events.Strip.SelectedHandler(StripManager_Selected);
            mAddStripToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.addStripToolStripMenuItem_Click);
            renameStripToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.renameStripToolStripMenuItem_Click);
        }

        private void StripManager_Selected(object sender, Events.Strip.SelectedEventArgs e)
        {
            renameStripToolStripMenuItem.Enabled = e.Selected;
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
                mProject = null;
            }
        }


        /// <summary>
        /// Initialize the application settings: get the settings from the saved user settings or the system
        /// and add the list of recent projects (at least those that actually exist) to the recent project menu.
        /// </summary>
        private void InitializeSettings()
        {
            mSettings = Settings.GetSettings();
            for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
            {
                if (!AddRecentProjectsItem((string)mSettings.RecentProjects[i]))
                {
                    mSettings.RecentProjects.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Clear the list of recent projects.
        /// </summary>
        private void ClearRecentList()
        {
            while (mOpenRecentProjectToolStripMenuItem.DropDownItems.Count > 2)
            {
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(0);
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
            if (AddRecentProjectsItem(path)) mSettings.RecentProjects.Insert(0, path);
        }

        /// <summary>
        /// Add an item in the recent projects list, if the file actually exists.
        /// </summary>
        /// <param name="path">The path of the item to add.</param>
        /// <returns>True if the file was added.</returns>
        private bool AddRecentProjectsItem(string path)
        {
            if (File.Exists(path))
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(path);
                item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { OpenProject(path); });
                mOpenRecentProjectToolStripMenuItem.DropDownItems.Insert(0, item);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when no project is opened.
        /// </summary>
        private void FormUpdateClosedProject()
        {
            this.Text = Localizer.Message("obi");
            mSaveProjectToolStripMenuItem.Enabled = false;                        // cannot save
            mSaveProjectasToolStripMenuItem.Enabled = false;                      // cannot save as
            mDiscardChangesToolStripMenuItem.Enabled = false;                     // cannot discard changes
            mCloseProjectToolStripMenuItem.Enabled = false;                       // cannot close
            foreach (ToolStripItem item in editToolStripMenuItem.DropDownItems)   // cannot do any edit
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in mTocToolStripMenuItem.DropDownItems)    // cannot modify the TOC
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in mStripsToolStripMenuItem.DropDownItems)  // cannot modify the strips
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
        /// Update the form to reflect enabled/disabled actions when a project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            this.Text = String.Format("{0} - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mSaveProjectToolStripMenuItem.Enabled = false;
            mSaveProjectasToolStripMenuItem.Enabled = true;
            mDiscardChangesToolStripMenuItem.Enabled = false;
            mCloseProjectToolStripMenuItem.Enabled = true;
            foreach (ToolStripItem item in editToolStripMenuItem.DropDownItems)     // can do edits
            {
                item.Enabled = true;
            }
            foreach (ToolStripItem item in mTocToolStripMenuItem.DropDownItems)     // can modify the TOC
            {
                item.Enabled = true;
            }
            foreach (ToolStripItem item in mStripsToolStripMenuItem.DropDownItems)  // can modify the strips
            {
                item.Enabled = true;
            }
            FormUpdateShowHideTOC();
            toolStripStatusLabel1.Text = String.Format(Localizer.Message("opened_project"), mProject.XUKPath);
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
        /// Update the form to reflect enabled/disabled actions when the project is modified.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            this.Text = String.Format("{0}* - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mSaveProjectToolStripMenuItem.Enabled = true;
            mSaveProjectasToolStripMenuItem.Enabled = true;
            mDiscardChangesToolStripMenuItem.Enabled = true;
            mCloseProjectToolStripMenuItem.Enabled = true;
            Ready();
        }

        /// <summary>
        /// Change the label of th Show/Hide TOC menu item depending on the visibility of the NCX panel.
        /// </summary>
        private void FormUpdateShowHideTOC()
        {
            mShowhideTableOfCOntentsToolStripMenuItem.Text =
                Localizer.Message(mProjectPanel.TOCPanelVisible ? "hide_toc_label" : "show_toc_label");
            foreach (ToolStripItem item in mTocToolStripMenuItem.DropDownItems)
            {
                item.Enabled = mProjectPanel.TOCPanelVisible;
            }
            mShowhideTableOfCOntentsToolStripMenuItem.Enabled = true;
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