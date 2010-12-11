namespace Obi.UserControls
{
    partial class RecordingToolBarForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordingToolBarForm));
            this.recordingToolBarToolStrip = new System.Windows.Forms.ToolStrip();
            this.m_recordingToolBarPlayBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarStopBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarRecordingBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarPrePhraseBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingGoToNextPhraseBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarNextPageBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarNextSectionBtn = new System.Windows.Forms.ToolStripButton();
            this.m_statusStrip = new System.Windows.Forms.StatusStrip();
            this.recordingToolStripStatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.recordingToolBarToolStrip.SuspendLayout();
            this.m_statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // recordingToolBarToolStrip
            // 
            this.recordingToolBarToolStrip.AutoSize = false;
            this.recordingToolBarToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_recordingToolBarPlayBtn,
            this.m_recordingToolBarStopBtn,
            this.m_recordingToolBarRecordingBtn,
            this.m_recordingToolBarPrePhraseBtn,
            this.m_recordingGoToNextPhraseBtn,
            this.m_recordingToolBarNextPageBtn,
            this.m_recordingToolBarNextSectionBtn});
            this.recordingToolBarToolStrip.Location = new System.Drawing.Point(0, 0);
            this.recordingToolBarToolStrip.Name = "recordingToolBarToolStrip";
            this.recordingToolBarToolStrip.Size = new System.Drawing.Size(284, 38);
            this.recordingToolBarToolStrip.TabIndex = 0;
            this.recordingToolBarToolStrip.Text = "toolStrip1";
            // 
            // m_recordingToolBarPlayBtn
            // 
            this.m_recordingToolBarPlayBtn.AutoSize = false;
            this.m_recordingToolBarPlayBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarPlayBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_recordingToolBarPlayBtn.Image")));
            this.m_recordingToolBarPlayBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_recordingToolBarPlayBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_recordingToolBarPlayBtn.Name = "m_recordingToolBarPlayBtn";
            this.m_recordingToolBarPlayBtn.Size = new System.Drawing.Size(32, 32);
            this.m_recordingToolBarPlayBtn.Text = "Play/Pause";
            this.m_recordingToolBarPlayBtn.Click += new System.EventHandler(this.m_recordingToolBarPlayBtn_Click);
            // 
            // m_recordingToolBarStopBtn
            // 
            this.m_recordingToolBarStopBtn.AutoSize = false;
            this.m_recordingToolBarStopBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarStopBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_recordingToolBarStopBtn.Image")));
            this.m_recordingToolBarStopBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_recordingToolBarStopBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_recordingToolBarStopBtn.Name = "m_recordingToolBarStopBtn";
            this.m_recordingToolBarStopBtn.Size = new System.Drawing.Size(32, 32);
            this.m_recordingToolBarStopBtn.Text = "Stop";
            this.m_recordingToolBarStopBtn.Click += new System.EventHandler(this.m_recordingToolBarStopBtn_Click);
            // 
            // m_recordingToolBarRecordingBtn
            // 
            this.m_recordingToolBarRecordingBtn.AutoSize = false;
            this.m_recordingToolBarRecordingBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarRecordingBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_recordingToolBarRecordingBtn.Image")));
            this.m_recordingToolBarRecordingBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_recordingToolBarRecordingBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_recordingToolBarRecordingBtn.Name = "m_recordingToolBarRecordingBtn";
            this.m_recordingToolBarRecordingBtn.Size = new System.Drawing.Size(32, 32);
            this.m_recordingToolBarRecordingBtn.Text = "Record";
            this.m_recordingToolBarRecordingBtn.Click += new System.EventHandler(this.m_recordingToolBarRecordingBtn_Click);
            // 
            // m_recordingToolBarPrePhraseBtn
            // 
            this.m_recordingToolBarPrePhraseBtn.AutoSize = false;
            this.m_recordingToolBarPrePhraseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarPrePhraseBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_recordingToolBarPrePhraseBtn.Image")));
            this.m_recordingToolBarPrePhraseBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_recordingToolBarPrePhraseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_recordingToolBarPrePhraseBtn.Margin = new System.Windows.Forms.Padding(4, 1, -8, 2);
            this.m_recordingToolBarPrePhraseBtn.Name = "m_recordingToolBarPrePhraseBtn";
            this.m_recordingToolBarPrePhraseBtn.Size = new System.Drawing.Size(34, 32);
            this.m_recordingToolBarPrePhraseBtn.Text = "Goto Previous Phrase";
            this.m_recordingToolBarPrePhraseBtn.Click += new System.EventHandler(this.m_recordingToolBarPrePhraseBtn_Click);
            // 
            // m_recordingGoToNextPhraseBtn
            // 
            this.m_recordingGoToNextPhraseBtn.AutoSize = false;
            this.m_recordingGoToNextPhraseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingGoToNextPhraseBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_recordingGoToNextPhraseBtn.Image")));
            this.m_recordingGoToNextPhraseBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_recordingGoToNextPhraseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_recordingGoToNextPhraseBtn.Margin = new System.Windows.Forms.Padding(16, 1, -10, 2);
            this.m_recordingGoToNextPhraseBtn.Name = "m_recordingGoToNextPhraseBtn";
            this.m_recordingGoToNextPhraseBtn.Size = new System.Drawing.Size(34, 32);
            this.m_recordingGoToNextPhraseBtn.Text = "Goto Next Phrase";
            this.m_recordingGoToNextPhraseBtn.Click += new System.EventHandler(this.m_recordingGoToNextPhraseBtn_Click);
            // 
            // m_recordingToolBarNextPageBtn
            // 
            this.m_recordingToolBarNextPageBtn.AutoSize = false;
            this.m_recordingToolBarNextPageBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarNextPageBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_recordingToolBarNextPageBtn.Image")));
            this.m_recordingToolBarNextPageBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_recordingToolBarNextPageBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_recordingToolBarNextPageBtn.Margin = new System.Windows.Forms.Padding(18, 1, -14, 2);
            this.m_recordingToolBarNextPageBtn.Name = "m_recordingToolBarNextPageBtn";
            this.m_recordingToolBarNextPageBtn.Size = new System.Drawing.Size(34, 36);
            this.m_recordingToolBarNextPageBtn.Text = "Goto Next Page";
            this.m_recordingToolBarNextPageBtn.Click += new System.EventHandler(this.m_recordingToolBarNextPageBtn_Click);
            // 
            // m_recordingToolBarNextSectionBtn
            // 
            this.m_recordingToolBarNextSectionBtn.AutoSize = false;
            this.m_recordingToolBarNextSectionBtn.AutoToolTip = false;
            this.m_recordingToolBarNextSectionBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarNextSectionBtn.Image = ((System.Drawing.Image)(resources.GetObject("m_recordingToolBarNextSectionBtn.Image")));
            this.m_recordingToolBarNextSectionBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_recordingToolBarNextSectionBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_recordingToolBarNextSectionBtn.Margin = new System.Windows.Forms.Padding(20, 1, -2, 0);
            this.m_recordingToolBarNextSectionBtn.Name = "m_recordingToolBarNextSectionBtn";
            this.m_recordingToolBarNextSectionBtn.Size = new System.Drawing.Size(32, 32);
            this.m_recordingToolBarNextSectionBtn.Text = "Goto Next Section";
            this.m_recordingToolBarNextSectionBtn.Click += new System.EventHandler(this.m_recordingToolBarNextSectionBtn_Click);
            // 
            // m_statusStrip
            // 
            this.m_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recordingToolStripStatusBar,
            this.m_StatusLabel});
            this.m_statusStrip.Location = new System.Drawing.Point(0, 51);
            this.m_statusStrip.Name = "m_statusStrip";
            this.m_statusStrip.Size = new System.Drawing.Size(284, 22);
            this.m_statusStrip.TabIndex = 1;
            this.m_statusStrip.Text = "statusStrip1";
            // 
            // recordingToolStripStatusBar
            // 
            this.recordingToolStripStatusBar.Name = "recordingToolStripStatusBar";
            this.recordingToolStripStatusBar.Size = new System.Drawing.Size(0, 17);
            // 
            // m_StatusLabel
            // 
            this.m_StatusLabel.Name = "m_StatusLabel";
            this.m_StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // RecordingToolBarForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 73);
            this.Controls.Add(this.m_statusStrip);
            this.Controls.Add(this.recordingToolBarToolStrip);
            this.Name = "RecordingToolBarForm";
            this.Text = "Obi recorder bar";
            this.recordingToolBarToolStrip.ResumeLayout(false);
            this.recordingToolBarToolStrip.PerformLayout();
            this.m_statusStrip.ResumeLayout(false);
            this.m_statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip recordingToolBarToolStrip;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarPrePhraseBtn;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarNextPageBtn;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarNextSectionBtn;
        private System.Windows.Forms.StatusStrip m_statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel recordingToolStripStatusBar;
        private System.Windows.Forms.ToolStripButton m_recordingGoToNextPhraseBtn;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarPlayBtn;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarRecordingBtn;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarStopBtn;
        private System.Windows.Forms.ToolStripStatusLabel m_StatusLabel;
    }
}