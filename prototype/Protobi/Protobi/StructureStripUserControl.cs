using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class StructureStripUserControl : Protobi.SeqStripUserControl
    {
        public StructureStripUserControl()
        {
            InitializeComponent();
            InitializeMembers();
        }

        public StructureStripUserControl(StructureStrip controller)
        {
            InitializeComponent();
            InitializeMembers(controller);
        }

        /// <summary>
        /// Resize the strip so that all of its contents can be shown.
        /// </summary>
        public override void ContentsSizeChanged()
        {
            mMinSize.Width = MinimumSize.Width + (label.Width > headingLabel.Width ? label.Width : headingLabel.Width);
            mMinSize.Height = MinimumSize.Height + headingLabel.Height + headingLabel.Margin.Bottom;
            // Grow horizontally when necessary
            if (Width < mMinSize.Width) Width = mMinSize.Width;
            if (Height != mMinSize.Height)
            {
                Height = mMinSize.Height;
                select_path = LeftPath(selectHandle.Width);
                size_path = RightPath(sizeHandle.Width);
            }
        }

        private void headingLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditHeading();
        }

        public void EditHeading()
        {
            StructureItemDialog dialog = new StructureItemDialog(((StructureStrip)mController).Heading);
            string label = ((StructureStrip)mController).Heading.Label;
            uint level = ((StructureStrip)mController).Heading.Level.Level;
            dialog.ShowDialog();
            if (dialog.DialogResult == DialogResult.OK)
            {
                ((StructureStrip)mController).Heading.Label = dialog.Title;
                ((StructureStrip)mController).Heading.Level.Level = dialog.Level.Level;
                UpdateHeading(dialog.Title);
                ((WorkAreaForm)ParentForm).PushUndo(new EditHeadingCommand(((StructureStrip)mController).Heading, label, level,
                    this));
            }
        }

        public void UpdateHeading(string label)
        {
            headingLabel.Text = label;
            ContentsSizeChanged();
        }
    }
}

