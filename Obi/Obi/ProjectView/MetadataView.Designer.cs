namespace Obi.ProjectView
{
    partial class MetadataView
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
            this.mLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // mLayout
            // 
            this.mLayout.AutoScroll = true;
            this.mLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mLayout.Location = new System.Drawing.Point(0, 0);
            this.mLayout.Margin = new System.Windows.Forms.Padding(0);
            this.mLayout.Name = "mLayout";
            this.mLayout.Size = new System.Drawing.Size(150, 150);
            this.mLayout.TabIndex = 0;
            this.mLayout.WrapContents = false;
            this.mLayout.SizeChanged += new System.EventHandler(this.mLayout_SizeChanged);
            // 
            // MetadataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mLayout);
            this.Name = "MetadataView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mLayout;

    }
}
