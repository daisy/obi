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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordingToolBarForm));
            this.recordingToolBarToolStrip = new System.Windows.Forms.ToolStrip();
            this.m_recordingToolBarElapseBackBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarPlayBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarStopBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarRecordingBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarPrePhraseBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingGoToNextPhraseBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarNextPageBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarNextSectionBtn = new System.Windows.Forms.ToolStripButton();
            this.m_recordingToolBarSectionEndBtn = new System.Windows.Forms.ToolStripButton();
            this.m_TODOBtn = new System.Windows.Forms.ToolStripButton();
            this.m_statusStrip = new System.Windows.Forms.StatusStrip();
            this.recordingToolStripStatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.m_Enlarge = new System.Windows.Forms.Button();
            this.m_Reduce = new System.Windows.Forms.Button();
            this.m_cbMonitorContinuously = new System.Windows.Forms.CheckBox();
            this.recordingToolBarToolStrip.SuspendLayout();
            this.m_statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // recordingToolBarToolStrip
            // 
            resources.ApplyResources(this.recordingToolBarToolStrip, "recordingToolBarToolStrip");
            this.recordingToolBarToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_recordingToolBarElapseBackBtn,
            this.m_recordingToolBarPlayBtn,
            this.m_recordingToolBarStopBtn,
            this.m_recordingToolBarRecordingBtn,
            this.m_recordingToolBarPrePhraseBtn,
            this.m_recordingGoToNextPhraseBtn,
            this.m_recordingToolBarNextPageBtn,
            this.m_recordingToolBarNextSectionBtn,
            this.m_recordingToolBarSectionEndBtn,
            this.m_TODOBtn});
            this.recordingToolBarToolStrip.Name = "recordingToolBarToolStrip";
            this.recordingToolBarToolStrip.TabStop = true;
            // 
            // m_recordingToolBarElapseBackBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarElapseBackBtn, "m_recordingToolBarElapseBackBtn");
            this.m_recordingToolBarElapseBackBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarElapseBackBtn.Name = "m_recordingToolBarElapseBackBtn";
            this.m_recordingToolBarElapseBackBtn.Click += new System.EventHandler(this.m_recordingToolBarElapseBackBtn_Click);
            // 
            // m_recordingToolBarPlayBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarPlayBtn, "m_recordingToolBarPlayBtn");
            this.m_recordingToolBarPlayBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarPlayBtn.Name = "m_recordingToolBarPlayBtn";
            this.m_recordingToolBarPlayBtn.Click += new System.EventHandler(this.m_recordingToolBarPlayBtn_Click);
            // 
            // m_recordingToolBarStopBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarStopBtn, "m_recordingToolBarStopBtn");
            this.m_recordingToolBarStopBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarStopBtn.Name = "m_recordingToolBarStopBtn";
            this.m_recordingToolBarStopBtn.Click += new System.EventHandler(this.m_recordingToolBarStopBtn_Click);
            // 
            // m_recordingToolBarRecordingBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarRecordingBtn, "m_recordingToolBarRecordingBtn");
            this.m_recordingToolBarRecordingBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarRecordingBtn.Name = "m_recordingToolBarRecordingBtn";
            this.m_recordingToolBarRecordingBtn.Click += new System.EventHandler(this.m_recordingToolBarRecordingBtn_Click);
            // 
            // m_recordingToolBarPrePhraseBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarPrePhraseBtn, "m_recordingToolBarPrePhraseBtn");
            this.m_recordingToolBarPrePhraseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarPrePhraseBtn.Margin = new System.Windows.Forms.Padding(4, 1, -8, 2);
            this.m_recordingToolBarPrePhraseBtn.Name = "m_recordingToolBarPrePhraseBtn";
            this.m_recordingToolBarPrePhraseBtn.Click += new System.EventHandler(this.m_recordingToolBarPrePhraseBtn_Click);
            // 
            // m_recordingGoToNextPhraseBtn
            // 
            resources.ApplyResources(this.m_recordingGoToNextPhraseBtn, "m_recordingGoToNextPhraseBtn");
            this.m_recordingGoToNextPhraseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingGoToNextPhraseBtn.Margin = new System.Windows.Forms.Padding(16, 1, -10, 2);
            this.m_recordingGoToNextPhraseBtn.Name = "m_recordingGoToNextPhraseBtn";
            this.m_recordingGoToNextPhraseBtn.Click += new System.EventHandler(this.m_recordingGoToNextPhraseBtn_Click);
            // 
            // m_recordingToolBarNextPageBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarNextPageBtn, "m_recordingToolBarNextPageBtn");
            this.m_recordingToolBarNextPageBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarNextPageBtn.Margin = new System.Windows.Forms.Padding(18, 1, -14, 2);
            this.m_recordingToolBarNextPageBtn.Name = "m_recordingToolBarNextPageBtn";
            this.m_recordingToolBarNextPageBtn.Click += new System.EventHandler(this.m_recordingToolBarNextPageBtn_Click);
            // 
            // m_recordingToolBarNextSectionBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarNextSectionBtn, "m_recordingToolBarNextSectionBtn");
            this.m_recordingToolBarNextSectionBtn.AutoToolTip = false;
            this.m_recordingToolBarNextSectionBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarNextSectionBtn.Margin = new System.Windows.Forms.Padding(20, 1, 6, 0);
            this.m_recordingToolBarNextSectionBtn.Name = "m_recordingToolBarNextSectionBtn";
            this.m_recordingToolBarNextSectionBtn.Click += new System.EventHandler(this.m_recordingToolBarNextSectionBtn_Click);
            // 
            // m_recordingToolBarSectionEndBtn
            // 
            resources.ApplyResources(this.m_recordingToolBarSectionEndBtn, "m_recordingToolBarSectionEndBtn");
            this.m_recordingToolBarSectionEndBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_recordingToolBarSectionEndBtn.Name = "m_recordingToolBarSectionEndBtn";
            this.m_recordingToolBarSectionEndBtn.Click += new System.EventHandler(this.m_recordingToolBarSectionEndBtn_Click);
            // 
            // m_TODOBtn
            // 
            resources.ApplyResources(this.m_TODOBtn, "m_TODOBtn");
            this.m_TODOBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_TODOBtn.Name = "m_TODOBtn";
            this.m_TODOBtn.Click += new System.EventHandler(this.m_TODOBtn_Click);
            // 
            // m_statusStrip
            // 
            this.m_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recordingToolStripStatusBar,
            this.m_StatusLabel});
            resources.ApplyResources(this.m_statusStrip, "m_statusStrip");
            this.m_statusStrip.Name = "m_statusStrip";
            // 
            // recordingToolStripStatusBar
            // 
            this.recordingToolStripStatusBar.Name = "recordingToolStripStatusBar";
            resources.ApplyResources(this.recordingToolStripStatusBar, "recordingToolStripStatusBar");
            // 
            // m_StatusLabel
            // 
            this.m_StatusLabel.Name = "m_StatusLabel";
            resources.ApplyResources(this.m_StatusLabel, "m_StatusLabel");
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // m_Enlarge
            // 
            resources.ApplyResources(this.m_Enlarge, "m_Enlarge");
            this.m_Enlarge.Name = "m_Enlarge";
            this.m_Enlarge.UseVisualStyleBackColor = true;
            this.m_Enlarge.Click += new System.EventHandler(this.m_Enlarge_Click);
            // 
            // m_Reduce
            // 
            resources.ApplyResources(this.m_Reduce, "m_Reduce");
            this.m_Reduce.Name = "m_Reduce";
            this.m_Reduce.UseVisualStyleBackColor = true;
            this.m_Reduce.Click += new System.EventHandler(this.m_Reduce_Click);
            // 
            // m_cbMonitorContinuously
            // 
            resources.ApplyResources(this.m_cbMonitorContinuously, "m_cbMonitorContinuously");
            this.m_cbMonitorContinuously.Name = "m_cbMonitorContinuously";
            this.m_cbMonitorContinuously.UseVisualStyleBackColor = true;
            // 
            // RecordingToolBarForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.m_cbMonitorContinuously);
            this.Controls.Add(this.m_Reduce);
            this.Controls.Add(this.m_Enlarge);
            this.Controls.Add(this.m_statusStrip);
            this.Controls.Add(this.recordingToolBarToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "RecordingToolBarForm";
            this.Load += new System.EventHandler(this.RecordingToolBarForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecordingToolBarForm_FormClosing);
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
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripButton m_TODOBtn;
        private System.Windows.Forms.Button m_Enlarge;
        private System.Windows.Forms.Button m_Reduce;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarElapseBackBtn;
        private System.Windows.Forms.ToolStripButton m_recordingToolBarSectionEndBtn;
        private System.Windows.Forms.CheckBox m_cbMonitorContinuously;
    }
}