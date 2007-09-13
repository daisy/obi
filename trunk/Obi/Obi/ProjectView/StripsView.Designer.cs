namespace Obi.ProjectView
{
    partial class StripsView
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
            this.mLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // mLayoutPanel
            // 
            this.mLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mLayoutPanel.Name = "mLayoutPanel";
            this.mLayoutPanel.Size = new System.Drawing.Size(150, 150);
            this.mLayoutPanel.TabIndex = 0;
            this.mLayoutPanel.WrapContents = false;
            // 
            // StripsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mLayoutPanel);
            this.Name = "StripsView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mLayoutPanel;
    }
}
