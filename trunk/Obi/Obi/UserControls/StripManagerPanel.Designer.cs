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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mAddStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRenameStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveStripUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveStripDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mImportAudioFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCutAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCopyAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPasteAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSplitAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mApplyPhraseDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMergeWithNextAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveAudioBlockForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveAudioBlockBackwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mEditAnnotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRemoveAnnotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mSetPageNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRemovePageNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mShowInTOCViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
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
            this.mFlowLayoutPanel.ContextMenuStrip = this.contextMenuStrip1;
            this.mFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mFlowLayoutPanel.Name = "mFlowLayoutPanel";
            this.mFlowLayoutPanel.Size = new System.Drawing.Size(150, 150);
            this.mFlowLayoutPanel.TabIndex = 1;
            this.mFlowLayoutPanel.WrapContents = false;
            this.mFlowLayoutPanel.Click += new System.EventHandler(this.mFlowLayoutPanel_Click);
            this.mFlowLayoutPanel.Leave += new System.EventHandler(this.mFlowLayoutPanel_Leave);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddStripToolStripMenuItem,
            this.mRenameStripToolStripMenuItem,
            this.mDeleteStripToolStripMenuItem,
            this.mMoveStripToolStripMenuItem,
            this.toolStripSeparator1,
            this.mImportAudioFileToolStripMenuItem,
            this.mCutAudioBlockToolStripMenuItem,
            this.mCopyAudioBlockToolStripMenuItem,
            this.mPasteAudioBlockToolStripMenuItem,
            this.mDeleteAudioBlockToolStripMenuItem,
            this.mSplitAudioBlockToolStripMenuItem,
            this.mApplyPhraseDetectionToolStripMenuItem,
            this.mMergeWithNextAudioBlockToolStripMenuItem,
            this.mMoveAudioBlockToolStripMenuItem,
            this.toolStripSeparator3,
            this.mEditAnnotationToolStripMenuItem,
            this.mRemoveAnnotationToolStripMenuItem,
            this.toolStripSeparator4,
            this.mSetPageNumberToolStripMenuItem,
            this.mRemovePageNumberToolStripMenuItem,
            this.toolStripSeparator2,
            this.mShowInTOCViewToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(224, 446);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mAddStripToolStripMenuItem
            // 
            this.mAddStripToolStripMenuItem.Name = "mAddStripToolStripMenuItem";
            this.mAddStripToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mAddStripToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mAddStripToolStripMenuItem.Text = "&Add strip";
            this.mAddStripToolStripMenuItem.Click += new System.EventHandler(this.mAddStripToolStripMenuItem_Click);
            // 
            // mRenameStripToolStripMenuItem
            // 
            this.mRenameStripToolStripMenuItem.Name = "mRenameStripToolStripMenuItem";
            this.mRenameStripToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.mRenameStripToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mRenameStripToolStripMenuItem.Text = "Re&name strip";
            this.mRenameStripToolStripMenuItem.Click += new System.EventHandler(this.mRenameStripToolStripMenuItem_Click);
            // 
            // mDeleteStripToolStripMenuItem
            // 
            this.mDeleteStripToolStripMenuItem.Name = "mDeleteStripToolStripMenuItem";
            this.mDeleteStripToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Delete)));
            this.mDeleteStripToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mDeleteStripToolStripMenuItem.Text = "&Delete strip";
            this.mDeleteStripToolStripMenuItem.Click += new System.EventHandler(this.mDeleteStripToolStripMenuItem_Click);
            // 
            // mMoveStripToolStripMenuItem
            // 
            this.mMoveStripToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mMoveStripUpToolStripMenuItem,
            this.mMoveStripDownToolStripMenuItem});
            this.mMoveStripToolStripMenuItem.Name = "mMoveStripToolStripMenuItem";
            this.mMoveStripToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mMoveStripToolStripMenuItem.Text = "&Move strip...";
            // 
            // mMoveStripUpToolStripMenuItem
            // 
            this.mMoveStripUpToolStripMenuItem.Name = "mMoveStripUpToolStripMenuItem";
            this.mMoveStripUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Up)));
            this.mMoveStripUpToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.mMoveStripUpToolStripMenuItem.Text = "&Up";
            this.mMoveStripUpToolStripMenuItem.Click += new System.EventHandler(this.upToolStripMenuItem_Click);
            // 
            // mMoveStripDownToolStripMenuItem
            // 
            this.mMoveStripDownToolStripMenuItem.Name = "mMoveStripDownToolStripMenuItem";
            this.mMoveStripDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Down)));
            this.mMoveStripDownToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.mMoveStripDownToolStripMenuItem.Text = "&Down";
            this.mMoveStripDownToolStripMenuItem.Click += new System.EventHandler(this.downToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // mImportAudioFileToolStripMenuItem
            // 
            this.mImportAudioFileToolStripMenuItem.Name = "mImportAudioFileToolStripMenuItem";
            this.mImportAudioFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.mImportAudioFileToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mImportAudioFileToolStripMenuItem.Text = "&Import audio";
            this.mImportAudioFileToolStripMenuItem.Click += new System.EventHandler(this.mImportAudioToolStripMenuItem_Click);
            // 
            // mCutAudioBlockToolStripMenuItem
            // 
            this.mCutAudioBlockToolStripMenuItem.Name = "mCutAudioBlockToolStripMenuItem";
            this.mCutAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mCutAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mCutAudioBlockToolStripMenuItem.Text = "&Cut phrase";
            this.mCutAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mCutBlockToolStripMenuItem_Click);
            // 
            // mCopyAudioBlockToolStripMenuItem
            // 
            this.mCopyAudioBlockToolStripMenuItem.Name = "mCopyAudioBlockToolStripMenuItem";
            this.mCopyAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mCopyAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mCopyAudioBlockToolStripMenuItem.Text = "Cop&y phrase";
            this.mCopyAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mCopyAudioBlockToolStripMenuItem_Click);
            // 
            // mPasteAudioBlockToolStripMenuItem
            // 
            this.mPasteAudioBlockToolStripMenuItem.Name = "mPasteAudioBlockToolStripMenuItem";
            this.mPasteAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mPasteAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mPasteAudioBlockToolStripMenuItem.Text = "Past&e phrase";
            this.mPasteAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mPasteAudioBlockToolStripMenuItem_Click);
            // 
            // mDeleteAudioBlockToolStripMenuItem
            // 
            this.mDeleteAudioBlockToolStripMenuItem.Name = "mDeleteAudioBlockToolStripMenuItem";
            this.mDeleteAudioBlockToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mDeleteAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mDeleteAudioBlockToolStripMenuItem.Text = "De&lete phrase";
            this.mDeleteAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mDeleteAudioBlockToolStripMenuItem_Click);
            // 
            // mSplitAudioBlockToolStripMenuItem
            // 
            this.mSplitAudioBlockToolStripMenuItem.Name = "mSplitAudioBlockToolStripMenuItem";
            this.mSplitAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mSplitAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mSplitAudioBlockToolStripMenuItem.Text = "&Split audio block";
            this.mSplitAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mSplitAudioBlockToolStripMenuItem_Click);
            // 
            // mApplyPhraseDetectionToolStripMenuItem
            // 
            this.mApplyPhraseDetectionToolStripMenuItem.Name = "mApplyPhraseDetectionToolStripMenuItem";
            this.mApplyPhraseDetectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.mApplyPhraseDetectionToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mApplyPhraseDetectionToolStripMenuItem.Text = "Apply p&hrase detection";
            this.mApplyPhraseDetectionToolStripMenuItem.Click += new System.EventHandler(this.mApplyPhraseDetectionToolStripMenuItem_Click);
            // 
            // mMergeWithNextAudioBlockToolStripMenuItem
            // 
            this.mMergeWithNextAudioBlockToolStripMenuItem.Name = "mMergeWithNextAudioBlockToolStripMenuItem";
            this.mMergeWithNextAudioBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.mMergeWithNextAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mMergeWithNextAudioBlockToolStripMenuItem.Text = "Mer&ge with next audio block";
            this.mMergeWithNextAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mMergeWithNextAudioBlockToolStripMenuItem_Click);
            // 
            // mMoveAudioBlockToolStripMenuItem
            // 
            this.mMoveAudioBlockToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mMoveAudioBlockForwardToolStripMenuItem,
            this.mMoveAudioBlockBackwardToolStripMenuItem});
            this.mMoveAudioBlockToolStripMenuItem.Name = "mMoveAudioBlockToolStripMenuItem";
            this.mMoveAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mMoveAudioBlockToolStripMenuItem.Text = "Mo&ve audio block...";
            // 
            // mMoveAudioBlockForwardToolStripMenuItem
            // 
            this.mMoveAudioBlockForwardToolStripMenuItem.Name = "mMoveAudioBlockForwardToolStripMenuItem";
            this.mMoveAudioBlockForwardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
            this.mMoveAudioBlockForwardToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mMoveAudioBlockForwardToolStripMenuItem.Text = "&Forward";
            this.mMoveAudioBlockForwardToolStripMenuItem.Click += new System.EventHandler(this.mMoveAudioBlockForwardToolStripMenuItem_Click);
            // 
            // mMoveAudioBlockBackwardToolStripMenuItem
            // 
            this.mMoveAudioBlockBackwardToolStripMenuItem.Name = "mMoveAudioBlockBackwardToolStripMenuItem";
            this.mMoveAudioBlockBackwardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Left)));
            this.mMoveAudioBlockBackwardToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mMoveAudioBlockBackwardToolStripMenuItem.Text = "&Backward";
            this.mMoveAudioBlockBackwardToolStripMenuItem.Click += new System.EventHandler(this.mMoveAudioBlockBackwardToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(220, 6);
            // 
            // mEditAnnotationToolStripMenuItem
            // 
            this.mEditAnnotationToolStripMenuItem.Name = "mEditAnnotationToolStripMenuItem";
            this.mEditAnnotationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mEditAnnotationToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mEditAnnotationToolStripMenuItem.Text = "&Edit annotation";
            this.mEditAnnotationToolStripMenuItem.Click += new System.EventHandler(this.mEditAudioBlockLabelToolStripMenuItem_Click);
            // 
            // mRemoveAnnotationToolStripMenuItem
            // 
            this.mRemoveAnnotationToolStripMenuItem.Name = "mRemoveAnnotationToolStripMenuItem";
            this.mRemoveAnnotationToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mRemoveAnnotationToolStripMenuItem.Text = "Rem&ove annotation";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(220, 6);
            // 
            // mSetPageNumberToolStripMenuItem
            // 
            this.mSetPageNumberToolStripMenuItem.Name = "mSetPageNumberToolStripMenuItem";
            this.mSetPageNumberToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.mSetPageNumberToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mSetPageNumberToolStripMenuItem.Text = "Se&t page number";
            this.mSetPageNumberToolStripMenuItem.Click += new System.EventHandler(this.mSetPageNumberToolStripMenuItem_Click);
            // 
            // mRemovePageNumberToolStripMenuItem
            // 
            this.mRemovePageNumberToolStripMenuItem.Name = "mRemovePageNumberToolStripMenuItem";
            this.mRemovePageNumberToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.mRemovePageNumberToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mRemovePageNumberToolStripMenuItem.Text = "&Remove page number";
            this.mRemovePageNumberToolStripMenuItem.Click += new System.EventHandler(this.mRemovePageNumberToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // mShowInTOCViewToolStripMenuItem
            // 
            this.mShowInTOCViewToolStripMenuItem.Name = "mShowInTOCViewToolStripMenuItem";
            this.mShowInTOCViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
            this.mShowInTOCViewToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.mShowInTOCViewToolStripMenuItem.Text = "Show in TOC vie&w";
            this.mShowInTOCViewToolStripMenuItem.Click += new System.EventHandler(this.mShowInTOCViewToolStripMenuItem_Click);
            // 
            // StripManagerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mFlowLayoutPanel);
            this.Controls.Add(this.label1);
            this.Name = "StripManagerPanel";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel mFlowLayoutPanel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mAddStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRenameStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mImportAudioFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mEditAnnotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSplitAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mShowInTOCViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMergeWithNextAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveStripUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveStripDownToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mRemoveAnnotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mApplyPhraseDetectionToolStripMenuItem;
    }
}
