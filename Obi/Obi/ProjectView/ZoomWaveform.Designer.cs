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
            this.toolStripZoomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelZooomWaveform
            // 
            this.panelZooomWaveform.AutoScroll = true;
            this.panelZooomWaveform.BackColor = System.Drawing.SystemColors.Control;
            this.panelZooomWaveform.Location = new System.Drawing.Point(3, 3);
            this.panelZooomWaveform.Name = "panelZooomWaveform";
            this.panelZooomWaveform.Size = new System.Drawing.Size(946, 488);
            this.panelZooomWaveform.TabIndex = 1;
            this.panelZooomWaveform.TabStop = true;
            // 
            // txtZoomSelected
            // 
            this.txtZoomSelected.Location = new System.Drawing.Point(19, 507);
            this.txtZoomSelected.Name = "txtZoomSelected";
            this.txtZoomSelected.ReadOnly = true;
            this.txtZoomSelected.Size = new System.Drawing.Size(702, 20);
            this.txtZoomSelected.TabIndex = 18;
            this.txtZoomSelected.Visible = false;
            // 
            // btntxtZoomSelected
            // 
            this.btntxtZoomSelected.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btntxtZoomSelected.BackColor = System.Drawing.SystemColors.Control;
            this.btntxtZoomSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btntxtZoomSelected.Location = new System.Drawing.Point(19, 507);
            this.btntxtZoomSelected.Name = "btntxtZoomSelected";
            this.btntxtZoomSelected.Size = new System.Drawing.Size(702, 23);
            this.btntxtZoomSelected.TabIndex = 2;
            this.btntxtZoomSelected.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btntxtZoomSelected.UseVisualStyleBackColor = false;
            // 
            // toolStripZoomPanel
            // 
            this.toolStripZoomPanel.AccessibleName = "Navigation & Zoom tool bar";
            this.toolStripZoomPanel.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripZoomPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClosetoolStrip,
            this.btnNextPhrasetoolStrip,
            this.btnPreviousPhrasetoolStrip,
            this.btnZoomIntoolStrip,
            this.btnZoomOuttoolStrip,
            this.btnResettoolStrip,
            this.btnZoomSelectiontoolStrip});
            this.toolStripZoomPanel.Location = new System.Drawing.Point(0, 0);
            this.toolStripZoomPanel.MinimumSize = new System.Drawing.Size(617, 28);
            this.toolStripZoomPanel.Name = "toolStripZoomPanel";
            this.toolStripZoomPanel.Size = new System.Drawing.Size(617, 28);
            this.toolStripZoomPanel.TabIndex = 3;
            this.toolStripZoomPanel.TabStop = true;
            this.toolStripZoomPanel.Text = "toolStrip1";
            // 
            // btnClosetoolStrip
            // 
            this.btnClosetoolStrip.AccessibleName = "";
            this.btnClosetoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnClosetoolStrip.Image")));
            this.btnClosetoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClosetoolStrip.Name = "btnClosetoolStrip";
            this.btnClosetoolStrip.Size = new System.Drawing.Size(53, 25);
            this.btnClosetoolStrip.Text = "Close";
            this.btnClosetoolStrip.Click += new System.EventHandler(this.btnClosetoolStrip_Click);
            // 
            // btnNextPhrasetoolStrip
            // 
            this.btnNextPhrasetoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPhrasetoolStrip.Image")));
            this.btnNextPhrasetoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextPhrasetoolStrip.Name = "btnNextPhrasetoolStrip";
            this.btnNextPhrasetoolStrip.Size = new System.Drawing.Size(85, 25);
            this.btnNextPhrasetoolStrip.Text = "Next Phrase";
            this.btnNextPhrasetoolStrip.Click += new System.EventHandler(this.btnNextPhrasetoolStrip_Click);
            // 
            // btnPreviousPhrasetoolStrip
            // 
            this.btnPreviousPhrasetoolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnPreviousPhrasetoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousPhrasetoolStrip.Image")));
            this.btnPreviousPhrasetoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousPhrasetoolStrip.Name = "btnPreviousPhrasetoolStrip";
            this.btnPreviousPhrasetoolStrip.Size = new System.Drawing.Size(104, 25);
            this.btnPreviousPhrasetoolStrip.Text = "Previous Phrase";
            this.btnPreviousPhrasetoolStrip.Click += new System.EventHandler(this.btnPreviousPhrasetoolStrip_Click);
            // 
            // btnZoomIntoolStrip
            // 
            this.btnZoomIntoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomIntoolStrip.Image")));
            this.btnZoomIntoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIntoolStrip.Name = "btnZoomIntoolStrip";
            this.btnZoomIntoolStrip.Size = new System.Drawing.Size(66, 25);
            this.btnZoomIntoolStrip.Text = "Zoom In";
            this.btnZoomIntoolStrip.Click += new System.EventHandler(this.btnZoomIntoolStrip_Click);
            // 
            // btnZoomOuttoolStrip
            // 
            this.btnZoomOuttoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOuttoolStrip.Image")));
            this.btnZoomOuttoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOuttoolStrip.Name = "btnZoomOuttoolStrip";
            this.btnZoomOuttoolStrip.Size = new System.Drawing.Size(74, 25);
            this.btnZoomOuttoolStrip.Text = "Zoom Out";
            this.btnZoomOuttoolStrip.Click += new System.EventHandler(this.btnZoomOuttoolStrip_Click);
            // 
            // btnResettoolStrip
            // 
            this.btnResettoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnResettoolStrip.Image")));
            this.btnResettoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnResettoolStrip.Name = "btnResettoolStrip";
            this.btnResettoolStrip.Size = new System.Drawing.Size(55, 25);
            this.btnResettoolStrip.Text = "Reset";
            this.btnResettoolStrip.Click += new System.EventHandler(this.btnResettoolStrip_Click);
            // 
            // btnZoomSelectiontoolStrip
            // 
            this.btnZoomSelectiontoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomSelectiontoolStrip.Image")));
            this.btnZoomSelectiontoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomSelectiontoolStrip.Name = "btnZoomSelectiontoolStrip";
            this.btnZoomSelectiontoolStrip.Size = new System.Drawing.Size(101, 25);
            this.btnZoomSelectiontoolStrip.Text = "Zoom Selection";
            this.btnZoomSelectiontoolStrip.Click += new System.EventHandler(this.mbtnZoomSelectiontoolStrip_Click);
            // 
            // ZoomWaveform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.toolStripZoomPanel);
            this.Controls.Add(this.btntxtZoomSelected);
            this.Controls.Add(this.txtZoomSelected);
            this.Controls.Add(this.panelZooomWaveform);
            this.Name = "ZoomWaveform";
            this.Size = new System.Drawing.Size(963, 559);
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

    }
}
