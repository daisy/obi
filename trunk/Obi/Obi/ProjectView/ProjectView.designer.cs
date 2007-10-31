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
            this.mTransportBarSplitter = new System.Windows.Forms.SplitContainer();
            this.mFindInTextSplitter = new System.Windows.Forms.SplitContainer();
            this.mTOCSplitter = new System.Windows.Forms.SplitContainer();
            this.mMetadataSplitter = new System.Windows.Forms.SplitContainer();
            this.mNoProjectLabel = new System.Windows.Forms.Label();
            this.mTOCView = new Obi.ProjectView.TOCView();
            this.mMetadataView = new Obi.ProjectView.MetadataView();
            this.mStripsView = new Obi.ProjectView.StripsView();
            this.mFindInText = new Obi.ProjectView.FindInText();
            this.mTransportBar = new Obi.ProjectView.TransportBar();
            this.mTransportBarSplitter.Panel1.SuspendLayout();
            this.mTransportBarSplitter.Panel2.SuspendLayout();
            this.mTransportBarSplitter.SuspendLayout();
            this.mFindInTextSplitter.Panel1.SuspendLayout();
            this.mFindInTextSplitter.Panel2.SuspendLayout();
            this.mFindInTextSplitter.SuspendLayout();
            this.mTOCSplitter.Panel1.SuspendLayout();
            this.mTOCSplitter.Panel2.SuspendLayout();
            this.mTOCSplitter.SuspendLayout();
            this.mMetadataSplitter.Panel1.SuspendLayout();
            this.mMetadataSplitter.Panel2.SuspendLayout();
            this.mMetadataSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTransportBarSplitter
            // 
            this.mTransportBarSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTransportBarSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mTransportBarSplitter.IsSplitterFixed = true;
            this.mTransportBarSplitter.Location = new System.Drawing.Point(0, 0);
            this.mTransportBarSplitter.Name = "mTransportBarSplitter";
            this.mTransportBarSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mTransportBarSplitter.Panel1
            // 
            this.mTransportBarSplitter.Panel1.Controls.Add(this.mFindInTextSplitter);
            // 
            // mTransportBarSplitter.Panel2
            // 
            this.mTransportBarSplitter.Panel2.Controls.Add(this.mTransportBar);
            this.mTransportBarSplitter.Panel2MinSize = 35;
            this.mTransportBarSplitter.Size = new System.Drawing.Size(964, 700);
            this.mTransportBarSplitter.SplitterDistance = 660;
            this.mTransportBarSplitter.TabIndex = 0;
            // 
            // mFindInTextSplitter
            // 
            this.mFindInTextSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFindInTextSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mFindInTextSplitter.IsSplitterFixed = true;
            this.mFindInTextSplitter.Location = new System.Drawing.Point(0, 0);
            this.mFindInTextSplitter.Name = "mFindInTextSplitter";
            this.mFindInTextSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mFindInTextSplitter.Panel1
            // 
            this.mFindInTextSplitter.Panel1.Controls.Add(this.mTOCSplitter);
            // 
            // mFindInTextSplitter.Panel2
            // 
            this.mFindInTextSplitter.Panel2.Controls.Add(this.mFindInText);
            this.mFindInTextSplitter.Panel2MinSize = 26;
            this.mFindInTextSplitter.Size = new System.Drawing.Size(964, 660);
            this.mFindInTextSplitter.SplitterDistance = 629;
            this.mFindInTextSplitter.TabIndex = 0;
            // 
            // mTOCSplitter
            // 
            this.mTOCSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCSplitter.Location = new System.Drawing.Point(0, 0);
            this.mTOCSplitter.Name = "mTOCSplitter";
            // 
            // mTOCSplitter.Panel1
            // 
            this.mTOCSplitter.Panel1.Controls.Add(this.mMetadataSplitter);
            // 
            // mTOCSplitter.Panel2
            // 
            this.mTOCSplitter.Panel2.Controls.Add(this.mStripsView);
            this.mTOCSplitter.Size = new System.Drawing.Size(964, 629);
            this.mTOCSplitter.SplitterDistance = 271;
            this.mTOCSplitter.TabIndex = 0;
            // 
            // mMetadataSplitter
            // 
            this.mMetadataSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMetadataSplitter.Location = new System.Drawing.Point(0, 0);
            this.mMetadataSplitter.Name = "mMetadataSplitter";
            this.mMetadataSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mMetadataSplitter.Panel1
            // 
            this.mMetadataSplitter.Panel1.Controls.Add(this.mTOCView);
            // 
            // mMetadataSplitter.Panel2
            // 
            this.mMetadataSplitter.Panel2.Controls.Add(this.mMetadataView);
            this.mMetadataSplitter.Size = new System.Drawing.Size(271, 629);
            this.mMetadataSplitter.SplitterDistance = 381;
            this.mMetadataSplitter.TabIndex = 0;
            // 
            // mNoProjectLabel
            // 
            this.mNoProjectLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mNoProjectLabel.AutoSize = true;
            this.mNoProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mNoProjectLabel.Location = new System.Drawing.Point(442, 340);
            this.mNoProjectLabel.Name = "mNoProjectLabel";
            this.mNoProjectLabel.Size = new System.Drawing.Size(81, 20);
            this.mNoProjectLabel.TabIndex = 1;
            this.mNoProjectLabel.Text = "No project";
            // 
            // mTOCView
            // 
            this.mTOCView.AutoScroll = true;
            this.mTOCView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCView.Location = new System.Drawing.Point(0, 0);
            this.mTOCView.Name = "mTOCView";
            this.mTOCView.Selection = null;
            this.mTOCView.Size = new System.Drawing.Size(271, 381);
            this.mTOCView.TabIndex = 0;
            // 
            // mMetadataView
            // 
            this.mMetadataView.BackColor = System.Drawing.Color.PeachPuff;
            this.mMetadataView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMetadataView.Location = new System.Drawing.Point(0, 0);
            this.mMetadataView.Name = "mMetadataView";
            this.mMetadataView.Size = new System.Drawing.Size(271, 244);
            this.mMetadataView.TabIndex = 0;
            // 
            // mStripsView
            // 
            this.mStripsView.BackColor = System.Drawing.Color.LightYellow;
            this.mStripsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mStripsView.Location = new System.Drawing.Point(0, 0);
            this.mStripsView.Name = "mStripsView";
            this.mStripsView.SelectedPhrase = null;
            this.mStripsView.SelectedSection = null;
            this.mStripsView.Selection = null;
            this.mStripsView.Size = new System.Drawing.Size(689, 629);
            this.mStripsView.TabIndex = 0;
            // 
            // mFindInText
            // 
            this.mFindInText.BackColor = System.Drawing.Color.Honeydew;
            this.mFindInText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFindInText.Location = new System.Drawing.Point(0, 0);
            this.mFindInText.Name = "mFindInText";
            this.mFindInText.Size = new System.Drawing.Size(964, 27);
            this.mFindInText.TabIndex = 0;
            // 
            // mTransportBar
            // 
            this.mTransportBar.BackColor = System.Drawing.Color.White;
            this.mTransportBar.LocalPlaylist = null;
            this.mTransportBar.Location = new System.Drawing.Point(0, 0);
            this.mTransportBar.Name = "mTransportBar";
            this.mTransportBar.PlayOnFocusEnabled = true;
            this.mTransportBar.ProjectView = null;
            this.mTransportBar.Selection = null;
            this.mTransportBar.Size = new System.Drawing.Size(1060, 35);
            this.mTransportBar.TabIndex = 0;
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mNoProjectLabel);
            this.Controls.Add(this.mTransportBarSplitter);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(964, 700);
            this.mTransportBarSplitter.Panel1.ResumeLayout(false);
            this.mTransportBarSplitter.Panel2.ResumeLayout(false);
            this.mTransportBarSplitter.ResumeLayout(false);
            this.mFindInTextSplitter.Panel1.ResumeLayout(false);
            this.mFindInTextSplitter.Panel2.ResumeLayout(false);
            this.mFindInTextSplitter.ResumeLayout(false);
            this.mTOCSplitter.Panel1.ResumeLayout(false);
            this.mTOCSplitter.Panel2.ResumeLayout(false);
            this.mTOCSplitter.ResumeLayout(false);
            this.mMetadataSplitter.Panel1.ResumeLayout(false);
            this.mMetadataSplitter.Panel2.ResumeLayout(false);
            this.mMetadataSplitter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer mTransportBarSplitter;
        private TransportBar mTransportBar;
        private System.Windows.Forms.SplitContainer mFindInTextSplitter;
        private System.Windows.Forms.SplitContainer mTOCSplitter;
        private StripsView mStripsView;
        private FindInText mFindInText;
        private System.Windows.Forms.SplitContainer mMetadataSplitter;
        private TOCView mTOCView;
        private MetadataView mMetadataView;
        private System.Windows.Forms.Label mNoProjectLabel;


    }
}
