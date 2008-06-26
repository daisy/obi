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
            this.trackLayout = new Bobi.View.TrackLayout();
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
            // trackLayout
            // 
            this.trackLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.trackLayout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackLayout.Location = new System.Drawing.Point(3, 19);
            this.trackLayout.Name = "trackLayout";
            this.trackLayout.Size = new System.Drawing.Size(0, 0);
            this.trackLayout.TabIndex = 1;
            this.trackLayout.WrapContents = false;
            this.trackLayout.MouseClick += new System.Windows.Forms.MouseEventHandler(this.layoutPanel_MouseClick);
            // 
            // Track
            // 
            this.AutoScrollMargin = new System.Drawing.Size(3, 3);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.trackLayout);
            this.Controls.Add(this.label);
            this.Name = "Track";
            this.Size = new System.Drawing.Size(49, 22);
            this.Click += new System.EventHandler(this.Track_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private TrackLayout trackLayout;
        //private System.Windows.Forms.FlowLayoutPanel layoutPanel;
    }
}
