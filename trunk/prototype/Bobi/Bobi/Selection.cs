using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi
{
    public abstract class Selection
    {
        public abstract View.ProjectView View { get; }
        public abstract void Deselect();
        public abstract void SelectControls();
    }

    public class NodeSelection: Selection
    {
        private View.ProjectView view;
        private List<urakawa.core.TreeNode> nodes;

        /// <summary>
        /// Create an empty selection.
        /// </summary>
        public NodeSelection(View.ProjectView view)
        {
            this.view = view; 
            this.nodes = new List<urakawa.core.TreeNode>();
        }

        /// <summary>
        /// Create a single node selection.
        /// </summary>
        public NodeSelection(View.ProjectView view, urakawa.core.TreeNode node): this(view)
        {
            AddNode(node);
        }


        /// <summary>
        /// Add a node to the selection.
        /// </summary>
        public void AddNode(urakawa.core.TreeNode node)
        {
            this.nodes.Add(node);
        }

        /// <summary>
        /// Deselect all selected tracks.
        /// </summary>
        public override void Deselect()
        {
            foreach (urakawa.core.TreeNode node in this.nodes) this.view.SelectControlForNode(node, false);
        }

        public override void SelectControls()
        {
            foreach (urakawa.core.TreeNode node in this.nodes) this.view.SelectControlForNode(node, true);
        }

        /// <summary>
        /// Get the view in which the selection is made.
        /// </summary>
        public override View.ProjectView View { get { return this.view; } }
    }
}
