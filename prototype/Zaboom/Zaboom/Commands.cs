using urakawa.core;
using urakawa.undo;

namespace Zaboom
{
    public class AddTreeNodeCommand: ICommand
    {
        private TreeNode node;
        private TreeNode parent;
        private int index;

        public AddTreeNodeCommand(TreeNode node, TreeNode parent, int index)
        {
            this.node = node;
            this.parent = parent;
            this.index = index;
        }

        #region ICommand Members

        public bool canUnExecute()
        {
            return true;
        }

        public void execute()
        {
            parent.insert(node, index);
        }

        public string getExecuteShortDescription()
        {
            return "Redo add node";
        }

        public string getUnExecuteShortDescription()
        {
            return "Undo add node";
        }

        public void unExecute()
        {
            parent.removeChild(index);
        }

        #endregion
    }
}
