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

    public abstract class SingleNodeSelection : Selection
    {
        private View.ProjectView view;  // view in which the selection is made

        /// <summary>
        /// Create an empty selection.
        /// </summary>
        public SingleNodeSelection(View.ProjectView view)
        {
            this.view = view; 
        }

        /// <summary>
        /// Get the view in which the selection is made.
        /// </summary>
        public override View.ProjectView View { get { return this.view; } }
    }

    public class NodeSelection: SingleNodeSelection
    {
        protected List<urakawa.core.TreeNode> nodes;  // list of selected nodes

        /// <summary>
        /// Create an empty selection.
        /// </summary>
        public NodeSelection(View.ProjectView view): base(view)
        {
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
            foreach (urakawa.core.TreeNode node in this.nodes) View.SelectControlForNode(node, false);
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
            foreach (urakawa.core.TreeNode node in this.nodes) View.SelectControlForNode(node, true);
        }

        /// <summary>
        /// Get the only selected node. Throw an exception if there is no or more than one node selected.
        /// </summary>
        public urakawa.core.TreeNode SingleNode
        {
            get
            {
                if (nodes.Count != 1) throw new Exception(string.Format("Expected a single node in selection, but got {0}.", nodes.Count));
                return nodes[0];
            }
        }
    }


    public class AudioSelection : SingleNodeSelection
    {
        private AudioNode node;  // the audio node that has a selection
        private bool isRange;    // true for range (from .. to), false for simple selection (from == to)
        private double from;     // begin of the range, or selection point
        private double to;       // end of the range, or selection point


        /// <summary>
        /// Create a new audio selection for an audio node from a time (in ms) to another time.
        /// </summary>
        public AudioSelection(View.ProjectView view, AudioNode node, double from, double to)
            : base(view)
        {
            this.node = node;
            this.isRange = true;
            this.from = from;
            this.to = to;
            EnsureTimesWithinRange();
        }

        public AudioSelection(View.ProjectView view, AudioNode node, double at)
            : base(view)
        {
            this.node = node;
            At = at;
        }

        public double At
        {
            get { return this.from; }
            set
            {
                this.isRange = false;
                this.from = value;
                this.to = value;
                EnsureTimesWithinRange();
            }
        }

        public override void Deselect()
        {
            View.SelectControlForNode(node, false);
        }

        public double From
        {
            get { return this.from; }
            set
            {
                this.isRange = true;
                this.from = value;
                EnsureTimesWithinRange();
            }
        }

        public bool IsRange { get { return this.isRange; } }

        public override int ItemsInSelection { get { return 1; } }

        public override void SelectControls()
        {
            View.SelectControlForNode(node, true);
        }

        /// <summary>
        /// End time of the range.
        /// </summary>
        public double To
        {
            get { return this.to; }
            set
            {
                this.isRange = true;
                this.to = value;
                EnsureTimesWithinRange();
            }
        }



        // Ensure that both times are within the range of the audio of the node
        private void EnsureTimesWithinRange()
        {
            if (this.from > this.to)
            {
                double t = this.to;
                this.to = this.from;
                this.from = t;
            }
            if (this.from < 0.0) this.from = 0.0;
            if (node != null && node.Audio != null && this.to > node.Audio.getAudioDuration().getTimeDeltaAsMillisecondFloat())
            {
                this.to = node.Audio.getAudioDuration().getTimeDeltaAsMillisecondFloat();
            }
        }
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
