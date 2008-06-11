using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi
{
    public abstract class Selection
    {
        public abstract View.ProjectView View { get; }
        public abstract void Deselect();
        public abstract int ItemsInSelection { get; }
        public abstract void SelectControls();
    }

    public class SelectionSetEventArgs
    {
        public Selection Selection;
        public SelectionSetEventArgs(Selection selection) { Selection = selection; }
    }

    public delegate void SelectionSetEventHandler(object sender, SelectionSetEventArgs e);

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

        /// <summary>
        /// Number of items (nodes) in the selection.
        /// </summary>
        public override int ItemsInSelection { get { return this.nodes.Count; } }

        /// <summary>
        /// Set the selection flag for all items in the selection.
        /// </summary>
        public override void SelectControls()
        {
            foreach (urakawa.core.TreeNode node in this.nodes) this.view.SelectControlForNode(node, true);
        }

        /// <summary>
        /// Get the view in which the selection is made.
        /// </summary>
        public override View.ProjectView View { get { return this.view; } }
    }

    public abstract class Clipboard
    {
        public abstract bool CanPaste(Selection selection);
    }

    public class NodeClipboard: Clipboard
    {
        private List<urakawa.core.TreeNode> nodes;

        public NodeClipboard()
        {
            nodes = new List<urakawa.core.TreeNode>();
        }

        public override bool CanPaste(Selection selection)
        {
            return false;
        }
    }
}
