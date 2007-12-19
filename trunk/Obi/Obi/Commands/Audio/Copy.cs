using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Audio
{
    class Copy: Command
    {
        private Clipboard mOldClipboard;
        private AudioClipboard mNewClipboard;

        public Copy(ProjectView.ProjectView view)
            : base(view)
        {
            mOldClipboard = view.Clipboard;
            mNewClipboard = new AudioClipboard((AudioSelection)view.Selection);
            Label = Localizer.Message("copy_audio");
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
