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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectView));
            this.mTransportBarSplitter = new System.Windows.Forms.SplitContainer();
            this.mFindInTextSplitter = new System.Windows.Forms.SplitContainer();
            this.mTOCSplitter = new System.Windows.Forms.SplitContainer();
            this.mMetadataSplitter = new System.Windows.Forms.SplitContainer();
            this.mTOCView = new Obi.ProjectView.TOCView();
            this.mMetadataView = new Obi.ProjectView.MetadataView();
            this.mPanelInfoLabelButton = new System.Windows.Forms.Button();
            this.mContentView = new Obi.ProjectView.ContentView();
            this.mFindInText = new Obi.ProjectView.FindInText();
            this.mTransportBar = new Obi.ProjectView.TransportBar();
            this.mNoProjectLabel = new System.Windows.Forms.Label();
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
            this.mTransportBarSplitter.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.mTransportBarSplitter, "mTransportBarSplitter");
            this.mTransportBarSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mTransportBarSplitter.Name = "mTransportBarSplitter";
            // 
            // mTransportBarSplitter.Panel1
            // 
            this.mTransportBarSplitter.Panel1.Controls.Add(this.mFindInTextSplitter);
            // 
            // mTransportBarSplitter.Panel2
            // 
            this.mTransportBarSplitter.Panel2.Controls.Add(this.mTransportBar);
            // 
            // mFindInTextSplitter
            // 
            this.mFindInTextSplitter.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.mFindInTextSplitter, "mFindInTextSplitter");
            this.mFindInTextSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mFindInTextSplitter.Name = "mFindInTextSplitter";
            // 
            // mFindInTextSplitter.Panel1
            // 
            this.mFindInTextSplitter.Panel1.Controls.Add(this.mTOCSplitter);
            // 
            // mFindInTextSplitter.Panel2
            // 
            this.mFindInTextSplitter.Panel2.BackColor = System.Drawing.Color.Red;
            this.mFindInTextSplitter.Panel2.Controls.Add(this.mFindInText);
            // 
            // mTOCSplitter
            // 
            resources.ApplyResources(this.mTOCSplitter, "mTOCSplitter");
            this.mTOCSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.mTOCSplitter.Name = "mTOCSplitter";
            // 
            // mTOCSplitter.Panel1
            // 
            this.mTOCSplitter.Panel1.Controls.Add(this.mMetadataSplitter);
            this.mTOCSplitter.Panel1.Controls.Add(this.mPanelInfoLabelButton);
            // 
            // mTOCSplitter.Panel2
            // 
            this.mTOCSplitter.Panel2.Controls.Add(this.mContentView);
            // 
            // mMetadataSplitter
            // 
            resources.ApplyResources(this.mMetadataSplitter, "mMetadataSplitter");
            this.mMetadataSplitter.Name = "mMetadataSplitter";
            // 
            // mMetadataSplitter.Panel1
            // 
            this.mMetadataSplitter.Panel1.Controls.Add(this.mTOCView);
            // 
            // mMetadataSplitter.Panel2
            // 
            this.mMetadataSplitter.Panel2.Controls.Add(this.mMetadataView);
            // 
            // mTOCView
            // 
            this.mTOCView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.mTOCView, "mTOCView");
            this.mTOCView.FullRowSelect = true;
            this.mTOCView.LabelEdit = true;
            this.mTOCView.Name = "mTOCView";
            this.mTOCView.Selection = null;
            // 
            // mMetadataView
            // 
            this.mMetadataView.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.mMetadataView, "mMetadataView");
            this.mMetadataView.Name = "mMetadataView";
            this.mMetadataView.Selection = null;
            // 
            // mPanelInfoLabelButton
            // 
            resources.ApplyResources(this.mPanelInfoLabelButton, "mPanelInfoLabelButton");
            this.mPanelInfoLabelButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPanelInfoLabelButton.BackColor = System.Drawing.Color.Transparent;
            this.mPanelInfoLabelButton.Name = "mPanelInfoLabelButton";
            this.mPanelInfoLabelButton.UseVisualStyleBackColor = false;
            this.mPanelInfoLabelButton.Leave += new System.EventHandler(this.mPanelInfoLabelButton_Leave);
            this.mPanelInfoLabelButton.Enter += new System.EventHandler(this.mPanelInfoLabelButton_Enter);
            // 
            // mContentView
            // 
            this.mContentView.AudioScale = 0.01F;
            this.mContentView.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.mContentView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mContentView.ColorSettings = null;
            resources.ApplyResources(this.mContentView, "mContentView");
            this.mContentView.Name = "mContentView";
            this.mContentView.Selection = null;
            this.mContentView.ZoomFactor = 1F;
            // 
            // mFindInText
            // 
            resources.ApplyResources(this.mFindInText, "mFindInText");
            this.mFindInText.BackColor = System.Drawing.SystemColors.Control;
            this.mFindInText.Name = "mFindInText";
            // 
            // mTransportBar
            // 
            this.mTransportBar.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.mTransportBar, "mTransportBar");
            this.mTransportBar.LocalPlaylist = null;
            this.mTransportBar.Name = "mTransportBar";
            this.mTransportBar.SelectionChangedPlaybackEnabled = true;
            // 
            // mNoProjectLabel
            // 
            resources.ApplyResources(this.mNoProjectLabel, "mNoProjectLabel");
            this.mNoProjectLabel.BackColor = System.Drawing.SystemColors.Control;
            this.mNoProjectLabel.Name = "mNoProjectLabel";
            // 
            // ProjectView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Controls.Add(this.mNoProjectLabel);
            this.Controls.Add(this.mTransportBarSplitter);
            this.DoubleBuffered = true;
            this.Name = "ProjectView";
            this.mTransportBarSplitter.Panel1.ResumeLayout(false);
            this.mTransportBarSplitter.Panel2.ResumeLayout(false);
            this.mTransportBarSplitter.ResumeLayout(false);
            this.mFindInTextSplitter.Panel1.ResumeLayout(false);
            this.mFindInTextSplitter.Panel2.ResumeLayout(false);
            this.mFindInTextSplitter.ResumeLayout(false);
            this.mTOCSplitter.Panel1.ResumeLayout(false);
            this.mTOCSplitter.Panel1.PerformLayout();
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
        private FindInText mFindInText;
        private System.Windows.Forms.SplitContainer mMetadataSplitter;
        private MetadataView mMetadataView;
        private System.Windows.Forms.Label mNoProjectLabel;
        private System.Windows.Forms.Button mPanelInfoLabelButton ;
        private TOCView mTOCView;
        private ContentView mContentView;


    }
}
