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
        private Project project;
        private double zoom;

        /// <summary>
        /// New project view
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Project = null;
            Zoom = 1.0;
        }


        /// <summary>
        /// Set the project for this view.
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
        /// Zoom factor for the view
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


        // Let's custom paint
        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }


        // Add a new track to the project (thread-safe)
        private void AddTrack()
        {
            if (InvokeRequired)
            {
                Invoke(new AddTrackDelegate(AddTrack));
            }
            else
            {
                Track t = new Track();
                t.Zoom = this.zoom;
                Controls.Add(t);
                SetFlowBreak(t, true);
            }
        }

        private delegate void AddTrackDelegate();

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
                AddTrack();
            }
            else if (e is urakawa.events.core.ChildRemovedEventArgs)
            {
                Controls.RemoveAt(((urakawa.events.core.ChildRemovedEventArgs)e).RemovedPosition);
            }
        }
    }
}
