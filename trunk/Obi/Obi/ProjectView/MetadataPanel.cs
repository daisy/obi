using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class MetadataPanel : UserControl
    {
        private MetadataView mView;                     // parent view
        private bool mSelected;                         // selected flag
        private urakawa.metadata.Metadata mEntry;       // corresponding entry
        private MetadataEntryDescription mDescription;  // and corresponding description (may be null for free metadata)

        /// <summary>
        /// Create a new metadata panel.
        /// </summary>
        public MetadataPanel(MetadataView view, urakawa.metadata.Metadata entry): this()
        {
            mView = view;
            mEntry = entry;
            EntryName = entry.getName();
            EntryContent = entry.getContent();
            mSelected = false;
        }

        public MetadataPanel() { InitializeComponent(); }


        /// <summary>
        /// The description of the entry in the panel.
        /// </summary>
        public MetadataEntryDescription Description { get { return mDescription; } }

        /// <summary>
        /// The metadata entry for this panel.
        /// </summary>
        public urakawa.metadata.Metadata Entry { get { return mEntry; } }

        /// <summary>
        /// Name for this panel.
        /// </summary>
        public string EntryName
        {
            get { return mNameComboBox.SelectedItem == null ? mNameComboBox.Text : mNameComboBox.SelectedItem.ToString(); }
            set
            {
                if (MetadataEntryDescription.GetDAISYEntries().ContainsKey(value))
                {
                    mDescription = MetadataEntryDescription.GetDAISYEntries()[value];
                }
                else
                {
                    mDescription = new MetadataEntryDescription(value, MetadataOccurrence.Optional,
                        Localizer.Message("missing_description"), true, false);
                    MetadataEntryDescription.AddCustomEntry(mDescription);
                }
                mNameComboBox.Items.Clear();
                mNameComboBox.Items.Add(mDescription);
                mNameComboBox.SelectedItem = mDescription;
                mNameComboBox.Enabled = mView.CanRemove(mDescription);
            }
        }

        /// <summary>
        /// Content for this panel.
        /// </summary>
        public string EntryContent
        {
            get { return mContentBox.Text; }
            set { mContentBox.Text = value; }
        }

        /// <summary>
        /// Selection status of the panel.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                UpdateColors();
            }
        }


        // Update colors to show selection status
        private void UpdateColors()
        {
            BackColor = mSelected ? Color.Yellow : Parent.BackColor;
        }

        // Click to select a panel
        private void MetadataPanel_Click(object sender, EventArgs e) { mView.SelectedPanel = this; }

        private void mContentBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mView.ModifiedEntryContent(mEntry, mContentBox.Text);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                EntryContent = mEntry.getContent();
            }
        }

        private void mNameComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mView.ModifiedEntryName(mEntry, mNameComboBox.Text);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                EntryName = mEntry.getName();
            }
        }

        private void mNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            mView.ModifiedEntryName(mEntry, mNameComboBox.SelectedItem.ToString());
        }

        private void mNameComboBox_DropDown(object sender, EventArgs e)
        {
            mNameComboBox.Items.Clear();
            foreach (MetadataEntryDescription d in MetadataEntryDescription.GetDAISYEntries().Values)
            {
                if ((d == mDescription) || (!d.ReadOnly && d.Repeatable)) mNameComboBox.Items.Add(d);
            }
        }
    }
}
