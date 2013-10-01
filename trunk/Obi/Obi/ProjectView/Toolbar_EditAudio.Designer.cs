namespace Obi.ProjectView
{
    partial class Toolbar_EditAudio
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Toolbar_EditAudio));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.mbtnCuttoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnDeletetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnSplittoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnPastetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnCopytoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnPraseDetectiontoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnMergetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AccessibleName = "Edit audio";
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbtnSplittoolStrip,
            this.mbtnMergetoolStrip,
            this.mbtnCuttoolStrip,
            this.mbtnCopytoolStrip,
            this.mbtnPastetoolStrip,
            this.mbtnDeletetoolStrip,
            this.mbtnPraseDetectiontoolStrip});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.MaximumSize = new System.Drawing.Size(204, 25);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(574, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(574, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.TabStop = true;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStrip1_MouseUp);
            this.toolStrip1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStrip1_MouseDown);
            this.toolStrip1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStrip1_MouseMove);
            this.toolStrip1.MouseHover += new System.EventHandler(this.toolStrip1_MouseHover);
            // 
            // mbtnCuttoolStrip
            // 
            this.mbtnCuttoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("mbtnCuttoolStrip.Image")));
            this.mbtnCuttoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbtnCuttoolStrip.Name = "mbtnCuttoolStrip";
            this.mbtnCuttoolStrip.Size = new System.Drawing.Size(46, 22);
            this.mbtnCuttoolStrip.Text = "Cut";
            this.mbtnCuttoolStrip.Click += new System.EventHandler(this.mbtnCuttoolStrip_Click);
            // 
            // mbtnDeletetoolStrip
            // 
            this.mbtnDeletetoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("mbtnDeletetoolStrip.Image")));
            this.mbtnDeletetoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbtnDeletetoolStrip.Name = "mbtnDeletetoolStrip";
            this.mbtnDeletetoolStrip.Size = new System.Drawing.Size(60, 22);
            this.mbtnDeletetoolStrip.Text = "Delete";
            this.mbtnDeletetoolStrip.Click += new System.EventHandler(this.mbtnDeletetoolStrip_Click);
            // 
            // mbtnSplittoolStrip
            // 
            this.mbtnSplittoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("mbtnSplittoolStrip.Image")));
            this.mbtnSplittoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbtnSplittoolStrip.Name = "mbtnSplittoolStrip";
            this.mbtnSplittoolStrip.Size = new System.Drawing.Size(50, 22);
            this.mbtnSplittoolStrip.Text = "Split";
            this.mbtnSplittoolStrip.Click += new System.EventHandler(this.mbtnSplittoolStrip_Click);
            // 
            // mbtnPastetoolStrip
            // 
            this.mbtnPastetoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("mbtnPastetoolStrip.Image")));
            this.mbtnPastetoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbtnPastetoolStrip.Name = "mbtnPastetoolStrip";
            this.mbtnPastetoolStrip.Size = new System.Drawing.Size(55, 22);
            this.mbtnPastetoolStrip.Text = "Paste";
            this.mbtnPastetoolStrip.Click += new System.EventHandler(this.mbtnPastetoolStrip_Click);
            // 
            // mbtnCopytoolStrip
            // 
            this.mbtnCopytoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("mbtnCopytoolStrip.Image")));
            this.mbtnCopytoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbtnCopytoolStrip.Name = "mbtnCopytoolStrip";
            this.mbtnCopytoolStrip.Size = new System.Drawing.Size(55, 22);
            this.mbtnCopytoolStrip.Text = "Copy";
            this.mbtnCopytoolStrip.Click += new System.EventHandler(this.mbtnCopytoolStrip_Click);
            // 
            // mbtnPraseDetectiontoolStrip
            // 
            this.mbtnPraseDetectiontoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("mbtnPraseDetectiontoolStrip.Image")));
            this.mbtnPraseDetectiontoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbtnPraseDetectiontoolStrip.Name = "mbtnPraseDetectiontoolStrip";
            this.mbtnPraseDetectiontoolStrip.Size = new System.Drawing.Size(116, 22);
            this.mbtnPraseDetectiontoolStrip.Text = "Phrase Detection";
            this.mbtnPraseDetectiontoolStrip.Click += new System.EventHandler(this.mbtnPraseDetectiontoolStrip_Click);
            // 
            // mbtnMergetoolStrip
            // 
            this.mbtnMergetoolStrip.Image = ((System.Drawing.Image)(resources.GetObject("mbtnMergetoolStrip.Image")));
            this.mbtnMergetoolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mbtnMergetoolStrip.Name = "mbtnMergetoolStrip";
            this.mbtnMergetoolStrip.Size = new System.Drawing.Size(154, 22);
            this.mbtnMergetoolStrip.Text = "Merge With Next Phrase";
            this.mbtnMergetoolStrip.Click += new System.EventHandler(this.mbtnMergetoolStrip_Click);
            // 
            // Toolbar_EditAudio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(583, 28);
            this.Name = "Toolbar_EditAudio";
            this.Size = new System.Drawing.Size(583, 28);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton mbtnCuttoolStrip;
        private System.Windows.Forms.ToolStripButton mbtnCopytoolStrip;
        private System.Windows.Forms.ToolStripButton mbtnPastetoolStrip;
        private System.Windows.Forms.ToolStripButton mbtnSplittoolStrip;
        private System.Windows.Forms.ToolStripButton mbtnDeletetoolStrip;
        private System.Windows.Forms.ToolStripButton mbtnMergetoolStrip;
        private System.Windows.Forms.ToolStripButton mbtnPraseDetectiontoolStrip;
    }
}
