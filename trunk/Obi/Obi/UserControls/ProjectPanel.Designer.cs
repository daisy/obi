namespace Obi.UserControls
{
    partial class ProjectPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectPanel));
            this.mNoProjectLabel = new System.Windows.Forms.Label();
            this.mSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mTransportBarPanel = new System.Windows.Forms.Panel();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mTOCPanel = new Obi.UserControls.TOCPanel();
            this.mStripManagerPanel = new Obi.UserControls.StripManagerPanel();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mSplitContainer.Panel1.SuspendLayout();
            this.mSplitContainer.Panel2.SuspendLayout();
            this.mSplitContainer.SuspendLayout();
            this.mTransportBarPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mNoProjectLabel
            // 
            this.mNoProjectLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mNoProjectLabel.AutoSize = true;
            this.mNoProjectLabel.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mNoProjectLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.mNoProjectLabel.Location = new System.Drawing.Point(285, 159);
            this.mNoProjectLabel.Name = "mNoProjectLabel";
            this.mNoProjectLabel.Size = new System.Drawing.Size(66, 12);
            this.mNoProjectLabel.TabIndex = 0;
            this.mNoProjectLabel.Text = "(No project)";
            // 
            // mSplitContainer
            // 
            this.mSplitContainer.AccessibleDescription = "";
            this.mSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mSplitContainer.BackColor = System.Drawing.Color.Transparent;
            this.mSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mSplitContainer.Name = "mSplitContainer";
            // 
            // mSplitContainer.Panel1
            // 
            this.mSplitContainer.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.mSplitContainer.Panel1.Controls.Add(this.mTOCPanel);
            // 
            // mSplitContainer.Panel2
            // 
            this.mSplitContainer.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.mSplitContainer.Panel2.Controls.Add(this.mStripManagerPanel);
            this.mSplitContainer.Size = new System.Drawing.Size(631, 289);
            this.mSplitContainer.SplitterDistance = 210;
            this.mSplitContainer.TabIndex = 1;
            // 
            // mTransportBarPanel
            // 
            this.mTransportBarPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTransportBarPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mTransportBarPanel.Controls.Add(this.mStopButton);
            this.mTransportBarPanel.Controls.Add(this.mPlayButton);
            this.mTransportBarPanel.Controls.Add(this.mPauseButton);
            this.mTransportBarPanel.Location = new System.Drawing.Point(0, 292);
            this.mTransportBarPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.mTransportBarPanel.Name = "mTransportBarPanel";
            this.mTransportBarPanel.Size = new System.Drawing.Size(631, 38);
            this.mTransportBarPanel.TabIndex = 2;
            // 
            // mPlayButton
            // 
            this.mPlayButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mPlayButton.BackgroundImage")));
            this.mPlayButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPlayButton.Location = new System.Drawing.Point(3, 3);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(32, 32);
            this.mPlayButton.TabIndex = 1;
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mPauseButton.BackgroundImage")));
            this.mPauseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPauseButton.Location = new System.Drawing.Point(3, 3);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(32, 32);
            this.mPauseButton.TabIndex = 2;
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Visible = false;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mTOCPanel
            // 
            this.mTOCPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCPanel.Location = new System.Drawing.Point(0, 0);
            this.mTOCPanel.Name = "mTOCPanel";
            this.mTOCPanel.SelectedSection = null;
            this.mTOCPanel.Size = new System.Drawing.Size(210, 289);
            this.mTOCPanel.TabIndex = 0;
            // 
            // mStripManagerPanel
            // 
            this.mStripManagerPanel.BackColor = System.Drawing.Color.Transparent;
            this.mStripManagerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mStripManagerPanel.Location = new System.Drawing.Point(0, 0);
            this.mStripManagerPanel.Name = "mStripManagerPanel";
            this.mStripManagerPanel.SelectedNode = null;
            this.mStripManagerPanel.SelectedPhraseNode = null;
            this.mStripManagerPanel.SelectedSectionNode = null;
            this.mStripManagerPanel.Size = new System.Drawing.Size(417, 289);
            this.mStripManagerPanel.TabIndex = 0;
            // 
            // mStopButton
            // 
            this.mStopButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mStopButton.BackgroundImage")));
            this.mStopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mStopButton.Location = new System.Drawing.Point(41, 3);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(32, 32);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // ProjectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mTransportBarPanel);
            this.Controls.Add(this.mSplitContainer);
            this.Controls.Add(this.mNoProjectLabel);
            this.Name = "ProjectPanel";
            this.Size = new System.Drawing.Size(631, 330);
            this.mSplitContainer.Panel1.ResumeLayout(false);
            this.mSplitContainer.Panel2.ResumeLayout(false);
            this.mSplitContainer.ResumeLayout(false);
            this.mTransportBarPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mNoProjectLabel;
        private System.Windows.Forms.SplitContainer mSplitContainer;
        private StripManagerPanel mStripManagerPanel;
        private TOCPanel mTOCPanel;
        private System.Windows.Forms.Panel mTransportBarPanel;
        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.Button mStopButton;

    }
}
