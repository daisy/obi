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
using Obi.UserControls;

using urakawa.core;

namespace Obi
{
    public partial class ObiForm : Form
    {
        private Project mProject;          // the project currently being authored
        private Settings mSettings;        // application settings
        private UndoRedoStack mUndoStack;  // the undo stack for this project (should it belong to the project?)
        
        private event Events.Project.StateChangedHandler ProjectStateChanged;  // track changes on the project

        private static readonly string XukFilter = "Obi project file (*.xuk)|*.xuk";  // filter for opening/saving XUK files

        /// <summary>
        /// Initialize a new form. No project is opened at creation time.
        /// </summary>
        public ObiForm()
        {
            InitializeComponent();
            mProject = null;
            ProjectStateChanged += new Events.Project.StateChangedHandler(mProject_StateChanged);
            GUIUpdateNoProject();
            UpdateShowHideTOC();
            InitializeSettings();
            mUndoStack = new UndoRedoStack();
            undo_label = mUndoToolStripMenuItem.Text;
            redo_label = mRedoToolStripMenuItem.Text;
            UndoStackChanged += new UndoStackChangedHandler(mUndoStack_UndoStackChanged);
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
        /// Save the settings when closing.
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

        #region Menu items

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void createNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(mSettings.DefaultPath);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (ClosedProject())
                {
                    mProject = new Project(dialog.Path, dialog.Title, mSettings.IdTemplate, mSettings.UserProfile);
                    OnProjectCreated();
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

        #endregion





        /// <summary>
        /// Open a project from a XUK file by prompting the user for a file location.
        /// </summary>
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (ClosedProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = XukFilter;
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    mProject = Project.Open(dialog.FileName);
                    AddRecentProject(dialog.FileName);
                    OnProjectOpened();
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
             * */
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
                openrecentProjectToolStripMenuItem.Enabled = true;
            }
            else
            {
                if (mSettings.RecentProjects.Contains(path))
                {
                    int i = mSettings.RecentProjects.IndexOf(path);
                    mSettings.RecentProjects.RemoveAt(i);
                    openrecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
                }
            }
            mSettings.RecentProjects.Insert(0, path);
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = Path.GetFileName(path);
            item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { OpenProject(path); });
            openrecentProjectToolStripMenuItem.DropDownItems.Insert(0, item);
        }

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        /// <param name="path">The path of the XUK file to open.</param>
        private void OpenProject(string path)
        {
            /*
            if (ClosedProject())
            {
                mProject = Project.Open(path);
                AddRecentProject(path);
                OnProjectOpened();
            }
            else
            {
                Ready();
            }
             * */
        }

        /// <summary>
        /// Clear the list of recently opened files (prompt the user first.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(Localizer.Message("clear_recent_text"),
                Localizer.Message("clear_recent_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ClearRecentList();
            }
            Ready();
        }

