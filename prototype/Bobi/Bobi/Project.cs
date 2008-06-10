using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi
{
    public class Project : urakawa.Project
    {
        public Uri Path;           // path to .xuk file
        private int changes;       // changes since last save
        private bool initialized;  // flag set once the project is properly initialized


        /// <summary>
        /// Create a new empty project.
        /// </summary>
        public Project() : base()
        {
            Path = null;
            this.changes = 0;
            setPresentation(getDataModelFactory().createPresentation(), 0);
            SetUndoRedoEvents();
            this.initialized = true;
        }

        /// <summary>
        /// Create a new, uninitialized project ready for reading.
        /// </summary>
        public Project(Uri path) : base()
        {
            Path = path;
            this.initialized = false;
        }


        /// <summary>
        /// True if there are changes since the last time the project was saved (or created.)
        /// </summary>
        public bool HasChanges { get { return this.changes != 0; } }

        /// <summary>
        /// True once the project has been properly initialized.
        /// </summary>
        public bool Initialized { get { return this.initialized; } }

        /// <summary>
        /// Get the number of tracks in the current project.
        /// </summary>
        public int NumberOfTracks { get { return getPresentation(0).getRootNode().getChildCount(); } }

        /// <summary>
        /// Open a XUK file at the set path.
        /// </summary>
        public void Open()
        {
            openXUK(Path);
            SetUndoRedoEvents();
            this.initialized = true;
        }

        /// <summary>
        /// Redo the last undone change.
        /// </summary>
        public void Redo()
        {
            urakawa.undo.UndoRedoManager redo = getPresentation(0).getUndoRedoManager();
            if (redo.canRedo()) redo.redo();
        }

        /// <summary>
        /// Save changes to the current path (if set.)
        /// </summary>
        public void Save()
        {
            if (Path != null)
            {
                saveXUK(Path);
                Changes(-this.changes);
            }
        }

        /// <summary>
        /// Undo the last change.
        /// </summary>
        public void Undo()
        {
            urakawa.undo.UndoRedoManager undo = getPresentation(0).getUndoRedoManager();
            if (undo.canUndo()) undo.undo();
        }


        // Keep track of the number of changes since open, and send an event when it changes.
        private void Changes(int n)
        {
            if (this.initialized)
            {
                this.changes += n;
                notifyChanged(new urakawa.events.DataModelChangedEventArgs(this));
            }
        }

        // Send changes events when commands are executed or undone.
        private void Project_commandDone(object sender, urakawa.events.undo.DoneEventArgs e) { Changes(+1); }
        private void Project_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e) { Changes(+1); }
        private void Project_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e) { Changes(-1); }

        // Set undo/redo events once the project is initialized.
        private void SetUndoRedoEvents()
        {
            getPresentation(0).getUndoRedoManager().commandDone += new EventHandler<urakawa.events.undo.DoneEventArgs>(Project_commandDone);
            getPresentation(0).getUndoRedoManager().commandReDone += new EventHandler<urakawa.events.undo.ReDoneEventArgs>(Project_commandReDone);
            getPresentation(0).getUndoRedoManager().commandUnDone += new EventHandler<urakawa.events.undo.UnDoneEventArgs>(Project_commandUnDone);
        }
    }
}
