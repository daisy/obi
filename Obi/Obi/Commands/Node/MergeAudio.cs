using urakawa.media.timing;

namespace Obi.Commands.Node
{
    class MergeAudio: Command
    {
        private PhraseNode mNode;
        private PhraseNode mNextNode;
        private Time mSplitPoint;
        private NodeSelection mSelection;

        public MergeAudio(ProjectView.ProjectView view, PhraseNode next)
            : base(view)
        {
            mNode = view.SelectedNodeAs<PhraseNode>();
            mNextNode = next;
            mSplitPoint = new urakawa.media.timing.Time(mNode.Audio.getDuration().getTimeDeltaAsMillisecondFloat());
            // Selection could be an audio selection?
            mSelection = new NodeSelection(mNode, view.Selection.Control);
            Label = Localizer.Message("merge_block_with_next");
        }

        /// <summary>
        /// Merge the selected (phrase) node with the following (phrase) node.
        /// </summary>
        public MergeAudio(ProjectView.ProjectView view):
            this(view, (PhraseNode)view.Selection.Node.ParentAs<ObiNode>().PhraseChild(view.Selection.Node.Index + 1))
        {
        }

        public static void Merge(PhraseNode node, PhraseNode next)
        {
            next.Detach();
            node.MergeAudioWith(next.Audio);
        }

        public override void execute()
        {
            Merge(mNode, mNextNode);
            if (UpdateSelection) View.Selection = mSelection;
        }

        public override void unExecute()
        {
            mNextNode.Audio = mNode.SplitAudio(mSplitPoint);
            mNode.InsertAfterSelf(mNextNode);
            base.unExecute();
        }
    }
}
