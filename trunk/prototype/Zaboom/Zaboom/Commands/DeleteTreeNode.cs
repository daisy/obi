using urakawa.core;

namespace Zaboom.Commands
{
    class DeleteTreeNode: Command
    {
        private TreeNode node;
        private TreeNode parent;
        private int index;

        /// <summary>
        /// Create a new command.
        /// </summary>
        /// <param name="node">The node to delete (must still be in the tree.)</param>
        public DeleteTreeNode(TreeNode node)
        {
            manager = null;  // will be set when added to its manager
            this.node = node;
            parent = node.getParent();
            index = parent.indexOf(node); 
        }

        #region Command Members

        /// <summary>
        /// Actually add the node to the tree and select it.
        /// </summary>
        public override void execute()
        {
            manager.ProjectPanel.Deselect();
            manager.ProjectPanel.Project.RemoveTreeNode(node);
        }

        public override string getExecuteShortDescription() { return "Redo delete node"; }
        public override string getUnExecuteShortDescription() { return "Undo delete node"; }

        /// <summary>
        /// Remove the node from the tree and restore the previous selection.
        /// </summary>
        public override void unExecute()
        {
            manager.ProjectPanel.Project.AddTreeNode(node, parent, index);
            manager.ProjectPanel.SelectNode(node);
        }

        #endregion
    }
}
