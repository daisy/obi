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
            mSplitPoint = new Time(view.Selection.Waveform.CursorTime);
        }

        public override string getShortDescription() { return Localizer.Message("split_block"); }

        public override void execute()
        {
            base.execute();
            ManagedAudioMedia newAudio = mNode.SplitAudio(mSplitPoint);
            mNewNode = ((Presentation)mNode.getPresentation()).CreatePhraseNode(newAudio);
            mNode.InsertAfterSelf(mNewNode);
            View.SelectedBlockNode = mNewNode;
        }

        public override void unExecute()
        {
            mNewNode.Detach();
            mNode.MergeAudioWith(mNewNode.Audio);
            base.unExecute();
        }
    }
}
