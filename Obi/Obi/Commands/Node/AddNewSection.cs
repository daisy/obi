namespace Obi.Commands.Node
{
    public class AddSectionNode: Command
    {
        protected ObiNode mParent;                        // the parent of the new section node
        protected int mIndex;                             // the index where the section node is inserted
        private SectionNode mNode;                        // the new section node
        private IControlWithRenamableSelection mControl;  // control in which the section is added

        /// <summary>
        /// Add a new section node in the context of the previous selection. Pass along the control
        /// in which the node is added as the selection might be null. The section node is created
        /// here and will be added when the command is executed.
        /// </summary>
        public AddSectionNode(ProjectView.ProjectView view, IControlWithRenamableSelection control): base(view)
        {
            mNode = view.Presentation.CreateSectionNode();
            SetParentAndIndex(view);
            mNode.Used = mParent.Used;
            mControl = control;
            view.SelectAndRenameSelection(new NodeSelection(mNode, mControl));
            Label = Localizer.Message("add_section");
        }

        // Set parent and index for the new node
        protected virtual void SetParentAndIndex(ProjectView.ProjectView view)
        {
            if (view.Selection == null)
            {
                mParent = view.Presentation.RootNode;
                mIndex = mParent.SectionChildCount;
            }
            else
            {
                mParent = view.Selection.ParentForNewNode(mNode);
                mIndex = view.Selection.IndexForNewNode(mNode);
            }
        }

        /// <summary>
        /// The new section node to be added.
        /// </summary>
        public SectionNode NewSection { get { return mNode; } }

        /// <summary>
        /// The parent of the new section node to be added.
        /// </summary>
        public ObiNode NewSectionParent { get { return mParent; } }

        /// <summary>
        /// Add or readd the new section node then restore this as the selection.
        /// </summary>
        public override void execute()
        {
            mParent.Insert(mNode, mIndex);
            if (mRedo)
            {
                View.Selection = new NodeSelection(mNode, mControl);
            }
            else
            {
                mRedo = true;
            }
        }

        /// <summary>
        /// Remove the section node.
        /// </summary>
        public override void unExecute()
        {
            mNode.Detach();
            base.unExecute();
        }
    }

    public class AddSubSection : AddSectionNode
    {
        public AddSubSection(ProjectView.ProjectView view) : base(view, (ProjectView.TOCView)view.Selection.Control)
        {
            Label = Localizer.Message("add_subsection");
        }

        protected override void SetParentAndIndex(Obi.ProjectView.ProjectView view)
        {
            mParent = view.Selection.Node;
            mIndex = mParent.SectionChildCount;
        }
    }

    public class InsertSectionNode : AddSectionNode
    {
        public InsertSectionNode(ProjectView.ProjectView view)
            : base(view, (IControlWithRenamableSelection)view.Selection.Control)
        {
            Label = Localizer.Message(view.Selection.Control is ProjectView.TOCView ? "insert_section" : "insert_strip");
        }

        // Set parent and index for the new node
        protected override void SetParentAndIndex(ProjectView.ProjectView view)
        {
            mParent = view.Selection.Node.ParentAs<ObiNode>();
            mIndex = view.Selection.Node.Index;
        }
    }
}
