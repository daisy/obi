using System;
using System.Collections.Generic;

using urakawa.command;
using urakawa.events;
using urakawa.events.undo;
using urakawa.media.data;
using urakawa.progress;

namespace Obi.Commands
{
    [XukNamespaceAttribute(DataModelFactory.NS)]
    public abstract class Command: urakawa.command.Command
    {
        //public static readonly string XUK_NS = DataModelFactory.NS;
        //public static readonly string XukString = typeof(Command).Name;
        //public override string GetTypeNameFormatted()
        //{
        //    return XukString;
        //}


        private ProjectView.ProjectView mView;   // the view that the command is executed in
        private NodeSelection mSelectionBefore;  // the selection before the command happened
        private string mLabel;                   // command label (can be overridden)
        protected bool mRedo;                    // true if redo, false on first execution
        private int m_ProgressPercentage = -1; //@singleSection: triggers progress changed event if value is positive

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
            m_ProgressPercentage = -1; //@singleSection
        }

        /// <summary>
        /// Create a new command for a view with no label.
        /// </summary>
        public Command(ProjectView.ProjectView view) : this(view, "") { }


        /// <summary>
        /// In Obi, all commands can be executed. Ain't that nice?
        /// </summary>
        //public virtual bool canExecute() { return true; }
        public override bool CanUnExecute  { get { return true; } }//sdk2 : temp

        //sdk2 public override void Execute() {}
        
        public void SetDescriptions(string str)
        {
            ShortDescription = str;
            LongDescription = str;
        }

        //sdk2
        ///// <summary>
        ///// Set the label for the command (if not using default.)
        ///// </summary>
        //public string Label { set { mLabel = value; } }

        /// <summary>
        /// Get the view that the command is executed into.
        /// </summary>
        public ProjectView.ProjectView View { get { return mView; } }

        /// <summary>
        /// set progress percentage for progress changed event, event is triggered only if positive value is assigned
        /// </summary>
        public int ProgressPercentage 
            {
            protected get
                {
                return m_ProgressPercentage;
                }
            set 
                { 
                m_ProgressPercentage = value; 
                } 
            }

        //@singleSection: this has to be triggered from derived classes as command execute has to be abstract
        protected void TriggerProgressChanged () { if (m_ProgressPercentage >= 0) mView.TriggerProgressChangedEvent ( ProjectView.ProjectView.ProgressBar_Command, m_ProgressPercentage ); }

        /// <summary>
        /// Reset the selection to what it was before the command was executed.
        /// </summary>
        public override void UnExecute() 
            {
            if (m_ProgressPercentage >= 0) mView.TriggerProgressChangedEvent ( "command",100 -  m_ProgressPercentage );//@singleSection
            if (UpdateSelection) mView.Selection = mSelectionBefore; 
            }

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
        public override IEnumerable<MediaData> UsedMediaData { get { return new List<MediaData>(); } }

        //sdk2
        ///// <summary>
        ///// The short description is the label of the command.
        ///// </summary>
        //public virtual string ShortDescription { get { return mLabel; } }

        ///// <summary>
        ///// We don't normally use long description, so just return the short one.
        ///// </summary>
        //public virtual string getLongDescription() { return getShortDescription(); }

        ///// <summary>
        ///// Get the presentation from the view's project
        ///// </summary>
        //public urakawa.Presentation getPresentation() { return mView.Presentation; }

        ///// <summary>
        ///// The presentation cannot be set.
        ///// </summary>
        //public void setPresentation(urakawa.Presentation newPres)
        //{
        //    // I am not sure what this message is supposed to mean?!
        //    throw new Exception("The presentation cannot be set on a command; set the pre instead.");
        //}

        ///// <summary>
        ///// Commands are not saved so far.
        ///// </summary>
        //public void xukIn(System.Xml.XmlReader source, urakawa.progress.IProgressHandler handler)
        //{
        //}

        ///// <summary>
        ///// Commands are not saved so far.
        ///// </summary>
        //public void xukOut(System.Xml.XmlWriter destination, Uri baseUri, IProgressHandler handler)
        //{
        //}

        //public string getXukLocalName() { return GetType().Name; }
        //public string getXukNamespaceUri() { return DataModelFactory.NS; }
    }

    public class UpdateSelection : Command
    {
        private NodeSelection mSelectionAfter;
        private bool m_RefreshSelectionForUnexecute;

        public UpdateSelection(ProjectView.ProjectView view, NodeSelection selection)
            : base(view)
        {
            mSelectionAfter = selection;
            m_RefreshSelectionForUnexecute = false;
        }

        public bool RefreshSelectionForUnexecute
        {
            get { return m_RefreshSelectionForUnexecute; }
            set { m_RefreshSelectionForUnexecute = value; }
        }
        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            View.Selection = mSelectionAfter;
            TriggerProgressChanged ();
        }

        public override void UnExecute ()
            {
            if (mSelectionAfter == null
                || (mSelectionAfter.Node != null && mSelectionAfter.Node.IsRooted))
                {
                if (!UpdateRefreshedSelection () )  base.UnExecute ();
                }
            }
        private bool UpdateRefreshedSelection()
        {
            if (ProgressPercentage >= 0) View.TriggerProgressChangedEvent("command", 100 - ProgressPercentage);//@singleSection
            if (UpdateSelection && m_RefreshSelectionForUnexecute && SelectionBefore != null)
            {
                View.Selection = new NodeSelection(SelectionBefore.Node, SelectionBefore.Control);
                return true;
            }
            return false;
        }


    }
}