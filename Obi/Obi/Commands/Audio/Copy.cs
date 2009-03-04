using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media.data;

namespace Obi.Commands.Audio
{
    class Copy: Command
    {
        private Clipboard mOldClipboard;
        private AudioClipboard mNewClipboard;

        public Copy(ProjectView.ProjectView view, PhraseNode node, AudioRange range)
            : base(view)
        {
            mOldClipboard = view.Clipboard;
            mNewClipboard = new AudioClipboard(node, range);
            Label = Localizer.Message("copy_audio");
        }

        public Copy(ProjectView.ProjectView view)
            : this(view, (PhraseNode)view.Selection.Node, ((AudioSelection)view.Selection).AudioRange) {}

        public override List<MediaData> getListOfUsedMediaData ()
            {
            List<MediaData> mediaList = new List<MediaData> ();
            if (mNewClipboard != null && mNewClipboard.Node is PhraseNode && ((PhraseNode)mNewClipboard.Node).Audio != null )
                mediaList.Add ( ((PhraseNode)mNewClipboard.Node).Audio.getMediaData ());

            return mediaList;
            }


        public override void execute()
        {
            View.Clipboard = mNewClipboard;
            View.Selection = SelectionBefore;
        }

        public override void unExecute()
        {
            View.Clipboard = mOldClipboard;
            base.unExecute();
        }
    }
}
