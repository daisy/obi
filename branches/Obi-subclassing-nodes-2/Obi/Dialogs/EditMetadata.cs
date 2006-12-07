using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class EditMetadata : Form
    {
        public Metadata mMetadata;

        public EditMetadata(Metadata metadata)
        {
            InitializeComponent();
            mMetadata = metadata;
            mIdentifierBox.Text = mMetadata.Id;
            mTitleBox.Text = mMetadata.Title;
            for (int i = 1; i < mMetadata.Titles.Count; ++i)
            {
                DcMetaSlice slice = AddAlternateTitleSlice();
                slice.Value = mMetadata.Titles[i];
            }
        }

        private void mOkButton_Click(object sender, EventArgs e)
        {
            mMetadata.Id = mIdentifierBox.Text;
            mMetadata.Titles.Clear();
            mMetadata.Titles.Add(mTitleBox.Text);
            foreach (Control c in mTitleLayoutPanel.Controls)
            {
                mMetadata.Titles.Add(((DcMetaSlice)c).Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddAlternateTitleSlice();
        }

        private DcMetaSlice AddAlternateTitleSlice()
        {
            DcMetaSlice slice = new DcMetaSlice("Alternate title:");
            slice.Button.Text = "-";
            slice.Button.Click +=
                new EventHandler(delegate(object sender_, EventArgs e_) { mTitleLayoutPanel.Controls.Remove(slice); });
            mTitleLayoutPanel.Controls.Add(slice);
            return slice;
        }
    }
}