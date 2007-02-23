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
        private SectionNode mNode;           // the core node for this strip
        private bool mSelected;              // currently selected
        private bool mRenaming;              // in the process of being renamed

        public static readonly float H1Size = 1.6f;  // relative font size of sections of level 1
        public static readonly float H2Size = 1.4f;  // relative font size of sections of level 2
        public static readonly float H3Size = 1.2f;  // relative font size of sections of level 3

        public delegate void ChangedMinimumSizeHandler(object sender, EventArgs e);

        #region properties

        /// <summary>
        /// The label of a strip is the title of the section it represents.
        /// </summary>
        public string Label
        {
            get { return mLabel.Text; }
            set
            {
                mRenameBox.Text = value;
                mLabel.Text = value;
                AccessibleDescription = value;
            }
        }

        /// <summary>
        /// The strip manager that manages this strip.
        /// </summary>
        public StripManagerPanel Manager
        {
            set { mManager = value; }
            get { return mManager; }
        }

        /// <summary>
        /// The section that the strip represents to.
        /// </summary>
        public SectionNode Node
        {
            get { return mNode; }
            set
            {
                mNode = value;
                Selected = false;
                SetStripFontSize();
                RefreshUsed();
            }
        }

        /// <summary>
        /// True if the strip is currently selected.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
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
        /// True if the strip (i.e. its section) is used.
        /// </summary>
        public bool Used
        {
            get { return mNode.Used; }
            set
            {
                if (mNode != null && mNode.Used != value)
                {
                    mNode.Used = value;
                    BackColor = mNode.Used ? Colors.SectionStripUsedBack : Colors.SectionStripUnusedBack;
                    ForeColor = mNode.Used ? Colors.SectionStripUsedFore : Colors.SectionStripUnusedBack;
                    mLabel.BackColor = BackColor;
                }
            }
        }

        /// <summary>
        /// Disable the transport bar when renaming.
        /// </summary>
        public bool Renaming
        {
            set
            {
                mRenaming = value;
                mManager.ProjectPanel.TransportBar.Enabled = !value;
                mManager.ProjectPanel.RenamingSection = value;
                mRenameBox.Visible = value;
                mLabel.Visible = !value;
                if (value)
                {
                    // start renaming
                    mRenameBox.Size = new Size(Width, mRenameBox.Height);
                    mRenameBox.Visible = true;
                    mRenameBox.BackColor = BackColor;
                    mRenameBox.SelectAll();
                    mAudioLayoutPanel.Focus();
                    mRenameBox.Focus();
                }
            }
        }

        #endregion

        /// <summary>
        /// Create an empty section strip.
        /// </summary>
        public SectionStrip()
        {
            InitializeComponent();
            InitializeToolTips();
            mManager = null;
            mNode = null;
            Selected = false;
            mRenaming = false;            
        }

        #region TextBox (the label strip)

        /// <summary>
        /// Leaving the text box updates the text property.
        /// </summary>
        private void mTextBox_Leave(object sender, EventArgs e)
        {
            Renaming = false;
        }

        /// <summary>
        /// Using this eventhandler to override the TextBox's native ContextMenu with that in StripManagerPanel
        /// </summary>
        ///<remarks>We seem to have to do this at every mousedown, 
        ///else the first pop of contextmenu is that of the textbox 
        ///(windows does the redraw before the event).</remarks>
        private void mTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.mRenameBox.ContextMenuStrip = this.Manager.PanelContextMenuStrip;
        }

        /// <summary>
        /// Typing return updates the text property; escape cancels the edit.
        /// </summary>
        private void mTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    Renaming = false;
                    mLabel.Text = mRenameBox.Text;
                    UpdateText();
                    break;
                case Keys.Escape:
                    Renaming = false;
                    mRenameBox.Text = mLabel.Text;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Upate the text label from the text box input.
        /// If the input is empty, then do not change the text and warn the user.
        /// The manager is then asked to send a rename event.
        /// </summary>
        private void UpdateText()
        {
            if (mRenameBox.Text != "")
            {
                mManager.RenamedSectionStrip(this);
            }
            else
            {
                MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                    Localizer.Message("empty_label_warning_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        
        #endregion
        
        #region SectionStrip (this)
        
        private void SectionStrip_Click(object sender, EventArgs e)
        {
            mManager.SelectedSectionNode = mNode;
        }

        private void SectionStrip_DoubleClick(object sender, EventArgs e)
        {
            mManager.SelectedSectionNode = mNode;
            ((ObiForm)mManager.ParentForm).Play(mNode);
        }

        public void SetStripFontSize()
        {
            if (mNode != null)
            {
                int nodeLevel = mNode.Level;
                SetTitleFontSize(((ObiForm)mManager.ParentForm).Settings.FontSize *
                    (nodeLevel == 1 ? H1Size :
                    nodeLevel == 2 ? H2Size :
                    nodeLevel == 3 ? H3Size : 1.0f));
            }
        }

        #endregion

        #region audio strip

        /// <summary>
        /// Append an audio block to the strip. This also appends the corresponding annotation block.
        /// </summary>
        /// <param name="block">The audio block to append.</param>
        public void AppendAudioBlock(AudioBlock block)
        {
            mAnnotationLayoutPanel.Controls.Add(block.AnnotationBlock);
            mAudioLayoutPanel.Controls.Add(block);
            block.AnnotationBlock.ChangedMinimumSize += new SectionStrip.ChangedMinimumSizeHandler(
                delegate(object sender, EventArgs e) { ManageAudioBlockWidth(block); }
            );
            ManageAudioBlockWidth(block);
            // fix the layout so that the two layout panels are correctly placed.
            if (mAudioLayoutPanel.Controls.Count == 1)
            {
                mAudioLayoutPanel.Location = new Point(mAudioLayoutPanel.Location.X,
                    mAnnotationLayoutPanel.Location.Y + mAnnotationLayoutPanel.Height + mAnnotationLayoutPanel.Margin.Bottom); 
            }
            RefreshAudioBlockLabels();
        }

        public void ManageAudioBlockWidth(AudioBlock block)
        {
            int widest = block.AnnotationBlock.MinimumSize.Width > block.MinimumSize.Width ?
                block.AnnotationBlock.MinimumSize.Width : block.MinimumSize.Width;
            if (block.AnnotationBlock.Width != widest) block.AnnotationBlock.Width = widest;
            if (block.Width != widest) block.Width = widest;
        }

        /// <summary>
        /// Insert an audio block in the strip, along with its annotation block.
        /// </summary>
        /// <param name="block">The block to insert.</param>
        /// <param name="index">The insertion position.</param>
        public void InsertAudioBlock(AudioBlock block, int index)
        {
            AppendAudioBlock(block);
            mAudioLayoutPanel.Controls.SetChildIndex(block, index);
            mAnnotationLayoutPanel.Controls.SetChildIndex(block.AnnotationBlock, index);
            RefreshAudioBlockLabels(); // refreshed twice :(
        }

        /// <summary>
        /// Remove an audio block from the strip. Its annotation block is removed as well.
        /// </summary>
        /// <param name="block">The block to remove.</param>
        public void RemoveAudioBlock(AudioBlock block)
        {
            int index = mAudioLayoutPanel.Controls.IndexOf(block);
            mAudioLayoutPanel.Controls.RemoveAt(index);
            mAnnotationLayoutPanel.Controls.RemoveAt(index);
            //ReflowTabOrder(index);
            // fix the layout again if the strip becomes empty.
            if (mAudioLayoutPanel.Controls.Count == 0) mAudioLayoutPanel.Location = mAnnotationLayoutPanel.Location;
            RefreshAudioBlockLabels();
        }

        /// <summary>
        /// Set the annotation block of an audio block.
        /// </summary>
        /// <param name="block">The audio block to set the annotation for.</param>
        /// <param name="annotation">The annotation for this block.</param>
        public void SetAnnotationBlock(AudioBlock block, string annotation)
        {
            block.AnnotationBlock.Label = annotation;
        }

        /// <summary>
        /// Clicking in the audio strip selects the strip but unselects the audio block.
        /// </summary>
        private void mAudioLayoutPanel_Click(object sender, EventArgs e)
        {
            mManager.SelectedSectionNode = mNode;
            mManager.SelectedPhraseNode = null;
        }

        /// <summary>
        /// Clicking in the annotation strip selects the strip but unselects the phrase.
        /// </summary>
        private void mAnnotationLayoutPanel_Click(object sender, EventArgs e)
        {
            mManager.SelectedSectionNode = mNode;
            mManager.SelectedPhraseNode = null;
        }

        #endregion

        /// <summary>
        /// The asset for an audio has been modified, so update it (and its node?)
        /// </summary>
        /// <param name="audioBlock">The block for which the asset has changed.</param>
        internal void UpdateAssetAudioBlock(AudioBlock audioBlock)
        {
            audioBlock.RefreshDisplay();
            // Assets.AudioMediaAsset asset = audioBlock.Node.Asset;
            // audioBlock.Time = Assets.MediaAsset.FormatTime(asset.LengthInMilliseconds);
        }

        /// <summary>
        /// set the font size for the title font
        /// </summary>
        private void SetTitleFontSize(float sz)
        {
            Font newfont = new Font(mRenameBox.Font.FontFamily, sz);
            mRenameBox.Font = newfont;
            mLabel.Font = newfont;
         
        }

        public float GetTitleFontSize()
        {
            return mRenameBox.Font.Size;
        }

        //md 20061009
        private void InitializeToolTips()
        {
            this.mToolTip.SetToolTip(this, Localizer.Message("section_strip_tooltip"));
            this.mToolTip.SetToolTip(this.mRenameBox, Localizer.Message("section_strip_name_tooltip"));
            this.mToolTip.SetToolTip(this.mLabel, Localizer.Message("section_strip_name_tooltip"));
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
                e.Graphics.DrawRectangle(pen, new Rectangle(Padding.All / 2,
                    Padding.All / 2,
                    Width - Padding.All,
                    Height - Padding.All));
                pen.Dispose();
            }
        }

        /// <summary>
        /// Refresh the strip to show its used state.
        /// </summary>
        public void RefreshUsed()
        {
            System.Diagnostics.Debug.Assert(mNode != null, "Refreshing strip with no node.");
            BackColor = mNode.Used ? Colors.SectionStripUsedBack : Colors.SectionStripUnusedBack;
            ForeColor = mNode.Used ? Colors.SectionStripUsedFore : Colors.SectionStripUnusedFore;
            foreach (Control c in mAudioLayoutPanel.Controls)
            {
                if (c is AudioBlock) ((AudioBlock)c).RefreshDisplay();
            }
        }

        /// <summary>
        /// Refresh the labels of all audio blocks when a new block is inserted or removed.
        /// </summary>
        private void RefreshAudioBlockLabels()
        {
            foreach (Control control in mAudioLayoutPanel.Controls)
            {
                AudioBlock block = control as AudioBlock;
                System.Diagnostics.Debug.Assert(block != null);
                block.RefreshLabels();
            }
        }
    }
}
