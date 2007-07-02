using urakawa.core;
using urakawa.undo;

namespace Zaboom.Commands
{
    public class AddTreeNode: Command
    {
        private TreeNode node;
        private TreeNode parent;
        private int index;

        /// <summary>
        /// Create a new command.
        /// </summary>
        /// <param name="node">The node to add (must not be in the tree yet.)</param>
        /// <param name="parent">Its parent node.</param>
        /// <param name="index">The insertion index.</param>
        public AddTreeNode(TreeNode node, TreeNode parent, int index)
        {
            this.manager = null;  // will be set when added to its manager
            this.node = node;
            this.parent = parent;
            this.index = index;
        }

        #region Command Members

        /// <summary>
        /// Actually add the node to the tree and select it.
        /// </summary>
        public override void execute()
        {
            manager.ProjectPanel.Project.AddTreeNode(node, parent, index);
            manager.ProjectPanel.SelectNode(node);
        }

        public override string getExecuteShortDescription() { return "Redo add node"; }
        public override string getUnExecuteShortDescription() { return "Undo add node"; }

        /// <summary>
        /// Remove the node from the tree and restore the previous selection.
        /// </summary>
        public override void unExecute()
        {
            manager.ProjectPanel.Project.RemoveTreeNode(node);
            manager.ProjectPanel.SelectList(previousSelection);
        }

        #endregion
    }
}
