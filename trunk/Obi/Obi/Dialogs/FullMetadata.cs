using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class FullMetadata : Form
    {
        private Metadata mMetadata;  // metadata object to update

        public FullMetadata()
        {
            InitializeComponent();
        }

        public FullMetadata(Metadata meta)
        {
            InitializeComponent();
            mMetadata = meta;
            RefreshCanAddItems();
        }

        /// <summary>
        /// Refresh the name box for items that can be added given the current metadata object.
        /// </summary>
        private void RefreshCanAddItems()
        {
            mNameBox.Items.Clear();
            foreach (MetadataItem item in mMetadata.CanAdd) mNameBox.Items.Add(item); 
        }

        /// <summary>
        /// Show the description of the currently selected metadata item.
        /// </summary>
        private void mNameBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (mNameBox.SelectedItem != null)
            {
                mDescriptionBox.Text = ((MetadataItem)mNameBox.SelectedItem).Content;
                mContentBox.Enabled = true;
                mAddItemButton.Enabled = true;
            }
            else
            {
                mDescriptionBox.Text = "";
                mContentBox.Enabled = false;
                mAddItemButton.Enabled = false;
            }
        }

        /// <summary>
        /// Add a new metadata item.
        /// </summary>
        private void mAddItemButton_Click(object sender, EventArgs e)
        {
            MetadataItem template = mNameBox.SelectedItem as MetadataItem;
            if (template != null)
            {
                MetadataItem item = new MetadataItem(template, mContentBox.Text);
                try
                {
                    mMetadata.Add(item);
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(mMetadataGrid);
                    row.Tag = item;
                    row.Cells[0].Value = item.Name;
                    row.Cells[1].Value = Localizer.Message(item.Repeatable ? "yes" : "no");
                    row.Cells[2].Value = item.Content;
                    mMetadataGrid.Rows.Add(row);
                }
                catch
                {
                    MessageBox.Show("Augh!");
                }
            }
            AcceptButton = mOKButton;
        }

        /// <summary>
        /// The content of metadata elements is edited in its cell, so update the corresponding item.
        /// </summary>
        private void mMetadataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            MetadataItem item = (MetadataItem)mMetadataGrid.Rows[e.RowIndex].Tag;
            item.Content = (string)mMetadataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            System.Diagnostics.Debug.Print("Modified content of <{0}> at row #{1} to <{2}>.\n",
                item.Name, e.RowIndex, item.Content);
        }

        /// <summary>
        /// Warn the user when deleting a row.
        /// </summary>
        private void mMetadataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            MetadataItem item = (MetadataItem)e.Row.Tag;
            if (item.Occurrence == MetadataOccurrence.Required && mMetadata.DeleteWarning(item))
            {
                e.Cancel = MessageBox.Show(Localizer.Message("really_delete_required_metadata_item_text"),
                    Localizer.Message("really_delete_required_metadata_item_caption"), MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.No;
            }
        }

        /// <summary>
        /// Delete the metadata item corresponding to a deleted row.
        /// </summary>
        private void mMetadataGrid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            mMetadata.Delete((MetadataItem)e.Row.Tag);
            RefreshCanAddItems();
        }

        /// <summary>
        /// Make the "add item" the accept button when we enter a new value.
        /// </summary>
        private void mContentBox_Enter(object sender, EventArgs e)
        {
            AcceptButton = mAddItemButton;
        }

        private void mContentBox_Leave(object sender, EventArgs e)
        {
            AcceptButton = mOKButton;
        }


    }
}