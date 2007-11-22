using System;

namespace Obi.Commands.Node
{
    public class Copy: Command
    {
        private string mLabel;                   // short description label
        private Clipboard mNewClipboard;         // the new contents of the clipboard
        private Clipboard mOldClipboard;         // the old contents of the clipboard
        private IControlWithSelection mControl;  // control in which the selection was originally made

        public Copy(ProjectView.ProjectView view, bool deep, string label): base(view)
        {
            mOldClipboard = view.Clipboard;
            mNewClipboard = new Clipboard(view.Selection.Node, deep);
            mControl = view.Selection.Control;
            mLabel = label;
        }

        public Copy(ProjectView.ProjectView view, bool deep) : this(view, deep, "") { }

        public override string getShortDescription() { return mLabel; }

        public override void execute()
        {
            base.execute();
            View.Clipboard = mNewClipboard;
            View.Selection = new NodeSelection(mNewClipboard.Node, mControl, false);
        }

        public override void unExecute()
        {
            View.Clipboard = mOldClipboard;
            base.unExecute();
        }
    }
}
