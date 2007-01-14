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
        private StripManagerPanel mManager;          // the manager for this strip
        private SectionNode mNode;                   // the core node for this strip
        private bool mSelected;                      // currently selected

        private static float mDefaultFontSize = 12;  // should move to Colors (and colors should be renamed)

        public delegate void ChangedMinimumSizeHandler(object sender, EventArgs e);

        #region properties

        public string Label
        {
            get { return mLabel.Text; }
            set
            {
                mRenameBox.Text = value;
                mLabel.Text = value;
            }
        }

        public StripManagerPanel Manager
        {
            set { mManager = value; }
            get { return mManager; }
        }

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

        public bool Selected
        {
            get { return mSelected; }
            set
            {
                if (mSelected != value)
                {
                    mSelected = value;
                    /*if (mSelected)
                    {
                        Size = new Size(Width + Colors.SelectionWidth * 2,
                            Height + Colors.SelectionWidth * 2);
                        Padding = new Padding(Colors.SelectionWidth);
                    }
                    else
                    {
                        Size = new Size(Width - Colors.SelectionWidth * 2,
                            Height - Colors.SelectionWidth * 2);
                        Padding = new Padding(0);
                    }*/
                    Invalidate();
                }
            }
        }

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

        #endregion

        public SectionStrip()
        {
            InitializeComponent();
            InitializeToolTips();
            Selected = false;
        }

        #region TextBox (the label strip)

        /// <summary>
        /// The strip has a normally readonly text box at the top.
        /// When renaming, the text box is initialized with the original label.
        /// The whole text is selected and the text box is given the focus so that the
        /// user can start editing right away.
        /// </summary>
        public void StartRenaming()
        {
            mLabel.Visible = false;
            mRenameBox.Size = new Size(Width, mRenameBox.Height);
            mRenameBox.Visible = true;
            mRenameBox.ReadOnly = false;
            mRenameBox.BackColor = BackColor;
            mRenameBox.SelectAll();
            mAudioLayoutPanel.Focus();
            mRenameBox.Focus();
        }

        /// <summary>
        /// Leaving the text box updates the text property.
        /// </summary>
        private void mTextBox_Leave(object sender, EventArgs e)
        {
            mRenameBox.ReadOnly = true;

            mLabel.Text = mRenameBox.Text;
            mLabel.Visible = true;
            mRenameBox.Visible = false;
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
                    mRenameBox.ReadOnly = true;
                    mLabel.Text = mRenameBox.Text;
                    mLabel.Visible = true;
                    mRenameBox.Visible = false;
                    UpdateText();
                    break;
                case Keys.Escape:
                    mRenameBox.Text = Project.GetTextMedia(this.Node).getText();
                    mRenameBox.ReadOnly = true;
                    mLabel.Text = mRenameBox.Text;
                    mLabel.Visible = true;
                    mRenameBox.Visible = false;

                    break;
                case Keys.F2:
                    if (mRenameBox.ReadOnly)
                    {
                        this.StartRenaming();
                    }
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

        internal void SetStripFontSize()
        {
            if (mNode != null)
            {
                int nodeLevel = mNode.Level;
                //float currentSize = GetTitleFontSize();
                if (nodeLevel == 1) SetTitleFontSize(mDefaultFontSize + 3);
                else if (nodeLevel == 2) SetTitleFontSize(mDefaultFontSize + 2);
                else if (nodeLevel == 3) SetTitleFontSize(mDefaultFontSize + 1);
                else SetTitleFontSize(mDefaultFontSize);
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
            if (mAudioLayoutPanel.Controls.Count == 1)
            {
                mAudioLayoutPanel.Location = new Point(mAudioLayoutPanel.Location.X,
                    mAnnotationLayoutPanel.Location.Y + mAnnotationLayoutPanel.Height + mAnnotationLayoutPanel.Margin.Bottom); 
            }
            // fix the layout so that the two layout panels are correctly placed.
            if (mAudioLayoutPanel.Controls.Count == 1)
            {
                mAudioLayoutPanel.Location = new Point(mAudioLayoutPanel.Location.X,
                    mAnnotationLayoutPanel.Location.Y + mAnnotationLayoutPanel.Height + mAnnotationLayoutPanel.Margin.Bottom); 
            }
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
            Assets.AudioMediaAsset asset = audioBlock.Node.Asset;
            audioBlock.AssetName = asset.Name;
            audioBlock.Time = asset.LengthInSeconds;
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
        internal void RefreshUsed()
        {
            System.Diagnostics.Debug.Assert(mNode != null, "Refreshing strip with no node.");
            BackColor = mNode.Used ? Colors.SectionStripUsedBack : Colors.SectionStripUnusedBack;
            ForeColor = mNode.Used ? Colors.SectionStripUsedFore : Colors.SectionStripUnusedFore;
            foreach (Control c in mAudioLayoutPanel.Controls)
            {
                if (c is AudioBlock) ((AudioBlock)c).RefreshUsed();
            }
        }
    }
}
