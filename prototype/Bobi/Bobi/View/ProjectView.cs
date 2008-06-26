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
        private double audioScale;         // current audio scale (zoom)
        private Clipboard clipboard;       // clipboard
        private AudioBlock playbackBlock;  // block currently playing
        private Project project;           // current project (may be null)
        private Selection selection;       // current selection
        private double zoom;               // current zoom factor

        public event SelectionSetEventHandler SelectionSet;  // selection was set from this control, or below

        private delegate void AddDelegate(urakawa.core.TreeNode node);  // delegate for thread-safe node addition
        private delegate void ClearProjectDelegate();                   // delegate for thread-safe clearing


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
            this.playbackBlock = null;
            AudioScale = 1.0;
            Zoom = 1.0;
        }


        /// <summary>
        /// Project-wide audio scale.
        /// </summary>
        public double AudioScale
        {
            get { return this.audioScale; }
            set
            {
                this.audioScale = value;
                SuspendLayout();
                foreach (Control c in Controls) if (c is Track) ((Track)c).AudioScale = value;
                ResumeLayout();
            }
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
            get { return ((BobiForm)Parent).ColorSettings; }
            set
            {
                BackColor = value.ProjectViewBackColor;
                foreach (Control c in Controls) if (c is Track) ((Track)c).Colors = value;
            }
        }

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
        /// The node currently playing.
        /// </summary>
        public AudioNode PlaybackNode
        {
            set
            {
                if (this.playbackBlock != null) playbackBlock.Playing = false;
                this.playbackBlock = value == null ? null : FindTrack(value).FindBlock(value);
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

        public void ReportPlaybackPosition(double time)
        {
            if (this.playbackBlock != null) this.playbackBlock.PlayingTime = time;
        }

        /// <summary>
        /// Select (or deselect) the control corresponding to the given node.
        /// </summary>
        public void SelectControlForNode(urakawa.core.TreeNode node, Selection selection)
        {
            Track track = FindTrack(node);
            if (node is TrackNode)
            {
                track.Selection = selection;
            }
            else if (node is AudioNode)
            {
                track.FindBlock((AudioNode)node).Selection = selection;
            }
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
            NodeSelection s = new NodeSelection(this);
            if (this.selection != null) this.selection.Deselect(s);
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
            if (this.selection != null) this.selection.Deselect(selection);
            this.selection = selection;
            if (this.selection != null) this.selection.SelectControls();
        }

        /// <summary>
        /// Select a single track from below (i.e. a the track.)
        /// </summary>
        public void SelectFromBelow(urakawa.core.TreeNode node)
        {
            SelectFromBelow(new NodeSelection(this, node));
        }

        /// <summary>
        /// Make a selection from below (i.e. from a track or a block.)
        /// </summary>
        public void SelectFromBelow(Selection selection)
        {
            if (this.selection != selection)
            {
                if (this.selection != null) this.selection.Deselect(selection);
                this.selection = selection;
                if (SelectionSet != null) SelectionSet(this, new SelectionSetEventArgs(this.selection));
            }
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
                SuspendLayout();
                foreach (Control c in Controls) if (c is Track) ((Track)c).Zoom = value;
                ResumeLayout();
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
                SuspendLayout();
                Track t = FindTrack(node);
                t.AddAudioBlock(new AudioBlock((AudioNode)node));
                t.Zoom = this.zoom;
                ResumeLayout();
            }
        }

        // Add a new node (track or audio) to the view
        private void AddNode(urakawa.core.TreeNode node)
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

        // Add a new track to the project (thread-safe)
        private void AddTrack(urakawa.core.TreeNode node)
        {
            if (InvokeRequired)
            {
                Invoke(new AddDelegate(AddTrack), node);
            }
            else
            {
                if (node is TrackNode)
                {
                    SuspendLayout();
                    Track t = new Track((TrackNode)node);
                    t.Zoom = this.zoom;
                    t.Colors = Colors;
                    Controls.Add(t);
                    SetFlowBreak(t, true);
                    ResumeLayout();
                }
            }
        }

        // Clear the project (thread-safe)
        private void ClearProject()
        {
            if (InvokeRequired)
            {
                Invoke(new ClearProjectDelegate(ClearProject));
            }
            else
            {
                SuspendLayout();
                Controls.Clear();
                ResumeLayout();
            }
        }

        // Find the track for a given node
        private Track FindTrack(urakawa.core.TreeNode node)
        {
            urakawa.core.TreeNode actual = node is AudioNode ? node.getParent() : node;
            foreach (Control c in Controls) if (c is Track && ((Track)c).Node == actual) return (Track)c;
            return null;
        }
        
        // React to changes in the project
        private void project_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            System.Diagnostics.Debug.Print(e.ToString());
            if (e is urakawa.events.presentation.RootNodeChangedEventArgs)
            {
                // this is a stub
            }
            else if (e is urakawa.events.core.ChildAddedEventArgs)
            {
                AddNode(((urakawa.events.core.ChildAddedEventArgs)e).AddedChild);
            }
            else if (e is urakawa.events.core.ChildRemovedEventArgs)
            {
                Controls.RemoveAt(((urakawa.events.core.ChildRemovedEventArgs)e).RemovedPosition);
            }
        }

        // Click to deselect.
        private void ProjectView_Click(object sender, EventArgs e)
        {
            if (this.selection != null) this.selection.Deselect(null);
            this.selection = null;
            if (SelectionSet != null) SelectionSet(this, new SelectionSetEventArgs(this.selection));
        }
    }
}
