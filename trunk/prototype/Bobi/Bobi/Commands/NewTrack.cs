using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi.Commands
{
    class NewTrack : urakawa.undo.ICommand
    {
        private urakawa.Presentation presentation;  // presentation in which the track is added
        private urakawa.core.TreeNode node;         // node for the new track

        public NewTrack(urakawa.Presentation presentation)
        {
            this.presentation = presentation;
            this.node = this.presentation.getTreeNodeFactory().createNode();
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
        
        public string getShortDescription() { return "new track"; }

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
    }
}
