using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.UserControls
{
    public partial class SectionStrip : UserControl
    {
        private StripManagerPanel mManager;  // the manager for this strip
        private CoreNode mNode;              // the core node for this strip

        public string Label
        {
            get
            {
                return mLabel.Text;
            }
            set
            {
                mLabel.Text = value;
            }
        }

        public StripManagerPanel Manager
        {
            set
            {
                mManager = value;
            }
        }

        public CoreNode Node
        {
            set
            {
                mNode = value;
            }
        }

        public SectionStrip()
        {
            InitializeComponent();
        }

        private void SectionStrip_Click(object sender, EventArgs e)
        {
            mManager.SelectedNode = mNode;
            Console.WriteLine("Selected {0}", mLabel);
        }

        public void MarkSelected()
        {
            BackColor = Color.Gold;
            Console.WriteLine("{0} becomes gold!", mLabel);
        }

        public void MarkDeselected()
        {
            BackColor = Color.PaleGreen;
            Console.WriteLine("{0} becomes green!", mLabel);
        }
    }
}
