namespace Obi.ProjectView
{
    partial class ZoomWaveform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZoomWaveform));
            this.panelZooomWaveform = new System.Windows.Forms.Panel();
            this.txtZoomSelected = new System.Windows.Forms.TextBox();
            this.btntxtZoomSelected = new System.Windows.Forms.Button();
            this.mtoolTipZoomWaveform = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripZoomPanel = new System.Windows.Forms.ToolStrip();
            this.btnClosetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.btnNextPhrasetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.btnPreviousPhrasetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIntoolStrip = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOuttoolStrip = new System.Windows.Forms.ToolStripButton();
            this.btnResettoolStrip = new System.Windows.Forms.ToolStripButton();
            this.btnZoomSelectiontoolStrip = new System.Windows.Forms.ToolStripButton();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.toolStripZoomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelZooomWaveform
            // 
            resources.ApplyResources(this.panelZooomWaveform, "panelZooomWaveform");
            this.panelZooomWaveform.BackColor = System.Drawing.SystemColors.Control;
            this.panelZooomWaveform.Name = "panelZooomWaveform";
            this.panelZooomWaveform.TabStop = true;
            // 
            // txtZoomSelected
            // 
            resources.ApplyResources(this.txtZoomSelected, "txtZoomSelected");
            this.txtZoomSelected.Name = "txtZoomSelected";
            this.txtZoomSelected.ReadOnly = true;
            // 
            // btntxtZoomSelected
            // 
            this.btntxtZoomSelected.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btntxtZoomSelected.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.btntxtZoomSelected, "btntxtZoomSelected");
            this.btntxtZoomSelected.Name = "btntxtZoomSelected";
            this.btntxtZoomSelected.UseVisualStyleBackColor = false;
            // 
            // toolStripZoomPanel
            // 
            resources.ApplyResources(this.toolStripZoomPanel, "toolStripZoomPanel");
            this.toolStripZoomPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClosetoolStrip,
            this.btnNextPhrasetoolStrip,
            this.btnPreviousPhrasetoolStrip,
            this.btnZoomIntoolStrip,
            this.btnZoomOuttoolStrip,
            this.btnResettoolStrip,
            this.btnZoomSelectiontoolStrip});
            this.toolStripZoomPanel.MinimumSize = new System.Drawing.Size(617, 28);
            this.toolStripZoomPanel.Name = "toolStripZoomPanel";
            this.toolStripZoomPanel.TabStop = true;
            // 
            // btnClosetoolStrip
            // 
            resources.ApplyResources(this.btnClosetoolStrip, "btnClosetoolStrip");
            this.btnClosetoolStrip.Name = "btnClosetoolStrip";
            this.btnClosetoolStrip.Click += new System.EventHandler(this.btnClosetoolStrip_Click);
            // 
            // btnNextPhrasetoolStrip
            // 
            resources.ApplyResources(this.btnNextPhrasetoolStrip, "btnNextPhrasetoolStrip");
            this.btnNextPhrasetoolStrip.Name = "btnNextPhrasetoolStrip";
            this.btnNextPhrasetoolStrip.Click += new System.EventHandler(this.btnNextPhrasetoolStrip_Click);
            // 
            // btnPreviousPhrasetoolStrip
            // 
            this.btnPreviousPhrasetoolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            resources.ApplyResources(this.btnPreviousPhrasetoolStrip, "btnPreviousPhrasetoolStrip");
            this.btnPreviousPhrasetoolStrip.Name = "btnPreviousPhrasetoolStrip";
            this.btnPreviousPhrasetoolStrip.Click += new System.EventHandler(this.btnPreviousPhrasetoolStrip_Click);
            // 
            // btnZoomIntoolStrip
            // 
            resources.ApplyResources(this.btnZoomIntoolStrip, "btnZoomIntoolStrip");
            this.btnZoomIntoolStrip.Name = "btnZoomIntoolStrip";
            this.btnZoomIntoolStrip.Click += new System.EventHandler(this.btnZoomIntoolStrip_Click);
            // 
            // btnZoomOuttoolStrip
            // 
            resources.ApplyResources(this.btnZoomOuttoolStrip, "btnZoomOuttoolStrip");
            this.btnZoomOuttoolStrip.Name = "btnZoomOuttoolStrip";
            this.btnZoomOuttoolStrip.Click += new System.EventHandler(this.btnZoomOuttoolStrip_Click);
            // 
            // btnResettoolStrip
            // 
            resources.ApplyResources(this.btnResettoolStrip, "btnResettoolStrip");
            this.btnResettoolStrip.Name = "btnResettoolStrip";
            this.btnResettoolStrip.Click += new System.EventHandler(this.btnResettoolStrip_Click);
            // 
            // btnZoomSelectiontoolStrip
            // 
            resources.ApplyResources(this.btnZoomSelectiontoolStrip, "btnZoomSelectiontoolStrip");
            this.btnZoomSelectiontoolStrip.Name = "btnZoomSelectiontoolStrip";
            this.btnZoomSelectiontoolStrip.Click += new System.EventHandler(this.mbtnZoomSelectiontoolStrip_Click);
            // 
            // ZoomWaveform
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.toolStripZoomPanel);
            this.Controls.Add(this.btntxtZoomSelected);
            this.Controls.Add(this.txtZoomSelected);
            this.Controls.Add(this.panelZooomWaveform);
            this.Name = "ZoomWaveform";
            this.toolStripZoomPanel.ResumeLayout(false);
            this.toolStripZoomPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelZooomWaveform;
        private System.Windows.Forms.TextBox txtZoomSelected;
        private System.Windows.Forms.Button btntxtZoomSelected;
        private System.Windows.Forms.ToolTip mtoolTipZoomWaveform;
        private System.Windows.Forms.ToolStrip toolStripZoomPanel;
        private System.Windows.Forms.ToolStripButton btnClosetoolStrip;
        private System.Windows.Forms.ToolStripButton btnNextPhrasetoolStrip;
        private System.Windows.Forms.ToolStripButton btnPreviousPhrasetoolStrip;
        private System.Windows.Forms.ToolStripButton btnZoomIntoolStrip;
        private System.Windows.Forms.ToolStripButton btnZoomOuttoolStrip;
        private System.Windows.Forms.ToolStripButton btnResettoolStrip;
        private System.Windows.Forms.ToolStripButton btnZoomSelectiontoolStrip;
        private System.Windows.Forms.HelpProvider helpProvider1;

    }
}
