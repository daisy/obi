using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class AnnotationBlock : AbstractBlock
    {
        private AudioBlock mAudioBlock;  // the corresponding audio block

        public event SectionStrip.ChangedMinimumSizeHandler ChangedMinimumSize;

        /// <summary>
        /// Do not use Node on an annotation block.
        /// </summary>
        public override PhraseNode Node
        {
            get { return mAudioBlock == null ? null : mAudioBlock.Node; }
            set { throw new Exception("Cannot set the node of an annotation block; set the node of its phrase block."); }
        }

        public override bool Selected
        {
            get { return base.Selected; }
            set
            {
                mSelected = value;
                if (mAudioBlock.Node.Annotation != "")
                {
                    if (mSelected) StartRenaming(); else UpdateText();
                }
            }
        }

        /// <summary>
        /// Used in the project or excluded from it.
        /// </summary>
        public override bool Used
        {
            set
            {
                BackColor = value ? Colors.AnnotationBlockUsed : Colors.AnnotationBlockUnused;
                mLabel.BackColor = BackColor;
            }
        }

        /// <summary>
        /// Audio block with which this annotation is synchronized.
        /// </summary>
        public AudioBlock AudioBlock
        {
            get { return mAudioBlock; }
            set
            {
                mAudioBlock = value;
                Used = mAudioBlock != null && mAudioBlock.Node != null ? mAudioBlock.Node.Used : false;
            }
        }

        /// <summary>
        /// The annotation (on a label)
        /// </summary>
        public string Label
        {
            get { return mLabel.Text; }
            set { mLabel.Text = value; }
        }

        public AnnotationBlock()
        {
            InitializeComponent();
            InitializeToolTips();
            Used = false;
        }

        private void mRenameBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    UpdateText();
                    break;
                case Keys.Escape:
                    mRenameBox.Visible = false;
                    break;
                default:
                    break;
            }
        }

        private void AnnotationBlock_Click(object sender, EventArgs e)
        {
            mAudioBlock.Manager.SelectedPhraseNode = null;
            Selected = true;
            // mAudioBlock.Manager.SelectedPhraseNode = mAudioBlock.Node;
        }

        public void StartRenaming()
        {
            mRenameBox.Width = Width - mRenameBox.Location.X - mRenameBox.Margin.Right;
            mRenameBox.BackColor = BackColor;
            mRenameBox.Text = mLabel.Text;
            mRenameBox.SelectAll();
            mRenameBox.Visible = true;
            mLabel.Visible = false;
            mRenameBox.Focus();
        }

        /// <summary>
        /// Upate the text label from the text box input.
        /// If the input is empty then do not change the text and warn the user.
        /// If the input text is the same as the original text, don't do anything.
        /// The manager is then asked to send a rename event.
        /// The rename event may not happen if there is already an audio block with the same name in the project.
        /// </summary>
        public void UpdateText()
        {
            if (mRenameBox.Text != "" && mRenameBox.Text != mLabel.Text)
            {
                mManager.EditedAudioBlockLabel(this.mAudioBlock, mRenameBox.Text);
            }
            //md20061011 added condition
            else if (mRenameBox.Text == "")
            {
                MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                    Localizer.Message("empty_label_warning_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            mLabel.Visible = true;
            mRenameBox.Visible = false;
        }

        private void mRenameBox_Leave(object sender, EventArgs e)
        {
            UpdateText();
        }

        //md 20061009
        private void InitializeToolTips()
        {
            this.mToolTip.SetToolTip(this, Localizer.Message("annotation_tooltip"));
            this.mToolTip.SetToolTip(this.mRenameBox, Localizer.Message("annotation_tooltip"));
            this.mToolTip.SetToolTip(this.mLabel, Localizer.Message("annotation_tooltip"));
        }

        /// <summary>
        /// Our own implementation of auto-sizing...
        /// </summary>
        private void mLabel_SizeChanged(object sender, EventArgs e)
        {
            MinimumSize = new Size(mLabel.Width + mLabel.Location.X + mLabel.Margin.Right, Height);
            if (ChangedMinimumSize != null) ChangedMinimumSize(this, new EventArgs());
        }

        private void AnnotationBlock_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Hi, my name is annotation block and my size is {0}.", Size);
        }
    }
}
