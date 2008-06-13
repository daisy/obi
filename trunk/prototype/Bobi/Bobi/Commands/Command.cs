using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi.Commands
{
    public abstract class Command: urakawa.undo.ICommand
    {
        protected View.ProjectView view;
        protected Selection selectionAfter;
        protected Selection selectionBefore;
        protected bool updateSelection;

        public Command(View.ProjectView view)
        {
            this.view = view;
            this.selectionBefore = view.Selection;
            this.updateSelection = true;
        }


        public Selection SelectionAfter { get { return this.selectionAfter; } }
        public Selection SelectionBefore { get { return this.selectionBefore; } }

        public bool UpdateSelection
        {
            get { return this.updateSelection; }
            set
            {
                this.updateSelection = value;
                if (changed != null) changed(this, new urakawa.events.DataModelChangedEventArgs(this));
            }
        }

        #region ICommand Members

        public event EventHandler<urakawa.events.undo.ExecutedEventArgs> executed;
        public event EventHandler<urakawa.events.undo.UnExecutedEventArgs> unExecuted;

        public virtual bool canUnExecute() { return true; }
        public virtual void execute() { if (executed != null) executed(this, new urakawa.events.undo.ExecutedEventArgs(this)); }
        public virtual void unExecute() { if (unExecuted != null) unExecuted(this, new urakawa.events.undo.UnExecutedEventArgs(this)); }
        public virtual List<urakawa.media.data.MediaData> getListOfUsedMediaData() { return new List<urakawa.media.data.MediaData>(); }

        public abstract string getLongDescription();
        public abstract string getShortDescription();

        #endregion

        #region IWithPresentation Members

        public virtual urakawa.Presentation getPresentation() { return this.view.Project.getPresentation(0); }
        public virtual void setPresentation(urakawa.Presentation newPres) { throw new Exception("Don't."); }

        #endregion

        #region IXukAble Members

        public string getXukLocalName() { return GetType().Name; }
        public string getXukNamespaceUri() { return ""; }
        public void xukIn(System.Xml.XmlReader source) {}
        public void xukOut(System.Xml.XmlWriter destination, Uri baseUri) {}

        #endregion

        #region IChangeNotifier Members

        public event EventHandler<urakawa.events.DataModelChangedEventArgs> changed;

        #endregion
    }
}
