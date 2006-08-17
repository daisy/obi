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
    /// <summary>
    /// An empty structure block, used as a base class for pages blocks and heading blocks.
    /// </summary>
    public partial class StructureBlock : AbstractBlock
    {
        private AudioBlock mAudioBlock;  // the corresponding audio block

        public AudioBlock AudioBlock
        {
            get { return mAudioBlock; }
            set { mAudioBlock = value; }
        }

        public string Label
        {
            get { return mLabelBox.Text; }
            set { mLabelBox.Text = value; }
        }

        public int _Width
        {
            set { mLabelBox.Size = new Size(value - mLabelBox.Margin.Left - mLabelBox.Margin.Right, mLabelBox.Size.Height); }
        }

        public StructureBlock()
        {
            InitializeComponent();
        }

        internal override void MarkDeselected()
        {
            BackColor = Color.PowderBlue;
            mLabelBox.BackColor = BackColor;
        }

        internal override void MarkSelected()
        {
            BackColor = Color.DeepSkyBlue;
            mLabelBox.BackColor = BackColor;
        }

        /// <summary>
        /// Clicking on the block selects the current phrase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mLabelBox_Click(object sender, EventArgs e)
        {
            mAudioBlock.Manager.SelectedPhraseNode = mAudioBlock.Node;
        }

        private void StructureBlock_Click(object sender, EventArgs e)
        {
            mAudioBlock.Manager.SelectedPhraseNode = mAudioBlock.Node;
        }

        /// <summary>
        /// Start editing the page label.
        /// When pressing return, or leaving the control, the change will be made.
        /// </summary>
        internal void StartEditingPageLabel()
        {
            string text = mLabelBox.Text;
            mLabelBox.Tag = text;  // quick hack to keep the previous value of the text
            mLabelBox.Text = "";
            mLabelBox.SelectedText = text;
            mLabelBox.Cursor = Cursors.IBeam;
            mLabelBox.ReadOnly = false;
            mLabelBox.Focus();
        }

        /// <summary>
        /// Return the label box to read only.
        /// </summary>
        private void StopEditingPageLabel()
        {
            mLabelBox.Cursor = Cursors.Default;
            mLabelBox.ReadOnly = true;
        }

        /// <summary>
        /// Catch return and escape events to signal the end of editing.
        /// </summary>
        private void mLabelBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    UpdateText();
                    break;
                case Keys.Escape:
                    mLabelBox.Text = (string)mLabelBox.Tag;
                    StopEditingPageLabel(); 
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set the page node from the label text.
        /// An empty string will have no effect.
        /// </summary>
        private void UpdateText()
        {
            StopEditingPageLabel();
            if (mLabelBox.Text == null || mLabelBox.Text == "")
            {
                mLabelBox.Text = (string)mLabelBox.Tag;
            }
            else
            {
                mManager.RequestToSetPageLabel(this, new Events.Node.SetPageEventArgs(this, mAudioBlock.Node, mLabelBox.Text));
            }
        }
    }
}
