using urakawa.media.data.audio;
using urakawa.media.timing;

namespace Obi.Commands.Node
{
    class SplitAudio: Command
    {
        private PhraseNode mNode;     // node to split
        private PhraseNode mNewNode;  // node after split
        private Time mSplitPoint;     // split point

        public SplitAudio(ProjectView.ProjectView view) : base(view)
        {
            mNode = (PhraseNode)view.SelectedBlockNode;
            mSplitPoint = new Time(((AudioSelection)view.Selection).WaveformSelection.CursorTime);
        }

        public override string getShortDescription() { return Localizer.Message("split_block"); }

        public static PhraseNode Split(PhraseNode node, Time splitPoint)
        {
            PhraseNode newNode = ((Presentation)node.getPresentation()).CreatePhraseNode(node.SplitAudio(splitPoint));
            node.InsertAfterSelf(newNode);
            return newNode;
        }

        public override void execute()
        {
            base.execute();
            mNewNode = Split(mNode, mSplitPoint);
            View.SelectedBlockNode = mNewNode;
        }

        public override void unExecute()
        {
            MergeAudio.Merge(mNode, mNewNode);
            base.unExecute();
        }
    }
}
