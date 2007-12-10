using System;
using System.Collections.Generic;
using urakawa;
using urakawa.media.data;
using urakawa.undo;

namespace Obi.Commands
{
    public abstract class Command: ICommand
    {
        private ProjectView.ProjectView mView;   // the view that the command is executed in
        private NodeSelection mSelectionBefore;  // the selection before the command happened
        private string mLabel;                   // command label (can be overridden)
        private bool mRedo;                      // true if redo, false on first execution

        /// <summary>
        /// Create a new command for a view.
        /// </summary>
        public Command(ProjectView.ProjectView view, string label)
        {
            mView = view;
            mSelectionBefore = mView.Selection;
            mLabel = label;
            mRedo = false;
        }

        public Command(ProjectView.ProjectView view) : this(view, "") { }

        /// <summary>
        /// Set the label for the command (if not using default.)
        /// </summary>
        public string Label { set { mLabel = value; } }

        /// <summary>
        /// Get the redo flag (true if the command is being redone, false if executed the first time.)
        /// </summary>
        public bool Redo { get { return mRedo; } }

        /// <summary>
        /// Get the view that the command is executed into.
        /// </summary>
        public ProjectView.ProjectView View { get { return mView; } }

        /// <summary>
        /// Execute and set the redo flag. If you need to use the redo flag, call base.execute() at the end of execute()!
        /// </summary>
        public virtual void execute() { mRedo = true; }

        /// <summary>
        /// Reset the selection to what it was before the command was executed.
        /// </summary>
        public virtual void unExecute() { mView.Selection = mSelectionBefore; }

        /// <summary>
        /// Get the selection before the command was executed.
        /// </summary>
        protected NodeSelection SelectionBefore { get { return mSelectionBefore; } }

        /// <summary>
        /// Normally commands are always undoable.
        /// </summary>
        public virtual bool canUnExecute() { return true; }

        /// <summary>
        /// Most commands do not use any media data.
        /// </summary>
        public virtual List<MediaData> getListOfUsedMediaData() { return new List<MediaData>(); }

        /// <summary>
        /// The short description is the label of the command.
        /// </summary>
        public virtual string getShortDescription() { return mLabel; }

        /// <summary>
        /// We don't normally use long description, so just return the short one.
        /// </summary>
        public virtual string getLongDescription() { return getShortDescription(); }

        /// <summary>
        /// Get the presentation from the view's project
        /// </summary>
        public urakawa.Presentation getPresentation() { return mView.Presentation; }

        /// <summary>
        /// The presentation cannot be set.
        /// </summary>
        public void setPresentation(urakawa.Presentation newPres)
        {
            // I am not sure what this message is supposed to mean?!
            throw new Exception("The presentation cannot be set on a command; set the pre instead.");
        }

        /// <summary>
        /// Commands are not saved so far.
        /// </summary>
        public void xukIn(System.Xml.XmlReader source)
        {
        }

        /// <summary>
        /// Commands are not saved so far.
        /// </summary>
        public void xukOut(System.Xml.XmlWriter destination, Uri baseUri)
        {
        }

        public string getXukLocalName() { return GetType().Name; }
        public string getXukNamespaceUri() { return DataModelFactory.NS; }
    }
}