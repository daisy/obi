using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;
using urakawa.undo;

namespace Zaboom.UserControls
{
    /// <summary>
    /// Panel displaying the project, i.e. blocks.
    /// The project panel also manages selections.
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private Project project;                                     // the project to display
        private Commands.CommandManager commandManager;              // command manager for the project
        private Dictionary<urakawa.core.TreeNode, Control> nodeMap;  // find controls for nodes
        private List<Selectable> selected;                           // currently selected
        private int pixelsPerSecond;                                 // project-wide size of the waveform display

        /// <summary>
        /// This event is raised when the selection changes.
        /// </summary>
        public SelectionChangedHandler SelectionChanged;

        /// <summary>
        /// Create a blank project panel (no project yet.)
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            project = null;
            commandManager = new Commands.CommandManager(this);
            nodeMap = new Dictionary<urakawa.core.TreeNode, Control>();
            selected = new List<Selectable>();
            pixelsPerSecond = WaveformPanel.DEFAULT_PIXELS_PER_SECOND;
            transportBar.Enabled = false;
        }

        /// <summary>
        /// Get the command manager of this project panel.
        /// </summary>
        public Commands.CommandManager CommandManager { get { return commandManager; } }

        /// <summary>
        /// Deselect all selected items.
        /// </summary>
        public void Deselect()
        {
            DeselectWithoutEvent();
            if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// Project-wide size of the waveform
        /// </summary>
        // TODO: move to samples per pixel, or have a float for lower numbers
        public int PixelsPerSecond
        {
            get { return pixelsPerSecond; }
            set
            {
                pixelsPerSecond = value;
                foreach (Control c in flowLayout.Controls)
                {
                    AudioBlock block = c as AudioBlock;
                    if (block != null) block.PixelsPerSecond = pixelsPerSecond;
                }
            }
        }

        /// <summary>
        /// Get the current project.
        /// Set the current project, only after a blank project was created (cannot set it again.)
        /// </summary>
        // TODO: close project
        public Project Project
        {
            get { return project; }
            set
            {
                if (project == null && value != null)
                {
                    project = value;
                    project.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(project_treeNodeAdded);
                    project.getPresentation().treeNodeRemoved += new TreeNodeRemovedEventHandler(project_treeNodeRemoved);
                    DummyBlock dummy = new DummyBlock();
                    dummy.Panel = this;
                    flowLayout.Controls.Add(dummy);
                }
            }
        }

        /// <summary>
        /// The selection is replaced from a single selectable element.
        /// </summary>
        public void ReplaceSelection(UserControls.Selectable s)
        {
            DeselectWithoutEvent();
            selected.Add(s);
            if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// Get the list of currently selected items.
        /// </summary>
        public List<Selectable> Selected { get { return selected; } }

        /// <summary>
        /// The selection is replaced from a list of selectable elements. The corresponding selectable items are selected.
        /// </summary>
        public void SelectList(List<UserControls.Selectable> ss)
        {
            DeselectWithoutEvent();
            foreach (UserControls.Selectable s in ss)
            {
                selected.Add(s);
                s.Selected = true;
            }
            if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// The selection is replaced from the project. The corresponding selectable item is selected.
        /// </summary>
        public void SelectNode(urakawa.core.TreeNode node)
        {
            if (!nodeMap.ContainsKey(node)) throw new Exception("No control for node!");
            Selectable s = nodeMap[node] as Selectable;
            if (s == null) throw new Exception("Control is not selectable!");
            ReplaceSelection(s);
            s.Selected = true;
        }

        /// <summary>
        /// An item's selected status was modified.
        /// </summary>
        public void ModifiedSelection(Selectable s)
        {
            if (s.Selected)
            {
                selected.Add(s);
            }
            else
            {
                selected.Remove(s);
            }
            if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// Deselect all selected items without sending an event.
        /// All previously selected items are notified of their deselection.
        /// </summary>
        private void DeselectWithoutEvent()
        {
            foreach (UserControls.Selectable s in selected) s.Selected = false;
            selected.Clear();
        }

        /// <summary>
        /// Clicking outside of a control deselects everything.
        /// </summary>
        private void flowLayout_Click(object sender, EventArgs e)
        {
            Deselect();
        }

        /// <summary>
        /// Handle addition of a node.
        /// </summary>
        private void project_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            AudioBlock block = new AudioBlock();
            block.Panel = this;
            block.Project = project;
            block.Node = e.getTreeNode();
            block.PixelsPerSecond = pixelsPerSecond;
            flowLayout.Controls.Add(block);
            flowLayout.Controls.SetChildIndex(block, e.getTreeNode().getParent().indexOf(e.getTreeNode()));
            nodeMap[e.getTreeNode()] = block;
        }

        /// <summary>
        /// Handle deletion of a node.
        /// </summary>
        private void project_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            flowLayout.Controls.Remove(nodeMap[e.getTreeNode()]);
            nodeMap.Remove(e.getTreeNode());
        }
    }

    public delegate void SelectionChangedHandler(ProjectPanel sender, EventArgs e);
}
