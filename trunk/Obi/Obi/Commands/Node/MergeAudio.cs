using urakawa.media.timing;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Basic merge commands to merge the two nodes given as argument.
    /// Does not preserve node attributes (used, TODO, etc.) so use as
    /// part of a composite command.
    /// </summary>
    class MergeAudio: Command
    {
        private PhraseNode mNode;          // the selected phrase
        private PhraseNode mNextNode;      // the following phrase to merge with
        private Time mSplitTime;           // the split time of the new merged node

        public MergeAudio(ProjectView.ProjectView view, PhraseNode next)
            : base(view)
        {
            mNode = view.SelectedNodeAs<PhraseNode>();
            mNextNode = next;
            mSplitTime = new urakawa.media.timing.Time(mNode.Audio.getDuration().getTimeDeltaAsMillisecondFloat());
            Label = Localizer.Message("merge_phrase_with_next");
        }

        /// <summary>
        /// Merge the selected phrase with the following phrase.
        /// </summary>
        public MergeAudio(ProjectView.ProjectView view):
            this(view, (PhraseNode)view.Selection.Node.ParentAs<ObiNode>().PhraseChild(view.Selection.Node.Index + 1)) {}

        /// <summary>
        /// Merge two nodes; the "next" one is removed after merging.
        /// </summary>
        public static void Merge(ProjectView.ProjectView view, PhraseNode node, PhraseNode next, bool updateSelection)
        {
            next.Detach();
            node.MergeAudioWith(next.Audio);
            if (updateSelection) view.SelectedBlockNode = node;
            view.UpdateBlocksLabelInStrip(node.AncestorAs<SectionNode>());
        }

        public override void execute()
        {
            Merge(View, mNode, mNextNode, UpdateSelection);
        }

        public override void unExecute()
        {
            SplitAudio.Split(View, mNode, mNextNode, mSplitTime, UpdateSelection);
            base.unExecute();
        }
    }
}
