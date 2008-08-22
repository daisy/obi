using urakawa.media.data.audio;
using urakawa.media.timing;

namespace Obi.Commands.Node
{
    class SplitAudio: Command
    {
        private PhraseNode mNode;          // node to split
        private PhraseNode mNewNode;       // node after split
        private Time mSplitPoint;          // split point

        public SplitAudio(ProjectView.ProjectView view) :
            this(view, view.TransportBar.SplitTime)
        {
        }

        public SplitAudio(ProjectView.ProjectView view, double splitTime): base(view)
        {
            mNode = view.GetNodeForSplit();
            mSplitPoint = new Time(splitTime);
            Label = Localizer.Message("split_phrase");
        }

        public override void execute()
        {
            mNewNode = View.Presentation.CreatePhraseNode(mNode.SplitAudio(mSplitPoint));
            if (mNode.CustomClass != null)
            {
                mNewNode.CustomClass = mNode.CustomClass;
            }
            else if (mNode.NodeKind != EmptyNode.Kind.Heading)
            {
                mNewNode.NodeKind = mNode.NodeKind;
            }
            mNewNode.Used = mNode.Used;
            mNewNode.TODO = mNewNode.TODO;
            mNode.InsertAfterSelf(mNewNode);
            if (UpdateSelection) View.SelectedBlockNode = mNewNode;
            if (mNewNode != null && mNewNode.ParentAs<ObiNode>() is SectionNode)
            {
                View.UpdateBlocksLabelInStrip(mNewNode.ParentAs<SectionNode>());
            }
        }

        public override void unExecute()
        {
            MergeAudio.Merge(mNode, mNewNode);
            if (mNode != null) View.UpdateBlocksLabelInStrip(mNode.ParentAs<SectionNode>());
            base.unExecute();
        }
    }
}
