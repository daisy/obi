using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class MetadataDialog : Form
    {
        private bool mTitleChanged;
        private bool mPublisherChanged;
        private bool mIdChanged;

        public TextBox TitleBox { get { return titleBox; } }
        public bool DidTitleChange { get { return mTitleChanged; } }
        public TextBox PublisherBox { get { return publisherBox; } }
        public bool DidPublisherChange { get { return mPublisherChanged; } }
        public TextBox DateBox { get { return dateBox; } }
        public TextBox DateEventBox { get { return dateEventBox; } }
        public TextBox IdBox { get { return idBox; } }
        public bool DidIdChange { get { return mIdChanged; } }
        public TextBox IdSchemeBox { get { return idSchemeBox; } }
        public ComboBox LanguageBox { get { return languageBox; } }

        public MetadataDialog()
        {
            InitializeComponent();
            mTitleChanged = false;
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
                languageBox.Items.Add(ci);
        }

        private void titleBox_TextChanged(object sender, EventArgs e)
        {
            mTitleChanged = true;
        }

        private void publisherBox_TextChanged(object sender, EventArgs e)
        {
            mPublisherChanged = true;
        }

        private void idBox_TextChanged(object sender, EventArgs e)
        {
            mIdChanged = true;
        }
    }
}