using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

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
            get { return mLabel.Text; }
            set { mLabel.Text = value; }
        }

        public int _Width
        {
            set { Size = new Size(value, Size.Height); }
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
        private void mLabelBox_Click(object sender, EventArgs e)
        {
            mAudioBlock.Manager.SelectedPhraseNode = (PhraseNode)mAudioBlock.Node;
        }

        private void StructureBlock_Click(object sender, EventArgs e)
        {
            mAudioBlock.Manager.SelectedPhraseNode = (PhraseNode)mAudioBlock.Node;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            mAudioBlock.Manager.SelectedPhraseNode = (PhraseNode)mAudioBlock.Node;
        }

        /// <summary>
        /// Start editing the page label.
        /// When pressing return, or leaving the control, the change will be made.
        /// </summary>
        internal void StartEditingPageNumber()
        {
            mLabelBox.Size = Size;
            mLabelBox.BackColor = BackColor;
            mLabelBox.Text = "";
            mLabelBox.SelectedText = mLabel.Text == "" ? Localizer.Message("no_page_label") : mLabel.Text;
            mLabelBox.Visible = true;
            mLabelBox.Focus();
        }

        /// <summary>
        /// Return the label box to read only.
        /// </summary>
        private void StopEditingPageNumber()
        {
            mLabelBox.Visible = false;
        }

        /// <summary>
        /// Catch return and escape events to signal the end of editing.
        /// </summary>
        private void mLabelBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    UpdatePageNumber();
                    break;
                case Keys.Escape:
                    StopEditingPageNumber(); 
                    break;
                default:
                    break;
            }
        }

        private void mLabelBox_Leave(object sender, EventArgs e)
        {
            StopEditingPageNumber();
        }

        /// <summary>
        /// Set the page node from the label text.
        /// An empty string will have no effect.
        /// </summary>
        private void UpdatePageNumber()
        {
            StopEditingPageNumber();
            if (mLabelBox.Text == null || mLabelBox.Text == "")
            {
                mLabelBox.Text = mLabel.Text;
            }
            else
            {
                // try to get a number out of this text
                Match m = System.Text.RegularExpressions.Regex.Match(mLabelBox.Text, "\\d+");
                if (m.Success)
                {
                    int pageNumber = Int32.Parse(m.Value);
                    mLabel.Text = pageNumber.ToString();
                    mManager.RequestToSetPageNumber(this, new Events.Node.SetPageEventArgs(this, mAudioBlock.Node, pageNumber));
                }
            }
        }
    }
}
