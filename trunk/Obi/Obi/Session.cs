using System;

namespace Obi
{
    /// <summary>
    /// The Obi work session. In the future, it may handle several presentations (i.e. several Obi projects.)
    /// </summary>
    public class Session
    {
        private DataModelFactory mDataModelFactory;  // the Obi data model factory (see below)
        private urakawa.Project mProject;            // the current project (as of now 1 presentation = 1 project)
        private string mPath;                        // path of the XUK file to save to
        private int mChangesCount;                   // changes since (or before) last save

        public event ProjectClosedEventHandler ProjectClosed;   // the project was closed
        public event EventHandler ProjectCreated;               // a new project was created
        public event EventHandler ProjectOpened;                // a project was opened
        public event EventHandler ProjectSaved;                 // a project was saved

        /// <summary>
        /// Create a new session for Obi.
        /// </summary>
        public Session()
        {
            mDataModelFactory = new DataModelFactory();
            mProject = null;
            mPath = null;
            mChangesCount = 0;
        }


        /// <summary>
        /// True if the project can be safely closed.
        /// </summary>
        public bool CanClose { get { return mChangesCount == 0; } }

        /// <summary>
        /// True if the redo stack is non-empty.
        /// </summary>
        public bool CanRedo { get { return mProject != null && Presentation.getUndoRedoManager().canRedo(); } }

        /// <summary>
        /// True if the project has unsaved changes.
        /// </summary>
        public bool CanSave { get { return mChangesCount != 0; } }

        /// <summary>
        /// True if the redo stack is non-empty.
        /// </summary>
        public bool CanUndo { get { return mProject != null && Presentation.getUndoRedoManager().canUndo(); } }

        /// <summary>
        /// True if there is a project currently open.
        /// </summary>
        public bool HasProject { get { return mProject != null; } }

        /// <summary>
        /// Get the path of the XUK file of the current presentation.
        /// </summary>
        public string Path { get { return mPath; } }

        /// <summary>
        /// Get the current (Obi) presentation.
        /// </summary>
        public Presentation Presentation { get { return mProject == null ? null : (Presentation)mProject.getPresentation(0); } }

        /// <summary>
        /// Get the description of the top redo command.
        /// </summary>
        public string RedoLabel { get { return Presentation.getUndoRedoManager().getRedoShortDescription(); } }

        /// <summary>
        /// Get the description of the top undo command.
        /// </summary>
        public string UndoLabel { get { return Presentation.getUndoRedoManager().getUndoShortDescription(); } }


        /// <summary>
        /// Close the last project.
        /// Will close no matter what, so check with CanClose before doing anything.
        /// </summary>
        public void Close()
        {
            if (mProject != null)
            {
                // if the project could not be opened, there is no presentation so this call may fail
                Presentation presentation = null;
                try { presentation = Presentation; } catch(Exception) {}
                mProject = null;
                mChangesCount = 0;
                if (ProjectClosed != null) ProjectClosed(this, new ProjectClosedEventArgs(presentation));
            }
        }

        /// <summary>
        /// Notify the session that the presentation has changed.
        /// </summary>
        public void PresentationHasChanged(int change) { mChangesCount += change; }

        /// <summary>
        /// Create a new presentation in the session, with a path to save its XUK file.
        /// </summary>
        public void NewPresentation(string path, string title, bool createTitleSection, string id, Settings settings)
        {
            mProject = new urakawa.Project();
            mProject.setDataModelFactory(mDataModelFactory);
            mProject.setPresentation(mDataModelFactory.createPresentation(), 0);
            mPath = path;
            mChangesCount = 0;
            Presentation.Initialize(this, title, createTitleSection, id, settings);
            if (ProjectCreated != null) ProjectCreated(this, null);
            ForceSave();
        }

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        public void Open(string path)
        {
            mProject = new urakawa.Project();
            mProject.setDataModelFactory(mDataModelFactory);
            mProject.openXUK(new Uri(path));
            mPath = path;
            Presentation.Initialize(this);
            // Hack to ignore the empty commands saved by the default undo/redo manager
            Presentation.getUndoRedoManager().flushCommands();
            if (ProjectOpened != null) ProjectOpened(this, null);
        }

        /// <summary>
        /// Save the current presentation to XUK.
        /// </summary>
        public void Save() { if (CanSave) ForceSave(); }

        /// <summary>
        /// Always save, regardless of changes.
        /// </summary>
        public void ForceSave()
        {
            mProject.saveXUK(new Uri(mPath));
            mChangesCount = 0;
            if (ProjectSaved != null) ProjectSaved(this, null);
        }

        /// <summary>
        /// Save as: save the project under a different location,
        /// and edit the newly saved project.
        /// </summary>
        public void SaveAs(string path)
        {
            try
            {
                System.IO.FileStream file = System.IO.File.Create(path);
                file.Close();
                Presentation.setRootUri(new Uri(path));
                mPath = path;
                ForceSave();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(
                    String.Format(Localizer.Message("cannot_create_file_text"), path, e.Message),
                    Localizer.Message("cannot_create_file_caption"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }

        }
    }

    public class ProjectClosedEventArgs : EventArgs
    {
        private Presentation mClosedPresentation;
        public ProjectClosedEventArgs(Presentation p) : base() { mClosedPresentation = p; }
        public Presentation ClosedPresentation { get { return mClosedPresentation; } }
    }

    public delegate void ProjectClosedEventHandler(object sender, ProjectClosedEventArgs e);
}