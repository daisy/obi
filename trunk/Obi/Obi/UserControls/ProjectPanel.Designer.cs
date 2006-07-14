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
            this.label1 = new System.Windows.Forms.Label();
            this.mSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tocPanel = new Obi.UserControls.TOCPanel();
            this.stripManagerPanel1 = new Obi.UserControls.StripManagerPanel();
            this.mSplitContainer.Panel1.SuspendLayout();
            this.mSplitContainer.Panel2.SuspendLayout();
            this.mSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(285, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "(No project)";
            // 
            // mSplitContainer
            // 
            this.mSplitContainer.BackColor = System.Drawing.Color.Transparent;
            this.mSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mSplitContainer.Name = "mSplitContainer";
            // 
            // mSplitContainer.Panel1
            // 
            this.mSplitContainer.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.mSplitContainer.Panel1.Controls.Add(this.tocPanel);
            // 
            // mSplitContainer.Panel2
            // 
            this.mSplitContainer.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.mSplitContainer.Panel2.Controls.Add(this.stripManagerPanel1);
            this.mSplitContainer.Size = new System.Drawing.Size(631, 330);
            this.mSplitContainer.SplitterDistance = 210;
            this.mSplitContainer.TabIndex = 1;
            // 
            // tocPanel
            // 
            this.tocPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tocPanel.Location = new System.Drawing.Point(0, 0);
            this.tocPanel.Name = "tocPanel";
            this.tocPanel.Size = new System.Drawing.Size(210, 330);
            this.tocPanel.TabIndex = 0;
            // 
            // stripManagerPanel1
            // 
            this.stripManagerPanel1.BackColor = System.Drawing.Color.Transparent;
            this.stripManagerPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stripManagerPanel1.Location = new System.Drawing.Point(0, 0);
            this.stripManagerPanel1.Name = "stripManagerPanel1";
            this.stripManagerPanel1.Size = new System.Drawing.Size(417, 330);
            this.stripManagerPanel1.TabIndex = 0;
            // 
            // ProjectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mSplitContainer);
            this.Controls.Add(this.label1);
            this.Name = "ProjectPanel";
            this.Size = new System.Drawing.Size(631, 330);
            this.mSplitContainer.Panel1.ResumeLayout(false);
            this.mSplitContainer.Panel2.ResumeLayout(false);
            this.mSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer mSplitContainer;
        private StripManagerPanel stripManagerPanel1;
        private TOCPanel tocPanel;

    }
}
