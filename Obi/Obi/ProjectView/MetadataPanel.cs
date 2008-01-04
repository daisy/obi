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
        private MetadataView mView;  // parent view
        private bool mSelected;      // selected flag

        /// <summary>
        /// Create a new metadata panel.
        /// </summary>
        public MetadataPanel(MetadataView view): this()
        {
            mView = view;
            mNameComboBox.Items.AddRange(MetadataEntryDescription.GetDAISYEntries().ToArray());
            mSelected = false;
        }

        public MetadataPanel() { InitializeComponent(); }


        /// <summary>
        /// Name for this panel.
        /// </summary>
        public string EntryName
        {
            get { return mNameComboBox.SelectedItem == null ? mNameComboBox.Text : mNameComboBox.SelectedItem.ToString(); }
            set
            {
                mNameComboBox.SelectedItem = null;
                foreach (object item in mNameComboBox.Items)
                {
                    if (item.ToString() == value)
                    {
                        mNameComboBox.SelectedItem = item;
                        break;
                    }
                }
                if (mNameComboBox.SelectedItem == null) mNameComboBox.Text = value;
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
    }
}
