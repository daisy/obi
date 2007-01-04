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
        public FullMetadata()
        {
            InitializeComponent();
        }

        public FullMetadata(Metadata meta)
        {
            InitializeComponent();
            foreach (MetadataItem item in meta.Templates)
            {
                mNameBox.Items.Add(item);
            }
        }

        private void mNameBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (mNameBox.SelectedItem != null)
            {
                mDescriptionBox.Text = ((MetadataItem)mNameBox.SelectedItem).Content;
                mAddItemButton.Enabled = true;
            }
            else
            {
                mDescriptionBox.Text = "";
                mAddItemButton.Enabled = false;
            }
        }

        private void mAddItemButton_Click(object sender, EventArgs e)
        {
            if (mNameBox.SelectedItem != null)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(mMetadataGrid);
                row.Cells[0].Value = mNameBox.Text;
                row.Cells[1].Value = mContentBox.Text;
                mMetadataGrid.Rows.Add(row);
            }
        }
    }
}