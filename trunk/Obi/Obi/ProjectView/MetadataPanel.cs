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
                mNameComboBox.AccessibleName = mDescription.Name;
            }
        }

        /// <summary>
        /// Content for this panel.
        /// </summary>
        public string EntryContent
        {
            get { return mContentBox.Text; }
            set
            {
                mContentBox.Text = value;
                mContentBox.AccessibleName = value;
            }
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

        private void mContentBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateEntryContent();
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
                if (mView.CanRemove(mDescription))
                {
                    UpdateEntryName();
                }
                else
                {
                    EntryName = mDescription.Name;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                EntryName = mDescription.Name;
            }
        }

        private void mNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (mView.CanRemove(mDescription))
            {
                UpdateEntryName();
            }
            else
            {
                EntryName = mDescription.Name;
            }
        }

        private void UpdateEntryName()
        {
            mNameComboBox.AccessibleName = mNameComboBox.Text;
            mView.ModifiedEntryName(mEntry, mNameComboBox.Text);
        }

        private void UpdateEntryContent()
        {
            mContentBox.AccessibleName = mContentBox.Text;
            mView.ModifiedEntryContent(mEntry, mContentBox.Text);
        }

        private void mNameComboBox_DropDown(object sender, EventArgs e)
        {
            mNameComboBox.Items.Clear();
            if (mView.CanRemove(mDescription))
            {
                List<MetadataEntryDescription> descriptions = new List<MetadataEntryDescription>();
                foreach (MetadataEntryDescription d in MetadataEntryDescription.GetDAISYEntries().Values)
                {
                    if ((d == mDescription) || (!d.ReadOnly && d.Repeatable)) descriptions.Add(d);
                }
                descriptions.Sort(delegate(MetadataEntryDescription d, MetadataEntryDescription d_) { return d.Name.CompareTo(d_.Name); });
                mNameComboBox.Items.AddRange(descriptions.ToArray());
            }
            else
            {
                mNameComboBox.Items.Add(mDescription);
            }
        }
    }
}
