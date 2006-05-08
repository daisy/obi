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
        public MetadataPanel()
        {
            InitializeComponent();
        }

        private void MetadataPanel_Load(object sender, EventArgs e)
        {
            titleBox.BackColor = BackColor;
            statusBox.BackColor = BackColor;
        }

        private void statusBox_Enter(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.BackColor = BackColor;
            box.ForeColor = Color.Black;
        }
    }
}
