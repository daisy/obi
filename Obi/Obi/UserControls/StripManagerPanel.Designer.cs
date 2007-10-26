namespace Obi.UserControls
{
    partial class StripManagerPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.mFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mInsertStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRenameStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCutStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCopyStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPasteStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMarkStripAsUnusedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowInTOCViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mImportAudioFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCutAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCopyAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPasteAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMarkPhraseAsUnusedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSplitAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mQuickSplitAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mApplyPhraseDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMergeWithPreviousAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveAudioBlockBackwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveAudioBlockForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mEditAnnotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRemoveAnnotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFocusOnAnnotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mSetPageNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRemovePageNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mGoTopageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Strip manager";
            // 
            // mFlowLayoutPanel
            // 
            this.mFlowLayoutPanel.AutoScroll = true;
            this.mFlowLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.mFlowLayoutPanel.ContextMenuStrip = this.mContextMenuStrip;
            this.mFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mFlowLayoutPanel.Name = "mFlowLayoutPanel";
            this.mFlowLayoutPanel.Size = new System.Drawing.Size(150, 150);
            this.mFlowLayoutPanel.TabIndex = 1;
            this.mFlowLayoutPanel.WrapContents = false;
            this.mFlowLayoutPanel.Click += new System.EventHandler(this.mFlowLayoutPanel_Click);
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mInsertStripToolStripMenuItem,
            this.mRenameStripToolStripMenuItem,
            this.mCutStripToolStripMenuItem,
            this.mCopyStripToolStripMenuItem,
            this.mPasteStripToolStripMenuItem,
            this.mDeleteStripToolStripMenuItem,
            this.mMarkStripAsUnusedToolStripMenuItem,
            this.mShowInTOCViewToolStripMenuItem,
            this.toolStripSeparator1,
            this.mImportAudioFileToolStripMenuItem,
            this.mCutAudioBlockToolStripMenuItem,
            this.mCopyAudioBlockToolStripMenuItem,
            this.mPasteAudioBlockToolStripMenuItem,
            this.mDeleteAudioBlockToolStripMenuItem,
            this.mMarkPhraseAsUnusedToolStripMenuItem,
            this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem,
            this.mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem,
            this.mSplitAudioBlockToolStripMenuItem,
            this.mQuickSplitAudioBlockToolStripMenuItem,
            this.mApplyPhraseDetectionToolStripMenuItem,
            this.mMergeWithPreviousAudioBlockToolStripMenuItem,
            this.mMoveAudioBlockToolStripMenuItem,
            this.toolStripSeparator3,
            this.mEditAnnotationToolStripMenuItem,
            this.mRemoveAnnotationToolStripMenuItem,
            this.mFocusOnAnnotationToolStripMenuItem,
            this.toolStripSeparator4,
            this.mSetPageNumberToolStripMenuItem,
            this.mRemovePageNumberToolStripMenuItem,
            this.mGoTopageToolStripMenuItem});
            this.mContextMenuStrip.Name = "contextMenuStrip1";
            this.mContextMenuStrip.ShowImageMargin = false;
            this.mContextMenuStrip.Size = new System.Drawing.Size(284, 638);
            this.mContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.mContextMenuStrip_Opening);
            // 
            // mInsertStripToolStripMenuItem
            // 
            this.mInsertStripToolStripMenuItem.Name = "mInsertStripToolStripMenuItem";
            this.mInsertStripToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mInsertStripToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mInsertStripToolStripMenuItem.Text = "&Insert strip";
            // 
            // mRenameStripToolStripMenuItem
            // 
            this.mRenameStripToolStripMenuItem.Name = "mRenameStripToolStripMenuItem";
            this.mRenameStripToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mRenameStripToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mRenameStripToolStripMenuItem.Text = "Rena&me strip";
            this.mRenameStripToolStripMenuItem.Click += new System.EventHandler(this.RenameStripOrEditAnnotation);
            // 
            // mCutStripToolStripMenuItem
            // 
            this.mCutStripToolStripMenuItem.Name = "mCutStripToolStripMenuItem";
            this.mCutStripToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mCutStripToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mCutStripToolStripMenuItem.Text = "Cu&t strip";
            // 
            // mCopyStripToolStripMenuItem
            // 
            this.mCopyStripToolStripMenuItem.Name = "mCopyStripToolStripMenuItem";
            this.mCopyStripToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mCopyStripToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mCopyStripToolStripMenuItem.Text = "&Copy strip";
            // 
            // mPasteStripToolStripMenuItem
            // 
            this.mPasteStripToolStripMenuItem.Name = "mPasteStripToolStripMenuItem";
            this.mPasteStripToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mPasteStripToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mPasteStripToolStripMenuItem.Text = "&Paste strip";
            // 
            // mDeleteStripToolStripMenuItem
            // 
            this.mDeleteStripToolStripMenuItem.Name = "mDeleteStripToolStripMenuItem";
            this.mDeleteStripToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mDeleteStripToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mDeleteStripToolStripMenuItem.Text = "&Delete strip";
            // 
            // mMarkStripAsUnusedToolStripMenuItem
            // 
            this.mMarkStripAsUnusedToolStripMenuItem.Name = "mMarkStripAsUnusedToolStripMenuItem";
            this.mMarkStripAsUnusedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.mMarkStripAsUnusedToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mMarkStripAsUnusedToolStripMenuItem.Text = "Mar&k strip as unused";
            this.mMarkStripAsUnusedToolStripMenuItem.Click += new System.EventHandler(this.ToggleUsedStripOrAudioBlockHandler);
            // 
            // mShowInTOCViewToolStripMenuItem
            // 
            this.mShowInTOCViewToolStripMenuItem.Name = "mShowInTOCViewToolStripMenuItem";
            this.mShowInTOCViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
            this.mShowInTOCViewToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mShowInTOCViewToolStripMenuItem.Text = "Show in TOC &view";
            this.mShowInTOCViewToolStripMenuItem.Click += new System.EventHandler(this.mShowInTOCViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(280, 6);
            // 
            // mImportAudioFileToolStripMenuItem
            // 
            this.mImportAudioFileToolStripMenuItem.Name = "mImportAudioFileToolStripMenuItem";
            this.mImportAudioFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.mImportAudioFileToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mImportAudioFileToolStripMenuItem.Text = "Import &audio files";
            this.mImportAudioFileToolStripMenuItem.Click += new System.EventHandler(this.mImportAudioToolStripMenuItem_Click);
            // 
            // mCutAudioBlockToolStripMenuItem
            // 
            this.mCutAudioBlockToolStripMenuItem.Name = "mCutAudioBlockToolStripMenuItem";
            this.mCutAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mCutAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mCutAudioBlockToolStripMenuItem.Text = "Cu&t phrase";
            // 
            // mCopyAudioBlockToolStripMenuItem
            // 
            this.mCopyAudioBlockToolStripMenuItem.Name = "mCopyAudioBlockToolStripMenuItem";
            this.mCopyAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mCopyAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mCopyAudioBlockToolStripMenuItem.Text = "&Copy phrase";
            // 
            // mPasteAudioBlockToolStripMenuItem
            // 
            this.mPasteAudioBlockToolStripMenuItem.Name = "mPasteAudioBlockToolStripMenuItem";
            this.mPasteAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mPasteAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mPasteAudioBlockToolStripMenuItem.Text = "&Paste phrase";
            // 
            // mDeleteAudioBlockToolStripMenuItem
            // 
            this.mDeleteAudioBlockToolStripMenuItem.Name = "mDeleteAudioBlockToolStripMenuItem";
            this.mDeleteAudioBlockToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mDeleteAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mDeleteAudioBlockToolStripMenuItem.Text = "&Delete phrase";
            // 
            // mMarkPhraseAsUnusedToolStripMenuItem
            // 
            this.mMarkPhraseAsUnusedToolStripMenuItem.Name = "mMarkPhraseAsUnusedToolStripMenuItem";
            this.mMarkPhraseAsUnusedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.mMarkPhraseAsUnusedToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mMarkPhraseAsUnusedToolStripMenuItem.Text = "Mar&k phrase as unused";
            this.mMarkPhraseAsUnusedToolStripMenuItem.Click += new System.EventHandler(this.ToggleUsedStripOrAudioBlockHandler);
            // 
            // mMarkAudioBlockAsSectionHeadingToolStripMenuItem
            // 
            this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Name = "mMarkAudioBlockAsSectionHeadingToolStripMenuItem";
            this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Text = "Mark audio block as section head&ing";
            this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Click += new System.EventHandler(this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem_Click);
            // 
            // mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem
            // 
            this.mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Name = "mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem";
            this.mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Text = "Unmark audio block as section head&ing";
            this.mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Click += new System.EventHandler(this.mMarkAudioBlockAsSectionHeadingToolStripMenuItem_Click);
            // 
            // mSplitAudioBlockToolStripMenuItem
            // 
            this.mSplitAudioBlockToolStripMenuItem.Name = "mSplitAudioBlockToolStripMenuItem";
            this.mSplitAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mSplitAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mSplitAudioBlockToolStripMenuItem.Text = "&Split audio block";
            this.mSplitAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mSplitAudioBlockToolStripMenuItem_Click);
            // 
            // mQuickSplitAudioBlockToolStripMenuItem
            // 
            this.mQuickSplitAudioBlockToolStripMenuItem.Name = "mQuickSplitAudioBlockToolStripMenuItem";
            this.mQuickSplitAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.P)));
            this.mQuickSplitAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mQuickSplitAudioBlockToolStripMenuItem.Text = "&Quick split audio block";
            this.mQuickSplitAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mQuickSplitAudioBlockToolStripMenuItem_Click);
            // 
            // mApplyPhraseDetectionToolStripMenuItem
            // 
            this.mApplyPhraseDetectionToolStripMenuItem.Name = "mApplyPhraseDetectionToolStripMenuItem";
            this.mApplyPhraseDetectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.mApplyPhraseDetectionToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mApplyPhraseDetectionToolStripMenuItem.Text = "Apply p&hrase detection";
            this.mApplyPhraseDetectionToolStripMenuItem.Click += new System.EventHandler(this.mApplyPhraseDetectionToolStripMenuItem_Click);
            // 
            // mMergeWithPreviousAudioBlockToolStripMenuItem
            // 
            this.mMergeWithPreviousAudioBlockToolStripMenuItem.Name = "mMergeWithPreviousAudioBlockToolStripMenuItem";
            this.mMergeWithPreviousAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.mMergeWithPreviousAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mMergeWithPreviousAudioBlockToolStripMenuItem.Text = "Mer&ge with previous audio block";
            this.mMergeWithPreviousAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mMergeWithPreviousAudioBlockToolStripMenuItem_Click);
            // 
            // mMoveAudioBlockToolStripMenuItem
            // 
            this.mMoveAudioBlockToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mMoveAudioBlockBackwardToolStripMenuItem,
            this.mMoveAudioBlockForwardToolStripMenuItem});
            this.mMoveAudioBlockToolStripMenuItem.Name = "mMoveAudioBlockToolStripMenuItem";
            this.mMoveAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mMoveAudioBlockToolStripMenuItem.Text = "Move au&dio block...";
            // 
            // mMoveAudioBlockBackwardToolStripMenuItem
            // 
            this.mMoveAudioBlockBackwardToolStripMenuItem.Name = "mMoveAudioBlockBackwardToolStripMenuItem";
            this.mMoveAudioBlockBackwardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Left)));
            this.mMoveAudioBlockBackwardToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.mMoveAudioBlockBackwardToolStripMenuItem.Text = "&Backward";
            this.mMoveAudioBlockBackwardToolStripMenuItem.Click += new System.EventHandler(this.mMoveAudioBlockBackwardToolStripMenuItem_Click);
            // 
            // mMoveAudioBlockForwardToolStripMenuItem
            // 
            this.mMoveAudioBlockForwardToolStripMenuItem.Name = "mMoveAudioBlockForwardToolStripMenuItem";
            this.mMoveAudioBlockForwardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
            this.mMoveAudioBlockForwardToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.mMoveAudioBlockForwardToolStripMenuItem.Text = "&Forward";
            this.mMoveAudioBlockForwardToolStripMenuItem.Click += new System.EventHandler(this.mMoveAudioBlockForwardToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(280, 6);
            // 
            // mEditAnnotationToolStripMenuItem
            // 
            this.mEditAnnotationToolStripMenuItem.Name = "mEditAnnotationToolStripMenuItem";
            this.mEditAnnotationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mEditAnnotationToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mEditAnnotationToolStripMenuItem.Text = "Edit ann&otation";
            this.mEditAnnotationToolStripMenuItem.Click += new System.EventHandler(this.RenameStripOrEditAnnotation);
            // 
            // mRemoveAnnotationToolStripMenuItem
            // 
            this.mRemoveAnnotationToolStripMenuItem.Name = "mRemoveAnnotationToolStripMenuItem";
            this.mRemoveAnnotationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Delete)));
            this.mRemoveAnnotationToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mRemoveAnnotationToolStripMenuItem.Text = "Remove anno&tation";
            this.mRemoveAnnotationToolStripMenuItem.Click += new System.EventHandler(this.mRemoveAnnotationToolStripMenuItem_Click);
            // 
            // mFocusOnAnnotationToolStripMenuItem
            // 
            this.mFocusOnAnnotationToolStripMenuItem.Name = "mFocusOnAnnotationToolStripMenuItem";
            this.mFocusOnAnnotationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.A)));
            this.mFocusOnAnnotationToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mFocusOnAnnotationToolStripMenuItem.Text = "&Focus on annotation";
            this.mFocusOnAnnotationToolStripMenuItem.Click += new System.EventHandler(this.mFocusOnAnnotationToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(280, 6);
            // 
            // mSetPageNumberToolStripMenuItem
            // 
            this.mSetPageNumberToolStripMenuItem.Name = "mSetPageNumberToolStripMenuItem";
            this.mSetPageNumberToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.mSetPageNumberToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mSetPageNumberToolStripMenuItem.Text = "S&et page number";
            this.mSetPageNumberToolStripMenuItem.Click += new System.EventHandler(this.mSetPageNumberToolStripMenuItem_Click);
            // 
            // mRemovePageNumberToolStripMenuItem
            // 
            this.mRemovePageNumberToolStripMenuItem.Name = "mRemovePageNumberToolStripMenuItem";
            this.mRemovePageNumberToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.mRemovePageNumberToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mRemovePageNumberToolStripMenuItem.Text = "&Remove page number";
            this.mRemovePageNumberToolStripMenuItem.Click += new System.EventHandler(this.mRemovePageNumberToolStripMenuItem_Click);
            // 
            // mGoTopageToolStripMenuItem
            // 
            this.mGoTopageToolStripMenuItem.Name = "mGoTopageToolStripMenuItem";
            this.mGoTopageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.G)));
            this.mGoTopageToolStripMenuItem.Size = new System.Drawing.Size(283, 22);
            this.mGoTopageToolStripMenuItem.Text = "Go to &page";
            this.mGoTopageToolStripMenuItem.Click += new System.EventHandler(this.mGoTopageToolStripMenuItem_Click);
            // 
            // StripManagerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mFlowLayoutPanel);
            this.Controls.Add(this.label1);
            this.Name = "StripManagerPanel";
            this.mContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel mFlowLayoutPanel;
        private System.Windows.Forms.ContextMenuStrip mContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mInsertStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRenameStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mImportAudioFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mEditAnnotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSplitAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mShowInTOCViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMergeWithPreviousAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveAudioBlockForwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveAudioBlockBackwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mCutAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPasteAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCopyAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSetPageNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRemovePageNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRemoveAnnotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mApplyPhraseDetectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMarkStripAsUnusedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMarkPhraseAsUnusedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCutStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCopyStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPasteStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mQuickSplitAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mGoTopageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mFocusOnAnnotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMarkAudioBlockAsSectionHeadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem;
    }
}
