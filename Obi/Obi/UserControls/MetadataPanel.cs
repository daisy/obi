using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class MetadataPanel : UserControl
    {
        public delegate void MetadataPanelDeletedHandler(object sender, EventArgs e);
        public event MetadataPanelDeletedHandler Deleted; 

        private bool mEditable;

        public MetadataPanel()
        {
            InitializeComponent();
            mNameBox.Items.AddRange(MetadataEntryDescription.GetDAISYEntries().ToArray());
            Editable = true;
        }

        public bool Editable
        {
            get { return mEditable; }
            set
            {
                if (mEditable != value)
                {
                    mEditable = value;
                    mNameBox.Enabled = value;
                }
            }
        }

        public string EntryName
        {
            get { return mNameBox.SelectedItem.ToString(); }
            set
            {
                foreach (object item in mNameBox.Items)
                {
                    if (item.ToString() == value)
                    {
                        mNameBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }
        
        public string EntryContent
        {
            get { return mContentBox.Text; }
            set { mContentBox.Text = value; }
        }

        private void mEditButton_Click(object sender, EventArgs e)
        {
            Editable = !mEditable;
        }

        private void mDeleteButton_Click(object sender, EventArgs e)
        {
            if (Deleted != null) Deleted(this, new EventArgs());
        }

        private void mNameBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Editable = false;
        }
    }
}
