using System;
using System.Drawing;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    /// <summary>
    /// The audio block is used to display a phrase.
    /// </summary>
    public partial class AudioBlock : AbstractBlock
    {
        private AnnotationBlock mAnnotationBlock;  // the annotation is taken out of the block

        public event SectionStrip.ChangedMinimumSizeHandler ChangedMinimumSize;

        #region properties

        /// <summary>
        /// Phrase node for this block.
        /// </summary>
        public override PhraseNode Node
        {
            get { return mNode; }
            set
            {
                mNode = value;
                if (mAnnotationBlock != null)
                {
                    mAnnotationBlock.Used = mNode != null ? mNode.Used : false;
                }
                if (mNode != null) RefreshDisplay();
            }
        }

        public override bool Selected
        {
            get { return base.Selected; }
            set
            {
                if (mSelected != value)
                {
                    mSelected = value;
                    Invalidate();
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
                /*BackColor = mNode != null && value ?
                    mNode.Asset.LengthInMilliseconds == 0.0 ? Colors.AudioBlockEmpty : Colors.AudioBlockUsed :
                    Colors.AudioBlockUnused;
                mLabel.BackColor = BackColor;*/
                RefreshUsed();
            }
        }

        /// <summary>
        /// Strip manager panel in which this block is displayed.
        /// </summary>
        public override StripManagerPanel Manager
        {
            get { return mManager; }
            set
            {
                mManager = value;
                mAnnotationBlock.Manager = value;
            }
        }

        /// <summary>
        /// Annotation corresponding to this block.
        /// </summary>
        public AnnotationBlock AnnotationBlock
        {
            get { return mAnnotationBlock; }
            set
            {
                mAnnotationBlock = value;
                mAnnotationBlock.AudioBlock = this;
            }
        }

        #endregion

        #region instantiators

        public AudioBlock()
            : base()
        {
            InitializeComponent();
            mAnnotationBlock = new AnnotationBlock();
            mAnnotationBlock.AudioBlock = this;
            this.mToolTip.SetToolTip(this, Localizer.Message("audio_block_tooltip"));
            this.mToolTip.SetToolTip(this.mTimeLabel, Localizer.Message("audio_block_duration_tooltip"));
        }

        #endregion

        #region AudioBlock (this)

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("click on audio blocks");
            mManager.SelectedPhraseNode = mNode;
        }

        private void AudioBlock_DoubleClick(object sender, EventArgs e)
        {
            mManager.SelectedPhraseNode = mNode;
            ((ObiForm)mManager.ParentForm).Play(mNode);
        }

        #endregion

        #region Rename Box

        /// <summary>
        /// Bring up the editable text box.
        /// TODO: maybe the labels should be replaced by non-editable text boxes.
        /// There is also a bug that occurs when editing for the second time, when
        /// no text is selected... strange.
        /// </summary>
        internal void StartEditingAnnotation()
        {
            mAnnotationBlock.StartRenaming();
        }

        #endregion

        /// <summary>
        /// Contents size changed, so update the minimum width.
        /// </summary>
        private void ContentsSizeChanged(object sender, EventArgs e)
        {
            RefreshWidth();
        }

        /// <summary>
        /// Paint borders on the block if selected. In thick red pen, no less.
        /// </summary>
        /// <remarks>Let's look at ControlPaint for better results.</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (mSelected)
            {
                Pen pen = new Pen(Colors.SelectionColor, Padding.All);
                e.Graphics.DrawRectangle(pen, new Rectangle(Padding.All / 2, Padding.All / 2,
                    Width - Padding.All, Height - Padding.All));
                pen.Dispose();
            }
        }

        /// <summary>
        /// Refresh the display when the node underneath has been modified.
        /// </summary>
        public void RefreshDisplay()
        {
            RefreshUsed();
            RefreshLabels();
            RefreshWidth();
        }

        /// <summary>
        /// Refresh the display of the block to show its used state.
        /// </summary>
        private void RefreshUsed()
        {
            BackColor = mNode != null && mNode.Used ?
                mNode.Asset.LengthInMilliseconds == 0.0 ? Colors.AudioBlockEmpty : Colors.AudioBlockUsed :
                Colors.AudioBlockUnused;
            if (mAnnotationBlock != null)
            {
                mAnnotationBlock.Used = mNode.Used;
            }
        }

        /// <summary>
        /// Refresh the labels of the block to show its timing and page number (if any.)
        /// </summary>
        private void RefreshLabels()
        {
            // Set the label
            if (mNode.PageProperty != null)
            {
                mLabel.Text = String.Format(Localizer.Message("page_number"), mNode.PageProperty.PageNumber);
            }
            else
            {
                mLabel.Text = Localizer.Message("audio_block_default_label");
            }
            // Set the time display
            mTimeLabel.Text = Assets.MediaAsset.FormatTime(mNode.Asset.LengthInMilliseconds);
        }

        /// <summary>
        /// Refresh the width of the block and its annotation.
        /// </summary>
        private void RefreshWidth()
        {
            int wlabel = mLabel.Width + mLabel.Location.X + mLabel.Margin.Right;
            int wtime = mTimeLabel.Width + mTimeLabel.Location.X + mTimeLabel.Margin.Right;
            int widest = wlabel > wtime ? wlabel : wtime;
            MinimumSize = new Size(widest, Height);
            if (ChangedMinimumSize != null) ChangedMinimumSize(this, new EventArgs());
        }
    }
}