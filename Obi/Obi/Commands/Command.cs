using System;
using System.Collections.Generic;
using urakawa;
using urakawa.media.data;
using urakawa.undo;

namespace Obi.Commands
{
    public abstract class Command: ICommand
    {
        private ProjectView.ProjectView mView;
        private NodeSelection mSelectionBefore;

        public Command(ProjectView.ProjectView view)
        {
            mView = view;
            mSelectionBefore = mView.Selection;
        }

        public ProjectView.ProjectView View { get { return mView; } }
        public virtual void execute() {}
        public virtual void unExecute() { mView.Selection = mSelectionBefore; }
        public virtual string getShortDescription() { return Localizer.Message("last command"); }

        /// <summary>
        /// Normally commands are always undoable.
        /// </summary>
        public virtual bool canUnExecute() { return true; }

        /// <summary>
        /// Most commands do not use any media data.
        /// </summary>
        public virtual List<MediaData> getListOfUsedMediaData() { return new List<MediaData>(); }

        /// <summary>
        /// We don't normally use long description, so just return the short one.
        /// </summary>
        public virtual string getLongDescription() { return getShortDescription(); }

        /// <summary>
        /// Get the presentation from the view's project
        /// </summary>
        public Presentation getPresentation() { return mView.Project.getPresentation(); }

        /// <summary>
        /// The presentation cannot be set.
        /// </summary>
        public void setPresentation(Presentation newPres)
        {
            throw new Exception("The presentation cannot be set on a command; set the project view instead.");
        }

        public void XukIn(System.Xml.XmlReader source)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void XukOut(System.Xml.XmlWriter destination)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string getXukLocalName()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string getXukNamespaceUri() { return Program.OBI_NS; }
    }

    /// <summary>
    /// The basic command class. All commands inherit from this one.
    /// THIS IS THE OLD CLASS.
    /// </summary>
    public abstract class Command__OLD__
	{
        /// <summary>
        /// The label of this command for the undo menu.
        /// </summary>
        public abstract string Label
        {
            get;
        }

        /// <summary>
        /// Do, or rather redo, the command from the initial state.
        /// </summary>
		public abstract void Do();
        
        /// <summary>
        /// Undo the command and bring everything back to the initial state.
        /// </summary>
        public abstract void Undo();
	}
}