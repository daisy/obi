using urakawa.core;
using urakawa.undo;

namespace Zaboom
{
    public class AddTreeNodeCommand: ICommand
    {
        private Project project;
        private TreeNode node;
        private TreeNode parent;
        private int index;

        public AddTreeNodeCommand(Project project, TreeNode node, TreeNode parent, int index)
        {
            this.project = project;
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
            project.AddTreeNode(node, parent, index);
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
            project.RemoveTreeNode(node);
        }

        #endregion
    }
}
