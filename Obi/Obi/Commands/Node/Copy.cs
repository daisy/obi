using System;

namespace Obi.Commands.Node
{
    public class Copy: Command
    {
        private Clipboard mNewClipboard;         // the new contents of the clipboard
        private Clipboard mOldClipboard;         // the old contents of the clipboard

        public Copy(ProjectView.ProjectView view, bool deep, string label): base(view)
        {
            mOldClipboard = view.Clipboard;
            mNewClipboard = new Clipboard(view.Selection.Node, deep);
            Label = label;
        }

        public Copy(ProjectView.ProjectView view, bool deep) : this(view, deep, "") { }

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
