using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media.data;

namespace Obi.Commands.Audio
{
    class Paste: Command
    {
        private PhraseNode mNode;
        private urakawa.media.data.audio.ManagedAudioMedia mMediaBefore;
        private urakawa.media.data.audio.ManagedAudioMedia mMediaAfter;
        private AudioSelection mSelectionAfter;

        public Paste(ProjectView.ProjectView view)
            : base(view)
        {
            mNode = view.SelectedNodeAs<PhraseNode>();
            AudioSelection selection = (AudioSelection)view.Selection;
            mMediaBefore = mNode.Audio;
            mMediaAfter = mMediaBefore.copy();
            urakawa.media.data.audio.ManagedAudioMedia copy;
            if (view.Clipboard is AudioClipboard)
            {
                AudioClipboard clipboard = (AudioClipboard)view.Clipboard;
                copy = ((PhraseNode)clipboard.Node).Audio.copy(
                    new urakawa.media.timing.Time(clipboard.AudioRange.SelectionBeginTime),
                    new urakawa.media.timing.Time(clipboard.AudioRange.SelectionEndTime));
            }
            else
            {
                copy = ((PhraseNode)view.Clipboard.Node).Audio.copy();
            }
            urakawa.media.data.audio.ManagedAudioMedia after;
            if (selection.AudioRange.HasCursor)
            {
                after = mMediaAfter.Split(new urakawa.media.timing.Time(selection.AudioRange.CursorTime));
            }
            else
            {
                after = mMediaAfter.Split(new urakawa.media.timing.Time(selection.AudioRange.SelectionEndTime));
                mMediaAfter.Split(new urakawa.media.timing.Time(selection.AudioRange.SelectionBeginTime));
            }
            double begin = mMediaAfter.Duration.AsTimeSpan.Milliseconds;
            mSelectionAfter = new AudioSelection(mNode, view.Selection.Control,
                new AudioRange(begin, begin + copy.Duration.AsTimeSpan.Milliseconds));
            mMediaAfter.mergeWith(copy);
            mMediaAfter.mergeWith(after);
            Label = Localizer.Message("paste_audio");
        }

        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                List<MediaData> mediaList = new List<MediaData>();
                if (mMediaAfter != null)
                    mediaList.Add(mMediaAfter.getMediaData());

                return mediaList;
            }
        }

        public override bool CanExecute { get { return true; } }


        public override void Execute()
        {
            mNode.Audio = mMediaAfter;
            View.Selection = mSelectionAfter;
        }

        public override void UnExecute()
        {
            mNode.Audio = mMediaBefore;
            base.UnExecute();
        }
    }
}
