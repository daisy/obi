using urakawa.media.timing;
using urakawa.media.data.audio;

namespace Obi.Commands.Audio
{
    class Delete : Command
    {
        private PhraseNode mNode;                 // the phrase node in which the deletion happens
        private ManagedAudioMedia mDeletedAudio;  // the deleted chunk of audio
        private Time mSplitTimeBegin;             // begin time of audio split
        private Time mSplitTimeEnd;               // end time of audio split
        private NodeSelection mSelectionAfter;    // selection after deletion (cursor at split point)

        public Delete(ProjectView.ProjectView view)
            : base(view)
        {
            mNode = (PhraseNode)view.Selection.Node;
            mDeletedAudio = null;
            mSplitTimeBegin = new Time(((AudioSelection)view.Selection).AudioRange.SelectionBeginTime);
            mSplitTimeEnd = new Time(((AudioSelection)view.Selection).AudioRange.SelectionEndTime);
            mSelectionAfter = new AudioSelection(mNode, view.Selection.Control,
                new AudioRange(mSplitTimeBegin.getTimeAsMillisecondFloat()));
            Label = Localizer.Message("delete_audio");
        }

        public override void execute()
        {
            ManagedAudioMedia after = mNode.SplitAudio(mSplitTimeEnd);
            mDeletedAudio = mNode.SplitAudio(mSplitTimeBegin);
            mNode.MergeAudioWith(after);
            View.Selection = mSelectionAfter;
        }

        public override void unExecute()
        {
            ManagedAudioMedia after = mNode.SplitAudio(mSplitTimeBegin);
            mNode.MergeAudioWith(mDeletedAudio);
            mNode.MergeAudioWith(after);
            base.unExecute();
        }
    }
}
