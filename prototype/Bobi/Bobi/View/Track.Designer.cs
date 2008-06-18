namespace Bobi.View
{
    partial class Track
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
            this.label = new System.Windows.Forms.Label();
            this.layoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(3, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(43, 16);
            this.label.TabIndex = 0;
            this.label.Text = "Track";
            this.label.Click += new System.EventHandler(this.Track_Click);
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layoutPanel.Location = new System.Drawing.Point(3, 19);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Size = new System.Drawing.Size(0, 0);
            this.layoutPanel.TabIndex = 1;
            this.layoutPanel.WrapContents = false;
            this.layoutPanel.Click += new System.EventHandler(this.Track_Click);
            // 
            // Track
            // 
            this.AutoScrollMargin = new System.Drawing.Size(3, 3);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.layoutPanel);
            this.Controls.Add(this.label);
            this.Name = "Track";
            this.Size = new System.Drawing.Size(49, 22);
            this.Click += new System.EventHandler(this.Track_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.FlowLayoutPanel layoutPanel;
    }
}
