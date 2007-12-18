using urakawa.media.timing;
using urakawa.media.data.audio;

namespace Obi.Commands.Node
{
    class DeleteAudio : Command
    {
        private PhraseNode mNode;                 // the phrase node in which the deletion happens
        private ManagedAudioMedia mDeletedAudio;  // the deleted chunk of audio
        private Time mSplitTimeBegin;
        private Time mSplitTimeEnd;
        private IControlWithSelection mControl;

        public DeleteAudio(ProjectView.ProjectView view)
            : base(view)
        {
            mNode = (PhraseNode)view.Selection.Node;
            mControl = view.Selection.Control;
            mDeletedAudio = null;
            mSplitTimeBegin = new Time(((AudioSelection)view.Selection).WaveformSelection.SelectionBeginTime);
            mSplitTimeEnd = new Time(((AudioSelection)view.Selection).WaveformSelection.SelectionEndTime);
        }

        public override string getShortDescription() { return Localizer.Message("delete_audio"); }

        public override void execute()
        {
            ManagedAudioMedia after = mNode.SplitAudio(mSplitTimeEnd);
            mDeletedAudio = mNode.SplitAudio(mSplitTimeBegin);
            mNode.MergeAudioWith(after);
            View.Selection = new AudioSelection(mNode, mControl, new WaveformSelection(mSplitTimeBegin.getTimeAsMillisecondFloat()));
            base.execute();
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
