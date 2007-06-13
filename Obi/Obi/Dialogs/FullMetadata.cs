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

        public List<UserControls.MetadataPanel> MetadataPanels
        {
            get
            {
                List<UserControls.MetadataPanel> panels = new List<UserControls.MetadataPanel>();
                foreach (Control c in mMetadataPanels.Controls)
                {
                    if (c is UserControls.MetadataPanel) panels.Add((UserControls.MetadataPanel)c);
                }
                return panels;
            }
        }

        public void AddPanel(string name, string content)
        {
            UserControls.MetadataPanel panel = AddPanel();
            panel.EntryName = name;
            panel.EntryContent = content;
            panel.Editable = false;
        }

        private void mAddButton_Click(object sender, EventArgs e)
        {
            AddPanel();
        }

        private UserControls.MetadataPanel AddPanel()
        {
            UserControls.MetadataPanel panel = new UserControls.MetadataPanel();
            mMetadataPanels.Controls.Add(panel);
            panel.Deleted += new Obi.UserControls.MetadataPanel.MetadataPanelDeletedHandler(panel_Deleted);
            panel.Focus();
            return panel;
        }

        void panel_Deleted(object sender, EventArgs e)
        {
            UserControls.MetadataPanel panel = (UserControls.MetadataPanel)sender;
            mMetadataPanels.Controls.Remove(panel);
            panel.Deleted -= new UserControls.MetadataPanel.MetadataPanelDeletedHandler(panel_Deleted);
        }

        private void mMetadataPanels_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in mMetadataPanels.Controls)
            {
                c.Width = mMetadataPanels.Width;
            }
        }
    }
}