using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class StructureItemDialog : Form
    {
        private StructureHead mHeading;

        public string Title { get { return titleBox.Text; } }
        public HeadingLevel Level { get { return (HeadingLevel)levelBox.SelectedItem; } }

        public StructureItemDialog()
        {
            InitializeComponent();
        }

        public StructureItemDialog(StructureHead heading)
        {
            InitializeComponent();
            mHeading = heading;
            titleBox.Text = heading.Label;
            for (uint i = 0; i < mHeading.Level.Level; ++i)
            {
                levelBox.Items.Add(new HeadingLevel(i));
            }
            levelBox.Items.Add(mHeading.Level);
            levelBox.Items.Add(new HeadingLevel(mHeading.Level.Level + 1));
            levelBox.SelectedItem = mHeading.Level;
        }
    }
}