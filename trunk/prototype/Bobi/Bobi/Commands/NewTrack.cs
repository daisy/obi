using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi.Commands
{
    class NewTrack : urakawa.undo.ICommand, ISelectionAfter
    {
        private urakawa.Presentation presentation;  // presentation in which the track is added
        private urakawa.core.TreeNode node;         // node for the new track
        private NodeSelection selectionAfter;       // selection after the node is added
        private Selection selectionBefore;          // selection before the command is executed
        private bool updateSelection;               // update selection flag

        public NewTrack(View.ProjectView view, urakawa.Presentation presentation)
        {
            this.presentation = presentation;
            this.node = this.presentation.getTreeNodeFactory().createNode();
            this.updateSelection = false;
            this.selectionAfter = new NodeSelection(view, this.node);
            this.selectionBefore = view.Selection;
        }

        #region ICommand Members

        public event EventHandler<urakawa.events.undo.ExecutedEventArgs> executed;
        public event EventHandler<urakawa.events.undo.UnExecutedEventArgs> unExecuted;

        public bool canUnExecute() { return true; }
        public List<urakawa.media.data.MediaData> getListOfUsedMediaData() { return new List<urakawa.media.data.MediaData>(); }
        
        public string getLongDescription()
        {
            return string.Format("Added new track (number of tracks in project: {0}.)",
                ((Project)this.presentation.getProject()).NumberOfTracks);
        }
        
        public string getShortDescription() { return "add new track"; }

        public void execute()
        {
            this.presentation.getRootNode().appendChild(this.node);
            if (executed != null) executed(this, new urakawa.events.undo.ExecutedEventArgs(this));
        }

        public void unExecute()
        {
            this.node.detach();
            if (unExecuted != null) unExecuted(this, new urakawa.events.undo.UnExecutedEventArgs(this));
        }


        #endregion

        #region IWithPresentation Members

        public urakawa.Presentation getPresentation() { return this.presentation; }
        public void setPresentation(urakawa.Presentation newPres) { this.presentation = newPres; }

        #endregion

        #region IXukAble Members

        public string getXukLocalName() { return GetType().Name; }
        public string getXukNamespaceUri() { return ""; }
        public void xukIn(System.Xml.XmlReader source) { }
        public void xukOut(System.Xml.XmlWriter destination, Uri baseUri) { }

        #endregion

        #region IChangeNotifier Members

        public event EventHandler<urakawa.events.DataModelChangedEventArgs> changed;

        #endregion

        #region ISelectionAfter Members

        public Selection SelectionAfter { get { return this.selectionAfter; } }
        public Selection SelectionBefore { get { return this.selectionBefore; } }

        public bool UpdateSelection
        {
            get { return this.updateSelection; }
            set { this.updateSelection = value; }
        }

        #endregion
    }
}
