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
        private List<urakawa.metadata.Metadata> mMetadataNodeList = new List<urakawa.metadata.Metadata>();

        public MetadataView()
        {
            InitializeComponent();
            mView = null;
            mSelection = null;
            AddListViewColumns();
        }

        private void AddListViewColumns()
        {
            mMetadataListView.Columns.Add("Meta data Name", 100, HorizontalAlignment.Left);
            mMetadataListView.Columns.Add("Content", 100, HorizontalAlignment.Left);
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
                AddMetadataToList();
                /*
                foreach (urakawa.metadata.Metadata m in mView.Presentation.getListOfMetadata())
                {
                    MetadataPanel panel = new MetadataPanel(this, m);
                    mLayout.Controls.Add(panel);
                }
                 */ 
                UpdatePanelSizes();
                mView.Presentation.MetadataEntryAdded += new MetadataEventHandler(Presentation_MetadataEntryAdded);
                mView.Presentation.MetadataEntryDeleted += new MetadataEventHandler(Presentation_MetadataEntryDeleted);
                mView.Presentation.MetadataEntryContentChanged += new MetadataEventHandler(Presentation_MetadataEntryContentChanged);
                mView.Presentation.MetadataEntryNameChanged += new MetadataEventHandler(Presentation_MetadataEntryNameChanged);
            }
        }

        private void AddMetadataToList()
        {
            string[] MetadataString = new string[2];
            ListViewItem Item = null;
            List<string> MetaDataNameCopy = new List<string>();
            
            foreach (urakawa.metadata.Metadata m in mView.Presentation.getListOfMetadata())
            {
                MetadataString[0] = m.getName();
                MetaDataNameCopy.Add ( m.getName() );
                MetadataString[1] = m.getContent();
                Item = new ListViewItem(MetadataString);
                mMetadataListView.Items.Add(Item);
                Item.Checked = true;
                mMetadataNodeList.Add(m);
                            }
            
            foreach (string name in mView.AddableMetadataNames)
            {
                if (!MetaDataNameCopy.Contains(name))
                {
                    MetadataString[0] = name;
                    MetadataString[1] = "";
                    Item = new ListViewItem(MetadataString);
                    mMetadataListView.Items.Add(Item);
                }
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
            //if (entry.getContent() != content) 
            
            {
                mView.Presentation.getUndoRedoManager().execute(new Commands.Metadata.ModifyContent(mView, entry, content));
            }
        }

        public void ModifiedEntryName(urakawa.metadata.Metadata entry, string name)
        {
            if (entry.getName() != name)
            {
                mView.Presentation.getUndoRedoManager().execute(new Commands.Metadata.ModifyName(mView, entry, name));
            }
        }
        
        protected override bool ProcessDialogKey(Keys KeyData)
        {
            if (KeyData == Keys.Tab
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, true, true, true, true);
                if (this.ActiveControl != null && c.TabIndex > this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else if (KeyData == (Keys)(Keys.Shift | Keys.Tab)
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, false, true, true, true);
                if (this.ActiveControl != null && c.TabIndex < this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else
                return base.ProcessDialogKey(KeyData);
        }

        private void mMetadataListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ItemSelectionAction();
        }

        private void mMetadataListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ItemSelectionAction();
        }

        private void ItemSelectionAction ()
        {
                        if (mMetadataListView.SelectedIndices.Count > 0)
            {
                                int FocussedIndex   = mMetadataListView.Items.IndexOf(mMetadataListView.FocusedItem);
                
            if (mMetadataListView.Items[FocussedIndex].Checked)
            {
                mMetadataTextbox.Enabled = true;
                mCommitButton.Enabled = true;
            }
            else
            {
                mMetadataTextbox.Enabled = false;
                mCommitButton.Enabled = false;
            }
            
            mMetadataEntryTextbox.Text = mMetadataListView.Items[FocussedIndex].SubItems[0].Text + ":" ;
            mMetadataEntryTextbox.TabStop = false;
            mMetadataEntryTextbox.ReadOnly = true;
            mMetadataTextbox.AccessibleName = mMetadataEntryTextbox.Text;
                                                            } // index check ends
else
            {
                // nothing selected
                mMetadataEntryTextbox.Text = "Metadata :";
                                mMetadataTextbox.AccessibleName = mMetadataEntryTextbox.Text;
                mMetadataTextbox.Enabled = false;
        }
}

        private void mAddNewButton_Click(object sender, EventArgs e)
        {
                        mMetadataTextbox.Visible = true;
                        mMetadataTextbox.AccessibleName = "Metadata Content:";
                        mMetadataEntryTextbox.TabStop = true;
                        mMetadataEntryTextbox.ReadOnly = false;
                        mMetadataEntryTextbox.AccessibleName = "Type Metadata entry:";
                        mMetadataEntryTextbox.Focus();
        }

        private void mCommitButton_Click(object sender, EventArgs e)
        {
            mMetadataEntryTextbox.Visible = true;
            mMetadataEntryTextbox.TabStop = false;
            mMetadataEntryTextbox.ReadOnly = true;
        }


    }
}
