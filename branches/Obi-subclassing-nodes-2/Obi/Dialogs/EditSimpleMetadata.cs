using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class EditSimpleMetadata : Form
    {
        private SimpleMetadata mMetadata;

        public EditSimpleMetadata(SimpleMetadata metadata)
        {
            InitializeComponent();
            mMetadata = metadata;
            mTitleBox.Text = mMetadata.Title;
            mAuthorBox.Text = mMetadata.Author;
            mPublisherBox.Text = mMetadata.Publisher;
            mIdentiferBox.Text = mMetadata.Identifier;
            foreach (CultureInfo c in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                mLanguageBox.Items.Add(c);
            }
            mLanguageBox.SelectedItem = mMetadata.Language;
        }

        private void mOKButton_Click(object sender, EventArgs e)
        {
            mMetadata.Title = mTitleBox.Text;
            mMetadata.Author = mAuthorBox.Text;
            mMetadata.Publisher = mPublisherBox.Text;
            mMetadata.Identifier = mIdentiferBox.Text;
            mMetadata.Language = (CultureInfo)mLanguageBox.SelectedItem;
        }
    }
}