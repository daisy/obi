using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;

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
            mMediaAfter = mMediaBefore.Copy();
            urakawa.media.data.audio.ManagedAudioMedia copy;
            if (view.Clipboard is AudioClipboard)
            {
                AudioClipboard clipboard = (AudioClipboard)view.Clipboard;
                
                copy = view.Presentation.MediaFactory.CreateManagedAudioMedia();
                WavAudioMediaData mediaData = ((WavAudioMediaData)((PhraseNode)clipboard.Node).Audio.AudioMediaData).Copy(
                    new Time((long)(clipboard.AudioRange.SelectionBeginTime * (Time.TIME_UNIT / 1000))),
                    new Time((long)(clipboard.AudioRange.SelectionEndTime * (Time.TIME_UNIT / 1000)))
                    );
                copy.AudioMediaData = mediaData;

            }
            else
            {
                copy = ((PhraseNode)view.Clipboard.Node).Audio.Copy();
            }
            urakawa.media.data.audio.ManagedAudioMedia after;
            if (selection.AudioRange.HasCursor)
            {
                after = mMediaAfter.Split(new Time((long)(selection.AudioRange.CursorTime * (Time.TIME_UNIT/1000.0))));
            }
            else
            {
                after = mMediaAfter.Split(new Time((long)(selection.AudioRange.SelectionEndTime * (Time.TIME_UNIT/1000.0))));
                mMediaAfter.Split(new Time((long)(selection.AudioRange.SelectionBeginTime * (Time.TIME_UNIT / 1000.0))));
            }
            double begin = mMediaAfter.Duration.AsTimeSpan.Milliseconds;
            mSelectionAfter = new AudioSelection(mNode, view.Selection.Control,
                new AudioRange(begin, begin + copy.Duration.AsTimeSpan.Milliseconds));
            mMediaAfter.AudioMediaData.MergeWith(copy.AudioMediaData);
            mMediaAfter.AudioMediaData.MergeWith(after.AudioMediaData);
            SetDescriptions(Localizer.Message("paste_audio"));
        }

        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                List<MediaData> mediaList = new List<MediaData>();
                if (mMediaAfter != null)
                    mediaList.Add(mMediaAfter.MediaData);

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
