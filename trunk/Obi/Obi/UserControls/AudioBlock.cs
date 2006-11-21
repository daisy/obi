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
        public override CoreNode Node
        {
            get { return mNode; }
            set { mNode = value; }
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

        /// <summary>
        /// Set the label (i.e. asset name) of this block
        /// </summary>
        public string Label
        {
            set { mLabel.Text = value; }
        }

        /// <summary>
        /// Set the page number associated with the block (empty string or page number)
        /// </summary>
        public string Page
        {
            set
            {
                mPage.ReadOnly = true;
                mPage.Text = value;
                mPage.Visible = value != "";
            }
        }

        /// <summary>
        /// Make sure that the page box is visible, with the correct color and size.
        /// </summary>
        private void ShowPageBox()
        {
            mPage.Visible = true;
            mPage.BackColor = BackColor;
            mPage.Size = new Size(Width, mPage.Height);
        }

        /// <summary>
        /// Total time display (string form.)
        /// </summary>
        public string Time
        {
            set { mTimeLabel.Text = value; }
        }

        /// <summary>
        /// Name of the audio asset (shown in the block.)
        /// </summary>
        public string AssetName
        {
            set { mLabel.Text = value; }
        }

/*med 20061120 svn merge: i think this function was removed for 0.7

        /// <summary>
        /// Update the width of the control.
        /// </summary>
        //md20061011
        public int _Width
        {
            set { Size = new Size(value, Size.Height); }
        }
*/
        #endregion
        
        #region instantiators

        public AudioBlock() : base()
        {
            InitializeComponent();
            this.TabStop = true;
            mAnnotationBlock = new AnnotationBlock();
            mAnnotationBlock.AudioBlock = this;
            InitializeToolTips();
        }

        #endregion

        #region AudioBlock (this)

        internal override void MarkDeselected()
        {
            BackColor = Color.MistyRose;
            mPage.BackColor = BackColor;
            mAnnotationBlock.MarkDeselected();
            // if we are editing but selected a different block, stop editing.
            StopEditingPageNumber();
        }

        internal override void MarkSelected()
        {
            BackColor = Color.LightPink;
            mPage.BackColor = BackColor;
            mAnnotationBlock.MarkSelected();
        }

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            mManager.SelectedPhraseNode = mNode; 
        }

        private void AudioBlock_DoubleClick(object sender, EventArgs e)
        {
            mManager.SelectedPhraseNode = mNode;
            ((ObiForm)ParentForm).Play(mNode);
        }

        //mg: for tab navigation et al
        private void AudioBlock_enter(object sender, EventArgs e)
        {
            mManager.SelectedPhraseNode = mNode;
            System.Diagnostics.Debug.Print("Audioblock:tabindex:" + this.TabIndex.ToString());
            // MarkSelected();
        }

        #endregion

        #region Rename Box

        /// <summary>
        /// Bring up the editable text box.
        /// TODO: maybe the labels should be replaced by non-editable text boxes.
        /// There is also a bug that occurs when editing for the second time, when
        /// no text is selected... strange.
        /// </summary>
        internal void StartRenaming()
        {
            mAnnotationBlock.StartRenaming();
        }

        #endregion

        /// <summary>
        /// Edit the page label for the structure block linked to this audio block.
        /// </summary>
        internal void StartEditingPageNumber()
        {
            ShowPageBox();
            mPage.ReadOnly = false;
            mPage.Tag = mPage.Text;
            if (mPage.Text == "") mPage.Text = Localizer.Message("no_page_label");
            mPage.SelectAll();
            mPage.Focus();
        }

        /// <summary>
        /// Stop the editing of the page number.
        /// </summary>
        private void StopEditingPageNumber()
        {
            if (!mPage.ReadOnly) Page = (string)mPage.Tag;
        }

        /// <summary>
        /// Catch enter and escape to update or cancel the page number.
        /// </summary>
        private void mPage_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                UpdatePageNumber();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                StopEditingPageNumber();
            }
        }

        /// <summary>
        /// Leaving the page box without validating cancels.
        /// </summary>
        private void mPage_Leave(object sender, EventArgs e)
        {
/*med 20061120 svn merge: this looks like it was replaced with StopEditingPageNumber()
            ShowPageBox();
            mPage.ReadOnly = false;
            mPage.Tag = mPage.Text;
            if (mPage.Text == "") mPage.Text = Localizer.Message("no_page_label");
            mPage.SelectAll();
            mPage.Focus();
*/
            StopEditingPageNumber();
        }

/*med 20061120 svn merge: i think this is old code
        /// <summary>
        /// Stop the editing of the page number.
        /// </summary>
        private void StopEditingPageNumber()
        {
            if (!mPage.ReadOnly) Page = (string)mPage.Tag;
        }

        /// <summary>
        /// Catch enter and escape to update or cancel the page number.
        /// </summary>
        private void mPage_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                UpdatePageNumber();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                StopEditingPageNumber();
            }
        }

        /// <summary>
        /// Leaving the page box without validating cancels.
        /// </summary>
        private void mPage_Leave(object sender, EventArgs e)
        {
            StopEditingPageNumber();
        }

        /// <summary>
        /// Set the page node from the label text.
        /// An empty string will have no effect.
        /// </summary>
        private void UpdatePageNumber()
        {
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(mPage.Text, "\\d+");
            if (m.Success)
            {
                try
                {
                    // the number may be too big
                    int pageNumber = Int32.Parse(m.Value);
                    mManager.RequestToSetPageNumber(this, new Events.Node.SetPageEventArgs(this, mNode, pageNumber));
                    mPage.ReadOnly = true;
                    Page = m.Value;
                }
                catch (Exception)
                {
                    StopEditingPageNumber();
                }
            }
            else
            {
                StopEditingPageNumber();
            }
        }

*/
        /// <summary>
        /// Set the page node from the label text.
        /// An empty string will have no effect.
        /// </summary>
        private void UpdatePageNumber()
        {
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(mPage.Text, "\\d+");
            if (m.Success)
            {
                try
                {
                    // the number may be too big
                    int pageNumber = Int32.Parse(m.Value);
                    mManager.RequestToSetPageNumber(this, new Events.Node.SetPageEventArgs(this, mNode, pageNumber));
                    mPage.ReadOnly = true;
                    Page = m.Value;
                }
                catch (Exception)
                {
                    StopEditingPageNumber();
                }
            }
            else
            {
                StopEditingPageNumber();
            }
        }


        //md 20061009
        private void InitializeToolTips()
        {
            this.mToolTip.SetToolTip(this, Localizer.Message("audio_block_tooltip"));
            this.mToolTip.SetToolTip(this.mTimeLabel, Localizer.Message("audio_block_duration_tooltip"));
        }
        /// <summary>
        /// Contents size changed, so update the minimum width.
        /// </summary>
        private void ContentsSizeChanged(object sender, EventArgs e)
        {
            int wlabel = mLabel.Width + mLabel.Location.X + mLabel.Margin.Right;
            int wtime = mTimeLabel.Width + mTimeLabel.Location.X + mTimeLabel.Margin.Right;
            int widest = wlabel > wtime ? wlabel : wtime;
            MinimumSize = new Size(widest, Height);
            if (ChangedMinimumSize != null) ChangedMinimumSize(this, new EventArgs());
        }
    }
}
