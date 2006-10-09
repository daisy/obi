using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class MetadataInfoPanel : UserControl
    {
        private Metadata mMetadataController = null;

        public Label Label { get { return label1; } }

        public Metadata MetadataController
        {
            set
            {
                if (mMetadataController == null)
                {
                    mMetadataController = value;
                    SetTitle();
                }
            }
        }

        public MetadataInfoPanel()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (mMetadataController != null)
            {
                mMetadataController.Edit();
                SetTitle();
            }
        }

        private void SetTitle()
        {
            string title = mMetadataController.MetadataModel.DcTitle.IsSet ?
                mMetadataController.MetadataModel.DcTitle.Content : Localizer.GetString("untitled");
            label1.Text = String.Format(Localizer.GetString("current_title"), title);
            BackColor = mMetadataController.MetadataModel.AllSet ?
                Color.FromArgb(255, 128, 255, 128) : Color.FromArgb(255, 255, 255, 128);
        }
    }
}
