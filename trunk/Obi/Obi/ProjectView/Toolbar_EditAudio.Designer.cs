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
            this.mbtnSplittoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnMergetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnCuttoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnCopytoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnPastetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnDeletetoolStrip = new System.Windows.Forms.ToolStripButton();
            this.mbtnPraseDetectiontoolStrip = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mbtnSplittoolStrip,
            this.mbtnMergetoolStrip,
            this.mbtnCuttoolStrip,
            this.mbtnCopytoolStrip,
            this.mbtnPastetoolStrip,
            this.mbtnDeletetoolStrip,
            this.mbtnPraseDetectiontoolStrip});
            this.toolStrip1.MaximumSize = new System.Drawing.Size(204, 25);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(574, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.TabStop = true;
            this.toolStrip1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStrip1_MouseUp);
            this.toolStrip1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStrip1_MouseDown);
            this.toolStrip1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStrip1_MouseMove);
            this.toolStrip1.MouseHover += new System.EventHandler(this.toolStrip1_MouseHover);
            // 
            // mbtnSplittoolStrip
            // 
            resources.ApplyResources(this.mbtnSplittoolStrip, "mbtnSplittoolStrip");
            this.mbtnSplittoolStrip.Name = "mbtnSplittoolStrip";
            this.mbtnSplittoolStrip.Click += new System.EventHandler(this.mbtnSplittoolStrip_Click);
            // 
            // mbtnMergetoolStrip
            // 
            resources.ApplyResources(this.mbtnMergetoolStrip, "mbtnMergetoolStrip");
            this.mbtnMergetoolStrip.Name = "mbtnMergetoolStrip";
            this.mbtnMergetoolStrip.Click += new System.EventHandler(this.mbtnMergetoolStrip_Click);
            // 
            // mbtnCuttoolStrip
            // 
            resources.ApplyResources(this.mbtnCuttoolStrip, "mbtnCuttoolStrip");
            this.mbtnCuttoolStrip.Name = "mbtnCuttoolStrip";
            this.mbtnCuttoolStrip.Click += new System.EventHandler(this.mbtnCuttoolStrip_Click);
            // 
            // mbtnCopytoolStrip
            // 
            resources.ApplyResources(this.mbtnCopytoolStrip, "mbtnCopytoolStrip");
            this.mbtnCopytoolStrip.Name = "mbtnCopytoolStrip";
            this.mbtnCopytoolStrip.Click += new System.EventHandler(this.mbtnCopytoolStrip_Click);
            // 
            // mbtnPastetoolStrip
            // 
            resources.ApplyResources(this.mbtnPastetoolStrip, "mbtnPastetoolStrip");
            this.mbtnPastetoolStrip.Name = "mbtnPastetoolStrip";
            this.mbtnPastetoolStrip.Click += new System.EventHandler(this.mbtnPastetoolStrip_Click);
            // 
            // mbtnDeletetoolStrip
            // 
            resources.ApplyResources(this.mbtnDeletetoolStrip, "mbtnDeletetoolStrip");
            this.mbtnDeletetoolStrip.Name = "mbtnDeletetoolStrip";
            this.mbtnDeletetoolStrip.Click += new System.EventHandler(this.mbtnDeletetoolStrip_Click);
            // 
            // mbtnPraseDetectiontoolStrip
            // 
            resources.ApplyResources(this.mbtnPraseDetectiontoolStrip, "mbtnPraseDetectiontoolStrip");
            this.mbtnPraseDetectiontoolStrip.Name = "mbtnPraseDetectiontoolStrip";
            this.mbtnPraseDetectiontoolStrip.Click += new System.EventHandler(this.mbtnPraseDetectiontoolStrip_Click);
            // 
            // Toolbar_EditAudio
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(583, 28);
            this.Name = "Toolbar_EditAudio";
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
