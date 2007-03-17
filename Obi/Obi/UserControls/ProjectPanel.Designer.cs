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
            this.mNoProjectLabel = new System.Windows.Forms.Label();
            this.mSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mTransportBar = new Obi.UserControls.TransportBar();
            this.mTOCPanel = new Obi.UserControls.TOCPanel();
            this.mStripManagerPanel = new Obi.UserControls.StripManagerPanel();
            this.mSplitContainer.Panel1.SuspendLayout();
            this.mSplitContainer.Panel2.SuspendLayout();
            this.mSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mNoProjectLabel
            // 
            this.mNoProjectLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mNoProjectLabel.AutoSize = true;
            this.mNoProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mNoProjectLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.mNoProjectLabel.Location = new System.Drawing.Point(369, 213);
            this.mNoProjectLabel.Name = "mNoProjectLabel";
            this.mNoProjectLabel.Size = new System.Drawing.Size(91, 20);
            this.mNoProjectLabel.TabIndex = 0;
            this.mNoProjectLabel.Text = "(No project)";
            // 
            // mSplitContainer
            // 
            this.mSplitContainer.AccessibleName = "";
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
            this.mSplitContainer.Size = new System.Drawing.Size(829, 409);
            this.mSplitContainer.SplitterDistance = 275;
            this.mSplitContainer.TabIndex = 1;
            // 
            // mTransportBar
            // 
            this.mTransportBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTransportBar.BackColor = System.Drawing.Color.White;
            this.mTransportBar.Enabled = false;
            this.mTransportBar.Location = new System.Drawing.Point(0, 415);
            this.mTransportBar.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.mTransportBar.Name = "mTransportBar";
            this.mTransportBar.Playlist = null;
            this.mTransportBar.ProjectPanel = null;
            this.mTransportBar.Size = new System.Drawing.Size(829, 32);
            this.mTransportBar.TabIndex = 2;
            // 
            // mTOCPanel
            // 
            this.mTOCPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCPanel.Location = new System.Drawing.Point(0, 0);
            this.mTOCPanel.Name = "mTOCPanel";
            this.mTOCPanel.SelectedSection = null;
            this.mTOCPanel.Size = new System.Drawing.Size(275, 409);
            this.mTOCPanel.TabIndex = 0;
            // 
            // mStripManagerPanel
            // 
            this.mStripManagerPanel.BackColor = System.Drawing.Color.Transparent;
            this.mStripManagerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mStripManagerPanel.Location = new System.Drawing.Point(0, 0);
            this.mStripManagerPanel.Name = "mStripManagerPanel";
            this.mStripManagerPanel.Padding = new System.Windows.Forms.Padding(3);
            this.mStripManagerPanel.SelectedNode = null;
            this.mStripManagerPanel.Size = new System.Drawing.Size(550, 409);
            this.mStripManagerPanel.TabIndex = 0;
            // 
            // ProjectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mTransportBar);
            this.Controls.Add(this.mSplitContainer);
            this.Controls.Add(this.mNoProjectLabel);
            this.Name = "ProjectPanel";
            this.Size = new System.Drawing.Size(829, 447);
            this.mSplitContainer.Panel1.ResumeLayout(false);
            this.mSplitContainer.Panel2.ResumeLayout(false);
            this.mSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mNoProjectLabel;
        private System.Windows.Forms.SplitContainer mSplitContainer;
        private StripManagerPanel mStripManagerPanel;
        private TOCPanel mTOCPanel;
        private TransportBar mTransportBar;

    }
}
