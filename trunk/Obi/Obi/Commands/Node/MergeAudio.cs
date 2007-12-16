namespace Obi.Commands.Node
{
    class MergeAudio: Command
    {
        private PhraseNode mNode;
        private PhraseNode mNextNode;
        private urakawa.media.timing.Time mSplitPoint;

        public MergeAudio(ProjectView.ProjectView view): base(view)
        {
            mNode = view.SelectedNodeAs<PhraseNode>();
            // TODO this will blow up sooner or later!!! Handle empty nodes at least...
            mNextNode = (PhraseNode)mNode.ParentAs<ObiNode>().PhraseChild(mNode.Index + 1);
            mSplitPoint = new urakawa.media.timing.Time(mNode.Audio.getDuration().getTimeDeltaAsMillisecondFloat());
        }

        public override string getShortDescription() { return Localizer.Message("merge_block_with_next"); }

        /// <summary>
        /// Merge the audio of a node with the next, and remove next.
        /// </summary>
        public static void Merge(PhraseNode node, PhraseNode next)
        {
            next.Detach();
            node.MergeAudioWith(next.Audio);
        }

        public override void execute()
        {
            base.execute();
            Merge(mNode, mNextNode);
        }

        public override void unExecute()
        {
            SplitAudio.Split(mNode, mSplitPoint);
            base.unExecute();
        }
    }
}
