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
        private Project mProject;

        public EditSimpleMetadata(Project project)
        {
            InitializeComponent();
            mProject = project;
            mTitleBox.Text = project.GetFirstMetadataItem(Metadata.DC_TITLE).getContent();
            mAuthorBox.Text = project.GetFirstMetadataItem(Metadata.DTB_NARRATOR).getContent();
            mPublisherBox.Text = project.GetFirstMetadataItem(Metadata.DC_PUBLISHER).getContent();
            mIdentiferBox.Text = project.GetFirstMetadataItem(Metadata.DC_IDENTIFIER).getContent();
            foreach (CultureInfo c in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                mLanguageBox.Items.Add(c);
            }
            string language = project.GetFirstMetadataItem(Metadata.DC_LANGUAGE).getContent();
            foreach (Object o in mLanguageBox.Items)
            {
                if (((CultureInfo)o).ToString() == language)
                {
                    mLanguageBox.SelectedItem = o;
                    break;
                }
            }
        }

        private void mOKButton_Click(object sender, EventArgs e)
        {
            mProject.GetFirstMetadataItem(Metadata.DC_TITLE).setContent(mTitleBox.Text);
            mProject.GetFirstMetadataItem(Metadata.DTB_NARRATOR).setContent(mAuthorBox.Text);
            mProject.GetFirstMetadataItem(Metadata.DC_PUBLISHER).setContent(mPublisherBox.Text);
            mProject.GetFirstMetadataItem(Metadata.DC_IDENTIFIER).setContent(mIdentiferBox.Text);
            mProject.GetFirstMetadataItem(Metadata.DC_LANGUAGE).setContent(((CultureInfo)mLanguageBox.SelectedItem).ToString());
        }
    }
}