        /// <summary>
        /// Clear the list of recent projects.
        /// </summary>
        private void ClearRecentList()
        {
            for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
            {
                openrecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            mSettings.RecentProjects.Clear();
            openrecentProjectToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Save the current project under its current name, or ask for one if none is defined yet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                if (mProject.XUKPath == null)
                {
                    SaveProjectAs();
                }
                else
                {
                    mProject.Save();
                    OnProjectSaved();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Save the project under a (presumably) different name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveProjectasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectAs();
        }

        /// <summary>
        /// Save the project under a file name supplied by the user through a file chooser.
        /// The currently open project is still the old project.
        /// </summary>
        private void SaveProjectAs()
        {
            /*
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = XukFilter;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (mProject.SetXukPath(dialog.FileName))
                {
                    mProject.Save();
                    OnProjectSaved();
                }
                else
                {
                    mProject.SaveAs(dialog.FileName);
                    Debug(String.Format(Localizer.Message("debug_saved_project"), dialog.FileName));
                }
                AddRecentProject(dialog.FileName);
            }
            else
            {
                Ready();
            }*/
        }
        
        /// <summary>
        /// Revert the project to its last saved state, or if it was never saved, just reset it to a blank project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (mProject.Unsaved)
            {
                DialogResult discard = MessageBox.Show(Localizer.Message("discard_changes_text"),
                    Localizer.Message("discard_changes_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (discard == DialogResult.Yes)
                {
                    if (mProject.XUKPath == null)
                    {
                        mProject = new Project(mProject.Metadata.Title, mProject.XUKPath, mProject.Metadata.Identifier,
                            mSettings.UserProfile);
                        OnProjectCreated();
                    }
                    else
                    {
                        mProject = Project.Open(mProject.XUKPath);
                        OnProjectOpened();
                    }
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
             * */
        }

        /// <summary>
        /// Close the current project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                mProject = null;
            }
        }

        /// <summary>
        /// Exit if and only if the currently open project was saved correctly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Check whether a project is currently open and not saved; prompt the user about whatto do.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool ClosedProject()
        {
            if (mProject != null && mProject.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    mProject.Save();
                    OnProjectSaved();
                }
                if (result == DialogResult.Cancel)
                {
                    Ready();
                    return false;
                }
                else
                {
                    OnProjectClosed();
                    return true;
                }
            }
            else
            {
                OnProjectClosed();
                Ready();
                return true;
            }
        }

        /// <summary>
        /// Edit the user profile through the user profile dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.UserProfile dialog = new Dialogs.UserProfile(mSettings.UserProfile);
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Debug(String.Format(Localizer.Message("debug_user_profile"), mSettings.UserProfile.ToString()));
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Edit the preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Edit the metadata for the project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject.Metadata);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mProject.Touch();
                    OnProjectModified();
                }
                Ready();
            }
        }

        /// <summary>
        /// Touch the project so that it seems that it was modified (used for debugging.)
        /// </summary>
        private void touchProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.Touch();
            OnProjectModified();
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
                UpdateShowHideTOC();
            }
            else
            {
                mProjectPanel.ShowTOCPanel();
                toolStripStatusLabel1.Text = Localizer.Message("status_toc_shown");
                UpdateShowHideTOC();
            }
        }

        /// <summary>
        /// Change the label of th Show/Hide TOC menu item depending on the visibility of the NCX panel.
        /// </summary>
        private void UpdateShowHideTOC()
        {
            tableOfContentsToolStripMenuItem.Text = Localizer.Message(mProjectPanel.TOCPanelVisible ?
                "hide_toc_label" : "show_toc_label");
            addChildSectionToolStripMenuItem.Enabled = mProjectPanel.TOCPanelVisible;
            addsiblingSectionToolStripMenuItem.Enabled = mProjectPanel.TOCPanelVisible && mProjectPanel.TOCPanel.Selected;
        }

        private void OnProjectClosed()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Closed));
        }

        private void OnProjectCreated()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Created));
        }

        private void OnProjectModified()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Modified));
        }

        private void OnProjectOpened()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Opened));
        }

        private void OnProjectSaved()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Saved));
        }

        private void mProject_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            switch (e.Change)
            {
                case Obi.Events.Project.StateChange.Closed:
                    GUIUpdateNoProject();
                    break;
                case Obi.Events.Project.StateChange.Created:
                    GUIUpdateSavedProject();
                    mProjectPanel.Project = e.Project;
                    tableOfContentsToolStripMenuItem.Enabled = true;
                    addChildSectionToolStripMenuItem.Click +=
                        new System.EventHandler(mProjectPanel.TOCPanel.addSubSectionToolStripMenuItem_Click);
                    addsiblingSectionToolStripMenuItem.Click +=
                        new System.EventHandler(mProjectPanel.TOCPanel.addSectionAtSameLevelToolStripMenuItem_Click);
                    UpdateShowHideTOC();
                    Debug(Localizer.Message("debug_created_project"));
                    break;
                case Obi.Events.Project.StateChange.Modified:
                    GUIUpdateUnsavedProject();
                    Debug(Localizer.Message("debug_touched_project"));
                    break;
                case Obi.Events.Project.StateChange.Opened:
                    GUIUpdateSavedProject();
                    mProjectPanel.Project = e.Project;
                    tableOfContentsToolStripMenuItem.Enabled = true;
                    UpdateShowHideTOC();
                    Debug(String.Format(Localizer.Message("debug_opened_project"), e.Project.XUKPath));
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    GUIUpdateSavedProject();
                    Debug(String.Format(Localizer.Message("debug_saved_project"), e.Project.XUKPath));
                    break;
                default:
                    Debug("******");
                    break;
            }
        }

        /// <summary>
        /// Utility method that prints to the console and outputs to the status bar.
        /// </summary>
        /// <param name="message">The message to print.</param>
        private void Debug(string message)
        {
            Console.WriteLine(message);
            toolStripStatusLabel1.Text = message;
        }

        /// <summary>
        /// Update the GUI to reflect enabled/disabled actions when no project is opened.
        /// </summary>
        private void GUIUpdateNoProject()
        {
            this.Text = Localizer.Message("obi");
            closeProjectToolStripMenuItem.Enabled = false;
            saveProjectToolStripMenuItem.Enabled = false;
            saveProjectasToolStripMenuItem.Enabled = false;
            touchProjectToolStripMenuItem.Enabled = false;
            revertToolStripMenuItem.Enabled = false;
            metadataToolStripMenuItem.Enabled = false;
            mProjectPanel.Project = null;
            tableOfContentsToolStripMenuItem.Enabled = false;
            Ready();
        }

        /// <summary>
        /// Update the GUI to reflect enabled/disabled actions when the project is saved.
        /// </summary>
        private void GUIUpdateSavedProject()
        {
            this.Text = String.Format("{0} - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            closeProjectToolStripMenuItem.Enabled = true;
            saveProjectToolStripMenuItem.Enabled = false;
            saveProjectasToolStripMenuItem.Enabled = true;
            touchProjectToolStripMenuItem.Enabled = true;
            revertToolStripMenuItem.Enabled = false;
            metadataToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Update the GUI to reflect enabled/disabled actions when the project is unsaved.
        /// </summary>
        private void GUIUpdateUnsavedProject()
        {
            this.Text = String.Format("{0}* - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            closeProjectToolStripMenuItem.Enabled = true;
            saveProjectToolStripMenuItem.Enabled = true;
            saveProjectasToolStripMenuItem.Enabled = true;
            touchProjectToolStripMenuItem.Enabled = true;
            revertToolStripMenuItem.Enabled = true;
            metadataToolStripMenuItem.Enabled = true;
            Ready();
        }

        /// <summary>
        /// Update the status bar to say "Ready."
        /// </summary>
        private void Ready()
        {
            toolStripStatusLabel1.Text = Localizer.Message("ready");
        }





        private string undo_label;
        private string redo_label;

        private delegate void UndoStackChangedHandler(object sender, EventArgs e);
        private event UndoStackChangedHandler UndoStackChanged;

        private void mStrip_Resized(object sender, ResizedEventArgs e)
        {
            PushUndo(new ResizeStripCommand((EmptyStrip)sender, e.Before, e.After));
        }

        private void PushUndo(Command command)
        {
            mUndoStack.Push(command);
            UndoStackChanged(this, new EventArgs());
        }

        private void mUndoStack_UndoStackChanged(object sender, EventArgs e)
        {
            if (mUndoStack.CanUndo)
            {
                mUndoToolStripMenuItem.Enabled = true;
                mUndoToolStripMenuItem.Text = String.Format(Localizer.Message("undo_label"), undo_label, mUndoStack.UndoLabel);
            }
            else
            {
                mUndoToolStripMenuItem.Enabled = false;
                mUndoToolStripMenuItem.Text = undo_label;
            }
            if (mUndoStack.CanRedo)
            {
                mRedoToolStripMenuItem.Enabled = true;
                mRedoToolStripMenuItem.Text = String.Format(Localizer.Message("redo_label"), redo_label, mUndoStack.RedoLabel);
            }
            else
            {
                mRedoToolStripMenuItem.Enabled = false;
                mRedoToolStripMenuItem.Text = redo_label;
            }
        }

        private void mUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mUndoStack.CanUndo)
            {
                mUndoStack.Undo();
                UndoStackChanged(this, new EventArgs());
            }
        }

        private void mRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mUndoStack.CanRedo)
            {
                mUndoStack.Redo();
                UndoStackChanged(this, new EventArgs());
            }
        }
    }
}