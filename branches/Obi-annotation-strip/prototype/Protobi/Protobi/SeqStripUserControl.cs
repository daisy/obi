using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class SeqStripUserControl : Protobi.StripUserControl
    {
        private ParStripUserControl mParent;

        private int old_pwidth;

        public ParStripUserControl ParentStrip { set { mParent = value; } }

        public SeqStripUserControl()
        {
            InitializeComponent();
            InitializeMembers();
        }

        public SeqStripUserControl(SeqStrip controller)
        {
            InitializeComponent();
            InitializeMembers(controller);
        }

        protected override void sizeHandle_MouseDown(object sender, MouseEventArgs e)
        {
            base.sizeHandle_MouseDown(sender, e);
            old_pwidth = mParent.Width;
        }

        protected override void sizeHandle_MouseUp(object sender, MouseEventArgs e)
        {
            if (old_width != Width)
            {
                ResizeStripCommand resize = new ResizeStripCommand(mController, new Size(old_width, Height), Size);
                ResizeStripCommand parent_resize = new ResizeStripCommand(((SeqStrip)mController).Parent,
                    new Size(old_pwidth, mParent.Height), mParent.Size);
                ((WorkAreaForm)ParentForm).PushUndo(new ConsCommand(resize.Label, resize, parent_resize));
            }
            resizing = false;
        }

        private void SeqStripUserControl_SizeChanged(object sender, EventArgs e)
        {
            if (mParent != null) mParent.ContentsSizeChanged();
        }
    }
}