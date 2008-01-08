using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class MetadataView : UserControl, IControlWithSelection
    {
        private ProjectView mView;         // parent project view
        private NodeSelection mSelection;  // current selection

        public MetadataView()
        {
            InitializeComponent();
            mView = null;
            mSelection = null;
        }


        /// <summary>
        /// The project view is showing a new presentation;
        /// reset the list of metadata panels.
        /// </summary>
        public void NewPresentation()
        {
            mLayout.Controls.Clear();
            if (mView.Presentation != null)
            {
                foreach (urakawa.metadata.Metadata m in mView.Presentation.getListOfMetadata())
                {
                    MetadataPanel panel = new MetadataPanel(this, m);
                    mLayout.Controls.Add(panel);
                }
                UpdatePanelSizes();
                mView.Presentation.MetadataEntryAdded += new MetadataEventHandler(Presentation_MetadataEntryAdded);
                mView.Presentation.MetadataEntryDeleted += new MetadataEventHandler(Presentation_MetadataEntryDeleted);
            }
        }

        /// <summary>
        /// A metadata entry can be removed if it is selected. TODO: check that it is not mandatory!
        /// </summary>
        public bool CanRemoveMetadata { get { return mSelection != null; } }


        // Add a new entry to the view
        private void Presentation_MetadataEntryAdded(object sender, MetadataEventArgs e)
        {
            MetadataPanel panel = new MetadataPanel(this, e.Entry);
            mLayout.Controls.Add(panel);
        }

        // Remove an entry from the view
        private void Presentation_MetadataEntryDeleted(object sender, MetadataEventArgs e)
        {
            Control panel = null;
            foreach (Control c in mLayout.Controls)
            {
                if (c is MetadataPanel && ((MetadataPanel)c).Entry == e.Entry)
                {
                    panel = c;
                    break;
                }
            }
            if (panel != null) mLayout.Controls.Remove(panel);
        }

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
            }
        }

        public MetadataPanel SelectedPanel
        {
            get { return mSelection == null ? null : ((MetadataSelection)mSelection).Panel; }
            set { mView.Selection = new MetadataSelection(mView.Presentation.RootNode, this, value); }
        }

        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (mSelection != null) ((MetadataSelection)mSelection).Panel.Selected = false;
                mSelection = value as MetadataSelection;
                if (mSelection != null) ((MetadataSelection)mSelection).Panel.Selected = true;
            }
        }


        private void mLayout_SizeChanged(object sender, EventArgs e) { UpdatePanelSizes(); }

        private void UpdatePanelSizes()
        {
            if (mLayout.Controls.Count > 0)
            {
                Control last = mLayout.Controls[mLayout.Controls.Count - 1];
                int w = mLayout.Width - (last.Location.Y + last.Height > Height ? SystemInformation.VerticalScrollBarWidth : 0);
                foreach (Control c in mLayout.Controls) c.Size = new Size(w, c.Height);
            }
        }
    }
}
