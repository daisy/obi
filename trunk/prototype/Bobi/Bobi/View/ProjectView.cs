using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class ProjectView : FlowLayoutPanel
    {
        private Clipboard clipboard;  // clipboard
        private Project project;      // current project (may be null)
        private Selection selection;  // current selection
        private double zoom;          // current zoom factor

        public event SelectionSetEventHandler SelectionSet;  // selection was set from this control, or below


        /// <summary>
        /// New project view
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Project = null;
            this.clipboard = null;
            this.selection = null;
            Zoom = 1.0;
        }


        /// <summary>
        /// Get the view's clipboard.
        /// </summary>
        public Clipboard Clipboard { get { return this.clipboard; } }

        /// <summary>
        /// Set the colors for a color scheme.
        /// </summary>
        public ColorSettings Colors
        {
            set
            {
                BackColor = value.ProjectViewBackColor;
                foreach (Control c in Controls) if (c is Track) ((Track)c).Colors = value;
            }
        }

        /// <summary>
        /// Color scheme of the parent form.
        /// </summary>
        public ColorSettings ColorSettings { get { return ((BobiForm)Parent).ColorSettings; } }

        /// <summary>
        /// True if there is at least one track that is not selected.
        /// </summary>
        public bool HasUnselectedTrack
        {
            get
            {
                foreach (Control c in Controls) if (c is Track && !((Track)c).Selected) return true;
                return false;
            }
        }

        /// <summary>
        /// The project for this view.
        /// </summary>
        public Project Project
        {
            get { return this.project; }
            set
            {
                ClearProject();
                this.project = value;
                if (value != null)
                {
                    this.project.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(project_changed);
                }
            }
        }

        /// <summary>
        /// Select (or deselect) the control corresponding to the given node.
        /// </summary>
        public void SelectControlForNode(urakawa.core.TreeNode node, bool selected)
        {
            Track track = FindTrack(node);
            if (track != null) track.Selected = selected;
        }

        /// <summary>
        /// Get the current selection; set it using SelectFromBelow or SelectFromTop.
        /// </summary>
        public Selection Selection { get { return this.selection; } }

        /// <summary>
        /// Select all tracks in the view (called from the form.)
        /// </summary>
        public void SelectAllFromAbove()
        {
            if (this.selection != null) this.selection.Deselect();
            this.selection = new NodeSelection(this);
            foreach (Control c in Controls)
            {
                if (c is Track) ((NodeSelection)this.selection).AddNode(((Track)c).Node);
            }
            this.selection.SelectControls();
        }

        /// <summary>
        /// Set a selection from above.
        /// </summary>
        public void SelectFromAbove(Selection selection)
        {
            if (this.selection != null) this.selection.Deselect();
            this.selection = selection;
            if (this.selection != null) this.selection.SelectControls();
        }

        /// <summary>
        /// Select a single track from below (i.e. from the track.)
        /// </summary>
        public void SelectFromBelow(urakawa.core.TreeNode node)
        {
            if (this.selection != null)
            {
                this.selection.Deselect();
            }
            this.selection = new NodeSelection(this, node);
            if (SelectionSet != null) SelectionSet(this, new SelectionSetEventArgs(this.selection));
        }

        /// <summary>
        /// Zoom factor for the view.
        /// </summary>
        public double Zoom
        {
            get { return this.zoom; }
            set
            {
                this.zoom = value;
                foreach (Control c in Controls)
                {
                    if (c is Track) ((Track)c).Zoom = value;
                }
            }
        }


        // Find the track for a given node
        private Track FindTrack(urakawa.core.TreeNode node)
        {
            foreach (Control c in Controls)
            {
                if (c is Track && ((Track)c).Node == node) return (Track)c;
            }
            return null;
        }

        // Let's custom paint (or more acurately, let's not.)
        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }


        private void AddedNode(urakawa.core.TreeNode node)
        {
            if (node is TrackNode)
            {
                AddTrack(node);
            }
            else if (node is AudioNode)
            {
                AddAudioBlock(node);
            }
        }

        // Add a new audio block to the project (thread-safe)
        private void AddAudioBlock(urakawa.core.TreeNode node)
        {
            if (InvokeRequired)
            {
                Invoke(new AddDelegate(AddAudioBlock), node);
            }
            else
            {
                FindTrack(node.getParent()).AddAudioBlock(new AudioBlock((AudioNode)node));
            }
        }

        // Add a new track to the project (thread-safe)
        private void AddTrack(urakawa.core.TreeNode node)
        {
            if (InvokeRequired)
            {
                Invoke(new AddDelegate(AddTrack), node);
            }
            else
            {
                Track t = new Track(node);
                t.Zoom = this.zoom;
                t.Colors = ColorSettings;
                Controls.Add(t);
                SetFlowBreak(t, true);
            }
        }

        private delegate void AddDelegate(urakawa.core.TreeNode node);

        // Clear the project (thread-safe)
        private void ClearProject()
        {
            if (InvokeRequired)
            {
                Invoke(new ClearProjectDelegate(ClearProject));
            }
            else
            {
                Controls.Clear();
            }
        }

        private delegate void ClearProjectDelegate();
        
        // React to changes in the project
        private void project_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            System.Diagnostics.Debug.Print(e.ToString());
            if (e is urakawa.events.presentation.RootNodeChangedEventArgs)
            {
            }
            else if (e is urakawa.events.core.ChildAddedEventArgs)
            {
                AddedNode(((urakawa.events.core.ChildAddedEventArgs)e).AddedChild);
            }
            else if (e is urakawa.events.core.ChildRemovedEventArgs)
            {
                Controls.RemoveAt(((urakawa.events.core.ChildRemovedEventArgs)e).RemovedPosition);
            }
        }

        // Click to deselect.
        private void ProjectView_Click(object sender, EventArgs e)
        {
            if (this.selection != null) this.selection.Deselect();
            this.selection = null;
            if (SelectionSet != null) SelectionSet(this, new SelectionSetEventArgs(this.selection));
        }
    }
}
