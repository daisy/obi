using urakawa.media.timing;
using urakawa.media.data.audio;

namespace Obi.Commands.Audio
{
    class Delete : Command
    {
        private PhraseNode mNode;                 // the phrase node in which the deletion happens
        private PhraseNode mDeleted;              // node to store the deleted audio (unrooted)
        private Time mSplitTimeBegin;             // begin time of audio split
        private Time mSplitTimeEnd;               // end time of audio split
        private NodeSelection mSelectionAfter;    // selection after deletion (cursor at split point)

        public Delete(ProjectView.ProjectView view)
            : base(view)
        {
            mNode = (PhraseNode)view.Selection.Node;
            mSplitTimeBegin = new Time(((AudioSelection)view.Selection).AudioRange.SelectionBeginTime);
            mSplitTimeEnd = new Time(((AudioSelection)view.Selection).AudioRange.SelectionEndTime);
            mSelectionAfter = new AudioSelection(mNode, view.Selection.Control,
                new AudioRange(mSplitTimeBegin.getTimeAsMillisecondFloat()));
            mDeleted = view.Presentation.CreatePhraseNode(mNode.Audio.copy(mSplitTimeBegin, mSplitTimeEnd));
            Label = Localizer.Message("delete_audio");
        }

        public PhraseNode Deleted { get { return mDeleted; } }

        public override void execute()
        {
            ManagedAudioMedia after = mNode.SplitAudio(mSplitTimeEnd);
            mNode.SplitAudio(mSplitTimeBegin);
            mNode.MergeAudioWith(after);
            View.Selection = mSelectionAfter;
        }

        public override void unExecute()
        {
            ManagedAudioMedia after = mNode.SplitAudio(mSplitTimeBegin);
            mNode.MergeAudioWith(mDeleted.Audio);
            mNode.MergeAudioWith(after);
            base.unExecute();
        }
    }
}
