namespace Obi.ProjectView
{
    partial class ProjectView
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
            this.mHSplitter = new System.Windows.Forms.SplitContainer();
            this.mVSplitter = new System.Windows.Forms.SplitContainer();
            this.mTOCView = new Obi.ProjectView.TOCView(this);
            this.mMetadataView = new Obi.ProjectView.MetadataView();
            this.mStripsView = new Obi.ProjectView.StripsView(this);
            this.mNoProjectLabel = new System.Windows.Forms.Label();
            this.mTransportBar = new Obi.UserControls.TransportBar();
            this.mFindInText = new Obi.ProjectView.FindInText();
            this.mHSplitter.Panel1.SuspendLayout();
            this.mHSplitter.Panel2.SuspendLayout();
            this.mHSplitter.SuspendLayout();
            this.mVSplitter.Panel1.SuspendLayout();
            this.mVSplitter.Panel2.SuspendLayout();
            this.mVSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // mHSplitter
            // 
            this.mHSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mHSplitter.Location = new System.Drawing.Point(0, 0);
            this.mHSplitter.Name = "mHSplitter";
            // 
            // mHSplitter.Panel1
            // 
            this.mHSplitter.Panel1.Controls.Add(this.mVSplitter);
            // 
            // mHSplitter.Panel2
            // 
            this.mHSplitter.Panel2.Controls.Add(this.mStripsView);
            this.mHSplitter.Size = new System.Drawing.Size(972, 543);
            this.mHSplitter.SplitterDistance = 226;
            this.mHSplitter.TabIndex = 1;
            // 
            // mVSplitter
            // 
            this.mVSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mVSplitter.Location = new System.Drawing.Point(0, 0);
            this.mVSplitter.Name = "mVSplitter";
            this.mVSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mVSplitter.Panel1
            // 
            this.mVSplitter.Panel1.Controls.Add(this.mTOCView);
            // 
            // mVSplitter.Panel2
            // 
            this.mVSplitter.Panel2.Controls.Add(this.mFindInText);
            this.mVSplitter.Panel2.Controls.Add(this.mMetadataView);
            this.mVSplitter.Size = new System.Drawing.Size(226, 543);
            this.mVSplitter.SplitterDistance = 338;
            this.mVSplitter.TabIndex = 0;
            // 
            // mTOCView
            // 
            this.mTOCView.AutoScroll = true;
            this.mTOCView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCView.Location = new System.Drawing.Point(0, 0);
            this.mTOCView.Name = "mTOCView";
            this.mTOCView.Selection = null;
            this.mTOCView.Size = new System.Drawing.Size(226, 338);
            this.mTOCView.TabIndex = 0;
            // 
            // mMetadataView
            // 
            this.mMetadataView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMetadataView.Location = new System.Drawing.Point(0, 0);
            this.mMetadataView.Name = "mMetadataView";
            this.mMetadataView.Size = new System.Drawing.Size(226, 201);
            this.mMetadataView.TabIndex = 0;
            // 
            // mStripsView
            // 
            this.mStripsView.BackColor = System.Drawing.Color.Transparent;
            this.mStripsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mStripsView.Location = new System.Drawing.Point(0, 0);
            this.mStripsView.Name = "mStripsView";
            this.mStripsView.SelectedSection = null;
            this.mStripsView.Size = new System.Drawing.Size(742, 543);
            this.mStripsView.TabIndex = 0;
            // 
            // mNoProjectLabel
            // 
            this.mNoProjectLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mNoProjectLabel.AutoSize = true;
            this.mNoProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mNoProjectLabel.ForeColor = System.Drawing.Color.Orange;
            this.mNoProjectLabel.Location = new System.Drawing.Point(241, 238);
            this.mNoProjectLabel.Name = "mNoProjectLabel";
            this.mNoProjectLabel.Size = new System.Drawing.Size(490, 108);
            this.mNoProjectLabel.TabIndex = 2;
            this.mNoProjectLabel.Text = "No project";
            // 
            // mTransportBar
            // 
            this.mTransportBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mTransportBar.BackColor = System.Drawing.Color.White;
            this.mTransportBar.LocalPlaylist = null;
            this.mTransportBar.Location = new System.Drawing.Point(0, 549);
            this.mTransportBar.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.mTransportBar.Name = "mTransportBar";
            this.mTransportBar.PlayOnFocusEnabled = true;
            this.mTransportBar.ProjectPanel = null;
            this.mTransportBar.ProjectView = null;
            this.mTransportBar.Selection = null;
            this.mTransportBar.Size = new System.Drawing.Size(960, 35);
            this.mTransportBar.TabIndex = 0;
            // 
            // searchText1
            // 
            this.mFindInText.BackColor = System.Drawing.Color.Chartreuse;
            this.mFindInText.Location = new System.Drawing.Point(3, 176);
            this.mFindInText.Name = "searchText1";
            this.mFindInText.Size = new System.Drawing.Size(209, 25);
            this.mFindInText.TabIndex = 1;
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mHSplitter);
            this.Controls.Add(this.mTransportBar);
            this.Controls.Add(this.mNoProjectLabel);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(972, 584);
            this.mHSplitter.Panel1.ResumeLayout(false);
            this.mHSplitter.Panel2.ResumeLayout(false);
            this.mHSplitter.ResumeLayout(false);
            this.mVSplitter.Panel1.ResumeLayout(false);
            this.mVSplitter.Panel2.ResumeLayout(false);
            this.mVSplitter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Obi.UserControls.TransportBar mTransportBar;
        private System.Windows.Forms.SplitContainer mHSplitter;
        private StripsView mStripsView;
        private System.Windows.Forms.SplitContainer mVSplitter;
        private TOCView mTOCView;
        private MetadataView mMetadataView;
        private System.Windows.Forms.Label mNoProjectLabel;
        private FindInText mFindInText;

    }
}
