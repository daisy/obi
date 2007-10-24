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
        private bool mHasUnsavedChanges;             // true when the project has unsaved changes

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
            mHasUnsavedChanges = false;
        }


        /// <summary>
        /// True if the project can be safely closed.
        /// </summary>
        public bool CanClose { get { return !mHasUnsavedChanges; } }

        /// <summary>
        /// True if the redo stack is non-empty.
        /// </summary>
        public bool CanRedo { get { return mProject != null && Presentation.UndoRedoManager.canRedo(); } }

        /// <summary>
        /// True if the project has unsaved changes.
        /// </summary>
        public bool CanSave { get { return mHasUnsavedChanges; } }

        /// <summary>
        /// True if the redo stack is non-empty.
        /// </summary>
        public bool CanUndo { get { return mProject != null && Presentation.UndoRedoManager.canUndo(); } }

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
        public string RedoLabel { get { return Presentation.UndoRedoManager.getRedoShortDescription(); } }

        /// <summary>
        /// Get the description of the top undo command.
        /// </summary>
        public string UndoLabel { get { return Presentation.UndoRedoManager.getUndoShortDescription(); } }


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
                mHasUnsavedChanges = false;
                if (ProjectClosed != null) ProjectClosed(this, new ProjectClosedEventArgs(presentation));
            }
        }

        /// <summary>
        /// Notify the session that the presentation has changed.
        /// </summary>
        public void PresentationHasChanged() { mHasUnsavedChanges = true; }

        /// <summary>
        /// Create a new presentation in the session, with a path to save its XUK file.
        /// </summary>
        public void NewPresentation(string path, string title, bool createTitleSection, string id, UserProfile userProfile)
        {
            mProject = new urakawa.Project();
            mProject.setDataModelFactory(mDataModelFactory);
            mProject.setPresentation(mDataModelFactory.createPresentation(), 0);
            mPath = path;
            mHasUnsavedChanges = true;
            Presentation.Initialize(title, createTitleSection, id, userProfile);
            if (ProjectCreated != null) ProjectCreated(this, null);
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
            Presentation.Initialize();
            if (ProjectOpened != null) ProjectOpened(this, null);
        }

        /// <summary>
        /// Save the current presentation to XUK.
        /// </summary>
        public void Save()
        {
            if (CanSave)
            {
                mProject.saveXUK(new Uri(mPath));
                mHasUnsavedChanges = false;
                if (ProjectSaved != null) ProjectSaved(this, null);
            }
        }

        /// <summary>
        /// Save as. Not sure yet about it.
        /// </summary>
        public void SaveAs(string path)
        {
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