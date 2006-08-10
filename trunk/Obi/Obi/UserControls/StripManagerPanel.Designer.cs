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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mImportAudioFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPlayAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSplitAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMergeWithNextAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRenameAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripSeparator1,
            this.mImportAudioFileToolStripMenuItem,
            this.mPlayAudioBlockToolStripMenuItem,
            this.mSplitAudioBlockToolStripMenuItem,
            this.mMergeWithNextAudioBlockToolStripMenuItem,
            this.mDeleteAudioBlockToolStripMenuItem,
            this.mRenameAudioBlockToolStripMenuItem,
            this.toolStripSeparator2,
            this.mShowInTOCViewToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(190, 236);
            // 
            // mAddStripToolStripMenuItem
            // 
            this.mAddStripToolStripMenuItem.Name = "mAddStripToolStripMenuItem";
            this.mAddStripToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mAddStripToolStripMenuItem.Text = "&Add strip";
            this.mAddStripToolStripMenuItem.Click += new System.EventHandler(this.mAddStripToolStripMenuItem_Click);
            // 
            // mRenameStripToolStripMenuItem
            // 
            this.mRenameStripToolStripMenuItem.Name = "mRenameStripToolStripMenuItem";
            this.mRenameStripToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mRenameStripToolStripMenuItem.Text = "&Rename strip";
            this.mRenameStripToolStripMenuItem.Click += new System.EventHandler(this.mRenameStripToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // mImportAudioFileToolStripMenuItem
            // 
            this.mImportAudioFileToolStripMenuItem.Name = "mImportAudioFileToolStripMenuItem";
            this.mImportAudioFileToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mImportAudioFileToolStripMenuItem.Text = "&Import audio file";
            this.mImportAudioFileToolStripMenuItem.Click += new System.EventHandler(this.mImportAssetToolStripMenuItem_Click);
            // 
            // mPlayAudioBlockToolStripMenuItem
            // 
            this.mPlayAudioBlockToolStripMenuItem.Name = "mPlayAudioBlockToolStripMenuItem";
            this.mPlayAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mPlayAudioBlockToolStripMenuItem.Text = "&Play audio block";
            this.mPlayAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mPlayAudioBlockToolStripMenuItem_Click);
            // 
            // mSplitAudioBlockToolStripMenuItem
            // 
            this.mSplitAudioBlockToolStripMenuItem.Name = "mSplitAudioBlockToolStripMenuItem";
            this.mSplitAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mSplitAudioBlockToolStripMenuItem.Text = "&Split audio block";
            this.mSplitAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mSplitAudioBlockToolStripMenuItem_Click);
            // 
            // mMergeWithNextAudioBlockToolStripMenuItem
            // 
            this.mMergeWithNextAudioBlockToolStripMenuItem.Name = "mMergeWithNextAudioBlockToolStripMenuItem";
            this.mMergeWithNextAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mMergeWithNextAudioBlockToolStripMenuItem.Text = "&Merge with next audio block";
            this.mMergeWithNextAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mMergeWithNextAudioBlockToolStripMenuItem_Click);
            // 
            // mDeleteAudioBlockToolStripMenuItem
            // 
            this.mDeleteAudioBlockToolStripMenuItem.Name = "mDeleteAudioBlockToolStripMenuItem";
            this.mDeleteAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mDeleteAudioBlockToolStripMenuItem.Text = "&Delete audio block";
            this.mDeleteAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mDeleteAudioBlockToolStripMenuItem_Click);
            // 
            // mRenameAudioBlockToolStripMenuItem
            // 
            this.mRenameAudioBlockToolStripMenuItem.Name = "mRenameAudioBlockToolStripMenuItem";
            this.mRenameAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mRenameAudioBlockToolStripMenuItem.Text = "Re&name audio block";
            this.mRenameAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.mRenameAudioBlockToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // mShowInTOCViewToolStripMenuItem
            // 
            this.mShowInTOCViewToolStripMenuItem.Name = "mShowInTOCViewToolStripMenuItem";
            this.mShowInTOCViewToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mShowInTOCViewToolStripMenuItem.Text = "Show in &TOC view";
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
        private System.Windows.Forms.ToolStripMenuItem mPlayAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRenameAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSplitAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mShowInTOCViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mDeleteAudioBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMergeWithNextAudioBlockToolStripMenuItem;
    }
}
