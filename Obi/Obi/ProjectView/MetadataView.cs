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
                mView.Presentation.MetadataEntryContentChanged += new MetadataEventHandler(Presentation_MetadataEntryContentChanged);
                mView.Presentation.MetadataEntryNameChanged += new MetadataEventHandler(Presentation_MetadataEntryNameChanged);
            }
        }

        /// <summary>
        /// A new entry of the given kind can be added if this is not readonly, and if it is repeatable
        /// or there is not yet any occurrence of it.
        /// </summary>
        public bool CanAdd(MetadataEntryDescription d)
        {
            return !d.ReadOnly && (d.Repeatable || mView.Presentation.getListOfMetadata(d.Name).Count == 0); 
        }

        /// <summary>
        /// A particular entry can be removed if it is not readonly and not the only occurrence in case of a required entry. 
        /// </summary>
        public bool CanRemove(MetadataEntryDescription d)
        {
            return !d.ReadOnly && (d.Occurrence != MetadataOccurrence.Required || mView.Presentation.getListOfMetadata(d.Name).Count > 1);
        }

        /// <summary>
        /// A metadata entry can be removed if it is selected. TODO: check that it is not mandatory!
        /// </summary>
        public bool CanRemoveMetadata
        {
            get
            {
                return mSelection != null && CanRemove(((MetadataSelection)mSelection).Panel.Description);
            }
        }

        // Add a new entry to the view
        private void Presentation_MetadataEntryAdded(object sender, MetadataEventArgs e)
        {
            MetadataPanel panel = new MetadataPanel(this, e.Entry);
            mLayout.Controls.Add(panel);
        }

        // Modify the content of an entry
        private void Presentation_MetadataEntryContentChanged(object sender, MetadataEventArgs e)
        {
            FindPanel(e.Entry).EntryContent = e.Entry.getContent();
        }

        // The name of a metadata entry changed.
        void Presentation_MetadataEntryNameChanged(object sender, MetadataEventArgs e)
        {
            FindPanel(e.Entry).EntryName = e.Entry.getName();
        }

        // Remove an entry from the view
        private void Presentation_MetadataEntryDeleted(object sender, MetadataEventArgs e)
        {
            mLayout.Controls.Remove(FindPanel(e.Entry) as Control);
        }

        private MetadataPanel FindPanel(urakawa.metadata.Metadata entry)
        {
            foreach (Control c in mLayout.Controls)
            {
                if (c is MetadataPanel && ((MetadataPanel)c).Entry == entry) return (MetadataPanel)c;
            }
            return null;
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

        /// <summary>
        /// Called from panels to modify the content of an entry
        /// </summary>
        public void ModifiedEntryContent(urakawa.metadata.Metadata entry, string content)
        {
            if (entry.getContent() != content) 
            {
                mView.Presentation.UndoRedoManager.execute(new Commands.Metadata.ModifyContent(mView, entry, content));
            }
        }

        public void ModifiedEntryName(urakawa.metadata.Metadata entry, string name)
        {
            if (entry.getName() != name)
            {
                mView.Presentation.UndoRedoManager.execute(new Commands.Metadata.ModifyName(mView, entry, name));
            }
        }
    }
}
