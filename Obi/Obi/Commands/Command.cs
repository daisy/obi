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

        /// <summary>
        /// Create a new command for a view.
        /// </summary>
        public Command(ProjectView.ProjectView view)
        {
            mView = view;
            mSelectionBefore = mView.Selection;
        }

        public ProjectView.ProjectView View { get { return mView; } }
        public virtual void execute() {}
        public virtual void unExecute() { mView.Selection = mSelectionBefore; }
        public virtual string getShortDescription() { return Localizer.Message("last command"); }

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