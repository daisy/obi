using System;
using System.Collections.Generic;

using urakawa.command;
using urakawa.events;
using urakawa.events.undo;
using urakawa.media.data;
using urakawa.progress;

namespace Obi.Commands
{
    public abstract class Command: ICommand
    {
        private ProjectView.ProjectView mView;   // the view that the command is executed in
        private NodeSelection mSelectionBefore;  // the selection before the command happened
        private string mLabel;                   // command label (can be overridden)
        protected bool mRedo;                    // true if redo, false on first execution

        public bool UpdateSelection;             // flag to set the selection update

        public event EventHandler<urakawa.events.command.ExecutedEventArgs> executed;
        public event EventHandler<urakawa.events.command.UnExecutedEventArgs> unExecuted;
        public event EventHandler<DataModelChangedEventArgs> changed;


        /// <summary>
        /// Create a new command for a view with a label.
        /// </summary>
        public Command(ProjectView.ProjectView view, string label)
        {
            mView = view;
            mSelectionBefore = mView.Selection;
            mLabel = label;
            mRedo = false;
            UpdateSelection = true;
        }

        /// <summary>
        /// Create a new command for a view with no label.
        /// </summary>
        public Command(ProjectView.ProjectView view) : this(view, "") { }


        /// <summary>
        /// In Obi, all commands can be executed. Ain't that nice?
        /// </summary>
        public virtual bool canExecute() { return true; }

        public abstract void execute();

        /// <summary>
        /// Set the label for the command (if not using default.)
        /// </summary>
        public string Label { set { mLabel = value; } }

        /// <summary>
        /// Get the view that the command is executed into.
        /// </summary>
        public ProjectView.ProjectView View { get { return mView; } }


        /// <summary>
        /// Reset the selection to what it was before the command was executed.
        /// </summary>
        public virtual void unExecute() { if (UpdateSelection) mView.Selection = mSelectionBefore; }

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
        public void xukIn(System.Xml.XmlReader source, ProgressHandler handler)
        {
        }

        /// <summary>
        /// Commands are not saved so far.
        /// </summary>
        public void xukOut(System.Xml.XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
        }

        public string getXukLocalName() { return GetType().Name; }
        public string getXukNamespaceUri() { return DataModelFactory.NS; }
    }

    public class UpdateSelection : Command
    {
        private NodeSelection mSelectionAfter;

        public UpdateSelection(ProjectView.ProjectView view, NodeSelection selection)
            : base(view)
        {
            mSelectionAfter = selection;
        }

        public override void execute()
        {
            View.Selection = mSelectionAfter;
        }
    }
}