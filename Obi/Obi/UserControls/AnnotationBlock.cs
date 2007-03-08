using System;
using System.Drawing;
using System.Windows.Forms;

namespace Obi.UserControls
{
    /// <summary>
    /// Block showing the annotation for a phrase in the annotation strip.
    /// </summary>
    public partial class AnnotationBlock : AbstractBlock
    {
        private AudioBlock mAudioBlock;  // the corresponding block in the audio strip

        public event SectionStrip.ChangedMinimumSizeHandler ChangedMinimumSize;

        /// <summary>
        /// Do not use Node on an annotation block.
        /// </summary>
        public override PhraseNode Node
        {
            get { return mAudioBlock == null ? null : mAudioBlock.Node; }
            set { throw new Exception("Cannot set the node of an annotation block; set the node of its phrase block."); }
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
                mRenameBox.BackColor = BackColor;
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

        /// <summary>
        /// Create a new annotation block.
        /// </summary>
        public AnnotationBlock()
        {
            InitializeComponent();
            InitializeToolTips();
            Used = false;
            TabStop = false;
        }

        /// <summary>
        /// Return validates the input in the rename box.
        /// Escape cancels and restores the previous value.
        /// </summary>
        private void mRenameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                UpdateText();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Renaming = false;
            }
        }

        /// <summary>
        /// Set the Renaming property to true to start renaming, and false to stop.
        /// </summary>
        public bool Renaming
        {
            set
            {
                mManager.ProjectPanel.TransportBar.Enabled = !value;
                mManager.AllowShortcuts = !value;
                if (value)
                {
                    mRenameBox.ReadOnly = false;
                    mRenameBox.Width = Width - mRenameBox.Location.X - mRenameBox.Margin.Right;
                    mRenameBox.Text = mLabel.Text;
                    mRenameBox.SelectAll();
                }
                mRenameBox.Visible = value;
                mLabel.Visible = !value;
                if (value) mRenameBox.Focus();
            }
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
            if (mRenameBox.Text != mLabel.Text)
            {
                mLabel.Text = mRenameBox.Text;
                mAudioBlock.Node.Project.EditAnnotationPhraseNode(mAudioBlock.Node, mLabel.Text);
            }
            Renaming = false;
        }

        /// <summary>
        /// Leaving the text box updates the text (if we were updating)
        /// or focuses back on the audio block.
        /// </summary>
        private void mRenameBox_Leave(object sender, EventArgs e)
        {
            if (mRenameBox.Visible && !mRenameBox.ReadOnly)
            {
                UpdateText();
            }
            else
            {
                mRenameBox.Visible = false;
                mAudioBlock.Focus();
            }
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
            System.Diagnostics.Debug.Print("Hi, annotation block here, my size is now {0}.", Size);
            MinimumSize = new Size(mLabel.Width + mLabel.Location.X + mLabel.Margin.Right, Height);
            if (ChangedMinimumSize != null) ChangedMinimumSize(this, new EventArgs());
        }

        //md 20061220 added; not working yet
        private void mRenameBox_KeyUp(object sender, KeyEventArgs e)
        {
            //catch clipboard events
            if (e.Control == true)
            {
                if (e.KeyCode == Keys.X)
                {
                    System.Diagnostics.Debug.Print("CUT!!");
                    mRenameBox.Cut();
                }
                else if (e.KeyCode == Keys.Z) mRenameBox.Undo();
                else if (e.KeyCode == Keys.C) mRenameBox.Copy();
                else if (e.KeyCode == Keys.V) mRenameBox.Paste();
                else if (e.KeyCode == Keys.A) mRenameBox.SelectAll();

                //even though this is set to true, it doesn't seem to stop the keyboard events
                //from going through to the rest of Obi
                e.SuppressKeyPress = true;
            }
        }

        private void mRenameBox_Enter(object sender, EventArgs e)
        {
            mManager.AllowShortcuts = false;
        }

        private void AnnotationBlock_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Entering annotation for <{0}> [TabIndex = {1}]",
                mAudioBlock.AccessibleDescription, TabIndex);
        }

        /// <summary>
        /// Show the text box for the annotation (read-only) and focus on it
        /// </summary>
        public void FocusOnAnnotation()
        {
            mRenameBox.ReadOnly = true;
            mRenameBox.Visible = true;
            mRenameBox.Focus();
            mManager.AllowShortcuts = true;
        }
    }
}
